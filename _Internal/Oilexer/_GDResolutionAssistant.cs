using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
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
