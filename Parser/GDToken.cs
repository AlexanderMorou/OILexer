using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    /// <summary>
    /// Provides a base <see cref="IGDToken"/> implementation.
    /// </summary>
    public abstract class GDToken :
        Token,
        IGDToken
    {
        /// <summary>
        /// Creates a new <see cref="GDToken"/> instance with the <paramref name="column"/>,
        /// <paramref name="line"/>, and <paramref name="position"/> provided.
        /// </summary>
        /// <param name="column">The column in line '<paramref name="line"/>' which the 
        /// <see cref="GDToken"/> is defined.</param>
        /// <param name="line">The line at which the <see cref="GDToken"/> is defined.</param>
        /// <param name="position">The locale that the <see cref="GDToken"/> is defined at.</param>
        protected GDToken(int column, int line, long position)
            : base(column, line, position)
        {
        }
        /// <summary>
        /// Creates a new <see cref="GDToken"/> instance initialized to a default state.
        /// </summary>
        protected GDToken()
            : base()
        {
        }

        /// <summary>
        /// The <see cref="GDTokenType"/> the <see cref="GDToken"/> is.
        /// </summary>
        public abstract GDTokenType TokenType
        {
            get;
        }
    }
}
