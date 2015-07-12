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
    /// <summary>
    /// Provides a base <see cref="IOilexerGrammarToken"/> implementation.
    /// </summary>
    public abstract class OilexerGrammarToken :
        Token,
        IOilexerGrammarToken
    {
        /// <summary>
        /// Creates a new <see cref="OilexerGrammarToken"/> instance with the <paramref name="column"/>,
        /// <paramref name="line"/>, and <paramref name="position"/> provided.
        /// </summary>
        /// <param name="column">The column in line '<paramref name="line"/>' which the 
        /// <see cref="OilexerGrammarToken"/> is defined.</param>
        /// <param name="line">The line at which the <see cref="OilexerGrammarToken"/> is defined.</param>
        /// <param name="position">The extensionsClass that the <see cref="OilexerGrammarToken"/> is defined at.</param>
        protected OilexerGrammarToken(int column, int line, long position)
            : base(column, line, position)
        {
        }
        /// <summary>
        /// Creates a new <see cref="OilexerGrammarToken"/> instance initialized to a default state.
        /// </summary>
        protected OilexerGrammarToken()
            : base()
        {
        }

        /// <summary>
        /// The <see cref="OilexerGrammarTokenType"/> the <see cref="OilexerGrammarToken"/> is.
        /// </summary>
        public abstract OilexerGrammarTokenType TokenType
        {
            get;
        }
    }
}
