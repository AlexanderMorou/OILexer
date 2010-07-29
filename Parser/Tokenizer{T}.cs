using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Oilexer.Parser
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
