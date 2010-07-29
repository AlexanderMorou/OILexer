using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Oilexer.Parser;
using System.CodeDom.Compiler;
using Oilexer.Parser.Builder;
using System.Globalization;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Inlining;
using Oilexer.Parser.GDFileData.TokenExpression;
using OM = System.Collections.ObjectModel;
using GDFD = Oilexer.Parser.GDFileData;

namespace Oilexer.Parser.Builder
{
    using Oilexer._Internal.Inlining;
    using Oilexer.Parser.GDFileData;
    using Oilexer.Utilities.Collections;
    using GDEntryCollection =
            OM::Collection<GDFD::IEntry>;
    using System.Diagnostics;
    using Oilexer._Internal;
using Oilexer.FiniteAutomata.Rules;
    using Oilexer.FiniteAutomata.Tokens;
    /// <summary>
    /// Provides a series of extension methdos for the parser compiler
    /// to operate upon the data sets provided by the grammar description
    /// language.
    /// </summary>
    internal static class ParserCompilerExtensions
    {
        public static readonly UnicodeCategory[] EmptyCategories = new UnicodeCategory[0];
        public static Dictionary<UnicodeCategory, RegularLanguageSet> unicodeCategoryData = BuildUnicodeCategories();
        public static Dictionary<UnicodeCategory, int> unicodeCategoryTrueCounts;
        public static Dictionary<UnicodeCategory, int> unicodeCategoryMaxPoint;
        private static Dictionary<UnicodeCategory, RegularLanguageSet> BuildUnicodeCategories()
        {
            unicodeCategoryMaxPoint = new Dictionary<UnicodeCategory, int>();
            unicodeCategoryTrueCounts = new Dictionary<UnicodeCategory, int>();
            Dictionary<UnicodeCategory, RegularLanguageSet> result = new Dictionary<UnicodeCategory, RegularLanguageSet>();
            UnicodeCategory[] categoryData = new UnicodeCategory[char.MaxValue + 1];
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                UnicodeCategory currentCategory = categoryData[i] = char.GetUnicodeCategory((char)(i));
                if (!unicodeCategoryTrueCounts.ContainsKey(currentCategory))
                    unicodeCategoryTrueCounts.Add(currentCategory, 0);
                unicodeCategoryTrueCounts[currentCategory]++;
                unicodeCategoryMaxPoint[currentCategory] = i;
            }
            foreach (var currentCategory in unicodeCategoryTrueCounts.Keys)
                if (!result.ContainsKey(currentCategory))
                    result.Add(currentCategory, new RegularLanguageSet(currentCategory, categoryData, unicodeCategoryMaxPoint[currentCategory]));
            return result;
        }

        public static void PropagateUnicodeCategoriesAndPartials(ref RegularLanguageSet target, RegularLanguageSet negativeNoise, out UnicodeCategory[] categories, out Dictionary<UnicodeCategory, RegularLanguageSet> categoriesWithNegativeAssertions)
        {
            List<UnicodeCategory> fullCategories = new List<UnicodeCategory>();
            fullCategories = new List<UnicodeCategory>();
            categoriesWithNegativeAssertions = new Dictionary<UnicodeCategory, RegularLanguageSet>();
            foreach (var category in unicodeCategoryData.Keys)
            {
                var categoryData = unicodeCategoryData[category];
                var intersection = categoryData & target;

                int compared = (int)(intersection.CountTrue());//unicodeCategoryData[category].Compare(target);
                if (compared == unicodeCategoryTrueCounts[category])
                {
                    fullCategories.Add(category);
                    target = target.SymmetricDifference(intersection);
                }
                else if (compared > unicodeCategoryTrueCounts[category] - compared)
                {

                    var categoryCopy = categoryData.SymmetricDifference(intersection);
                    categoryCopy = categoryCopy.SymmetricDifference(categoryCopy.Intersect(negativeNoise));
                    target = target.SymmetricDifference(intersection);

                    if (categoryCopy.IsEmpty)
                        categoriesWithNegativeAssertions.Add(category, null);
                    else
                        categoriesWithNegativeAssertions.Add(category, categoryCopy);
                    //if there are more elements that overlap than there are not,
                    //include this category with a negative assertion.
                }
            }
            categories = fullCategories.ToArray();
        }

