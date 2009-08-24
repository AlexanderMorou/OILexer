using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
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
