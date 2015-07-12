using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IOilexerGrammarTokenEntry :
        IOilexerGrammarScannableEntry,
        INamedTokenSource
    {
        /// <summary>
        /// Returns the name of the <see cref="IOilexerGrammarNamedEntry"/>.
        /// </summary>
        new string Name { get; }
        /// <summary>
        /// Returns the <see cref="ITokenExpressionSeries"/> which defines the branches of
        /// the <see cref="IOilexerGrammarTokenEntry"/>.
        /// </summary>
        ITokenExpressionSeries Branches { get; }
        /// <summary>
        /// Returns whether the <see cref="IOilexerGrammarTokenEntry"/> is unhinged.
        /// </summary>
        /// <remarks>Unhinged tokens can appear anywhere regardless
        /// of the context.</remarks>
        bool Unhinged { get; set; }
        /// <summary>
        /// Returns the list of token names that have a lower precedence than the current token.
        /// </summary>
        List<OilexerGrammarTokens.IdentifierToken> LowerPrecedenceNames { get; }
        /// <summary>
        /// Returns the list of token instances that have a lower precedence than the current token.
        /// </summary>
        IOilexerGrammarTokenEntry[] LowerPrecedenceTokens { get; }
        /// <summary>
        /// Returns whether the <see cref="IOilexerGrammarTokenEntry"/> is forced into being
        /// a recognizer type token.
        /// </summary>
        bool ForcedRecognizer { get; }
        /// <summary>
        /// Returns whether the <see cref="IOilexerGrammarTokenEntry"/> represents an identity that is contextual in nature.
        /// </summary>
        /// <remarks>Noted by a '-' after the ':=' of a given token to note that it is not pervasive against those which
        /// it might be ambiguous against.</remarks>
        bool Contextual { get; }
    }
}
