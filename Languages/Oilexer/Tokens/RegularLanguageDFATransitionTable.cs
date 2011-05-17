using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class RegularLanguageDFATransitionTable :
        FiniteAutomataSingleTargetTransitionTable<RegularLanguageSet, RegularLanguageDFAState>
    {
        public bool Contains(char @char)
        {
            foreach (var key in this.Keys)
                if (key.Contains(@char))
                    return true;
            return false;
        }

        public RegularLanguageDFAState this[char @char]
        {
            get
            {
                foreach (var key in this.Keys)
                    if (key.Contains(@char))
                        return this[key];
                throw new ArgumentException("char");
            }
        }

    }
}
