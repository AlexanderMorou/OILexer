using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a standard and state driven
    /// tokenizer.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Obtain the next state-driven token.
        /// </summary>
        /// <param name="token">The token the parser wants, indicates the scope
        /// to which a parse will occur.</param>
        void NextToken(long parserState);

        /// <summary>
        /// Obtain the next state independent token (default behavior.)
        /// </summary>
        /// <remarks><para>Typical of parsers such as Lex.</para>
        /// http://dinosaur.compilertools.net/#lex
        /// <para>Additionally, conflicts between token patterns are resolved on a 
        /// First come first serve (FCFS) basis.</para>
        /// </remarks>
        void NextToken();

        /// <summary>
        /// Returns the last parsed token by the <see cref="ITokenizer"/>.
        /// </summary>
        IToken CurrentToken { get; }

        /// <summary>
        /// Performs a lookahead in the current stream.
        /// </summary>
        /// <param name="howFar">The number of characters to look ahead.</param>
        /// <returns>The character of the index in the stream, if <paramref name="howFar"/> 
        /// is in range.  <see cref="Char.MinValue"/> otherwise.</returns>
        char LookAhead(long howFar);

        /// <summary>
        /// Clears the current stream buffer and returns the characters associated.
        /// </summary>
        /// <returns>An array of <see cref="System.Char"/> relative to the characters in the stream.</returns>
        char[] Flush();

        /// <summary>
        /// Clears the current stream buffer and returns the characters associated based upon the <paramref name="length"/> provided.
        /// </summary>
        /// <param name="length">The number of characters to retrieve from the buffer.</param>
        /// <returns>An array of <see cref="System.Char"/> relative to the characters in the stream
        /// limited by <paramref name="length"/> or the current length of the buffer.</returns>
        char[] Flush(long length);

        /// <summary>
        /// Returns/sets the location in the current stream.
        /// </summary>
        long Position { get; set; }
        /// <summary>
        /// Returns the current length of the data stream associated to the 
        /// <see cref="ITokenizer"/>.
        /// </summary>
        long Length { get; }
        /// <summary>
        /// Returns the <see cref="Int64"/> value representing
        /// the point in the file where the 
        /// <paramref name="line"/> is.
        /// </summary>
        /// <param name="line">The <see cref="Int32"/> value representing
        /// the line to obtain the positional start of.</param>
        /// <returns>A <see cref="Int64"/> value representing the position
        /// that <paramref name="line"/> starts.</returns>
        long GetPositionFromLine(int line);
        /// <summary>
        /// Returns the current line index based on the current <see cref="Position"/>.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> value which denotes the current line.</returns>
        int GetLineIndex();
        /// <summary>
        /// Returns the line number of the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">A <see cref="System.Int64"/> which denotes the relative
        /// location to obtain the line index from.</param>
        /// <returns>A <see cref="System.Int32"/> value which denotes the line relative
        /// to <paramref name="position"/>.</returns>
        int GetLineIndex(long position);
        /// <summary>
        /// Returnns the current column in the current line derived from <see cref="Position"/>.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> value which denotes the current column.</returns>
        int GetColumnIndex();
        /// <summary>
        /// Returns the column index at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">A <see cref="System.Int64"/> which denotes the relative
        /// location to obtain the column index from.</param>
        /// <returns>A <see cref="System.Int32"/> value which denotes the column relative
        /// to <paramref name="position"/>.</returns>
        int GetColumnIndex(long position);
        /// <summary>
        /// Closes the data stream associated to the <see cref="ITokenizer"/>, if applicable.
        /// </summary>
        void CloseStream();
        /// <summary>
        /// Returns the current error that is present instead of the current token.
        /// </summary>
        IParserSyntaxError CurrentError { get; }

        /// <summary>
        /// Scans the input stream for a <paramref name="target"/> <see cref="System.String"/> and
        /// only <paramref name="seekPastTarget"/> when required.
        /// </summary>
        /// <param name="target">The <see cref="System.String"/> to find in the stream.</param>
        /// <param name="seekPastTarget">Whether to seek past <paramref name="target"/>.</param>
        /// <returns>A <see cref="System.String"/> containing the text between the current
        /// <see cref="Position"/> and <paramref name="target"/>.</returns>
        /// <remarks>The <see cref="Position"/> is moved to the start of <paramref name="target"/> if
        /// <paramref name="seekPastTarget"/> is true, and after <paramref name="target"/>
        /// if <paramref name="seekPastTarget"/> is false.</remarks>
        string Scan(string target, bool seekPastTarget);

        /// <summary>
        /// Returns the name of the file being parsed.
        /// </summary>
        /// <remarks>Does not have to be a file if the stream is in memory; used for
        /// error tracking.</remarks>
        string FileName { get; }
    }
}
