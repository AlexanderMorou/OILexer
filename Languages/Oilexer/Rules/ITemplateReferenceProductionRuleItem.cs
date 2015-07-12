
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a production rule item which
    /// references a template and provides the paramteters to the template reference.
    /// </summary>
    public interface ITemplateReferenceProductionRuleItem :
        IProductionRuleItem,
        IControlledCollection<IProductionRuleSeries>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ITemplateReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITemplateReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ITemplateReferenceProductionRuleItem"/>.</returns>
        new ITemplateReferenceProductionRuleItem Clone();
        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleTemplateEntry"/> which the 
        /// <see cref="ITemplateReferenceProductionRuleItem"/> references.
        /// </summary>
        IOilexerGrammarProductionRuleTemplateEntry Reference { get; }
    }
}
