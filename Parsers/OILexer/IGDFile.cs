using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Cst;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a grammar description file.
    /// </summary>
    public interface IGDFile :
        ICollection<IEntry>,
        IConcreteNode
    {
        /// <summary>
        /// Returns the <see cref="IReadOnlyCollection"/> of files that the <see cref="IGDFile"/>
        /// was created from.
        /// </summary>
        IReadOnlyCollection<string> Files { get; }
        /// <summary>
        /// Returns the <see cref="IGDFileOptions"/> which determines the options related to the 
        /// resulted generation process.
        /// </summary>
        IGDFileOptions Options { get; }

        IList<string> Includes { get; }

        IReadOnlyCollection<IGDRegion> Regions { get; }
    }
}
