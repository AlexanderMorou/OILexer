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
    internal class RuleDataNamedRuleReferenceItem :
        RuleDataRuleReferenceItem,
        IRuleDataNamedItem
    {
        public RuleDataNamedRuleReferenceItem(string name, IRuleReferenceProductionRuleItem sourceItem)
            : base(sourceItem)
        {
            this.Name = name;
        }
        public RuleDataNamedRuleReferenceItem(string name, IProductionRuleEntry reference)
            : base(reference)
        {
            this.Name = name;
        }

        #region IRuleDataNamedItem Members

        public string Name { get; private set; }

        #endregion

        protected override bool AreEqualNaturedImpl(IRuleDataItem other)
        {
            if (other == null)
                throw new ArgumentNullException("other");
            var cOther = other as RuleDataNamedRuleReferenceItem;
            if (cOther == null)
                return false;
            return cOther.Name == this.Name && base.AreEqualNaturedImpl(other);
        }

        /// <summary>
        /// Obtains the an instance for <see cref="RuleDataRuleReferenceItem.MergeWith(IRuleDataItem)"/>.
        /// </summary>
        /// <returns>A new <see cref="RuleDataNamedRuleReferenceItem"/> instance.</returns>
        protected override RuleDataRuleReferenceItem GetMergeWithInstance()
        {
            return new RuleDataNamedRuleReferenceItem(this.Name, base.Reference);
        }

        public override string ToString()
        {
            return string.Format("{0}{1} {2}", this.Reference.Name, base.GetRankText(), this.Name);
        }
    }
}
