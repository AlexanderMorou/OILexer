using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a common-ground named entry in a
    /// grammar description file.
    /// </summary>
    public interface IOilexerGrammarNamedEntry :
        IOilexerGrammarEntry
    {
        /// <summary>
        /// Returns the name of the <see cref="IOilexerGrammarNamedEntry"/>.
        /// </summary>
        string Name { get; }
    }
}
