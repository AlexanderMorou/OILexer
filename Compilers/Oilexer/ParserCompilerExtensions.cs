using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using OilexerGrammarFD = AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using OM = System.Collections.ObjectModel;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    using System.Diagnostics;
    using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
    using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
    using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
    using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
    using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
    using AllenCopeland.Abstraction.Utilities.Collections;
    using OilexerGrammarEntryCollection =
            OM::Collection<OilexerGrammarFD::IOilexerGrammarEntry>;
    using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
    using AllenCopeland.Abstraction.Slf.FiniteAutomata;
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

        private static Tuple<RegularLanguageSet, UnicodeCategory[], Dictionary<UnicodeCategory, RegularLanguageSet>> Breakdown(RegularLanguageSet check, RegularLanguageSet fullCheck)
        {
            //if (check.IsNegativeSet)
            //    return new Tuple<RegularLanguageSet, UnicodeCategory[], Dictionary<UnicodeCategory, RegularLanguageSet>>(check, new UnicodeCategory[0], new Dictionary<UnicodeCategory, RegularLanguageSet>());
            var noise = fullCheck.RelativeComplement(check);
            UnicodeCategory[] unicodeCategories;
            Dictionary<UnicodeCategory, RegularLanguageSet> partialCategories = new Dictionary<UnicodeCategory, RegularLanguageSet>();
            ParserCompilerExtensions.PropagateUnicodeCategoriesAndPartials(ref check, noise, out unicodeCategories, out partialCategories);
            return new Tuple<RegularLanguageSet, UnicodeCategory[], Dictionary<UnicodeCategory, RegularLanguageSet>>(check, unicodeCategories, partialCategories);
        }
        public static RegularLanguageDFAUnicodeGraph BuildUnicodeGraph(this RegularLanguageDFAState state)
        {
            RegularLanguageDFAUnicodeGraph result = new RegularLanguageDFAUnicodeGraph() { UnicodeGraph = new UnicodeTargetGraph() };
            if (state.OutTransitions.Count == 0)
                return result;
            var fullSet = state.OutTransitions.FullCheck;
            foreach (var transition in state.OutTransitions)
            {
                /* *
                 * Send in the current transition's requirement, along with the full set
                 * to breakdown the unicode subsets contained within.
                 * */
                var breakdown = Breakdown(transition.Key, fullSet);
                /* *
                 * If the remainder of the unicode breakdown does not overlap enough
                 * of a category to include it, denote the remainder.
                 * */
                if (breakdown.Item1 != null && !breakdown.Item1.IsEmpty)
                    result.Add(breakdown.Item1, transition.Value);
                /* *
                 * If there are partial and full unicode sets,
                 * push them into the unicode target logic result.UnicodeGraph.
                 * */
                if (breakdown.Item2.Length > 0 ||
                    breakdown.Item3.Count > 0)
                {
                    IUnicodeTarget target = null;
                    if (!result.UnicodeGraph.TryGetValue(transition.Value, out target))
                        target = result.UnicodeGraph.Add(transition.Value, transition.Value == state);
                    //Full sets are simple.
                    foreach (var category in breakdown.Item2)
                        target.Add(category);
                    var item3 = breakdown.Item3;
                    /* *
                     * Partial sets are a bit more interesting.
                     * */
                    foreach (var partialCategory in item3.Keys)
                    {
                        /* *
                         * If the partial set doesn't contain a remainder,
                         * the original remainder was consumed by the overall 
                         * checks that occur before it.
                         * *
                         * As an example, if the category is Ll, assuming there
                         * are other paths that utilize a-z, the original check used to
                         * construct the unicode breakdown would note this, but
                         * the full set sent into the breakdown method would negate
                         * the negative set (if a-z are already checked,
                         * there is no need to check that the character -isn't- 
                         * in that range).
                         * */
                        if (item3[partialCategory] == null)
                            target.Add(partialCategory);
                        else
                            target.Add(partialCategory, item3[partialCategory]);
                    }
                }
            }
            return result;
        }

        public static void PropagateUnicodeCategoriesAndPartials(ref RegularLanguageSet target, RegularLanguageSet negativeNoise, out UnicodeCategory[] categories, out Dictionary<UnicodeCategory, RegularLanguageSet> categoriesWithNegativeAssertions)
        {
            List<UnicodeCategory> fullCategories = new List<UnicodeCategory>();
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
                    //if there are more elements that overlap than there are not,
                    //include this category with a negative assertion.
                    var categoryCopy = categoryData ^ intersection;
                    categoryCopy = categoryCopy ^ (categoryCopy & negativeNoise);
                    target ^= intersection;

                    if (categoryCopy.IsEmpty)
                        categoriesWithNegativeAssertions.Add(category, null);
                    else
                        categoriesWithNegativeAssertions.Add(category, categoryCopy);
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

        internal static string GetUnicodeCategoryString(UnicodeCategory target)
        {
            switch (target)
            {
                case UnicodeCategory.Control:
                    return (":Cc:");
                case UnicodeCategory.Format:
                    return (":Cf:");
                case UnicodeCategory.OtherNotAssigned:
                    return (":Cn:");
                case UnicodeCategory.PrivateUse:
                    return (":Co:");
                case UnicodeCategory.Surrogate:
                    return (":Cs:");
                case UnicodeCategory.LowercaseLetter:
                    return (":Ll:");
                case UnicodeCategory.ModifierLetter:
                    return (":Lm:");
                case UnicodeCategory.OtherLetter:
                    return (":Lo:");
                case UnicodeCategory.TitlecaseLetter:
                    return (":Lt:");
                case UnicodeCategory.UppercaseLetter:
                    return (":Lu:");
                case UnicodeCategory.EnclosingMark:
                    return (":Me:");
                case UnicodeCategory.NonSpacingMark:
                    return (":Mn:");
                case UnicodeCategory.SpacingCombiningMark:
                    return (":Mc:");
                case UnicodeCategory.DecimalDigitNumber:
                    return (":Nd:");
                case UnicodeCategory.LetterNumber:
                    return (":Nl:");
                case UnicodeCategory.OtherNumber:
                    return (":No:");
                case UnicodeCategory.ConnectorPunctuation:
                    return (":Pc:");
                case UnicodeCategory.ClosePunctuation:
                    return (":Pe:");
                case UnicodeCategory.DashPunctuation:
                    return (":Pd:");
                case UnicodeCategory.FinalQuotePunctuation:
                    return (":Pf:");
                case UnicodeCategory.OpenPunctuation:
                    return (":Ps:");
                case UnicodeCategory.OtherPunctuation:
                    return (":Po:");
                case UnicodeCategory.InitialQuotePunctuation:
                    return (":Pi:");
                case UnicodeCategory.CurrencySymbol:
                    return (":Sc:");
                case UnicodeCategory.ModifierSymbol:
                    return (":Sk:");
                case UnicodeCategory.MathSymbol:
                    return (":Sm:");
                case UnicodeCategory.OtherSymbol:
                    return (":So:");
                case UnicodeCategory.LineSeparator:
                    return (":Zl:");
                case UnicodeCategory.ParagraphSeparator:
                    return (":Zp:");
                case UnicodeCategory.SpaceSeparator:
                    return (":Zs:");
            }
            return null;
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

        public static void InitLookups(this IOilexerGrammarFile file, _OilexerGrammarResolutionAssistant resolutionAid = null)
        {
            OilexerGrammarLinkerCore.resolutionAid = resolutionAid;
            OilexerGrammarLinkerCore.errorEntries = (file.GetErrorEnumerator());
            OilexerGrammarLinkerCore.tokenEntries = (file.GetTokenEnumerator());
            OilexerGrammarLinkerCore.ruleEntries = (file.GetRulesEnumerator());
            OilexerGrammarLinkerCore.ruleTemplEntries = (file.GetTemplateRulesEnumerator());
        }

        public static void InitLookups(this IEnumerable<IOilexerGrammarFile> files, _OilexerGrammarResolutionAssistant resolutionAid = null)
        {
            OilexerGrammarLinkerCore.resolutionAid = resolutionAid;
            OilexerGrammarLinkerCore.errorEntries = (OilexerGrammarLinkerCore.GetErrorEnumerator(files));
            OilexerGrammarLinkerCore.tokenEntries = (OilexerGrammarLinkerCore.GetTokenEnumerator(files));
            OilexerGrammarLinkerCore.ruleEntries = (OilexerGrammarLinkerCore.GetRulesEnumerator(files));
            OilexerGrammarLinkerCore.ruleTemplEntries = (OilexerGrammarLinkerCore.GetTemplateRulesEnumerator(files));
        }

        public static void ClearLookups()
        {
            OilexerGrammarLinkerCore.resolutionAid = null;
            OilexerGrammarLinkerCore.errorEntries = null;
            OilexerGrammarLinkerCore.tokenEntries = null;
            OilexerGrammarLinkerCore.ruleEntries = null;
            OilexerGrammarLinkerCore.ruleTemplEntries = null;
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
            foreach (var rule in OilexerGrammarLinkerCore.ruleEntries)
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
                ((ControlledCollection<IProductionRule>)series).baseList.Remove(expression);
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
                ((ProductionRule)(expression)).baseList.Remove(item);
        }

        public static bool InlineTokens(IOilexerGrammarFile file)
        {
            IOilexerGrammarTokenEntry[] originals = OilexerGrammarLinkerCore.tokenEntries.ToArray();
            InlinedTokenEntry[] result = new InlinedTokenEntry[originals.Length];
            Dictionary<IOilexerGrammarTokenEntry, InlinedTokenEntry> originalNewLookup = new Dictionary<IOilexerGrammarTokenEntry, InlinedTokenEntry>();
            for (int i = 0; i < result.Length; i++)
                result[i] = OilexerGrammarInliningCore.Inline(originals[i], file);
            for (int i = 0; i < result.Length; i++)
                originalNewLookup.Add(originals[i], result[i]);
            for (int i = 0; i < result.Length; i++)
                result[i].ResolveLowerPrecedencesAgain(originalNewLookup);
            OilexerGrammarEntryCollection gdec = file as OilexerGrammarEntryCollection;
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

        private static void ReplaceTokenReference(IOilexerGrammarTokenEntry sourceElement, InlinedTokenEntry destination)
        {
            foreach (var rule in OilexerGrammarLinkerCore.ruleEntries)
                ReplaceTokenReference(rule, sourceElement, destination);
        }

        private static void ReplaceTokenReference(IProductionRuleSeries target, IOilexerGrammarTokenEntry sourceElement, InlinedTokenEntry destination)
        {
            foreach (var expression in target)
                ReplaceTokenReference(expression, sourceElement, destination);
        }

        private static void ReplaceTokenReference(IProductionRule target, IOilexerGrammarTokenEntry sourceElement, InlinedTokenEntry destination)
        {
            foreach (var item in target)
                ReplaceTokenReference(item, sourceElement, destination);
        }

        private static void ReplaceTokenReference(IProductionRuleItem target, IOilexerGrammarTokenEntry sourceElement, InlinedTokenEntry destination)
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
        /// <param name="file">The <see cref="IOilexerGrammarFile"/> which contains the tokens
        /// to enumerate.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> associated to the tokens of the
        /// <paramref name="file"/> provided.</returns>
        public static IEnumerable<InlinedTokenEntry> GetInlinedTokens(this IOilexerGrammarFile file)
        {
            return from e in file
                   let iE = e as InlinedTokenEntry
                   where iE != null
                   select iE;
        }

        /// <summary>
        /// Obtains the tokens associated to a grammar description file.
        /// </summary>
        /// <param name="file">The <see cref="IOilexerGrammarFile"/> which contains the tokens
        /// to enumerate.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> associated to the tokens of the
        /// <paramref name="file"/> provided.</returns>
        public static IEnumerable<OilexerGrammarTokenEntry> GetTokens(this IOilexerGrammarFile file)
        {
            return from e in file
                   let iE = e as OilexerGrammarTokenEntry
                   where iE != null
                   select iE;
        }

        /// <summary>
        /// Obtains the tokens associated to a grammar description file.
        /// </summary>
        /// <param name="file">The <see cref="IOilexerGrammarFile"/> which contains the tokens
        /// to enumerate.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> associated to the tokens of the
        /// <paramref name="file"/> provided.</returns>
        public static IEnumerable<OilexerGrammarTokenEntry> GetSortedTokens(this IOilexerGrammarFile file)
        {
            return from e in file
                   let iE = e as OilexerGrammarTokenEntry
                   where iE != null
                   orderby iE.Name
                   select iE;
        }

        /// <summary>
        /// Obtains the rules associated to a grammar description file.
        /// </summary>
        /// <param name="file">The <see cref="IOilexerGrammarFile"/> which contains the rules 
        /// to enumerate.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> associated to the rules of the
        /// <paramref name="file"/> provided.</returns>
        public static IEnumerable<IOilexerGrammarProductionRuleEntry> GetRules(this IOilexerGrammarFile file)
        {
            return from e in file
                   let iE = e as IOilexerGrammarProductionRuleEntry
                   where iE != null
                   orderby iE.Name
                   select iE;
        }

        public static SyntacticalNFAState BuildNFA(this IProductionRuleSeries series, IGrammarSymbolSet symbols, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> replacements)
        {
            return series.BuildNFA(null, symbols, lookup, replacements);
        }

        public static SyntacticalNFAState BuildNFA(this IProductionRuleSeries series, SyntacticalNFAState root, IGrammarSymbolSet symbols, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> replacements)
        {
#if ShortcircuitToFinishLexer
            return root;
#else
            SyntacticalNFAState state = root;
            bool first = true;
            foreach (var expression in series)
            {
                if (state == null)
                {
                    state = expression.BuildNFA(symbols, lookup, replacements);
                    first = false;
                }
                else
                {
                    var expressionNFA = expression.BuildNFA(symbols, lookup, replacements);
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
                var source = (replacements.ContainsKey(series) ? (IProductionRuleSource)replacements[series] : (IProductionRuleSource)series);
                state.SetInitial(source);
                foreach (var fState in flatForm)
                    fState.SetIntermediate(source);
                foreach (var edge in state.ObtainEdges())
                    edge.SetFinal(source);
            }
            return state;
#endif
        }

        public static SyntacticalNFAState BuildNFA(this IProductionRule rule, IGrammarSymbolSet symbols, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> replacements)
        {
            SyntacticalNFAState state = null;
            foreach (var item in rule)
                if (state == null)
                    state = item.BuildNFA(symbols, lookup, replacements);
                else
                {
                    var nextState = item.BuildNFA(symbols, lookup, replacements);
                    if (nextState != null)
                        state.Concat(nextState);
                }
            return state;
        }

        public static SyntacticalNFAState BuildNFA(this IProductionRuleItem item, IGrammarSymbolSet symbols, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> replacements)
        {
            SyntacticalNFAState result = null;
            if (item is IProductionRuleGroupItem)
                result = ((IProductionRuleSeries)item).BuildNFA(symbols, lookup, replacements);
            else if (item is ILiteralCharReferenceProductionRuleItem)
                result = ((ILiteralCharReferenceProductionRuleItem)(item)).BuildNFA(symbols, lookup, replacements);
            else if (item is ILiteralStringReferenceProductionRuleItem)
                result = ((ILiteralStringReferenceProductionRuleItem)(item)).BuildNFA(symbols, lookup, replacements);
            else if (item is ITokenReferenceProductionRuleItem)
                result = ((ITokenReferenceProductionRuleItem)(item)).BuildNFA(symbols, lookup, replacements);
            else if (item is IRuleReferenceProductionRuleItem)
                result = ((IRuleReferenceProductionRuleItem)(item)).BuildNFA(symbols, lookup, replacements);
            else
                throw new ArgumentException("series");
            if (result == null)
                return null;
            var source = replacements.ContainsKey(item) ? (IProductionRuleSource)replacements[item] : (IProductionRuleSource)item;
            result.HandleRepeatCycle<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource, SyntacticalNFARootState, IProductionRuleItem>(item, item, OilexerGrammarInliningCore.ProductionRuleRootStateClonerCache, () => new SyntacticalNFAState(lookup, (GrammarSymbolSet)symbols));
            List<SyntacticalNFAState> flatForm = new List<SyntacticalNFAState>();
            SyntacticalNFAState.FlatlineState(result, flatForm);
            result.SetInitial(source);
            foreach (var fState in flatForm)
                fState.SetIntermediate(source);
            foreach (var edge in result.ObtainEdges())
                edge.SetFinal(source);
            return result;
        }

        public static SyntacticalNFAState BuildNFA<T, TLiteral>(this ILiteralReferenceProductionRuleItem<T, TLiteral> item, IGrammarSymbolSet symbols, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> replacements)
            where TLiteral :
                ILiteralTokenItem<T>
        {
            SyntacticalNFAState state = new SyntacticalNFAState(lookup, (GrammarSymbolSet)symbols);
            GrammarVocabulary movement = new GrammarVocabulary(symbols, symbols[item.Literal]);
            var stateEnd = new SyntacticalNFAState(lookup, (GrammarSymbolSet)symbols);
            state.MoveTo(movement, stateEnd);
            return state;
        }

        public static SyntacticalNFAState BuildNFA(this ITokenReferenceProductionRuleItem tokenReference, IGrammarSymbolSet symbols, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> replacements)
        {
            SyntacticalNFAState state = new SyntacticalNFAState(lookup, (GrammarSymbolSet)symbols);
            var symbol = symbols[tokenReference.Reference];
            GrammarVocabulary movement = null;
            if (symbol is IGrammarConstantItemSymbol)
            {
                IGrammarConstantItemSymbol constantItemSymbol = (IGrammarConstantItemSymbol)symbol;
                movement = new GrammarVocabulary(symbols, (from s in symbols
                                                               let cis = s as IGrammarConstantItemSymbol
                                                               where cis != null && cis.Source == constantItemSymbol.Source
                                                           select cis).ToArray());
            }
            else
                movement = new GrammarVocabulary(symbols, symbol);
            var stateEnd = new SyntacticalNFAState(lookup, (GrammarSymbolSet)symbols);
            state.MoveTo(movement, stateEnd);
            return state;
        }

        public static SyntacticalNFAState BuildNFA(this IRuleReferenceProductionRuleItem ruleReference, IGrammarSymbolSet symbols, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> replacements)
        {
            SyntacticalNFAState state = new SyntacticalNFAState(lookup, (GrammarSymbolSet)symbols);
            GrammarVocabulary movement = new GrammarVocabulary(symbols, symbols[ruleReference.Reference]);
            var stateEnd = new SyntacticalNFAState(lookup, (GrammarSymbolSet)symbols);
            state.MoveTo(movement, stateEnd);
            return state;
        }

        public static Dictionary<RegularLanguageDFAState, RegularLanguageDFAHandlingType> DetermineStateHandlingTypes(Dictionary<RegularLanguageDFAState, RegularLanguageDFAStateJumpData> multitargetLookup, RegularLanguageDFAUnicodeGraph dfaTransitionTable)
        {

            var subset = ((dfaTransitionTable.UnicodeGraph == null) ? (new IUnicodeTarget[0]) : (IEnumerable<IUnicodeTarget>)dfaTransitionTable.UnicodeGraph.Values).Select(k => k.Target);
            var setToIterate = subset.Concat(dfaTransitionTable.Values.Where(target => multitargetLookup.ContainsKey(target))).Concat(dfaTransitionTable.Values.Intersect(subset).Where(k => !multitargetLookup.ContainsKey(k))).Distinct().ToArray();
            var jumpTargetStates = new HashSet<RegularLanguageDFAState>(
                from target in setToIterate
                where !multitargetLookup.ContainsKey(target)
                select target);
            var externalJumpTargets = new HashSet<RegularLanguageDFAState>(
                from target in setToIterate
                where multitargetLookup.ContainsKey(target)
                select target);
            var inlineableTargets =
                new HashSet<RegularLanguageDFAState>(dfaTransitionTable.Values.Except(jumpTargetStates.Concat(externalJumpTargets)));
            var falsePositiveInlineableTargets = new HashSet<RegularLanguageDFAState>();
            foreach (var inlineElement in inlineableTargets)
            {
                var check = dfaTransitionTable.Where(kvp => kvp.Value == inlineElement).Select(kvp => kvp.Key).Aggregate(RegularLanguageSet.UnionAggregateDelegate);
                if (!dfaTransitionTable.ContainsKey(check))
                    falsePositiveInlineableTargets.Add(inlineElement);
                else
                    /* Large groupings of characters could be quicker as an if statement across the range, i.e.: 'a' <= nextChar && nextChar <= 'z' vs 26 separate jump notations. */
                    if (check.GetInefficientSwitchCases().Count() > 0)
                        falsePositiveInlineableTargets.Add(inlineElement);
            }
            if (falsePositiveInlineableTargets.Count > 0)
            {
                inlineableTargets = new HashSet<RegularLanguageDFAState>(inlineableTargets.Except(falsePositiveInlineableTargets));
                jumpTargetStates = new HashSet<RegularLanguageDFAState>(jumpTargetStates.Concat(falsePositiveInlineableTargets));
            }
            var allStates = new HashSet<RegularLanguageDFAState>(
                jumpTargetStates
                    .Concat(externalJumpTargets)
                    .Concat(inlineableTargets));
            var dfaHandlingMechanisms =
                (from state in allStates
                 let type = jumpTargetStates.Contains(state) ? RegularLanguageDFAHandlingType.LocalJump : externalJumpTargets.Contains(state) ? RegularLanguageDFAHandlingType.GlobalJump : inlineableTargets.Contains(state) ? RegularLanguageDFAHandlingType.Inline : RegularLanguageDFAHandlingType.Unknown
                 select new { State = state, Type = type }).ToDictionary(k => k.State, v => v.Type);
            return dfaHandlingMechanisms;
        }

        public static IEnumerable<RegularLanguageSet.SwitchPair<char, RegularLanguageSet.RangeSet>> GetInefficientSwitchCases(this RegularLanguageSet check)
        {
            return check.GetRange().Where(r => r.Which == RegularLanguageSet.SwitchPairElement.CharacterRange);
        }

        public static IEnumerable<ITokenSource> FilterSourceTokenItems(this IEnumerable<IInlinedTokenItem> tokenItems, IGrammarSymbolSet symbols)
        {
            return from item in FilterSourceTokenItemsInternal(tokenItems, symbols)
                   orderby item is IOilexerGrammarTokenEntry ? ((IOilexerGrammarTokenEntry)(item)).Name : ((ITokenItem)(item)).Name
                   select item;
        }

        public static IEnumerable<ITokenSource> FilterSourceTokenItemsInternal(this IEnumerable<IInlinedTokenItem> tokenItems, IGrammarSymbolSet symbols)
        {
            var subSymbols = symbols.Where(k => k is IGrammarTokenSymbol).Cast<IGrammarTokenSymbol>().ToArray();
            var grouped = (from t in tokenItems
                           group t by t.Root).ToDictionary(k => k.Key, v =>
                           {
                               var relevantSubset =
                                   subSymbols.Where(k => k.Source == v.Key);
                               var firstElement = relevantSubset.FirstOrDefault();
                               if (firstElement == null)
                                   return new ITokenSource[0];
                               if (firstElement is IGrammarVariableSymbol || firstElement is IGrammarConstantEntrySymbol)
                                   return (IEnumerable<ITokenSource>)new ITokenSource[1] { firstElement.Source };
                               else if (firstElement is IGrammarConstantItemSymbol)
                               {
                                   var furtherRelevantSubset = relevantSubset.Where(k => k is IGrammarConstantItemSymbol).Cast<IGrammarConstantItemSymbol>().Where(k => Enumerable.Contains<ITokenSource>(v, k.SourceItem));
                                   return (IEnumerable<ITokenSource>)furtherRelevantSubset.Select(k => k.SourceItem).Cast<ITokenSource>().ToArray();
                               }
                               return (IEnumerable<ITokenSource>)(new ITokenSource[0]);
                           });
            return grouped.Values.Aggregate((a, b) => a.Concat(b));
        }

        public static IEnumerable<IInlinedTokenItem> ObtainEdgeSourceTokenItems(this RegularLanguageDFAState state)
        {
            return ObtainEdgeSourceTokenItems(state.Sources);
        }

        public static IEnumerable<IInlinedTokenItem> ObtainEdgeSourceTokenItems(this IEnumerable<Tuple<ITokenSource, FiniteAutomationSourceKind>> sources)
        {
            var sourceOwners = sources.Where(k => k.Item1 is IOilexerGrammarTokenEntry).Where(k => k.Item2 == FiniteAutomationSourceKind.Final).ToArray();
            var middletown = 
                (from s in sources
                 where (s.Item2 & FiniteAutomationSourceKind.Final) == FiniteAutomationSourceKind.Final
                 let isi = s.Item1 as ICaptureTokenStructuralItem
                 let subQuery = from s2 in isi == null ? new ITokenSource[0] : (IEnumerable<ITokenSource>)isi.Sources
                                select s2
                 from isiElement in subQuery.DefaultIfEmpty()
                 let currentItem = (IInlinedTokenItem)((s.Item1 is IInlinedTokenItem) ? s.Item1 : isiElement)
                 where currentItem != null
                 where sourceOwners.Any(k => k.Item1 == currentItem.Root && k.Item2 == FiniteAutomationSourceKind.Final)
                 select currentItem).ToArray();
            return middletown;
        }

        public static IEnumerable<ITokenSource> GetActualTokenSources(this IEnumerable<Tuple<ITokenSource, FiniteAutomationSourceKind>> sources)
        {
            var sourceOwners = sources.Where(k => k.Item1 is IOilexerGrammarTokenEntry).Where(k => k.Item2 == FiniteAutomationSourceKind.Final).ToArray();
            return
                (from s in sources
                 let isi = s.Item1 as ICaptureTokenStructuralItem
                 let subQuery = from s2 in isi == null ? new ITokenSource[0] : (IEnumerable<ITokenSource>)isi.Sources
                                select s2
                 from isiElement in subQuery.DefaultIfEmpty()
                 let currentItem = (ITokenSource)((s.Item1 is IInlinedTokenItem) ? s.Item1 : isiElement) ?? ((s.Item1 is IOilexerGrammarTokenEntry) ? s.Item1 : null)
                 where currentItem != null
                 select currentItem).ToArray();
        }

        public static void FilterByPrecedence(this TokenPrecedenceTable tpt, Dictionary<OilexerGrammarTokenEntry, IGrammarTokenSymbol[]> tokens)
        {
            if (tpt.Count > 1)
                for (int setIndex = 1; setIndex < tpt.Count; setIndex++)
                {
                    var currentElements = tpt[setIndex];
                    foreach (var tokenEntry in currentElements)
                        tokens.Remove(tokenEntry);
                }
        }

        public static string ToLocationDetails(this IInlinedTokenItem item, IOilexerGrammarFile file)
        {
            if (item.Root.FileName.ToLower().StartsWith(file.RelativeRoot.ToLower()))
                return string.Format(@"{0}.{1} at location: line {2}, column {3}", string.IsNullOrEmpty(item.Name) ? string.Empty : string.Format("{0} in ", item.Name), item.Root.FileName.Substring(file.RelativeRoot.Length), item.Line, item.Column);
            return item.Root.FileName;
        }

        public static string ToLocationDetails(this InlinedTokenEntry entry, IOilexerGrammarFile file)
        {
            if (entry.FileName.ToLower().StartsWith(file.RelativeRoot.ToLower()))
                return string.Format(@"{0}.{1} at location: line {2}, column {3}", string.IsNullOrEmpty(entry.Name) ? string.Empty : string.Format("{0} in ", entry.Name), entry.FileName.Substring(file.RelativeRoot.Length), entry.Line, entry.Column);
            return entry.FileName;
        }


        public static bool AllSourcesAreSameIdentity(this IEnumerable<IProductionRuleSource> sources)
        {
            var firstSource = sources.First();
            if (firstSource is ILiteralReferenceProductionRuleItem)
            {
                ILiteralReferenceProductionRuleItem sourceItem = (ILiteralReferenceProductionRuleItem)(firstSource);
                return sources.Skip(1).All(s => s is ILiteralReferenceProductionRuleItem && ((ILiteralReferenceProductionRuleItem)s).Literal == sourceItem.Literal);
            }
            else
                Debug.Assert(false, "Unknown entity type for flag item.");
            return false;
        }

        public static ProductionRuleProjectionNode GetRuleNodeFromFullSeries(this Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> fullSeries, IOilexerGrammarProductionRuleEntry entry)
        {
            return fullSeries.First(p => p.Value.Rule == entry).Value.RootNode;
        }

        public static IEnumerable<KeysValuePair<MultikeyedDictionaryKeys<TKey1, TKey2>, TValue>> Filter<TKey1, TKey2, TValue>(this IMultikeyedDictionary<TKey1, TKey2, TValue> dict, TKey1 key1Filter)
        {
            return from ksvp in dict
                   where ksvp.Keys.Key1.Equals(key1Filter)
                   select ksvp;
        }

        public static IEnumerable<KeysValuePair<MultikeyedDictionaryKeys<TKey1, TKey2, TKey3>, TValue>> Filter<TKey1, TKey2, TKey3, TValue>(this IMultikeyedDictionary<TKey1, TKey2, TKey3, TValue> dict, TKey1 key1Filter, TKey2 key2Filter)
        {
            return from ksvp in dict
                   where ksvp.Keys.Key1.Equals(key1Filter) &&
                         ksvp.Keys.Key2.Equals(key2Filter)
                   select ksvp;
        }

        public static IEnumerable<KeysValuePair<MultikeyedDictionaryKeys<TKey1, TKey2, TKey3>, TValue>> Filter<TKey1, TKey2, TKey3, TValue>(this IMultikeyedDictionary<TKey1, TKey2, TKey3, TValue> dict, TKey1 key1Filter)
        {
            return from ksvp in dict
                   where ksvp.Keys.Key1.Equals(key1Filter)
                   select ksvp;
        }

        public static IDictionary<TKey3, TValue> FilterToDictionary<TKey1, TKey2, TKey3, TValue>(this IMultikeyedDictionary<TKey1, TKey2, TKey3, TValue> dict, TKey1 key1Filter, TKey2 key2Filter)
        {
            var result = new Dictionary<TKey3, TValue>();
            foreach (var ksvp in dict.Filter(key1Filter, key2Filter))
                result.Add(ksvp.Keys.Key3, ksvp.Value);
            return result;
        }

        public static IDictionary<TKey2, TValue> FilterToDictionary<TKey1, TKey2, TValue>(this IMultikeyedDictionary<TKey1, TKey2, TValue> dict, TKey1 key1Filter)
        {
            var result = new Dictionary<TKey2, TValue>();
            foreach (var ksvp in dict.Filter(key1Filter))
                result.Add(ksvp.Keys.Key2, ksvp.Value);
            return result;
        }

        public static IMultikeyedDictionary<TKey2, TKey3, TValue> FilterToDictionary<TKey1, TKey2, TKey3, TValue>(this IMultikeyedDictionary<TKey1, TKey2, TKey3, TValue> dict, TKey1 key1Filter)
        {
            var result = new MultikeyedDictionary<TKey2, TKey3, TValue>();
            foreach (var ksvp in dict.Filter(key1Filter))
                result.Add(ksvp.Keys.Key2, ksvp.Keys.Key3, ksvp.Value);
            return result;
        }

    }
}
