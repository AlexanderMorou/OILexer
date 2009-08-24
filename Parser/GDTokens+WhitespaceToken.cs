using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    partial class GDTokens
    {
        public class WhitespaceToken :
            GDToken
        {
            private string whiteSpace;
            public WhitespaceToken(string whiteSpace, int line, int column, long position)
                : base(column, line, position)
            {
                this.whiteSpace = whiteSpace;
            }

            public override GDTokenType TokenType
            {
                get { return GDTokenType.Whitespace; }
            }

            public override int Length
            {
                get { return this.WhiteSpace.Length; }
            }

            public string WhiteSpace
            {
                get
                {
                    return this.whiteSpace;
                }
            }

            public override bool ConsumedFeed
            {
                get { return true; }
            }
        }
    }
}
