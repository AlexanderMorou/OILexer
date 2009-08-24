using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal abstract class RuleDataItem :
        IRuleDataItem
    {
        public RuleDataItem()
        {
            this.Rank = 0;
        }

        #region IRuleDataItem Members

        public int Rank { get; set; }

        /// <summary>
        /// Returns the <see cref="RuleDataItemType"/> of the current element.
        /// </summary>
        public abstract RuleDataItemType ElementType { get; }

        public bool AreEqualNatured(IRuleDataItem other)
        {
            return   other.GetType() == this.GetType() &&
                    (ElementType == other.ElementType) &&
                     AreEqualNaturedImpl(other);
        }

        protected abstract bool AreEqualNaturedImpl(IRuleDataItem other);

        public abstract IRuleDataItem ObtainBestNatured(IRuleDataItem[] equalNaturedSet);

        /// <summary>
        /// Merges the <paramref name="other"/> <see cref="IRuleDataItem"/>
        /// with the current <see cref="RuleDataItem"/>.
        /// </summary>
        /// <param name="other">The <see cref="IRuleDataItem"/> to merge 
        /// with the current element.</param>
        /// <returns>A <see cref="IRuleDataItem"/> which represents the merge
        /// between the current <paramref name="RuleDataItem"/> and the
        /// <paramref name="other"/> <see cref="IRuleDataItem"/>.</returns>
        public abstract IRuleDataItem MergeWith(IRuleDataItem other);

        #endregion

        protected virtual string GetRankText()
        {
            string result = "";
            for (int i = 0; i < this.Rank; i++)
                result += "[]";
            return result;
        }
    }
}
