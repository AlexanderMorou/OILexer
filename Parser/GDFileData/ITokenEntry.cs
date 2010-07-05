using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.Parser.GDFileData
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
        List<string> LowerPrecedenceNames { get; }
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
