using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata
{
    public interface IFiniteAutomataSingleTargetTransitionTable<TCheck, TState> :
        IFiniteAutomataTransitionTable<TCheck, TState, TState>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IFiniteAutomataState<TCheck, TState>
    {
    }
}
