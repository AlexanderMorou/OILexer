using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="System.String"/>
    /// literal defined in an <see cref="IProductionRuleEntry"/>.
    /// </summary>
    public interface ILiteralStringProductionRuleItem :
        ILiteralProductionRuleItem<string>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralStringProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralStringProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralStringProductionRuleItem"/>.</returns>
        new ILiteralStringProductionRuleItem Clone();
        /// <summary>
        /// Returns whether the <see cref="ILiteralStringProductionRuleItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        bool CaseInsensitive { get; }
    }
}
