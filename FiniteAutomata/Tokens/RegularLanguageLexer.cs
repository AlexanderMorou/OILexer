using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser;
using Oilexer.Parser.Builder;
using Oilexer._Internal.Inlining;
using System.IO;
using Oilexer.FiniteAutomata.Rules;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Tokens
{
    internal class RegularLanguageLexer
    {
        /// <summary>
        /// The parser builder which brought about the current
        /// language's <see cref="RegularLanguageLexer"/>.
        /// </summary>
        private ParserBuilder builder;
        /// <summary>
        /// The series of state machines associated to the current 
        /// <see cref="RegularLanguageLexer"/>.
        /// </summary>
        private Dictionary<InlinedTokenEntry, IRegularLanguageStateMachine> stateMachines = new Dictionary<InlinedTokenEntry, IRegularLanguageStateMachine>();
        /// <summary>
        /// The current buffer size.
        /// </summary>
        private long bufferSize;

        /// <summary>
        /// The current buffer position.
        /// </summary>
        private long bufferPosition;

        /// <summary>
        /// The current series of characters which represent the active file.
        /// </summary>
        private char[] buffer;

        /// <summary>
        /// The <see cref="Stream"/> which represents the current file.
        /// </summary>
        private Stream source;

        /// <summary>
        /// The <see cref="TextReader"/> which accepts multiple character
        /// encodings from which the <paramref name="source"/> is read.
        /// </summary>
        private TextReader sourceReader;
        /// <summary>
        /// The <see cref="GrammarVocabulary"/> which represents the 
        /// tokens within the current language which are not bound to
        /// the rules of the language, such as whitespace and comments.
        /// </summary>
        private GrammarVocabulary unhingedVocabulary;
        /// <summary>
        /// The <see cref="GrammarVocabulary"/> which represents the 
        /// end of file token.
        /// </summary>
        private GrammarVocabulary eofToken;

        /// <summary>
        /// Doubles the size of the buffer.
        /// </summary>
        private void DoubleBuffer()
        {
            //If the buffer isn't null.
            if (this.buffer != null)
            {
                //create a temporary buffer.
                char[] buffer = new char[(this.buffer.LongLength * 2)];
                //copy the characters from the current buffer to the new 
                //one.
                this.buffer.CopyTo(buffer, 0);
                //store the buffer.
                this.buffer = buffer;
            }
            else
            {
                //Create a new buffer.
                this.buffer = new char[1024];
                //Indicate the actual size of the buffer.
                this.bufferSize = 0;
            }
        }

        private bool LookAhead(int distance, out char result)
        {
            //If there's no source input, exit.
            if (this.source == null ||
                this.sourceReader == null)
            {
                result = char.MinValue;
                return true;
            }
            long lookPoint = (this.bufferPosition + (distance + 1));
            /* *
             * Increase the size of the buffer until it can contain the 
             * character at the current point designated by 'distance' relative
             * to the current buffer position.
             * */
            while ((this.buffer == null) || ((this.bufferSize == this.buffer.LongLength) || ((lookPoint + 1024) > this.buffer.LongLength)))
                this.DoubleBuffer();
            // Check to see if the ahead needs read in.
            if (this.bufferSize < lookPoint)
            {
                /* *
                 * Calculate the actual number of bytes to read.
                 * */
                int bytesToRead = ((int)((lookPoint - this.bufferSize)));
                /* *
                 * Adjust the actual read start point based upon the 
                 * actual size of the bufer relative to the destination.
                 * */
                int readLoc = ((int)((lookPoint - bytesToRead)));
                /* *
                 * Make the number of bytes read in count, read in a minimum
                 * of 1KB of data.
                 * */
                if (bytesToRead < 1024)
                    bytesToRead = 1024;
                //Read in the data.
                int actualBytes = this.sourceReader.Read(this.buffer, readLoc, bytesToRead);
                /* *
                 * Increase the buffer size by the number of bytes
                 * read in; it may differ from the number of bytes
                 * to read if the file's end is reached in the read
                 * operation.
                 * */
                this.bufferSize = (this.bufferSize + actualBytes);
            }
            /* *
             * If after reading in data, the ahead position is greater than the
             * size of the buffer, we've reached the end of the file.
             * */
            if (lookPoint > this.bufferSize)
            {
                result = char.MinValue;
                //The end of the file has been reached.
                return true;
            }
            else //... otherwise, assign the result to the value within the buffer
                result = this.buffer[(lookPoint - 1)];
            //The end of the file hasn't been reached.
            return false;
        }

        public RegularLanguageLexer(ParserBuilder builder)
        {
            this.builder = builder;
            var tokens = builder.Source.GetTokens().ToArray();
            var fullGrammarBreakdown = GrammarVocabulary.ObtainCompleteSet(builder).Breakdown;
            this.TokenTable = new RegularLanguageTokenTable();
            //
            this.eofToken = new GrammarVocabulary(builder.GrammarSymbols, builder.GrammarSymbols[builder.EOFToken]);
            foreach (var token in tokens)
                if (token.DFAState != null)
                {
                    this.TokenTable.Add(token.DFAState.OutTransitions.FullCheck, new RegularLanguageTokenTable.Target() { token });
                    RegularCaptureType captureType = token.DetermineKind();
                    if (captureType == RegularCaptureType.Recognizer || captureType == RegularCaptureType.Capturer)
                        this.stateMachines.Add(token, new RegularLanguageRecognizerStateMachine(token.DFAState, builder));
                    else if (captureType == RegularCaptureType.Transducer)
                        this.stateMachines.Add(token, new RegularLanguageTransducerStateMachine(token.DFAState, builder));
                    if (token.Unhinged)
                    {
                        GrammarVocabulary currentUnhingedVocabulary = null;
                        IGrammarTokenSymbol singleSymbol = null;
                        foreach (var symbol in fullGrammarBreakdown.CaptureTokens)
                            if (symbol.Source == token)
                            {
                                singleSymbol = symbol;
                                break;
                            }
                        if (singleSymbol == null)
                        {
                            foreach (var symbol in fullGrammarBreakdown.ConstantTokens)
                                if (symbol.Source == token)
                                {
                                    singleSymbol = symbol;
                                    break;
                                }
                            if (singleSymbol == null)
                            {
                                if (fullGrammarBreakdown.LiteralSeriesTokens.ContainsKey(token))
                                    currentUnhingedVocabulary = new GrammarVocabulary(builder.GrammarSymbols, fullGrammarBreakdown.LiteralSeriesTokens[token].ToArray());
                                goto noSingleCheck;
                            }
                        }
                        currentUnhingedVocabulary = new GrammarVocabulary(builder.GrammarSymbols, singleSymbol);

                    noSingleCheck: ;
                        if (unhingedVocabulary == null && currentUnhingedVocabulary != null)
                            unhingedVocabulary = currentUnhingedVocabulary;
                        else if (currentUnhingedVocabulary != null)
                            unhingedVocabulary |= currentUnhingedVocabulary;
                    }
                }
        }

        public RegularLanguageTokenTable TokenTable { get; private set; }

        public void Open(string filename)
        {
            if (!File.Exists(filename))
                return;
            this.source = new FileStream(filename, FileMode.Open);
            this.sourceReader = new StreamReader(this.source);
        }

        public void Close()
        {
            if (this.source == null ||
                this.sourceReader == null)
                return;
            this.source.Dispose();
            this.sourceReader.Dispose();
            this.source = null;
            this.sourceReader = null;
            this.buffer = null;
            this.bufferPosition = 0;
            this.bufferSize = 0;
        }

        internal void MoveTo(long p)
        {
            if (this.bufferPosition != p)
                this.bufferPosition = p;
        }

        /// <summary>
        /// Obtains the next token within the stream based upon the 
        /// <paramref name="acceptable"/> set of terms.
        /// </summary>
        /// <param name="acceptable">The <paramref name="GrammarVocabulary"/>
        /// that is valid in the current scope.</param>
        /// <returns>A <see cref="RegularLanguageScanData"/> which represents
        /// the tokens for the current parse.</returns>
        public RegularLanguageScanData NextToken(GrammarVocabulary acceptable)
        {
            /* *
             * Ensure that the current vocabulary contains the unhinged
             * elements, like whitespace and comments.
             * */
            acceptable |= unhingedVocabulary;
            RegularLanguageTokenTable.Target currentTarget = null;
            char currentChar;
            int lookAheadDistance = 0;
            /* *
             * Get the next character in the stream.
             * */
            bool fileEnd = this.LookAhead(0, out currentChar);
            //If the file's end wasn't encountered...
            if (!fileEnd)
            {
                /* *
                 * Iterate through the token groupings which
                 * contain the current character.
                 * *
                 * When an element that contains the character exists
                 * select it, so only the state machines that start
                 * with the character are initialized.
                 * */
                foreach (var element in this.TokenTable.Keys)
                    if (element.Contains(currentChar))
                    {
                        currentTarget = this.TokenTable[element];
                        break;
                    }
                /* *
                 * If no set of machines start with the current character
                 * then we've encountered an unknown symbol for the current
                 * language.
                 * */
                if (currentTarget == null)
                    return new RegularLanguageScanData(this.bufferPosition);
                /* *
                 * Obtain a series of state machines and a set of flags
                 * which denotes their active status.
                 * */
                var workingSet = currentTarget.ObtainWorkingStateMachineData(acceptable, this.stateMachines, p => new { ActiveMachines = p.Item1, Machines = p.Item2.ToArray() });
                /* *
                 * Unlikely, but if it failed, return.
                 * */
                if (workingSet.ActiveMachines.Length == 0)
                    return new RegularLanguageScanData(this.bufferPosition);
                bool machinesActive;
                /* *
                 * Reset all the machines in scope.
                 * */
                for (int i = 0; i < workingSet.ActiveMachines.Length; i++)
                    workingSet.Machines[i].Reset();
                do
                {
                    machinesActive = false;
                    /* *
                     * Iterate through the active machines, stepping through
                     * their states.
                     * */
                    for (int i = 0; i < workingSet.ActiveMachines.Length; i++)
                    {
                        /* *
                         * If the machine is active and the character
                         * at this point in the stream is not a valid transitional
                         * character in the state machine, deactivate
                         * that machine.
                         * */
                        if (workingSet.ActiveMachines[i] && !workingSet.Machines[i].Next(currentChar))
                            workingSet.ActiveMachines[i] = false;
                        /* *
                         * If it hasn't been noted that a machine is active and
                         * the current character is a valid state of the current
                         * machine, note that there is an active machine working
                         * to continue the loop.
                         * */
                        if (!machinesActive && workingSet.ActiveMachines[i])
                            machinesActive = true;
                    }
                    /* *
                     * Obtain the next character and increment the look-ahead
                     * distance, also determine if the file has ended.
                     * */
                    fileEnd = this.LookAhead(++lookAheadDistance, out currentChar);
                /* *
                 * If there are machines active, and the file's end hasn't been
                 * reached, continue.
                 * */
                } while (machinesActive && !fileEnd);
                /* *
                 * Adhering to maximal munch principle, attain the longest
                 * token(s) possible.
                 * */
                int longest = 0;
                for (int i = 0; i < workingSet.Machines.Length; i++)
                {
                    /* *
                     * If the current state machine is in a valid state and its
                     * length is greater than the current longest element, then 
                     * denote the longest element.
                     * */
                    int currentLength = 0;
                    if (workingSet.Machines[i].InValidEndState && (currentLength = workingSet.Machines[i].LongestLength) > longest)
                        longest = currentLength;
                }
                /* *
                 * If the longest element is zero characters long and the file's end
                 * has been reached, return an error.
                 * */
                if (longest == 0 && fileEnd)
                    return new RegularLanguageScanData(this.bufferPosition);
                /* *
                 * Obtain a new scan data.
                 * */
                var result = new RegularLanguageScanData(this.bufferPosition);
                /* *
                 * Iterate through the state machines, those that are equal to the longest
                 * length observed and which are in a valid exit state, add their
                 * reults to the scan results observed in this pass.
                 * */
                for (int i = 0; i < workingSet.Machines.Length; i++)
                {
                    IRegularLanguageStateMachine current = workingSet.Machines[i];
                    if (current.InValidEndState && current.LongestLength == longest)
                        current.AddEntries(result);
                }
                return result;
            }
            else
                /* *
                 * The file end is observed at the current location,
                 * yield an 'end of file' token.
                 * */
                return new RegularLanguageScanData(this.bufferPosition) { new RegularLanguageScanData.Entry(eofToken) };
        }
    }
}
