using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using RegularRelationshipSet = System.Collections.Generic.Dictionary<Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageBitArray, Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageBitArray>;
//using Oilexer._Internal.UI.Visualization;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens.StateSystem
{
    partial class RegularLanguageState
    {
        public partial class TransitionTable :
            IEnumerable<RegularLanguageTransitionNode> //:
            //IVisualTransitionTable<RegularLanguageState, RegularLanguageTransitionNode, RegularLanguageBitArray>
        {
            private bool autoMerge;
            public TransitionTable(bool autoMerge)
            {
                this.autoMerge = autoMerge;
            }

            private Dictionary<RegularLanguageBitArray, RegularLanguageTransitionNode> backup = new Dictionary<RegularLanguageBitArray, RegularLanguageTransitionNode>();

            public bool Contains(RegularLanguageBitArray check)
            {
                return this.backup.ContainsKey(check);
            }

            public IEnumerable<RegularLanguageState> this[RegularLanguageBitArray check]
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

            public RegularLanguageTransitionNode GetNode(RegularLanguageBitArray check)
            {
                if (!Contains(check))
                    throw new ArgumentException("check");
                return this.backup[check];
            }

            public void Add(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> targets)
            {
                if (this.autoMerge)
                    this.Push(check, targets);
                else
                    this._Add(check, targets);
            }

            public void Add(RegularLanguageTransitionNode node)
            {
                if (autoMerge)
                    this.Push(node.Check, node.Targets, node.Sources);
                else
                    this._Add(node);
            }

            public void Add(RegularLanguageBitArray check, RegularLanguageState targets)
            {
                if (autoMerge)
                    this.Push(check, targets);
                else
                    this._Add(check, targets);
            }

            public void Add(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> targets, IEnumerable<RegularLanguageTransitionNode.SourceElement> sources)
            {
                if (autoMerge)
                    this.Push(check, targets, sources);
                else
                {
                    var aNode = this._Add(check, targets);
                    foreach (var source in sources)
                        if (!aNode.Sources.Contains(source))
                            aNode.Sources.Add(source);
                }
            }

            public void Add(RegularLanguageBitArray check, RegularLanguageState target, IEnumerable<RegularLanguageTransitionNode.SourceElement> sources)
            {
                if (this.autoMerge)
                    this.Push(check, new RegularLanguageState[] { target }, sources);
                else
                {
                    var aNode = this._Add(check, target);
                    foreach (var source in sources)
                        if (!aNode.Sources.Contains(source))
                            aNode.Sources.Add(source);
                }
            }

            public void Add(RegularLanguageBitArray check, RegularLanguageState target, RegularLanguageTransitionNode.SourceElement source)
            {
                if (this.autoMerge)
                    this.Push(check, new RegularLanguageState[] { target }, new RegularLanguageTransitionNode.SourceElement[] { source });
                else
                {
                    var aNode = this._Add(check, target);
                    if (!aNode.Sources.Contains(source))
                        aNode.Sources.Add(source);
                }
            }

            private void Push(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> targets)
            {
                this.Push(check, targets, null);
            }

            private RegularLanguageTransitionNode _Add(RegularLanguageTransitionNode node)
            {
                if (!this.Contains(node.Check))
                {
                    var newElement = new RegularLanguageTransitionNode(node.Check, node.Targets, node.Sources);
                    this.backup.Add(node.Check, newElement);
                    return newElement;
                }
                var result = this.backup[node.Check];
                foreach (var target in node.Targets)
                    if (!result.Targets.Contains(target))
                        result.Targets.Add(target);
                return result;
            }

            private RegularLanguageTransitionNode _Add(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> targets)
            {
                if (!this.Contains(check))
                {
                    var newElement = new RegularLanguageTransitionNode(check, targets);
                    this.backup.Add(check, newElement);
                    return newElement;
                }
                var result = this.backup[check];
                foreach (var target in targets)
                    if (!result.Targets.Contains(target))
                        result.Targets.Add(target);
                return result;
            }

            private RegularLanguageTransitionNode _Add(RegularLanguageBitArray check, RegularLanguageState target)
            {
                if (!this.Contains(check))
                {
                    var newElement = new RegularLanguageTransitionNode(check, target);
                    this.backup.Add(check, newElement);
                    return newElement;
                }
                var result = this.backup[check];
                if (!result.Targets.Contains(target))
                    result.Targets.Add(target);
                return result;
            }

            public void Remove(RegularLanguageBitArray check)
            {
                if (!this.Contains(check))
                    throw new ArgumentOutOfRangeException("check");
                var removed = this.backup[check];
                this.backup.Remove(check);
                removed.Targets.Clear();
                removed.check = null;
            }

            internal void Remove(RegularLanguageTransitionNode node)
            {
                if (this.backup.ContainsValue(node))
                    this.backup.Remove(node.Check);
            }

            public void Remove(RegularLanguageBitArray check, RegularLanguageState target)
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

            #region IEnumerable<RegularLanguageTransitionNode> Members

            public IEnumerator<RegularLanguageTransitionNode> GetEnumerator()
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
            /// Returns the series of <see cref="RegularLanguageBitArray"/> 
            /// elements which determine the transitional requirements for
            /// the <see cref="TransitionTable"/>.
            /// </summary>
            public IEnumerable<RegularLanguageBitArray> Checks
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
            public IEnumerable<RegularLanguageState[]> TargetSets
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
            /// <see cref="RegularLanguageTransitionNode.SourceElement"/> instances associated
            /// to the <see cref="TransitionTable"/>'s <see cref="RegularLanguageTransitionNode"/>
            /// instances.
            /// </summary>
            public IEnumerable<RegularLanguageTransitionNode.SourceElement> Sources
            {
                get
                {
                    List<RegularLanguageTransitionNode.SourceElement> result = new List<RegularLanguageTransitionNode.SourceElement>();
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
            public IEnumerable<RegularLanguageState> Targets
            {
                get
                {
                    List<RegularLanguageState> result = new List<RegularLanguageState>();
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

            public RegularLanguageBitArray GetCheckRange()
            {
                RegularLanguageBitArray result = null;
                foreach (var item in this.Checks)
                    result |= item;
                return result;
            }

            private void Push(RegularLanguageTransitionNode pushed)
            {
                this.Push(pushed.Check, pushed.Targets, pushed.Sources);
            }

            private void Push(RegularLanguageBitArray check, RegularLanguageState state)
            {
                this.Push(check, new RegularLanguageState[] { state }, null);
            }

            private void Push(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> checkTarget, IEnumerable<RegularLanguageTransitionNode.SourceElement> sources)
            {
                //The areas that need processed.
                List<RegularLanguageBitArray> targets = new List<RegularLanguageBitArray>();
                //The points of overlap between sets.
                RegularRelationshipSet intersections = new RegularRelationshipSet();
                //The complements of the intersections (what's left)
                RegularRelationshipSet complements = new RegularRelationshipSet();
                /* *
                 * The diminishing syntax bit array, basically after
                 * each intersection, it's the complement between
                 * the intersection and the remaining set, which
                 * often zeroes out the larger the overall
                 * transition table grows.
                 * */
                RegularLanguageBitArray diminishingSubset = check;

                foreach (var key in this.Checks.ToArray())
                {
                    var intersection = key & diminishingSubset;
                    if (intersection == null || intersection.AllFalse)
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
                    if (diminishingSubset.AllFalse)
                        //Break if there's nothing left
                        break;
                }
                TransitionTable backup = new TransitionTable(false);
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
                    if (!complement.AllFalse)
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
                if (!diminishingSubset.AllFalse)
                {
                    this._Add(diminishingSubset, checkTarget);
                    if (sources != null)
                        this.GetNode(diminishingSubset).Sources.AddRange(sources);
                }
            }

        }
    }
}
