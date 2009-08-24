using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class RuleDataNamedEnumItem :
        RuleDataEnumItem,
        IRuleDataNamedItem
    {
        public RuleDataNamedEnumItem(string name, TokenEnumFinalData finalData, ILiteralReferenceProductionRuleItem reference)
            : base(finalData, reference)
        {
            this.Name = name;
        }
        public RuleDataNamedEnumItem(string name, TokenEnumFinalData finalData, ITokenReferenceProductionRuleItem reference)
            : base(finalData, reference)
        {
            this.Name = name;
        }

        internal RuleDataNamedEnumItem(string name, TokenEnumFinalData finalData)
            : base(finalData)
        {
            this.Name = name;
        }

        #region IRuleDataNamedItem Members

        /// <summary>
        /// Returns the <see cref="String"/> representing the 
        /// name of the current <see cref="RuleDataNamedEnumItem"/>.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        /// <summary>
        /// Returns whether the <paramref name="other"/> element
        /// is of proper nature relative to the current
        /// <see cref="RuleDataNamedEnumItem"/>.
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
            var cOther = other as RuleDataNamedEnumItem;
            if (cOther == null)
                return false;
            return cOther.Name == this.Name && base.AreEqualNaturedImpl(other);
        }

        /// <summary>
        /// Obtains the best natured <see cref="IRuleDataItem"/> from the
        /// <paramref name="equalNaturedSet"/> provided.
        /// </summary>
        /// <param name="equalNaturedSet">An array of <see cref="IRuleDataItem"/> 
        /// elements which are equally natured to the current
        /// <see cref="RuleDataNamedEnumItem"/>.</param>
        /// <returns>The best natured <see cref="IRuleDataItem"/> of the
        /// <paramref name="equalNaturedSet"/>.</returns>
        /// <remarks>Returns the transitionFirst element since named groupings
        /// are explicitly declared.</remarks>
        public override IRuleDataItem ObtainBestNatured(IRuleDataItem[] equalNaturedSet)
        {
            if (equalNaturedSet == null)
                throw new ArgumentException("equalNaturedSet");
            /* *
             * Named elements are forced groupings, no two elements of the same
             * name and same type should exist, even if the sub-set information
             * is different.
             * */
            return equalNaturedSet[0];
        }

        /// <summary>
        /// Merges the <paramref name="other"/> <see cref="IRuleDataItem"/>
        /// with the current <see cref="RuleDataItem"/>.
        /// </summary>
        /// <param name="other">The <see cref="IRuleDataItem"/> to merge 
        /// with the current element.</param>
        /// <returns>A <see cref="IRuleDataItem"/> which represents the merge
        /// between the current <paramref name="RuleDataNamedEnumItem"/> and the
        /// <paramref name="other"/> <see cref="IRuleDataItem"/>.</returns>
        public override IRuleDataItem MergeWith(IRuleDataItem other)
        {
            if (other == null)
                throw new ArgumentNullException("other");
            if (!this.AreEqualNatured(other))
                throw new ArgumentException("other");
            IRuleDataEnumItem cOther = ((IRuleDataEnumItem)(other));
            return base.MergeWithImpl(cOther);
        }

        protected override RuleDataEnumItem MergeWithInstanceImpl()
        {
            return new RuleDataNamedEnumItem(this.Name, base.finalData);
        }

        public override string ToString()
        {
            if (this.finalData.TokenInterface != null)
                return string.Format("{0}{1} {2}", this.GetTypeName(), this.Rank > 0 ? GetRankText() : string.Empty, this.Name);
            return null;
        }
    }
}
