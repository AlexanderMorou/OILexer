using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    public class TokenStream<T> :
        ControlledCollection<T>,
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
