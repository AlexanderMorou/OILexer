using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata
{
    partial class NFAState<TCheck, TState, TDFA, TSourceElement>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            NFAState<TCheck, TState, TDFA, TSourceElement>
        where TDFA :
            FiniteAutomataState<TCheck, TDFA, TDFA, TSourceElement>,
            IDFAState<TCheck, TDFA, TSourceElement>,
            new()
        where TSourceElement :
            IFiniteAutomataSource
    {
        private class DFAEntryTable :
            Dictionary<TCheck, DFAEntrySet>
        {
            private Func<TDFA> GetState;
            public DFAEntryTable(Func<TDFA> getState)
            {
                this.GetState = getState;
            }
            public bool ContiansEntryFor(TCheck condition, List<TState> set)
            {
                if (!this.ContainsKey(condition))
                    return false;
                return this[condition].Contains(set);
            }

            public DFAEntry Add(TCheck condition, List<TState> set)
            {
                if (!this.ContainsKey(condition))
                    base.Add(condition, new DFAEntrySet(condition));
                if (!this[condition].Contains(set))
                    this[condition].Add(new DFAEntry(set, GetState()));
                return this[condition][set];
            }
        }
    }
}
