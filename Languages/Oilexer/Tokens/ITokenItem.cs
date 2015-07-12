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
    /// Defines properties and methods for working with an expression item defined 
    /// in a <see cref="IOilexerGrammarTokenEntry"/>.
    /// </summary>
    public interface ITokenItem :
        IScannableEntryItem,
        INamedTokenSource
    {
        /// <summary>
        /// Returns the name of the <see cref="ITokenItem"/>, if it was defined.
        /// </summary>
        /// <remarks>Can be null.</remarks>
        new string Name { get; set; }
        /// <summary>
        /// Creates a copy of the current <see cref="ITokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITokenItem"/> with the data
        /// members of the current <see cref="ITokenItem"/>.</returns>
        new ITokenItem Clone();
        /// <summary>
        /// Returns the default value associated with the <see cref="ITokenItem"/>.
        /// </summary>
        /// <remarks>
        /// If the <see cref="ITokenItem"/> is a literal, this refers to the default string value.
        /// If the <see cref="ITokenItem"/> is a <see cref="ITokenGroupItem"/>, the value
        /// refers to a soft-reference of a member contained within a simple literal series.
        /// </remarks>
        string DefaultSoftRefOrValue { get; }
    }
}
