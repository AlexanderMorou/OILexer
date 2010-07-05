using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public enum TemplatePartExpectedSpecial
    {
        /// <summary>
        /// No special expectancy.
        /// </summary>
        None,
        /// <summary>
        /// Expects a token reference.
        /// </summary>
        Token,
        /// <summary>
        /// Expects a rule reference.
        /// </summary>
        Rule,
        /// <summary>
        /// Expects a token, or a rule.
        /// </summary>
        TokenOrRule,
    }

    public class ProductionRuleTemplatePart :
        ProductionRuleItem,
        IProductionRuleTemplatePart
    {
        TemplatePartExpectedSpecial specialExpectancy;
        private bool repeatSeries;
        private IProductionRuleItem expectedSpecific;

        public ProductionRuleTemplatePart(string name, bool repeatSeries, TemplatePartExpectedSpecial specialExpectancy, int line, int column, long position)
            : base(column, line, position)
        {
            this.specialExpectancy = specialExpectancy;
            this.repeatSeries = repeatSeries;
            this.Name = name;
            this.RepeatOptions = repeatSeries ? ScannableEntryItemRepeatInfo.OneOrMore : ScannableEntryItemRepeatInfo.None;
        }

        public ProductionRuleTemplatePart(string name, bool inSeries, string expectedTargetName, int line, int column, long position)
            : base(column, line, position)
        {
            this.Name = name;
            this.repeatSeries = inSeries;
            if (expectedTargetName != null)
                this.expectedSpecific = new SoftReferenceProductionRuleItem(expectedTargetName, null, line, column, position, false, false);
            this.RepeatOptions = repeatSeries ? ScannableEntryItemRepeatInfo.OneOrMore : ScannableEntryItemRepeatInfo.None;
        }
        private ProductionRuleTemplatePart(string name, TemplatePartExpectedSpecial specialExpectancy, IProductionRuleItem reference, int line, int column, long position)
            : base(column, line, position)
        {
            this.Name = name;
            this.specialExpectancy = specialExpectancy;
            this.expectedSpecific = reference;
        }
        protected override object OnClone()
        {
            ProductionRuleTemplatePart prtp = new ProductionRuleTemplatePart(this.Name, this.SpecialExpectancy, this.expectedSpecific, this.Line, this.Column, this.Position);
            base.CloneData(prtp);
            return prtp;
        }

        #region IProductionRuleTemplatePart Members

        public TemplatePartExpectedSpecial SpecialExpectancy
        {
            get { return this.specialExpectancy; }
        }

        public IProductionRuleItem ExpectedSpecific
        {
            get { return this.expectedSpecific; }
            internal set
            {
                this.expectedSpecific = value;
            }
        }

        #endregion
    }
}
