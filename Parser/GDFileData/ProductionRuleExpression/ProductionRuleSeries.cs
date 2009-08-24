using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public class ProductionRuleSeries :
        ReadOnlyCollection<IProductionRule>,
        IProductionRuleSeries
    {
        public ProductionRuleSeries(ICollection<IProductionRule> series)
        {
            foreach (IProductionRule ipr in series)
                this.baseCollection.Add(ipr);
        }

        public ProductionRuleSeries(ICollection<IProductionRuleSeries> serii)
        {
            foreach (IProductionRuleSeries iprs in serii)
                foreach (IProductionRule ipr in iprs)
                    this.baseCollection.Add(ipr);
        }

    }
}