        internal static void PropagateUnicodeCategories(ref RegularLanguageSet toStringArray, out UnicodeCategory[] toStringUnicodeCategories)
        {
            PropagateUnicodeCategories(ref toStringArray, true, out toStringUnicodeCategories);
        }
        internal static void PropagateUnicodeCategories(ref RegularLanguageSet toStringArray, bool minimizeSet, out UnicodeCategory[] toStringUnicodeCategories)
        {
            var resultSet = new List<UnicodeCategory>();

            foreach (var unicodeCategory in unicodeCategoryData.Keys)
            {
                var currentUnicodeSubset = unicodeCategoryData[unicodeCategory].Intersect(toStringArray);
                if (currentUnicodeSubset.Equals(unicodeCategoryData[unicodeCategory]))
                {
                    resultSet.Add(unicodeCategory);
                    toStringArray = toStringArray.SymmetricDifference(currentUnicodeSubset);
                }
            }
            toStringArray.Reduce();
            toStringUnicodeCategories = resultSet.ToArray();
        }

        private static bool SetsEqual(this BitArray left, BitArray right)
        {
            if (left.Length != right.Length)
                return false;
            int[] leftSet  = new int[(int)Math.Ceiling(((double)left.Length / 32D))],
                  rightSet = new int[leftSet.Length];
            left.CopyTo(leftSet, 0);
            right.CopyTo(rightSet, 0);
            for (int i = 0; i < leftSet.Length; i++)
                if (leftSet[i] != rightSet[i])
                    return false;
            return true;
        }

        private static int Compare(this BitArray left, BitArray right)
        {
            int compared = 0;
            int minLength = Math.Min(left.Length, right.Length);
            for (int i = 0; i < minLength; i++)
                if (left[i] && right[i])
                    compared++;
            return compared;
        }

        public static int CountTrue(this BitArray target)
        {
            int result = 0;
            for (int i = 0; i < target.Length; i++)
                if (target[i])
                    result++;
            return result;
        }

