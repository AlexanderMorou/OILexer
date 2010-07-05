using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

#if x64
using SlotType = System.UInt64;
#elif x86
using SlotType = System.UInt32;
#endif
namespace Oilexer.FiniteAutomata
{
    using OperationSignature = Action<uint, uint, uint, uint, uint, uint, bool, bool, SlotType[], SlotType[], SlotType[]>;
    using System.Threading;
    /// <summary>
    /// Provides a base implementation of a 
    /// <see cref="IFiniteAutomataSet"/> whose individual elements
    /// from the full set are represented by a single bit.
    /// </summary>
    /// <typeparam name="TCheck">The direct inheritor of the 
    /// <see cref="FiniteAutomataBitSet{TCheck}"/> used as a result
    /// of set operations.</typeparam>
    public partial class FiniteAutomataBitSet<TCheck> :
        IFiniteAutomataSet<TCheck>
        where TCheck :
            FiniteAutomataBitSet<TCheck>,
            new()
    {
        
        /// <summary>
        /// Data member for <see cref="IsNegativeSet"/>.
        /// </summary>
        private bool isNegativeSet;
        /// <summary>
        /// Data member for <see cref="Offset"/>.
        /// </summary>
        private uint offset;
        /// <summary>
        /// Data member for <see cref="FullLength"/>
        /// </summary>
        private uint fullLength;
        /// <summary>
        /// Data member for <see cref="Length"/>.
        /// </summary>
        private uint length;
        /// <summary>
        /// Data member for <see cref="Complement"/>.
        /// </summary>
        private TCheck complement;

        private static Dictionary<TCheck, Dictionary<TCheck, TCheck>> IntersectCache = new Dictionary<TCheck, Dictionary<TCheck, TCheck>>();
        private static Dictionary<TCheck, Dictionary<TCheck, TCheck>> SymmetricDifferenceCache = new Dictionary<TCheck, Dictionary<TCheck, TCheck>>();
        private static Dictionary<TCheck, Dictionary<TCheck, TCheck>> UnionCache = new Dictionary<TCheck, Dictionary<TCheck, TCheck>>();
        internal static TCheck NullInst;
        private static int unionCacheHits = 0;
        private static int intersectionCacheHits = 0;
        private static int symmetricDifferenceCacheHits = 0;

#if x86 
        internal const SlotType SlotBitCount = 0x20;
        internal const SlotType ShiftValue = 1U;
#elif x64
        internal const SlotType SlotBitCount = 0x40;
        internal const SlotType ShiftValue = 1UL;
#endif 
        private static object cacheLock = new object();
        private const SlotType SlotMaxValue = SlotType.MaxValue;
        private static OperationSignature unionOp = UnionProcess;
        private static OperationSignature intersectOp = IntersectionProcess;
        private static OperationSignature symmetricDifferenceOp = SymmetricDifferenceProcess;
        private SlotType[] values;
        private SlotType lastMask;
        private SlotType lastMod;
        protected void Set(SlotType[] values, uint offset, uint length, uint fullLength) { this.Set(values, offset, length, fullLength, false); }
        protected void Set(SlotType[] values, uint offset, uint length, uint fullLength, bool isNegativeSet) { this.Set(values, offset, length, fullLength, isNegativeSet, true); }
        protected void Set(SlotType[] values, uint offset, uint length, uint fullLength, bool isNegativeSet, bool reduce)
        {
            this.isNegativeSet = isNegativeSet;
            this.values = values;
            this.offset = offset;
            this.length = length;
            this.lastMod = SlotBitCount;
            this.lastMask = SlotMaxValue;
            this.fullLength = fullLength;
            if (reduce)
                this.Reduce();
        }
        private static bool gettingSD = false;
        public FiniteAutomataBitSet()
        {

            if (NullInst == null && !gettingSD)
            {
                gettingSD = true;
                NullInst = this.GetCheck();
            }
        }

        /// <summary>
        /// Obtains a new <typeparamref name="TCheck"/> instance
        /// for set operations.
        /// </summary>
        /// <returns>A new <typeparamref name="TCheck"/> instance.</returns>
        /// <remarks>Inheritors override if there is initialization
        /// code necessary for proper function.</remarks>
        protected virtual TCheck GetCheck()
        {
            return new TCheck();
        }

        /// <summary>
        /// Returns whether the <paramref name="obj"/> provided is
        /// equal to the current <see cref="FiniteAutomataBitSet{TCheck}"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare
        /// against.</param>
        /// <returns>true if the subset information from the
        /// <paramref name="obj"/> is equal to the current 
        /// <see cref="FiniteAutomationBitSet{TCheck}"/>;
        /// and false, if <paramref name="obj"/> is not an instance
        /// of <typeparamref name="TCheck"/>, or otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TCheck))
                return false;
            return this.Equals((TCheck)obj);
        }

