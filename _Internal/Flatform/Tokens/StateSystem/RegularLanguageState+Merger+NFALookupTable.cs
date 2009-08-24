using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;
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
            internal class NFALookupTable
            {
                private List<NFALookup> lookups = new List<NFALookup>();
                public bool ContainsTransitionFor(RegularLanguageState[] state, RegularLanguageBitArray transition)
                {
                    /* *
                     * Basically where a series of states intersect they contain a single
                     * result state that is dependent upon the states that exist in the 
                     * intersection.  This ensures that if states A, B, and C overlap on
                     * rule E, there is a state created for it, and if states A and B overlap on
                     * rule P, there is a separate state created for the intersection.
                     * */
                    foreach (var lookup in lookups)
                    {
                        if (lookup.NFACount != state.Length)
                            continue;
                        bool match = true;
                        var nfaC = lookup.NFACount;
                        for (int i = 0; i < nfaC; i++)
                        {
                            bool currentMatch = false;
                            var nfaCur = lookup.NFAState[i];
                            for (int j = 0; j < state.Length; j++)
                                if (nfaCur == state[j])
                                {
                                    currentMatch = true;
                                    break;
                                }
                            if (!currentMatch)
                            {
                                match = false;
                                break;
                            }
                        }
                        if (!match)
                            continue;
                        if (lookup.Requirement == transition)
                            return true;
                    }

                    return false;
                }

                public NFALookup CreateTransitionFor(RegularLanguageState[] original, RegularLanguageBitArray transition)
                {
                    RegularLanguageState dfaState = new RegularLanguageState();
                    NFALookup result = new NFALookup(original, dfaState, transition);
                    if (original.Any(p => p.IsMarked))
                        dfaState.MakeEdge();
                    lookups.Add(result);
                    return result;
                }

                public NFALookup GetTransitionFor(RegularLanguageState[] state, RegularLanguageBitArray target)
                {
                    foreach (var lookup in lookups)
                    {
                        if (lookup.NFAState.All(p => state.Contains(p)) &&
                            lookup.NFAState.Count() == state.Length)
                            if (lookup.Requirement == target)
                                return lookup;
                    }
                    //Better safe than sorry.
                    throw new ArgumentOutOfRangeException("target");
                }
            }
        }
    }
}
