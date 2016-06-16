using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a unicode graph
    /// </summary>
    internal interface IUnicodeTargetGraph :
        IControlledDictionary<RegularLanguageDFAState, IUnicodeTarget>,
        IEquatable<IUnicodeTargetGraph>
    {
        /// <summary>
        /// Inserts and returns a new <see cref="IUnicodeTarget"/>
        /// implementation with the <paramref name="target"/> specified.
        /// </summary>
        /// <param name="target">The <see cref="RegularLanguageDFAState"/>
        /// to which the <see cref="IUnicodeTarget"/> is directed towards.</param>
        /// <param name="targetIsOrigin">Whether the source <see cref="RegularLanguageState"/>
        /// that is calling for the graph target, is the same as the <paramref name="target"/> described.</param>
        /// <returns>A new <see cref="IUnicodeTarget"/> implementation
        /// with the <paramref name="target"/> specified.</returns>
        IUnicodeTarget Add(RegularLanguageDFAState target, bool targetIsOrigin);
        bool Equals(IUnicodeTargetGraph other, bool relaxOriginatingState);
    }
}
