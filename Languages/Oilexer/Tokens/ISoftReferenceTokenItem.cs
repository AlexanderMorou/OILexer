using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
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
