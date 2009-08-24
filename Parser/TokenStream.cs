using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser
{
    public class TokenStream :
        ControlledStateCollection<IToken>,
        ITokenStream
    {
        public TokenStream(IToken[] tokens)
        {
            foreach (IToken t in tokens)
                this.baseCollection.Add(t);
        }
        internal TokenStream(ICollection<IToken> baseCopy)
            : base(baseCopy)
        {
        }

        internal T get_Item<T>(int index)
        {
            return (T)this[index];
        }
    }
}
