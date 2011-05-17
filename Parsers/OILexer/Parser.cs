using System;
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
    public abstract class Parser :
        IParser
    {
        //private ITokenStream lookaheadStream = null;
        protected IList<IToken> originalFormTokens = null;

        /// <summary>
        /// Data member for <see cref="State"/>.
        /// </summary>
        private long state;

        private ParserTokenizerMode tokenizerMode = ParserTokenizerMode.Traditional;
        /// <summary>
        /// Data member for <see cref="CurrentTokenizer"/>
        /// </summary>
        private ITokenizer currentTokenizer = null;
        protected Parser(IList<IToken> originalFormTokens = null)
        {
            if (originalFormTokens == null)
                originalFormTokens = new List<IToken>();
            this.originalFormTokens = originalFormTokens;
            //this.lookaheadStream = new TokenStream(this.originalFormTokens);
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
            return LookAheadImpl(howFar);
        }

        protected virtual IToken LookAheadImpl(int howFar)
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
                    StreamPosition = this.originalFormTokens[0].Position;
                return originalFormTokens[howFar];
            }
        }

        public void PushAhead(IToken token, int howFar = 0)
        {
            this.originalFormTokens.Insert(howFar, token);
        }

        public IToken PopAhead(bool move)
        {
            IToken r = LookAhead(0);
            if (r != null)
            {
                originalFormTokens.RemoveAt(0);
                if (r.Length != -1 && move)
                    StreamPosition = r.Position + r.Length;
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

        public virtual char TokenizerLookAhead(int howFar)
        {
            return this.CurrentTokenizer.LookAhead(howFar);
        }

        public virtual int AheadLength
        {
            get { return this.originalFormTokens.Count; }
        }

        public virtual ITokenStream<T> GetAhead<T>(int count)
            where T :
                IToken
        {
            List<T> streamItems = new List<T>();

            for (int i = 0; i < count; i++)
            {
                T it = (T)LookAhead(i);
                if (it != null)
                    streamItems.Add(it);
            }
            ClearAhead();
            if (streamItems.Count > 0)
            {
                IToken t = streamItems[streamItems.Count - 1];
                StreamPosition = t.Position + t.Length;
            }
            return new TokenStream<T>(streamItems);
        }

        #endregion

        protected virtual void ClearAhead()
        {
            this.originalFormTokens.Clear();
        }

        public virtual long StreamPosition
        {
            get
            {
                return this.CurrentTokenizer.Position;
            }
            set
            {
                this.CurrentTokenizer.Position = value;
            }
        }
    }
}
