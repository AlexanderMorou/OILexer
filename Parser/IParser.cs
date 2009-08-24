using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    /// <summary>
    /// Defines properties and methods for working with a parser that verifies the syntax
    /// of a given grammar.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Returns the size of the look-ahead.
        /// </summary>
        int AheadLength { get; }
        IToken GetCurrentToken();

        /// <summary>
        /// Returns the current tokenizer that's responsible for the tokenization process.
        /// </summary>
        ITokenizer CurrentTokenizer { get; }

        /// <summary>
        /// Returns the mode the <see cref="CurrentTokenizer"/> is running in.
        /// </summary>
        ParserTokenizerMode TokenizerMode { get; }

        /// <summary>
        /// Returns the state the <see cref="IParser"/> is in with relation to the
        /// <see cref="CurrentTokenizer"/> if <see cref="TokenizerMode"/> is
        /// <see cref="ParserTokenizerMode.StateBased"/>.
        /// </summary>
        long State { get; }

        /// <summary>
        /// Returns a <see cref="IToken"/> from the <see cref="CurrentTokenizer"/> if a lookahead
        /// <see cref="howFar"/> hasn't been done or the previous lookahead(s) haven't been <see cref="PopAhead"/>'ed
        /// </summary>
        /// <param name="howFar">The number of tokens to look ahead.</param>
        /// <returns></returns>
        IToken LookAhead(int howFar);

        ITokenStream GetAhead(int count);

        /// <summary>
        /// Returns a <see cref="System.Char"/> after the last token in the <see cref="LookAhead(int)"/> 
        /// stream.
        /// </summary>
        /// <param name="howFar">how many characters past the last token in the lookahead stream to go.</param>
        /// <returns>A <see cref="System.Char"/> of the character <paramref name="howFar"/> past
        /// the last token in the look-ahead stream.</returns>
        char LookPast(int howFar);
        /// <summary>
        /// Pushes ahead a token that's not from the stream 
        /// </summary>
        /// <param name="token">The token to push into the look-ahead stream.</param>
        /// <param name="howFar">How far to push the token.</param>
        void PushAhead(IToken token, int howFar);
        /// <summary>
        /// Pushes ahead a token that's not from the stream.
        /// </summary>
        /// <param name="token">The token to push into the look-ahead stream.</param>
        void PushAhead(IToken token);
        IToken PopAhead(bool move);
        IToken PopAhead();
        /*
        /// <summary>
        /// Reverts the stream to a previous state by the <paramref name="number"/> of 
        /// tokens in the stream.
        /// </summary>
        /// <param name="number">A positive integer indicating how far to go back in the stream.</param>
        void Revert(int number);
        /// <summary>
        /// Unwinds the stream to a previous state and retrieves the symbols removed.
        /// </summary>
        /// <param name="number">A positive integer indicating how far to go back in the stream.</param>
        ITokenStream Unwind(int number);
        /// <summary>
        /// Unwinds the stream to before any tokens were entered onto the stack.
        /// </summary>
        /// <returns>A <see cref="ITokenStream"/> relative to the tokens that were present.</returns>
        ITokenStream Unwind();
         * */
    }
}
