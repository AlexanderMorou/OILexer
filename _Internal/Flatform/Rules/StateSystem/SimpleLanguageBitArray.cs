using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
//using Oilexer._Internal.UI.Visualization;
using System.Diagnostics;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules.StateSystem
{
    /// <summary>
    /// Provides a structured class for handling the various language elements in a
    /// bit-array form.
    /// </summary>
    /// <remarks>Enables the use of intersections, unions, complements, but requires
    /// a fair amount of data to enable proper functionality.</remarks>
    internal class SimpleLanguageBitArray :
        IEquatable<SimpleLanguageBitArray>//,
        //IVisualBitSet
    {
        /// <summary>
        /// Data member which holds the subset informaiton for the
        /// <see cref="SimpleLanguageBitArray"/>.
        /// </summary>
        private Dictionary<ITokenEntry, TokenBitArray> tokenSubsetInfo = new Dictionary<ITokenEntry, TokenBitArray>();
        /// <summary>
        /// Stores the true/false information about which rules are referred to by
        /// the <see cref="SimpleLanguageBitArray"/>.
        /// </summary>
        private SetCommon.MinimalSetData ruleData;
        /// <summary>
        /// Stores the true/false information about which tokens are referred to by the
        /// <see cref="SimpleLanguageBitArray"/>.
        /// </summary>
        private SetCommon.MinimalSetData tokenData;
        /// <summary>
        /// Final set data for the tokens; utilized to simplify access to 
        /// token sub-set information
        /// </summary>
        internal TokenFinalDataSet finalDataSet;
        /// <summary>
        /// Stores the full series of production rule entries associated to the
        /// bit array for the language.
        /// </summary>
        private IProductionRuleEntry[] fullRuleRef = null;
        /// <summary>
        /// Stores the full series of token entries associated to the bit array for the
        /// language.
        /// </summary>
        private ITokenEntry[] fullTokenRef = null;

        #region Token Data set Merging Hack
        /* *
         * Because you can't refer to op_* directly according to the C# compiler (CS0571).
         * Refer to http://msdn.microsoft.com/en-us/library/z47a7kdw.aspx for more information.
         * */
        private static Func<TokenBitArray, TokenBitArray, TokenBitArray> TokenMergeOr = new Func<TokenBitArray, TokenBitArray, TokenBitArray>(MergeTokenArraysBitwiseOr);
        private static Func<TokenBitArray, TokenBitArray, TokenBitArray> TokenMergeAnd = new Func<TokenBitArray, TokenBitArray, TokenBitArray>(MergeTokenArraysBitwiseAnd);
        private static Func<TokenBitArray, TokenBitArray, TokenBitArray> TokenMergeExclusiveOr = new Func<TokenBitArray, TokenBitArray, TokenBitArray>(MergeTokenArraysBitwiseExclusiveOr);

        private static TokenBitArray MergeTokenArraysBitwiseOr(TokenBitArray left, TokenBitArray right)
        {
            return left | right;
        }

        private static TokenBitArray MergeTokenArraysBitwiseAnd(TokenBitArray left, TokenBitArray right)
        {
            return left & right;
        }

        private static TokenBitArray MergeTokenArraysBitwiseExclusiveOr(TokenBitArray left, TokenBitArray right)
        {
            return left ^ right;
        }
        #endregion


        #region Finding a TokenItem's Owner TokenEntry
        /// <summary>
        /// Determines the owner of the given <paramref name="source"/> 
        /// <see cref="ITokenItem"/> given the full <paramref name="set"/> 
        /// of tokens provided.
        /// </summary>
        /// <param name="source">The <see cref="ITokenItem"/> to find the owner of.</param>
        /// <param name="set">The <paramref name="ITokenEntry"/> series to 
        /// scan through.</param>
        /// <returns>A <see cref="ITokenEntry"/> which owns the <paramref name="source"/>
        /// if applicable; null, otherwise.</returns>
        private static ITokenEntry FindOwner(ITokenItem source, ITokenEntry[] set)
        {
            foreach (var token in set)
                if (IsOwner(source, token.Branches))
                    return token;
            return null;
        }

        private static bool IsOwner(ITokenItem source, ITokenExpressionSeries target)
        {
            foreach (var expression in target)
                if (IsOwner(source, expression))
                    return true;
            return false;
        }

        private static bool IsOwner(ITokenItem source, ITokenExpression target)
        {
            foreach (var item in target)
            {
                if (item == source)
                    return true;
                if (item is ITokenGroupItem)
                    if (IsOwner(source, (ITokenExpressionSeries)(item)))
                        return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Returns the index of the <paramref name="element"/> in the
        /// <paramref name="sourceData"/> provided.
        /// </summary>
        /// <typeparam name="T">The type of elements to scan through.</typeparam>
        /// <param name="element">The element to find the index of in 
        /// <paramref name="sourceData"/>.</param>
        /// <param name="sourceData">The array of <typeparamref name="T"/>
        /// elements which contains the <paramref name="element"/> provided.</param>
        /// <returns>A <see cref="Int32"/> value representing the ordinal index the
        /// of <paramref name="element"/> provided, if found; -1, otherwise.</returns>
        private static int GetIndexOf<T>(T element, T[] sourceData)
            where T :
                class
        {
            for (int i = 0; i < sourceData.Length; i++)
                if (sourceData[i] == element)
                    return i;
            return -1;
        }

        /// <summary>
        /// Creates a new <see cref="SimpleLanguageBitArray"/> with the
        /// <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="SimpleLanguageBitArray"/> to copy.</param>
        [DebuggerStepThrough]
        public SimpleLanguageBitArray(SimpleLanguageBitArray source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            this.finalDataSet = source.finalDataSet;
            this.fullRuleRef = source.fullRuleRef;
            this.fullTokenRef = source.fullTokenRef;
            this.ruleData = source.ruleData;
            this.tokenData = source.tokenData;
            //Copy the token subset data.
            foreach (var tk in source.tokenSubsetInfo)
                this.tokenSubsetInfo.Add(tk.Key, new TokenBitArray(tk.Value));
        }

        /// <summary>
        /// Creates a new <see cref="SimpleLanguageBitArray"/> with the 
        /// <paramref name="ruleSet"/>, <paramref name="tokenSet"/>, <paramref name="target"/>,
        /// and <paramref name="finalDataSet"/> provided.
        /// </summary>
        /// <param name="ruleSet">The series of <see cref="IProductionRuleEntry"/> instances
        /// to relate to for retrieving sub-set information.</param>
        /// <param name="tokenSet">The series of <see cref="ITokenEntry"/> instances 
        /// to relate to for retrieving sub-set information.</param>
        /// <param name="target">The single <see cref="IProductionRuleEntry"/> to refer to
        /// initially.</param>
        /// <param name="finalDataSet">The <see cref="TokenFinalDataSet"/> which contains
        /// full sub-set information about the tokens.</param>
        [DebuggerStepThrough]
        public SimpleLanguageBitArray(IProductionRuleEntry[] ruleSet, ITokenEntry[] tokenSet,
                                   IProductionRuleEntry target, TokenFinalDataSet finalDataSet)
            : this(ruleSet, tokenSet, finalDataSet)
        {
            int index = GetIndexOf(target, ruleSet);
            BitArray ruleData = new BitArray(1);
            ruleData[0] = true;
            this.ruleData = new SetCommon.MinimalSetData((uint)index, (uint)ruleSet.Length, ruleData, false);
        }

        /// <summary>
        /// Creates a new <see cref="SimpleLanguageBitArray"/> with the 
        /// <paramref name="ruleSet"/>, <paramref name="tokenSet"/>, <paramref name="target"/>
        /// and <paramref name="finalDataSet"/> provided.
        /// </summary>
        /// <param name="ruleSet">The series of <see cref="IProductionRuleEntry"/> instances
        /// to relate to for retrieving sub-set information.</param>
        /// <param name="tokenSet">The series of <see cref="ITokenEntry"/> instances 
        /// to relate to for retrieving sub-set information.</param>
        /// <param name="target">The single <see cref="ITokenEntry"/> to refer 
        /// to initially.</param>
        /// <param name="finalDataSet">The <see cref="TokenFinalDataSet"/> which contains
        /// full sub-set information about the tokens.</param>
        [DebuggerStepThrough]
        public SimpleLanguageBitArray(IProductionRuleEntry[] ruleSet, ITokenEntry[] tokenSet,
                                   ITokenEntry target, TokenFinalDataSet finalDataSet)
            : this(ruleSet, tokenSet, finalDataSet)
        {
            int index = GetIndexOf(target, tokenSet);
            BitArray tokenData = new BitArray(1);
            tokenData[0] = true;
            this.tokenData = new SetCommon.MinimalSetData((uint)index, (uint)tokenSet.Length, tokenData, false);
            /* *
             * A full reference to a token element includes the full subset data of an
             * enumerator type token.
             * */
            if (finalDataSet[target] is TokenEnumFinalData)
            {
                var elements = finalDataSet[target].Elements.ToArray();
                this.tokenSubsetInfo.Add(target, new TokenBitArray(elements, elements));
            }
        }

        [DebuggerStepThrough]
        public SimpleLanguageBitArray(IProductionRuleEntry[] ruleSet, ITokenEntry[] tokenSet,
                                   ITokenItem target, TokenFinalDataSet finalDataSet)
            : this(ruleSet, tokenSet, finalDataSet)
        {
            var owner = FindOwner(target, tokenSet);
            if (owner == null)
                return;
            int index = GetIndexOf(owner, tokenSet);
            BitArray tokenData = new BitArray(1);
            tokenData[0] = true;
            this.tokenData = new SetCommon.MinimalSetData((uint)index, (uint)tokenSet.Length, tokenData, false);
            tokenSubsetInfo.Add(owner, new TokenBitArray(target, finalDataSet[owner].Elements.ToArray()));
        }

        /* *
         * Used for binary operators, and other constructors.
         * */
        [DebuggerStepThrough]
        private SimpleLanguageBitArray(IProductionRuleEntry[] ruleSet, ITokenEntry[] tokenSet, TokenFinalDataSet finalDataSet)
        {
            this.tokenData = new SetCommon.MinimalSetData(0, (uint)tokenSet.Length, null, false);
            this.ruleData = new SetCommon.MinimalSetData(0, (uint)ruleSet.Length, null, false);
            this.fullTokenRef = tokenSet;
            this.fullRuleRef = ruleSet;
            this.finalDataSet = finalDataSet;
        }

        [DebuggerStepThrough]
        private static void CombineTokenData(SimpleLanguageBitArray source, SimpleLanguageBitArray target, Func<TokenBitArray, TokenBitArray, TokenBitArray> operation, bool cleanTail)
        {
            int observedSetLength = target.tokenSubsetInfo.Count;
            int nTrue = 0;
            int setSize = (int)Math.Ceiling(((double)(observedSetLength)) / SetCommon.MinimalSetData.setSize);
            uint[] observed2 = cleanTail ? new uint[setSize] : null;
            //BitArray observed = cleanTail ? new BitArray(target.tokenSubsetInfo.Count) : null;
            foreach (var tokenSet in source.tokenSubsetInfo)
            {
                if (target.tokenSubsetInfo.ContainsKey(tokenSet.Key))
                {
                    if (cleanTail)
                        for (int i = 0; i < observedSetLength; i++)
                            if (target.tokenSubsetInfo.Keys.ElementAt(i) ==
                                tokenSet.Key)
                            {
                                nTrue++;
                                observed2[i / (int)SetCommon.MinimalSetData.setSize] |= (uint)1 << i % (int)SetCommon.MinimalSetData.setSize;
                                //observed[i] = true;
                                break;
                            }
                    target.tokenSubsetInfo[tokenSet.Key] = operation(target.tokenSubsetInfo[tokenSet.Key], tokenSet.Value);
                    if (target.tokenSubsetInfo[tokenSet.Key].AllFalse)
                        target.tokenSubsetInfo.Remove(tokenSet.Key);
                }
                else
                {
                    var current = operation(tokenSet.Value, null);
                    if (current != null && !current.AllFalse)
                    {
                        target.tokenSubsetInfo.Add(tokenSet.Key, current);
                        if (cleanTail)
                        {
                            int nSize = (int)Math.Ceiling(((double)(observedSetLength)) / (int)SetCommon.MinimalSetData.setSize);
                            if (nSize > setSize)
                            {
                                uint[] newObserved2 = new uint[nSize];
                                for (int i = 0; i < setSize; i++)
                                    newObserved2[i] = observed2[i];
                                newObserved2[setSize] = 1;
                                observed2 = newObserved2;
                                setSize = nSize;
                            }
                            else
                                observed2[observedSetLength / (int)SetCommon.MinimalSetData.setSize] |= (uint)1 << observedSetLength % (int)SetCommon.MinimalSetData.setSize;
                            nTrue++;
                            observedSetLength++;
                            //BitArray newObserved = new BitArray(observed.Length + 1);
                            //for (int i = 0; i < observed.Length; i++)
                            //    newObserved[i] = observed[i];
                            //newObserved[observed.Length] = true;
                            //observed = newObserved;
                        }
                    }
                }
            }
            if (!cleanTail)
                return;
            int observedIndex = 0;
            int disposedIndex = 0;
            ITokenEntry[] disposed = new ITokenEntry[observedSetLength - nTrue];
            foreach (var tokenEntry in target.tokenSubsetInfo.Keys)
            {
                if ((observed2[observedIndex / (int)SetCommon.MinimalSetData.setSize] & 1 << observedIndex % (int)SetCommon.MinimalSetData.setSize) == 0)
                {
                    var currentMerge = operation(target.tokenSubsetInfo[tokenEntry], null);
                    if (currentMerge == null)
                        disposed[disposedIndex++] = tokenEntry;
                }
                observedIndex++;
            }
            for (int i = 0; i < disposed.Length; i++)
                if (disposed[i] == null)
                    break;
                else
                    target.tokenSubsetInfo.Remove(disposed[i]);
        }

        [DebuggerStepThrough]
        public static SimpleLanguageBitArray operator |(SimpleLanguageBitArray left, SimpleLanguageBitArray right)
        {
            if (left == null)
                return right;
            else if (right == null)
                return left;
            SimpleLanguageBitArray result = new SimpleLanguageBitArray(left.fullRuleRef, left.fullTokenRef, left.finalDataSet);
            result.tokenData = left.tokenData | right.tokenData;
            CombineTokenData(left, result, TokenMergeOr, false);
            CombineTokenData(right, result, TokenMergeOr, false);
            result.ruleData = left.ruleData | right.ruleData;
            return result;
        }

        /// <summary>
        /// Performs a bitwise and operation on the members represented by the
        /// <see cref="SimpleLanguageBitArray"/>.
        /// </summary>
        /// <param name="left">The <see cref="SimpleLanguageBitArray"/> on the left side of the operation.</param>
        /// <param name="right">The <see cref="SimpleLanguageBitArray"/> on the right side of the operation.</param>
        /// <returns>A new <see cref="SimpleLanguageBitArray"/> instance with the intersecting members of the
        /// <paramref name="left"/> and <paramref name="right"/> <see cref="SimpleLanguageBitArray"/>.</returns>
        [DebuggerStepThrough]
        public static SimpleLanguageBitArray operator &(SimpleLanguageBitArray left, SimpleLanguageBitArray right)
        {
            if (left == null)
                return left;
            else if (right == null)
                return right;
            SimpleLanguageBitArray result = new SimpleLanguageBitArray(left.fullRuleRef, left.fullTokenRef, left.finalDataSet);
            var tokenTempData = left.tokenData | right.tokenData;
            result.tokenData = left.tokenData & right.tokenData;
            /* *
             * The result contains no data to start, so
             * unify the elements, then '&' the dataset that
             * results.
             * */
            CombineTokenData(left, result, TokenMergeOr, false);
            CombineTokenData(right, result, TokenMergeAnd, true);
            CorrectTokenSetData(result, tokenTempData);
            result.ruleData = left.ruleData & right.ruleData;
            result.ruleData.Reduce();
            result.tokenData.Reduce();
            foreach (var item in result.tokenSubsetInfo.Values)
                item.setData.Reduce();
            if (result.IsNullSet)
                return null;
            return result;
        }

        /* *
         * Serves to correct the token set data since a simple bitwise exclusion on the 
         * members isn't sufficient enough to represent the subset relationships involved.
         * *
         * This is due to the 'enumeration' type tokens which serve to classify (group)
         * certain tokens based upon the author's language description.
         * *
         * Example:
         * A ::= 'B':B; | 'C':C; <-- 'A' would contain two subset values, B, and C.
         * */
        [DebuggerStepThrough]
        private static void CorrectTokenSetData(SimpleLanguageBitArray result, SetCommon.MinimalSetData tokenTempData)
        {
            BitArray resultSet = new BitArray((int)tokenTempData.Length);
            for (uint i = tokenTempData.Offset; i < tokenTempData.Offset + tokenTempData.Length; i++)
                if (tokenTempData[i])
                    if (result.finalDataSet[result.fullTokenRef[i]] is TokenEnumFinalData)
                        if (!result.tokenSubsetInfo.ContainsKey(result.fullTokenRef[i]))
                            resultSet[((int)(i - tokenTempData.Offset))] = false;
                        else
                            resultSet[((int)(i - tokenTempData.Offset))] = true;
                    else if (i >= result.tokenData.Offset && i < result.tokenData.Offset + result.tokenData.Length)
                        resultSet[(int)(i - tokenTempData.Offset)] = result.tokenData[i];
            var resultSet2 = new SetCommon.MinimalSetData(tokenTempData.Offset, tokenTempData.FullSetLength, resultSet, false);
            resultSet2.Reduce();
            result.tokenData = resultSet2;
        }

        /// <summary>
        /// Performs a bitwise exclusive or operation on the members represented by the
        /// <see cref="SimpleLanguageBitArray"/>.
        /// </summary>
        /// <param name="left">The <see cref="SimpleLanguageBitArray"/> on the left side of the operation.</param>
        /// <param name="right">The <see cref="SimpleLanguageBitArray"/> on the right side of the operation.</param>
        /// <returns>A new <see cref="SimpleLanguageBitArray"/> instance with the members exclusive to the
        /// <paramref name="left"/> and <paramref name="right"/> <see cref="SimpleLanguageBitArray"/>.</returns>
        [DebuggerStepThrough]
        public static SimpleLanguageBitArray operator ^(SimpleLanguageBitArray left, SimpleLanguageBitArray right)
        {
            if (left == null)
                return right;
            else if (right == null)
                return left;
            SimpleLanguageBitArray result = new SimpleLanguageBitArray(left.fullRuleRef, left.fullTokenRef, left.finalDataSet);
            var tokenTempData = left.tokenData | right.tokenData;
            result.tokenData = left.tokenData ^ right.tokenData;
            /* *
             * The result contains no data to start, so
             * unify the elements, then '^' the dataset that
             * results.
             * */
            CombineTokenData(left, result, TokenMergeOr, false);
            /* *
             * Instructs it to cleanup the subset data since there will be occasions
             * where the subset data is zeroed out, but the relationship in the lookup
             * will exist as an empty set.
             * */
            CombineTokenData(right, result, TokenMergeExclusiveOr, true);
            /* *
             * The previous method only works on the subset lookup, the individual
             * token data bit array, once exclusive-or'ed with the right side subset 
             * data will loose synch (show as 'not there' when a relationship exists)
             * if it is not corrected.
             * */
            CorrectTokenSetData(result, tokenTempData);
            result.ruleData = left.ruleData ^ right.ruleData;
            //Cleanup. 
            /* *
             * Time is required to do this work, but it's necessary to ensure future
             * calculations on sets are minimal, and report correctly.
             * */
            result.ruleData.Reduce();
            result.tokenData.Reduce();
            foreach (var item in result.tokenSubsetInfo.Values)
                item.setData.Reduce();
            return result;
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> operand is equal to the
        /// <paramref name="right"/> operand.
        /// </summary>
        /// <param name="left">The <see cref="SimpleLanguageBitArray"/> on the left side of the operation.</param>
        /// <param name="right">The <see cref="SimpleLanguageBitArray"/> on the right side of the 
        /// operation.</param>
        /// <returns>true if <paramref name="left"/> <see cref="SimpleLanguageBitArray"/> is
        /// equal to the <paramref name="right"/> <see cref="SimpleLanguageBitArray"/></returns>
        [DebuggerStepThrough]
        public static bool operator ==(SimpleLanguageBitArray left, SimpleLanguageBitArray right)
        {
            if (object.ReferenceEquals(left, null))
                return object.ReferenceEquals(right, null);
            if (object.ReferenceEquals(right, null))
                return false;
            if (SetCommon.SetsNotEqual(left.tokenData, right.tokenData))
                return false;
            if (SetCommon.SetsNotEqual(left.ruleData, right.ruleData))
                return false;
            if (left.tokenSubsetInfo.Count != right.tokenSubsetInfo.Count)
                return false;
            foreach (var tokenEntry in left.tokenSubsetInfo.Keys)
            {
                if (!right.tokenSubsetInfo.ContainsKey(tokenEntry))
                    return false;
                if (right.tokenSubsetInfo[tokenEntry] != left.tokenSubsetInfo[tokenEntry])
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Determines whether the <paramref name="left"/> operand is not equal
        /// to the <paramref name="right"/> operand.
        /// </summary>
        /// <param name="left">The <see cref="SimpleLanguageBitArray"/> on the left side
        /// of the operation.</param>
        /// <param name="right">The <see cref="SimpleLanguageBitArray"/> on the right
        /// side of the operation.</param>
        /// <returns>true if <paramref name="left"/> does not equal <paramref name="right"/>
        /// in value.</returns>
        public static bool operator !=(SimpleLanguageBitArray left, SimpleLanguageBitArray right)
        {
            if (object.ReferenceEquals(left, null))
                return !object.ReferenceEquals(right, null);
            if (object.ReferenceEquals(right, null))
                return true;
            if (SetCommon.SetsNotEqual(left.tokenData, right.tokenData))
                return true;
            if (SetCommon.SetsNotEqual(left.ruleData, right.ruleData))
                return true;
            if (left.tokenSubsetInfo.Count != right.tokenSubsetInfo.Count)
                return true;
            foreach (var tokenEntry in left.tokenSubsetInfo.Keys)
            {
                if (!right.tokenSubsetInfo.ContainsKey(tokenEntry))
                    return true;
                if (right.tokenSubsetInfo[tokenEntry] != left.tokenSubsetInfo[tokenEntry])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="SimpleLanguageBitArray"/>.
        /// </summary>
        /// <returns>A <see cref="Int32"/> value representing the hash of the
        /// <see cref="SimpleLanguageBitArray"/> in its current state.</returns>
        public override int GetHashCode()
        {
            //return base.GetHashCode();
            int result = 0;
            result |= this.tokenData.GetHashCode();
            result ^= this.ruleData.GetHashCode();
            foreach (var item in this.tokenSubsetInfo)
            {
                result ^= item.Key.Name.GetHashCode();
                result ^= item.Value.GetHashCode();
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is SimpleLanguageBitArray)
                return ((SimpleLanguageBitArray)(obj)) == this;
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var ruleSubset = this.ruleData.GetSubSet(this.fullRuleRef);
            var tokenSubset = this.tokenData.GetSubSet(this.fullTokenRef);

            bool first = true;
            foreach (var rule in ruleSubset)
            {
                if (first)
                    first = false;
                else
                    sb.Append(" | ");
                sb.Append(rule.Name);
            }
            first = true;
            foreach (var token in tokenSubset)
            {
                if (first)
                {
                    first = false;
                    if (sb.Length != 0)
                        sb.Append(" | ");
                }
                else
                    sb.Append(" | ");
                sb.Append(token.Name);
                if (this.tokenSubsetInfo.ContainsKey(token))
                {
                    sb.Append(" (");
                    sb.Append(this.tokenSubsetInfo[token].ToString());
                    sb.Append(")");
                }
            }
            return sb.ToString();
        }

        [DebuggerStepThrough]
        public IProductionRuleEntry[] GetRuleRange()
        {
            return this.ruleData.GetSubSet(this.fullRuleRef);
        }

        [DebuggerStepThrough]
        public ITokenEntry[] GetTokenRange()
        {
            return this.tokenData.GetSubSet(this.fullTokenRef);
        }

        /* *
         * For cases where token, or rule data, is just in the way.
         * */
        [DebuggerStepThrough]
        public void DisposeTokenData()
        {
            this.tokenData = new SetCommon.MinimalSetData(0, 0, null);
        }

        [DebuggerStepThrough]
        public void DisposeRuleData()
        {
            this.ruleData = new SetCommon.MinimalSetData(0, 0, null);
        }

        #region IEquatable<SimpleLanguageBitArray> Members

        public bool Equals(SimpleLanguageBitArray other)
        {
            return this == other;
        }

        #endregion

        public bool IsNullSet
        {
            [DebuggerStepThrough]
            get
            {
                if (!this.tokenData.AllFalse)
                    return false;
                if (!this.ruleData.AllFalse)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Returns the <see cref="TokenBitArray"/> relative to the
        /// <paramref name="entry"/> provided.
        /// </summary>
        /// <param name="entry">The <see cref="ITokenEntry"/> to retrieve
        /// sub-set information on.</param>
        /// <returns>A <see cref="TokenBitArray"/> relative to the sub-set
        /// information for the <paramref name="entry"/> provided.</returns>
        public TokenBitArray GetSubsetInformation(ITokenEntry entry)
        {
            int index = GetIndexOf(entry, this.fullTokenRef);
            if (index > -1 && this.tokenData[(uint)index])
                return new TokenBitArray(this.tokenSubsetInfo[entry]);
            else
                return null;
        }

        /// <summary>
        /// Shatters the <see cref="SimpleLanguageBitArray"/> into an array of unique elements.
        /// </summary>
        /// <returns>An array of <see cref="SimpleLanguageBitArray"/> representing each distinct
        /// value represented by the current <see cref="SimpleLanguageBitArray"/>.</returns>
        [DebuggerStepThrough]
        public SimpleLanguageBitArray[] Shatter()
        {
            uint size = this.tokenData.CountTrue();
            /* *
             * Obtain the proper size: 
             * rules + tokens - subset token entry count + sum(subset token entries)
             * */
            size += this.ruleData.CountTrue();
            if (this.tokenData.Length == 0 &&
                this.tokenSubsetInfo.Count > 0)
                this.tokenSubsetInfo.Clear();
            size -= (uint)this.tokenSubsetInfo.Count;
            foreach (var key in tokenSubsetInfo.Keys)
                size += tokenSubsetInfo[key].CountTrue();
            SimpleLanguageBitArray[] result = new SimpleLanguageBitArray[size];
            uint index = 0;
            //Push rules.
            for (uint i = this.ruleData.Offset; i < this.ruleData.Offset + this.ruleData.Length; i++)
            {
                if (ruleData[i])
                {
                    SimpleLanguageBitArray current = new SimpleLanguageBitArray(this.fullRuleRef, this.fullTokenRef, this.finalDataSet);
                    BitArray c = new BitArray(1);
                    c[0] = true;
                    current.ruleData = new SetCommon.MinimalSetData(i, ruleData.FullSetLength, c);
                    result[index++] = current;
                }
            }
            //Push tokens.
            for (uint i = this.tokenData.Offset; i < this.tokenData.Offset + this.tokenData.Length; i++)
            {
                if (this.tokenData[i])
                {
                    //For now, exclude subsets.
                    if (this.tokenSubsetInfo.ContainsKey(this.fullTokenRef[i]))
                        continue;
                    SimpleLanguageBitArray current = new SimpleLanguageBitArray(this.fullRuleRef, this.fullTokenRef, this.finalDataSet);
                    BitArray c = new BitArray(1);
                    c[0] = true;
                    current.tokenData = new SetCommon.MinimalSetData(i, tokenData.FullSetLength, c);
                    result[index++] = current;
                }
            }
            //Push subset token elements, excluded above, included here.
            foreach (var token in this.tokenSubsetInfo.Keys)
            {
                uint currentIndex = (uint)GetIndexOf(token, fullTokenRef);
                var currentDataSet = this.tokenSubsetInfo[token];
                for (uint i = currentDataSet.setData.Offset; i < currentDataSet.setData.Offset + currentDataSet.setData.Length; i++)
                {
                    if (currentDataSet.setData[i])
                    {
                        BitArray c = new BitArray(1);
                        c[0] = true;
                        SimpleLanguageBitArray current = new SimpleLanguageBitArray(this.fullRuleRef, this.fullTokenRef, this.finalDataSet);
                        current.tokenData = new SetCommon.MinimalSetData(currentIndex, (uint)this.fullTokenRef.Length, c);
                        TokenBitArray tba = new TokenBitArray(currentDataSet.fullSetRef[(int)i], currentDataSet.fullSetRef);
                        current.tokenSubsetInfo.Add(fullTokenRef[(int)currentIndex], tba);
                        result[index++] = current;
                    }
                }
            }
            return result;
        }

        #region IVisualBitSet Members

        //bool IVisualBitSet.IsEmptySet
        //{
        //    get { return this.IsNullSet; }
        //}

        //bool IVisualBitSet.IsFullSet
        //{
        //    get {
        //        if (!this.ruleData.AllTrue)
        //            return false;
        //        if (!this.tokenData.AllTrue)
        //            return false;
        //        foreach (var t in this.tokenSubsetInfo.Keys)
        //            if (!this.tokenSubsetInfo[t].AllTrue)
        //                return false;
        //        return true;
        //    }
        //}

        #endregion
    }
}
