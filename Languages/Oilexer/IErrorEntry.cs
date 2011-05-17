using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with an error entry which denotes information
    /// about a grammar-defined error and its constant number.
    /// </summary>
    public interface IErrorEntry :
        INamedEntry
    {
        /// <summary>
        /// Returns the message associated with the entry.
        /// </summary>
        string Message { get; }
        /// <summary>
        /// Returns the error number associated with the <see cref="IErrorEntry"/>.
        /// </summary>
        int Number { get; }
    }
}
