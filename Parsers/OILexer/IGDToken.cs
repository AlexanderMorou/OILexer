using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright � 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
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
