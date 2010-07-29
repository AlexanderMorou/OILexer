using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for a soft reference whose validity is
    /// unconfirmed.
    /// </summary>
    public interface ISoftReferenceProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the name of the target the <see cref="ISoftReferenceProductionRuleItem"/> refers
        /// to.
        /// </summary>
        string PrimaryName { get; }
        /// <summary>
        /// Returns the name of the member in the target (<see cref="PrimaryName"/>).
        /// </summary>
        string SecondaryName { get; }
        bool IsFlag { get; }
        bool Counter { get; }
        GDTokens.IdentifierToken PrimaryToken { get; }
        GDTokens.IdentifierToken SecondaryToken { get;}

    }
}