        public static int Max(this BitArray target)
        {
            for (int i = target.Length - 1; i >= 0; i--)
                if (target[i])
                    return i;
            return -1;
        }
        public static string ToString(this BitArray chars, bool inverted)
        {
            return chars.ToString(inverted, EmptyCategories);
        }
        public static string ToString(this BitArray chars, bool inverted, UnicodeCategory[] categories)
        {
            bool inRange = false;
            BitArray used = new BitArray(chars);

            //if (inverted)
            //    used = used.Not();
            int rangeStart = -1;
            StringBuilder range = new StringBuilder();
            bool hitAny = false;
            for (int i = 0; i < chars.Length; i++)
            {
                if (!inRange)
                {
                    if (used[i])
                    {
                        if (!hitAny)
                        {
                            if (inverted)
                                range.Append("^");
                            hitAny = true;
                        }
                        if (used.Length <= i + 1)
                            inRange = false;
                        else
                            inRange = used[i + 1];
                        if (!inRange)
                            range.Append(GetCharacterString((char)i));
                        else
                            rangeStart = i;
                    }
                }
                else
                    if (!used[i] || i == chars.Length - 1)
                    {
                        inRange = false;
                        range.Append(GetCharacterString((char)rangeStart));
                        range.Append('-');
                        range.Append(GetCharacterString((char)(i - (i == chars.Length - 1 ? 0 : 1))));
                    }
            }
            if (!hitAny)
            {
                if (inverted)
                {
                    if (categories.Length == 0)
                        range.Append("ALL");
                    else
                        range.Insert(0, '^');
                }
                else if (categories.Length == 0)
                    range.Append("None");
            }
            for (int i = 0; i < categories.Length; i++)
            {
                switch (categories[i])
                {
                    case UnicodeCategory.Control:
                        range.Append(":Cc:");
                        break;
                    case UnicodeCategory.Format:
                        range.Append(":Cf:");
                        break;
                    case UnicodeCategory.OtherNotAssigned:
                        range.Append(":Cn:");
                        break;
                    case UnicodeCategory.PrivateUse:
                        range.Append(":Co:");
                        break;
                    case UnicodeCategory.Surrogate:
                        range.Append(":Cs:");
                        break;
                    case UnicodeCategory.LowercaseLetter:
                        range.Append(":Ll:");
                        break;
                    case UnicodeCategory.ModifierLetter:
                        range.Append(":Lm:");
                        break;
                    case UnicodeCategory.OtherLetter:
                        range.Append(":Lo:");
                        break;
                    case UnicodeCategory.TitlecaseLetter:
                        range.Append(":Lt:");
                        break;
                    case UnicodeCategory.UppercaseLetter:
                        range.Append(":Lu:");
                        break;
                    case UnicodeCategory.EnclosingMark:
                        range.Append(":Me:");
                        break;
                    case UnicodeCategory.NonSpacingMark:
                        range.Append(":Mn:");
                        break;
                    case UnicodeCategory.SpacingCombiningMark:
                        range.Append(":Mc:");
                        break;
                    case UnicodeCategory.DecimalDigitNumber:
                        range.Append(":Nd:");
                        break;
                    case UnicodeCategory.LetterNumber:
                        range.Append(":Nl:");
                        break;
                    case UnicodeCategory.OtherNumber:
                        range.Append(":No:");
                        break;
                    case UnicodeCategory.ConnectorPunctuation:
                        range.Append(":Pc:");
                        break;
                    case UnicodeCategory.ClosePunctuation:
                        range.Append(":Pe:");
                        break;
                    case UnicodeCategory.DashPunctuation:
                        range.Append(":Pd:");
                        break;
                    case UnicodeCategory.FinalQuotePunctuation:
                        range.Append(":Pf:");
                        break;
                    case UnicodeCategory.OpenPunctuation:
                        range.Append(":Ps:");
                        break;
                    case UnicodeCategory.OtherPunctuation:
                        range.Append(":Po:");
                        break;
                    case UnicodeCategory.InitialQuotePunctuation:
                        range.Append(":Pi:");
                        break;
                    case UnicodeCategory.CurrencySymbol:
                        range.Append(":Sc:");
                        break;
                    case UnicodeCategory.ModifierSymbol:
                        range.Append(":Sk:");
                        break;
                    case UnicodeCategory.MathSymbol:
                        range.Append(":Sm:");
                        break;
                    case UnicodeCategory.OtherSymbol:
                        range.Append(":So:");
                        break;
                    case UnicodeCategory.LineSeparator:
                        range.Append(":Zl:");
                        break;
                    case UnicodeCategory.ParagraphSeparator:
                        range.Append(":Zp:");
                        break;
                    case UnicodeCategory.SpaceSeparator:
                        range.Append(":Zs:");
                        break;
                }
            }
            return string.Format("{0}", range.ToString());
        }

        internal static string GetCharacterString(char c)
        {
            //Special escapes.
            if (c == (char)0x5E)
                return "\\x5E";
            else if (c == char.MinValue)
                return "\\0";
            else if (c == '\\')
                return @"\\";
            else if (c == '\'')
                return "\\'";
            else if (c == '\r')
                return "\\r";
            else if (c == '\t')
                return "\\t";
            else if (c == '\v')
                return "\\v";
            else if (c == '\f')
                return "\\f";
            else if (c == '\n')
                return "\\n";
            else if (c == (char)32)
                return "\\x20";
            else if (c == char.MinValue)
                return "\\0";
            else if (c > (char)0xCC && c < 256)
            {
                string s = string.Format("{0:X}", (int)c);
                while (s.Length < 2)
                    s = "0" + s;
                return string.Format("\\x{0}", s);
            }
            else if (c >= 256)
            {
                string s = string.Format("{0:X}", (int)c);
                while (s.Length < 4)
                    s = "0" + s;
                return string.Format("\\u{0}", s);
            }
            return c.ToString();
        }

        public static void InitLookups(this IGDFile file, _GDResolutionAssistant resolutionAid = null)
        {
            LinkerCore.resolutionAid = resolutionAid;
            LinkerCore.errorEntries = (file.GetErrorEnumerator());
            LinkerCore.tokenEntries = (file.GetTokenEnumerator());
            LinkerCore.ruleEntries = (file.GetRulesEnumerator());
            LinkerCore.ruleTemplEntries = (file.GetTemplateRulesEnumerator());
        }

