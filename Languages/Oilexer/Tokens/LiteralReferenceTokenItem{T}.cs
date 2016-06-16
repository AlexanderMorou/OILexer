using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public abstract class LiteralReferenceTokenItem<TValue, TLiteral> :
        TokenItem,
        ILiteralReferenceTokenItem<TValue, TLiteral>
        where TLiteral :
            ILiteralTokenItem<TValue>
    {
        private TLiteral reference;
        private IOilexerGrammarTokenEntry entryReference;

        public LiteralReferenceTokenItem(IOilexerGrammarTokenEntry entryReference, TLiteral reference, int column, int line, long position)
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

        public IOilexerGrammarTokenEntry Source
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
