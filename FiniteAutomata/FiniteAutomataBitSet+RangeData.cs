using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Oilexer.FiniteAutomata
{
    partial class FiniteAutomataBitSet<TCheck>
        where TCheck :
            FiniteAutomataBitSet<TCheck>,
            new()
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
        public enum ABSelect
        {
            A,
            B,
        }
        public struct ABCSet<TA, TB>
            where TA :
                struct
            where TB :
                struct
        {

            public ABSelect Which { get; private set; }

            public TA? A { get; private set; }
            public TB? B { get; private set; }

            public ABCSet(TA a)
                : this()
            {
                this.A = a;
                this.Which = ABSelect.A;
            }

            public ABCSet(TB b)
                : this()
            {
                this.B = b;
                this.Which = ABSelect.B;
            }
        }
        public struct RangeData :
            IEnumerable<ABCSet<char, RangeSet>>
        {
            List<ABCSet<char, RangeSet>> orderedData;
            public List<char> Singletons { get; private set; }
            public List<RangeSet> Sets { get; private set; }
            internal RangeData(TCheck source)
                : this()
            {
                this.orderedData = new List<ABCSet<char, RangeSet>>();
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
                    orderedData.Add(new ABCSet<char, RangeSet>((char)rangeStart));
                }
                else if (rangeStart == i - 2)
                {
                    Singletons.Add((char)rangeStart);
                    Singletons.Add((char)(rangeStart + 1));
                    orderedData.Add(new ABCSet<char, RangeSet>((char)(rangeStart)));
                    orderedData.Add(new ABCSet<char, RangeSet>((char)(rangeStart + 1)));
                }
                else
                {
                    Sets.Add(new RangeSet((char)rangeStart, (char)(i - 1)));
                    orderedData.Add(new ABCSet<char, RangeSet>(new RangeSet((char)rangeStart, (char)(i - 1))));
                }
                inRange = false;
                return inRange;
            }

            #region IEnumerable<AOrBSet<char,RangeSet>> Members

            public IEnumerator<ABCSet<char, RangeSet>> GetEnumerator()
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


        protected void Mirror(TCheck set)
        {
            if (set == null)
                return;
            this.values = set.values;
            this.offset = set.offset;
            this.lastMask = set.lastMask;
            this.lastMod = set.lastMod;
            this.length = set.length;
            this.fullLength = set.FullLength;
            this.complement = set.complement;
        }
    }
}