        public static void InitLookups(this IEnumerable<IGDFile> files, _GDResolutionAssistant resolutionAid = null)
        {
            LinkerCore.resolutionAid = resolutionAid;
            LinkerCore.errorEntries = (LinkerCore.GetErrorEnumerator(files));
            LinkerCore.tokenEntries = (LinkerCore.GetTokenEnumerator(files));
            LinkerCore.ruleEntries = (LinkerCore.GetRulesEnumerator(files));
            LinkerCore.ruleTemplEntries = (LinkerCore.GetTemplateRulesEnumerator(files));
        }

        public static void ClearLookups()
        {
            LinkerCore.resolutionAid = null;
            LinkerCore.errorEntries = null;
            LinkerCore.tokenEntries = null;
            LinkerCore.ruleEntries = null;
            LinkerCore.ruleTemplEntries = null;
        }

        /// <summary>
        /// Cleans up the rules defined in a given grammar description parse.
        /// </summary>
        /// <param name="parserResults">The <see cref="IParserResults{T}"/> which
        /// contains the grammar description file.</param>
        /// <remarks>'Cleanup' means to remove rules which have no functional use, such as
        /// containing no terminals/non-terminals.</remarks>
        public static void CleanupRules()
        {
            foreach (var rule in LinkerCore.ruleEntries)
                CleanupRule(rule);
        }

        private static void CleanupRule(IProductionRuleSeries series)
        {
            List<IProductionRule> terminalList = new List<IProductionRule>();
            foreach (var expression in series)
            {
                //Only dispose rules that were null post-cleanup.
                if (expression.Count == 0)
                    continue;
                CleanupRule(expression);
                if (expression.Count == 0)
                    terminalList.Add(expression);
            }
            foreach (var expression in terminalList)
                ((ReadOnlyCollection<IProductionRule>)series).baseCollection.Remove(expression);
        }

        private static void CleanupRule(IProductionRule expression)
        {
            List<IProductionRuleGroupItem> terminalList = new List<IProductionRuleGroupItem>();
            foreach (var item in expression)
            {
                if (item is IProductionRuleGroupItem)
                {
                    var gItem = (IProductionRuleGroupItem)item;
                    if (gItem.Count == 0)
                        terminalList.Add(gItem);
                    else
                        CleanupRule(gItem);
                }
            }
            foreach (var item in terminalList)
                ((ProductionRule)(expression)).baseCollection.Remove(item);
        }

        public static bool InlineTokens(IGDFile file, CompilerErrorCollection errors)
        {
            ITokenEntry[] originals = LinkerCore.tokenEntries.ToArray();
            InlinedTokenEntry[] result = new InlinedTokenEntry[originals.Length];
            Dictionary<ITokenEntry, InlinedTokenEntry> originalNewLookup = new Dictionary<ITokenEntry, InlinedTokenEntry>();
            for (int i = 0; i < result.Length; i++)
                result[i] = InliningCore.Inline(originals[i]);
            for (int i = 0; i < result.Length; i++)
                originalNewLookup.Add(originals[i], result[i]);
            for (int i = 0; i < result.Length; i++)
                result[i].ResolveLowerPrecedencesAgain(originalNewLookup);
            GDEntryCollection gdec = file as GDEntryCollection;
            if (gdec == null)
                return false;
            for (int i = 0; i < originals.Length; i++)
            {
                gdec.Insert(gdec.IndexOf(originals[i]), result[i]);
                /* *
                 * The rule building phase will require direct references
                 * to the tokens, because it uses them in a context lookup.
                 * *
                 * Time to replace every reference for the token with
                 * the inlined version.
                 * *
                 * This includes literal reference members for the same reason.
                 * */
                ReplaceTokenReference(originals[i], result[i]);
                file.Remove(originals[i]);
            }
            return true;
        }

        private static void ReplaceTokenReference(ITokenEntry sourceElement, InlinedTokenEntry destination)
        {
            foreach (var rule in LinkerCore.ruleEntries)
                ReplaceTokenReference(rule, sourceElement, destination);
        }

