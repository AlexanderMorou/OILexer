using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
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
