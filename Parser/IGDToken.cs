using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    /// <summary>
    /// Defines properties and methods for working with a token extracted from a grammar
    /// description file.
    /// </summary>
    public interface IGDToken :
        IToken
    {
        /// <summary>
        /// Returns the <see cref="GrammarTokenType"/> relative to the current
        /// <see cref="IGDToken"/> implementation.
        /// </summary>
        GDTokenType TokenType { get; }
    }
}
