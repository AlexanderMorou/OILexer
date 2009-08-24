using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for working with a reference to a
    /// literal character from a <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ILiteralCharReferenceProductionRuleItem :
        ILiteralReferenceProductionRuleItem<char, ILiteralCharTokenItem>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralCharReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralCharReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralCharReferenceProductionRuleItem"/>.</returns>
        new ILiteralCharReferenceProductionRuleItem Clone();
    }
}
