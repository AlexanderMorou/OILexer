using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class RuleDataRuleReferenceItem :
        RuleDataItem,
        IRuleDataRuleReferenceItem
    {
        private IRuleReferenceProductionRuleItem sourceItem;
        private IRuleDataRuleReferenceItem leftMerge,
                                           rightMerge;
        public RuleDataRuleReferenceItem(IProductionRuleEntry reference)
        {
            this.Reference = reference;
        }

        public RuleDataRuleReferenceItem(IRuleReferenceProductionRuleItem sourceItem)
        {
            this.Reference = sourceItem.Reference;
            this.sourceItem = sourceItem;
        }

        /// <summary>
        /// Returns the <see cref="RuleDataItemType"/> of the current element.
        /// </summary>
        /// <remarks>Returns <see cref="RuleDataItemType.Rule"/>.</remarks>
        public override RuleDataItemType ElementType
        {
            get { return RuleDataItemType.Rule; }
        }

        #region IRuleDataRuleReferenceItem Members

        /// <summary>
        /// Returns the <see cref="IProductionRuleEntry"/> the 
        /// <see cref="RuleDataRuleReferenceItem"/> represents.
        /// </summary>
        public IProductionRuleEntry Reference { get; private set; }

        #endregion

        /// <summary>
        /// Returns whether the <paramref name="other"/> element
        /// is of proper nature relative to the current
        /// <see cref="RuleDataRuleReferenceItem"/>.
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
            var cOther = other as RuleDataRuleReferenceItem;
            if (cOther == null)
                return false;
            return cOther.Reference == this.Reference;
        }

        /// <summary>
        /// Obtains the best natured <see cref="IRuleDataItem"/> from the
        /// <paramref name="equalNaturedSet"/> provided.
        /// </summary>
        /// <param name="equalNaturedSet">An array of <see cref="IRuleDataItem"/> 
        /// elements which are equally natured to the current
        /// <see cref="RuleDataRuleReferenceItem"/>.</param>
        /// <returns>The best natured <see cref="IRuleDataItem"/> of the
        /// <paramref name="equalNaturedSet"/>.</returns>
        /// <remarks>All rule references of equal sources are equivalent; thus, this
        /// returns the transitionFirst element.</remarks>
        public override IRuleDataItem ObtainBestNatured(IRuleDataItem[] equalNaturedSet)
        {
            if (equalNaturedSet == null)
                throw new ArgumentNullException("equalNaturedSet");
            return equalNaturedSet[0];
        }

        public override IRuleDataItem MergeWith(IRuleDataItem other)
        {
            if (!this.AreEqualNatured(other))
                throw new ArgumentException("other");
            var cOther = (IRuleDataRuleReferenceItem)other;
            var mergeItem = GetMergeWithInstance();
            mergeItem.Rank = Math.Max(this.Rank, other.Rank);
            mergeItem.leftMerge = this;
            mergeItem.rightMerge = cOther;
            return mergeItem;
        }

        /// <summary>
        /// Obtains the an instance for <see cref="MergeWith(IRuleDataItem)"/>.
        /// </summary>
        /// <returns>A new <see cref="RuleDataRuleReferenceItem"/> instance.</returns>
        protected virtual RuleDataRuleReferenceItem GetMergeWithInstance()
        {
            return new RuleDataRuleReferenceItem(this.Reference);
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", this.Reference.Name, base.GetRankText());
        }
    }
}
