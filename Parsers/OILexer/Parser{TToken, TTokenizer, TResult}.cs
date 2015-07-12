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
    public abstract class Parser<TToken, TTokenizer, TResult> :
        Parser,
        IParser<TToken, TTokenizer, TResult>
        where TToken :
            IToken
        where TTokenizer :
            ITokenizer<TToken>
        where TResult :
            IConcreteNode
    {
        public Parser(IList<IToken> originalFormTokens = null)
            : base(originalFormTokens)
        {
        }

        #region IParser<TToken,TTokenizer> Members

        public new TToken GetCurrentToken()
        {
            return (TToken)base.GetCurrentToken();
        }

        /// <summary>
        /// Returns the current <typeparamref name="TTokenizer"/> that's responsible for the 
        /// tokenization process for the current file.
        /// </summary>
        public new TTokenizer CurrentTokenizer
        {
            get { return (TTokenizer)base.CurrentTokenizer; }
            protected set { base.CurrentTokenizer = value; }
        }

        /// <summary>
        /// Parses the <paramref name="fileName"/> and returns the appropriate <see cref="IParserResults{T}"/>
        /// </summary>
        /// <param name="fileName">The file to parse.</param>
        /// <returns>A <see cref="IParserResults{T}"/> that indicates the success of the operation
        /// with an instance of <typeparamref name="TResult"/>, if successful.</returns>
        public abstract IParserResults<TResult> Parse(string fileName);

        /// <summary>
        /// Parses the <paramref name="stream"/> and returns the appropriate <see cref="IParserResults{T}"/>
        /// </summary>
        /// <param name="s">The stream to parse.</param>
        /// <param name="fileName">The file name used provided an error is encountered in the <see cref="Stream"/>, <paramref name="s"/>.</param>
        /// <returns>A <see cref="IParserResults{T}"/> that indicates the success of the operation
        /// with an instance of <typeparamref name="TResult"/>, if successful.</returns>
        public abstract IParserResults<TResult> Parse(Stream s, string fileName);

        public new TToken LookAhead(int howFar)
        {
            return (TToken)base.LookAhead(howFar);
        }

        public void PushAhead(TToken token, int howFar)
        {
            base.PushAhead(token, howFar);
        }

        public void PushAhead(TToken token)
        {
            base.PushAhead(token);
        }

        public new TToken PopAhead(bool move)
        {
            return ((TToken)(base.PopAhead(move)));
        }
        public new TToken PopAhead()
        {
            return ((TToken)(base.PopAhead()));
        }

        #endregion

        public ITokenStream<TToken> GetAhead(int count)
        {
            return base.GetAhead<TToken>(count);
        }
    }
}
