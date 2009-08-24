using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public class LiteralCharReferenceTokenItem :
        LiteralReferenceTokenItem<char, ILiteralCharTokenItem>,
        ILiteralCharReferenceTokenItem
    {
        public LiteralCharReferenceTokenItem(ITokenEntry entryReference, ILiteralCharTokenItem reference, int column, int line, long position)
            : base(entryReference, reference, column, line, position)
        {

        }

        protected override object OnClone()
        {
            LiteralCharReferenceTokenItem result = new LiteralCharReferenceTokenItem(this.Source, this.Literal, this.Column, this.Line, this.Position);
            this.CloneData(result);
            return result;
        }
    }
}
