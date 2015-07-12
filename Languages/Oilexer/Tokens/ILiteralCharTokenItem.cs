using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="System.Char"/> literal 
    /// defined in an <see cref="IOilexerGrammarTokenEntry"/>.
    /// </summary>
    public interface ILiteralCharTokenItem :
        ILiteralTokenItem<char>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralCharTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralCharTokenItem"/> with the data
        /// members of the current <see cref="ILiteralCharTokenItem"/>.</returns>
        new ILiteralCharTokenItem Clone();
        /// <summary>
        /// Returns whether the <see cref="ILiteralCharTokenItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        bool CaseInsensitive { get; }
    }
}
