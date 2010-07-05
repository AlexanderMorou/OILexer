using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser
{
    public abstract class Parser :
        IParser
    {
        private ITokenStream lookaheadStream = null;

        private IList<IToken> originalFormTokens = null;

        /// <summary>
        /// Data member for <see cref="State"/>.
        /// </summary>
        private long state;

        private ParserTokenizerMode tokenizerMode = ParserTokenizerMode.Traditional;
        /// <summary>
        /// Data member for <see cref="CurrentTokenizer"/>
        /// </summary>
        private ITokenizer currentTokenizer = null;
        protected Parser()
        {
            this.originalFormTokens = new List<IToken>();
            this.lookaheadStream = new TokenStream(originalFormTokens);
        }

        #region IParser Members

        public IToken GetCurrentToken()
        {
            return this.LookAhead(0);
        }

        public ITokenizer CurrentTokenizer
        {
            get { return this.currentTokenizer; }
            protected set { this.currentTokenizer = value; }
        }

        public IToken LookAhead(int howFar)
        {
            if (howFar < originalFormTokens.Count)
            {
                return originalFormTokens[howFar];
            }
            else
            {
                for (int i = originalFormTokens.Count; i < howFar; i++)
                    LookAhead(i);
                if (this.originalFormTokens.Count > 0)
                    this.currentTokenizer.Position = this.originalFormTokens[this.originalFormTokens.Count - 1].Position + this.originalFormTokens[this.originalFormTokens.Count - 1].Length;
                
                CurrentTokenizer.NextToken(this.State);
                if (CurrentTokenizer.CurrentToken != null)
                {
                    originalFormTokens.Add(CurrentTokenizer.CurrentToken);
                }
                else
                    return null;
                if (this.originalFormTokens.Count > 0)
                    this.CurrentTokenizer.Position = this.originalFormTokens[0].Position;
                return originalFormTokens[howFar];
            }
        }

        public void PushAhead(IToken token, int howFar)
        {
            this.originalFormTokens.Insert(howFar, token);
        }

        public void PushAhead(IToken token)
        {
            this.PushAhead(token, 0);
        }

        public IToken PopAhead(bool move)
        {
            IToken r = LookAhead(0);
            if (r != null)
            {
                originalFormTokens.RemoveAt(0);
                if (r.Length != -1 && move)
                    this.CurrentTokenizer.Position = r.Position + r.Length;
            }
            return r;
        }
        public IToken PopAhead()
        {
            return PopAhead(true);
        }

        /// <summary>
        /// Returns the mode the <see cref="CurrentTokenizer"/> is running in.
        /// </summary>
        public ParserTokenizerMode TokenizerMode
        {
            get
            {
                return this.tokenizerMode;
            }
            protected set
            {
                this.tokenizerMode = value;
            }
        }

        /// <summary>
        /// Returns the state the <see cref="Parser"/> is in with relation to the
        /// <see cref="CurrentTokenizer"/> if <see cref="TokenizerMode"/> is
        /// <see cref="ParserTokenizerMode.StateBased"/>.
        /// </summary>
        public long State
        {
            get
            {
                switch (tokenizerMode)
                {
                    case ParserTokenizerMode.StateBased:
                        return this.state;
                    case ParserTokenizerMode.Traditional:
                    default:
                        return Tokenizer.IndependentState;
                }
            }
            protected set
            {
                this.state = value;
            }
        }
        /*
        public ITokenStream Unwind(int number)
        {
            if (number > this.lookaheadStream.Count)
                number = this.lookaheadStream.Count;
            IToken[] buffer = new IToken[number];
            for (int i = 0; i < number; i++)
                buffer[i] = this.lookaheadStream[this.lookaheadStream.Count - number + i];
            for (int i = 0; i < number; i++)
                this.originalFormTokens.RemoveAt(this.lookaheadStream.Count - number);
            return new TokenStream(buffer);
        }

        public ITokenStream Unwind()
        {
            return this.Unwind(this.lookaheadStream.Count);
        }

         * */

        public char LookPast(int howFar)
        {
            if (this.lookaheadStream.Count > 0)
            {
                char re = char.MinValue;
                IToken i = lookaheadStream[lookaheadStream.Count - 1];
                this.CurrentTokenizer.Position = i.Position + i.Length;
                re = this.CurrentTokenizer.LookAhead(howFar);
                this.CurrentTokenizer.Position = lookaheadStream[0].Position;
                return re;
            }
            else
                return this.CurrentTokenizer.LookAhead(howFar);
        }

        public int AheadLength
        {
            get { return this.originalFormTokens.Count; }
        }

        public ITokenStream GetAhead(int count)
        {
            List<IToken> streamItems = new List<IToken>();

            for (int i = 0; i < count; i++)
            {
                IToken it = LookAhead(i);
                if (it != null)
                    streamItems.Add(it);
            }
            this.originalFormTokens.Clear();
            if (streamItems.Count > 0)
            {
                IToken t = streamItems[streamItems.Count - 1];
                this.CurrentTokenizer.Position = t.Position + t.Length;
            }
            return new TokenStream(streamItems);
        }

        #endregion

        protected void ClearAhead()
        {
            this.originalFormTokens.Clear();
        }

    }
}
