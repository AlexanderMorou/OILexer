using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class RegularLanguageTransducerStateMachine :
        IRegularLanguageStateMachine
    {
        private RegularLanguageDFAState current;
        private RegularLanguageDFARootState initial;
        private InlinedTokenEntry entry;
        private ParserBuilder builder;
        private Dictionary<ITokenSource, Tuple<GrammarVocabulary, IGrammarConstantItemSymbol>> crossReferences;
        private GrammarVocabulary terminals;
        public RegularLanguageTransducerStateMachine(RegularLanguageDFARootState initial, ParserBuilder builder)
        {
            this.initial = initial;
            this.builder = builder;
            this.entry = (InlinedTokenEntry)initial.Entry;
            this.crossReferences = this.BuildCrossReference();
            terminals = null;
        }

        private Dictionary<ITokenSource, Tuple<GrammarVocabulary, IGrammarConstantItemSymbol>> BuildCrossReference()
        {
            lock (this.builder)
            {
                var breakdown = GrammarVocabulary.ObtainCompleteSet(this.builder).Breakdown;
                var sources = (from source in this.initial.SourceSet
                               where source is ILiteralTokenItem
                               let lTsource = source as ILiteralTokenItem
                               where (!(string.IsNullOrEmpty(lTsource.Name)))
                               select source).ToArray();
                GrammarBreakdown.TokenElements elements = breakdown.LiteralSeriesTokens[this.entry];
                return sources.ToDictionary(key => key, value =>
                {
                    var valueSymbol = elements.First(valueLookup => valueLookup.SourceItem == value);
                    return new Tuple<GrammarVocabulary, IGrammarConstantItemSymbol>(new GrammarVocabulary(builder.GrammarSymbols, valueSymbol), valueSymbol);
                });
            }
        }

        #region IRegularLanguageStateMachine Members

        public bool Next(char c)
        {
            //iterate through the current state's outgoing transitions.
            foreach (var transition in current.OutTransitions.Keys)
            {
                /* *
                 * If the transition contains the character, then it's
                 * the best candidate for transition.  This is due to
                 * the deterministic nature of the state machine used.
                 * */
                if (transition.Contains(c))
                {
                    //Advance the state.
                    current = current.OutTransitions[transition];

                    if (current.IsEdge)
                    {
                        foreach (var terminal in (from source in current.Sources
                                                  where source.Item2 == FiniteAutomationSourceKind.Final
                                                  where this.crossReferences.ContainsKey(source.Item1)
                                                  let symbolEntry = this.crossReferences[source.Item1]
                                                  select new { Symbol = symbolEntry.Item2, Grammar = symbolEntry.Item1 }))
                            if (this.terminals == null)
                                this.terminals = terminal.Grammar;
                            else
                            {
                                if (terminal.Symbol.SourceItem is ILiteralStringTokenItem)
                                {
                                    ILiteralStringTokenItem stringElement = terminal.Symbol.SourceItem as ILiteralStringTokenItem;
                                    if (stringElement.SiblingAmbiguity)
                                        this.terminals |= terminal.Grammar;
                                    else
                                        this.terminals = terminal.Grammar;
                                }
                                else
                                    this.terminals = terminal.Grammar;
                            }
                    }
                    return this.current.OutTransitions.Count > 0;
                }
            }
            return false;
        }

        public void Reset()
        {
            this.current = this.initial;
            this.terminals = null;
        }

        public bool InValidEndState
        {
            get { return terminals != null; }
        }

        public int LongestLength
        {
            get
            {
                if (!this.InValidEndState)
                    return 0;
                else
                {
                    return (from symbol in this.terminals.Breakdown.LiteralSeriesTokens[this.entry]
                            let length = symbol.SourceItem.Value.ToString().Length
                            orderby length descending
                            select length).First();
                }
            }
        }

        public RegularLanguageScanData.Entry GetEntry()
        {
            if (this.InValidEndState)
                return new RegularLanguageScanData.Entry(terminals);
            else
                return new RegularLanguageScanData.Entry(GrammarVocabulary.NullInst);
        }

        public void AddEntries(RegularLanguageScanData result)
        {
            foreach (var symbol in from symbol in this.terminals.GetSymbols()
                                   let symbolGrammar = new GrammarVocabulary(this.builder.GrammarSymbols, symbol)
                                   select symbolGrammar)
                result.Add(new RegularLanguageScanData.Entry(symbol));
        }

        #endregion
    }
}
