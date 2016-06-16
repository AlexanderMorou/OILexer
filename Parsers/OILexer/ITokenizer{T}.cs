using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    public interface ITokenizer<T> :
        ITokenizer
        where T :
            IToken
    {
        /// <summary>
        /// Returns the last parsed token by the <see cref="ITokenizer{T}"/>.
        /// </summary>
        new T CurrentToken { get; }
        
    }
}
