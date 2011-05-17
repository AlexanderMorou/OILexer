using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IAmbiguousGDEntity
    {
        /// <summary>
        /// Disambiguifies the <see cref="IAmbiguousEntity"/>.
        /// </summary>
        /// <param name="context">The <see cref="IGDFile"/> used to clear up the ambiguity.</param>
        void Disambiguify(IGDFile context, IEntry root);
    }
}
