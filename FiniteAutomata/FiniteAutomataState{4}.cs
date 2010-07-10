using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Oilexer._Internal;
namespace Oilexer.FiniteAutomata
{
    public abstract class FiniteAutomataState<TCheck, TState, TForwardNodeTarget, TSourceElement> :
        IFiniteAutomataState<TCheck, TState, TForwardNodeTarget, TSourceElement>
        where TCheck :
            IFiniteAutomataSet<TCheck>,
            new()
        where TState :
            FiniteAutomataState<TCheck, TState, TForwardNodeTarget, TSourceElement>
        where TSourceElement :
            IFiniteAutomataSource
    {
        private Dictionary<TSourceElement, FiniteAutomationSourceKind> sources = new Dictionary<TSourceElement, FiniteAutomationSourceKind>();
        internal static List<TState> enumStack = new List<TState>();
        internal static List<TState> sourceStack = new List<TState>();
        private bool isMarked = false;
        private bool forcedNoEdge = false;
        private IFiniteAutomataMultiTargetTransitionTable<TCheck, TState> inTransitions;
        private IFiniteAutomataTransitionTable<TCheck, TState, TForwardNodeTarget> outTransitions;
        protected FiniteAutomataState()
        {
            this.StateValue = -1;
        }

        #region IFiniteAutomataState<TCheck,TState,TForwardNodeTarget,TBackwardNodeTarget> Members

        public IFiniteAutomataTransitionTable<TCheck, TState, TForwardNodeTarget> OutTransitions
        {
            get {
                if (this.outTransitions == null)
                    this.outTransitions = this.InitializeOutTransitionTable();
                return this.outTransitions;
            }
        }

        protected virtual IFiniteAutomataMultiTargetTransitionTable<TCheck, TState> InitializeInTransitionTable()
        {
            return new FiniteAutomataMultiTargetTransitionTable<TCheck, TState>(false);
        }

        protected abstract IFiniteAutomataTransitionTable<TCheck, TState, TForwardNodeTarget> InitializeOutTransitionTable();

        #endregion

        #region IFiniteAutomataState<TCheck,TState> Members

        /// <summary>
        /// Returns/sets whether the current <see cref="FiniteAutomataState{TCheck, TState, TForwardNodeTarget, TSourceElement}"/>
        /// is an edge state.
        /// </summary>
        public bool IsEdge
        {
            get
            {
                return (this.OutTransitions.Count == 0 && !forcedNoEdge) || isMarked;
            }
            set
            {
                if (value && this.forcedNoEdge)
                    forcedNoEdge = false;
                this.isMarked = value;
            }
        }

        public bool ForcedNoEdge
        {
            get
            {
                return this.forcedNoEdge;
            }
            set
            {
                this.forcedNoEdge = value;
            }
        }

        /// <summary>
        /// Returns whether the current <see cref="FiniteAutomataState{TCheck, TState, TForwardNodeTarget, TSourceElement}"/> 
        /// is marked as an edge state regardless of the number of
        /// outgoing transitions.
        /// </summary>
        public bool IsMarked
        {
            get {
                return this.isMarked;
            }
        }

        public IFiniteAutomataMultiTargetTransitionTable<TCheck, TState> InTransitions
        {
            get
            {
                if (this.inTransitions == null)
                    this.inTransitions = this.InitializeInTransitionTable();
                return this.inTransitions;
            }
        }

        IFiniteAutomataTransitionTable<TCheck, TState> IFiniteAutomataState<TCheck, TState>.OutTransitions
        {
            get { return this.OutTransitions; }
        }

        /// <summary>
        /// Obtains the edges of the current
        /// <see cref="FiniteAutomataState{TCheck, TState, TForwardNodeTarget, TSourceElement}"/>
        /// which are plausible points to terminate the current state.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance which
        /// yields the edges of the current 
        /// <see cref="FiniteAutomataState{TCheck, TState, TForwardNodeTarget, TSourceElement}"/>.
        /// </returns>
        public abstract IEnumerable<TState> ObtainEdges();


        #endregion

        internal void MovedInto(TCheck check, List<TState> target)
        {
            this.InTransitions.Add(check, target);
        }

        internal void MovedInto(TCheck check, TState target)
        {
            this.InTransitions.AddState(check, target);
        }


        #region IFiniteAutomataState<TCheck,TState> Members

        /// <summary>
        /// Creates a transition from the current 
        /// <see cref="FiniteAutomataState{TCheck, TState, TForwardNodeTarget, TSourceElement}}"/>
        /// to the <paramref name="target"/> with the
        /// <paramref name="condition"/> for transition provided.
        /// </summary>
        /// <param name="condition">The <typeparamref name="TCheck"/>
        /// which restricts the move.</param>
        /// <param name="target">The <typeparamref name="TState"/>
        /// to move into.</param>
        public abstract void MoveTo(TCheck condition, TState target);

        #endregion


        internal abstract int CountStates();

        /// <summary>
        /// Iterates and assigns all elements within the state transition
        /// scope their uniqueness value.
        /// </summary>
        internal void Enumerate()
        {
            int state = 0;
            List<TState> enumStack = new List<TState>();
            this.EnumerateSet(ref state, enumStack);
            this.Enumerate(ref state, enumStack);
            var edges = this.ObtainEdges();
            foreach (var edge in edges)
            {
                if (edge.StateValue == -1)
                    edge.StateValue = state++;
            }
        }

        /// <summary>
        /// Iterates and assigns all elements within the state transition
        /// scope their uniqueness value continuing from the 
        /// <paramref name="stateValue"/> provided.
        /// </summary>
        /// <param name="stateValue">The most recently calculated state
        /// value.</param>
        internal void EnumerateSet(ref int stateValue, List<TState> enumStack)
        {
            if (enumStack.Contains((TState)this))
                return;
            //Non-terminals only.
            if (this.OutTransitions.Count > 0 && this.StateValue == -1)
                this.StateValue = stateValue++;
        }

        private void Enumerate(ref int stateValue, List<TState> enumStack)
        {
            var thisTState = this as TState;
            if (enumStack.Contains(thisTState))
                return;
            enumStack.Add(thisTState);
            if (this.OutTransitions.Count > 0)
            {
                /* *
                 * Index siblings relative to one another,
                 * then have them recurse their elements.
                 * */
                foreach (var item in this.OutTransitions.Targets)
                    item.EnumerateSet(ref stateValue, enumStack);
                foreach (var item in this.OutTransitions.Targets)
                    item.Enumerate(ref stateValue, enumStack);
            }
            if (enumStack[0] == this)
                enumStack.Clear();
        }

        /// <summary>
        /// Returns the <see cref="Int32"/> value unique 
        /// to the current state.
        /// </summary>
        public int StateValue { get; internal set; }

        public void SetInitial(TSourceElement source)
        {
            if (!sources.ContainsKey(source))
                sources.Add(source, FiniteAutomationSourceKind.None);
            this.sources[source] &= ~FiniteAutomationSourceKind.Intermediate;
            this.sources[source] |= FiniteAutomationSourceKind.Initial;
        }

        public void SetIntermediate(TSourceElement source)
        {
            if (!sources.ContainsKey(source))
                sources.Add(source, FiniteAutomationSourceKind.None);
            this.sources[source] |= FiniteAutomationSourceKind.Intermediate;
        }

        public void SetRepeat(TSourceElement source)
        {
            if (!sources.ContainsKey(source))
                sources.Add(source, FiniteAutomationSourceKind.None);
            this.sources[source] |= FiniteAutomationSourceKind.RepeatPoint;
        }

        public void SetFinal(TSourceElement source)
        {
            if (!sources.ContainsKey(source))
                sources.Add(source, FiniteAutomationSourceKind.None);
            this.sources[source] &= ~FiniteAutomationSourceKind.Intermediate;
            this.sources[source] |= FiniteAutomationSourceKind.Final;
        }

        protected void UnifySources(TState target)
        {
            foreach (TSourceElement source in target.sources.Keys)
                this.sources.Add(source, target.sources[source]);
        }

        internal void ReplicateSourcesToAlt<TState2, TForwardNodeTarget>(TState2 altTarget)
            where TState2 :
                FiniteAutomataState<TCheck, TState2, TForwardNodeTarget, TSourceElement>
        {
            foreach (var source in this.sources.Keys)
                if (!altTarget.sources.ContainsKey(source))
                    altTarget.sources.Add(source, this.sources[source]);
                else
                    altTarget.sources[source] |= this.sources[source];
        }
        internal void ReplicateSources(TState target)
        {
            ReplicateSourcesToAlt<TState, TForwardNodeTarget>(target);
        }

        internal void IterateSources(Action<TSourceElement, FiniteAutomationSourceKind> iterator)
        {
            foreach (TSourceElement source in this.sources.Keys)
            {
                var kind = sources[source];
                if ((kind & FiniteAutomationSourceKind.Intermediate) == FiniteAutomationSourceKind.Intermediate)
                    iterator(source, FiniteAutomationSourceKind.Intermediate);
                if ((kind & FiniteAutomationSourceKind.Initial) == FiniteAutomationSourceKind.Initial)
                    iterator(source, FiniteAutomationSourceKind.Initial);
                if ((kind & FiniteAutomationSourceKind.RepeatPoint) == FiniteAutomationSourceKind.RepeatPoint)
                    iterator(source, FiniteAutomationSourceKind.RepeatPoint);
                if ((kind & FiniteAutomationSourceKind.Final) == FiniteAutomationSourceKind.Final)
                    iterator(source, FiniteAutomationSourceKind.Final);
            }
        }

        internal IEnumerable<Tuple<TSourceElement, FiniteAutomationSourceKind>> Sources
        {
            get
            {
                foreach (var source in this.sources.Keys)
                    yield return new Tuple<TSourceElement, FiniteAutomationSourceKind>(source, this.sources[source]);
            }
        }

        public int SourceCount
        {
            get
            {
                return this.sources.Count;
            }
        }
    }
}
