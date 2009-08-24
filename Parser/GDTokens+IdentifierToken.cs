using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
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
                get { return GDTokenType.Identifier; }
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
