using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Parser.GDFileData.TokenExpression;
using System.Diagnostics;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens.StateSystem
{
    partial class RegularLanguageState
    {
        public class ForkTransitions
        {
            public IEnumerable<ForkTransition> GetEnumerator()
            {
                foreach (var fork in data)
                    yield return fork;
                yield break;
            }

            #region Fork Gathering

            public static ForkTransitions GetForkTransitionGraph(RegularLanguageState initialState, List<ITokenItem> deterministicAspect)
            {
                ForkTransitions result = new ForkTransitions();
                int forkInitial = 0;
                GetForkTransitionGraph(initialState, deterministicAspect, result, ref forkInitial);
                return result;
            }
            public static void GetForkTransitionGraph(RegularLanguageState target, List<ITokenItem> deterministicAspect, ForkTransitions destination, ref int forkValue)
            {
                GetForkTransitionGraph(target, deterministicAspect, destination, ref forkValue, null, null);
            }

            public static void GetForkTransitionGraph(RegularLanguageState target, List<ITokenItem> deterministicAspect, ForkTransitions destination, ref int forkValue, RegularLanguageBitArray currentTransitionKey, RegularLanguageState originalTarget)
            {
                /* *
                 * Added this fix because left-consumed elements don't mix well with the sources
                 * for right-consumed elements.
                 * */
                if (target.OutTransitions != null && target.OutTransitions.Count > 0)
                    foreach (var transition in target.OutTransitions)
                        foreach (var state in transition.Targets)
                            GetForkTransitionGraph(state, deterministicAspect, destination, ref forkValue, transition.Check, target);

                foreach (var source in target.sources)
                    if (!destination.ContainsFork(source))
                        destination.AddTransition(source, target);
                    else
                    {
                        var cTrans = destination.GetTransitionFor(source);
                        if (!cTrans.TargetStates.Contains(target))
                            cTrans.TargetStates.Add(target);
                    }
            }

            #endregion

            private List<ForkTransition> data = new List<ForkTransition>();

            public bool ContainsFork(ITokenItem target)
            {
                foreach (var transition in data)
                    if (transition.Target == target)
                        return true;
                return false;
            }

            public void AddForkFor(ITokenItem target, RegularLanguageState targetState)
            {
                var transition = GetTransitionFor(target);
                transition.TargetStates.Add(targetState);
            }

            public ForkTransition GetTransitionFor(ITokenItem target)
            {
                foreach (var transition in data)
                    if (transition.Target == target)
                        return transition;
                return null;
            }

            public ForkTransition AddTransition(ITokenItem target, RegularLanguageState forkingPoint)
            {
                if (this.ContainsFork(target))
                    return GetTransitionFor(target);
                ForkTransition result = new ForkTransition(target, forkingPoint);
                this.data.Add(result);
                return result;
            }
        }
        public class StateTransitionSet : Dictionary<RegularLanguageState, RegularLanguageBitArray> { }
        public class ForkTransition
        {
            public ITokenItem Target { get; private set; }
            public List<RegularLanguageState> TargetStates { get; private set; }
            public ForkTransition(ITokenItem source, RegularLanguageState targetState)
            {
                this.Target = source;
                this.TargetStates = new List<RegularLanguageState>(new RegularLanguageState[] { targetState });
            }

            public override string ToString()
            {
                return this.Target.ToString();
            }
        }
    }
}
