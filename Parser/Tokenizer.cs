using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;

namespace Oilexer.Parser
{
    /// <summary>
    /// Provides a base tokenizer for the OILexer grammar language.
    /// </summary>
    public abstract partial class Tokenizer :
        ITokenizer
    {
        /// <summary>
        /// Input stream.
        /// </summary>
        private Stream stream;
        /// <summary>
        /// Buffer used to look ahead, but only so far.
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// The actual size of the data inside <see cref="buffer"/>.
        /// </summary>
        private long bufferSize = 0;
        /// <summary>
        /// The start location of the buffer.
        /// </summary>
        private long bufferStartLocale = 0;
        /// <summary>
        /// The series of <see cref="System.Int64"/> that indicate the position that each
        /// line starts at.
        /// </summary>
        private List<long> lineStarts = new List<long>();
        /// <summary>
        /// The current <see cref="CompilerError"/> resulted from the last <see cref="NextToken(int)"/>
        /// call.
        /// </summary>
        private CompilerError currentError = null;
        /// <summary>
        /// The current <see cref="IToken"/> resulted from the last <see cref="NextToken(int)"/> 
        /// call.
        /// </summary>
        private IToken currentToken;
        /// <summary>
        /// The file name associated with <see cref="stream"/>.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Defines a tokenizer state which indicates when used with <see cref="NextToken(int)"/>
        /// yields the same as any state (all possible tokens).
        /// </summary>
        /// <remarks>Essentially turns off the lexical analysis' context dependency.</remarks>
        public const long IndependentState = -1;

        /// <summary>
        /// Data structure used to handle whether the <see cref="NextToken(int)"/>
        /// was successful; if so, what value it contained, if not the error
        /// or reason why.
        /// </summary>
        internal protected struct NextTokenResults
        {
            /// <summary>
            /// Data member for <see cref="Token"/>.
            /// </summary>
            IToken token;
            /// <summary>
            /// Data member for <see cref="Error"/>.
            /// </summary>
            CompilerError error;

            /// <summary>
            /// Creates a new <see cref="NextTokenResults"/> instance with the 
            /// <paramref name="token"/> provided.
            /// </summary>
            /// <param name="token">The token of a successful operation.</param>
            public NextTokenResults(IToken token)
            {
                this.token = token;
                this.error = null;
            }

            /// <summary>
            /// Creates a new <see cref="NextTokenResults"/> instance with the 
            /// <paramref name="error"/> provided.
            /// </summary>
            /// <param name="error">The error of an unsuccessful operation.</param>
            public NextTokenResults(CompilerError error)
            {
                this.error = error;
                this.token = null;
            }

            /// <summary>
            /// Returns the token that resulted from the operation.
            /// </summary>
            /// <remarks>Null if <see cref="Successful"/> is false.</remarks>
            public IToken Token
            {
                get
                {
                    return this.token;
                }
            }

            /// <summary>
            /// Returns whether or not the operation was successful.
            /// </summary>
            public bool Successful
            {
                get
                {
                    return this.error == null && this.token != null;
                }
            }

            /// <summary>
            /// Returns the error that resulted from the operation.
            /// </summary>
            /// <remarks>Null if <see cref="Successful"/> is true.</remarks>
            public CompilerError Error
            {
                get
                {
                    return this.error;
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="Tokenizer"/> instance with the <paramref name="stream"/>
        /// and <paramref name="fileName"/> provided.
        /// </summary>
        /// <param name="stream">The stream from which the <see cref="Tokenizer"/> is to
        /// obtain information from.</param>
        /// <param name="fileName">The file name used to log errors.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="stream"/> is null.</exception>
        protected Tokenizer(Stream stream, string fileName)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            this.fileName = fileName;
            stream.Position = 0;
            this.stream = stream;
            lineStarts.Add(0);
            NewBuffer();
        }

        private void NewBuffer()
        {
            this.buffer = new byte[128];
        }

        private void CheckBuffer(long chars)
        {
            if (buffer.LongLength < chars)
            {
                //Double the buffer size and copy it over, check again.
                byte[] buffer2 = new byte[buffer.LongLength * 2];
                buffer.CopyTo(buffer2, 0);
                buffer = buffer2;
                CheckBuffer(chars);
            }
        }

        /// <summary>
        /// Returns the current line index based on the current <see cref="Position"/>.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> value which denotes the current line.</returns>
        public int GetLineIndex()
        {
            return GetLineIndex(this.Position);
        }

        /// <summary>
        /// Returns the line number of the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">A <see cref="System.Int64"/> which denotes the relative
        /// location to obtain the line index from.</param>
        /// <returns>A <see cref="System.Int32"/> value which denotes the line relative
        /// to <paramref name="position"/>.</returns>
        public int GetLineIndex(long position)
        {
            if (position < 0)
                return -1;
            if (lineStarts[lineStarts.Count - 1] <= position)
                return lineStarts.Count;
            int furthest = 0;
            for (int i = 0; i < lineStarts.Count; i++)
                if (lineStarts[i] <= position)
                {
                    furthest= i + 1;
                }
            return furthest;
        }

        /// <summary>
        /// Returnns the current column in the current line derived from <see cref="Position"/>.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> value which denotes the current column.</returns>
        public int GetColumnIndex()
        {
            return GetColumnIndex(this.Position);
        }

        /// <summary>
        /// Returns the column index at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">A <see cref="System.Int64"/> which denotes the relative
        /// location to obtain the column index from.</param>
        /// <returns>A <see cref="System.Int32"/> value which denotes the column relative
        /// to <paramref name="position"/>.</returns>
        public int GetColumnIndex(long position)
        {
            if (position < 0)
                return -1;
            if (lineStarts[lineStarts.Count - 1] < position)
                return (int)(position - lineStarts[lineStarts.Count - 1]) + 1;
            int lastClosest = -1;
            for (int i = 0; i < lineStarts.Count; i++)
                if (position >= lineStarts[i])
                    lastClosest = i;
                else if (lineStarts[i] > position)
                {
                    if (lastClosest == -1)
                        return 1;
                    else
                        return (int)(position - lineStarts[lastClosest]) + 1;
                }
            return (int)(position - lineStarts[lineStarts.Count - 1]) + 1;
        }

        #region ITokenizer Members

        /// <summary>
        /// Obtain the next state-driven token.
        /// </summary>
        /// <param name="token">The token the parser wants, indicates the scope
        /// to which a parse will occur.</param>
        public void NextToken(long parserState)
        {
            this.CurrentError = null;
            this.CurrentToken = null;
            NextTokenResults ntr = this.NextTokenInternal(parserState);
            if (ntr.Successful)
            {
                this.CurrentToken = ntr.Token;
                if (!ntr.Token.ConsumedFeed)
                    this.Flush(ntr.Token.Length);
            }
            else
            {
                //Move one...
                this.Position++;
                this.CurrentError = ntr.Error;
            }

        }

        /// <summary>
        /// Obtain the next state independent token (default behavior.)
        /// </summary>
        /// <remarks><para>Typical of parsers such as Lex.</para>
        /// http://dinosaur.compilertools.net/#lex
        /// <seealso cref="Tokenizer.IndependentState"/>
        /// </remarks>
        public void NextToken()
        {
            this.NextToken(Tokenizer.IndependentState);
        }

        protected abstract NextTokenResults NextTokenInternal(long parserState);

        /// <summary>
        /// Performs a lookahead in the current stream.
        /// </summary>
        /// <param name="howFar">The number of characters to look ahead.</param>
        /// <returns>The character of the index in the stream, if <paramref name="howFar"/> 
        /// is in range.  <see cref="Char.MinValue"/> otherwise.</returns>
        public char LookAhead(long howFar)
        {
            if (Position + (howFar + 1) > stream.Length)
                return char.MinValue;
            CheckBuffer(howFar + 1);
            if (bufferSize < (howFar + 1))
                bufferSize += stream.Read(buffer, (int)bufferSize, (int)((howFar + 1) - bufferSize));
            return (char)buffer[howFar];
        }

        /// <summary>
        /// Clears the current stream buffer and returns the characters associated.
        /// </summary>
        /// <returns>An array of <see cref="System.Char"/> relative to the characters in the stream.</returns>
        public char[] Flush()
        {
            if (this.bufferSize == 0)
                return new char[0];
            return this.Flush(this.bufferSize);
        }

        /// <summary>
        /// Clears the current stream buffer and returns the characters associated based upon the <paramref name="length"/> provided.
        /// </summary>
        /// <param name="length">The number of characters to retrieve from the buffer.</param>
        /// <returns>An array of <see cref="System.Char"/> relative to the characters in the stream
        /// limited by <paramref name="length"/> or the current length of the buffer.</returns>
        public char[] Flush(long length)
        {
            long count = length;
            if (count > bufferSize)
                count = bufferSize;
            char[] result = new char[count];
            //bool inCrLf = false;
            //long crLfStart = 0;
            Array.Copy(buffer, 0, result, 0, count);
            for (long i = 0; i < count; i++)
            {
                char cB = (char)result[i];
                //Works, but if they request a GetLineIndex beyond the scope of the 
                //lineStarts array, they get the last line, regardless if the position they give
                //is thirty lines later.
                if (cB == '\r')
                {
                    if (i < result.Length - 1)
                        if (result[i + 1] == '\n' &&
                            !lineStarts.Contains(bufferStartLocale + i + 2))
                        {
                            lineStarts.Add(bufferStartLocale + i + 2);
                            continue;
                        }
                    if (!lineStarts.Contains(bufferStartLocale + i + 1))
                        lineStarts.Add(bufferStartLocale + i + 1);
                }
                else if (cB == '\n' &&
                    !lineStarts.Contains(bufferStartLocale + i + 1))
                    lineStarts.Add(bufferStartLocale + i + 1);
            }
            stream.Position = bufferStartLocale + count;
            bufferStartLocale = stream.Position;
            bufferSize = 0;
            NewBuffer();
            return result;
        }

        /// <summary>
        /// Returns/sets the location in the current stream.
        /// </summary>
        public long Position
        {
            get
            {
                return (int)bufferStartLocale;
            }
            set
            {
                this.Flush();
                this.stream.Seek(value, SeekOrigin.Begin);
                bufferStartLocale = stream.Position;
            }
        }

        /// <summary>
        /// Returns the current length of the data stream associated to the 
        /// <see cref="ITokenizer"/>.
        /// </summary>
        public long Length
        {
            get
            {
                return this.stream.Length;
            }
        }

        /// <summary>
        /// Closes the data stream associated to the <see cref="Tokenizer"/>.
        /// </summary>
        public void CloseStream()
        {
            if (this.stream != null)
                this.stream.Close();
            this.stream = null;
        }

        /// <summary>
        /// Returns the last parsed token by the <see cref="ITokenizer"/>.
        /// </summary>
        public IToken CurrentToken
        {
            get { return this.currentToken; }
            private set { this.currentToken = value; }
        }

        /// <summary>
        /// Returns the current error that is present instead of the current token.
        /// </summary>
        public CompilerError CurrentError
        {
            get
            {
                return this.currentError;
            }
            private set
            {
                this.currentError = value;
            }
        }

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
        public string Scan(string target, bool seekPastTarget)
        {
            int lookahead = 0;
            /* *
             * Iterate through the rest of the stream (minus the length of the target) 
             * for a total of 'x' times which means it iterates completely through the 
             * target 'x' times.
             * */
            while ((this.Length - (this.Position + lookahead)) >= target.Length)
            {
                bool valid = true;
                for (int i = 0; i < target.Length; i++)
                {
                    if (this.LookAhead(lookahead + i) != target[i])
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    if (seekPastTarget)
                        return new string(this.Flush(lookahead + target.Length));
                    else
                        return new string(this.Flush(lookahead));
                }
                lookahead++;
            }
            return null;
        }

        public bool ScanCheck(string target, bool seekPastTarget, ref int ahead)
        {
            int lookahead = ahead;
            /* *
             * Iterate through the rest of the stream (minus the length of the target) 
             * for a total of 'x' times which means it iterates completely through the 
             * target 'x' times.
             * */
            while ((this.Length - (this.Position + lookahead)) >= target.Length)
            {
                bool valid = true;
                for (int i = 0; i < target.Length; i++)
                {
                    if (this.LookAhead(lookahead + i) != target[i])
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    if (seekPastTarget)
                        ahead = lookahead + target.Length - 1;
                    else
                        ahead = lookahead - 1;
                    return true;
                }
                lookahead++;
            }
            ahead = lookahead;
            return false;
        }

        /// <summary>
        /// Returns the <see cref="FileName"/> associated to the current <see cref="Tokenizer"/>'s
        /// input stream.
        /// </summary>
        public string FileName
        {
            get { return this.fileName; }
        }

        #endregion
    }
}
