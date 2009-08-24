using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public interface ITemplateParamReferenceProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the <see cref="IProductionRuleTemplatePart"/> the <see cref="ITemplateParamReferenceProductionRuleItem"/>
        /// references.
        /// </summary>
        IProductionRuleTemplatePart Reference { get; }
        /// <summary>
        /// Returns the <see cref="IProductionRuleTemplateEntry"/> which the contains the 
        /// <see cref="Reference"/> the <see cref="ITemplateParamReferenceProductionRuleItem"/> references.
        /// </summary>
        IProductionRuleTemplateEntry Source { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ITemplateParamReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITemplateParamReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ITemplateParamReferenceProductionRuleItem"/>.</returns>
        new ITemplateParamReferenceProductionRuleItem Clone();
    }
}
