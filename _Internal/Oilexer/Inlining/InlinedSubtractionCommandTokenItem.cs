using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    internal class InlinedSubtractionCommandTokenItem :
        SubtractionCommandTokenItem,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;
        public InlinedSubtractionCommandTokenItem(ISubtractionCommandTokenItem source, IOilexerGrammarTokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(OilexerGrammarInliningCore.Inline(source.Left, sourceRoot, root, oldNewLookup),OilexerGrammarInliningCore.Inline(source.Right, sourceRoot, root, oldNewLookup), source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;
            this.SiblingAmbiguity = source.SiblingAmbiguity;
        }

        public ISubtractionCommandTokenItem Source { get; private set; }

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
            get
            {
                return this.state;
            }
        }

        public void BuildState(Dictionary<ITokenSource, Captures.ICaptureTokenStructuralItem> sourceReplacementLookup)
        {
            var left = ((InlinedTokenExpressionSeries)this.Left);
            var right = ((InlinedTokenExpressionSeries)this.Right);
            var thisReplacement = sourceReplacementLookup.ContainsKey(this) ? (ITokenSource)(sourceReplacementLookup[this]) : (ITokenSource)this;
            left.BuildState(sourceReplacementLookup);
            right.BuildState(sourceReplacementLookup);
            var result = this.Subtract(left.State, right.State);
            List<RegularLanguageNFAState> flatline = new List<RegularLanguageNFAState>();
            RegularLanguageNFAState.FlatlineState(result, flatline);
            foreach (var fState in flatline)
                fState.SetIntermediate(thisReplacement);
            result.SetInitial(thisReplacement);
            foreach (var edge in result.ObtainEdges())
                edge.SetFinal(thisReplacement);
            result.HandleRepeatCycle<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource, RegularLanguageNFARootState, IInlinedTokenItem>(this, thisReplacement, OilexerGrammarInliningCore.TokenRootStateClonerCache, OilexerGrammarInliningCore.TokenStateClonerCache);
            this.state = result;
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
                            result.MoveTo(collision, Subtract(leftTarget, rightTarget));
            }
            return result;
        }

        #endregion
    }
}
