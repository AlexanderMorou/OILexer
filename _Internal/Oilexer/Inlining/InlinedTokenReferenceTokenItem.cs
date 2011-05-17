using System;
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
    internal class InlinedTokenReferenceTokenItem :
        TokenGroupItem,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;
        public InlinedTokenReferenceTokenItem(ITokenReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(source.Reference.FileName, InliningCore.Inline(source.Reference.Branches.ToArray(), sourceRoot, root, oldNewLookup), source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;
        }

        public ITokenReferenceTokenItem Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public ITokenEntry SourceRoot { get; private set; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="InlinedLiteralCharTokenItem"/>.
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
            RegularLanguageNFAState state = null;
            foreach (var expression in this.Cast<InlinedTokenExpression>())
                if (state == null)
                    state = expression.NFAState;
                else
                    state.Union(expression.NFAState);
            List<RegularLanguageNFAState> flatline = new List<RegularLanguageNFAState>();
            RegularLanguageNFAState.FlatlineState(state, flatline);
            foreach (var fState in flatline)
                fState.SetIntermediate(this);
            state.SetInitial(this);
            foreach (var edge in state.ObtainEdges())
                edge.SetFinal(this);
            return state;
        }

        #endregion


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Source.Reference.Name);
            if (this.Name != null && this.Name != string.Empty)
                sb.Append(string.Format(":{0};{1}", this.Name, this.ToStringFurtherOptions()));
            sb.Append(RepeatOptions.ToString());
            return sb.ToString();
        }

    }
}
