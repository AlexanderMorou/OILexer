using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Defines properties and methods for a soft reference whose validity is
    /// unconfirmed.
    /// </summary>
    public interface ISoftReferenceTokenItem :
        ITokenItem
    {
        /// <summary>
        /// Returns the name of the target the <see cref="ISoftReferenceTokenItem"/> refers
        /// to.
        /// </summary>
        string PrimaryName { get; }
        /// <summary>
        /// Returns the name of the member in the target (<see cref="PrimaryName"/>).
        /// </summary>
        string SecondaryName { get; }
        GDTokens.IdentifierToken PrimaryToken { get; }
        GDTokens.IdentifierToken SecondaryToken { get; }
    }
}
