using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer._Internal
{
    internal interface _GDResolutionAssistant
    {
        void ResolvedSinglePartToRule(ISoftReferenceProductionRuleItem item, IProductionRuleEntry primary);
        void ResolvedSinglePartToToken(ISoftReferenceProductionRuleItem item, ITokenEntry primary);
        void ResolvedSinglePartToToken(ISoftReferenceTokenItem item, ITokenEntry primary);
        void ResolvedDualPartToTokenItem(ISoftReferenceProductionRuleItem item, ITokenEntry primary, ITokenItem secondary);
        void ResolvedDualPartToTokenItem(ISoftReferenceTokenItem item, ITokenEntry primary, ITokenItem secondary);
        void ResolvedSinglePartToTemplateParameter(IProductionRuleTemplateEntry entry, IProductionRuleTemplatePart primaryTarget, ISoftReferenceProductionRuleItem primary);
        void ResolvedSinglePartToRuleTemplate(ISoftTemplateReferenceProductionRuleItem item, IProductionRuleTemplateEntry primary);
    }
}
