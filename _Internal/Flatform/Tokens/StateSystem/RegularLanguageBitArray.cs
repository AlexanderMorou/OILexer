using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
//using Oilexer._Internal.UI.Visualization;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens.StateSystem
{
    /// <summary>
    /// Provides a custom bit array used for somewhat optimized
    /// set comparisons, using little memory.
    /// </summary>
    internal sealed class RegularLanguageBitArray :
        IEquatable<RegularLanguageBitArray>//,
        //IVisualBitSet
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
        public struct AOrBSet<TA, TB>
            where TA :
                struct
            where TB :
                struct
        {
            public enum ABSelect
            {
                A,
                B
            }

            public ABSelect Which { get; private set; }

            public TA? A { get; private set; }
            public TB? B { get; private set; }

            public AOrBSet(TA a)
                : this()
            {
                this.A = a;
                this.Which = ABSelect.A;
            }

            public AOrBSet(TB b)
                : this()
            {
                this.B = b;
                this.Which = ABSelect.B;
            }
        }
        public struct RangeData :
            IEnumerable<AOrBSet<char, RangeSet>>
        {
            List<AOrBSet<char, RangeSet>> orderedData;
            public List<char> Singletons { get; private set; }
            public List<RangeSet> Sets { get; private set; }
            internal RangeData(RegularLanguageBitArray source)
                : this()
            {
                this.orderedData = new List<AOrBSet<char, RangeSet>>();
                Singletons = new List<char>();
                Sets = new List<RangeSet>();
                source.Reduce();
                bool inRange = false;
                uint rangeStart = 0;
                uint start = source.IsInverted ? 0 : source.setData.Offset;
                for (uint i = start; i < source.Length; i++)
                {
                    if (source[i])
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
                TerminateRange(inRange, rangeStart, source.Length);
            }

            private bool TerminateRange(bool inRange, uint rangeStart, uint i)
            {
                if (rangeStart == i - 1)
                {
                    Singletons.Add((char)rangeStart);
                    orderedData.Add(new AOrBSet<char, RangeSet>((char)rangeStart));
                }
                else if (rangeStart == i - 2)
                {
                    Singletons.Add((char)rangeStart);
                    Singletons.Add((char)(rangeStart + 1));
                    orderedData.Add(new AOrBSet<char, RangeSet>((char)(rangeStart)));
                    orderedData.Add(new AOrBSet<char, RangeSet>((char)(rangeStart + 1)));
                }
                else
                {
                    Sets.Add(new RangeSet((char)rangeStart, (char)(i - 1)));
                    orderedData.Add(new AOrBSet<char, RangeSet>(new RangeSet((char)rangeStart, (char)(i - 1))));
                }
                inRange = false;
                return inRange;
            }

            #region IEnumerable<AOrBSet<char,RangeSet>> Members

            public IEnumerator<AOrBSet<char, RangeSet>> GetEnumerator()
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

        private SetCommon.MinimalSetData setData;

        /// <summary>
        /// Reduces the <see cref="RegularLanguageBitArray"/> to a minimal operating set.
        /// </summary>
        public void Reduce()
        {
            this.setData.Reduce();
        }

        /// <summary>
        /// Creates a new <see cref="RegularLanguageBitArray"/> with the
        /// <see cref="BitArray"/> which describes the data.
        /// </summary>
        /// <param name="original">The <see cref="BitArray"/> used to
        /// initialize the <see cref="RegularLanguageBitArray"/>.</param>
        public RegularLanguageBitArray(BitArray original)
            : this(original, false)
        {
        }

        public RegularLanguageBitArray(BitArray original, int offset)
        {
            this.setData = new SetCommon.MinimalSetData((uint)offset, ushort.MaxValue + 1, original, false);
        }

        public RegularLanguageBitArray(RegularLanguageBitArray target)
        {
            //this.setData = new SetCommon.MinimalSetData(target.setData);
            this.setData = new SetCommon.MinimalSetData(target.setData.Offset, ushort.MaxValue+1, target.setData.WorkingSet == null ? null : new BitArray(target.setData.GetIntWorkingSet()), target.setData.Inverted);
        }

        /// <summary>
        /// Creates a new <see cref="RegularLanguageBitArray"/> with the
        /// <see cref="BitArray"/> which describes the data.
        /// </summary>
        /// <param name="original">The <see cref="BitArray"/> used to
        /// initialize the <see cref="RegularLanguageBitArray"/>.</param>
        /// <param name="inverted">Whether the <paramref name="original"/>
        /// <see cref="BitArray"/> is to be considered in an inverted
        /// state.</param>
        public RegularLanguageBitArray(BitArray original, bool inverted)
        {
            this.setData = new SetCommon.MinimalSetData(0, ushort.MaxValue + 1, original == null ? null : new BitArray(original), inverted);
            this.setData.Reduce();
        }

        public RegularLanguageBitArray(int length)
        {
            this.setData = new SetCommon.MinimalSetData(0, ushort.MaxValue + 1, new BitArray(length), false);
        }

        public RegularLanguageBitArray(int length, bool inverted)
            : this(length)
        {
            this.setData.Inverted = inverted;
        }

        private RegularLanguageBitArray()
        {
            this.setData = new SetCommon.MinimalSetData(0, ushort.MaxValue + 1, null, false);
        }

        /// <summary>
        /// Returns/sets the value at the <paramref name="index"/> provided.
        /// </summary>
        /// <param name="index">The index of the element in the series.</param>
        /// <returns>true if the value at the given point is in the range of the bitarray.</returns>
        public bool this[uint index]
        {
            get
            {
                return this.setData[index];
            }
            set
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index");
                if (this.setData.WorkingSet == null)
                {
                    this.setData = new SetCommon.MinimalSetData(index, ushort.MaxValue + 1, new BitArray(1000), this.setData.Inverted);
                    this.setData[0] = value ^ this.setData.Inverted;
                }
                else if (this.setData.Length + this.setData.Offset <= index)
                {
                    /* *
                     * If no change will result from this, don't perform the operation.
                     * */
                    if (value ^ this.setData.Inverted)
                    {
                        BitArray newArray = new BitArray((int)(index + 1));
                        for (uint i = 0; i < newArray.Length; i++)
                            newArray[(int)i] = this.setData[i - this.setData.Offset];
                        newArray[(int)(index - this.setData.Offset)] = true;
                        this.setData = new SetCommon.MinimalSetData(this.setData.Offset, setData.FullSetLength, newArray, this.setData.Inverted);
                        this.Reduce();
                    }
                }
                else if (index < this.setData.Offset)
                {
                    /* *
                     * If no change will result from this, don't perform the operation.
                     * */
                    if (value ^ this.setData.Inverted)
                    {
                        BitArray newArray = new BitArray((int)(this.setData.Offset + this.setData.Length));
                        for (uint i = 0; i < newArray.Length; i++)
                            newArray[(int)i] = this.setData[i - this.setData.Offset];
                        newArray[(int)index] = true;
                        this.setData = new SetCommon.MinimalSetData(index, ushort.MaxValue + 1, newArray, this.setData.Inverted);
                        this.Reduce();
                    }
                }
                else
                    this.setData[index] = value;
            }
        }

        /// <summary>
        /// Returns whether all elements contained within the
        /// <see cref="RegularLanguageBitArray"/> are false.
        /// </summary>
        public bool AllFalse
        {
            get
            {
                return this.setData.AllFalse;
            }
        }

        public bool AllTrue
        {
            get
            {
                return this.setData.AllTrue;
            }
        }

        public static RegularLanguageBitArray operator |(RegularLanguageBitArray left, RegularLanguageBitArray right)
        {
            if (left == null)
                return right;
            else if (right == null)
                return left;
            var result = new RegularLanguageBitArray();
            result.setData = SetCommon.BitwiseOr(left.setData, right.setData);
            return result;
        }

        /// <summary>
        /// Performs a bitwise and operation on the <paramref name="left"/> <see cref="RegularLanguageBitArray"/>
        /// with the <paramref name="right"/> <see cref="RegularLanguageBitArray"/>.
        /// </summary>
        /// <param name="left">The transitionFirst <see cref="RegularLanguageBitArray"/>.</param>
        /// <param name="right">The second <see cref="RegularLanguageBitArray"/>.</param>
        /// <returns>A new <see cref="RegularLanguageBitArray"/> which is a bitwise and merger
        /// between the <paramref name="left"/> and <paramref name="right"/> <see cref="RegularLanguageBitArray"/>
        /// considering special cases of inversion.</returns>
        public static RegularLanguageBitArray operator &(RegularLanguageBitArray left, RegularLanguageBitArray right)
        {
            if (left == null)
                return left;
            else if (right == null)
                return right;
            var result = new RegularLanguageBitArray();
            result.setData = SetCommon.BitwiseAnd(left.setData, right.setData);
            return result;
        }

        /// <summary>
        /// Performs an exclusive or operation on two 
        /// <see cref="RegularLanguageBitArray"/> instances.
        /// </summary>
        /// <param name="left">The transitionFirst <see cref="RegularLanguageBitArray"/> to perform the exclusive or
        /// operation on.</param>
        /// <param name="right">The second <see cref="RegularLanguageBitArray"/> to perform the exclusive or
        /// operation on.</param>
        /// <returns>A new <see cref="RegularLanguageBitArray"/> after obtaining the exclusive set of 
        /// values form the <paramref name="left"/> and <paramref name="right"/> operands considering special
        /// cases of inversion.</returns>
        public static RegularLanguageBitArray operator ^(RegularLanguageBitArray left, RegularLanguageBitArray right)
        {
            if (left == null)
                return right;
            else if (right == null)
                return left;
            var result = new RegularLanguageBitArray();
            result.setData = SetCommon.BitwiseExclusiveOr(left.setData, right.setData);
            return result;
        }

        /// <summary>
        /// Returns the offset which denotes how many items are nonexistent
        /// in the <see cref="RegularLanguageBitArray"/> from zero.
        /// </summary>
        public uint Offset { get {
            if (this.setData.Inverted)
                return ushort.MinValue;
            else
                return this.setData.Offset;
        }
            set
            {
                if (this.setData.WorkingSet == null)
                    return;
                this.setData.Offset = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="Int32"/> value representing the overall length
        /// of the <see cref="RegularLanguageBitArray"/>.
        /// </summary>
        public uint Length
        {
            get
            {
                if (this.setData.WorkingSet == null)
                    if (this.setData.Inverted)
                        return ushort.MaxValue + 1;
                    else
                        return 0;
                if (!this.setData.Inverted)
                    return this.setData.Offset + this.setData.Length;
                return ushort.MaxValue + 1;
            }
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> <see cref="RegularLanguageBitArray"/> is
        /// equal to the <paramref name="right"/> <see cref="RegularLanguageBitArray"/>.
        /// </summary>
        /// <param name="left">The <see cref="RegularLanguageBitArray"/> to compare.</param>
        /// <param name="right">The <see cref="RegularLanguageBitArray"/> compared to <paramref name="left"/>.</param>
        /// <returns></returns>
        public static bool Equals(RegularLanguageBitArray left, RegularLanguageBitArray right)
        {
            return left == right;
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> <see cref="RegularLanguageBitArray"/>
        /// is equal to the <paramref name="right"/> <see cref="RegularLanguageBitArray"/>.
        /// </summary>
        /// <param name="left">The <see cref="RegularLanguageBitArray"/> to compare.</param>
        /// <param name="right">The <see cref="RegularLanguageBitArray"/> compared to <paramref name="left"/>.</param>
        /// <returns>true if the <paramref name="left"/> <see cref="RegularLanguageBitArray"/>
        /// equals the <paramref name="right"/> <see cref="RegularLanguageBitArray"/>; false otherwise.</returns>
        public static bool operator ==(RegularLanguageBitArray left, RegularLanguageBitArray right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return true;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;
            if (object.ReferenceEquals(left, right))
                return true;
            return SetCommon.SetsEqual(left.setData, right.setData);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> <see cref="RegularLanguageBitArray"/>
        /// is not equal to the <paramref name="right"/> <see cref="RegularLanguageBitArray"/>.
        /// </summary>
        /// <param name="left">The <see cref="RegularLanguageBitArray"/> to compare.</param>
        /// <param name="right">The <see cref="RegularLanguageBitArray"/> compared to <paramref name="left"/>.</param>
        /// <returns>false if the <paramref name="left"/> <see cref="RegularLanguageBitArray"/>
        /// equals the <paramref name="right"/> <see cref="RegularLanguageBitArray"/>; true otherwise.</returns>
        public static bool operator !=(RegularLanguageBitArray left, RegularLanguageBitArray right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return false;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return true;
            if (object.ReferenceEquals(left, right))
                return false;
            return SetCommon.SetsNotEqual(left.setData, right.setData);
        }

        public override bool Equals(object obj)
        {
            if (obj is RegularLanguageBitArray)
                return ((RegularLanguageBitArray)(obj)) == this;
            return false;
        }

        public override string ToString()
        {
            StringBuilder result = null;
            if (this.setData.Inverted)
                if (this.setData.WorkingSet == null)
                    return "\u0000 -> \uFFFF";
                else
                {
                    result = new StringBuilder();
                    result.Append("[");
                    result.Append("^");
                }
            else if (this.setData.WorkingSet == null)
                return "{NONE}";
            else
            {
                result = new StringBuilder();
                result.Append("[");
            }


            BuildStandardResult(result);

            result.Append("]");
            return result.ToString();
        }

        private void BuildStandardResult(StringBuilder result)
        {
            bool inRange = false;
            uint rangeStart = 0xFFFFFFFF;
            for (uint i = 0; i < this.setData.Length; i++)
            {
                if ((this.setData[i + setData.Offset] ^ this.IsInverted) && !inRange)
                {
                    var current = (char)(this.setData.Offset + i);
                    string @char = current.ToString();
                    switch (current)
                    {
                        case '\r':
                            @char = @"\r";
                            break;
                        case '\n':
                            @char = @"\n";
                            break;
                        case '\t':
                            @char = @"\t";
                            break;
                        case '\b':
                            @char = @"\b";
                            break;
                        case '\v':
                            @char = @"\v";
                            break;
                        default:
                            if (current < 32)
                            {
                                string p = string.Format("{0:X}", (int)current);
                                while (p.Length < 2)
                                    p = "0" + p;
                                @char = @"\x" + p;
                            }
                            else if (current > 'z')
                            {
                                string p = string.Format("{0:X}", (int)current);
                                while (p.Length < 4)
                                    p = "0" + p;
                                @char = @"\u" + p;
                            }
                            break;
                    }
                    result.Append(@char);
                    inRange = true;
                    rangeStart = i;
                }
                else if (inRange && !(this.setData[i + setData.Offset] ^ this.IsInverted))
                    RangeEndLogic(result, ref inRange, ref rangeStart, i);
            }
            if (inRange)
                RangeEndLogic(result, ref inRange, ref rangeStart, this.setData.Length);
        }

        private void RangeEndLogic(StringBuilder result, ref bool inRange, ref uint rangeStart, uint i)
        {
            /* *
             * The range was two characters, don't include the '-'.
             * */
            if (rangeStart == i - 2)
                result.Append((char)(this.setData.Offset + i - 1));
            else if (rangeStart != i - 1)
            {
                result.Append('-');
                result.Append((char)(this.setData.Offset + i - 1));
            }
            inRange = false;
            rangeStart = 0xFFFFFFFF;
        }

        public override int GetHashCode()
        {
            return this.setData.GetHashCode();
        }

        public bool IsInverted
        {
            get { return this.setData.Inverted; }
            set
            {
                this.setData.Inverted = true;
            }
        }

        #region IEquatable<RegularLanguageBitArray> Members

        public bool Equals(RegularLanguageBitArray other)
        {
            return other == this;
        }

        #endregion

        internal uint CountTrue()
        {
            return this.setData.CountTrue();
        }

        internal RangeData GetRange()
        {
            return new RangeData(this);
        }

        #region IVisualBitSet Members

        //bool IVisualBitSet.IsEmptySet
        //{
        //    get { return this.AllFalse; }
        //}

        //bool IVisualBitSet.IsFullSet
        //{
        //    get { return this.AllTrue; }
        //}

        #endregion
    }
}
