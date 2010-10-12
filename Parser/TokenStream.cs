using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser
{
    public class TokenStream<T> :
        ControlledStateCollection<T>,
        ITokenStream<T>
        where T :
            IToken
    {
        public TokenStream(T[] tokens)
        {
            foreach (T t in tokens)
                this.baseList.Add(t);
        }
        internal TokenStream(IList<T> baseCopy)
            : base(baseCopy)
        {
        }

    }
}
