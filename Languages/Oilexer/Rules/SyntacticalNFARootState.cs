using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class SyntacticalNFARootState :
        SyntacticalNFAState
    {
        private IOilexerGrammarProductionRuleEntry entry;
        internal ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup;
        internal GrammarSymbolSet symbols;
        public SyntacticalNFARootState(IOilexerGrammarProductionRuleEntry entry, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup, GrammarSymbolSet symbols)
            : base(lookup, symbols)
        {
            this.entry = entry;
            this.lookup = lookup;
        }

        protected override SyntacticalDFAState GetRootDFAState()
        {
            return new SyntacticalDFARootState(this.entry, lookup, symbols);
        }

        /// <summary>
        /// Creates a version of the current
        /// <see cref="RegularLanguageNFARootState"/> which is 
        /// deterministic by creating a left-side union on elements
        /// which overlap on their <see cref="RegularLanguageSet"/> 
        /// transition requirements.
        /// </summary>
        /// <returns>A new <see cref="RegularLanguageDFARootState"/> 
        /// instance which represents the current automation
        /// in a deterministic manner.</returns>
        public new SyntacticalDFARootState DeterminateAutomata()
        {
            return (SyntacticalDFARootState)base.DeterminateAutomata();
        }

        public IOilexerGrammarProductionRuleEntry Source { get { return this.entry; } }

    }
}
