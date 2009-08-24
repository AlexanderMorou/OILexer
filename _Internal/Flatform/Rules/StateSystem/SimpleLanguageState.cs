using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using Oilexer.Utilities.Collections;
using Oilexer._Internal.Flatform.Rules;
using System.IO;
using System.CodeDom.Compiler;
using Oilexer.Parser.GDFileData;
using System.Diagnostics;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules.StateSystem
{
    using StateDuplicationTable = Dictionary<SimpleLanguageState, List<SimpleLanguageState>>;
    using ScatteredStateLookupTable = Dictionary<SimpleLanguageState, List<SimpleLanguageBitArray>>;
    //using Oilexer._Internal.UI.Visualization;
    /// <summary>
    /// Provides a simple state class for state transitions from a rule, independent of
    /// look-ahead depth awareness.
    /// </summary>
    internal partial class SimpleLanguageState :
        IEquatable<SimpleLanguageState>//,
        //IVisualState<SimpleLanguageState, SimpleLanguageTransitionNode, SimpleLanguageBitArray>
    {
        #region SimpleLanguageState Fields
        /// <summary>
        /// Data member for <see cref="Transitions"/>.
        /// </summary>
        private TransitionTable outtransitions;
        /// <summary>
        /// Data member for <see cref="InTransitions"/>.
        /// </summary>
        private TransitionTable intransitions;
        private bool isEdge;
        private SimpleLanguageBitArray initialLookahead;
        private int stateValue = -1;

        #region Watches to prevent stack overflows
        private static Dictionary<SimpleLanguageState, SimpleLanguageState> cloneSet = new Dictionary<SimpleLanguageState, SimpleLanguageState>();
        private static List<SimpleLanguageState> enumStack = new List<SimpleLanguageState>();
        private static List<SimpleLanguageTransitionNode> loopbackTrail = new List<SimpleLanguageTransitionNode>();
        private static List<SimpleLanguageTransitionNode> loopbackTrail2 = new List<SimpleLanguageTransitionNode>();
        private static List<SimpleLanguageState> lookaheadFinalSet = new List<SimpleLanguageState>();
        private static List<SimpleLanguageState> countList = new List<SimpleLanguageState>();
        private static List<SimpleLanguageState> ToStringStack = new List<SimpleLanguageState>();
        private static List<SimpleLanguageState> edgeStack = new List<SimpleLanguageState>();

        private static Recursor<string> RecursiveAid;
        private const string GetLookAheadName = "ObtainMergedTransitions";
        #endregion

        #endregion

        #region Static Constructor

        static SimpleLanguageState()
        {
            RecursiveAid = new Recursor<string>();
            PrepareCounterRecursor();
            PrepareEdgeRecursor();
        }

        /* *
         * Just having fun with lambdas.
         * */
        private static void PrepareCounterRecursor()
        {
            int count = 0;
            RecursiveAid.PrepareRecursion<SimpleLanguageState, int>("Counter", state =>
            {
                count++;
                foreach (var transition in state.OutTransitions)
                    RecursiveAid.Recurse<int, SimpleLanguageState>("Counter", transition.Targets);
            }, () =>
            {
                int countBackup = count;
                count = 0;
                return countBackup;
            });
        }


        private static IList<SimpleLanguageState> edgeRecursorSeries = new List<SimpleLanguageState>();

        private static void PrepareEdgeRecursor()
        {
            RecursiveAid.PrepareRecursion<SimpleLanguageState, IEnumerable<SimpleLanguageState>>("ObtainEdges", state =>
                {
                    if (state.IsEdge())
                        edgeRecursorSeries.Add(state);
                    foreach (var transition in state.OutTransitions)
                        RecursiveAid.Recurse<IEnumerable<SimpleLanguageState>, SimpleLanguageState>("ObtainEdges", transition.Targets);
                }, EdgeRecursorTail);
        }

        private static IEnumerable<SimpleLanguageState> EdgeRecursorTail()
        {
            var edgeCopy = new List<SimpleLanguageState>(edgeRecursorSeries);
            edgeRecursorSeries.Clear();
            foreach (var edge in edgeCopy)
                yield return edge;
        }

        #endregion

        #region SimpleLanguageState Properties

        /// <summary>
        /// Returns the outgoing <see cref="TransitionTable"/> for 
        /// the <see cref="SimpleLanguageState"/>.
        /// </summary>
        public TransitionTable OutTransitions
        {
            get
            {
                if (this.outtransitions == null)
                    this.outtransitions = new TransitionTable();
                return this.outtransitions;
            }
        }

        /// <summary>
        /// Returns the incoming <see cref="TransitionTable"/> for 
        /// the <see cref="SimpleLanguageState"/>.
        /// </summary>
        public TransitionTable InTransitions
        {
            get
            {
                if (this.intransitions == null)
                    this.intransitions = new TransitionTable();
                return this.intransitions;
            }
        }

        public SimpleLanguageBitArray MergeSet
        {
            get
            {
                return this.initialLookahead;
            }
        }
        #endregion

        #region SimpleLanguageState Constructors

        /// <summary>
        /// Creates a new <see cref="SimpleLanguageState"/> initialized
        /// to a default state.
        /// </summary>
        public SimpleLanguageState()
        {
        }

        #endregion

        #region Move To Logic

        public void MoveTo(SimpleLanguageBitArray check, SimpleLanguageState target)
        {
            this.OutTransitions.Add(check, target);
            target.InTransitions.Add(check, this);
        }

        internal void MoveTo(SimpleLanguageTransitionNode transition)
        {
            this.MoveTo(transition.Check, transition.Targets, transition.Sources);
        }

        public void MoveTo(SimpleLanguageBitArray check, SimpleLanguageState target, IEnumerable<SimpleLanguageTransitionNode.SourceElement> sources)
        {
            this.OutTransitions.Add(check, target, sources);
            target.InTransitions.Add(check, this, sources);
        }

        public void MoveTo(SimpleLanguageBitArray check, SimpleLanguageState target, SimpleLanguageTransitionNode.SourceElement source)
        {
            this.OutTransitions.Add(check, target, source);
            target.InTransitions.Add(check, this, source);
        }

        public void MoveTo(SimpleLanguageBitArray check, IEnumerable<SimpleLanguageState> targets, IEnumerable<SimpleLanguageTransitionNode.SourceElement> sources)
        {
            this.OutTransitions.Add(check, targets, sources);
            foreach (var target in targets)
                target.InTransitions.Add(check, this, sources);
        }

        public void MoveTo(SimpleLanguageBitArray check, IEnumerable<SimpleLanguageState> targets)
        {
            this.OutTransitions.Add(check, targets);
            foreach (var target in targets)
                target.InTransitions.Add(check, this);
        }

        public void MoveToOptional(SimpleLanguageBitArray check, SimpleLanguageState target, IEnumerable<SimpleLanguageState> edges)
        {
            if (edges == null)
                throw new ArgumentNullException("edges");
            this.MoveTo(check, target);
            foreach (var edge in edges)
                edge.MoveTo(check, target);
        }

        public void MoveToAndStar(SimpleLanguageBitArray check, SimpleLanguageState target, IEnumerable<SimpleLanguageState> edges)
        {
            if (check == null)
                throw new ArgumentNullException("check");
            if (target == null)
                throw new ArgumentNullException("target");
            if (edges == null)
                throw new ArgumentNullException("edgeTarget");
            /* *
             * Move only the edges, the idea here is the
             * current state exists before all the
             * edges, and thus the current state should not
             * move.
             * */
            foreach (var edge in edges)
            {
                foreach (var transition in this.OutTransitions)
                    edge.MoveTo(transition.Check, transition.Targets);
                edge.MoveTo(check, target);
            }
        }

        public void MoveToStar(SimpleLanguageBitArray check, SimpleLanguageState target, IEnumerable<SimpleLanguageState> edges)
        {
            this.MoveToAndStar(check, target, edges);
            this.MoveTo(check, target);
        }

        internal void RequiredStar()
        {
            var edges = this.ObtainEdges();
            foreach (var edge in edges)
            {
                edge.MakeEdge();
                foreach (var transition in this.OutTransitions.ToArray())
                    edge.MoveTo(transition.Check, transition.Targets);
            }
        }

        public void Star()
        {
            this.RequiredStar();
            this.MakeEdge();
        }
        #endregion

        /// <summary>
        /// Clones the current <see cref="SimpleLanguageState"/>.
        /// </summary>
        /// <returns>A new, exact copy, of the <see cref="SimpleLanguageState"/>.</returns>
        /// <remarks>Not a shallow copy.</remarks>
        public SimpleLanguageState Clone()
        {
            if (cloneSet.ContainsKey(this))
                return cloneSet[this];
            var result = this.GetNewInstance();
            result.initialLookahead = this.initialLookahead;
            cloneSet.Add(this, result);
            foreach (var transition in this.OutTransitions)
            {
                result.MoveTo(transition.Check, transition.Targets.OnAll(p =>
                {
                    var r = p.Clone();
                    if (p.IsMarked)
                        r.MakeEdge();
                    return r;
                }));
                result.OutTransitions.GetNode(transition.Check).Sources.AddRange(transition.Sources);
            }

            if (cloneSet.ElementAt(0).Key == this)
                cloneSet.Clear();
            return result;
        }

        /// <summary>
        /// Creates a new instance of the state internally for
        /// derivation purposes.
        /// </summary>
        internal virtual SimpleLanguageState GetNewInstance()
        {
            return new SimpleLanguageState();
        }

        public static SimpleLanguageState operator |(SimpleLanguageState left, SimpleLanguageState right)
        {
            var result = left.Clone();
            var rRight = right.Clone();
            if (right.IsMarked || left.IsMarked)
                result.MakeEdge();
            foreach (var edge in rRight.ObtainEdges())
                edge.MakeEdge();
            foreach (var transition in rRight.OutTransitions)
            {
                result.MoveTo(transition.Check, transition.Targets, transition.Sources);
                /* *
                 * The right node was cloned so the original isn't tampered with.
                 * *
                 * At the same time, it's not necessary for the clone to exist
                 * in the final result.
                 * */
                foreach (var target in transition.Targets)
                    target.InTransitions.Remove(transition.Check, rRight);

            }
            return result;
        }

        internal virtual void MakeEdge()
        {
            this.isEdge = true;
        }

        /// <summary>
        /// Obtains the edges for the current <see cref="SimpleLanguageState"/>.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> which iterates the 
        /// edges of the current <see cref="SimpleLanguageState"/>.</returns>
        public IEnumerable<SimpleLanguageState> ObtainEdges()
        {
            //var edges = ObtainEdges(this);
            //var dEdges = SimpleLanguageState.RecursiveAid.RecurseSingle<IEnumerable<SimpleLanguageState>, SimpleLanguageState>("ObtainEdges", this);
            //Debug.Assert(edges.SequenceEqual(dEdges));
            foreach (var item in ObtainEdges(this))//SimpleLanguageState.RecursiveAid.RecurseSingle<IEnumerable<SimpleLanguageState>, SimpleLanguageState>("ObtainEdges", this))
                yield return item;
        }

        private static IList<SimpleLanguageState> ObtainEdges(SimpleLanguageState source)
        {
            IList<SimpleLanguageState> result = new List<SimpleLanguageState>();
            ObtainEdges(source, result);
            return result;
        }

        /// <summary>
        /// Obtains the edges for a given <paramref name="source"/> <see cref="SimpleLanguageState"/>.
        /// </summary>
        /// <param name="source">The <see cref="SimpleLanguageState"/> from which to start
        /// edge scanning at.</param>
        /// <param name="target">The <see cref="IList{T}"/> to receive the edges.</param>
        private static void ObtainEdges(SimpleLanguageState source, IList<SimpleLanguageState> target)
        {
            if (edgeStack.Contains(source))
                return;
            edgeStack.Add(source);
            if (source.IsEdge())
                target.Add(source);
            foreach (var transition in source.OutTransitions)
                foreach (var transitionState in transition.Targets)
                    ObtainEdges(transitionState, target);
            if (edgeStack[0] == source)
                edgeStack.Clear();
        }

        /// <summary>
        /// Clears the previously marked 'terminal' status on a non-terminal edge.
        /// </summary>
        internal void ClearEdge()
        {
            if (this.IsMarked)
                this.isEdge = false;
        }

        /// <summary>
        /// Returns whether the <see cref="SimpleLanguageState"/> is marked as an edge
        /// even considering it has transitions going past it.
        /// </summary>
        /// <remarks>Used to represent non-terminal edges.</remarks>
        public bool IsMarked { get { return this.isEdge; } }

        /// <summary>
        /// Returns whether the current <see cref="SimpleLanguageState"/> is an edge.
        /// </summary>
        /// <returns>true if the current <see cref="SimpleLanguageState"/> is an edge; false
        /// otherwise.</returns>
        /// <remarks>If the number of transitions from a <see cref="SimpleLanguageState"/>
        /// is zero, returns true; however also true, if <see cref="IsMarked"/> is true;
        /// false otherwise.</remarks>
        internal bool IsEdge()
        {
            if (this.isEdge)
                return true;
            if (this.OutTransitions.Count() == 0)
                return true;
            return false;
        }

        private static List<SimpleLanguageTransitionNode> GetRuleList = new List<SimpleLanguageTransitionNode>();
        internal virtual SimpleLanguageRuleState GetRule()
        {
            bool startedRuleList = GetRuleList.Count == 0;
            if (this is SimpleLanguageRuleState)
                return (SimpleLanguageRuleState)this;
            try
            {
                foreach (var transition in this.InTransitions)
                {
                    if (GetRuleList.Contains(transition))
                        continue;
                    GetRuleList.Add(transition);
                    SimpleLanguageRuleState result = null;
                    foreach (var target in transition.Targets)
                    {
                        result = target.GetRule();
                        if (result != null)
                            return result;
                    }
                }
                return null;
            }
            finally
            {
                if (startedRuleList)
                    GetRuleList.Clear();
            }
        }

        public override bool Equals(object obj)
        {
            return object.ReferenceEquals(obj, this);
        }

        public override string ToString()
        {
            bool firstOnStack = ToStringStack.Count == 0;
            if (ToStringStack.Contains(this))
                return string.Format("* ({0})", this.StateValue);
            ToStringStack.Add(this);
            try
            {
                MemoryStream ms = new MemoryStream();
                TextWriter tw = new StreamWriter(ms);
                IndentedTextWriter itw = new IndentedTextWriter(tw);
                itw.Indent++;
                if (this.OutTransitions != null && this.OutTransitions.Count > 0)
                {
                    foreach (var item in this.OutTransitions)
                    {
                        foreach (var subItem in item.Targets)
                        {
                            string current = subItem.ToString();
                            string[] currentLines = current.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            itw.Write("{{{0}}}",item.Check);
                            for (int i = 0; i < currentLines.Length; i++)
                            {
                                string line = currentLines[i];
                                itw.Write("->{0}", subItem.StateValue);
                                itw.Write(line.TrimStart());
                                itw.WriteLine();
                            }
                        }
                    }
                }
                else
                {
                    itw.Write("<END>");
                    itw.WriteLine();
                }
                itw.Indent--;
                itw.Flush();
                TextReader sr = new StreamReader(ms);
                ms.Seek(0, SeekOrigin.Begin);
                string result = "   " + sr.ReadToEnd();
                itw.Close();
                tw.Close();
                sr.Close();
                sr.Dispose();
                tw.Dispose();
                ms.Close();
                ms.Dispose();
                itw.Dispose();
                if (firstOnStack)
                    result = string.Format("SimpleState Count: {0}\r\n {{{1}}}", this.Count().ToString(), this.stateValue) + "\r\n" + result;
                return result;
            }
            finally
            {
                if (firstOnStack)
                    ToStringStack.Clear();
            }
        }

        /// <summary>
        /// Returns the number of states including the current as well as all
        /// transitions contained thereafter.
        /// </summary>
        /// <returns>An <see cref="Int32"/> value representing the number
        /// of states from the current State.</returns>
        public int Count()
        {
            if (countList.Contains(this))
                return 0;
            countList.Add(this);
            int result = 1;
            foreach (var transition in this.OutTransitions)
                foreach (var state in transition.Targets)
                    result += state.Count();
            if (countList[0] == this)
                countList.Clear();
            return result;
        }

        /// <summary>
        /// Returns the <see cref="Int32"/> value representing the unique value of the
        /// state.
        /// </summary>
        public int StateValue
        {
            get
            {
                return this.stateValue;
            }
        }

        #region SimpleState indexing
        /// <summary>
        /// Iterates and assigns all elements within the state transition scope their uniqueness value.
        /// </summary>
        internal void Enumerate()
        {
            int state = 0;
            this.EnumerateSet(ref state);
            this.Enumerate(ref state);
            var edges = this.ObtainEdges();
            foreach (var edge in edges)
            {
                if (edge.stateValue == -1)
                    edge.stateValue = state++;
            }
        }

        /// <summary>
        /// Iterates and assigns all elements within the state transition scope their uniqueness value
        /// continuing from the <paramref name="stateValue"/> provided.
        /// </summary>
        /// <param name="stateValue">The most recently calculated state value.</param>
        internal void EnumerateSet(ref int stateValue)
        {
            if (enumStack.Contains(this))
                return;
            //Non-terminals only.
            if (this.OutTransitions.Count > 0 && this.stateValue == -1)
                this.stateValue = stateValue++;
        }

        internal void Enumerate(ref int stateValue)
        {
            if (enumStack.Contains(this))
                return;
            enumStack.Add(this);
            if (this.OutTransitions.Count > 0)
            {
                /* *
                 * Index siblings relative to one another,
                 * then have them recurse their elements.
                 * */
                foreach (var item in this.OutTransitions.Targets)
                    item.EnumerateSet(ref stateValue);
                foreach (var item in this.OutTransitions.Targets)
                    item.Enumerate(ref stateValue);
            }
            if (enumStack[0] == this)
                enumStack.Clear();
        }


        #endregion

        internal List<SimpleLanguageState> GetFlatform()
        {
            List<SimpleLanguageState> flatform = new List<SimpleLanguageState>();
            Merger.FlatlineState(this, flatform);
            return flatform;
        }

        private static List<SimpleLanguageState> ObtainStateSetEdges(IProductionRuleEntry[] set, Dictionary<IProductionRuleEntry, FlattenedRuleEntry> lookup)
        {
            List<SimpleLanguageState> result = new List<SimpleLanguageState>();
            List<SimpleLanguageState> currentSet = new List<SimpleLanguageState>();
            foreach (var rule in set)
            {
                var ruleData = lookup[rule];
                var state = ruleData.SimpleState;
                ObtainEdges(state, currentSet);
                result.AddRange(currentSet);
                currentSet.Clear();
            }
            return result;
        }

        private TransitionTable mergedTransitions = null;
        internal TransitionTable ObtainMergedTransitions(FlattenedRuleEntry entryData)
        {
            if (mergedTransitions != null)
                return mergedTransitions;
            TransitionTable tT = mergedTransitions = new TransitionTable();
            int count = 0;
            int t = 0;
            do
            {
                count = tT.Count;
                this.ObtainMergedTransitionsInternal(entryData, tT);
                loopbackTrail2.Clear();
                t++;
            } while (tT.Count != count);
            return tT;
        }

        private void ObtainMergedTransitionsInternal(FlattenedRuleEntry entryData, TransitionTable tT)
        {
            List<SimpleLanguageTransitionNode> loopbackAgain = new List<SimpleLanguageTransitionNode>();
            foreach (var transition in this.OutTransitions)
            {
                if (!loopbackTrail2.Contains(transition))
                {
                    loopbackAgain.Add(transition);
                    loopbackTrail2.Add(transition);
                }
                else
                    continue;
                tT.Add(transition.Check, transition.Targets, transition.Sources);
                foreach (var rule in transition.Check.GetRuleRange())
                {
                    entryData.ruleLookup[rule].SimpleState.ObtainMergedTransitionsInternal(entryData, tT);
                }
            }
        }


        #region IEquatable<SimpleLanguageState> Members

        public bool Equals(SimpleLanguageState other)
        {
            return object.ReferenceEquals(other, this);
        }

        #endregion

        /// <summary>
        /// Retursn an <see cref="Int32"/> value representing a value 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int result = 0;
            if (this.OutTransitions.Count > 0)
                result = this.OutTransitions.GetCheckRange().GetHashCode();
            if (this.InTransitions.Count > 0)
                result ^= this.InTransitions.GetCheckRange().GetHashCode();
            result ^= this.StateValue;
            return result;
        }

        internal IEnumerable<SimpleLanguageTransitionNode> ObtainShortestRouteToRule(SimpleLanguageRuleState ruleReference)
        {
            List<SimpleLanguageTransitionNode> result = new List<SimpleLanguageTransitionNode>();
            ObtainShortestRouteToRule(ruleReference, ref result, this);
            return ((IEnumerable<SimpleLanguageTransitionNode>)(result)).Reverse();
        }

        private static List<SimpleLanguageState> routeTracker = new List<SimpleLanguageState>();
        private static bool ObtainShortestRouteToRule(SimpleLanguageRuleState target, ref  List<SimpleLanguageTransitionNode> resultRoute, SimpleLanguageState currentPoint)
        {
            if (routeTracker.Contains(currentPoint))
                return false;
            if (currentPoint == target)
                return true;
            routeTracker.Add(currentPoint);
            //The full list of paths found.
            List<List<SimpleLanguageTransitionNode>> foundSets = new List<List<SimpleLanguageTransitionNode>>();
            /* *
             * The current path being explored, duplicated from the resultant route as it is
             * upon entering ObtainShortestRouteToRule.
             * */
            List<SimpleLanguageTransitionNode> currentSet = new List<SimpleLanguageTransitionNode>(resultRoute);
            /* *
             * Recursively backtrack until a path to the rule for the series is found.
             * */
            foreach (var transition in currentPoint.InTransitions)
            {
                currentSet.Add(transition);
                var currentSetCopy = new List<SimpleLanguageTransitionNode>(currentSet);
                /* *
                 * When a match is found, returning it isn't priority immediately,
                 * this is the shortest route, so watch the path and continue.
                 * */
                foreach (var transitionTarget in transition.Targets)
                    if (ObtainShortestRouteToRule(target, ref currentSet, transitionTarget))
                    {
                        foundSets.Add(currentSet);
                        currentSet = currentSetCopy;
                    }
                currentSet.Remove(transition);
            }
            if (routeTracker[0] == currentPoint)
                routeTracker.Clear();
            /* *
             * Iterate the found paths, return the one that has the least transition nodes.
             * */
            if (foundSets.Count > 0)
            {
                List<SimpleLanguageTransitionNode> smallest = null;
                foreach (var list in foundSets)
                    if (list == null)
                        continue;
                    else if (smallest == null)
                        smallest = list;
                    else if (smallest.Count > list.Count)
                        smallest = list;
                resultRoute = smallest;
                return true;
            }
            else
                return false;
        }

        #region IVisualState<SimpleLanguageState,SimpleLanguageTransitionNode,SimpleLanguageBitArray> Members

        //IVisualTransitionTable<SimpleLanguageState, SimpleLanguageTransitionNode, SimpleLanguageBitArray> IVisualState<SimpleLanguageState, SimpleLanguageTransitionNode, SimpleLanguageBitArray>.OutTransitions
        //{
        //    get { return this.OutTransitions; }
        //}

        //IVisualTransitionTable<SimpleLanguageState, SimpleLanguageTransitionNode, SimpleLanguageBitArray> IVisualState<SimpleLanguageState, SimpleLanguageTransitionNode, SimpleLanguageBitArray>.InTransitions
        //{
        //    get { return this.InTransitions; }
        //}

        //bool IVisualState<SimpleLanguageState, SimpleLanguageTransitionNode, SimpleLanguageBitArray>.IsEdge
        //{
        //    get { return this.IsEdge(); }
        //}

        public bool IsInitialState
        {
            get { return this.StateValue == 0; }
        }

        #endregion
    }
}
