using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="System.Char"/> literal 
    /// defined in an <see cref="IProductionRuleEntry"/>.
    /// </summary>
    public interface ILiteralCharProductionRuleItem :
        ILiteralProductionRuleItem<char>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralCharProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralCharProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralCharProductionRuleItem"/>.</returns>
        new ILiteralCharProductionRuleItem Clone();
        /// <summary>
        /// Returns whether the <see cref="ILiteralCharProductionRuleItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        bool CaseInsensitive { get; }
    }
}
