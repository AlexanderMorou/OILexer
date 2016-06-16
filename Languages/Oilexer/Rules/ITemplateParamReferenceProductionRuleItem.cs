using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
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
        /// Returns the <see cref="IOilexerGrammarProductionRuleTemplateEntry"/> which the contains the 
        /// <see cref="Reference"/> the <see cref="ITemplateParamReferenceProductionRuleItem"/> references.
        /// </summary>
        IOilexerGrammarProductionRuleTemplateEntry Source { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ITemplateParamReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITemplateParamReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ITemplateParamReferenceProductionRuleItem"/>.</returns>
        new ITemplateParamReferenceProductionRuleItem Clone();
    }
}
