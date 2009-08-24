using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public interface IProductionRuleTemplatePart :
        IProductionRuleItem
    {
        TemplatePartExpectedSpecial SpecialExpectancy { get; }
        IProductionRuleItem ExpectedSpecific { get; }
    }
}
