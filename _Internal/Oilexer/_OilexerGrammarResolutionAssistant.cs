using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    internal interface _OilexerGrammarResolutionAssistant
    {
        void ResolvedSinglePartToRule(ISoftReferenceProductionRuleItem item, IOilexerGrammarProductionRuleEntry primary);
        void ResolvedSinglePartToToken(ISoftReferenceProductionRuleItem item, IOilexerGrammarTokenEntry primary);
        void ResolvedSinglePartToToken(ISoftReferenceTokenItem item, IOilexerGrammarTokenEntry primary);
        void ResolvedDualPartToTokenItem(ISoftReferenceProductionRuleItem item, IOilexerGrammarTokenEntry primary, ITokenItem secondary);
        void ResolvedDualPartToTokenItem(ISoftReferenceTokenItem item, IOilexerGrammarTokenEntry primary, ITokenItem secondary);
        void ResolvedSinglePartToTemplateParameter(IOilexerGrammarProductionRuleTemplateEntry entry, IProductionRuleTemplatePart primaryTarget, ISoftReferenceProductionRuleItem primary);
        void ResolvedSinglePartToRuleTemplate(ISoftTemplateReferenceProductionRuleItem item, IOilexerGrammarProductionRuleTemplateEntry primary);
    }
}
