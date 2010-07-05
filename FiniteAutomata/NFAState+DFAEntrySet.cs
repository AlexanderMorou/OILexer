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
        private class DFAEntrySet :
            List<DFAEntry>
        {
            public TCheck Condition { get; private set; }
            public DFAEntrySet(TCheck condition)
            {
                this.Condition = condition;
            }
            public bool Contains(List<TState> set)
            {
                return this.Any(p => p.Equals(set));
            }
            public DFAEntry this[List<TState> set]
            {
                get
                {
                    return this.First(p => p.Equals(set));
                }
            }
        }
    }
}
