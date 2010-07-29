﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer._Internal;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
namespace Oilexer.VSIntegration
{
    internal class GDResolutionAssistant :
        _GDResolutionAssistant
    {
        internal GDFileHandlerBase handler;

        internal GDResolutionAssistant(GDFileHandlerBase handler)
        {
            this.handler = handler;
        }

        #region _GDResolutionAssistant Members

        public void ResolvedSinglePartToRule(ISoftReferenceProductionRuleItem item, IProductionRuleEntry primary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
        }

        public void ResolvedSinglePartToToken(ISoftReferenceProductionRuleItem item, ITokenEntry primary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
        }

        public void ResolvedSinglePartToToken(ISoftReferenceTokenItem item, ITokenEntry primary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
        }

        public void ResolvedDualPartToTokenItem(ISoftReferenceProductionRuleItem item, ITokenEntry primary, ITokenItem secondary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
            if (item.SecondaryToken != null)
                this.handler.ReclassifyToken(item.SecondaryToken, secondary);
        }

        public void ResolvedDualPartToTokenItem(ISoftReferenceTokenItem item, ITokenEntry primary, ITokenItem secondary)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
            if (item.SecondaryToken != null)
                this.handler.ReclassifyToken(item.SecondaryToken, secondary);
        }

        public void ResolvedSinglePartToTemplateParameter(IProductionRuleTemplateEntry entry, IProductionRuleTemplatePart iprtp, ISoftReferenceProductionRuleItem item)
        {
            if (item.PrimaryToken != null)
                this.handler.ReclassifyToken(item.PrimaryToken, iprtp);
        }

        public void ResolvedSinglePartToRuleTemplate(ISoftTemplateReferenceProductionRuleItem item, IProductionRuleTemplateEntry primary)
        {
            if (item.PrimaryToken != null)
            {
                this.handler.ReclassifyToken(item.PrimaryToken, primary);
            }
        }
        #endregion
    }
}