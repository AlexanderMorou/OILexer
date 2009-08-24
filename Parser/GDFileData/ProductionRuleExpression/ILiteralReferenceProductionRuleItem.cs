using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{

    public interface ILiteralReferenceProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the source of the literal that the <see cref="ILiteralReferenceProductionRuleItem"/>
        /// relates to.
        /// </summary>
        ITokenEntry Source { get; }
        /// <summary>
        /// Returns the literal the <see cref="ILiteralReferenceProductionRuleItem"/> references.
        /// </summary>
        ILiteralTokenItem Literal { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralReferenceProductionRuleItem"/>.</returns>
        new ILiteralReferenceProductionRuleItem Clone();
        /// <summary>
        /// Returns whether the literal the <see cref="ILiteralReferenceProductionRuleItem"/>
        /// points to was originally a flag.
        /// </summary>
        bool IsFlag { get; }
        /// <summary>
        /// Returns whether the original <see cref="ILiteralProductionRuleItem"/> 
        /// was a counter.
        /// </summary>
        bool Counter { get; }
    }
}
