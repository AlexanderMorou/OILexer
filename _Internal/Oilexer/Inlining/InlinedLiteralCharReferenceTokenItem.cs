using System;
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
    internal class InlinedLiteralCharReferenceTokenItem :
        LiteralCharTokenItem,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;
        public InlinedLiteralCharReferenceTokenItem(ILiteralCharReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root)
            : base(source.Literal.Value, source.Literal.CaseInsensitive, source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;
        }

        public ILiteralCharReferenceTokenItem Source { get; private set; }

        public ITokenEntry SourceRoot { get; private set; }

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

        #endregion

        private RegularLanguageNFAState BuildNFAState()
        {
            RegularLanguageNFAState rootState = new RegularLanguageNFAState();
            RegularLanguageNFAState endState = new RegularLanguageNFAState();
            rootState.MoveTo(new RegularLanguageSet(!this.CaseInsensitive, this.Value), endState);
            rootState.SetInitial(this);
            endState.SetFinal(this);
            return rootState;
        }

        public override string ToString()
        {
            return this.Source.ToString();
        }

    }
}
