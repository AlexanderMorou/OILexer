using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright � 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="System.String"/>
    /// literal defined in an <see cref="IOilexerGrammarTokenEntry"/>.
    /// </summary>
    public interface ILiteralStringTokenItem :
        ILiteralTokenItem<string>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralStringTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralStringTokenItem"/> with the data
        /// members of the current <see cref="ILiteralStringTokenItem"/>.</returns>
        new ILiteralStringTokenItem Clone();
        /// <summary>
        /// Returns whether the <see cref="ILiteralStringTokenItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        bool CaseInsensitive { get; }
    }
}
