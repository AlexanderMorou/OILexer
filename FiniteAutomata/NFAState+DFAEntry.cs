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
        private class DFAEntry :
            HashSet<TState>
        {
            public TDFA DFA { get; private set; }
            public DFAEntry(IEnumerable<TState> states, TDFA state)
                : base(states)
            {
                this.DFA = state;
                bool noEdge = states.Any(p => p.ForcedNoEdge);
                bool isEdge = states.Any(p => p.IsEdge);
                if (noEdge)
                    this.DFA.ForcedNoEdge = true;
                else if (isEdge)
                    this.DFA.IsEdge = true;
            }
            public bool Equals(List<TState> sequence)
            {
                return this.Count == sequence.Count &&
                       sequence.All(p => this.Contains(p));
            }

        }
    }
}
