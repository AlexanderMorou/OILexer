using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal interface IUnicodeTarget :
        IControlledDictionary<UnicodeCategory, IUnicodeTargetCategory>,
        IEquatable<IUnicodeTarget>
    {
        bool TargetIsOrigin { get; }
        /// <summary>
        /// Returns the <see cref="RegularLanguageDFAState"/> associated to the
        /// unicode target.
        /// </summary>
        RegularLanguageDFAState Target { get; }
        /// <summary>
        /// Inserts and returns a new <see cref="IUnicodeTargetCategory"/>
        /// implementation which refers to the <paramref name="category"/>
        /// provided.
        /// </summary>
        /// <param name="category">The <see cref="UnicodeCategory"/>
        /// to which the <see cref="IUnicodeTarget"/>
        /// is constrained by.</param>
        /// <returns>A new <see cref="IUnicodeTargetCategory"/> implementation
        /// which refers to the <paramref name="category"/>
        /// provided.</returns>
        IUnicodeTargetCategory Add(UnicodeCategory category);
        /// <summary>
        /// Inserts and returns a new <see cref="IUnicodeTargetPartialCategory"/>
        /// implementation which refers, partially, to the <paramref name="category"/>
        /// provided, with exception to the <paramref name="negativeAssertion"/>
        /// set.
        /// </summary>
        /// <param name="category">The <see cref="UnicodeCategory"/>
        /// to which the <see cref="IUnicodeTarget"/>
        /// is constrained by.</param>
        /// <param name="negativeAssertion">The <see cref="RegularLanguageSet"/>
        /// the <see cref="IUnicodeTarget"/> does not contain of the original
        /// <paramref name="category"/> provided.</param>
        /// <returns>A new <see cref="IUnicodeTargetPartialCategory"/>
        /// implementation which refers, partially, to the <paramref name="category"/>
        /// provided, with exception to the <paramref name="negativeAssertion"/>
        /// set.</returns>
        IUnicodeTargetPartialCategory Add(UnicodeCategory category, RegularLanguageSet negativeAssertion);
    }
}
