using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class ProductionRuleNormalContext
    {
        private PredictionTreeLeaf node;
        private IIntermediateClassMethodMember parseMethod;
        private IIntermediateClassMethodMember parseInternalMethod;
        private IControlledDictionary<SyntacticalDFAState, ProductionRuleNormalAdapter> adapterLookup;
        internal void ConnectAdapter(ProductionRuleNormalAdapter adapter, ParserCompiler associatedBuilder)
        {
            this.Adapter = adapter;
            this.AssociatedBuilder = associatedBuilder;
        }

        public IIntermediateClassMethodMember ParseMethod
        {
            get
            {
                return this.RootAdapter.AssociatedContext.parseMethod;
            }
            set
            {
                this.RootAdapter.AssociatedContext.parseMethod = value;
            }
        }

        public IControlledDictionary<SyntacticalDFAState, ProductionRuleNormalAdapter> StateAdapterLookup
        {
            get
            {
                return this.RootAdapter.AssociatedContext.adapterLookup;
            }
            internal set
            {
                this.adapterLookup = value;
            }
        }

        public ParserCompiler AssociatedBuilder { get; private set; }

        public ProductionRuleNormalAdapter Adapter { get; private set; }

        public PredictionTreeLeaf Leaf { get { return this.node ?? (this.node = this.ConnectNode()); } }

        public IOilexerGrammarProductionRuleEntry Rule
        {
            get
            {
                return this.Leaf.Rule;
            }
        }

        public bool RequiresProjection
        {
            get
            {
                return this.AssociatedBuilder.AdvanceMachines.ContainsKey(this.AssociatedBuilder.AllProjectionNodes[this.Adapter.AssociatedState]);
            }
        }

        public bool RequiresFollowProjection
        {
            get
            {
                if (this.AssociatedBuilder.FollowAmbiguousNodes != null && this.AssociatedBuilder.FollowAmbiguousNodes.Length > 0)
                    return this.AssociatedBuilder.FollowAmbiguousNodes.Contains(this.Leaf);
                else
                    return false;
            }
        }

        public PredictionTreeDFAdapter GetPredictiveProjection()
        {
            if (this.RequiresProjection)
                return this.AssociatedBuilder.AdvanceMachines[this.AssociatedBuilder.AllProjectionNodes[this.Adapter.AssociatedState]];
            else
                throw new InvalidOperationException("This Normal Rule State does not require a predictive projection.");
        }

        public bool IsRuleNode
        {
            get
            {
                return this.Adapter.AssociatedState is SyntacticalDFARootState;
            }
        }

        public ProductionRuleNormalAdapter RootAdapter
        {
            get
            {
                if (this.IsRuleNode)
                    return this.Adapter;
                return this.AssociatedBuilder.RuleAdapters[this.Leaf.Rule];
            }
        }

        /// <summary>
        /// Returns the <see cref="IIntermediateClassType"/> associated to the rule of the automation.
        /// </summary>
        public IIntermediateInterfaceType ModelInterface
        {
            get
            {
                return this.AssociatedBuilder.RelationalModelMapping[this.Rule].ImplementationDetails.Value.RelativeInterface;
            }
        }

        private PredictionTreeLeaf ConnectNode()
        {
            return this.AssociatedBuilder.AllProjectionNodes[this.Adapter.AssociatedState];
        }

        public IIntermediateClassMethodMember ParseInternalMethod
        {
            get
            {
                return this.RootAdapter.AssociatedContext.parseInternalMethod;
            }
            set
            {
                this.RootAdapter.AssociatedContext.parseInternalMethod = value;
            }
        }
    }
}
