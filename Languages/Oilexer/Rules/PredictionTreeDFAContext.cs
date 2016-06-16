using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class PredictionTreeDFAContext
    {
        private PredictionTreeLeaf node;
        private PredictionTreeDFAdapter rootAdapter;
        private PredictionTreeBranch[] recursiveBranches;
        private bool gotDecision;
        private PredictionTreeDestination decision;
        private bool gotReduction;

        private Dictionary<PredictionTreeDFAContext, ProductionRuleLookAheadBucket> automationBuckets;
        private Dictionary<ProductionRuleLookAheadBucket, List<PredictionTreeDFAContext>> locallyDefinedBuckets;
        private Dictionary<ProductionRuleLookAheadBucket, PredictionTreeDFAContext> referencedBuckets;
        private Dictionary<ProductionRuleLookAheadBucket, int> bucketUseCount = new Dictionary<ProductionRuleLookAheadBucket, int>();
        private ProductionRuleProjectionReduction reduction;
        private PredictionTreeLeaf rootNode;
        private bool? _isLeftRecursive;
        public PredictionTreeDFAdapter Adapter { get; internal set; }

        public PredictionTreeDFAdapter RootAdapter { get { return this.rootAdapter; } }

        public bool IsRuleNode
        {
            get
            {
                return this.rootNode.Veins.DFAOriginState is SyntacticalDFARootState;
            }
        }

        /// <summary>
        /// Returns the <see cref="ParserCompiler"/> which was 
        /// responsible for creating this <see cref="PredictionTreeDFAContext"/>.
        /// </summary>
        public ParserCompiler AssociatedCompiler { get; internal set; }

        internal void Connect(PredictionTreeDFAdapter adapter, ParserCompiler compiler)
        {
            this.Adapter = adapter;
            this.AssociatedCompiler = compiler;
        }

        internal void PostConnect(PredictionTreeLeaf rootNode, PredictionTreeDFAdapter rootAdapter)
        {
            this.rootNode = rootNode;
            this.rootAdapter = rootAdapter;
        }

        public bool IsLeftRecursiveProjection
        {
            get
            {
                return this.rootNode.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.None;
            }
        }

        public ProductionRuleLeftRecursionType LeftRecursiveType
        {
            get
            {
                if (this._IsLeftRecursiveProjectionInternal)
                    return ProductionRuleLeftRecursionType.Direct;
                else
                    return IsLeftRecursiveProjection 
                        ? ProductionRuleLeftRecursionType.Indirect
                        : ProductionRuleLeftRecursionType.None;
            }
        }

        public bool RequiresLeftRecursiveCaution
        {
            get
            {
                if (!this.IsLeftRecursiveProjection)
                    return false;
                if (this.LeftRecursiveType == ProductionRuleLeftRecursionType.Direct)
                    return true;
                else
                    return (from tr in this.rootNode.Veins.DFAOriginState.OutTransitions.FullCheck.SymbolicBreakdown(this.AssociatedCompiler).Rules.Values
                            where tr.Leaf.Veins.DFAOriginState.OutTransitions.Count == 1
                            from inPath in this.rootNode.IncomingPaths
                            where inPath.Contains(tr.Leaf)
                            let index = inPath.IndexOf(tr.Leaf)
                            let myIndex = inPath.IndexOf(this.rootNode)
                            where index < inPath.Depth && index < myIndex
                            where inPath.GetDeviationAt(index) == 0
                            select 1).Any();
            }
        }

        public bool RequiresInnerRecursionSwap { get; set; }

        internal IEnumerable<PredictionTreeBranch> RecursiveBranches
        {
            get
            {
                if (this.IsRuleNode || (this.RootAdapter != null && this.RootAdapter.AssociatedContext != null && this.RootAdapter.AssociatedContext == this))
                    return this.recursiveBranches ?? (this.recursiveBranches = (this.ConnectRecursiveBranches().ToArray()));
                else
                    return this.RootAdapter.AssociatedContext.RecursiveBranches;
            }
        }

        private IEnumerable<PredictionTreeBranch> ConnectRecursiveBranches()
        {
            if (this.rootAdapter.AssociatedContext != this)
                yield break;
            foreach (var transition in this.rootNode.LookAhead.Keys)
                foreach (var path in this.rootNode.LookAhead[transition])
                    if (path.GetRecursionType() != ProductionRuleLeftRecursionType.None)
                        yield return path;
        }

        internal PredictionTreeDestination Decision
        {
            get
            {

                return this.decision ?? (this.gotDecision ? this.decision : (this.decision = this.ConnectDecision()));
            }
        }

        private PredictionTreeDestination ConnectDecision()
        {
            this.gotDecision = true;
            var decisionNode = this.Adapter.AssociatedState.Sources.FirstOrDefault(k => k.Item1 is PredictionTreeDestination);
            if (decisionNode != null)
                return (PredictionTreeDestination)decisionNode.Item1;
            return null;
        }

        private ProductionRuleProjectionReduction ConnectReduction()
        {
            this.gotReduction = true;
            var reductionNode = this.Adapter.AssociatedState.Sources.FirstOrDefault(k => k.Item1 is ProductionRuleProjectionReduction);
            if (reductionNode != null)
                return (ProductionRuleProjectionReduction)reductionNode.Item1;
            return null;
        }

        internal ProductionRuleProjectionReduction Reduction
        {
            get
            {
                return this.reduction ?? (this.gotReduction ? this.reduction : (this.reduction = this.ConnectReduction()));
            }
        }

        internal ProductionRuleLookAheadBucket CreateBucket(PredictionTreeDFAContext bucketOwner)
        {
            if (this.automationBuckets == null)
                this.automationBuckets = new Dictionary<PredictionTreeDFAContext, ProductionRuleLookAheadBucket>();
            ProductionRuleLookAheadBucket result;
            if (!this.automationBuckets.TryGetValue(bucketOwner, out result))
                this.automationBuckets.Add(bucketOwner, result = new ProductionRuleLookAheadBucket() { Owner = bucketOwner, BucketID = this.automationBuckets.Count + 1 });
            if (!bucketUseCount.ContainsKey(result))
                bucketUseCount.Add(result, 0);
            bucketUseCount[result]++;
            return result;
        }

        internal void DefineBucketOnTarget(PredictionTreeDFAContext bucketReferencer)
        {
            if (this.locallyDefinedBuckets == null)
                this.locallyDefinedBuckets = new Dictionary<ProductionRuleLookAheadBucket, List<PredictionTreeDFAContext>>();
            var bucket = this.RootAdapter.AssociatedContext.CreateBucket(this);
            List<PredictionTreeDFAContext> sourcesWhichUseBucket;
            if (!this.locallyDefinedBuckets.TryGetValue(bucket, out sourcesWhichUseBucket))
                this.locallyDefinedBuckets.Add(bucket, sourcesWhichUseBucket = new List<PredictionTreeDFAContext>());
            if (!sourcesWhichUseBucket.Contains(bucketReferencer))
                sourcesWhichUseBucket.Add(bucketReferencer);
            bucketReferencer.ReferenceBucketFromTarget(this, bucket);
        }

        internal void ReferenceBucketFromTarget(PredictionTreeDFAContext context, ProductionRuleLookAheadBucket bucket)
        {
            if (this.referencedBuckets == null)
                this.referencedBuckets = new Dictionary<ProductionRuleLookAheadBucket, PredictionTreeDFAContext>();
            if (!this.referencedBuckets.ContainsKey(bucket))
                this.referencedBuckets.Add(bucket, context);
        }


        public IEnumerable<ProductionRuleLookAheadBucket> FullAutomationBuckets
        {
            get
            {
                if (this.RootAdapter.AssociatedContext.automationBuckets == null)
                    this.RootAdapter.AssociatedContext.automationBuckets = new Dictionary<PredictionTreeDFAContext, ProductionRuleLookAheadBucket>();
                foreach (var bucket in this.RootAdapter.AssociatedContext.automationBuckets.Values)
                    yield return bucket;
            }
        }
        public IEnumerable<ProductionRuleLookAheadBucket> ReferencedBuckets
        {
            get
            {
                if (this.referencedBuckets == null)
                    referencedBuckets = new Dictionary<ProductionRuleLookAheadBucket, PredictionTreeDFAContext>();
                foreach (var bucket in referencedBuckets.Keys)
                    yield return bucket;
            }
        }

        public IEnumerable<ProductionRuleLookAheadBucket> DefinedBuckets
        {
            get
            {
                foreach (var bucket in this.locallyDefinedBuckets.Keys)
                    yield return bucket;
            }
        }

        public ControlledDictionary<SyntacticalDFAState, PredictionTreeDFAdapter> StateAdapterLookup { get; set; }

        private bool _IsLeftRecursiveProjectionInternal
        {
            get
            {
                return (this._isLeftRecursive ?? (this._isLeftRecursive = this.Initialize_IsLeftRecursiveProjectionInternal())).Value;
            }
        }

        private bool? Initialize_IsLeftRecursiveProjectionInternal()
        {
            if (this.rootAdapter != this.Adapter)
                return false;

            /* ToDo: Do a path depth search that looks at the paths introduced from the root node of the
             * projections entered and see if from 1->Depth the current node yields the current node. */
            List<SyntacticalDFAState> stateSet = new List<SyntacticalDFAState>();
            SyntacticalDFAState.FlatlineState(this.Adapter.AssociatedState, stateSet);
            if (!stateSet.Contains(this.Adapter.AssociatedState))
                stateSet.Insert(0, this.Adapter.AssociatedState);
            var aggSources = (from targetState in stateSet
                              from s in targetState.Sources
                              select s).ToArray();
            return (from s in aggSources
                    let decision = s.Item1 as PredictionTreeDestination
                    where decision != null
                    let symbolicDecision = decision.DecidingFactor.SymbolicBreakdown(this.AssociatedCompiler)
                    where symbolicDecision.Rules.Count > 0
                    let firstRule = symbolicDecision.Rules.Values.First()
                    let decisiveAdapter = this.AssociatedCompiler.AllRuleAdapters[firstRule.Rule, firstRule.DFAState]
                    where decisiveAdapter.AssociatedContext.Leaf.ContainsKey(rootNode.Rule)
                    select 1).Any();
        }
    }

    public enum LeftRecursionHandling
    {
        NonGreedyMechanism,//nonGreedy
        AvoidanceMechanism,//includeRuleContext
    }
}