        /// <summary>
        /// Obtains the hash for the current <see cref="FiniteAutomataBitset{TCheck}"/>.
        /// </summary>
        /// <returns>A <see cref="Int32"/> value representing the 
        /// bitwise hash of the current <see cref="FiniteAutomataBitset{TCheck}"/>.</returns>
        public override int GetHashCode()
        {
            int r = (int)(this.offset ^ this.length);
            if (this.values != null) {
                r ^= ~this.values.Length;
            }
                //for (int i = 0; i < this.values.Length; i++)
                //    r ^= this.values[i].GetHashCode();
            return r;
        }

        #region IFiniteAutomataSet<TCheck> Members

        /// <summary>
        /// Determines whether the current <see cref="FiniteAutomataBitSet{TCheck}"/>
        /// contains no elements from the full set.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return ((this.values == null) || (this.length == 0)) && (!(this.isNegativeSet));
            }
        }

        /// <summary>
        /// Creates a union of the current <see cref="FiniteAutomataBitSet{TCheck}"/> and the
        /// <paramref name="other"/> provided.
        /// </summary>
        /// <param name="other">The other <typeparamref name="TCheck"/> to
        /// create a union with.</param>
        /// <returns>A new <typeparamref name="TCheck"/> which contains
        /// the elements of both the current instance and the 
        /// <paramref name="other"/> instance provided.</returns>
        public TCheck Union(TCheck other)
        {
            var tThis = (TCheck)this;
            if (Equals(tThis, other))
                return tThis;
            if (other == null)
                return (TCheck)this;
            return SetOperation((TCheck)(this), other, unionOp, (li, ri) => li || ri, GetCheck, ref unionCacheHits, UnionCache);
        }

        /// <summary>
        /// Creates an intersection of the current <see cref="FiniteAutomataBitSet{TCheck}"/>
        /// and the <paramref name="other"/> provided.
        /// </summary>
        /// <param name="other">The <typeparamref name="TCheck"/> to make the 
        /// intersection of.</param>
        /// <returns>A new <typeparamref name="TCheck"/> which represents
        /// the points of overlap between the current <see cref="FiniteAutomataBitSet{TCheck}"/>
        /// and the <paramref name="other"/> provided.</returns>
        public TCheck Intersect(TCheck other)
        {
            var tThis = (TCheck)this;
            if (Equals(tThis, other))
                return tThis;
            if (other == null)
            {
                var result = new TCheck();
                result.Set(null, 0, 0, this.FullLength);
                return result;
            }
            return SetOperation(tThis, other, intersectOp, (li, ri) => li && ri, GetCheck, ref intersectionCacheHits, IntersectCache);
        }
        /// <summary>
        /// Returns the complement of the current <see cref="FiniteAutomataBitSet{TCheck}"/>.
        /// </summary>
        /// <returns>A new <typeparamref name="TCheck"/> which represents the
        /// complement of the current <see cref="FiniteAutomataBitSet{TCheck}"/>.</returns>
        public TCheck Complement()
        {
            if (this.complement == null)
            {
                this.complement = GetCheck();
                this.complement.values = this.values;
                this.complement.offset = this.offset;
                this.complement.length = this.length;
                this.complement.lastMask = this.lastMask;
                this.complement.lastMod = this.lastMod;
                this.complement.fullLength = this.fullLength;
                this.complement.complement = (TCheck)this;
                this.complement.isNegativeSet = !this.isNegativeSet;
            }
            return this.complement;
        }

        /// <summary>
        /// Creates a new set which represents the symmetric
        /// difference between the current <see cref="FiniteAutomataBitSet{TCheck}"/> 
        /// and the <paramref name="other"/> provided.
        /// </summary>
        /// <param name="other">The <typeparamref name="TCheck"/> to make the
        /// symmetric difference of.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance which represents the
        /// elements from both sets that are mutually exclusive between the two.</returns>
        public TCheck SymmetricDifference(TCheck other)
        {
            var tThis = (TCheck)this;
            if (Equals(tThis, other))
                return NullInst;
            if (other == null)
                return (TCheck)this;
            return SetOperation((TCheck)this, other, symmetricDifferenceOp, (li, ri) => li ^ ri, GetCheck, ref symmetricDifferenceCacheHits, SymmetricDifferenceCache);
        }

        /// <summary>
        /// Creates a new set which represents the relative complement
        /// of the <paramref name="other"/> <typeparamref name="TCheck"/> instance
        /// relative to the current <see cref="FiniteAutomataBitSet{TCheck}"/>.
        /// </summary>
        /// <param name="other">The <typeparamref name="TCheck"/> to
        /// use to create the symmetric difference to intersect.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance which represents
        /// the elements exclusive to the current 
        /// <see cref="FiniteAutomataBitSet{TCheck}"/>.
        /// </returns>
        /// <remarks><para>A&#8898;&#8705;B</para><para>
        /// i.e. The intersection of A and the complement of B.</para></remarks>
        public TCheck RelativeComplement(TCheck other)
        {
            return (this & other.Complement());
        }

        #endregion

        #region IEquatable<TCheck> Members

        /// <summary>
        /// Returns whether the <paramref name="other"/> is equal to the
        /// current <see cref="FiniteAutomataBitSet{TCheck}"/>.
        /// </summary>
        /// <param name="other">The <typeparamref name="TCheck"/>
        /// to compare against.</param>
        /// <returns>true if the subset information from the <paramref name="other"/>
        /// is equal to the current <see cref="FiniteAutomationBitSet{TCheck}"/></returns>
        public bool Equals(TCheck other)
        {
            return Equals((TCheck)this, (TCheck)other);
        }

        #endregion

        /// <summary>
        /// Returns the number of bits within the subset.
        /// </summary>
        public uint Length
        {
            get
            {
                return this.length;
            }
        }

        /// <summary>
        /// Returns the number of bits within the full set.
        /// </summary>
        public uint FullLength
        {
            get
            {
                return this.fullLength;
            }
            internal set
            {
                this.fullLength = value;
            }
        }

        /// <summary>
        /// Returns the number of bits from the full set
        /// which are skipped because no pertinent data exists.
        /// </summary>
        public uint Offset
        {
            get
            {
                return this.offset;
            }
        }

        /// <summary>
        /// Returns whether the current subset is a negative set.
        /// </summary>
        public bool IsNegativeSet
        {
            get
            {
                return this.isNegativeSet;
            }
            internal set
            {
                this.isNegativeSet = value;
            }
        }

        /// <summary>
        /// Returns whether the element at the specified
        /// <paramref name="index"/> is within the current
        /// subset.
        /// </summary>
        /// <param name="index">The <see cref="Int32"/> value relative
        /// to the full set to check for within the current
        /// subset.</param>
        /// <returns>true if the element at the <paramref name="index"/>
        /// provided is within the current subset; false, otherwise.</returns>
        /// <remarks>Adjusted to accomodate whether the
        /// current <see cref="FiniteAutomataBitSet{TCheck}"/>
        /// is a negative set.</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when the
        /// <paramref name="index"/> provided is less than zero or greater than
        /// the length of the full set in which the <see cref="FiniteAutomataBitSet{TCheck}"/>
        /// is a subset of.</exception>
        public bool this[uint index]
        {
            get
            {
                if (index < 0 || index >= this.fullLength)
                    throw new ArgumentOutOfRangeException("index");
                else if (index < this.offset || index >= this.offset + this.length)
                    return this.isNegativeSet;
                SlotType indexShift = ((SlotType)(index - this.Offset));
                return ((this.values[indexShift / SlotBitCount] & (SlotType)(ShiftValue << ((int)(indexShift % SlotBitCount)))) != 0) ^ this.isNegativeSet;
            }
        }

        internal bool GetThisInternal(uint index)
        {
            if (index < this.Offset || index >= this.Offset + this.Length)
                throw new ArgumentOutOfRangeException("index");
            SlotType indexShift = ((SlotType)(index - this.Offset));
            return ((this.values[indexShift / SlotBitCount] & (SlotType)(ShiftValue << ((int)(indexShift % SlotBitCount)))) != 0);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> 
        /// <typeparamref name="TCheck"/> instance and the 
        /// <paramref name="right"/> <typeparamref name="TCheck"/>
        /// instance have equal subset data.
        /// </summary>
        /// <param name="left">The left <typeparamref name="TCheck"/> operand.</param>
        /// <param name="right">The right <typeparamref name="TCheck"/> operand.</param>
        /// <returns>true if the <paramref name="left"/> and <paramref name="right"/>
        /// operands have equal subset information.</returns>
        public static bool Equals(TCheck left, TCheck right)
        {
            if (left == null)
                return right == null;
            else if (right == null)
                return false;
            if ((left.values == null && left.isNegativeSet) && right.values != null)
                return false;
            else if (left.values != null && right.values == null && right.isNegativeSet)
                return false;
            if (left.values == null && right.values == null &&
                left.isNegativeSet && right.isNegativeSet)
                return true;
            else if (left.values == null && right.values == null &&
                     left.isNegativeSet == right.isNegativeSet)
                return true;
            if (left.offset != right.offset)
                return false;
            if (left.length != right.length)
                return false;
            if (left.isNegativeSet != right.isNegativeSet)
                return false;
            SlotType[] leftSet = left.values;
            SlotType[] rightSet = right.values;
            for (int i = 0; i < leftSet.Length; i++)
                if (leftSet[i] != rightSet[i])
                    return false;
            return true;
        }

        private static TCheck SetOperation(
            TCheck left, TCheck right, 
            Action<uint, uint, uint, uint, uint, uint, bool, bool, SlotType[], SlotType[], SlotType[]> @operator,
            Func<bool, bool, bool> inversionPredicate,
            Func<TCheck> checkCreator,
            ref int cacheHits,
            Dictionary<TCheck, Dictionary<TCheck, TCheck>> dictionaryCache)
        {

            Dictionary<TCheck, TCheck> leftCache;
            Dictionary<TCheck, TCheck> rightCache;
            lock (dictionaryCache)
            {
                if (!dictionaryCache.TryGetValue(left, out leftCache))
                    dictionaryCache.Add(left, leftCache = new Dictionary<TCheck, TCheck>());
                if (!dictionaryCache.TryGetValue(right, out rightCache))
                    dictionaryCache.Add(right, rightCache = new Dictionary<TCheck, TCheck>());
            }
            TCheck operation;
            bool leftFlag = false;
            try
            {
                Monitor.Enter(leftCache, ref leftFlag);
                if (!leftCache.TryGetValue(right, out operation))
                {

                /* left offset */   uint     lo  = left.offset,
               /* right offset */            ro  = right.offset,
             /* minimum offset */            mo  = Math.Min(lo, ro);
                /* left length */   uint     ll  = left.length,
               /* right length */            rl  = right.length,
                 /* new length */            nl  = Math.Max(lo + ll, ro + rl);
              /* left inverted */   bool     li  = left.isNegativeSet,
             /* right inverted */            ri  = right.isNegativeSet;
       /* out Int32 set length */   uint     oil = (uint)(Math.Ceiling(((double)(nl - mo)) / SlotBitCount));
           /* left working set */ SlotType[] lws = left.values,
          /* right working set */            rws = right.values,
            /* out working set */            ows = oil > 0 ? new SlotType[oil] : null;

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
                    operation = checkCreator();
                    operation.offset = mo;
                    operation.length = nl - mo;
                    operation.isNegativeSet = invert;
                    operation.fullLength = Math.Max(left.fullLength, right.fullLength);

                    SlotType lastMod = (SlotType)operation.length % SlotBitCount;
                    if (lastMod == 0)
                        if (nl == 0)
                        {
                            operation.lastMod = 0;
                            operation.lastMask = 0;
                        }
                        else
                        {
                            operation.lastMask = SlotMaxValue;
                            operation.lastMod = SlotBitCount;
                        }
                    else
                    {
                        operation.lastMod = lastMod;
                        operation.lastMask = (SlotType)(ShiftValue << (int)lastMod) - 1;
                    }
                    if (invert && oil > 0)
                        /* *
                            * On inverted sets, fix the excessive '1' bits that shouldn't be there,
                            * but get placed due to the lazy nature of the method employed.
                            * */
                        ows[ows.Length - 1] &= operation.lastMask;
                    if (oil > 0)
                        operation.values = ows;
                    else
                        operation.values = null;
                    /* *
                        * Reduction especially necessary for exclusive operations
                        * which only select parts.
                        * */
                    operation.Reduce();
                    if (operation.IsEmpty)
                    {
                        operation.values = null;
                        operation = NullInst;
                    }
                    Monitor.Pulse(leftCache);
                    leftCache.Add(right, operation);
                    bool rightLock = false;
                    try
                    {
                        Monitor.Enter(rightCache, ref rightLock);
                        rightCache.Add(left, operation);
                    }
                    finally
                    {
                        if (rightLock)
                            Monitor.Exit(rightCache);
                    }
                }
                else
                {
                    lock (cacheLock)
                        cacheHits++;
                }
            }
            finally
            {
                if (leftFlag)
                    Monitor.Exit(leftCache);
            }
            return operation;
        }

        internal virtual void Reduce()
        {
            //Skip instances where there's nothing to do.
            if (IsEmpty)
                return;
            if (AllTrue)
                return;
            if (AllFalse)
            {
                if (this.values != null)
                {
                    this.isNegativeSet = false;
                    goto _Nullset;
                }
                return;
            }
            SlotType first = SlotMaxValue;
            SlotType last = SlotMaxValue;
            for (SlotType i = 0; i < (SlotType)this.values.Length; i++)
            {
                if (this.values[i] == 0)
                    continue;
                SlotType currentSize = i == (SlotType)(this.values.Length - 1) ? this.lastMod : SlotBitCount;
                for ( uint j = 0; j < currentSize; j++)
                {
                    if (((this.values[i] & (SlotType)(ShiftValue << (int)j)) != 0))
                    {
                        SlotType curIndex = i * SlotBitCount + j;
                        if (first == SlotMaxValue)
                            first = i * SlotBitCount + j;
                        last = curIndex;
                    }
                }
            }
            /* *
             * Make sure the reduction is necessary.
             * */
            if (first == 0 &&
                last == (this.length - 1))
                return;
            /* *
             * If there was no element hit, there's no data.
             * It's highly unlikely the current subset is  
             * 2^[32 or 64] elements.
             * */
            if (last == SlotMaxValue)
                goto _Nullset;
            SlotType   finalCount = ((last - first) + 1);
            SlotType   finalSetSize = (SlotType)Math.Ceiling(((double)(finalCount)) / SlotBitCount);
            if (finalSetSize == 0)
                goto _Nullset;
            SlotType   bitShift = first % (int)SlotBitCount;
            SlotType   indexOffset = first / (int)SlotBitCount;
            SlotType   checkHeight = (SlotType)Math.Ceiling(((double)(last + 1)) / SlotBitCount);
            SlotType[] resultSet = new SlotType[finalSetSize];
            for (SlotType i = indexOffset; i < checkHeight; i++)
            {
                SlotType currentSize = i == (SlotType)(this.values.Length - 1) ? this.lastMod : SlotBitCount;
                for (uint j = 0; j < currentSize; j++)
                {
                    SlotType curIndex = i * SlotBitCount + j;
                    SlotType resultSetIndex = (curIndex - first) / SlotBitCount;
                    SlotType resultIndex = i * SlotBitCount + j - bitShift;
                    if ((this.values[i] & (SlotType)(ShiftValue << (int)j)) != 0)
                        resultSet[resultSetIndex] |= (SlotType)(ShiftValue << (int)(resultIndex % SlotBitCount));
                }
            }
            SlotType lastMod = finalCount % (int)SlotBitCount;
            if (lastMod == 0)
            {
                this.lastMask = SlotMaxValue;
                this.lastMod  = SlotBitCount;
            }
            else
            {
                this.lastMask = (SlotType)(ShiftValue << (int)lastMod) - 1;
                this.lastMod = lastMod;
            }
            this.offset += (uint)first;
            this.values = resultSet;
            this.length = (uint)finalCount;
            return;
        _Nullset:
            this.values = null;
            this.offset = 0;
            this.lastMod = 0;
            this.lastMask = 0;
            this.length = 0;
        }

        /// <summary>
        /// Returns whether the values of the current <see cref="FiniteAutomataBitSet{TCheck}"/>
        /// all false.
        /// </summary>
        /// <returns>true if all elements are false, when <see cref="IsNegativeSet"/>
        /// all values in the <see cref="FiniteAutomataBitSet{TCheck}"/> must be
        /// true.  If the <see cref="values"/> are null returns false if 
        /// <see cref="IsNegativeSet"/> is true.</returns>
        public bool AllFalse
        {
            get
            {
                if (this.values == null)
                    return !this.isNegativeSet;
                SlotType exclusivePoint = this.values.Length < 2 ? 0 : (SlotType)(this.values.Length - 1);
                for (SlotType i = 0; i < exclusivePoint; i++)
                    if (this.IsNegativeSet)
                    {
                        if (this.values[i] != SlotMaxValue)
                            return false;
                    }
                    else if (this.values[i] != SlotType.MinValue)
                        return false;
                if (this.IsNegativeSet)
                    return (this.values[this.values.Length - 1] & this.lastMask) == this.lastMask;
                else
                    return (this.values[this.values.Length - 1] & this.lastMask) == 0;
            }
        }

        /// <summary>
        /// Returns whether the values of the current <see cref="FiniteAutomataBitSet{TCheck}"/>
        /// all true.
        /// </summary>
        /// <returns>true if all elements are true, when <see cref="IsNegativeSet"/>
        /// all values in the <see cref="FiniteAutomataBitSet{TCheck}"/> must be
        /// false.  If the associated <see cref="values"/> are null returns true if 
        /// <see cref="IsNegativeSet"/> is true.</returns>
        public bool AllTrue
        {
            get
            {
                if (this.values == null)
                    return this.isNegativeSet;
                SlotType exclusivePoint = this.values.Length < 2 ? 0 : (SlotType)(this.values.Length - 1);
                for (SlotType i = 0; i < exclusivePoint; i++)
                    if (this.IsNegativeSet)
                    {
                        if (this.values[i] != SlotType.MinValue)
                            return false;
                    }
                    else if (this.values[i] != SlotMaxValue)
                        return false;
                return ((this.values[this.values.Length - 1] & lastMask) == lastMask);
            }
        }

        [DebuggerStepThrough]
        private static void UnionProcess(uint lo, uint ro, uint ll, uint rl, uint mo, uint nl, bool li, bool ri, SlotType[] lws, SlotType[] rws, SlotType[] ows)
        {
            if (ows == null)
                return;
            for (SlotType i = mo; i < nl; i++)
            {
                SlotType lri = i - lo, //left relative index.
                         rri = i - ro, //right relative index.
                         ori = i - mo; //out relative index.
                if (((lri >= 0 && lri < ll) && ((lws[lri / SlotBitCount] & (SlotType)(ShiftValue << (int)(lri % SlotBitCount))) != 0)) ^ li ||
                    ((rri >= 0 && rri < rl) && ((rws[rri / SlotBitCount] & (SlotType)(ShiftValue << (int)(rri % SlotBitCount))) != 0)) ^ ri)
                    ows[ori / SlotBitCount] |= (SlotType)(ShiftValue << (int)(ori % SlotBitCount));
            }
        }

        private static void IntersectionProcess(uint lo, uint ro, uint ll, uint rl, uint mo, uint nl, bool li, bool ri, SlotType[] lws, SlotType[] rws, SlotType[] ows)
        {
            if (ows == null)
                return;
            for (SlotType i = mo; i < nl; i++)
            {
                int lri = (int)(i - lo), //left relative index.
                    rri = (int)(i - ro), //right relative index.
                    ori = (int)(i - mo); //out relative index.
                if (((lri >= 0 && lri < ll) && ((lws[(SlotType)lri / SlotBitCount] & (SlotType)(ShiftValue << (int)((SlotType)lri % SlotBitCount))) != 0)) ^ li &&
                    ((rri >= 0 && rri < rl) && ((rws[(SlotType)rri / SlotBitCount] & (SlotType)(ShiftValue << (int)((SlotType)rri % SlotBitCount))) != 0)) ^ ri)
                    ows[(SlotType)ori / SlotBitCount] |= (SlotType)(ShiftValue << (int)((SlotType)ori % SlotBitCount));
            }
        }

        [DebuggerStepThrough]
        private static void SymmetricDifferenceProcess(uint lo, uint ro, uint ll, uint rl, uint mo, uint nl, bool li, bool ri, SlotType[] lws, SlotType[] rws, SlotType[] ows)
        {
            if (ows == null)
                return;
            for (SlotType i = mo; i < nl; i++)
            {
                SlotType lri = i - lo, //left relative index.
                         rri = i - ro, //right relative index.
                         ori = i - mo; //out relative index.
                if (((lri >= 0 && lri < ll) && ((lws[lri / SlotBitCount] &  (SlotType)(ShiftValue  << (int)(lri % SlotBitCount))) != 0)) ^ li ^
                    ((rri >= 0 && rri < rl) && ((rws[rri / SlotBitCount] & (SlotType)(ShiftValue << (int)(rri % SlotBitCount))) != 0)) ^ ri)
                    ows[ori / SlotBitCount] |= (SlotType)(ShiftValue << (int)(ori % SlotBitCount));
            }
        }

        /// <summary>
        /// Performs a bitwise or operation on the <paramref name="left"/>
        /// and <paramref name="right"/> sets returning a new <typeparamref name="TCheck"/>
        /// instance of the elements from both the <paramref name="left"/> set
        /// and the <paramref name="right"/> set.
        /// </summary>
        /// <param name="left">The left <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <param name="right">The right <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance
        /// as a union of the <paramref name="left"/> and
        /// <paramref name="right"/> sets.</returns>
        public static TCheck operator |(FiniteAutomataBitSet<TCheck> left, FiniteAutomataBitSet<TCheck> right)
        {
            if (Object.ReferenceEquals(left, null))
                if (right is TCheck)
                    return (TCheck)right;
                else
                    throw new ArgumentException(string.Format("left or right must be an instance of TCheck ({0})", typeof(TCheck).Name), "right");
            else if (Object.ReferenceEquals(right, null))
                if (left is TCheck)
                    return (TCheck)left;
                else
                    throw new ArgumentException(string.Format("left or right must be an instance of TCheck ({0})", typeof(TCheck).Name), "left");
            if (right is TCheck)
                return left.Union((TCheck)right);
            else if (left is TCheck)
                return right.Union((TCheck)left);
            else
                throw new ArgumentException(string.Format("left or right must be an instance of TCheck ({0})", typeof(TCheck).Name), "left");
        }

        /// <summary>
        /// Obtains the complement of the <paramref name="operand"/> provided.
        /// </summary>
        /// <param name="operand">The <see cref="FiniteAutomataBitSet{TCheck}"/>
        /// instance to obtain the complement of.</param>
        /// <returns>A new <typeparamref name="TCheck"/> which represents the
        /// complement of the <paramref name="operand"/>.</returns>
        /// <exception cref="System.ArgumentNullException">thrown when the
        /// <paramref name="operand"/> is null.</exception>
        public static TCheck operator ~(FiniteAutomataBitSet<TCheck> operand)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");
            return operand.Complement();
        }

        /// <summary>
        /// Performs a bitwise And operation on the <paramref name="left"/> and 
        /// <paramref name="right"/> sets returning a new <typeparamref name="TCheck"/>
        /// instance of the elements from the <paramref name="left"/> set that
        /// are also in the <paramref name="right"/> set.
        /// </summary>
        /// <param name="left">The left <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <param name="right">The right <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance which
        /// contains the elements from the <paramref name="left"/>
        /// set which are also contained within the <paramref name="right"/>
        /// set.</returns>
        public static TCheck operator &(FiniteAutomataBitSet<TCheck> left, FiniteAutomataBitSet<TCheck> right)
        {
            if (Object.ReferenceEquals(left, null) ||
                Object.ReferenceEquals(right, null))
                return null;
            if (right is TCheck)
                return left.Intersect((TCheck)right);
            else if (left is TCheck)
                return right.Intersect((TCheck)left);
            else
                throw new ArgumentException(string.Format("left or right must be an instance of TCheck ({0})", typeof(TCheck).Name), "left");
        }

        /// <summary>
        /// Performs a bitwise exclusive or operation on the 
        /// <paramref name="left"/> and <paramref name="right"/> sets.
        /// </summary>
        /// <param name="left">The left <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <param name="right">The right <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance
        /// with the elements from the <paramref name="left"/> set
        /// which aren't present in the <paramref name="right"/> set as well as the
        /// elements from the <paramref name="right"/> set which aren't present 
        /// in the <paramref name="left"/> set.</returns>
        public static TCheck operator ^(FiniteAutomataBitSet<TCheck> left, FiniteAutomataBitSet<TCheck> right)
        {
            if (Object.ReferenceEquals(left, null))
                if (right is TCheck)
                    return (TCheck)right;
                else
                    throw new ArgumentException(string.Format("left or right must be an instance of TCheck ({0})", typeof(TCheck).Name), "left");
            else if (Object.ReferenceEquals(right, null))
                if (left is TCheck)
                    return (TCheck)left;
                else
                    throw new ArgumentException(string.Format("left or right must be an instance of TCheck ({0})", typeof(TCheck).Name), "left");
            if (right is TCheck)
                return left.SymmetricDifference((TCheck)right);
            else if (left is TCheck)
                return right.SymmetricDifference((TCheck)left);
            else
                throw new ArgumentException(string.Format("left or right must be an instance of TCheck ({0})", typeof(TCheck).Name), "left");
        }

        /// <summary>
        /// Performs a bitwise exclusive or operation on the 
        /// <paramref name="left"/> and <paramref name="right"/> sets.
        /// </summary>
        /// <param name="left">The left <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <param name="right">The right <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance
        /// with the elements from the <paramref name="left"/> set
        /// which aren't present in the <paramref name="right"/> set as well as the
        /// elements from the <paramref name="right"/> set which aren't present 
        /// in the <paramref name="left"/> set.</returns>
        public static TCheck BitwiseExclusiveOr(TCheck left, TCheck right)
        {
            if (object.ReferenceEquals(left, null))
                return right;
            else if (object.ReferenceEquals(right, null))
                return left;
            return left.SymmetricDifference(right);
        }

        /// <summary>
        /// Performs a bitwise or operation on the <paramref name="left"/>
        /// and <paramref name="right"/> sets returning a new <typeparamref name="TCheck"/>
        /// instance of the elements from both the <paramref name="left"/> set
        /// and the <paramref name="right"/> set.
        /// </summary>
        /// <param name="left">The left <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <param name="right">The right <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance
        /// as a union of the <paramref name="left"/> and
        /// <paramref name="right"/> sets.</returns>
        public static TCheck BitwiseOr(TCheck left, TCheck right)
        {
            if (object.ReferenceEquals(left, null))
                return right;
            else if (object.ReferenceEquals(right, null))
                return left;
            return left.Union(right);
        }

        /// <summary>
        /// Performs a bitwise And operation on the <paramref name="left"/> and 
        /// <paramref name="right"/> sets returning a new <typeparamref name="TCheck"/>
        /// instance of the elements from the <paramref name="left"/> set that
        /// are also in the <paramref name="right"/> set.
        /// </summary>
        /// <param name="left">The left <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <param name="right">The right <typeparamref name="TCheck"/> 
        /// set of the operation.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance which
        /// contains the elements from the <paramref name="left"/>
        /// set which are also contained within the <paramref name="right"/>
        /// set.</returns>
        public static TCheck BitwiseAnd(TCheck left, TCheck right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return null;
            return left.Intersect(right);
        }

        /// <summary>
        /// Subtracts the elements in the <paramref name="right"/> set
        /// from the <paramref name="left"/> set.
        /// </summary>
        /// <param name="left">The <typeparamref name="TCheck"/> instance which
        /// contains the primary set of elements.</param>
        /// <param name="right">The <typeparamref name="TCheck"/> instance which
        /// contains the secondary set of elements to remove from the <paramref name="left"/>
        /// set.</param>
        /// <returns>A new <typeparamref name="TCheck"/> instance with the
        /// elements from the <paramref name="left"/> set that are not an
        /// element of the <paramref name="right"/> set.</returns>
        public static TCheck SetSubtraction(TCheck left, TCheck right)
        {
            if (object.ReferenceEquals(left, null))
                return null;
            else if (object.ReferenceEquals(right, null))
                return left;
            return left.RelativeComplement(right);
        }

        public static int CountComputations()
        {
            return CountComputationCaches() +
                   unionCacheHits + 
                   symmetricDifferenceCacheHits +
                   intersectionCacheHits;
        }

        public static int CountComputationCaches()
        {
            return Sum(UnionCache, SymmetricDifferenceCache, IntersectCache);
        }

        private static int Sum(params IDictionary<TCheck, Dictionary<TCheck, TCheck>>[] set)
        {
            //An example of how to abuse C#'s new features.
            int result = 0;
            foreach (var dict in set)
                result += dict.Values.Sum(p => p.Count);
            return result;
        }

        internal uint CountTrue()
        {
            uint r = 0;
            for (uint i = this.Offset; i < this.Offset + this.Length; i++)
                if (this[i])
                    r++;
            return r;
        }

        internal uint CountFalse()
        {
            uint r = 0;
            for (uint i = this.Offset; i < this.Offset + this.Length; i++)
                if (!this[i])
                    r++;
            return r;
        }

        internal uint TrueCount
        {
            get
            {
                if (this.IsNegativeSet)
                    return this.FullLength - CountFalse();
                else
                    return this.CountTrue();
            }
        }

        internal RangeData GetRange()
        {
            return new RangeData((TCheck)this);
        }

    }
}
