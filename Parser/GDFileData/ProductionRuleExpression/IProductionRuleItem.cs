using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for working with an expression item defined 
    /// in a <see cref="IProductionRuleEntry"/>.
    /// </summary>
    public interface IProductionRuleItem :
        IScannableEntryItem
    {
        /// <summary>
        /// Creates a copy of the current <see cref="IProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="IProductionRuleItem"/> with the data
        /// members of the current <see cref="IProductionRuleItem"/>.</returns>
        new IProductionRuleItem Clone();
    }
}
