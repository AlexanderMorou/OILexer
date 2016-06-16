using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface IProductionRuleGroupItem :
        IProductionRuleSeries,
        IProductionRuleItem
    {
        /// <summary>
        /// Creates a copy of the current <see cref="IProductionRuleGroupItem"/>.
        /// </summary>
        /// <returns>A new <see cref="IProductionRuleGroupItem"/> with the data
        /// members of the current <see cref="IProductionRuleGroupItem"/>.</returns>
        new IProductionRuleGroupItem Clone();
    }
}
