using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AllenCopeland.Abstraction.Slf.Cst;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a generic parser that verifies the 
    /// syntax of a given grammar with comprehensive error reporting.
    /// </summary>
    /// <typeparam name="TToken">The root-type of token used by the grammar.</typeparam>
    /// <typeparam name="TTokenizer">The type of tokenizer used by the parser.</typeparam>
    public interface IParser<TToken, TTokenizer, TResult> :
        IParser
        where TResult :
            IConcreteNode
        where TToken :
            IToken
        where TTokenizer :
            ITokenizer<TToken>
    {
        /// <summary>
        /// Returns the current <typeparamref name="TToken"/> from the <see cref="CurrentTokenizer"/>.
        /// </summary>
        new TToken GetCurrentToken();

        /// <summary>
        /// Returns the current <typeparamref name="TTokenizer"/> that's responsible for the tokenization process.
        /// </summary>
        new TTokenizer CurrentTokenizer { get; }

        /// <summary>
        /// Parses the <paramref name="fileName"/> and returns the appropriate <see cref="IParserResults{T}"/>
        /// </summary>
        /// <param name="fileName">The file to parse.</param>
        /// <returns>A <see cref="IParserResults{T}"/> that indicates the success of the operation
        /// with an instance of <typeparamref name="TResult"/>, if successful.</returns>
        IParserResults<TResult> Parse(string fileName);

        /// <summary>
        /// Parses the <paramref name="stream"/> and returns the appropriate <see cref="IParserResults{T}"/>
        /// </summary>
        /// <param name="s">The stream to parse.</param>
        /// <param name="fileName">The file name used provided an error is encountered in the <see cref="Stream"/>, <paramref name="s"/>.</param>
        /// <returns>A <see cref="IParserResults{T}"/> that indicates the success of the operation
        /// with an instance of <typeparamref name="TResult"/>, if successful.</returns>
        IParserResults<TResult> Parse(Stream s, string fileName);

        /// <summary>
        /// Returns a <see cref="IToken"/> from the <see cref="CurrentTokenizer"/> if a lookahead
        /// <see cref="howFar"/> hasn't been done or the previous lookahead(s) haven't been <see cref="PopAhead"/>'ed
        /// </summary>
        /// <param name="howFar">The number of tokens to look ahead.</param>
        /// <returns></returns>
        new TToken LookAhead(int howFar);
        /// <summary>
        /// Pushes ahead a token that's not from the stream 
        /// </summary>
        /// <param name="token">The token to push into the look-ahead stream.</param>
        /// <param name="howFar">How far to push the token.</param>
        void PushAhead(TToken token, int howFar = 0);
        /// <summary>
        /// Pushes ahead a token that's not from the stream.
        /// </summary>
        /// <param name="token">The token to push into the look-ahead stream.</param>
        void PushAhead(TToken token);
        new TToken PopAhead(bool move);
        new TToken PopAhead();

        ITokenStream<TToken> GetAhead(int count);
    }
}
