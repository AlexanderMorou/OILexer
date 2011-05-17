using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Defines properties and methods for working with an entry contained
    /// within a preprocessor whose inclusion is determined upon evaluating
    /// the condition of the preprocessor in which it resides.
    /// </summary>
    public interface IPreprocessorEntryContainer :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the <see cref="IEntry"/> contained by the entry
        /// container.
        /// </summary>
        IEntry Contained { get; }
    }
}
