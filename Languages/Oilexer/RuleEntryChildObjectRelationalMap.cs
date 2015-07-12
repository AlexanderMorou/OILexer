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
        internal RuleEntryChildObjectRelationalMap(IOilexerGrammarScannableEntry[] implementsSeries, OilexerGrammarFileObjectRelationalMap fileMap, IOilexerGrammarProductionRuleEntry entry)
            : base(implementsSeries, fileMap, entry)
        {

        }

        //#region IRuleEntryChildObjectRelationalMap Members


        //#endregion
    }
}
