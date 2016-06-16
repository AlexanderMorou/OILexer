using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Defines properties and methods for working with a grouped expression series inside
    /// a <see cref="IOilexerGrammarTokenEntry"/>.
    /// </summary>
    public interface ITokenGroupItem :
        ITokenExpressionSeries,
        ITokenItem
    {
        /// <summary>
        /// Returns the line the <see cref="IGroupTokenItem"/> was declared at.
        /// </summary>
        new int Line { get; }
        /// <summary>
        /// Returns the column in the <see cref="Line"/> the <see cref="IGroupTokenItem"/>
        /// was declared at.
        /// </summary>
        new int Column { get; }
        /// <summary>
        /// Returns the index in the original stream the <see cref="IGroupTokenItem"/> was declared at.
        /// </summary>
        new long Position { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ITokenGroupItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITokenGroupItem"/> with the data
        /// members of the current <see cref="ITokenGroupItem"/>.</returns>
        new ITokenGroupItem Clone();
    }
}
