using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Utilities;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Compilers;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class GrammarSymbolSet :
        ControlledCollection<IGrammarSymbol>,
        IGrammarSymbolSet
    {
        //internal Dictionary<InlinedTokenEntry, ContextualAmbiguity> ambiguities = new Dictionary<InlinedTokenEntry,ContextualAmbiguity>();
        private List<IGrammarAmbiguousSymbol> ambiguitySymbols;
        private Dictionary<IGrammarAmbiguousSymbol, IIntermediateClassPropertyMember> ambiguityKeySymbolsCriteria;
        private Dictionary<ITokenSource, IGrammarTokenSymbol> tokenToSymbolLookup = new Dictionary<ITokenSource, IGrammarTokenSymbol>();
        private Dictionary<IOilexerGrammarProductionRuleEntry, IGrammarRuleSymbol> ruleToSymbolLookup = new Dictionary<IOilexerGrammarProductionRuleEntry, IGrammarRuleSymbol>();

        /// <summary>
        /// Creates a new <see cref="GrammarSymbolSet"/> with the <paramref name="source"/> file provided.
        /// </summary>
        /// <param name="source">The grammar description file which denotes the symbols of the language.</param>
        public GrammarSymbolSet(IOilexerGrammarFile source)
        {
            /* *
             * The symbol set will be used to construct the language's primary enumerator.  Since it will
             * consist of elements from a very large pool (beyond 64 elements), it will consist of a structure
             * with 'slots', the elements from this listing will be a sorted representation of that enum's values.
             * * 
             * The bits will be aligned relative to their position in this listing.
             * */
            IEnumerable<InlinedTokenEntry> tokens = source.GetInlinedTokens();
            IEnumerable<IOilexerGrammarProductionRuleEntry> rules = source.GetRules();
            List<IGrammarSymbol> sorted = new List<IGrammarSymbol>();
            foreach (var token in tokens)
                if (token is OilexerGrammarInlinedTokenEofEntry)
                    sorted.Add(new GrammarConstantEntrySymbol(token));
                else
                    switch (token.DetermineKind())
                    {
                        case RegularCaptureType.Recognizer:
                            //Simple literal values are quick recognizers.
                            if (token.Branches.Count == 1 &&
                                token.Branches[0].Count == 1 &&
                                token.Branches[0][0] is ILiteralTokenItem &&
                                token.Branches[0][0].RepeatOptions.Options == ScannableEntryItemRepeatOptions.None)
                            {
                                var item = new GrammarConstantEntrySymbol(token);
                                sorted.Add(item);
                                tokenToSymbolLookup.Add(token, item);
                            }
                            else
                                goto case RegularCaptureType.Capturer;
                            break;
                        case RegularCaptureType.Transducer:
                        case RegularCaptureType.Capturer:
                            //Element is a variable capturer/recognizer.
                            {
                                var item = new GrammarVariableSymbol(token);
                                sorted.Add(item);
                                tokenToSymbolLookup.Add(token, item);
                            }
                            break;
                        case RegularCaptureType.ContextfulTransducer:
                            /* *
                             * A transducer which matches a literal from a set.
                             * */
                            InjectTokenLiteralSet(token, sorted);

                            break;
                    }
            /* *
             * Add rules last, quickest sort is a list already in order.
             * *
             * Rules won't be a part of the in-order list of elements, but it is important to be able to refer to
             * all constructs in a uniform manner for the sake of the parser compiler.
             * */
            foreach (var rule in rules)
            {
                var symbol = new GrammarRuleSymbol(rule);
                sorted.Add(symbol);
                this.ruleToSymbolLookup.Add(rule, symbol);
            }
            sorted.Sort(new GrammarSymbolComparer());

            foreach (var item in sorted)
                base.baseList.Add(item);
        }

        public IGrammarTokenSymbol GetSymbolFromEntry(IOilexerGrammarTokenEntry token)
        {
            IGrammarTokenSymbol symbol;
            if (this.tokenToSymbolLookup.TryGetValue(token, out symbol))
                return (IGrammarTokenSymbol)symbol;
            return null;
        }

        public IEnumerable<IGrammarTokenSymbol> GetSymbolsFromEntry(IOilexerGrammarTokenEntry token)
        {
            {
                IGrammarTokenSymbol symbol;
                if (this.tokenToSymbolLookup.TryGetValue(token, out symbol))
                {
                    yield return symbol;
                    yield break;
                }
            }
            {
                foreach (var symbol in this.tokenToSymbolLookup.Values.Where(k => k.Source == token))
                    yield return symbol;
            }
        }

        public IEnumerable<IGrammarTokenSymbol> GetSymbolsFromEntries(IEnumerable<IOilexerGrammarTokenEntry> tokens)
        {
            foreach (var token in tokens)
                foreach (var symbol in GetSymbolsFromEntry(token))
                    yield return symbol;
        }

        public IEnumerable<IGrammarRuleSymbol> GetSymbolsFromEntries(IEnumerable<IOilexerGrammarProductionRuleEntry> rules)
        {
            foreach (var rule in rules)
                yield return GetSymbolFromEntry(rule);
        }

        public IGrammarRuleSymbol GetSymbolFromEntry(IOilexerGrammarProductionRuleEntry rule)
        {
            IGrammarRuleSymbol symbol;
            if (this.ruleToSymbolLookup.TryGetValue(rule, out symbol))
                return symbol;
            return null;
        }

        public IEnumerable<IGrammarRuleSymbol> GetRuleSymbols(IEnumerable<IOilexerGrammarProductionRuleEntry> rules)
        {
            foreach (var rule in rules)
            {
                IGrammarRuleSymbol result;
                if (this.ruleToSymbolLookup.TryGetValue(rule, out result))
                    yield return result;
            }

        }

        private void InjectTokenLiteralSet(InlinedTokenEntry token, ICollection<IGrammarSymbol> sorted)
        {
            InjectTokenLiteralSet(token, token.Branches, sorted);
        }
        private void InjectTokenLiteralSet(InlinedTokenEntry token, ITokenExpressionSeries series, ICollection<IGrammarSymbol> sorted)
        {
            foreach (var expression in series)
                InjectTokenLiteralSet(token, expression, sorted);
        }
        private void InjectTokenLiteralSet(InlinedTokenEntry token, ITokenExpression expression, ICollection<IGrammarSymbol> sorted)
        {
            if (expression.Count != 1)
                return;
            if (expression[0] is ILiteralTokenItem)
                InjectTokenLiteralSet(token, (ILiteralTokenItem)(expression[0]), sorted);
            else if (expression[0] is ITokenGroupItem)
                InjectTokenLiteralSet(token, (ITokenGroupItem)(expression[0]), sorted);
        }

        private void InjectTokenLiteralSet(InlinedTokenEntry token, ILiteralTokenItem item, ICollection<IGrammarSymbol> sorted)
        {
            var symbol = new GrammarConstantItemSymbol(token, item);
            sorted.Add(symbol);
            this.tokenToSymbolLookup.Add(item, symbol);
        }

        public IEnumerable<IGrammarTokenSymbol> SymbolsFromSources(IEnumerable<ITokenSource> sources)
        {
            return (from s in sources
                    let identity = DeriveProperIdentity(s)
                    join r in this.tokenToSymbolLookup on identity equals r.Key
                    select (IGrammarTokenSymbol)r.Value)
#if DEBUG
                /*Indentation Fix.*/
                    .ToArray()
#endif
                /*Indentation Fix.*/
                    ;
        }

        private ITokenSource DeriveProperIdentity(ITokenSource s)
        {
            if (s is IInlinedTokenItem)
            {
                var iiti = (IInlinedTokenItem)s;

                var tokenEntry = iiti.Root;
                if (iiti is ILiteralTokenItem ||
                    iiti is ILiteralStringTokenItem)
                {
                    if (tokenEntry.Branches.Count == 1 &&
                        tokenEntry.Branches[0].Count == 1)
                        return this.tokenToSymbolLookup.ContainsKey(iiti) ? (ITokenSource)iiti : (ITokenSource)tokenEntry;
                }
                if (this.tokenToSymbolLookup.ContainsKey(tokenEntry))
                    return tokenEntry;
                else if (this.tokenToSymbolLookup.ContainsKey(iiti))
                    return iiti;
                else
                    return tokenEntry;
            }
            return s;
        }

        private bool IsLiteralFromConstantEntry(IInlinedTokenItem item)
        {
            if (item.Root.Branches.Count == 1 && 
                item.Root.Branches[0].Count == 1 && 
                item.Root.Branches[0][0] == item &&
                item is ILiteralTokenItem)
                return !this.tokenToSymbolLookup.Keys.Contains(item);
            return false;
        }

        //#region IGrammarSymbolSet Members

        public IGrammarSymbol this[IOilexerGrammarTokenEntry entry]
        {
            get
            {
                return this.FirstOrDefault(currentEntry =>
                    {
                        if (currentEntry is IGrammarTokenSymbol)
                        {
                            var currentCES = (IGrammarTokenSymbol)currentEntry;
                            return currentCES.Source == entry;
                        }
                        else if (currentEntry is IGrammarConstantEntrySymbol)
                        {
                            var currentCES = (IGrammarConstantEntrySymbol)currentEntry;
                            return currentCES.Source == entry;
                        }
                        return false;
                    });
            }
        }

        public IGrammarSymbol this[ILiteralTokenItem entry]
        {
            get {
                return this.FirstOrDefault(currentEntry =>
                {
                    if (currentEntry is IGrammarConstantItemSymbol)
                    {
                        var currentCES = (IGrammarConstantItemSymbol)currentEntry;
                        return currentCES.SourceItem == entry;
                    }
                    return false;
                });
            }
        }

        public IGrammarSymbol this[IOilexerGrammarProductionRuleEntry entry]
        {
            get
            {
                return this.FirstOrDefault(currentEntry =>
                {
                    if (currentEntry is IGrammarRuleSymbol)
                    {
                        var currentCES = (IGrammarRuleSymbol)currentEntry;
                        return currentCES.Source == entry;
                    }
                    return false;
                });
            }
        }

        //#endregion

        /// <summary>
        /// Creates the necessary framework to support marshaling of ambiguous contexts.
        /// </summary>
        internal void GenerateAmbiguityContext(List<IGrammarAmbiguousSymbol> ambiguities)
        {
            /* *
             * ClearCache is actually here to bring the 'GrammarVocabulary' class into scope, the instance count on the type should be zero at this
             * point, if it's not, it could mean some analysis on the GrammarVocabulary type will later fail due to the significant bits that 
             * represent the subsets of the GrammarSymbolSet would then be incorrect.
             * */
            GrammarVocabulary.ClearCache();
            /* *
             * This sorting was handled earlier, with ambiguous context present, we're going to sort again.  The small cost to sorting previously is
             * worth it due to the simplicity it offers in debugging with the set sorted.
             * */
            var presortedSet = this.ToList();
            this.ambiguitySymbols = ambiguities;
            presortedSet.Sort(new GrammarSymbolComparer(this));
            this.baseList.Clear();
            /* *
             * To remove the need to unnecessarily break down every single token interaction against an ambiguous context, we employ a simple concept:
             *  If the bit-array offset/length relative to the full set of our items is not even within the range of the ambiguous elements, then
             *  skip the check.
             * */
            var firstAmbiguity = presortedSet.FirstOrDefault(s => ambiguitySymbols.Any(a => a.Contains(s)));
            if (firstAmbiguity != null)
            {
                AmbiguitySymbolStart = presortedSet.IndexOf(firstAmbiguity);
                //The count is the number of tokens that show up within the subsets that make up the ambiguities parameter.
                AmbiguitySymbolCount = presortedSet.Count(s => ambiguitySymbols.Any(a => a.Contains(s)));
            }
            this.AddRange(presortedSet.Concat(ambiguities).ToArray());
        }

        internal void PruneUnusedAmbiguities(ICompilerErrorCollection errors, ParserCompiler compiler)
        {
            /* *
             * We count the hits against a given ambiguity as they are encountered in the wild.
             * *
             * To avoid creating a bunch of noise in the result grammar, we prune the items that aren't observed 
             * in the grammar at large.
             * */
            var unused = this.AmbiguousSymbols.Where(ambiguity => ambiguity.Occurrences == 0).ToArray();
            /* *
             * Obtain the indices of the items that are actually observed.
             * *
             * We'll be removing them from the set, but 'softly'.  We can't actually remove items at this juncture because there are dependent
             * data sets that -do- rely on items after the unused indices that don't appear at the end.
             * *
             * The GrammarVocabulary represents a set of significant bits, one for each symbol, 
             * */
            var unusedIndices = unused.Select(unusedItem => this.IndexOf(unusedItem));
            foreach (var unusedElement in unused)
                errors.ModelWarning<GrammarVocabulary>(OilexerGrammarCore.CompilerWarnings.UnobservedLexicalAmbiguity, unusedElement.AmbiguityKey, unusedElement.AmbiguityKey.ToString());
            foreach (var unusedIndex in unusedIndices.OrderByDescending(k => k))
                if (unusedIndex == this.baseList.Count - 1)
                    this.baseList.RemoveAt(unusedIndex);
                else
                    this.baseList[unusedIndex] = null;

            this.ambiguitySymbols = this.ambiguitySymbols.Except(unused).ToList();
            var remaining = this.ambiguitySymbols.Cast<GrammarAmbiguousSymbol>();
            foreach (var remainingElement in remaining)
                errors.ModelWarning<GrammarVocabulary>(OilexerGrammarCore.CompilerWarnings.LexicalAmbiguity, remainingElement.AmbiguityKey, remainingElement.AmbiguityKey.ToString());
            int ambiguityIndex = 0;
            /* *
             * Reindex the living ambiguity subset, this simplifies the perception of the set, since it's logically curious when a 
             * set of ordered elements is discontinuous.
             * */
            foreach (var element in remaining)
            {
                element.AmbiguityCount = this.ambiguitySymbols.Count;
                element.AmbiguityNumber = ++ambiguityIndex;
                compiler.AmbiguityDetail.Add(element, new GrammarVocabularyAmbiguitySymbolBreakdown { Symbol = element });
            }
        }

        public void GenerateAmbiguitySymbolStoreVariations(GrammarVocabularyModelBuilder runningModel, ParserCompiler compiler)
        {
            /* *
             * The grammar vocabulary model represents the unique variations observed in a given language and provides
             * a convenient mechanism from which to access them.
             * *
             * We're adding the ambiguity keys to the result symbol store, these are more for convenience of those who
             * may have to maintain the result code.
             * */
            this.ambiguityKeySymbolsCriteria = new Dictionary<IGrammarAmbiguousSymbol, IIntermediateClassPropertyMember>();

            foreach (var ambiguity in this.ambiguitySymbols)
            {
                ambiguityKeySymbolsCriteria.Add(
                    ambiguity,
                    compiler.AmbiguityDetail[ambiguity].AmbiguityKeyReference = 
                        runningModel.GenerateSymbolstoreVariation(
                            ambiguity.AmbiguityKey,
                            "AmbiguityKey",
                            string.Format(
                                "Returns the necessary set of tokens in order for the @s:{0}.{1}; to be matched within the edge state of the main lexer state machine.",
                                runningModel.GetSingletonReference(ambiguity).Parent.Name,
                                runningModel.GetSingletonReference(ambiguity).Name)));
            }
        }

        public IEnumerable<IGrammarTokenSymbol> TokenSymbols { get { return this.SymbolsOfType<IGrammarTokenSymbol>(); } }
        public IEnumerable<IGrammarRuleSymbol>  RuleSymbols  { get { return this.SymbolsOfType<IGrammarRuleSymbol> (); } }

        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> of <see cref="IGrammarAmbiguousSymbol"/> elements.
        /// </summary>
        public IEnumerable<IGrammarAmbiguousSymbol> AmbiguousSymbols { get { return this.ambiguitySymbols; } }

        public IEnumerable<T> SymbolsOfType<T>()
            where T :
                IGrammarSymbol
        {
            return this.Where(symbol => symbol is T).Cast<T>(); 
        }

        /// <summary>
        /// Returns the <see cref="IGrouping{TKey, TElement}"/> of the ambiguities observed on the edges of the core lexer's state machine.
        /// </summary>
        public IEnumerable<IGrouping<RegularLanguageDFAState, IGrammarAmbiguousSymbol>> AmbiguitiesByEdge
        {
            get
            {
                return from ambiguity in this.AmbiguousSymbols
                       orderby ambiguity.Count descending
                       from edge in ambiguity.EdgeStates
                       group ambiguity by edge;
            }
        }

        /// <summary>
        /// Returns the <see cref="Int32"/> value denoting the offset within the <see cref="GrammarSymbolSet"/> the ambiguous symbols
        /// start.
        /// </summary>
        public int AmbiguitySymbolStart { get; private set; }
        /// <summary>
        /// Returns the <see cref="Int32"/> value denoting the number of symbols which are observed within ambiguous contexts.
        /// </summary>
        public int AmbiguitySymbolCount { get; private set; }

        /// <summary>
        /// Returns the <see cref="Boolean"/> value denoting whether the <paramref name="gv"/> provided is potentially ambiguous
        /// due to the start/end of the subset of the <see cref="GrammarSymbolSet"/> represented by <paramref name="gv"/>.
        /// </summary>
        /// <param name="gv">The <see cref="GrammarVocabulary"/> which is potentially ambiguous.</param>
        /// <returns>true when the start/end of the subset <paramref name="gv"/> is within the range of <see cref="AmbiguitySymbolStart"/>
        /// and the end <see cref="AmbiguitySymbolStart"/> + <see cref="AmbiguitySymbolCount"/>.</returns>
        /// <remarks>When a grammar is potentially ambiguous it must later be checked against the set of known ambiguities.</remarks>
        public bool IsGrammarPotentiallyAmbiguous(GrammarVocabulary gv)
        {
            /* *
             * This small change knocked off 6% of the follow prediction time.
             * *
             * Might not sound like a lot, but when your execution time is an hour, 6% is 3 and a half minutes.
             * */
            if (gv == null)
                return false;
            /* An ambiguity is represented by two or more symbols that are lexically equivalent under certain circumstances.
             * A single element isn't ambiguous with anything. */
            if (gv.TrueCount <= 1)
                return false;
            //If the first item is after the last ambiguous symbol, then there's no possible overlap.
            if (gv.Offset >= AmbiguitySymbolStart + AmbiguitySymbolCount)
                return false;
            //If the offset+length is less than the start of all ambiguous symbols, there's no possible overlap.
            if (gv.Offset + gv.Length < AmbiguitySymbolStart)
                return false;
            //May be ambiguous, but a full check needs performed.
            return true;
        }
    }
}
