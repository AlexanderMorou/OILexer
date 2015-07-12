using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class TokenReferenceTokenItem :
        TokenItem,
        ITokenReferenceTokenItem
    {
        private IOilexerGrammarTokenEntry reference;

        /// <summary>
        /// Creates a new <see cref="TokenReferenceTokenItem"/> with the <paramref name="reference"/>,
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="reference">The <see cref="IOilexerGrammarTokenEntry"/> that the <see cref="ITokenReferenceTokenItem"/>
        /// references.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="TokenReferenceTokenItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="TokenReferenceTokenItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="TokenReferenceTokenItem"/> was declared.</param>
        public TokenReferenceTokenItem(IOilexerGrammarTokenEntry reference, int column, int line, long position)
            : base(column, line, position)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");
            this.reference = reference;
        }

        #region ITokenReferenceTokenItem Members

        public IOilexerGrammarTokenEntry Reference
        {
            get { return this.reference; }
        }

        #endregion

        protected override object OnClone()
        {
            TokenReferenceTokenItem trti = new TokenReferenceTokenItem(this.Reference, this.Column, this.Line, this.Position);
            base.CloneData(trti);
            return trti;
        }
        public override string ToString()
        {
            return string.Format("{0}{1}", this.Reference.Name, base.ToString());
        }
    }
}
