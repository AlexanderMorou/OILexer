using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface IProductionRuleSeries :
        IControlledCollection<IProductionRule>,
        IProductionRuleSource
    {
        /// <summary>
        /// Obtains the string form of the body of the 
        /// <see cref="IProductionRuleSeries"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> representing
        /// the elements within the description of the
        /// <see cref="IProductionRuleSeries"/>.</returns>
        string GetBodyString();
    }
}
