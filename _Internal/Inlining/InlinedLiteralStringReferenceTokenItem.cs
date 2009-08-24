using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Inlining
{
    internal class InlinedLiteralStringReferenceTokenItem :
        LiteralStringTokenItem,
        IInlinedTokenItem
    {

        public InlinedLiteralStringReferenceTokenItem(ILiteralStringReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root)
            : base(source.Literal.Value, source.Literal.CaseInsensitive, source.Column, source.Line, source.Position)
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

        #endregion

        public override string ToString()
        {
            return this.Source.ToString();
        }

    }
}
