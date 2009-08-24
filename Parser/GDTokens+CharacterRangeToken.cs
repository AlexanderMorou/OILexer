using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using Oilexer._Internal;

namespace Oilexer.Parser
{
    partial class GDTokens
    {
        public class CharacterRangeToken :
            GDToken
        {
            private BitArray ranges;
            bool inverted;
            private int length;

            internal BitArray Ranges
            {
                get
                {
                    return this.ranges;
                }
            }

            public CharacterRangeToken(bool inverted, char[] range, int length, int line, int column, long position)
                : base(line, column, position)
            {
                this.length = length;
                //if (inverted)
                //    ranges = new BitArray(ushort.MaxValue + 1);
                //else
                ranges = new BitArray((int)range.Max() + 1, false);
                for (int i = 0; i < range.Length; i++)
                    ranges[range[i]] = true;

                this.inverted = inverted;
            }

            public bool Inverted
            {
                get
                {
                    return this.inverted;
                }
            }

            public override string ToString()
            {
                return ProjectConstructor.BitArrayToString(this.ranges, this.inverted);
            }

            public override GDTokenType TokenType
            {
                get { return GDTokenType.CharacterRange; }
            }

            public override int Length
            {
                get { return this.length; }
            }

            public override bool ConsumedFeed
            {
                get { return false; }
            }
        }
    }
}
