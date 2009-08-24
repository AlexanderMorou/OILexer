using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using Oilexer.Utilities.Collections;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    /// <summary>
    /// Provides common functionality for set operations.
    /// </summary>
    internal static class SetCommon
    {
        
        /// <summary>
        /// Provides a structure for the information necessary to describe
        /// the subset of a much larger working set.
        /// </summary>
        public struct MinimalSetData
        {
            private delegate void BitwiseOperatorProcessor(uint lo, uint ro, uint ll, uint rl, uint mo, uint nl, bool li, bool ri, uint[] lws, uint[] rws, uint[] ows);
            private delegate bool BitwiseOperatorInvertPredicate(bool li, bool ri);
            /// <summary>
            /// The sub set size used by the <see cref="MinimalSetData"/>.
            /// </summary>
            internal const uint setSize = 32;
            #region MinimalSetData Properties
            /// <summary>
            /// Returns the <see cref="Int32"/> which discerns
            /// the minimal set's offset to the initial true value
            /// in the series.
            /// </summary>
            public uint Offset { get; set; }
            /// <summary>
            /// Returns the <see cref="Int32"/> which discerns
            /// the number of values in the full set.
            /// </summary>
            public uint FullSetLength { get; private set; }
            /// <summary>
            /// Returns whether the current set is inverted.
            /// </summary>
            public bool Inverted { get; set; }
            public uint[] WorkingSet;

            public int[] GetIntWorkingSet()
            {
                if (this.WorkingSet == null)
                    return null;
                int[] result = new int[this.WorkingSet.Length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = (int)this.WorkingSet[i];
                return result;
            }
            public uint Length { get; private set; }
            private uint lastMask;
            private uint lastMod;
            #endregion

            #region MinimalSetData Constructors
            /// <summary>
            /// Creates a new <see cref="MinimalSetData"/> 
            /// with the <paramref name="offset"/>, <paramref name="length"/> and
            /// <paramref name="initialSet"/> used by the <see cref="MinimalSetData"/>.
            /// </summary>
            /// <param name="offset">The <see cref="Int32"/> which discerns
            /// the minimal set's offset to the initial true value
            /// in the series.</param>
            /// <param name="fullsetLength">The <see cref="Int32"/> 
            /// which denotes the length of the full working 
            /// set of values.
            /// </param>
            /// <param name="initialSet">The <see cref="BitArray"/> 
            /// instance which represents the flags for the subset 
            /// on which elements are contained from the main 
            /// working set.</param>
            /// <remarks><see cref="Inverted"/> defaults to false.</remarks>
            [DebuggerStepThrough]
            public MinimalSetData(uint offset, uint fullsetLength, BitArray initialSet)
                : this(offset, fullsetLength, initialSet, false) { }

            /// <summary>
            /// Creates a new <see cref="MinimalSetData"/> 
            /// with the <paramref name="offset"/>, <paramref name="length"/>,
            /// <paramref name="initialSet"/> and <paramref name="inverted"/> status
            /// used by the <see cref="MinimalSetData"/>.
            /// </summary>
            /// <param name="offset">The <see cref="Int32"/> which discerns
            /// the minimal set's offset to the initial true value
            /// in the series.</param>
            /// <param name="fullsetLength">The <see cref="Int32"/> 
            /// which denotes the length of the full working 
            /// set of values.
            /// </param>
            /// <param name="initialSet">The <see cref="BitArray"/> 
            /// instance which represents the flags for the subset 
            /// on which elements are contained from the main 
            /// working set.</param>
            /// <param name="inverted">Whether the 
            /// <paramref name="resultSet"/> represents a set that
            /// is inverted.</param>
            [DebuggerStepThrough]
            public MinimalSetData(uint offset, uint fullsetLength, BitArray initialSet, bool inverted)
                : this()
            {
                this.Inverted = inverted;
                this.Offset = offset;
                this.FullSetLength = fullsetLength;
                if (initialSet != null)
                    Length = (uint)initialSet.Length;
                else
                    this.Length = 0;
                lastMod = this.Length % setSize;
                if (lastMod == 0)
                {
                    if (this.Length == 0)
                    {
                        lastMod = 0;
                        lastMask = 0;
                    }
                    else
                    {
                        lastMod = setSize;
                        lastMask = 0xFFFFFFFF;
                    }
                }
                else
                    lastMask = (uint)(1 << (int)lastMod) - 1;
                if (initialSet != null && initialSet.Length != 0)
                {
                    this.WorkingSet = new uint[(int)Math.Ceiling(((double)(this.Length)) / setSize)];
                    int[] r = new int[this.WorkingSet.Length];
                    initialSet.CopyTo(r, 0);
                    for (int i = 0; i < r.Length; i++)
                        this.WorkingSet[i] = (uint)r[i];
                }
            }

            internal MinimalSetData(MinimalSetData original)
                : this()
            {
                this.lastMask = original.lastMask;
                this.lastMod = original.lastMod;
                this.Offset = original.Offset;
                this.Length = original.Length;
                this.FullSetLength = original.FullSetLength;
                this.Inverted = original.Inverted;
                if (original.WorkingSet != null)
                {
                    this.WorkingSet = new uint[original.WorkingSet.Length];
                    original.WorkingSet.CopyTo(this.WorkingSet, 0);
                }
            }

            public bool this[uint index]
            {
                [DebuggerStepThrough]
                get
                {
                    if (!Inverted)
                    {
                        if (index < this.Offset || index >= this.Offset + this.Length)
                            throw new ArgumentOutOfRangeException("index");
                    }
                    else if (Inverted)
                        if (index < 0 || index >= this.FullSetLength)
                            throw new ArgumentOutOfRangeException("index");
                        else if (index < this.Offset || index >= this.Offset + this.Length)
                            return this.Inverted;
                    uint indexShift = index - this.Offset;
                    return ((this.WorkingSet[indexShift / setSize] & (1 << ((int)indexShift % (int)setSize))) != 0) ^ this.Inverted;
                }
                set
                {
                    if (!Inverted)
                    {
                        if (index < this.Offset || index >= this.Offset + this.Length)
                            throw new ArgumentOutOfRangeException("index");
                    }
                    else if (Inverted)
                        if (index < 0 || index >= this.FullSetLength)
                            throw new ArgumentOutOfRangeException("index");
                    uint indexShift = index - this.Offset;
                    if (value ^ Inverted)
                        this.WorkingSet[indexShift / setSize] |= (uint)(1 << (int)(indexShift % setSize));
                    else
                        /* *
                         * Set the bit of the element at the current index, 
                         * then invert the int32 value and set the working 
                         * set int at that index to the and-operation of 
                         * itself and that value.
                         * Effectively, if it's bit 0 at index zero, you get:
                         * this.WorkingSet[0] &= 0xFFFFFFFE;
                         * */
                        this.WorkingSet[indexShift / setSize] &= ~(uint)(1 << (int)(indexShift % setSize));
                }
            }

            [DebuggerStepThrough]
            public void Reduce()
            {
                if (AllTrue)
                    return;
                if (AllFalse)
                {
                    if (this.WorkingSet != null)
                        goto _Nullset;
                    else if (this.WorkingSet == null)
                        return;
                }
                uint first = uint.MaxValue;
                uint last = uint.MaxValue;
                for (uint i = 0; i < this.WorkingSet.Length; i++)
                {
                    if (this.WorkingSet[i] == 0)
                        continue;
                    uint currentSize = i == this.WorkingSet.Length - 1 ? this.lastMod : setSize;
                    for (uint j = 0; j < currentSize; j++)
                    {
                        if ((this.WorkingSet[i] & (1 << (int)j)) != 0)
                        {
                            uint curIndex = i * setSize + j;
                            if (first == uint.MaxValue)
                                first = i * (int)setSize + j;
                            last = curIndex;
                        }
                    }
                }
                if (last == uint.MaxValue)
                    goto _Nullset;

                uint finalCount = ((last - first) + 1);
                uint finalSetSize = (uint)Math.Ceiling(((double)(finalCount)) / setSize);
                if (finalSetSize == 0)
                    goto _Nullset;
                uint bitShift = first % (int)setSize;
                uint indexOffset = first / (int)setSize;
                uint checkHeight = (uint)Math.Ceiling(((double)(last + 1)) / setSize);
                uint[] resultSet = new uint[finalSetSize];
                for (uint i = indexOffset; i < checkHeight; i++)
                {
                    uint currentSize = i == this.WorkingSet.Length - 1 ? this.lastMod : setSize;
                    for (uint j = 0; j < currentSize; j++)
                    {
                        uint curIndex = i * setSize + j;
                        uint resultSetIndex = (curIndex - first) / setSize;
                        uint resultIndex = i * setSize + j - bitShift;
                        if ((this.WorkingSet[i] & (1 << (int)j)) != 0)
                            resultSet[resultSetIndex] |= (uint)(1 << (int)(resultIndex % setSize));
                    }
                }
                uint lastMod = finalCount % (int)setSize;
                if (lastMod == 0)
                {
                    this.lastMod = 32;
                    this.lastMask = 0xFFFFFFFF;
                }
                else
                {
                    this.lastMask = (uint)(1 << (int)lastMod) - 1;
                    this.lastMod = lastMod;
                }
                this.Offset += first;
                this.WorkingSet = resultSet;
                this.Length = finalCount;
                return;
            _Nullset:
                this.WorkingSet = null;
                this.Offset = 0;
                this.lastMod = 0;
                this.lastMask = 0;
                this.Length = 0;
            }

            #endregion

            [DebuggerStepThrough]
            public bool InRange(uint i)
            {
                if (this.WorkingSet == null)
                    return false;
                if (i >= this.Offset && i < this.Offset + this.Length)
                    return true;
                return false;
            }

            [DebuggerStepThrough]
            public static MinimalSetData operator ++(MinimalSetData l)
            {
                return default(MinimalSetData);
            }

            [DebuggerStepThrough]
            public MinimalSetData Clone()
            {
                uint[] ws = null;
                if (this.WorkingSet != null)
                {
                    ws = new uint[this.WorkingSet.Length];
                    this.WorkingSet.CopyTo(ws, 0);
                }
                MinimalSetData result = new MinimalSetData();
                result.WorkingSet = ws;
                result.Offset = this.Offset;
                result.Length = this.Length;
                result.FullSetLength = this.FullSetLength;
                result.Inverted = this.Inverted;
                result.lastMask = this.lastMask;
                result.lastMod = this.lastMod;
                return result;
            }

            [DebuggerStepThrough]
            private static MinimalSetData BitwiseOperator(
                           MinimalSetData  left, 
                           MinimalSetData  right,
                 BitwiseOperatorProcessor @operator,
           BitwiseOperatorInvertPredicate  inversionPredicate)
            {
                 /* left offset */   uint  lo  = left.Offset,
                /* right offset */         ro  = right.Offset,
              /* minimum offset */         mo  = Math.Min(lo, ro);
                 /* left length */   uint  ll  = left.Length,
                /* right length */         rl  = right.Length,
              /* maximum length */         ml  = Math.Max(ll, rl),
                  /* new length */         nl  = Math.Max(lo + ll, ro + rl);
               /* left inverted */   bool  li  = left.Inverted,
              /* right inverted */         ri  = right.Inverted;
        /* out Int32 set length */    int  oil = (int)(Math.Ceiling(((double)(nl - mo)) / setSize));
            /* left working set */ uint[]  lws = left.WorkingSet,
           /* right working set */         rws = right.WorkingSet,
             /* out working set */         ows = oil > 0 ? new uint[oil] : null;
                
                /* *
                 * Eases code management when setup/finalization
                 * logic is in one place.
                 * *
                 * Loops and other logic out-sourced to @operator(...).
                 * */
                @operator(lo, ro, ll, rl, mo, nl, li, ri, lws, rws, ows);
                /* *
                 * Different bitwise operations have different rules
                 * for whether the result set is inverted.
                 * */
                bool invert = inversionPredicate(li, ri);
                if (invert)
                    //If it is inverted, flip the bits of each subset.
                    for (int i = 0; i < oil; i++)
                        ows[i] = ~ows[i];
                MinimalSetData result = new MinimalSetData();
                result.Offset = mo;
                result.Length = nl - mo;
                result.Inverted = invert;
                result.FullSetLength = Math.Max(left.FullSetLength, right.FullSetLength);
                uint lastMod = result.Length % setSize;
                /* *
                 * Nonsense logic necessary for proper functionality.
                 * *
                 * Easier if calculated as little as possible. 
                 * Just in case this programmer is an idiot:
                 *      less code to change.
                 * */
                if (lastMod == 0)
                    if (nl == 0)
                    {
                        result.lastMod = 0;
                        result.lastMask = 0;
                    }
                    else
                    {
                        result.lastMask = 0xFFFFFFFF;
                        result.lastMod = setSize;
                    }
                else
                {
                    result.lastMod = lastMod;
                    result.lastMask = (uint)(1 << (int)lastMod) - 1;
                }
                if (invert && oil > 0)
                    /* *
                     * On inverted sets, fix the excessive '1' bits that shouldn't be there,
                     * but get placed due to the lazy nature of the method employed.
                     * */
                    ows[ows.Length - 1] &= result.lastMask;
                if (oil > 0)
                    result.WorkingSet = ows;
                else
                    result.WorkingSet = null;
                /* *
                 * Reduction especially necessary for exclusive operations
                 * which only select parts.
                 * */
                result.Reduce(); 
                return result;
            }

            [DebuggerStepThrough]
            public static MinimalSetData operator |(MinimalSetData left, MinimalSetData right)
            {
                return BitwiseOperator(
                    left, 
                    right, 
                    BitwiseOrProcess, 
                    (li, ri) => li || ri);
            }

            [DebuggerStepThrough]
            public static MinimalSetData operator &(MinimalSetData left, MinimalSetData right)
            {
                return BitwiseOperator(
                    left, 
                    right, 
                    BitwiseAndProcess, 
                    (li, ri) => li && ri);
            }

            [DebuggerStepThrough]
            public static MinimalSetData operator ^(MinimalSetData left, MinimalSetData right)
            {
                return BitwiseOperator(
                    left, 
                    right, 
                    BitwiseExclusiveOrProcess, 
                    (li, ri) => li != ri);
            }

            [DebuggerStepThrough]
            private static void BitwiseOrProcess(uint lo, uint ro, uint ll, uint rl, uint mo, uint nl, bool li, bool ri, uint[] lws, uint[] rws, uint[] ows)
            {
                if (ows == null)
                    return;
                for (uint i = mo; i < nl; i++)
                {
                    uint lri = i - lo, //left relative index.
                         rri = i - ro, //right relative index.
                         ori = i - mo; //out relative index.
                    if (((lri >= 0 && lri < ll) && ((lws[lri / setSize] & (1 << (int)(lri % setSize))) != 0)) ^ li ||
                        ((rri >= 0 && rri < rl) && ((rws[rri / setSize] & (1 << (int)(rri % setSize))) != 0)) ^ ri)
                        ows[ori / setSize] |= (uint)(1 << ((int)ori % (int)setSize));
                }
            }

            [DebuggerStepThrough]
            private static void BitwiseAndProcess(uint lo, uint ro, uint ll, uint rl, uint mo, uint nl, bool li, bool ri, uint[] lws, uint[] rws, uint[] ows)
            {
                if (ows == null)
                    return;
                for (uint i = mo; i < nl; i++)
                {
                    uint lri = i - lo, //left relative index.
                         rri = i - ro, //right relative index.
                         ori = i - mo; //out relative index.
                    if (((lri >= 0 && lri < ll) && ((lws[lri / setSize] & (1 << (int)(lri % setSize))) != 0)) ^ li &&
                        ((rri >= 0 && rri < rl) && ((rws[rri / setSize] & (1 << (int)(rri % setSize))) != 0)) ^ ri)
                        ows[ori / setSize] |= (uint)(1 << ((int)ori % (int)setSize));
                }
            }

            [DebuggerStepThrough]
            private static void BitwiseExclusiveOrProcess(uint lo, uint ro, uint ll, uint rl, uint mo, uint nl, bool li, bool ri, uint[] lws, uint[] rws, uint[] ows)
            {
                if (ows == null)
                    return;
                for (uint i = mo; i < nl; i++)
                {
                    uint lri = i - lo, //left relative index.
                         rri = i - ro, //right relative index.
                         ori = i - mo; //out relative index.
                    if (((lri >= 0 && lri < ll) && ((lws[lri / setSize] & (1 << (int)(lri % setSize))) != 0)) ^ li ^
                        ((rri >= 0 && rri < rl) && ((rws[rri / setSize] & (1 << (int)(rri % setSize))) != 0)) ^ ri)
                        ows[ori / setSize] |= (uint)(1 << (int)(ori % setSize));
                }
            }

            /// <summary>
            /// Returns whether the values of the current <see cref="MinimalSetData"/>
            /// all false.
            /// </summary>
            /// <returns>true if all elements are false, when <see cref="Inverted"/>
            /// no all values in the <see cref="MinimalSetData"/> must be
            /// true.  If <see cref="WorkingSet"/> is null returns false if 
            /// <see cref="Inverted"/> is true.</returns>
            public bool AllFalse
            {
                [DebuggerStepThrough]
                get
                {
                    if (this.WorkingSet == null)
                        return !this.Inverted;
                    for (uint i = 0; i < this.WorkingSet.Length - 1; i++)
                        if (WorkingSet[i] != 0)
                            return false;
                    return (WorkingSet[WorkingSet.Length - 1] & this.lastMask) == 0;
                }
            }

            /// <summary>
            /// Returns whether the values of the current <see cref="MinimalSetData"/>
            /// all true.
            /// </summary>
            /// <returns>true if all elements are true, when <see cref="Inverted"/>
            /// no all values in the <see cref="MinimalSetData"/> must be
            /// false.  If <see cref="WorkingSet"/> is null returns true if 
            /// <see cref="Inverted"/> is true.</returns>
            public bool AllTrue
            {
                [DebuggerStepThrough]
                get
                {
                    if (this.WorkingSet == null)
                        return this.Inverted;
                    for (int i = 0; i < this.WorkingSet.Length - 1; i++)
                        if (WorkingSet[i] != 0xFFFFFFFF)
                            return false;
                    return ((WorkingSet[this.WorkingSet.Length - 1] & lastMask) == lastMask);
                }
            }

            /// <summary>
            /// Returns the number of values within the primary set that are true.
            /// </summary>
            /// <returns></returns>
            [DebuggerStepThrough]
            public uint CountTrue()
            {
                if (this.WorkingSet == null)
                    if (this.Inverted)
                        return this.FullSetLength;
                    else
                        return 0;
                uint r = 0;
                for (int i = 0; i < this.WorkingSet.Length - 1; i++)
                    for (int j = 0; j < 32; j++)
                        if (((WorkingSet[i] & (1 << j)) != 0) ^ this.Inverted)
                            r++;
                for (int i = 0; i < lastMod; i++)
                    if (((WorkingSet[WorkingSet.Length - 1] & (1 << i)) != 0) ^ this.Inverted)
                        r++;
                if (this.Inverted)
                    return this.FullSetLength - r;
                else
                    return r;
            }

            [DebuggerStepThrough]
            public override bool Equals(object obj)
            {
                if (obj is MinimalSetData)
                    return SetCommon.SetsEqual(this, ((MinimalSetData)(obj)));
                return false;
            }

            [DebuggerStepThrough]
            public override int GetHashCode()
            {
                int result = 0;
                result |= this.Length.GetHashCode();
                if (this.WorkingSet != null)
                {
                    for (int i = 0; i < WorkingSet.Length; i++)
                        result ^= WorkingSet[i].GetHashCode();
                }
                return result ^ (int)this.Offset ^ (Inverted ? 1 : 0) ^ (int)(this.FullSetLength ^ this.Length);
            }
        }

        /// <summary>
        /// Reduces the <paramref name="currentSet"/> into its minimal form should it have
        /// changed.
        /// </summary>
        /// <param name="currentSet">The <see cref="MinimalSetData"/> which describes the
        /// current set in its present form.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static MinimalSetData Reduce(this MinimalSetData currentSet)
        {
            MinimalSetData result = currentSet.Clone();
            result.Reduce();
            return result;
        }

        /// <summary>
        /// Performs a bitwise or operation on the <paramref name="left"/> and
        /// <paramref name="right"/> operands.
        /// </summary>
        /// <param name="left">The <see cref="MinimalSetData"/> which describes
        /// the left side of the equation.</param>
        /// <param name="right">The <see cref="MinimalSetData"/> which describes the
        /// right side of the equation.</param>
        /// <returns>A new <see cref="MinimalSetData"/> which represents
        /// the minimal form of a union between the two sets.</returns>
        public static MinimalSetData BitwiseOr(MinimalSetData left, MinimalSetData right)
        {
            return left | right;
        }

        /// <summary>
        /// Performs a bitwise and operation on the <paramref name="left"/> and
        /// <paramref name="right"/> operands.
        /// </summary>
        /// <param name="left">The <see cref="MinimalSetData"/> which describes
        /// the left side of the equation.</param>
        /// <param name="right">The <see cref="MinimalSetData"/> which describes the
        /// right side of the equation.</param>
        /// <returns>A new <see cref="MinimalSetData"/> which represents
        /// the minimal form of the intersection of two sets.</returns>
        public static MinimalSetData BitwiseAnd(MinimalSetData left, MinimalSetData right)
        {
            return left & right;
        }

        /// <summary>
        /// Performs a bitwise and operation on the <paramref name="left"/> and
        /// <paramref name="right"/> operands.
        /// </summary>
        /// <param name="left">The <see cref="MinimalSetData"/> which describes
        /// the left side of the equation.</param>
        /// <param name="right">The <see cref="MinimalSetData"/> which describes the
        /// right side of the equation.</param>
        /// <returns>A new <see cref="MinimalSetData"/> which represents
        /// the minimal form of the compliment of two sets.</returns>
        public static MinimalSetData BitwiseExclusiveOr(MinimalSetData left, MinimalSetData right)
        {
            return left ^ right;
        }

        /// <summary>
        /// Returns whether two <see cref="MinimalSetData"/> structures are equal to one another.
        /// </summary>
        /// <param name="left">The <see cref="MinimalSetData"/> which describes
        /// the left side of the equation.</param>
        /// <param name="right">The <see cref="MinimalSetData"/> which describes the
        /// right side of the equation.</param>
        /// <returns>true if the <paramref name="left"/> and <paramref name="right"/> are,
        /// value wise, equivalent.</returns>
        public static bool SetsEqual(MinimalSetData left, MinimalSetData right)
        {
            if ((left.WorkingSet == null && left.Inverted) && right.WorkingSet != null)
                return false;
            else if (left.WorkingSet != null && right.WorkingSet == null && right.Inverted)
                return false;
            if (left.WorkingSet == null && right.WorkingSet == null &&
                left.Inverted && right.Inverted)
                return true;
            else if (left.WorkingSet == null && right.WorkingSet == null &&
                     left.Inverted == right.Inverted)
                return true;
            if (left.Offset != right.Offset)
                return false;
            if (left.Length != right.Length)
                return false;
            if (left.Inverted != right.Inverted)
                return false;
            uint[] leftSet = left.WorkingSet;
            uint[] rightSet = right.WorkingSet;

            for (int i = 0; i < leftSet.Length; i++)
                if (leftSet[i] != rightSet[i])
                    return false;
            return true;
        }

        public static bool SetsNotEqual(MinimalSetData left, MinimalSetData right)
        {
            if (left.Offset != right.Offset)
                return true;
            if (left.WorkingSet == null && left.Inverted && right.WorkingSet != null)
                return true;
            else if (left.WorkingSet != null && right.WorkingSet == null && right.Inverted)
                return true;
            else if (left.WorkingSet == null && right.WorkingSet == null &&
                     left.Inverted == right.Inverted)
                return false;
            if (left.Length != right.Length)
                return true;
            for (int i = 0; i < left.WorkingSet.Length; i++)
                if (left.WorkingSet[i] != right.WorkingSet[i])
                    return true;
            return false;
        }
        /// <summary>
        /// Obtains the subset of a given series for the <paramref name="sourceData"/> and 
        /// <paramref name="fullSource"/> provided.
        /// </summary>
        /// <typeparam name="T">The type of elements in the full set and to return.</typeparam>
        /// <param name="sourceData">The <see cref="SetCommon.MinimalSetData"/> which indicates
        /// which elements of the <paramref name="fullSet"/> to return.</param>
        /// <param name="fullSet">The series of <typeparamref name="T"/> instances
        /// that relates to the <paramref name="sourceData"/> bits to return.</param>
        /// <returns>An array of <typeparamref name="T"/> instances according to the 
        /// bit-flags noted by the <paramref name="sourceData"/> relative to the <paramref name="fullSet"/>
        /// provided.</returns>
        public static T[] GetSubSet<T>(this SetCommon.MinimalSetData sourceData, T[] fullSet)
        {
            int currentElementIndex = 0;
            T[] result = new T[sourceData.CountTrue()];
            for (int i = 0; i < fullSet.Length; i++)
                if (sourceData.InRange((uint)i) && sourceData[(uint)i])
                    result[currentElementIndex++] = fullSet[i];
            return result;
        }

    }
}
