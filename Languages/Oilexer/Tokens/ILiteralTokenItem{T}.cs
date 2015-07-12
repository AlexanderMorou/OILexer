using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Defines properties and methods for working with a generic <see cref="ILiteralTokenItem"/>
    /// of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value represented by the constant.</typeparam>
    public interface ILiteralTokenItem<T>  :
        ILiteralTokenItem
    {
        /// <summary>
        /// Returns the value defined by the <see cref="ILiteralTokenItem{T}"/>.
        /// </summary>
        new T Value { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralTokenItem{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralTokenItem{T}"/> with the data
        /// members of the current <see cref="ILiteralTokenItem{T}"/>.</returns>
        new ILiteralTokenItem<T> Clone();

        OilexerGrammarTokens.IdentifierToken IsFlagToken { get; }
    }
}
