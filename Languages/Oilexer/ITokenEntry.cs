using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface ITokenEntry :
        IScannableEntry,
        ITokenSource
    {
        /// <summary>
        /// Returns the <see cref="ITokenExpressionSeries"/> which defines the branches of
        /// the <see cref="ITokenEntry"/>.
        /// </summary>
        ITokenExpressionSeries Branches { get; }
        /// <summary>
        /// Returns whether the <see cref="ITokenEntry"/> is unhinged.
        /// </summary>
        /// <remarks>Unhinged tokens can appear anywhere regardless
        /// of the context.</remarks>
        bool Unhinged { get; set; }

        /// <summary>
        /// Returns the list of token names that have a lower precedence than the current token.
        /// </summary>
        List<GDTokens.IdentifierToken> LowerPrecedenceNames { get; }
        /// <summary>
        /// Returns the list of token instances that have a lower precedence than the current token.
        /// </summary>
        ITokenEntry[] LowerPrecedenceTokens { get; }
        /// <summary>
        /// Returns whether the <see cref="ITokenEntry"/> is forced into being
        /// a recognizer type token.
        /// </summary>
        bool ForcedRecognizer { get; }
    }
}
