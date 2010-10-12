using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData
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
