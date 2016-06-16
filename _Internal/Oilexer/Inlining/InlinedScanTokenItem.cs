using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    internal class InlinedScanCommandTokenItem :
        ScanCommandTokenItem,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;

        public InlinedScanCommandTokenItem(IScanCommandTokenItem source, IOilexerGrammarTokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(OilexerGrammarInliningCore.Inline(source.SearchTarget, sourceRoot, root, oldNewLookup), source.SeekPast, source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.SiblingAmbiguity = source.SiblingAmbiguity;
            this.Name = source.Name;
        }

        public IScanCommandTokenItem Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public IOilexerGrammarTokenEntry SourceRoot { get; private set; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="InlinedScanCommandTokenItem"/>.
        /// </summary>
        public InlinedTokenEntry Root { get; private set; }

        #region IInlinedTokenItem Members

        ITokenItem IInlinedTokenItem.Source
        {
            get { return this.Source; }
        }

        public RegularLanguageNFAState State
        {
            get {
                if (this.state == null)
                {
                    this.state = this.BuildNFAState();
                }
                return this.state;
            }
        }


        private RegularLanguageNFAState BuildNFAState()
        {
            return state;
        }

        #endregion

        public void BuildState(Dictionary<ITokenSource, Captures.ICaptureTokenStructuralItem> sourceReplacementLookup)
        {
            var thisReplacement = sourceReplacementLookup.ContainsKey(this) ? (ITokenSource)(sourceReplacementLookup[this]) : (ITokenSource)this;
            var target = (InlinedTokenExpressionSeries)this.SearchTarget;
            target.BuildState(sourceReplacementLookup);
            RegularLanguageNFAState current = null;
            Stack<RegularLanguageNFAState> states = new Stack<RegularLanguageNFAState>(new RegularLanguageNFAState[] { target.State });
            List<RegularLanguageNFAState> covered = new List<RegularLanguageNFAState>();
            states.Peek().SetInitial(thisReplacement);
            //Step through the sequence until it's finished with the all states
            //associated to the scan operation.
            while (states.Count > 0)
            {
                current = states.Pop();
                if (covered.Contains(current))
                    continue;
                covered.Add(current);
                RegularLanguageSet currentSet = current.OutTransitions.FullCheck;
                if (!currentSet.IsEmpty)
                {
                    foreach (var transition in current.OutTransitions.Values)
                        foreach (var transitionTarget in transition)
                            if (!covered.Contains(transitionTarget))
                                states.Push(transitionTarget);
                    currentSet = currentSet.Complement();
                    if (!(currentSet.IsEmpty))
                        current.MoveTo(currentSet, target.State);
                }
            }
            state = target.State;
            List<RegularLanguageNFAState> flatline = new List<RegularLanguageNFAState>();
            RegularLanguageNFAState.FlatlineState(state, flatline);
            foreach (var fState in flatline)
                fState.SetIntermediate(thisReplacement);
            state.SetInitial(thisReplacement);
            foreach (var edge in State.ObtainEdges())
                edge.SetFinal(thisReplacement);
            state.HandleRepeatCycle<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource, RegularLanguageNFARootState, IInlinedTokenItem>(this, thisReplacement, OilexerGrammarInliningCore.TokenRootStateClonerCache, OilexerGrammarInliningCore.TokenStateClonerCache);
        }
    }
}
