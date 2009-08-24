using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="IProductionRuleItem"/> that 
    /// references a <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ITokenReferenceProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which the <see cref="ITokenReferenceProductionRuleItem"/> 
        /// relates to.
        /// </summary>
        ITokenEntry Reference { get; }
    }
}
