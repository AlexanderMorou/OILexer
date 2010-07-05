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
        private ParserBuilder builder;
        private Dictionary<InlinedTokenEntry, IRegularLanguageStateMachine> stateMachines = new Dictionary<InlinedTokenEntry, IRegularLanguageStateMachine>();
        private long bufferSize;

        private long bufferPosition;

        private char[] buffer;

        private Stream source;

        private TextReader sourceReader;
        private GrammarVocabulary unhingedVocabulary;
        private void DoubleBuffer()
        {
            if (this.buffer != null)
            {
                char[] buffer = new char[(this.buffer.LongLength * 2)];
                this.buffer.CopyTo(buffer, 0);
                this.buffer = buffer;
            }
            else
            {
                this.buffer = new char[1024];
                this.bufferSize = 0;
            }
        }

        private bool LookAhead(int distance, out char result)
        {
            if (this.source == null ||
                this.sourceReader == null)
            {
                result = char.MinValue;
                return true;
            }
            long lookPoint = (this.bufferPosition + (distance + 1));
        bufferCheck:
            if ((this.buffer == null) || ((this.bufferSize == this.buffer.LongLength) || ((lookPoint + 1024) > this.buffer.LongLength)))
            {
                this.DoubleBuffer();
                goto bufferCheck;
            }
            // Check to see if the ahead needs read in.
            if (this.bufferSize < lookPoint)
            {
                int bytesToRead = ((int)((lookPoint - this.bufferSize)));
                int readLoc = ((int)((lookPoint - bytesToRead)));
                if (bytesToRead < 1024)
                    bytesToRead = 1024;
                int actualBytes = this.sourceReader.Read(this.buffer, readLoc, bytesToRead);
                this.bufferSize = (this.bufferSize + actualBytes);
            }
            if (lookPoint > this.bufferSize)
            {
                result = char.MinValue;
                return true;
            }
            else
                result = this.buffer[(lookPoint - 1)];
            return false;
        }

        public RegularLanguageLexer(ParserBuilder builder)
        {
            this.builder = builder;
            var tokens = builder.Source.GetTokens().ToArray();
            var fullGrammarBreakdown = GrammarVocabulary.ObtainCompleteSet(builder).Breakdown;
            this.TokenTable = new RegularLanguageTokenTable();

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

        public RegularLanguageScanData NextToken(GrammarVocabulary acceptable)
        {
            acceptable |= unhingedVocabulary;
            RegularLanguageTokenTable.Target currentTarget = null;
            char currentChar;
            int lookAheadDistance = 0;
            bool fileEnd = this.LookAhead(0, out currentChar);
            if (!fileEnd)
            {
                foreach (var element in this.TokenTable.Keys)
                    if (element.Contains(currentChar))
                    {
                        currentTarget = this.TokenTable[element];
                        break;
                    }
                if (currentTarget == null)
                    return new RegularLanguageScanData(this.bufferPosition);
                var workingSet = currentTarget.ObtainWorkingStateMachineData(acceptable, this.stateMachines);
                if (workingSet.Item1.Length == 0)
                    return new RegularLanguageScanData(this.bufferPosition);
                bool machinesActive = false;
                for (int i = 0; i < workingSet.Item1.Length; i++)
                    workingSet.Item2[i].Reset();
                do
                {
                    machinesActive = false;
                    for (int i = 0; i < workingSet.Item1.Length; i++)
                    {
                        if (workingSet.Item1[i] && !workingSet.Item2[i].Next(currentChar))
                            workingSet.Item1[i] = false;
                        if (!machinesActive && workingSet.Item1[i])
                            machinesActive = true;
                    }
                    fileEnd = this.LookAhead(++lookAheadDistance, out currentChar);
                } while (machinesActive && !fileEnd);
                int longest = 0;
                for (int i = 0; i < workingSet.Item2.Length; i++)
                {
                    int currentLength = 0;
                    if (workingSet.Item2[i].InValidEndState && (currentLength = workingSet.Item2[i].LongestLength) > longest)
                        longest = currentLength;
                }
                if (longest == 0 && fileEnd)
                    return new RegularLanguageScanData(this.bufferPosition);
                var result = new RegularLanguageScanData(this.bufferPosition);
                for (int i = 0; i < workingSet.Item2.Length; i++)
                    if (workingSet.Item2[i].InValidEndState && workingSet.Item2[i].LongestLength == longest)
                        workingSet.Item2[i].AddEntries(result);
                return result;
            }
            else
                return new RegularLanguageScanData(this.bufferPosition) { new RegularLanguageScanData.Entry(new GrammarVocabulary(builder.GrammarSymbols, builder.GrammarSymbols[builder.EOFToken])) };
        }
    }
}
