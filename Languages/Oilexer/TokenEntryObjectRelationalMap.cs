using AllenCopeland.Abstraction.Slf.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    internal class TokenEntryObjectRelationalMap :
        EntryObjectRelationalMap,
        ITokenEntryObjectRelationalMap
    {
        public TokenEntryObjectRelationalMap(IOilexerGrammarScannableEntry[] implementsSeries, OilexerGrammarFileObjectRelationalMap fileMap, IOilexerGrammarTokenEntry entry)
            : base(implementsSeries, fileMap, entry)
        {
            this.DebuggerProxyTargets = new List<IIntermediateClassType>();
        }
        public new IOilexerGrammarProductionRuleEntry Entry { get { return (IOilexerGrammarProductionRuleEntry)base.Entry; } }
        public IIntermediateClassType DebuggerProxy { get; set; }
        public List<IIntermediateClassType> DebuggerProxyTargets { get; set; }
    }
}
