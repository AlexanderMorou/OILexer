using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class RuleDataCaptureItem :
        RuleDataTokenItemBase,
        IRuleDataCaptureItem
    {

        private IRuleDataCaptureItem leftMerge,
                                     rightMerge;
        private ITokenReferenceProductionRuleItem sourceElement;
        /// <summary>
        /// Creates a new <see cref="RuleDataCaptureItem"/> with the
        /// <paramref name="source"/> capture token.
        /// </summary>
        /// <param name="source">The capturing <see cref="ITokenEntry"/> 
        /// which the <see cref="RuleDataCaptureItem"/> wraps.</param>
        public RuleDataCaptureItem(ITokenEntry source, TokenFinalData finalData)
            : base(source, finalData)
        {
        }

        /// <summary>
        /// Creates a new <see cref="RuleDataCaptureItem"/> with the
        /// <paramref name="sourceElement"/> reference.
        /// </summary>
        /// <param name="sourceElement">The capturing <see cref="ITokenReferenceProductionRuleItem"/> 
        /// which the <see cref="RuleDataCaptureItem"/> wraps.</param>
        public RuleDataCaptureItem(ITokenReferenceProductionRuleItem sourceElement, TokenFinalData finalData)
            : this(sourceElement.Reference, finalData)
        {
            this.sourceElement = sourceElement;
        }

        /// <summary>
        /// Returns the type of element the <see cref="RuleDataCaptureItem"/>
        /// is.
        /// </summary>
        /// <remarks>Returns <see cref="RuleDataItemType.Capture"/></remarks>
        public override RuleDataItemType ElementType
        {
            get { return RuleDataItemType.Capture; }
        }

        /// <summary>
        /// Returns whether the <paramref name="other"/> element
        /// is of proper nature relative to the current
        /// <see cref="RuleDataCaptureItem"/>.
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
            var cOther = other as RuleDataCaptureItem;
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
        /// <see cref="RuleDataCaptureItem"/>.</param>
        /// <returns>The best natured <see cref="IRuleDataItem"/> of the
        /// <paramref name="equalNaturedSet"/>.</returns>
        /// <remarks>All captures of equal sources are equivalent; thus, this
        /// returns the transitionFirst element.</remarks>
        public override IRuleDataItem ObtainBestNatured(IRuleDataItem[] equalNaturedSet)
        {
            if (equalNaturedSet == null)
                throw new ArgumentNullException("equalNaturedSet");
            return equalNaturedSet[0];
        }

        public override sealed IRuleDataItem MergeWith(IRuleDataItem other)
        {
            if (!this.AreEqualNatured(other))
                throw new ArgumentException("other");
            var cOther = (IRuleDataCaptureItem)other;
            var mergeItem = GetMergeWithInstance();
            mergeItem.Rank = Math.Max(this.Rank, other.Rank);
            mergeItem.leftMerge = this;
            mergeItem.rightMerge = cOther;
            return mergeItem;
        }


        /// <summary>
        /// Obtains the an instance for <see cref="MergeWith(IRuleDataItem)"/>.
        /// </summary>
        /// <returns>A new <see cref="RuleDataCaptureItem"/> instance.</returns>
        protected virtual RuleDataCaptureItem GetMergeWithInstance()
        {
            return new RuleDataCaptureItem(this.Source, this.FinalData);
        }

        public override string ToString()
        {
            if (this.FinalData.TokenInterface != null)
                return string.Format("{0}{1}", this.FinalData.TokenBaseType.Name, this.Rank > 0 ? GetRankText() : string.Empty);
            return null;
        }

    }
}
