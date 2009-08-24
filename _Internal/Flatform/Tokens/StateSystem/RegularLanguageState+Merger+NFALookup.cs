using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens.StateSystem
{
    partial class RegularLanguageState
    {
        partial class Merger
        {
            internal class NFALookup
            {
                public NFALookup(RegularLanguageState[] nfaState, RegularLanguageState dfaState,
                                 RegularLanguageBitArray requirement)
                {
                    RegularLanguageState[] set = new RegularLanguageState[nfaState.Length];
                    nfaState.CopyTo(set, 0);
                    this.NFAState = set;
                    this.NFACount = set.Length;
                    this.DFAState = dfaState;
                    this.Requirement = requirement;
                }
                public RegularLanguageState[] NFAState { get; private set; }
                public RegularLanguageState DFAState { get; private set; }
                public RegularLanguageBitArray Requirement { get; private set; }
                public int NFACount { get; private set; }
                public override string ToString()
                {
                    return Requirement.ToString() + " " + DFAState.ToString();
                }
            }
        }
    }
}
