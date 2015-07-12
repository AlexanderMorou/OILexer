using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a basic structure which outlines the ambiguity of an
    /// edge state on a production rule and the necessary information
    /// needed to disambiguate it through a series of look-ahead
    /// steps, which would include the call chain needed to cause the
    /// ambiguity.
    /// </summary>
    public class ProductionRuleProjectionFollow :
        IProductionRuleSource
    {
        private ProductionRuleProjectionDPath[] initialVariations;
        /// <summary>
        /// The <see cref="ProductionRuleProjectionDPathSet"/> which denotes
        /// the series of paths which are in an initial deadlock for 
        /// disambiguation.
        /// </summary>
        private ProductionRuleProjectionDPathSet initialPaths;
        /// <summary>
        /// The <see cref="ProductionRuleProjectionNode"/> which delineates
        /// the ambiguity.
        /// </summary>
        private ProductionRuleProjectionNode edgeNode;
        private SyntacticalNFAState nfaState;
        private SyntacticalDFAState dfaState;
        /// <summary>
        /// Returns the <see cref="Int32"/> value denoting the
        /// number of original paths which tied for the given 
        /// transition.
        /// </summary>
        /// <remarks>
        /// This context is largely used to simplify building the resultant
        /// predictor.</remarks>
        public int TyingPaths
        {
            get
            {
                return this.initialPaths.Count;
            }
        }

        /// <summary>
        /// Returns the <see cref="ProductionRuleProjectionNode"/> which
        /// delineates the ambiguity.
        /// </summary>
        public ProductionRuleProjectionNode EdgeNode
        {
            get
            {
                return this.edgeNode;
            }
        }

        public ProductionRuleProjectionDPathSet InitialPaths
        {
            get
            {
                return this.initialPaths;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ProductionRuleProjectionFollow"/> instance
        /// with the <paramref name="initialPaths"/>, and <paramref name="edgeNode"/>
        /// provided.
        /// </summary>
        /// <param name="initialPaths">
        /// The <see cref="ProductionRuleProjectionDPathSet"/> built from the
        /// analysis of the incoming paths to a rule which are in an initial
        /// deadlock for disambiguation upon the rule entering the 
        /// <paramref name="edgeNode"/>.
        /// </param>
        /// <param name="edgeNode">The <see cref="ProductionRuleProjectionNode"/> which
        /// delineates the ambiguity.</param>
        internal ProductionRuleProjectionFollow(ProductionRuleProjectionDPathSet initialPaths, ProductionRuleProjectionNode edgeNode)
        {
            this.initialPaths = initialPaths;
            this.edgeNode = edgeNode;
            /* *
             * Initial variations are used to deconstruct the incoming states
             * *
             * Necessary because certain commonality groupings yield similarities
             * that are only similar to the edgeNode, which means the transition
             * sets are identical.
             * *
             * ToDo: Verify this claim.
             * */
        }


        internal void BuildNFA(Dictionary<SyntacticalDFAState, ProductionRuleProjectionNode> allProjectionNodes, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> ruleDFAStates, Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> ruleGrammarLookup, GrammarSymbolSet grammarSymbolSet, ICompilerErrorCollection compilerErrorCollection)
        {
            this.nfaState = new SyntacticalNFAState(ruleDFAStates, grammarSymbolSet);
            this.nfaState.SetInitial(this);
            var targetState = this.InitialPaths.GetNFAState(ruleDFAStates, (GrammarSymbolSet)grammarSymbolSet);

            this.InitialPaths.BuildNFA(allProjectionNodes, ruleGrammarLookup, compilerErrorCollection, ruleDFAStates, (GrammarSymbolSet)grammarSymbolSet);
            nfaState.MoveTo(this.InitialPaths.Discriminator, targetState);
        }

        internal void DeterminateAutomata()
        {
            if (this.nfaState != null)
                this.dfaState = nfaState.DeterminateAutomata();
            else
                throw new InvalidOperationException();
        }

        internal void ReduceAutomata(bool recognizer, Func<SyntacticalDFAState, SyntacticalDFAState, bool> additionalReducer = null)
        {
            if (this.dfaState != null)
                this.dfaState.ReduceDFA(recognizer, additionalReducer);
            else
                throw new InvalidOperationException();
        }

        internal void Enumerate(SyntacticAnalysisCore.StateLocker slock)
        {
            if (this.dfaState != null)
                lock (slock)
                {
                    int stateIndex = slock.State;
                    this.dfaState.Enumerate(ref stateIndex);
                    slock.State = stateIndex;
                }
            else
                throw new InvalidOperationException();
        }

        internal void Adapt(ParserCompiler compiler)
        {
            if (this.dfaState != null)
            {
                var adaptLookup = new Dictionary<SyntacticalDFAState, ProductionRuleProjectionAdapter>();
                this.Adapter = ProductionRuleProjectionAdapter.Adapt(this.dfaState, ref adaptLookup, compiler);
                this.Adapter.AssociatedContext.StateAdapterLookup =  new ControlledDictionary<SyntacticalDFAState,ProductionRuleProjectionAdapter>(adaptLookup);
                foreach (var adapter in adaptLookup.Values)
                    adapter.AssociatedContext.PostConnect(this.EdgeNode, this.Adapter);
            }
            else
                throw new InvalidOperationException();
        }

        public IOilexerGrammarProductionRuleEntry Rule
        {
            get { return this.edgeNode.Rule; }
        }

        public ProductionRuleProjectionAdapter Adapter { get; private set; }

        internal void ReplaceState(SyntacticalDFAState replacement, ProductionRuleProjectionAdapter adapter)
        {
            this.dfaState = replacement;
            this.Adapter = adapter;
        }

        public SyntacticalDFAState DFAState { get { return this.dfaState; } }
    }
}
