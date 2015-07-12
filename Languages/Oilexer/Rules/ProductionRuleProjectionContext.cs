using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class ProductionRuleProjectionContext
    {
        private ProductionRuleProjectionNode node;
        private ProductionRuleProjectionAdapter rootAdapter;
        private ProductionRuleProjectionDPath[] recursiveBranches;
        private bool gotDecision;
        private ProductionRuleProjectionDecision decision;
        private bool gotReduction;

        private Dictionary<ProductionRuleProjectionContext, ProductionRuleLookAheadBucket> automationBuckets;
        private Dictionary<ProductionRuleLookAheadBucket, List<ProductionRuleProjectionContext>> locallyDefinedBuckets;
        private Dictionary<ProductionRuleLookAheadBucket, ProductionRuleProjectionContext> referencedBuckets;
        private Dictionary<ProductionRuleLookAheadBucket, int> bucketUseCount = new Dictionary<ProductionRuleLookAheadBucket, int>();
        private ProductionRuleProjectionReduction reduction;
        private ProductionRuleProjectionNode rootNode;
        private bool? _isLeftRecursive;
        public ProductionRuleProjectionAdapter Adapter { get; internal set; }

        public ProductionRuleProjectionAdapter RootAdapter { get { return this.rootAdapter; } }

        public bool IsRuleNode
        {
            get
            {
                return this.rootNode.Value.OriginalState is SyntacticalDFARootState;
            }
        }

        /// <summary>
        /// Returns the <see cref="ParserCompiler"/> which was 
        /// responsible for creating this <see cref="ProductionRuleProjectionContext"/>.
        /// </summary>
        public ParserCompiler AssociatedCompiler { get; internal set; }

        internal void Connect(ProductionRuleProjectionAdapter adapter, ParserCompiler compiler)
        {
            this.Adapter = adapter;
            this.AssociatedCompiler = compiler;
        }

        internal void PostConnect(ProductionRuleProjectionNode rootNode, ProductionRuleProjectionAdapter rootAdapter)
        {
            this.rootNode = rootNode;
            this.rootAdapter = rootAdapter;
        }

        public bool IsLeftRecursiveProjection
        {
            get
            {
                return this.rootNode.Value.LeftRecursionType != ProductionRuleLeftRecursionType.None;
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

        internal IEnumerable<ProductionRuleProjectionDPath> RecursiveBranches
        {
            get
            {
                if (this.IsRuleNode || (this.RootAdapter != null && this.RootAdapter.AssociatedContext != null && this.RootAdapter.AssociatedContext == this))
                    return this.recursiveBranches ?? (this.recursiveBranches = (this.ConnectRecursiveBranches().ToArray()));
                else
                    return this.RootAdapter.AssociatedContext.RecursiveBranches;
            }
        }

        private IEnumerable<ProductionRuleProjectionDPath> ConnectRecursiveBranches()
        {
            if (this.rootAdapter.AssociatedContext != this)
                yield break;
            foreach (var transition in this.rootNode.LookAhead.Keys)
                foreach (var path in this.rootNode.LookAhead[transition])
                    if (path.GetRecursionType() != ProductionRuleLeftRecursionType.None)
                        yield return path;
        }

        internal ProductionRuleProjectionDecision Decision
        {
            get
            {

                return this.decision ?? (this.gotDecision ? this.decision : (this.decision = this.ConnectDecision()));
            }
        }

        private ProductionRuleProjectionDecision ConnectDecision()
        {
            this.gotDecision = true;
            var decisionNode = this.Adapter.AssociatedState.Sources.FirstOrDefault(k => k.Item1 is ProductionRuleProjectionDecision);
            if (decisionNode != null)
                return (ProductionRuleProjectionDecision)decisionNode.Item1;
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

        internal ProductionRuleLookAheadBucket CreateBucket(ProductionRuleProjectionContext bucketOwner)
        {
            if (this.automationBuckets == null)
                this.automationBuckets = new Dictionary<ProductionRuleProjectionContext, ProductionRuleLookAheadBucket>();
            ProductionRuleLookAheadBucket result;
            if (!this.automationBuckets.TryGetValue(bucketOwner, out result))
                this.automationBuckets.Add(bucketOwner, result = new ProductionRuleLookAheadBucket() { Owner = bucketOwner, BucketID = this.automationBuckets.Count + 1 });
            if (!bucketUseCount.ContainsKey(result))
                bucketUseCount.Add(result, 0);
            bucketUseCount[result]++;
            return result;
        }

        internal void DefineBucketOnTarget(ProductionRuleProjectionContext bucketReferencer)
        {
            if (this.locallyDefinedBuckets == null)
                this.locallyDefinedBuckets = new Dictionary<ProductionRuleLookAheadBucket, List<ProductionRuleProjectionContext>>();
            var bucket = this.RootAdapter.AssociatedContext.CreateBucket(this);
            List<ProductionRuleProjectionContext> sourcesWhichUseBucket;
            if (!this.locallyDefinedBuckets.TryGetValue(bucket, out sourcesWhichUseBucket))
                this.locallyDefinedBuckets.Add(bucket, sourcesWhichUseBucket = new List<ProductionRuleProjectionContext>());
            if (!sourcesWhichUseBucket.Contains(bucketReferencer))
                sourcesWhichUseBucket.Add(bucketReferencer);
            bucketReferencer.ReferenceBucketFromTarget(this, bucket);
        }

        internal void ReferenceBucketFromTarget(ProductionRuleProjectionContext context, ProductionRuleLookAheadBucket bucket)
        {
            if (this.referencedBuckets == null)
                this.referencedBuckets = new Dictionary<ProductionRuleLookAheadBucket, ProductionRuleProjectionContext>();
            if (!this.referencedBuckets.ContainsKey(bucket))
                this.referencedBuckets.Add(bucket, context);
        }


        public IEnumerable<ProductionRuleLookAheadBucket> FullAutomationBuckets
        {
            get
            {
                if (this.RootAdapter.AssociatedContext.automationBuckets == null)
                    this.RootAdapter.AssociatedContext.automationBuckets = new Dictionary<ProductionRuleProjectionContext, ProductionRuleLookAheadBucket>();
                foreach (var bucket in this.RootAdapter.AssociatedContext.automationBuckets.Values)
                    yield return bucket;
            }
        }
        public IEnumerable<ProductionRuleLookAheadBucket> ReferencedBuckets
        {
            get
            {
                if (this.referencedBuckets == null)
                    referencedBuckets = new Dictionary<ProductionRuleLookAheadBucket, ProductionRuleProjectionContext>();
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

        public ControlledDictionary<SyntacticalDFAState, ProductionRuleProjectionAdapter> StateAdapterLookup { get; set; }

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
                    let decision = s.Item1 as ProductionRuleProjectionDecision
                    where decision != null
                    let symbolicDecision = decision.DecidingFactor.SymbolicBreakdown(this.AssociatedCompiler)
                    where symbolicDecision.Rules.Count > 0
                    let firstRule = symbolicDecision.Rules.Values.First()
                    let decisiveAdapter = this.AssociatedCompiler.AllRuleAdapters[firstRule.Rule, firstRule.DFAState]
                    where decisiveAdapter.AssociatedContext.Node.ContainsKey(rootNode.Rule)
                    select 1).Any();
        }
    }
}
