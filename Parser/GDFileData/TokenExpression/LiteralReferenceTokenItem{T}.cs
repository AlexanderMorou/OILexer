using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public abstract class LiteralReferenceTokenItem<TValue, TLiteral> :
        TokenItem,
        ILiteralReferenceTokenItem<TValue, TLiteral>
        where TLiteral :
            ILiteralTokenItem<TValue>
    {
        private TLiteral reference;
        private ITokenEntry entryReference;

        public LiteralReferenceTokenItem(ITokenEntry entryReference, TLiteral reference, int column, int line, long position)
            : base(column, line, position)
        {
            if (entryReference == null)
                throw new ArgumentNullException("entryReference");
            if (reference == null)
                throw new ArgumentNullException("reference");
            this.entryReference = entryReference;
            this.reference = reference;
        }

        #region ILiteralReferenceTokenItem<TValue,TLiteral> Members

        public TLiteral Literal
        {
            get { return this.reference; }
        }

        #endregion

        #region ILiteralReferenceTokenItem Members

        public ITokenEntry Source
        {
            get { return this.entryReference; }
        }

        ILiteralTokenItem ILiteralReferenceTokenItem.Literal
        {
            get { return this.Literal; }
        }

        #endregion
        public override string ToString()
        {
            return string.Format("{0}.{1}{2}", Source.Name, Literal.Name, base.ToString());
        }
    }
}
