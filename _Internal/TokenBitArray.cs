using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
using System.Linq;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    /// <summary>
    /// Provides a token bit array which relates a series of token instances through
    /// a simplistic bit array.
    /// </summary>
    internal class TokenBitArray  :
        IEquatable<TokenBitArray>
    {
        /// <summary>
        /// The full set of token items used to represent the set.
        /// </summary>
        internal ITokenItem[] fullSetRef = null;
        /// <summary>
        /// The <see cref="SetCommon.MinimalSetData"/>
        /// used to represent the set in minimal form.
        /// </summary>
        internal SetCommon.MinimalSetData setData;

        /* *
         * Constructor control, ensures that it can't be created without a
         * series of tokens to reference.
         * */
        private TokenBitArray() { }

        /// <summary>
        /// Creates a new <see cref="TokenBitArray"/> with the <paramref name="source"/>
        /// to copy.
        /// </summary>
        /// <param name="source">The <see cref="TokenBitArray"/> to copy the values of.</param>
        public TokenBitArray(TokenBitArray source)
        {
            this.fullSetRef = source.fullSetRef;
            this.setData = source.setData;
        }

        public override int GetHashCode()
        {
            return this.setData.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TokenBitArray))
                return false;
            return ((TokenBitArray)(obj)) == this;
        }

        /// <summary>
        /// Obtains the index of a token item within the full series.
        /// </summary>
        /// <param name="series">The <see cref="ITokenItem"/> series relative to the
        /// subset represented by the token.</param>
        /// <param name="element">The <see cref="ITokenItem"/> represented by the current
        /// element.</param>
        /// <returns>A <see cref="System.UInt32"/> value of the index of the <paramref name="element"/>
        /// in the <paramref name="series"/> provided.</returns>
        private static uint GetIndexOfElement(ITokenItem[] series, ITokenItem element)
        {
            for (uint i = 0; i < series.Length; i++)
                if (series[i] == element)
                    return i;
            return 0xFFFFFFFF;
        }

        public TokenBitArray(ITokenItem[] activeElements, ITokenItem[] fullElements)
        {
            //Initialize the token bit array to the minimal set of elements necessary
            //to represent the subset information.
            this.fullSetRef = fullElements;
            uint fullSetCount = (uint)fullElements.Length;
            uint[] set = new uint[activeElements.Length];
            for (int i = 0; i < set.Length; i++)
                set[i] = GetIndexOfElement(fullElements, activeElements[i]);
            uint currentMinimum = set[0];
            for (int i = 1; i < set.Length; i++)
                currentMinimum = Math.Min(currentMinimum, set[i]);
            uint offset = currentMinimum;
            BitArray originalSet = new BitArray((int)(fullSetCount - currentMinimum));
            for (int i = 0; i < set.Length; i++)
                originalSet[(int)(set[i] - offset)] = true;
            this.setData = new SetCommon.MinimalSetData(offset, fullSetCount, originalSet, false);
            this.setData.Reduce();
        }

        public TokenBitArray(ITokenItem activeElement, ITokenItem[] fullElements)
            : this(new ITokenItem[1] { activeElement }, fullElements)
        {
        }

        public static TokenBitArray operator ^(TokenBitArray left, TokenBitArray right)
        {
            if (left == null)
                return right;
            else if (right == null)
                return left;
            TokenBitArray result = new TokenBitArray();
            result.fullSetRef = left.fullSetRef;
            result.setData = SetCommon.BitwiseExclusiveOr(left.setData, right.setData);
            return result;
        }

        public static TokenBitArray operator |(TokenBitArray left, TokenBitArray right)
        {
            if (left == null)
                return right;
            else if (right == null)
                return left;
            TokenBitArray result = new TokenBitArray();
            result.fullSetRef = left.fullSetRef;
            result.setData = SetCommon.BitwiseOr(left.setData, right.setData);
            return result;
        }

        public static TokenBitArray operator &(TokenBitArray left, TokenBitArray right)
        {
            if (left == null)
                return left;
            else if (right == null)
                return right;
            TokenBitArray result = new TokenBitArray();
            result.fullSetRef = left.fullSetRef;
            result.setData = SetCommon.BitwiseAnd(left.setData, right.setData);
            return result;
        }

        public static bool operator ==(TokenBitArray left, TokenBitArray right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return true;
            else if (object.ReferenceEquals(left, null))
                return false;
            else if (object.ReferenceEquals(right, null))
                return false;
            return SetCommon.SetsEqual(left.setData, right.setData);
        }

        public static bool operator !=(TokenBitArray left, TokenBitArray right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return false;
            else if (object.ReferenceEquals(left, null))
                return true;
            else if (object.ReferenceEquals(right, null))
                return true;
            return SetCommon.SetsNotEqual(left.setData, right.setData);
        }

        private void Reduce()
        {
            this.setData.Reduce();
            //this.setData = SetCommon.Reduce(setData);
        }

        public bool AllFalse
        {
            get
            {
                return setData.AllFalse;
            }
        }

        public bool AllTrue
        {
            get
            {
                return setData.AllTrue;
            }
        }

        public uint CountTrue()
        {
            return this.setData.CountTrue();
        }

        public ITokenItem[] GetSeries()
        {
            if (this.setData.WorkingSet == null)
                return new ITokenItem[0];
            int resultOffset = 0;
            ITokenItem[] result = new ITokenItem[CountTrue()];
            for (uint i = this.setData.Offset; i < this.setData.Offset + this.setData.Length; i++)
                if (this.setData[i])
                    result[resultOffset++] = fullSetRef[i];
            return result;
        }

        public override string ToString()
        {
            if (this.setData.WorkingSet == null)
                return "{NONE}";
            ITokenItem[] series = this.GetSeries();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < series.Length; i++)
            {
                if (i > 0)
                    sb.Append(" | ");
                sb.Append(series[i].Name);
            }
            return sb.ToString();
        }

        #region IEquatable<TokenBitArray> Members

        public bool Equals(TokenBitArray other)
        {
            return other == this;
        }

        #endregion
    }
}
