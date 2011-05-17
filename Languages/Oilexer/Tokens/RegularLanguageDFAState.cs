using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Provides a regular language deterministic state.
    /// </summary>
    public class RegularLanguageDFAState :
        DFAState<RegularLanguageSet, RegularLanguageDFAState, ITokenSource>
    {
        public RegularLanguageDFAState()
        {
        }

        protected override bool SourceSetPredicate(ITokenSource source)
        {
            if (!(source is ITokenItem))
                return false;
            var tSource = (ITokenItem)source;
            return !string.IsNullOrEmpty(tSource.Name);
        }

        protected override IFiniteAutomataTransitionTable<RegularLanguageSet, RegularLanguageDFAState, RegularLanguageDFAState> InitializeOutTransitionTable()
        {
            return new RegularLanguageDFATransitionTable();
        }
    }
}
