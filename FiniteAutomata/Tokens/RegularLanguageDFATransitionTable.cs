using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Tokens
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
