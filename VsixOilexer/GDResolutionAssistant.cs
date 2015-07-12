using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    internal class GDResolutionAssistant :
        _OilexerGrammarResolutionAssistant
    {
        internal GDFileHandlerBase handler;

        internal GDResolutionAssistant(GDFileHandlerBase handler)
        {
            this.handler = handler;
        }

        #region _OilexerGrammarResolutionAssistant Members

        public void ResolvedSinglePartToRule(ISoftReferenceProductionRuleItem item, IOilexerGrammarProductionRuleEntry primary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
        }

        public void ResolvedSinglePartToToken(ISoftReferenceProductionRuleItem item, IOilexerGrammarTokenEntry primary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
        }

        public void ResolvedSinglePartToToken(ISoftReferenceTokenItem item, IOilexerGrammarTokenEntry primary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
        }

        public void ResolvedDualPartToTokenItem(ISoftReferenceProductionRuleItem item, IOilexerGrammarTokenEntry primary, ITokenItem secondary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
            if (item.SecondaryToken != null)
                this.handler.ReclassifyToken(item.SecondaryToken, secondary);
        }

        public void ResolvedDualPartToTokenItem(ISoftReferenceTokenItem item, IOilexerGrammarTokenEntry primary, ITokenItem secondary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
            if (item.SecondaryToken != null)
                this.handler.ReclassifyToken(item.SecondaryToken, secondary);
        }

        public void ResolvedSinglePartToTemplateParameter(IOilexerGrammarProductionRuleTemplateEntry entry, IProductionRuleTemplatePart iprtp, ISoftReferenceProductionRuleItem item)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, iprtp);
        }

        public void ResolvedSinglePartToRuleTemplate(ISoftTemplateReferenceProductionRuleItem item, IOilexerGrammarProductionRuleTemplateEntry primary)
        {
            if (item.PrimaryToken != null)
            {
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
            }
        }
        #endregion
    }
}
