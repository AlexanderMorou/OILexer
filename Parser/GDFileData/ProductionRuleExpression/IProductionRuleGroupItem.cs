using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
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
