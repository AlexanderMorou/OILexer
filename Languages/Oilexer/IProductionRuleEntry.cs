using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="IEntry"/> production rule.
    /// Used to express a part of syntax for a <see cref="IGDFile"/>.
    /// </summary>
    public interface IProductionRuleEntry :
        IProductionRuleSeries,
        IScannableEntry
    {
        /// <summary>
        /// Returns/sets whether the elements of 
        /// the <see cref="IProductionRuleEntry"/>
        /// inherit the name of the 
        /// <see cref="IProductionRuleEntry"/>.
        /// </summary>
        bool ElementsAreChildren { get; set; }
    }
}
