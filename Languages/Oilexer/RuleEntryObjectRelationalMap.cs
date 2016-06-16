using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
/*---------------------------------------------------------------------\
| Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    using RuleTree = KeyedTree<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using RuleTreeNode = KeyedTreeNode<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using AllenCopeland.Abstraction.Slf.Ast;
    internal class RuleEntryObjectRelationalMap :
        EntryObjectRelationalMap,
        IRuleEntryObjectRelationalMap
    {

        public new IOilexerGrammarProductionRuleEntry Entry
        {
            get { return (IOilexerGrammarProductionRuleEntry)base.Entry; }
        }

        public RuleEntryObjectRelationalMap(IOilexerGrammarScannableEntry[] implementsSeries, OilexerGrammarFileObjectRelationalMap fileMap, IOilexerGrammarProductionRuleEntry entry)
            : base(implementsSeries, fileMap, entry)
        {
            this.DebuggerProxyTargets = new List<IIntermediateClassType>();
        }


        public IIntermediateClassType DebuggerProxy { get; set; }

        public List<IIntermediateClassType> DebuggerProxyTargets { get; set; }
    }
}