        private static void ReplaceTokenReference(IProductionRuleSeries target, ITokenEntry sourceElement, InlinedTokenEntry destination)
        {
            foreach (var expression in target)
                ReplaceTokenReference(expression, sourceElement, destination);
        }

        private static void ReplaceTokenReference(IProductionRule target, ITokenEntry sourceElement, InlinedTokenEntry destination)
        {
            foreach (var item in target)
                ReplaceTokenReference(item, sourceElement, destination);
        }

        private static void ReplaceTokenReference(IProductionRuleItem target, ITokenEntry sourceElement, InlinedTokenEntry destination)
        {
            if (target is IProductionRuleGroupItem)
                ReplaceTokenReference((IProductionRuleSeries)target, sourceElement, destination);
            else if (target is ITokenReferenceProductionRuleItem)
            {
                var tTarget = target as TokenReferenceProductionRuleItem;
                if (tTarget.Reference != sourceElement)
                    return;
                tTarget.Reference = destination;
            }
            else if (target is ILiteralStringReferenceProductionRuleItem)
            {
                var tTarget = target as LiteralStringReferenceProductionRuleItem;
                if (tTarget.Source != sourceElement)
                    return;
                tTarget.Source = destination;
                /* *
                 * The inlined token provides a lookup to assist
                 * reference replacement.
                 * */
                tTarget.Literal = (ILiteralStringTokenItem)destination.OldNewLookup[tTarget.Literal];
            }
            else if (target is ILiteralCharReferenceProductionRuleItem)
            {
                var tTarget = target as LiteralCharReferenceProductionRuleItem;
                if (tTarget.Source != sourceElement)
                    return;
                tTarget.Source = destination;
                tTarget.Literal = (ILiteralCharTokenItem)destination.OldNewLookup[tTarget.Literal];
            }
        }

        /// <summary>
        /// Obtains the tokens associated to a grammar description file.
        /// </summary>
        /// <param name="file">The <see cref="IGDFile"/> which contains the tokens
        /// to enumerate.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> associated to the tokens of the
        /// <paramref name="file"/> provided.</returns>
        public static IEnumerable<InlinedTokenEntry> GetTokens(this IGDFile file)
        {
            return from e in file
                   let iE = e as InlinedTokenEntry
                   where iE != null
                   orderby iE.Name
                   select iE;
        }

        /// <summary>
        /// Obtains the rules associated to a grammar description file.
        /// </summary>
        /// <param name="file">The <see cref="IGDFile"/> which contains the rules 
        /// to enumerate.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> associated to the rules of the
        /// <paramref name="file"/> provided.</returns>
        public static IEnumerable<IProductionRuleEntry> GetRules(this IGDFile file)
        {
            return from e in file
                   let iE = e as IProductionRuleEntry
                   where iE != null
                   orderby iE.Name
                   select iE;
        }

        public static ParserBuilderResults Build(this IGDFile file, List<string> streamAnalysisFiles, CompilerErrorCollection errors, Action<ParserBuilderPhase> phaseShifter)
        {
            ParserBuilder builder = new ParserBuilder(file, errors, streamAnalysisFiles);

            foreach (ParserBuilderPhase phase in builder)
                phaseShifter(phase);

            return new ParserBuilderResults() { Project = builder.Project, PhaseTimes = new ReadOnlyDictionary<ParserBuilderPhase, TimeSpan>(builder.PhaseTimes), RuleStateMachines= builder.RuleDFAStates };
        }

        public static SyntacticalNFAState BuildNFA(this IProductionRuleSeries series, IGrammarSymbolSet symbols, ParserBuilder builder)
        {
            return series.BuildNFA(null, symbols, builder);
        }

        public static SyntacticalNFAState BuildNFA(this IProductionRuleSeries series, SyntacticalNFAState root, IGrammarSymbolSet symbols, ParserBuilder builder)
        {
            SyntacticalNFAState state = root;
            bool first = true;
            foreach (var expression in series)
            {
                if (state == null)
                    state = expression.BuildNFA(symbols, builder);
                else
                {
                    var expressionNFA = expression.BuildNFA(symbols, builder);
                    if (first)
                    {
                        bool isEdge = expressionNFA.IsEdge;
                        first = false;
                        state.Union(expressionNFA);
                        if (state.IsEdge && !isEdge)
                            state.IsEdge = isEdge;
                    }
                    else
                        state.Union(expressionNFA);
                }
            }
            if (!(series is IProductionRuleGroupItem))
            {
                List<SyntacticalNFAState> flatForm = new List<SyntacticalNFAState>();
                SyntacticalNFAState.FlatlineState(state, flatForm);
                foreach (var fState in flatForm)
                    fState.SetIntermediate(series);
                state.SetInitial(series);
                foreach (var edge in state.ObtainEdges())
                    edge.SetFinal(series);
            }
            return state;
        }

