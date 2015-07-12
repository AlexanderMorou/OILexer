using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public class OilexerGrammarProductionRuleTemplateEntry :
        OilexerGrammarProductionRuleEntry,
        IOilexerGrammarProductionRuleTemplateEntry
    {
        /// <summary>
        /// Data member for <see cref="PartNames"/>.
        /// </summary>
        private IProductionRuleTemplateParts parts;

        public OilexerGrammarProductionRuleTemplateEntry(string name, EntryScanMode scanMode, IList<IProductionRuleTemplatePart> partNames, string fileName, int column, int line, long position)
            : base(name, scanMode, fileName, column, line, position)
        {
            this.parts = new PartCollection(partNames);
        }

        //#region IOilexerGrammarProductionRuleTemplateEntry Members

        /// <summary>
        /// The names of the parts associated with the <see cref="OilexerGrammarProductionRuleTemplateEntry"/>.
        /// </summary>
        public IProductionRuleTemplateParts Parts
        {
            get { return this.parts; }
        }

        //#endregion

        protected class PartCollection :
            ControlledCollection<IProductionRuleTemplatePart>,
            IProductionRuleTemplateParts
        {
            internal PartCollection()
            {
            }
            internal PartCollection(IList<IProductionRuleTemplatePart> prtp)
                : base(prtp)
            {
                
            }
        }

        //#region IOilexerGrammarProductionRuleTemplateEntry Members


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

        //#endregion
        public override string ToString()
        {
#if DEBUG
            if (this.debugString == null)
                this.debugString = string.Format("{0}<{3}> ::=\r\n{2} {1};", this.Name, GetBodyString(), IsRuleCollapsePoint ? ">" : string.Empty, string.Join(", ", from k in this.Parts
                                                                                                                                                                 select k.Name));
            return this.debugString;
#else
            return string.Format("{0}<{3}> ::={2} {1};", this.Name, GetBodyString(), IsRuleCollapsePoint ? ">" : string.Empty, string.Join(", ", from k in this.Parts
                                                                                                                                                 select k.Name));
#endif
        }
    }
}
