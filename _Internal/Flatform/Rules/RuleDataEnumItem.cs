using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Utilities.Collections;
using Oilexer.Utilities.Common;
using Oilexer.Utilities.Arrays;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class RuleDataEnumItem :
        RuleDataTokenItemBase,
        IRuleDataEnumItem
    {
        private IRuleDataEnumItem leftMerge,
                                  rightMerge;
        private IProductionRuleItem reference;
        protected TokenEnumFinalData finalData = null;
        public override RuleDataItemType ElementType
        {
            get { return RuleDataItemType.Enumeration; }
        }

        public RuleDataEnumItem(TokenEnumFinalData finalData, ILiteralReferenceProductionRuleItem reference)
            : this(finalData)
        {
            this.CoveredSet = new TokenBitArray(reference.Literal, finalData.FinalItemLookup.Keys.ToArray());
            this.reference = reference;
        }

        public RuleDataEnumItem(TokenEnumFinalData finalData, ITokenReferenceProductionRuleItem reference)
            : this(finalData)
        {
            var fullSet = finalData.FinalItemLookup.Keys.ToArray();
            this.CoveredSet = new TokenBitArray(fullSet, fullSet);
            this.reference = reference;
        }

        protected RuleDataEnumItem(TokenEnumFinalData finalData)
            : base(finalData.Entry, finalData)
        {
            this.finalData = finalData;
        }

        #region IRuleDataEnumItem Members

        /// <summary>
        /// Returns the <see cref="TokenBitArray"/> of elements
        /// covered by the current
        /// <see cref="RuleDataEnumItem"/>.
        /// </summary>
        public TokenBitArray CoveredSet { get; private set; }

        #endregion

        /// <summary>
        /// Returns whether the <paramref name="other"/> element
        /// is of proper nature relative to the current
        /// <see cref="RuleDataEnumItem"/>.
        /// </summary>
        /// <param name="other">The <see cref="IRuleDataItem"/> to
        /// compare against.</param>
        /// <returns>true if the <paramref name="other"/> is of equivalent
        /// nature; false, otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">thrown when <paramref name="other"/>
        /// is null.</exception>
        protected override bool AreEqualNaturedImpl(IRuleDataItem other)
        {
            if (other == null)
                throw new ArgumentNullException("other");
            var cOther = other as RuleDataEnumItem;
            if (cOther == null)
                return false;
            return cOther.Source == this.Source;
        }

        /// <summary>
        /// Obtains the best natured <see cref="IRuleDataItem"/> from the
        /// <paramref name="equalNaturedSet"/> provided.
        /// </summary>
        /// <param name="equalNaturedSet">An array of <see cref="IRuleDataItem"/> 
        /// elements which are equally natured to the current
        /// <see cref="RuleDataEnumItem"/>.</param>
        /// <returns>The best natured <see cref="IRuleDataItem"/> of the
        /// <paramref name="equalNaturedSet"/>.</returns>
        public override IRuleDataItem ObtainBestNatured(IRuleDataItem[] equalNaturedSet)
        {
            int maxSimilar = -1;
            int similarCount = 0;
            IRuleDataItem closest = null;
            int thisTrue = (int)this.CoveredSet.CountTrue();
            /* *
             * Sift through the elements and return the case of the largest
             * intersection.
             * */
            for (int i = 0; i < equalNaturedSet.Length; i++)
            {
                var currentElement = equalNaturedSet[i] as RuleDataEnumItem;
                if (currentElement == null)
                    continue;
                var currentSubSet = currentElement.CoveredSet;
                var currentTrue = (int)currentElement.CoveredSet.CountTrue();
                var intersection = this.CoveredSet & currentSubSet;
                int trueCount = (int)intersection.CountTrue();
                /* *
                 * If the intersection is greater than the last, 
                 * reassign as needed; however, if another case exists
                 * where the number of true elements is equal, also check
                 * the number of true elements on the current set
                 * to the number of true elements to the currently selected
                 * similar set.  The smallest set is likely the closest
                 * match.
                 * *
                 * The second case is especially true when you have a 
                 * full range element and a sub-set that is equal to the 
                 * current: the equal set is a better match than the
                 * larger set.
                 * */
                if ((trueCount > maxSimilar) ||
                    (trueCount == maxSimilar &&
                     similarCount != thisTrue &&
                     currentTrue < similarCount))
                {
                    maxSimilar = trueCount;
                    similarCount = currentTrue;
                    closest = currentElement;
                }
            }
            return closest;
        }

        public override IRuleDataItem MergeWith(IRuleDataItem other)
        {
            if (other == null)
                throw new ArgumentNullException("other");
            if (!this.AreEqualNatured(other))
                throw new ArgumentException("other");
            IRuleDataEnumItem cOther = ((IRuleDataEnumItem)(other));
            return MergeWithImpl(cOther);
        }

        /// <summary>
        /// Implementation version of the merge with method.
        /// </summary>
        /// <param name="other">The other rule data item to merge with.</param>
        /// <returns></returns>
        protected IRuleDataItem MergeWithImpl(IRuleDataEnumItem other)
        {
            var union = other.CoveredSet | this.CoveredSet;
            RuleDataEnumItem unionResult = MergeWithInstanceImpl();
            unionResult.Rank = Math.Max(this.Rank, other.Rank);
            unionResult.CoveredSet = union;
            unionResult.leftMerge = this;
            unionResult.rightMerge = other;
            return unionResult;
        }

        protected virtual RuleDataEnumItem MergeWithInstanceImpl()
        {
            return new RuleDataEnumItem(this.finalData);
        }

        public RuleDataNamedEnumItem GetNamedForm(string name)
        {
            RuleDataNamedEnumItem result = new RuleDataNamedEnumItem(name, this.finalData);
            result.Source = this.Source;
            result.leftMerge = this.leftMerge;
            result.rightMerge = this.rightMerge;
            result.CoveredSet = this.CoveredSet;
            return result;
        }

        protected string GetTypeName()
        {
            if (this.leftMerge != null)
                return ((RuleDataEnumItem)(this.leftMerge)).GetTypeName();
            else if (this.reference is ILiteralReferenceProductionRuleItem)
            {
                var lSource = (ILiteralReferenceProductionRuleItem)this.reference;
                if (lSource.Counter)
                    return typeof(int).Name;
                else if (lSource.IsFlag)
                    return typeof(bool).Name;
                else
                    return this.finalData.TokenInterface.Name;
            }
            else
                return this.finalData.TokenInterface.Name;
        }

        private ILiteralReferenceProductionRuleItem GetLiteralRef()
        {
            if (this.leftMerge != null)
                return ((RuleDataEnumItem)(this.leftMerge)).GetLiteralRef();
            if (this.reference is ILiteralReferenceProductionRuleItem)
                return (ILiteralReferenceProductionRuleItem)this.reference;
            return null;
        }

        protected override string GetRankText()
        {
            var lRef = this.GetLiteralRef();
            if (lRef != null && lRef.Counter)
            {
                /* *
                 * Counters inherently consume a rank of their own.
                 * */
                string result = "";
                for (int i = 0; i < this.Rank - 1; i++)
                    result += "[]";
                return result;
            }
            return base.GetRankText();
        }
    }
}
