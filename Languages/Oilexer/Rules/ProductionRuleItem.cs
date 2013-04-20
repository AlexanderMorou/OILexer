using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a base abstract implementation of <see cref="IProductionRuleItem"/>.
    /// </summary>
    public abstract class ProductionRuleItem :
        ScannableEntryItem,
        IProductionRuleItem
    {
        /// <summary>
        /// Creates a new <see cref="ProductionRuleItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="ProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="ProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="ProductionRuleItem"/> was declared.</param>
        protected ProductionRuleItem(int column, int line, long position)
            : base(column, line, position)
        {

        }

        protected ProductionRuleItem(Dictionary<string, string> constraints, int column, int line, long position)
            : base(column, line, position)
        {
            this.ConditionalConstraints = new ControlledDictionary<string, string>(constraints);
        }

        #region IProductionRuleItem Members

        public new IProductionRuleItem Clone()
        {
            var result = ((ProductionRuleItem)(base.Clone()));
            if (this.ConditionalConstraints != null)
                result.ConditionalConstraints = this.ConditionalConstraints;
            return result;
        }

        public IControlledDictionary<string, string> ConditionalConstraints { get; private set; }

        #endregion

        protected override string ToStringFurtherOptions()
        {
            return string.Empty;
        }

    }
}
