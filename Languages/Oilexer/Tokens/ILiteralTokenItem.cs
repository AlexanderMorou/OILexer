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
    /// Defines a constant used in a <see cref="IOilexerGrammarTokenEntry"/>.
    /// </summary>
    public interface ILiteralTokenItem :
        ITokenItem
    {
        /// <summary>
        /// Returns the value defined by the <see cref="ILiteralTokenItem"/>.
        /// </summary>
        object Value { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralTokenItem"/> with the data
        /// members of the current <see cref="ILiteralTokenItem"/>.</returns>
        new ILiteralTokenItem Clone();
        /// <summary>
        /// Returns whether the captured literal is a flag.  That is, if captured successfully,
        /// is stored as a boolean value only.  Only applies to optional members.
        /// </summary>
        bool? IsFlag { get; }
    }
}
