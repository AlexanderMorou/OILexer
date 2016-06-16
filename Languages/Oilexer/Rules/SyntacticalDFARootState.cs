using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
/*---------------------------------------------------------------------\
| Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    [DebuggerDisplay("{DebuggerToString(),nq}")]
    public class SyntacticalDFARootState :
        SyntacticalDFAState
    {
        private IOilexerGrammarProductionRuleEntry entry;

        public SyntacticalDFARootState(IOilexerGrammarProductionRuleEntry entry, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
            : base(lookup, symbols)
        {
            this.entry = entry;
        }
        internal IOilexerGrammarProductionRuleEntry Entry
        {
            get
            {
                return this.entry;
            }
        }

        public string EntryName
        {
            get
            {
                return this.Entry.Name;
            }
        }

        internal string DebuggerToString()
        {
            return string.Format("EntryState {0}", this.EntryName);
        }

        public override string ToString()
        {
            return string.Format("Root state for {0}", this.Entry.Name);
        }

        public void ReduceDFA()
        {
            this.ReduceDFA(entry.MaxReduce);
        }
    }
}
