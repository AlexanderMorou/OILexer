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
    internal class InlinedLiteralStringReferenceTokenItem :
        LiteralStringTokenItem,
        IInlinedTokenItem
    {

        public InlinedLiteralStringReferenceTokenItem(ILiteralStringReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root)
            : base(source.Literal.Value, source.Literal.CaseInsensitive, source.Column, source.Line, source.Position, source.Literal.SiblingAmbiguity)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            if (!string.IsNullOrEmpty(source.Name))
                this.Name = source.Name;
            else
                this.Name = source.Literal.Name;
        }

        public ILiteralStringReferenceTokenItem Source { get; private set; }

        public ITokenEntry SourceRoot { get; private set; }

        public InlinedTokenEntry Root { get; private set; }

        #region IInlinedTokenItem Members

        ITokenItem IInlinedTokenItem.Source
        {
            get { return this.Source; }
        }

        public RegularLanguageNFAState State
        {
            get {
                return this.BuildStringState(this.CaseInsensitive, this.Value);
            }
        }
        #endregion

        public override string ToString()
        {
            return this.Source.ToString();
        }


    }
}
