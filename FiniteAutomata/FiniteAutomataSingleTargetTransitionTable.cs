using System;
using System.Collections.Generic;
using System.Text;
/*---------------------------------------------------------------------\
| Copyright © 2009 Allen Copeland Jr.                                  |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */

namespace Oilexer.FiniteAutomata
{
    public class FiniteAutomataSingleTargetTransitionTable<TCheck, TState> :
        FiniteAutomataTransitionTable<TCheck, TState, TState>,
        IFiniteAutomataSingleTargetTransitionTable<TCheck, TState>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IFiniteAutomataState<TCheck, TState>
    {
        public override void Add(TCheck check, TState target)
        {
            IDictionary<TCheck, IFiniteAutomataTransitionNode<TCheck, TState>> colliders;
            var remainder = base.GetColliders(check, out colliders);
            if (colliders.Count > 0)
                throw new ArgumentException("target");
            base.AddInternal(check, target);
        }

        protected override TState GetStateTarget(TState state)
        {
            return state;
        }

        public override IEnumerable<TState> Targets
        {
            get { return this.Values; }
        }

    }
}
