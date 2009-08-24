using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Diagnostics;
using SyntaxRelationshipSet = System.Collections.Generic.Dictionary<Oilexer._Internal.Flatform.Rules.StateSystem.SimpleLanguageBitArray, Oilexer._Internal.Flatform.Rules.StateSystem.SimpleLanguageBitArray>;
//using Oilexer._Internal.UI.Visualization;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules.StateSystem
{
    partial class SimpleLanguageState
    {
        public partial class TransitionTable :
            IEnumerable<SimpleLanguageTransitionNode>//,
            //IVisualTransitionTable<SimpleLanguageState, SimpleLanguageTransitionNode, SimpleLanguageBitArray>
        {

            private Dictionary<SimpleLanguageBitArray, SimpleLanguageTransitionNode> backup = new Dictionary<SimpleLanguageBitArray, SimpleLanguageTransitionNode>();

            public bool Contains(SimpleLanguageBitArray check)
            {
                return this.backup.ContainsKey(check);
            }

            public IEnumerable<SimpleLanguageState> this[SimpleLanguageBitArray check]
            {
                get
                {
                    if (!this.Contains(check))
                        throw new ArgumentOutOfRangeException("check");
                    var currentTransition = this.backup[check];
                    for (int i = 0; i < currentTransition.Targets.Count; i++)
                        yield return currentTransition.Targets[i];
                    yield break;
                }
            }

            public SimpleLanguageTransitionNode GetNode(SimpleLanguageBitArray check)
            {
                if (!Contains(check))
                    throw new ArgumentException("check");
                return this.backup[check];
            }

            public void Add(SimpleLanguageBitArray check, IEnumerable<SimpleLanguageState> targets)
            {
                this.Push(check, targets);
            }

            public void Add(SimpleLanguageTransitionNode checkTargetsSources)
            {
                this.Push(checkTargetsSources.Check, checkTargetsSources.Targets, checkTargetsSources.Sources);
            }

            public void Add(SimpleLanguageBitArray check, SimpleLanguageState targets)
            {
                this.Push(check, targets);
            }

            public void Add(SimpleLanguageBitArray check, IEnumerable<SimpleLanguageState> targets, IEnumerable<SimpleLanguageTransitionNode.SourceElement> sources)
            {
                this.Push(check, targets, sources);
            }

            public void Add(SimpleLanguageBitArray check, SimpleLanguageState target, IEnumerable<SimpleLanguageTransitionNode.SourceElement> sources)
            {
                this.Push(check, new SimpleLanguageState[] { target }, sources);
            }

            public void Add(SimpleLanguageBitArray check, SimpleLanguageState target, SimpleLanguageTransitionNode.SourceElement source)
            {
                this.Push(check, new SimpleLanguageState[] { target }, new SimpleLanguageTransitionNode.SourceElement[] { source });
            }

            private void Push(SimpleLanguageBitArray check, IEnumerable<SimpleLanguageState> targets)
            {
                this.Push(check, targets, null);
            }

            private SimpleLanguageTransitionNode _Add(SimpleLanguageBitArray check, IEnumerable<SimpleLanguageState> targets)
            {
                if (!this.Contains(check))
                {
                    var newElement = new SimpleLanguageTransitionNode(check, targets);
                    this.backup.Add(check, newElement);
                    return newElement;
                }
                var result = this.backup[check];
                foreach (var target in targets)
                    if (!result.Targets.Contains(target))
                        result.Targets.Add(target);
                return result;
            }

            private SimpleLanguageTransitionNode _Add(SimpleLanguageBitArray check, SimpleLanguageState target)
            {
                if (!this.Contains(check))
                {
                    var newElement = new SimpleLanguageTransitionNode(check, target);
                    this.backup.Add(check, newElement);
                    return newElement;
                }
                var result = this.backup[check];
                if (!result.Targets.Contains(target))
                    result.Targets.Add(target);
                return result;
            }

            public void Remove(SimpleLanguageBitArray check)
            {
                if (!this.Contains(check))
                    throw new ArgumentOutOfRangeException("check");
                var removed = this.backup[check];
                this.backup.Remove(check);
                removed.Targets.Clear();
                removed.Check = null;
            }

            internal void Remove(SimpleLanguageTransitionNode node)
            {
                if (this.backup.ContainsValue(node))
                    this.backup.Remove(node.Check);
            }

            public void Remove(SimpleLanguageBitArray check, SimpleLanguageState target)
            {
                if (!this.Contains(check))
                    throw new ArgumentOutOfRangeException("check");
                var removeTarget = this.backup[check];
                if (!removeTarget.Targets.Contains(target))
                    throw new ArgumentOutOfRangeException("target");
                removeTarget.Targets.Remove(target);
                if (removeTarget.Targets.Count == 0)
                    this.Remove(check);
            }

            #region IEnumerable<SimpleLanguageTransitionNode> Members

            public IEnumerator<SimpleLanguageTransitionNode> GetEnumerator()
            {
                foreach (var item in backup)
                    yield return item.Value;
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            /// <summary>
            /// Returns the series of <see cref="SimpleLanguageBitArray"/> 
            /// elements which determine the transitional requirements for
            /// the <see cref="TransitionTable"/>.
            /// </summary>
            public IEnumerable<SimpleLanguageBitArray> Checks
            {
                get
                {
                    return this.backup.Keys;
                }
            }

            /// <summary>
            /// Returns the <see cref="IEnumerable{T}"/> of 
            /// target sets, independent of the transition used 
            /// to reach them.
            /// </summary>
            public IEnumerable<SimpleLanguageState[]> TargetSets
            {
                get
                {
                    foreach (var item in backup.Values)
                    {
                        yield return item.Targets.ToArray();
                    }
                }
            }

            /// <summary>
            /// Returns the <see cref="IEnumerable{T}"/> of the
            /// <see cref="SimpleLanguageTransitionNode.SourceElement"/> instances associated
            /// to the <see cref="TransitionTable"/>'s <see cref="SimpleLanguageTransitionNode"/>
            /// instances.
            /// </summary>
            public IEnumerable<SimpleLanguageTransitionNode.SourceElement> Sources
            {
                get
                {
                    List<SimpleLanguageTransitionNode.SourceElement> result = new List<SimpleLanguageTransitionNode.SourceElement>();
                    foreach (var kvp in backup)
                        result.AddRange(kvp.Value.Sources.ToArray());
                    foreach (var resultItem in result.Distinct())
                        yield return resultItem;
                }
            }

            /// <summary>
            /// Returns the <see cref="IEnumerable{T}"/> instance
            /// capable of iterating all of the transition element 
            /// target states, regardless of the context used to 
            /// reach said state(s).
            /// </summary>
            public IEnumerable<SimpleLanguageState> Targets
            {
                get
                {
                    List<SimpleLanguageState> result = new List<SimpleLanguageState>();
                    foreach (var kvp in backup)
                        result.AddRange(kvp.Value.Targets.ToArray());
                    foreach (var resultItem in result.Distinct())
                        yield return resultItem;
                }
            }

            /// <summary>
            /// Returns the number of elements in the <see cref="TransitionTable"/>.
            /// </summary>
            public int Count
            {
                get
                {
                    return this.backup.Count;
                }
            }

            public SimpleLanguageBitArray GetCheckRange()
            {
                SimpleLanguageBitArray result = null;
                foreach (var item in this.Checks)
                    result |= item;
                return result;
            }

            private void Push(SimpleLanguageTransitionNode pushed)
            {
                this.Push(pushed.Check, pushed.Targets, pushed.Sources);
            }

            private void Push(SimpleLanguageBitArray check, SimpleLanguageState state)
            {
                this.Push(check, new SimpleLanguageState[] { state }, null);
            }

            private void Push(SimpleLanguageBitArray check, IEnumerable<SimpleLanguageState> checkTarget, IEnumerable<SimpleLanguageTransitionNode.SourceElement> sources)
            {
                //The areas that need processed.
                List<SimpleLanguageBitArray> targets = new List<SimpleLanguageBitArray>();
                //The points of overlap between sets.
                SyntaxRelationshipSet intersections = new SyntaxRelationshipSet();
                //The complements of the intersections (what's left)
                SyntaxRelationshipSet complements = new SyntaxRelationshipSet();
                /* *
                 * The diminishing syntax bit array, basically after
                 * each intersection, it's the complement between
                 * the intersection and the remaining set, which
                 * often zeroes out the larger the overall
                 * transition table grows.
                 * */
                SimpleLanguageBitArray diminishingSubset = check;

                foreach (var key in this.Checks.ToArray())
                {
                    var intersection = key & diminishingSubset;
                    if (intersection == null || intersection.IsNullSet)
                        //No intersection existed.
                        continue;
                    //Where they overlap
                    intersections.Add(key, intersection);
                    //What's left of the overlap.
                    complements.Add(key, key ^ intersection);
                    //Subtract the intersecting point.
                    diminishingSubset ^= intersection;
                    //Defines which sectors need truncated.
                    targets.Add(key);
                    if (diminishingSubset.IsNullSet)
                        //Break if there's nothing left
                        break;
                }
                TransitionTable backup = new TransitionTable();
                //Create a backup of the overlapped areas.
                foreach (var item in targets)
                    backup._Add(item, this[item]);
                foreach (var target in targets)
                {
                    var intersection = intersections[target];
                    var complement = complements[target];
                    this.Remove(target);
                    /* *
                     * _Add the intersection to the transitions, using the backup 
                     * to reissue the segmented elements.
                     * */
                    this._Add(intersection, backup[target]);
                    this._Add(intersection, checkTarget);
                    var backupNode = backup.GetNode(target);
                    var currentNode = this.GetNode(intersection);
                    foreach (var source in backupNode.Sources)
                        if (!currentNode.Sources.Contains(source))
                            currentNode.Sources.Add(source);
                    /* _Add the current state set to the intersection point. */
                    if (sources != null)
                        foreach (var source in sources)
                            if (!currentNode.Sources.Contains(source))
                                currentNode.Sources.Add(source);
                    /* *
                     * On the area that didn't overlap, remember to reissue the
                     * elements that were segmented back to the complimentary (non-overlapped)
                     * area.
                     * */
                    if (!complement.IsNullSet)
                    {
                        this._Add(complement, backup[target]);
                        currentNode = this.GetNode(complement);
                        foreach (var source in backup.GetNode(target).Sources)
                            if (!currentNode.Sources.Contains(source))
                                currentNode.Sources.Add(source);
                    }
                }
                /* *
                 * If either nothing overlapped, or there was a remainder,
                 * issue it to the transition table.
                 * */
                if (!diminishingSubset.IsNullSet)
                {
                    this._Add(diminishingSubset, checkTarget);
                    if (sources != null)
                        this.GetNode(diminishingSubset).Sources.AddRange(sources);
                }
            }

        }
    }
}
