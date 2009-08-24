using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    /// <summary>
    /// The type of token the <see cref="IGDToken"/> is.
    /// </summary>
    public enum GDTokenType
    {
        /// <summary>
        /// The token represents a pathExplorationComment.
        /// </summary>
        Comment,
        /// <summary>
        /// The token represents an identifier.
        /// </summary>
        Identifier,
        /// <summary>
        /// The token represents a range of characters.
        /// </summary>
        CharacterRange,
        /// <summary>
        /// The token represents a single character.
        /// </summary>
        CharacterLiteral,
        /// <summary>
        /// The token represents a string literal.
        /// </summary>
        StringLiteral,
        /// <summary>
        /// The token represents a numeric literal.
        /// </summary>
        NumberLiteral,
        /// <summary>
        /// The token represents an operator that can have an impact on interpretation of current
        /// and future tokens.
        /// </summary>
        Operator,
        /// <summary>
        /// The token represents ignorable whitespace that's used to segment the document space
        /// for readability.
        /// </summary>
        Whitespace,
        /// <summary>
        /// The token represents a preprocessor directive 
        /// </summary>
        PreprocessorDirective,
        /// <summary>
        /// <para>The token represents a slight hack to direct left-recursion where the non-recursive
        /// aspect is parsed continuously and wrapped in the defining rule post-parse.</para>
        /// <para>Reference tokens represent the wrapped form placed upon the token stack.</para>
        /// </summary>
        ReferenceToken
    }
}