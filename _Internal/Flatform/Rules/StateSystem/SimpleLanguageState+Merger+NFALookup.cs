using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules.StateSystem
{
    partial class SimpleLanguageState
    {
        partial class Merger
        {
            /// <summary>
            /// Data class used to represent the necessary data for creating a
            /// DFA node from a NFA source, contains all the original
            /// NFA states that were used to create the <see cref="NFALookup.DFAState"/>.
            /// </summary>
            internal class NFALookup
            {
                public NFALookup(SimpleLanguageState[] nfaState, SimpleLanguageState dfaState,
                                 SimpleLanguageBitArray requirement)
                {
                    this.NFAState = nfaState;
                    NFACount = nfaState.Length;
                    this.DFAState = dfaState;
                    this.Requirement = requirement;
                }
                public SimpleLanguageState[] NFAState { get; private set; }
                public int NFACount { get; private set; }
                public SimpleLanguageState DFAState { get; private set; }
                public SimpleLanguageBitArray Requirement { get; private set; }
                public override string ToString()
                {
                    return Requirement.ToString() + " " + DFAState.ToString();
                }
            }
        }
    }
}
