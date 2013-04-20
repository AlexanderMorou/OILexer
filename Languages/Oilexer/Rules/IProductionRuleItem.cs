using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with an expression item defined 
    /// in a <see cref="IProductionRuleEntry"/>.
    /// </summary>
    public interface IProductionRuleItem :
        IScannableEntryItem,
        IProductionRuleSource
    {
        /// <summary>
        /// Creates a copy of the current <see cref="IProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="IProductionRuleItem"/> with the data
        /// members of the current <see cref="IProductionRuleItem"/>.</returns>
        new IProductionRuleItem Clone();
        IControlledDictionary<string, string> ConditionalConstraints { get; }
    }
}
