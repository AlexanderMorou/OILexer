using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using Oilexer.Utilities.Collections;
using Oilexer.Parser.GDFileData.TokenExpression;
//using Oilexer._Internal.UI.Visualization;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens.StateSystem
{
    internal partial class RegularLanguageState :
        IEquatable<RegularLanguageState>//,
        //IVisualState<RegularLanguageState, RegularLanguageTransitionNode, RegularLanguageBitArray>
    {
        private List<ITokenItem> sources = new List<ITokenItem>();
        private bool isEdge;

        /// <summary>
        /// Returns the <see cref="Int32"/> value representing the unique value of the
        /// state.
        /// </summary>
        public int StateValue { get; private set; }

        /// <summary>
        /// Returns the <see cref="TransitionTable"/> which denotes the set
        /// of transitions which leave the current state.
        /// </summary>
        public TransitionTable OutTransitions { get; private set; }

        /// <summary>
        /// Returns the <see cref="TransitionTable"/> which denotes
        /// the set of transitions which enter the current state.
        /// </summary>
        public TransitionTable InTransitions { get; private set; }

        public IEnumerable<ITokenItem> Sources
        {
            get
            {
                if (sources == null ||
                    sources.Count == 0)
                    yield break;
                foreach (var item in this.sources)
                    yield return item;
            }
        }

        #region Watches to prevent stack overflows
        private static Dictionary<RegularLanguageState, RegularLanguageState> cloneSet = new Dictionary<RegularLanguageState, RegularLanguageState>();
        private static List<RegularLanguageState> enumStack = new List<RegularLanguageState>();
        private static List<RegularLanguageTransitionNode> loopbackTrail = new List<RegularLanguageTransitionNode>();
        private static List<RegularLanguageTransitionNode> loopbackTrail2 = new List<RegularLanguageTransitionNode>();
        private static List<RegularLanguageState> lookaheadFinalSet = new List<RegularLanguageState>();
        private static List<RegularLanguageState> countList = new List<RegularLanguageState>();
        private static List<RegularLanguageState> ToStringStack = new List<RegularLanguageState>();
        private static List<RegularLanguageState> edgeStack = new List<RegularLanguageState>();

        #endregion

        public RegularLanguageState()
        {
            this.OutTransitions = new TransitionTable(true);
            this.InTransitions = new TransitionTable(false);
            this.StateValue = -1;
        }

        #region Move To Logic

        public void MoveTo(RegularLanguageBitArray check, RegularLanguageState target)
        {
            this.OutTransitions.Add(check, target);
            target.InTransitions.Add(check, this);
        }

        internal void MoveTo(RegularLanguageTransitionNode transition)
        {
            this.MoveTo(transition.Check, transition.Targets, transition.Sources);
        }

        public void MoveTo(RegularLanguageBitArray check, RegularLanguageState target, IEnumerable<RegularLanguageTransitionNode.SourceElement> sources)
        {
            this.OutTransitions.Add(check, target, sources);
            target.InTransitions.Add(check, this, sources);
        }

        public void MoveTo(RegularLanguageBitArray check, RegularLanguageState target, RegularLanguageTransitionNode.SourceElement source)
        {
            this.OutTransitions.Add(check, target, source);
            target.InTransitions.Add(check, this, source);
        }

        public void MoveTo(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> targets, IEnumerable<RegularLanguageTransitionNode.SourceElement> sources)
        {
            this.OutTransitions.Add(check, targets, sources);
            foreach (var target in targets)
                target.InTransitions.Add(check, this, sources);
        }

        public void MoveTo(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> targets)
        {
            this.OutTransitions.Add(check, targets);
            foreach (var target in targets)
                target.InTransitions.Add(check, this);
        }

        public void MoveToOptional(RegularLanguageBitArray check, RegularLanguageState target, IEnumerable<RegularLanguageState> edges)
        {
            if (edges == null)
                throw new ArgumentNullException("edges");
            this.MoveTo(check, target);
            foreach (var edge in edges)
                edge.MoveTo(check, target);
        }

        public void MoveToAndStar(RegularLanguageBitArray check, RegularLanguageState target, IEnumerable<RegularLanguageState> edges)
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
                    edge.MoveTo(transition);
                edge.MoveTo(check, target);
            }
        }

        public void MoveToStar(RegularLanguageBitArray check, RegularLanguageState target, IEnumerable<RegularLanguageState> edges)
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
                    edge.MoveTo(transition);
            }
        }

        public void Star()
        {
            this.RequiredStar();
            this.MakeEdge();
        }
        #endregion
        /// <summary>
        /// Clones the current <see cref="RegularLanguageState"/>.
        /// </summary>
        /// <returns>A new, exact copy, of the <see cref="RegularLanguageState"/>.</returns>
        /// <remarks>Not a shallow copy.</remarks>
        public RegularLanguageState Clone()
        {
            if (cloneSet.ContainsKey(this))
                return cloneSet[this];
            var result = this.GetNewInstance();
            foreach (var source in this.Sources)
                result.sources.Add(source);
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
        internal virtual RegularLanguageState GetNewInstance()
        {
            return new RegularLanguageState();
        }

        public static RegularLanguageState operator |(RegularLanguageState left, RegularLanguageState right)
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
        /// Obtains the edges for the current <see cref="RegularLanguageState"/>.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> which iterates the 
        /// edges of the current <see cref="RegularLanguageState"/>.</returns>
        public IEnumerable<RegularLanguageState> ObtainEdges()
        {
            //var edges = ObtainEdges(this);
            //var dEdges = RegularLanguageState.RecursiveAid.RecurseSingle<IEnumerable<RegularLanguageState>, RegularLanguageState>("ObtainEdges", this);
            //Debug.Assert(edges.SequenceEqual(dEdges));
            foreach (var item in ObtainEdges(this))//RegularLanguageState.RecursiveAid.RecurseSingle<IEnumerable<RegularLanguageState>, RegularLanguageState>("ObtainEdges", this))
                yield return item;
        }

        private static IList<RegularLanguageState> ObtainEdges(RegularLanguageState source)
        {
            IList<RegularLanguageState> result = new List<RegularLanguageState>();
            ObtainEdges(source, result);
            return result;
        }

        /// <summary>
        /// Obtains the edges for a given <paramref name="source"/> <see cref="RegularLanguageState"/>.
        /// </summary>
        /// <param name="source">The <see cref="RegularLanguageState"/> from which to start
        /// edge scanning at.</param>
        /// <param name="target">The <see cref="IList{T}"/> to receive the edges.</param>
        private static void ObtainEdges(RegularLanguageState source, IList<RegularLanguageState> target)
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
        /// Returns whether the <see cref="RegularLanguageState"/> is marked as an edge
        /// even considering it has transitions going past it.
        /// </summary>
        /// <remarks>Used to represent non-terminal edges.</remarks>
        public bool IsMarked { get { return this.isEdge; } }

        /// <summary>
        /// Returns whether the current <see cref="RegularLanguageState"/> is an edge.
        /// </summary>
        /// <returns>true if the current <see cref="RegularLanguageState"/> is an edge; false
        /// otherwise.</returns>
        /// <remarks>If the number of transitions from a <see cref="RegularLanguageState"/>
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

        private static List<RegularLanguageTransitionNode> GetRuleList = new List<RegularLanguageTransitionNode>();

        public override bool Equals(object obj)
        {
            return obj == this;
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
                    if (this.IsEdge())
                    {
                        itw.Write("<END>");
                        itw.WriteLine();
                    }
                    foreach (var item in this.OutTransitions)
                    {
                        foreach (var subItem in item.Targets)
                        {
                            string current = subItem.ToString();
                            string[] currentLines = current.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            itw.Write("{0}", item.Check);
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
                string result = "    " + sr.ReadToEnd();
                itw.Close();
                tw.Close();
                sr.Close();
                sr.Dispose();
                tw.Dispose();
                ms.Close();
                ms.Dispose();
                itw.Dispose();
                if (firstOnStack)
                    result = string.Format("Regular State Count: {0}\r\n {{Start {1}}}", this.Count().ToString(), this.StateValue) + "\r\n" + result;
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
                if (edge.StateValue == -1)
                    edge.StateValue = state++;
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
            if (this.OutTransitions.Count > 0 && this.StateValue == -1)
                this.StateValue = stateValue++;
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

        internal List<RegularLanguageState> GetFlatform()
        {
            List<RegularLanguageState> flatform = new List<RegularLanguageState>();
            Merger.FlatlineState(this, flatform);
            return flatform;
        }

        #region IEquatable<RegularLanguageState> Members

        public bool Equals(RegularLanguageState other)
        {
            return other == this;
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

        public void AddSource(ITokenItem source)
        {
            if (!sources.Contains(source))
                this.sources.Add(source);
        }

        private bool InstateContains(RegularLanguageState target)
        {

            foreach (var item in InTransitions.TargetSets)
                if (item.Contains(target))
                    return true;
            return false;
        }

        internal void KillInState(RegularLanguageState target)
        {
            if (InstateContains(target))
            {
                if (target.OutTransitions != null)
                {
                    foreach (var transition in target.OutTransitions)
                    {
                        var transitionCheck = transition.Check;
                        if (transition.Targets.Contains(this) &&
                            this.InTransitions.Contains(transitionCheck) &&
                            this.InTransitions[transitionCheck].Contains(target))
                        {
                            target.OutTransitions.Remove(transitionCheck, this);
                            this.InTransitions.Remove(transitionCheck, target);
                            break;
                        }
                    }
                }
                else
                    foreach (var check in this.InTransitions.Checks)
                        if (InTransitions[check].Contains(target))
                        {
                            InTransitions.Remove(check, target);
                            break;
                        }
            }
        }


        #region IVisualState<RegularLanguageState,RegularLanguageTransitionNode,RegularLanguageBitArray> Members

        //IVisualTransitionTable<RegularLanguageState, RegularLanguageTransitionNode, RegularLanguageBitArray> IVisualState<RegularLanguageState, RegularLanguageTransitionNode, RegularLanguageBitArray>.OutTransitions
        //{
        //    get { return this.OutTransitions; }
        //}

        //IVisualTransitionTable<RegularLanguageState, RegularLanguageTransitionNode, RegularLanguageBitArray> IVisualState<RegularLanguageState, RegularLanguageTransitionNode, RegularLanguageBitArray>.InTransitions
        //{
        //    get { return this.InTransitions; }
        //}

        //bool IVisualState<RegularLanguageState, RegularLanguageTransitionNode, RegularLanguageBitArray>.IsEdge
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
