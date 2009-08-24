using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public class LiteralStringReferenceTokenItem :
        LiteralReferenceTokenItem<string, ILiteralStringTokenItem>,
        ILiteralStringReferenceTokenItem
    {
        public LiteralStringReferenceTokenItem(ITokenEntry entryReference, ILiteralStringTokenItem reference, int column, int line, long position)
            : base(entryReference, reference, column, line, position)
        {

        }
        protected override object OnClone()
        {
            LiteralStringReferenceTokenItem result = new LiteralStringReferenceTokenItem(this.Source, this.Literal, this.Column, this.Line, this.Position);
            this.CloneData(result);
            return result;
        }

    }
}
