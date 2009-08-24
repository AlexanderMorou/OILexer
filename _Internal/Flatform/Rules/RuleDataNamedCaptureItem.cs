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
    internal class RuleDataNamedCaptureItem :
        RuleDataCaptureItem,
        IRuleDataNamedItem
    {
        /// <summary>
        /// Creates a new <see cref="RuleDataNamedCaptureItem"/> with the
        /// <paramref name="source"/> capture token.
        /// </summary>
        /// <param name="name">The <see cref="String"/> value representing the 
        /// name of the current <see cref="RuleDataNamedCaptureItem"/></param>
        /// <param name="source">The capturing <see cref="ITokenEntry"/> 
        /// which the <see cref="RuleDataNamedCaptureItem"/> wraps.</param>
        public RuleDataNamedCaptureItem(string name, ITokenEntry source, TokenFinalData finalData)
            : base(source, finalData)
        {
            this.Name = name;
        }
        
        /// <summary>
        /// Creates a new <see cref="RuleDataNamedCaptureItem"/> with the
        /// <paramref name="sourceElement"/> reference.
        /// </summary>
        /// <param name="sourceElement">The capturing <see cref="ITokenReferenceProductionRuleItem"/> 
        /// which the <see cref="RuleDataNamedCaptureItem"/> wraps.</param>
        public RuleDataNamedCaptureItem(string name, ITokenReferenceProductionRuleItem sourceElement, TokenFinalData finalData)
            : base(sourceElement, finalData)
        {
            this.Name = name;
        }

        #region IRuleDataNamedItem Members

        /// <summary>
        /// Returns the <see cref="String"/> representing the 
        /// name of the current <see cref="RuleDataNamedCaptureItem"/>.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        /// <summary>
        /// Returns whether the <paramref name="other"/> element
        /// is of proper nature relative to the current
        /// <see cref="RuleDataNamedCaptureItem"/>.
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
            var cOther = other as RuleDataNamedCaptureItem;
            if (cOther == null)
                return false;
            return cOther.Name == this.Name && base.AreEqualNaturedImpl(other);
        }

        protected override RuleDataCaptureItem GetMergeWithInstance()
        {
            return new RuleDataNamedCaptureItem(this.Name, base.Source, this.FinalData);
        }

        public override string ToString()
        {
            if (this.FinalData.TokenInterface != null)
            {
                return string.Format("{0}{1} {2}", this.FinalData.TokenBaseType.Name, this.Rank > 0 ? GetRankText() : string.Empty, this.Name);
            }
            return null;
        }

    }
}
