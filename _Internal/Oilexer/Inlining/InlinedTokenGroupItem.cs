﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    internal class InlinedTokenGroupItem :
        TokenGroupItem,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;
        public InlinedTokenGroupItem(ITokenGroupItem source, IOilexerGrammarTokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(source.FileName, OilexerGrammarInliningCore.Inline(source.ToArray(), sourceRoot, root, oldNewLookup), source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;
        }
        public ITokenGroupItem Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public IOilexerGrammarTokenEntry SourceRoot { get; private set; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="InlinedTokenGroupItem"/>.
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

        #endregion

        #region IInlinedTokenItem Members

        public void BuildState(Dictionary<ITokenSource, Captures.ICaptureTokenStructuralItem> sourceReplacementLookup)
        {
            var thisReplacement = sourceReplacementLookup.ContainsKey(this) ? (ITokenSource)(sourceReplacementLookup[this]) : (ITokenSource)this;
            RegularLanguageNFAState state = null;
            foreach (var expression in this.Cast<InlinedTokenExpression>())
            {
                expression.BuildState(sourceReplacementLookup);
                if (state == null)
                    state = expression.NFAState;
                else
                    state.Union(expression.NFAState);
            }
            List<RegularLanguageNFAState> flatline = new List<RegularLanguageNFAState>();
            RegularLanguageNFAState.FlatlineState(state, flatline);
            foreach (var fState in flatline)
                fState.SetIntermediate(thisReplacement);
            state.SetInitial(thisReplacement);
            foreach (var edge in state.ObtainEdges())
                edge.SetFinal(thisReplacement);
            state.HandleRepeatCycle<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource, RegularLanguageNFARootState, IInlinedTokenItem>(this, thisReplacement, OilexerGrammarInliningCore.TokenRootStateClonerCache, OilexerGrammarInliningCore.TokenStateClonerCache);
            this.state = state;
        }

        #endregion

    }
}
