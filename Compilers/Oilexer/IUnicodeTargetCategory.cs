using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal interface IUnicodeTargetCategory :
        IEquatable<IUnicodeTargetCategory>
    {
        /// <summary>
        /// Returns the <see cref="UnicodeCategory"/> 
        /// referred to by the <see cref="IUnicodeTargetCategory"/>.
        /// </summary>
        UnicodeCategory TargetedCategory { get; }
    }
}
