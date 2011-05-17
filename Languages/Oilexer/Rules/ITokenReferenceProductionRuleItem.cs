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

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="IProductionRuleItem"/> that 
    /// references a <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ITokenReferenceProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which the <see cref="ITokenReferenceProductionRuleItem"/> 
        /// relates to.
        /// </summary>
        ITokenEntry Reference { get; }
    }
}
