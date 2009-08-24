using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines a constant used in a <see cref="IProductionRuleEntry"/>.
    /// </summary>
    public interface ILiteralProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the value defined by the <see cref="ILiteralProductionRuleItem"/>.
        /// </summary>
        object Value { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralProductionRuleItem"/>.</returns>
        new ILiteralProductionRuleItem Clone();
        /// <summary>
        /// Returns whether the <see cref="ILiteralProductionRuleItem"/> is
        /// a flag.
        /// </summary>
        /// <remarks>Flags only persist as a <see cref="System.Boolean"/>.</remarks>
        bool Flag { get; }
        /// <summary>
        /// Returns whether the <see cref="ILiteralProductionRuleItem"/> is a
        /// counter.
        /// </summary>
        bool Counter { get; }
    }
}
