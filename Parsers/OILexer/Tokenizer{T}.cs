using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    public abstract class Tokenizer<T> :
        Tokenizer,
        ITokenizer<T>
        where T :
            IToken
    {
        protected Tokenizer(Stream stream, string fileName)
            : base(stream, fileName)
        {

        }
        internal Tokenizer()
            : base()
        {
        }

        #region ITokenizer<T> Members

        public new T CurrentToken
        {
            get { return (T)base.CurrentToken; }
        }

        #endregion
    }
}
