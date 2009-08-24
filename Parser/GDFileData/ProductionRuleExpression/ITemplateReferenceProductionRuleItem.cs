using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for working with a production rule item which
    /// references a template and provides the paramteters to the template reference.
    /// </summary>
    public interface ITemplateReferenceProductionRuleItem :
        IProductionRuleItem,
        IReadOnlyCollection<IProductionRuleSeries>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ITemplateReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITemplateReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ITemplateReferenceProductionRuleItem"/>.</returns>
        new ITemplateReferenceProductionRuleItem Clone();
        /// <summary>
        /// Returns the <see cref="IProductionRuleTemplateEntry"/> which the 
        /// <see cref="ITemplateReferenceProductionRuleItem"/> references.
        /// </summary>
        IProductionRuleTemplateEntry Reference { get; }
    }
}
