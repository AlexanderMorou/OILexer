using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public interface ILiteralProductionRuleItem<T>  :
        ILiteralProductionRuleItem
    {
        /// <summary>
        /// Returns the value defined by the <see cref="ILiteralProductionRuleItem{T}"/>.
        /// </summary>
        new T Value { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralProductionRuleItem{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralProductionRuleItem{T}"/> with the data
        /// members of the current <see cref="ILiteralProductionRuleItem{T}"/>.</returns>
        new ILiteralProductionRuleItem<T> Clone();
    }
}
