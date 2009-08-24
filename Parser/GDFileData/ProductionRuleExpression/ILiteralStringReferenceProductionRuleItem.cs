using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="ILiteralStringReferenceProductionRuleItem"/>
    /// </summary>
    public interface ILiteralStringReferenceProductionRuleItem :
        ILiteralReferenceProductionRuleItem<string, ILiteralStringTokenItem>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralStringReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralStringReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralStringReferenceProductionRuleItem"/>.</returns>
        new ILiteralStringReferenceProductionRuleItem Clone();
    }
}
