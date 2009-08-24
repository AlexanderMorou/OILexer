using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for working with a soft reference to an
    /// <see cref="IProductionRuleTemplateEntry"/>.
    /// </summary>
    public interface ISoftTemplateReferenceProductionRuleItem :
        ISoftReferenceProductionRuleItem
    {
        /// <summary>
        /// Returns the parts to suppliment the template's parts.
        /// </summary>
        IReadOnlyCollection<IProductionRuleSeries> Parts { get; } 
    }
}
