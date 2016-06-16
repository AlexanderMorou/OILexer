using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    partial class OilexerGrammarTokens
    {
        public class ReferenceToken :
            OilexerGrammarToken
        {
            private object reference;
            /// <summary>
            /// Creates a new <see cref="ReferenceToken"/> instance with the <paramref name="column"/>,
            /// <paramref name="line"/>, and <paramref name="position"/> provided.
            /// </summary>
            /// <param name="reference">The object which is referenced by the <see cref="ReferenceToken"/>.</param>
            /// <param name="column">The column in line '<paramref name="line"/>' which the 
            /// <see cref="ReferenceToken"/> is defined.</param>
            /// <param name="line">The line at which the <see cref="ReferenceToken"/> is defined.</param>
            /// <param name="position">The extensionsClass that the <see cref="ReferenceToken"/> is defined at.</param>
            public ReferenceToken(object reference, int column, int line, long position)
                : base(column, line, position)
            {
                this.reference=reference;
            }

            public object Reference
            {
                get
                {
                    return this.reference;
                }
            }


            public override OilexerGrammarTokenType TokenType
            {
                get { return OilexerGrammarTokenType.ReferenceToken; }
            }

            public override int Length
            {
                get { return -1; }
            }

            public override bool ConsumedFeed
            {
                get { return true; }
            }
        }
    }
}
