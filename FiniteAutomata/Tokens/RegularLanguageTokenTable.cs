using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer._Internal.Inlining;

namespace Oilexer.FiniteAutomata.Tokens
{
    internal partial class RegularLanguageTokenTable :
        FiniteAutomataTransitionTable<RegularLanguageSet, RegularLanguageDFAState, RegularLanguageTokenTable.Target>
    {
        public override void Add(RegularLanguageSet check, Target target)
        {
            IDictionary<RegularLanguageSet, IFiniteAutomataTransitionNode<RegularLanguageSet, Target>> colliders;
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
                var targetSet = new Target(currentNode.Target);
                foreach (var subTarget in target)
                    if (!targetSet.Contains(subTarget))
                        targetSet.Add(subTarget);
                base.AddInternal(intersection, targetSet);
            }
            if (!remainder.IsEmpty)
                base.AddInternal(remainder, new Target(target));
        altSkip: ;
        }

        protected override Target GetStateTarget(RegularLanguageDFAState state)
        {
            if (!(state is RegularLanguageDFARootState))
                throw new ArgumentException("state");
            return new Target { (InlinedTokenEntry)(((RegularLanguageDFARootState)state).Entry) };
        }

        public override IEnumerable<RegularLanguageDFAState> Targets
        {
            get { throw new NotSupportedException(); }
        }
    }
}
