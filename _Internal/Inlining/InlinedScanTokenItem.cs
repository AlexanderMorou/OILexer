using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.FiniteAutomata.Tokens;
using System.Collections;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Inlining
{
    internal class InlinedScanCommandTokenItem :
        ScanCommandTokenItem,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;

        public InlinedScanCommandTokenItem(IScanCommandTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(InliningCore.Inline(source.SearchTarget, sourceRoot, root, oldNewLookup), source.SeekPast, source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;
        }

        public IScanCommandTokenItem Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public ITokenEntry SourceRoot { get; private set; }

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
                    this.state.HandleRepeatCycle<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource, RegularLanguageNFARootState, IInlinedTokenItem>(this, InliningCore.TokenRootStateClonerCache, InliningCore.TokenStateClonerCache);
                }
                return this.state;
            }
        }


        private RegularLanguageNFAState BuildNFAState()
        {
            if (state == null)
            {
                var target = (InlinedTokenExpressionSeries)this.SearchTarget;
                RegularLanguageNFAState current = null;
                Stack<RegularLanguageNFAState> states = new Stack<RegularLanguageNFAState>(new RegularLanguageNFAState[] { target.State });
                List<RegularLanguageNFAState> covered = new List<RegularLanguageNFAState>();
                //Step through the sequence until it's finished with the all states
                //associated to the scan operation.
                while (states.Count > 0)
                {
                    current = states.Pop();
                    if (covered.Contains(current))
                        continue;
                    covered.Add(current);
                    RegularLanguageSet currentSet = null;
                    foreach (var transition in current.OutTransitions.Keys)
                        if (currentSet == null)
                            currentSet = transition;
                        else
                            currentSet |= transition;
                    if (currentSet != null)
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
                    fState.SetIntermediate(this);
                state.SetInitial(this);
                foreach (var edge in State.ObtainEdges())
                    edge.SetFinal(this);
            }
            return state;
        }

        #endregion
    }
}
