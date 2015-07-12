using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    public abstract class Token :
        IToken
    {
        /// <summary>
        /// Data member for <see cref="Column"/>.
        /// </summary>
        private int column;
        /// <summary>
        /// Data member for <see cref="Line"/>.
        /// </summary>
        private int line;
        /// <summary>
        /// Data member for <see cref="Position"/>.
        /// </summary>
        private long position;


        /// <summary>
        /// Creates a new <see cref="Token"/> with the <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/> provided.
        /// </summary>
        /// <param name="column">The character in the current <see cref="Line"/> the <see cref="Token"/> starts at.</param>
        /// <param name="line">The line the <see cref="IToken"/> is at.</param>
        /// <param name="position">The stream position for the <see cref="IToken"/>.</param>
        protected Token(int column, int line, long position)
        {
            this.column = column;
            this.line = line;
            this.position = position;
        }

        /// <summary>
        /// Creates a new <see cref="Token"/> initialized to a default state.
        /// </summary>
        protected Token()
        {

        }

        #region IToken Members

        /// <summary>
        /// Returns/sets the character in the current <see cref="Line"/> the <see cref="Token"/> starts at.
        /// </summary>
        public int Column
        {
            get
            {
                return this.column;
            }
            protected internal set
            {
                this.column = value;
            }
        }

        /// <summary>
        /// Returns/sets the line the <see cref="Token"/> is at.
        /// </summary>
        public int Line
        {
            get
            {
                return this.line;
            }
            protected internal set
            {
                this.line = value;
            }
        }

        /// <summary>
        /// Returns/sets the stream position for the <see cref="Token"/>.
        /// </summary>
        public long Position
        {
            get
            {
                return this.position;
            }
            protected internal set
            {
                this.position = value;
            }
        }

        public abstract int Length
        {
            get;
        }

        #endregion

        #region IToken Members


        public abstract bool ConsumedFeed
        {
            get;
        }

        #endregion
    }
}
