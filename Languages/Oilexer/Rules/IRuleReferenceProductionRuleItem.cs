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
    /// Defines properties and methods for working with a <see cref="IProductionRuleItem"/>
    /// which references a <see cref="IProductionRuleEntry"/>.
    /// </summary>
    public interface IRuleReferenceProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the <see cref="IProductionRuleEntry"/> which the 
        /// <see cref="IRuleReferenceProductionRuleItem"/> references.
        /// </summary>
        IProductionRuleEntry Reference { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="IRuleReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="IRuleReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="IRuleReferenceProductionRuleItem"/>.</returns>
        new IRuleReferenceProductionRuleItem Clone();
    }
}
