using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IRuleEntryObjectRelationalMap :
        IEntryObjectRelationalMap
    {
        new IReadOnlyCollection<IProductionRuleEntry> Implements { get; }
        /// <summary>
        /// Obtains an enumerable state machine which yields the sequences of 
        /// variations of which the <see cref="IRuleEntryObjectRelationalMap"/>'s
        /// <see cref="Entry"/> can be.
        /// </summary>
        new IEnumerable<IEnumerable<IProductionRuleEntry>> Variations { get; }
        /// <summary>
        /// The <see cref="IProductionRuleEntry"/> which is represented by the <see cref="IRuleEntryObjectRelationalMap"/>.
        /// </summary>
        new IProductionRuleEntry Entry { get; }
    }
}
