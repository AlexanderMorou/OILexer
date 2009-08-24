using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    partial class GDTokens
    {
        public class NumberLiteral :
            GDToken
        {
            private string numberRef;

            public NumberLiteral(string numberRef, int column, int line, long position)
                : base(column, line, position)
            {
                this.numberRef = numberRef;
            }

            public int GetCleanValue()
            {
                if (this.numberRef.Length > 2 && this.numberRef[1] == 'x')
                {
                    return Convert.ToInt32(numberRef.Substring(2), 16);
                }
                bool @decimal = true;
                bool containsInvalid = false;
                foreach (char c in numberRef)
                {
                    if (c >= 'A' && c <= 'F')
                    {
                        @decimal = false;
                        break;
                    }
                    else if (c < '0' || c > '9')
                    {
                        containsInvalid = true;
                        break;
                    }
                }
                if (containsInvalid)
                    return int.MinValue;
                if (@decimal)
                    return Convert.ToInt32(numberRef, 10);
                else
                    return Convert.ToInt32(numberRef, 16);
            }

            public override GDTokenType TokenType
            {
                get { return GDTokenType.NumberLiteral; }
            }

            public override int Length
            {
                get {
                    return numberRef.Length;
                }
            }

            public override bool ConsumedFeed
            {
                get { return true; }
            }
        }
    }
}
