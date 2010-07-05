using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using Oilexer.Parser.Builder;
using Oilexer._Internal;
using System.Globalization;
using Oilexer.FiniteAutomata.Tokens;

namespace Oilexer.Parser
{
    partial class GDTokens
    {
        public class CharacterRangeToken :
            GDToken
        {
            private RegularLanguageSet ranges;
            bool inverted;
            private int length;

            internal RegularLanguageSet Ranges
            {
                get
                {
                    return this.ranges;
                }
            }

            internal CharacterRangeToken(bool inverted, char[] singleTons, Tuple<char, char>[] rangeSet, UnicodeCategory[] letterCategories, int length, int line, int column, long position)
                : base(line, column, position)
            {
                this.ranges = RegularLanguageSet.GetRegularLanguageSet(inverted, singleTons, rangeSet, letterCategories);
                this.length = length;
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
                return this.ranges.ToString();
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
