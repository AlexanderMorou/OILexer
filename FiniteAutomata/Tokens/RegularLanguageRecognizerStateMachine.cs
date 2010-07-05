using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.FiniteAutomata.Rules;
using Oilexer._Internal.Inlining;
using Oilexer.Parser.Builder;

namespace Oilexer.FiniteAutomata.Tokens
{
    public class RegularLanguageRecognizerStateMachine :
        IRegularLanguageStateMachine
    {
        private RegularLanguageDFAState current;
        private RegularLanguageDFARootState initial;
        private InlinedTokenEntry entry;
        private ParserBuilder builder;
        private int exitLength;
        internal char[] buffer;

        internal int actualSize = 0;
        bool inValidEdgeState;

        #region Buffer tracking
        private void GrowBuffer(int totalSize)
        {
            if (this.buffer == null)
            {
                this.buffer = new char[totalSize];
                return;
            }
            if (this.buffer.Length >= totalSize)
                return;
            int pNew = (this.actualSize * 2);
            if (totalSize > pNew)
                pNew = totalSize;
            char[] newBuffer = new char[pNew];
            this.buffer.CopyTo(newBuffer, 0);
            this.buffer = newBuffer;
        }

        public void Push(string s)
        {
            if (this.buffer == null)
                this.GrowBuffer(s.Length);
            else if (this.buffer.Length < (this.actualSize + s.Length))
                this.GrowBuffer((this.actualSize + s.Length));
            for (int i = 0; (i < s.Length); i++)
            {
                this.buffer[this.actualSize] = s[i];
                this.actualSize++;
            }
        }

        public override sealed string ToString()
        {
            char[] result = new char[this.actualSize];
            for (int i = 0; (i < this.actualSize); i++)
            {
                result[i] = this.buffer[i];
            }
            return new string(result);
        }

        public string GetCapture()
        {
            char[] result = new char[this.exitLength];
            for (int i = 0; (i < this.exitLength); i++)
            {
                result[i] = this.buffer[i];
            }
            return new string(result);
        }

        public void Push(char c)
        {
            if (this.buffer == null)
                this.GrowBuffer(2);
            else if (this.buffer.Length < (this.actualSize + 1))
                this.GrowBuffer((this.actualSize + 1));
            this.buffer[this.actualSize] = c;
            this.actualSize++;
        }
        #endregion

        public RegularLanguageRecognizerStateMachine(RegularLanguageDFARootState initial, ParserBuilder builder)
        {
            this.initial = initial;
            this.builder = builder;
            this.entry = (InlinedTokenEntry)initial.Entry;
        }

        #region IRegularLanguageStateMachine Members

        /// <summary>
        /// Resets the state machine back to its initial state.
        /// </summary>
        public void Reset()
        {
            this.current = initial;
            exitLength = 0;
            inValidEdgeState = this.current.IsEdge;
            this.actualSize = 0;
        }

        /// <summary>
        /// Moves to the next position inside the state machine.
        /// </summary>
        /// <param name="c">The character which acts as the key in 
        /// transitioning from one state of the machine to the next.
        /// </param>
        /// <returns>true if the state machine has a transition in 
        /// its current state for the provided <paramref name="c"/>; 
        /// false, otherwise.</returns>
        public bool Next(char c)
        {
            //iterate through the current state's outgoing transitions.
            foreach (var transition in current.OutTransitions.Keys)
                /* *
                 * If the transition contains the character, then it's
                 * the best candidate for transition.  This is due to
                 * the deterministic nature of the state machine used.
                 * */
                if (transition.Contains(c))
                {
                    //Advance the state.
                    current = current.OutTransitions[transition];
                    //Push the character
                    Push(c);
                    /* *
                     * If the state is a valid edge of the machine
                     * mark it as so and note the exit length.
                     * */
                    if (current.IsEdge)
                    {
                        inValidEdgeState = true;
                        exitLength = actualSize;
                    }
                    if (current.OutTransitions.Count > 0)
                        return true;
                }
            return false;
        }

        /// <summary>
        /// Returns whether the <see cref="IRegularLanguageStateMachine"/>
        /// is in a valid end state.
        /// </summary>
        public bool InValidEndState
        {
            get { return this.inValidEdgeState; }
        }

        public int LongestLength
        {
            get
            {
                if (this.inValidEdgeState)
                    return this.exitLength;
                else
                    return 0;
            }
        }

        #endregion

        #region IRegularLanguageStateMachine Members
        
        public void AddEntries(RegularLanguageScanData result)
        {
            if (this.InValidEndState)
                result.Add(new RegularLanguageScanData.CaptureEntry(new GrammarVocabulary(builder.GrammarSymbols, builder.GrammarSymbols[this.entry]), this.GetCapture()));
        }

        #endregion
    }
}
