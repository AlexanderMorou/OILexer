using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
/*---------------------------------------------------------------------\
| Copyright © 2009 Allen Copeland Jr.                                  |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */

namespace Oilexer.FiniteAutomata
{
    public class FiniteAutomataMultiTargetTransitionTable<TCheck, TState> :
        FiniteAutomataTransitionTable<TCheck, TState, List<TState>>,
        IFiniteAutomataMultiTargetTransitionTable<TCheck, TState>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            IFiniteAutomataState<TCheck, TState>
    {
        private bool autoSegment;

        public FiniteAutomataMultiTargetTransitionTable()
            : this(true)
        {
        }

        public FiniteAutomataMultiTargetTransitionTable(bool autoSegment)
        {
            this.autoSegment = autoSegment;
        }

        public override void Add(TCheck check, List<TState> target)
        {
            if (autoSegment)
            {
                IDictionary<TCheck, IFiniteAutomataTransitionNode<TCheck, List<TState>>> colliders;
                var remainder = base.GetColliders(check, out colliders);
                if (colliders.Count == 1 && remainder.IsEmpty)
                {
                    var first = colliders.First();
                    if (!base.ContainsKey(first.Key))
                        goto alternate;
                    var currentNode = base.GetNode(check);
                    foreach (var state in target)
                    {
                        if (!currentNode.Target.Contains(state))
                            currentNode.Target.Add(state);
                    }
                    goto altSkip;
                }
            alternate:
                foreach (var intersection in colliders.Keys)
                {
                    var currentNode = colliders[intersection];
                    base.Remove(currentNode.Check);
                    var nodeRemainder = currentNode.Check.SymmetricDifference(intersection);
                    if (!nodeRemainder.IsEmpty)
                        base.AddInternal(nodeRemainder, currentNode.Target);
                    List<TState> targetSet = new List<TState>(currentNode.Target);
                    foreach (var subTarget in target)
                        if (!targetSet.Contains(subTarget))
                            targetSet.Add(subTarget);
                    base.AddInternal(intersection, targetSet);
                }
                if (!remainder.IsEmpty)
                    base.AddInternal(remainder, new List<TState>(target));
            altSkip: ;
            }
            else
                if (base.ContainsKey(check))
                {
                    var currentNode = base.GetNode(check);
                    foreach (var state in target)
                        if (!currentNode.Target.Contains(state))
                            currentNode.Target.Add(state);
                }
                else
                    base.AddInternal(check, target);
        }

        protected override List<TState> GetStateTarget(TState state)
        {
            return new List<TState>() { state };
        }

        public override IEnumerable<TState> Targets
        {
            get {
                HashSet<TState> targetsPassed = new HashSet<TState>();
                foreach (var set in this.Values)
                    foreach (var state in set)
                    {
                        if (!targetsPassed.Contains(state))
                        {
                            targetsPassed.Add(state);
                            yield return state;
                        }
                    }
            }
        }

        /// <summary>
        /// Removes a state by the given target.
        /// </summary>
        /// <param name="check">The <typeparamref name="TCheck"/>
        /// denoting the condition for the transition.</param>
        /// <param name="target">The <typeparamref name="TState"/>
        /// to remove from the set.</param>
        public void Remove(TCheck check, TState target)
        {
            if (!this.ContainsKey(check))
                throw new KeyNotFoundException();
            var currentNode = base.GetNode(check);
            if (currentNode.Target.Count > 1)
                if (!currentNode.Target.Contains(target))
                    throw new ArgumentOutOfRangeException("target");
                else
                    currentNode.Target.Remove(target);
            else
                base.Remove(check);
        }
    }
}
