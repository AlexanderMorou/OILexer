using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.FiniteAutomata.Tokens;
using Oilexer.FiniteAutomata;

namespace Oilexer._Internal.Inlining
{
    internal class InlinedSubtractionCommandTokenItem :
        SubtractionCommandTokenItem,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;
        public InlinedSubtractionCommandTokenItem(ISubtractionCommandTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(InliningCore.Inline(source.Left, sourceRoot, root, oldNewLookup),InliningCore.Inline(source.Right, sourceRoot, root, oldNewLookup), source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;
        }

        public ISubtractionCommandTokenItem Source { get; private set; }

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
            RegularLanguageNFAState left = ((InlinedTokenExpressionSeries)this.Left).State;
            RegularLanguageNFAState right = ((InlinedTokenExpressionSeries)this.Right).State;
            var result = this.Subtract(left, right);
            List<RegularLanguageNFAState> flatline = new List<RegularLanguageNFAState>();
            RegularLanguageNFAState.FlatlineState(result, flatline);
            foreach (var fState in flatline)
                fState.SetIntermediate(this);
            result.SetInitial(this);
            foreach (var edge in result.ObtainEdges())
                edge.SetFinal(this);
            return result;
        }

        private RegularLanguageNFAState Subtract(RegularLanguageNFAState left, RegularLanguageNFAState right)
        {
            /* *
             * Include the right-element where the left overlaps it;
             * however, every other point of the right-automation
             * is ignored.
             * * 
             * Where the left and right automation are at an edge
             * point, the automation does not yield an edge due to 
             * the exclusive nature of the operation.
             * */
            RegularLanguageNFAState result = new RegularLanguageNFAState();
            if (left.IsEdge)
            {
                if (right.IsEdge)
                    result.ForcedNoEdge = true;
                else
                    result.IsEdge = true;
            }
            foreach (var transition in left.OutTransitions.Keys)
            {
                IDictionary<RegularLanguageSet, IFiniteAutomataTransitionNode<RegularLanguageSet, List<RegularLanguageNFAState>>> colliders;
                RegularLanguageSet remainder = right.OutTransitions.GetColliders(transition, out colliders);
                if (!remainder.IsEmpty)
                    foreach (var target in left.OutTransitions[transition])
                        result.MoveTo(remainder, target);
                foreach (var collision in colliders.Keys)
                    foreach (var leftTarget in left.OutTransitions[transition])
                        foreach (var rightTarget in colliders[collision].Target)
                            result.MoveTo(collision, Subtract(leftTarget,rightTarget));
            }
            return result;
        }

        #endregion
    }
}
