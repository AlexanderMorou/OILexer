using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    partial class RegularLanguageSet
    {
        public struct RangeSet
        {
            public char Start { get; private set; }
            public char End { get; private set; }
            internal RangeSet(char start, char end)
                : this()
            {
                this.Start = start;
                this.End = end;
            }
        }
        public enum SwitchPairElement
        {
            A,
            B,
        }
        public struct SwitchPair<TA, TB>
            where TA :
                struct
            where TB :
                struct
        {

            public SwitchPairElement Which { get; private set; }

            public TA? A { get; private set; }
            public TB? B { get; private set; }

            public SwitchPair(TA a)
                : this()
            {
                this.A = a;
                this.Which = SwitchPairElement.A;
            }

            public SwitchPair(TB b)
                : this()
            {
                this.B = b;
                this.Which = SwitchPairElement.B;
            }
        }
        public struct RangeData :
            IEnumerable<SwitchPair<char, RangeSet>>
        {
            List<SwitchPair<char, RangeSet>> orderedData;
            public List<char> Singletons { get; private set; }
            public List<RangeSet> Sets { get; private set; }
            internal RangeData(RegularLanguageSet source)
                : this()
            {
                this.orderedData = new List<SwitchPair<char, RangeSet>>();
                Singletons = new List<char>();
                Sets = new List<RangeSet>();
                source.Reduce();
                bool inRange = false;
                uint rangeStart = 0;
                uint start = source.IsNegativeSet ? 0 : source.Offset;
                for (uint i = start; i < source.Offset + source.Length; i++)
                {
                    if (source[i] ^ source.IsNegativeSet)
                    {
                        if (!inRange)
                        {
                            inRange = true;
                            rangeStart = i;
                        }
                    }
                    else if (inRange)
                        inRange = TerminateRange(inRange, rangeStart, i);
                }
                TerminateRange(inRange, rangeStart, source.Offset + source.Length);
            }

            private bool TerminateRange(bool inRange, uint rangeStart, uint i)
            {
                if (rangeStart == i - 1)
                {
                    Singletons.Add((char)rangeStart);
                    orderedData.Add(new SwitchPair<char, RangeSet>((char)rangeStart));
                }
                else if (rangeStart == i - 2)
                {
                    Singletons.Add((char)rangeStart);
                    Singletons.Add((char)(rangeStart + 1));
                    orderedData.Add(new SwitchPair<char, RangeSet>((char)(rangeStart)));
                    orderedData.Add(new SwitchPair<char, RangeSet>((char)(rangeStart + 1)));
                }
                else if (inRange)
                {
                    Sets.Add(new RangeSet((char)rangeStart, (char)(i - 1)));
                    orderedData.Add(new SwitchPair<char, RangeSet>(new RangeSet((char)rangeStart, (char)(i - 1))));
                }
                inRange = false;
                return inRange;
            }

            #region IEnumerable<SwitchPair<char,RangeSet>> Members

            public IEnumerator<SwitchPair<char, RangeSet>> GetEnumerator()
            {
                foreach (var rd in this.orderedData)
                    yield return rd;
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }
    }
}
