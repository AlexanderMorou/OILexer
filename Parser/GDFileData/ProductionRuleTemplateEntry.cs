using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using System.Diagnostics;

namespace Oilexer.Parser.GDFileData
{
    public class ProductionRuleTemplateEntry :
        ProductionRuleEntry,
        IProductionRuleTemplateEntry
    {
        /// <summary>
        /// Data member for <see cref="PartNames"/>.
        /// </summary>
        private IProductionRuleTemplateParts parts;

        public ProductionRuleTemplateEntry(string name, EntryScanMode scanMode, IList<IProductionRuleTemplatePart> partNames, string fileName, int column, int line, long position)
            : base(name, scanMode, fileName, column, line, position)
        {
            this.parts = new PartCollection(partNames);
        }

        #region IProductionRuleTemplateEntry Members

        /// <summary>
        /// The names of the parts associated with the <see cref="ProductionRuleTemplateEntry"/>.
        /// </summary>
        public IProductionRuleTemplateParts Parts
        {
            get { return this.parts; }
        }

        #endregion

        protected class PartCollection :
            ReadOnlyCollection<IProductionRuleTemplatePart>,
            IProductionRuleTemplateParts
        {
            internal PartCollection()
            {
            }
            internal PartCollection(ICollection<IProductionRuleTemplatePart> prtp)
                : base(prtp)
            {
                
            }
        }

        #region IProductionRuleTemplateEntry Members


        [DebuggerStepThrough]
        public TemplateArgumentInformation GetArgumentInformation()
        {
            bool isFixed=true;
            int invalid = 0;
            int @fixed = 0;
            int dynamic = 0;
            foreach (IProductionRuleTemplatePart iprtp in this.parts)
            {
                if (iprtp.RepeatOptions == ScannableEntryItemRepeatInfo.OneOrMore)
                {
                    if (isFixed)
                        isFixed = false;
                    dynamic++;
                }
                else if (iprtp.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                {
                    if (!isFixed)
                        invalid++;
                    else
                        @fixed++;
                }
            }
            return new TemplateArgumentInformation(@fixed, dynamic, invalid);
        }

        #endregion
    }
}
