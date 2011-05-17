using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class TemplateParamReferenceProductionRuleItem :
        ProductionRuleItem,
        ITemplateParamReferenceProductionRuleItem
    {
        /// <summary>
        /// Data member for <see cref="Source"/>.
        /// </summary>
        private IProductionRuleTemplateEntry source;
        /// <summary>
        /// Data member for <see cref="Reference"/>.
        /// </summary>
        private IProductionRuleTemplatePart reference;

        /// <summary>
        /// Creates a new <see cref="TemplateParamReferenceProductionRuleItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="TemplateParamReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="TemplateParamReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="TemplateParamReferenceProductionRuleItem"/> was declared.</param>
        public TemplateParamReferenceProductionRuleItem(IProductionRuleTemplateEntry source, IProductionRuleTemplatePart reference, int column, int line, long position)
            : base(column, line, position)
        {
            this.source = source;
            this.reference = reference;
        }

        #region ITemplateParamReferenceProductionRuleItem Members

        public IProductionRuleTemplatePart Reference
        {
            get { return this.reference; }
        }

        public IProductionRuleTemplateEntry Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Creates a copy of the current <see cref="TemplateParamReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITemplateParamReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="TemplateParamReferenceProductionRuleItem"/>.</returns>
        public new ITemplateParamReferenceProductionRuleItem Clone()
        {
            return ((ITemplateParamReferenceProductionRuleItem)(base.Clone()));
        }

        #endregion

        protected override sealed object OnClone()
        {
            TemplateParamReferenceProductionRuleItem result = new TemplateParamReferenceProductionRuleItem(this.Source, this.Reference, this.Column, this.Line, this.Position);
            this.CloneData(result);
            return result;
        }
    }
}
