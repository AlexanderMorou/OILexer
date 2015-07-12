using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal interface IUnicodeTargetPartialCategory :
        IUnicodeTargetCategory,
        IEquatable<IUnicodeTargetPartialCategory>
    {
        /// <summary>
        /// Returns the <see cref="RegularLanguageSet"/> which represents
        /// the negative-assertion to exclude when checking for the category.
        /// </summary>
        RegularLanguageSet NegativeAssertion { get; }
    }
}
