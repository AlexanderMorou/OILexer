using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    partial class GDTokens
    {
        public class IdentifierToken :
            GDToken
        {
            /// <summary>
            /// Data member for <see cref="Name"/>.
            /// </summary>
            private string name;
            private GDTokenType tokenType = GDTokenType.Identifier;
            public IdentifierToken(string name, int column, int line, long position)
                : base(column, line, position)
            {
                this.name = name;
            }

            /// <summary>
            /// Returns the name of the <see cref="IdentifierToken"/>.
            /// </summary>
            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public override GDTokenType TokenType
            {
                get { return this.tokenType; }
            }

            internal void SetTokenType(GDTokenType newType, IScannableEntry entry = null)
            {
                this.tokenType = newType;
            }

            public override int Length
            {
                get { return this.Name.Length; }
            }

            public override bool ConsumedFeed
            {
                get { return true; }
            }
        }
    }
}
