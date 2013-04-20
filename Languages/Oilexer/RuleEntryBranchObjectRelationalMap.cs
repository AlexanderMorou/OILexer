using AllenCopeland.Abstraction.Slf.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    class RuleEntryBranchObjectRelationalMap :
        RuleEntryObjectRelationalMap,
        IRuleEntryBranchObjectRelationalMap
    {
        internal RuleEntryBranchObjectRelationalMap(IIntermediateEnumType casesEnum, IProductionRuleEntry[] implementsSeries, GDFileObjectRelationalMap fileMap, IProductionRuleEntry entry)
            : base(implementsSeries, fileMap, entry)
        {
            this.CasesEnum = casesEnum;
        }

        #region IRuleEntryBranchObjectRelationalMap Members

        public IIntermediateEnumType CasesEnum { get; private set; }

        #endregion
    }
}
