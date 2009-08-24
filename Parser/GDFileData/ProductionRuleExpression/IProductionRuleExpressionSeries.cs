using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public interface IProductionRuleSeries :
        IReadOnlyCollection<IProductionRule>
    {
    }
}
