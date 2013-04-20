using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    internal class RuleEntryChildObjectRelationalMap :
        RuleEntryObjectRelationalMap,
        IRuleEntryChildObjectRelationalMap
    {
        private IControlledDictionary<IProductionRuleEntry, IIntermediateEnumFieldMember> caseFields;
        internal RuleEntryChildObjectRelationalMap(IProductionRuleEntry[] implementsSeries, GDFileObjectRelationalMap fileMap, IProductionRuleEntry entry)
            : base(implementsSeries, fileMap, entry)
        {

        }

        #region IRuleEntryChildObjectRelationalMap Members

        public IControlledDictionary<IProductionRuleEntry, IIntermediateEnumFieldMember> CaseFields
        {
            get
            {
                if (this.caseFields == null)
                    this.caseFields = new ControlledDictionary<IProductionRuleEntry, IIntermediateEnumFieldMember>(
                        from caseField in fileMap.CasesLookup
                        where caseField.Keys.Key2 == this.Entry
                        select new KeyValuePair<IProductionRuleEntry, IIntermediateEnumFieldMember>(caseField.Keys.Key1, caseField.Value));
                return this.caseFields;
            }
        }

        #endregion
    }
}