        public static SyntacticalNFAState BuildNFA(this IProductionRule rule, IGrammarSymbolSet symbols, ParserBuilder builder)
        {
            SyntacticalNFAState state = null;
            foreach (var item in rule)
                if (state == null)
                    state = item.BuildNFA(symbols, builder);
                else
                    state.Concat(item.BuildNFA(symbols, builder));
            return state;
        }

        public static SyntacticalNFAState BuildNFA(this IProductionRuleItem item, IGrammarSymbolSet symbols, ParserBuilder builder)
        {
            SyntacticalNFAState result = null;
            if (item is IProductionRuleGroupItem)
                result = ((IProductionRuleSeries)item).BuildNFA(symbols, builder);
            else if (item is ILiteralCharReferenceProductionRuleItem)
                result = ((ILiteralCharReferenceProductionRuleItem)(item)).BuildNFA(symbols, builder);
            else if (item is ILiteralStringReferenceProductionRuleItem)
                result = ((ILiteralStringReferenceProductionRuleItem)(item)).BuildNFA(symbols, builder);
            else if (item is ITokenReferenceProductionRuleItem)
                result = ((ITokenReferenceProductionRuleItem)(item)).BuildNFA(symbols, builder);
            else if (item is IRuleReferenceProductionRuleItem)
                result = ((IRuleReferenceProductionRuleItem)(item)).BuildNFA(symbols, builder);
            else
                throw new ArgumentException("series");
            result.HandleRepeatCycle<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource, SyntacticalNFARootState, IProductionRuleItem>(item, InliningCore.ProductionRuleRootStateClonerCache, () => new SyntacticalNFAState(builder));
            result.SetInitial(item);
            List<SyntacticalNFAState> flatForm = new List<SyntacticalNFAState>();
            SyntacticalNFAState.FlatlineState(result, flatForm);
            foreach (var fState in flatForm)
                fState.SetIntermediate(item);
            result.SetInitial(item);
            foreach (var edge in result.ObtainEdges())
                edge.SetFinal(item);
            return result;
        }

        public static SyntacticalNFAState BuildNFA<T, TLiteral>(this ILiteralReferenceProductionRuleItem<T, TLiteral> item, IGrammarSymbolSet symbols, ParserBuilder builder)
            where TLiteral :
                ILiteralTokenItem<T>
        {
            SyntacticalNFAState state = new SyntacticalNFAState(builder);
            GrammarVocabulary movement = new GrammarVocabulary(symbols, symbols[item.Literal]);
            var stateEnd = new SyntacticalNFAState(builder);
            state.MoveTo(movement, stateEnd);
            return state;
        }

        public static SyntacticalNFAState BuildNFA(this ITokenReferenceProductionRuleItem tokenReference, IGrammarSymbolSet symbols, ParserBuilder builder)
        {
            SyntacticalNFAState state = new SyntacticalNFAState(builder);
            GrammarVocabulary movement = new GrammarVocabulary(symbols, symbols[tokenReference.Reference]);
            var stateEnd = new SyntacticalNFAState(builder);
            state.MoveTo(movement, stateEnd);
            return state;
        }

        public static SyntacticalNFAState BuildNFA(this IRuleReferenceProductionRuleItem ruleReference, IGrammarSymbolSet symbols, ParserBuilder builder)
        {
            SyntacticalNFAState state = new SyntacticalNFAState(builder);
            GrammarVocabulary movement = new GrammarVocabulary(symbols, symbols[ruleReference.Reference]);
            var stateEnd = new SyntacticalNFAState(builder);
            state.MoveTo(movement, stateEnd);
            return state;
        }
    }
}
