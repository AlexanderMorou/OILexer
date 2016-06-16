using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Utilities.Arrays;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.FlowAnalysis;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using System.Runtime.CompilerServices;


namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    partial class ParserBuilder
    {
        private const string includeRuleContextName = "includeRuleContext";
        private const string nonGreedyName = "nonGreedyState";
        private const string lastPredictionResultName = "lastPredictionResult";
        private MultikeyedDictionary<PredictionTreeLeaf, GrammarVocabulary, IIntermediateClassMethodMember, PredictionTreeFollow[]> followDiscriminatorContext = new MultikeyedDictionary<PredictionTreeLeaf, GrammarVocabulary, IIntermediateClassMethodMember, PredictionTreeFollow[]>();
        private int _predictionCount;
        #if FALSE
        private void BuildCollapsePointRule(ProductionRuleNormalAdapter adapter, IIntermediateClassMethodMember parseMethod, IIntermediateMethodParameterMember<Abstract.Members.IClassMethodMember, IIntermediateClassMethodMember, IClassType, IIntermediateClassType> lookAheadParam, ITypedLocalMember localContext)
        {
            /* Collapse point rules should NEVER have follow predictions. */
            /* Since we know these rules can ONLY be a single symbol as a result, we can take a few shortcuts. */
            parseMethod.Assign(this._StateImpl.GetReference(), adapter.AssociatedState.StateValue.ToPrimitive());
            if (adapter.AssociatedContext.RequiresProjection)
            {
                var node = this.Compiler.AllProjectionNodes[adapter.AssociatedState];
                bool recursive = adapter.AssociatedContext.Node.Value.LeftRecursionType != ProductionRuleLeftRecursionType.None;
                IParameterMember leftRecursiveState = null;
                //if (recursive)
                //    leftRecursiveState = parseMethod.Locals.Add(new TypedName("enableRulePath", RuntimeCoreType.Boolean, this._identityManager));
                var collapseAdapter = this.Compiler.AdvanceMachines[node];
                var currentSwitch = parseMethod.Switch(GetPredictionInvokeExpression((IMemberReferenceExpression)lookAheadParam.GetReference(), collapseAdapter, leftRecursiveState));//predictMethod.GetReference().Invoke(lookAheadParam.GetReference()).RightComment(string.Format("State {0}", adapter.AssociatedState.StateValue)));
                parseMethod.Call(this.compiler.RuleSymbolBuilder.DoReductionImpl.GetReference(localContext.GetReference()).Invoke(this._SymbolStreamImpl.GetReference(), lookAheadParam.GetReference(), 1.ToPrimitive()));
                foreach (var targetTransition in adapter.OutgoingTransitions.Keys)
                {
                    ProductionRuleNormalAdapter targetAdapter = adapter.OutgoingTransitions[targetTransition];
                    var currentStateCase = currentSwitch.Case(targetAdapter.AssociatedState.StateValue.ToPrimitive().LeftComment(targetTransition.ToString()));
                    //currentStateCase.Comment();
                    var ruleVariant = targetTransition.GetRuleVariant();
                    Debug.Assert(ruleVariant.TrueCount <= 1, "There were more than one rules targeted by the transition!");
                    if (ruleVariant.TrueCount > 0)
                    {
                        var subParseMethod = parseInternalMethods[this.Compiler.RuleAdapters[((IGrammarRuleSymbol)ruleVariant.GetSymbols()[0]).Source]];
                        currentStateCase.Call(subParseMethod.GetReference().Invoke(lookAheadParam.GetReference()));
                    }
                }
                var yieldError = currentSwitch.Case(true);
                yieldError.Call(this.compiler.RuleSymbolBuilder.DoReductionImpl.GetReference(localContext.GetReference()).Invoke(this._SymbolStreamImpl.GetReference(), lookAheadParam.GetReference(), (1).ToPrimitive()));
                yieldError.Call(this.compiler.RuleSymbolBuilder.DelineateCapture.GetReference(localContext.GetReference()).Invoke("__HasError".ToPrimitive(), IntermediateGateway.TrueValue));

            }
            else
            {
                var dfaState = adapter.AssociatedState;
                var rule = adapter.AssociatedContext.Rule;
                var node = this.Compiler.AllProjectionNodes[adapter.AssociatedState];
                var currentSwitch = parseMethod.Switch(this.SymbolStreamBuilder.LookAheadMethodImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(lookAheadParam.GetReference()).RightComment(string.Format("State {0}", adapter.AssociatedState.StateValue)));
                parseMethod.Call(this.compiler.RuleSymbolBuilder.DoReductionImpl.GetReference(localContext.GetReference()).Invoke(this._SymbolStreamImpl.GetReference(), lookAheadParam.GetReference(), 1.ToPrimitive()));
                /* Breakdown the look-ahead table by the original targets that they were supposed to target.  There shouldn't be overlap, if there is, something's seriously wrong (with my code)! */
                bool singleTarget = adapter.AssociatedState.OutTransitions.Count == 1;
                var transitionsGroupedByOriginalTarget =
                    (from transitionKey in node.LookAhead.Keys
                     let decisionPathSet = node.LookAhead[transitionKey]
                     let originalTransition =
                        singleTarget
                            ? dfaState.OutTransitions.Keys.First()
                            : decisionPathSet.ProjectedRootTransition
                     let transitionTarget =
                        singleTarget
                            ? this.Compiler.AllProjectionNodes[dfaState.OutTransitions.Values.First()]
                            : decisionPathSet.ProjectedRootTarget
                     where Assert(string.Format("Parse{0}", rule.Name), transitionKey, node, originalTransition, transitionTarget)
                     group transitionKey
                     by new
                     {
                         TargetState = transitionTarget,
                         TransitionKey = originalTransition
                     }).ToDictionary(k => new
                     {
                         Target = k.Key.TargetState,
                         TransitionKey = k.Key.TransitionKey.SymbolicBreakdown(this.compiler),
                     }, v =>
                     {
                         var aggregate = v.Aggregate(GrammarVocabulary.UnionAggregateDelegate);
                         var tokens = aggregate.GetTokenVariant().SymbolicBreakdown(this.compiler);
                         var rules = aggregate.GetRuleVariant().SymbolicBreakdown(this.compiler);
                         return new { Rules = rules, Tokens = tokens };
                     });
                foreach (var transitionKeyInfo in transitionsGroupedByOriginalTarget.Keys)
                {
                    var detail = transitionsGroupedByOriginalTarget[transitionKeyInfo];

                    if (detail.Tokens.Tokens.Count > 0 || detail.Tokens.Ambiguities.Count > 0)
                    {
                        var currentCase = currentSwitch.Case(
                            detail.Tokens.Tokens.Values
                            .Select(tokDet => (IExpression)tokDet.Identity.GetReference())
                            .Concat(
                                detail.Tokens.Ambiguities.Values
                                .Select(ambigDet => (IExpression)ambigDet.Identity.GetReference())).ToArray());
                        HandleCollapsePointSingleRuleTarget(transitionKeyInfo.Target.Value.OriginalState, lookAheadParam, currentCase, transitionKeyInfo.TransitionKey, methodInvocationHandlingType: ParserRuleHandlingType.AsRule);
                    }
                    if (detail.Rules.Rules.Count > 0)
                    {
                        var currentCase = currentSwitch.Case(
                            detail.Rules.Rules.Values.Select(ruleDet => (IExpression)ruleDet.Identity.GetReference()).ToArray());
                        HandleCollapsePointSingleRuleTarget(transitionKeyInfo.Target.Value.OriginalState, lookAheadParam, currentCase, transitionKeyInfo.TransitionKey, methodInvocationHandlingType: ParserRuleHandlingType.AsSymbol);
                    }
                }
                var yieldError = currentSwitch.Case(true);
                yieldError.Call(this.compiler.RuleSymbolBuilder.DoReductionImpl.GetReference(localContext.GetReference()).Invoke(this._SymbolStreamImpl.GetReference(), lookAheadParam.GetReference(), (1).ToPrimitive()));
                yieldError.Call(this.compiler.RuleSymbolBuilder.DelineateCapture.GetReference(localContext.GetReference()).Invoke("__HasError".ToPrimitive(), IntermediateGateway.TrueValue));

            }
        }
        #endif

        private void HandleCollapsePointSingleRuleTarget(SyntacticalDFAState dfaState, IIntermediateMethodParameterMember<Abstract.Members.IClassMethodMember, IIntermediateClassMethodMember, IClassType, IIntermediateClassType> lookAheadParam, ISwitchCaseBlockStatement currentCase, GrammarVocabularySymbolicBreakdown origIdents, ParserRuleHandlingType methodInvocationHandlingType)
        {
            if (origIdents.Rules.Count > 1)
                Debug.Assert(false, "Error: rule Collapse point detection failure.");
            else if (origIdents.Rules.Count == 1)
            {
                var ruleDet = origIdents.Rules.Values.First();
                if (methodInvocationHandlingType == ParserRuleHandlingType.AsRule)
                    currentCase.Call(ruleDet.InternalParseMethod.GetReference().Invoke(lookAheadParam.GetReference()));
                else
                    currentCase.Comment(string.Format("No-op, {0} was reduced onto the stack.", origIdents.Rules.Values.First().Rule.Name));
            }
        }


        internal void BuildParser()
        {
            this.BuildPeekInitialContextsFor();
            this.BuildBorrowOuterContext();
            var rulesContext = (from rule in this.Compiler.RuleDFAStates.Keys
                                orderby rule.Name
                                select (OilexerGrammarProductionRuleEntry)rule).ToArray();
            foreach (var rule in rulesContext)
                this.BuildRuleScaffolding(rule);
            int predictIndex = 0;
            var nodesNeedingDisambiguators =
                (from node in this.compiler.AllProjectionNodes.Values
                 where node.FollowAmbiguities.Count() > 0
                 select node).ToArray();
            int maxCount =
                this.compiler.AdvanceMachines.Count + 
                this.compiler.FollowAmbiguousNodes.Sum(k => k.FollowAmbiguities.Count()) + 
                nodesNeedingDisambiguators.Length;
            foreach (var advMachine in this.compiler.AdvanceMachines.Keys)
            {
                var machine = this.compiler.AdvanceMachines[advMachine];
                var predictMethod = CreatePredictParseMethod(advMachine, ++predictIndex, maxCount);
                predictMethod.AccessLevel = AccessLevelModifiers.Private;
                predictMethods.Add(machine, predictMethod);
            }

            if (this.compiler.FollowAmbiguousNodes != null && this.compiler.FollowAmbiguousNodes.Length > 0)
                foreach (var followAmbiguityNode in this.compiler.FollowAmbiguousNodes)
                    foreach (var followAmbiguity in followAmbiguityNode.FollowAmbiguities)
                    {
                        if (!followPredictMethods.ContainsKey(followAmbiguity.Adapter))
                        {
                            var predictMethod = CreateFollowPredictParseMethod(followAmbiguity, ++predictIndex, maxCount);
                            predictMethod.AccessLevel = AccessLevelModifiers.Private;
                            followPredictMethods.Add(followAmbiguity.Adapter, predictMethod);
                        }
                    }
            this._predictionCount = maxCount;
            foreach (var node in nodesNeedingDisambiguators)
                this.BuildDiscriminatorScaffolding(node, ++predictIndex, maxCount);
            bool firstPrediction = true;
            object firstPredictionLock = new object();
#if ParallelProcessing
            Parallel.ForEach(this.compiler.AdvanceMachines.Keys, advMachineNode =>
            {
#else
            foreach (var advMachineNode in this.compiler.AdvanceMachines.Keys)
#endif
                {

                    var projectionAdapter = this.Compiler.AdvanceMachines[advMachineNode];
                    var predictMethod = this.predictMethods[projectionAdapter];
                    bool isFirstPredictionCopy;
#if ParallelProcessing
                    lock (firstPredictionLock)
#endif
                    {
                        isFirstPredictionCopy = firstPrediction;
                        if (firstPrediction)
                            firstPrediction = false;
                    }
                    this.BuildProjection(projectionAdapter, predictMethod, advMachineNode, predictMethod.Parameters["laDepth"], isFirstPredictionCopy);
                }
#if ParallelProcessing
            });
#endif
            firstPrediction = true;
            HashSet<PredictionTreeDFAdapter> processedFollows = new HashSet<PredictionTreeDFAdapter>();
            if (this.compiler.FollowAmbiguousNodes != null && this.compiler.FollowAmbiguousNodes.Length > 0)
#if ParallelProcessing
                Parallel.ForEach(this.compiler.FollowAmbiguousNodes, followAmbiguityNode =>
                {
#else
                foreach (var followAmbiguityNode in this.compiler.FollowAmbiguousNodes)
#endif
                    foreach (var followAmbiguity in followAmbiguityNode.FollowAmbiguities)
                    {
                        if (processedFollows.Add(followAmbiguity.Adapter))
                        {
                            bool isFirstPrediction;
#if ParallelProcessing
                            lock (firstPredictionLock)
#endif
                                isFirstPrediction = firstPrediction;
                            var predictMethod = followPredictMethods[followAmbiguity.Adapter];
                            var predictAdapter = followAmbiguity.Adapter;
                            this.BuildProjection(followAmbiguity.Adapter, predictMethod, followAmbiguity.EdgeNode, predictMethod.Parameters["laDepth"], isFirstPrediction);
#if ParallelProcessing
                            lock (firstPredictionLock)
#endif
                                if (firstPrediction)
                                    firstPrediction = false;
                        }
                    }
#if ParallelProcessing
                });
#endif
            this._predictionCount -= nodesNeedingDisambiguators.Select(k => this.followDiscriminatorContext.FilterToDictionary(k).Count + 1).Sum();
            foreach (var node in nodesNeedingDisambiguators)
                this.BuildDiscriminatorBody(node);
#if ParallelProcessing
            Parallel.ForEach(rulesContext, rule =>
            {
#else
            foreach (var rule in rulesContext)
#endif
                this.BuildParseMethod(rule);
#if ParallelProcessing
            });
#endif
        }

        private void BuildBorrowOuterContext()
        {
            var borrowContextMethod = this.ParserClass.Methods.Add(
                new TypedName("BorrowOuterContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol),
                new TypedNameSeries(
                    new TypedName("laDepth", RuntimeCoreType.Int32, this._identityManager),
                    new TypedName("borrowedIdentity", this.compiler.SymbolStoreBuilder.Identities)));
            borrowContextMethod.AccessLevel = AccessLevelModifiers.Private;
            borrowContextMethod.Comment("We're going to look at the symbol on the stream and if it's not the identity, check deeper as long as they exist as one nested symbol only.\r\n-\r\nAvoid calling LookAhead which might mess up the stack");
            var laDepth = borrowContextMethod.Parameters["laDepth"];
            var identity = borrowContextMethod.Parameters["borrowedIdentity"];
            var currentIdentityCheck = borrowContextMethod.If(this.compiler.GenericSymbolStreamBuilder.CountImpl.GetReference(this._SymbolStreamImpl.GetReference()).Subtract(1).GreaterThanOrEqualTo(laDepth.GetReference()));
            var currentSymbol = currentIdentityCheck.Locals.Add(new TypedName("currentSymbol", this.compiler.CommonSymbolBuilder.ILanguageSymbol), this.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this._SymbolStreamImpl.GetReference(), laDepth.GetReference()));
            currentSymbol.AutoDeclare = false;
            currentIdentityCheck.DefineLocal(currentSymbol);
            var currentRule = currentIdentityCheck.Locals.Add(new TypedName("currentRule", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol), currentSymbol.GetReference().As(this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol));
            currentRule.AutoDeclare = false;
            currentIdentityCheck.DefineLocal(currentRule);
            
            var whileRuleNotNull = currentIdentityCheck.While(currentRule.InequalTo(IntermediateGateway.NullValue));
            whileRuleNotNull.If(this.compiler.CommonSymbolBuilder.Identity.GetReference(currentRule.GetReference()).EqualTo(identity.GetReference()))
                .Return(currentRule.GetReference());
            var identityEqualToOne = whileRuleNotNull.If(this.compiler.RuleSymbolBuilder.Count.GetReference(currentRule.GetReference()).EqualTo(1));
            identityEqualToOne.CreateNext();
            identityEqualToOne.Next.Break();
            identityEqualToOne.Assign(currentSymbol.GetReference(), this.compiler.RuleSymbolBuilder.Indexer.GetReference(currentRule.GetReference(), IntermediateGateway.NumberZero));
            identityEqualToOne.Assign(currentRule.GetReference(), currentSymbol.GetReference().As(this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol));
            borrowContextMethod.Return(IntermediateGateway.NullValue);
            this.BorrowOuterContext = borrowContextMethod;
        }

        private void BuildPeekInitialContextsFor()
        {
            var lookAheadDepths = this.ParserClass.Fields.Add(new TypedName("_lookAheadDepths", ((IClassType)this._identityManager.ObtainTypeReference(typeof(Stack<>))).MakeGenericClosure(this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32))));
            lookAheadDepths.InitializationExpression = lookAheadDepths.FieldType.GetNewExpression();
            lookAheadDepths.AccessLevel = AccessLevelModifiers.Private;
            this._LookAheadDepthsImpl = lookAheadDepths;
            var peekInitialContextsForOvr = this.ParserClass.Methods.Add(
                new TypedName("PeekStackForInitialContext", RuntimeCoreType.Boolean, this._identityManager),
                new TypedNameSeries(
                    new TypedName("identity", this.compiler.SymbolStoreBuilder.Identities)));
            var peekInitialContextsFor = this.ParserClass.Methods.Add(
                new TypedName("PeekStackForInitialContext", RuntimeCoreType.Boolean, this._identityManager),
                new TypedNameSeries(
                    new TypedName("identity", this.compiler.SymbolStoreBuilder.Identities),
                    new TypedName("laDepth", RuntimeCoreType.Int32, this._identityManager)));
            peekInitialContextsForOvr.Return(peekInitialContextsFor.GetReference().Invoke(peekInitialContextsForOvr.Parameters["identity"].GetReference(), (-1).ToPrimitive()));
            var identity = peekInitialContextsFor.Parameters["identity"];
            var laDepth = peekInitialContextsFor.Parameters["laDepth"];

            var currentContext =
                peekInitialContextsFor.Locals.Add(
                    new TypedName("currentContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol),
                    this._CurrentContextImpl.GetReference());
            var enumeratorLocal = 
                peekInitialContextsFor.Locals.Add(
                    new TypedName("laDepthEnum", ((IInterfaceType)(this._identityManager.ObtainTypeReference(typeof(IEnumerator<>)))).MakeGenericClosure(this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32))), lookAheadDepths.GetReference().GetMethod("GetEnumerator").Invoke());
            enumeratorLocal.AutoDeclare = false;
            var stackLADepthCheck = new CSharpConditionalExpression(
                (ICSharpLogicalOrExpression)lookAheadDepths.GetReference().GetProperty("Count").GreaterThan(IntermediateGateway.NumberZero.RightComment("If we start at a left-recursive rule...")).AffixTo(CSharpOperatorPrecedences.LogicalOrOperation),
                (ICSharpConditionalExpression)lookAheadDepths.GetReference().GetMethod("Peek").Invoke().AffixTo(CSharpOperatorPrecedences.ConditionalOperation), 
                (ICSharpConditionalExpression)IntermediateGateway.NumberZero.RightComment("...there would be nothing to peek.").AffixTo(CSharpOperatorPrecedences.ConditionalOperation));
            var laDepthCheck = new CSharpConditionalExpression(
                (ICSharpLogicalOrExpression)(laDepth.InequalTo(-1).AffixTo(CSharpOperatorPrecedences.LogicalOrOperation)),
                (ICSharpConditionalExpression)(laDepth.GetReference().AffixTo(CSharpOperatorPrecedences.ConditionalOperation)),
                stackLADepthCheck);
            var entryLADepth = 
                peekInitialContextsFor.Locals.Add(
                    new TypedName("initialLADepth", RuntimeCoreType.Int32, this._identityManager),
                    laDepthCheck);
            var enumUsing = peekInitialContextsFor.Using(enumeratorLocal.GetDeclarationStatement());
            
            var whileMovingAndNonNull =
                enumUsing.While(enumeratorLocal.GetReference().GetMethod("MoveNext").Invoke().BitwiseAnd(currentContext.GetReference().InequalTo(IntermediateGateway.NullValue)));
            var currentDepth = whileMovingAndNonNull.Locals.Add(new TypedName("currentLADepth", RuntimeCoreType.Int32, this._identityManager), enumeratorLocal.GetReference().GetProperty("Current"));
            currentDepth.AutoDeclare = false;
            whileMovingAndNonNull.DefineLocal(currentDepth);
            whileMovingAndNonNull
                .If(entryLADepth.GetReference().InequalTo(currentDepth.GetReference()))
                    .Return(IntermediateGateway.FalseValue);
            whileMovingAndNonNull.If(this.Compiler.CommonSymbolBuilder.Identity.GetReference(currentContext.GetReference()).EqualTo(identity.GetReference()))
                .Return(IntermediateGateway.TrueValue);
            whileMovingAndNonNull.Assign(currentContext.GetReference(), this.Compiler.RuleSymbolBuilder.Parent.GetReference(currentContext.GetReference()));
            peekInitialContextsFor.Return(IntermediateGateway.FalseValue);
            peekInitialContextsFor.AccessLevel = AccessLevelModifiers.Private;
            this.PeekInitialContextsForImpl = peekInitialContextsFor;
            this.PeekInitialContextsForImplOvr = peekInitialContextsForOvr;
        }

        private void BuildDiscriminatorBody(PredictionTreeLeaf node)
        {
            var discriminatorDetails =
                this.followDiscriminatorContext
                    .FilterToDictionary(node);
            var predictMethods =
                (from ksvp in discriminatorDetails
                 select ksvp.Keys.Key2).Distinct().ToArray();
            var equivalentMethod = new Dictionary<IIntermediateClassMethodMember, List<IIntermediateClassMethodMember>>();
            var inverseLookup = new Dictionary<IIntermediateClassMethodMember, IIntermediateClassMethodMember>();
            HashSet<IIntermediateClassMethodMember> toIgnore = new HashSet<IIntermediateClassMethodMember>();
            var walker = new DiscriminatorEquivalenceWalker();
            foreach (var predictMethod in predictMethods)
            {
                if (toIgnore.Contains(predictMethod))
                    continue;
                foreach (var predictMethodAlt in predictMethods.Except(toIgnore.Concat(new[] { predictMethod })).ToArray())
                {
                    if (walker.Visit(predictMethod, predictMethodAlt))
                    {
                        toIgnore.Add(predictMethodAlt);
                        List<IIntermediateClassMethodMember> currentSet;
                        if (!equivalentMethod.TryGetValue(predictMethod, out currentSet))
                            equivalentMethod.Add(predictMethod, currentSet = new List<IIntermediateClassMethodMember>());
                        currentSet.Add(predictMethodAlt);
                        predictMethod.RemarksText = string.Format("{0} discriminators merged into one.", currentSet.Count + 1);
                        inverseLookup.Add(predictMethodAlt, predictMethod);
                    }
                }
            }
            
            var discriminatorIdentifiers =
                new Dictionary<IIntermediateClassMethodMember, int>();
            int methodOffset = 1;
            foreach (var predictionMethod in predictMethods)
                discriminatorIdentifiers.Add(predictionMethod, methodOffset++);
            var discriminatorMethod = this.followDiscriminatorMethods[node];
            var lookAheadDepth = discriminatorMethod.Parameters["laDepth"];
            BuildParseForestInRegion(discriminatorMethod, node.Veins.DFAOriginState);
            var discriminatorSwitch = GetLookAheadSwitch(discriminatorMethod, lookAheadDepth);
            var vocabs = discriminatorDetails.Select(k => k.Keys.Key1).Distinct();
            var predictionSelector = discriminatorMethod.Locals.Add(new TypedName("predictionSelector", RuntimeCoreType.Int32, this._identityManager), IntermediateGateway.NumberZero);
            var currentContext = discriminatorMethod.Locals.Add(new TypedName("currentContext", this.Compiler.RuleSymbolBuilder.ILanguageRuleSymbol), this._CurrentContextImpl.GetReference());
            var fullLookup = new Dictionary<IIntermediateClassMethodMember, List<PredictionTreeBranch>>();
            foreach (var vocab in vocabs)
            {
                // Obtain a dictionary from the follow context that denotes the paths that lead to this ambiguity
                var invertedContext = DistributeEpsilonData(discriminatorDetails, vocab);
                var standardContext =
                    (from kvp in invertedContext
                     group kvp.Key by kvp.Value).ToDictionary(k => k.Key, v => v.ToArray());
                //var standardContext = invertedContext.ToDictionary(k => k.Value, v => v.Key);
                foreach (var element in standardContext.Keys)
                {
                    List<PredictionTreeBranch> pathSets;
                    if (!fullLookup.TryGetValue(element, out pathSets))
                        fullLookup.Add(element, pathSets = new List<PredictionTreeBranch>());
                    pathSets.AddRange(standardContext[element]);
                }
                /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
                 * Once that's handled, we'll create a walkable tree that we can use to build the switch that will *
                 * check the context stack repeatedly until all known ambiguous path nodes have been exhausted, if *
                 * no prediction method is hit during this, there is no follow ambiguity.                          *
                 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
                var walkableTree = CreateWalkableDeterminationTree(invertedContext);
                var symbolicVocabulary = vocab.SymbolicBreakdown(this.compiler);
                var caseIdentities =
                    symbolicVocabulary.Tokens.Values
                    .Select(t => t.Identity.GetReference())
                    .Concat(symbolicVocabulary.Rules.Values
                    .Select(r => r.Identity.GetReference()))
                    .Concat(symbolicVocabulary.Ambiguities.Values
                    .Select(a => a.Identity.GetReference())).Cast<IExpression>().ToArray();
                var currentCase = discriminatorSwitch.Case(caseIdentities);
                BuildDiscriminatorBodyStep((IBlockStatementParent)currentCase, walkableTree, walkableTree.Value, currentContext, predictionSelector, discriminatorIdentifiers, standardContext);
            }
            var yieldSwitch = discriminatorMethod.Switch(predictionSelector.GetReference());
            
            foreach (var predictionMethod in discriminatorIdentifiers.Keys)
            {
                var id = discriminatorIdentifiers[predictionMethod];
                var currentCase = yieldSwitch.Case(id.ToPrimitive());
                var pathBuilder = new StringBuilder();
                bool first = true;
                foreach (var prediction in fullLookup[predictionMethod])
                {
                    if (first)
                        first = false;
                    else
                        pathBuilder.AppendLine();
                    pathBuilder.Append(prediction.ToString());
                }
                currentCase.Comment(pathBuilder.ToString());
                var predictionTarget = predictionMethod;
                if (inverseLookup.ContainsKey(predictionMethod))
                    predictionTarget = inverseLookup[predictionMethod];
                currentCase.Return(predictionTarget.GetReference().Invoke(lookAheadDepth.GetReference()));

            }

            foreach (var predictMethod in predictMethods.Except(toIgnore))
                predictMethod.Name = string.Format(string.Format("_Predict{{0:{0}}}Following{{1}}", new string('0', this._predictionCount.ToString().Length)), ++this._predictionCount, node.Rule.Name);
            discriminatorMethod.Name = string.Format("_Predict{0}DiscriminatorFollowing{1}", string.Format(string.Format("{{0:{0}}}", new string('0', this._predictionCount.ToString().Length)), ++this._predictionCount), node.Rule.Name);
            foreach (var ignored in toIgnore)
                ignored.Parent.Methods.Remove(ignored.UniqueIdentifier);
            yieldSwitch.Case(true).Return(IntermediateGateway.NumberZero);
        }

        private ISwitchStatement BuildDiscriminatorBodyStep(
            IBlockStatementParent statementTarget,
            KeyedTreeNode<PredictionTreeLeaf, IIntermediateClassMethodMember> discriminatorNode,
            IIntermediateClassMethodMember currentPrediction,
            ITypedLocalMember currentContext,
            ITypedLocalMember predictionSelector,
            Dictionary<IIntermediateClassMethodMember, int> discriminatorIdentifiers,
            Dictionary<IIntermediateClassMethodMember, PredictionTreeBranch[]> pathLookup)
        {
            var currentSubswitch = statementTarget.Switch(this.compiler.RuleSymbolBuilder.InitialState.GetReference(currentContext.GetReference()));
            foreach (var predictionNode in discriminatorNode.Keys)
            {
                
                var predictionNext = discriminatorNode[predictionNode];
                var currentAltPrediction = predictionNext.Value ?? currentPrediction;
                var predictionNodeCase = currentSubswitch.Case(predictionNode.Veins.DFAOriginState.StateValue.ToPrimitive());
                if (predictionNext.Count > 0)
                {

                    var ifNullCheck = predictionNodeCase.If(this.compiler.RuleSymbolBuilder.Parent.GetReference(currentContext.GetReference()).InequalTo(IntermediateGateway.NullValue));
                    ifNullCheck.Assign(currentContext.GetReference(), this.compiler.RuleSymbolBuilder.Parent.GetReference(currentContext.GetReference()));
                    var innerSwitch =
                        BuildDiscriminatorBodyStep(
                            (IBlockStatementParent)ifNullCheck, 
                            predictionNext, 
                            currentAltPrediction,
                            currentContext, 
                            predictionSelector, 
                            discriminatorIdentifiers, 
                            pathLookup);
                    if (currentAltPrediction != null)
                        AssignPredictionSelector(innerSwitch.Case(true), predictionSelector, discriminatorIdentifiers, currentAltPrediction, pathLookup);
                    ifNullCheck.CreateNext();
                    if (currentAltPrediction != null)
                        AssignPredictionSelector(ifNullCheck.Next, predictionSelector, discriminatorIdentifiers, currentAltPrediction, pathLookup);
                }
                else if (currentAltPrediction != null)
                    AssignPredictionSelector(predictionNodeCase, predictionSelector, discriminatorIdentifiers, currentAltPrediction, pathLookup);
            }
            return currentSubswitch;
        }

        private static void AssignPredictionSelector(IBlockStatementParent statementTarget, ITypedLocalMember predictionSelector, Dictionary<IIntermediateClassMethodMember, int> discriminatorIdentifiers, IIntermediateClassMethodMember currentAltPrediction,
            Dictionary<IIntermediateClassMethodMember, PredictionTreeBranch[]> pathLookup)
        {
            StringBuilder pathComment = new StringBuilder();
            pathComment.AppendFormat("{0}: ", currentAltPrediction.Name);

            bool first=true;

            foreach (var path in pathLookup[currentAltPrediction])
            {
                if (first)
                    first = false;
                else
                    pathComment.AppendLine();
                pathComment.Append(path);
            }
            statementTarget.Comment(pathComment.ToString());//string.Format("{0}: {1}", currentAltPrediction.Name, pathLookup[currentAltPrediction]));
            statementTarget.Assign(predictionSelector.GetReference(), discriminatorIdentifiers[currentAltPrediction].ToPrimitive());
        }

        public KeyedTreeNode<PredictionTreeLeaf, IIntermediateClassMethodMember> CreateWalkableDeterminationTree(Dictionary<PredictionTreeBranch, IIntermediateClassMethodMember> inversionContext)
        {
            var root = new KeyedTreeNode<PredictionTreeLeaf, IIntermediateClassMethodMember>();
            foreach (var predictionPath in inversionContext.Keys)
            {
                var currentTreeNode = root;
                var targetPrediction = inversionContext[predictionPath];
                //Depth is zero-based, so taking depth will omit the common top-level node.
                var choppedInvertedPath = predictionPath.Take(predictionPath.Depth).Reverse().ToArray();

                foreach (var predictionNode in choppedInvertedPath)
                {
                    if (!currentTreeNode.ContainsKey(predictionNode))
                        currentTreeNode._Add(predictionNode, new KeyedTreeNode<PredictionTreeLeaf, IIntermediateClassMethodMember>());
                    currentTreeNode = currentTreeNode[predictionNode];
                }
                //At the end of the trail, we need to specify what method to call, if any.
                Debug.Assert(currentTreeNode.Value == null, "Prediction Failure: Non-deterministic follow prediction.");
                currentTreeNode.Value = targetPrediction;
            }
            return root;
        }

        private static Dictionary<PredictionTreeBranch, IIntermediateClassMethodMember> DistributeEpsilonData(IMultikeyedDictionary<GrammarVocabulary, IIntermediateClassMethodMember, PredictionTreeFollow[]> discriminatorDetails, GrammarVocabulary vocab)
        {
            var vocabContext = discriminatorDetails.FilterToDictionary(vocab);
            return (from predictMethod in vocabContext.Keys
                    let followSetForMethod = vocabContext[predictMethod]
                    from predictFollowDetail in followSetForMethod
                    from predictionTopLevelPath in predictFollowDetail.InitialPaths.GetEpsilonLevel(0)
                    select new { Key = predictionTopLevelPath, Value = predictMethod }).ToDictionary(k => k.Key, v => v.Value);
        }

        private void BuildProjection(
            PredictionTreeDFAdapter projectionAdapter,
            IIntermediateClassMethodMember predictMethod,
            PredictionTreeLeaf originatingNode,
            IIntermediateMethodParameterMember<IClassMethodMember, IIntermediateClassMethodMember, IClassType, IIntermediateClassType>
                                            lookAheadParam,
            bool firstPrediction)
        {
            var flowGraph = SyntacticalDFAFlowGraph.CreateFlowGraph(projectionAdapter.AssociatedState);
            var prExitLADepth = predictMethod.Locals.Add(new TypedName("exitLADepth", RuntimeCoreType.Int32, this._identityManager), (-1).ToPrimitive());
            var prCurrentLADepth = predictMethod.Locals.Add(new TypedName("currentLADepth", RuntimeCoreType.Int32, this._identityManager), lookAheadParam.GetReference());
            var prExitState = predictMethod.Locals.Add(new TypedName("exitState", RuntimeCoreType.Int32, this._identityManager), (-1).ToPrimitive());
            var maxStateIndex = flowGraph.PluralTargets.Concat(flowGraph.Singletons).Select(k => k.Value.StateValue).Max();
            var visited = new HashSet<SyntacticalDFAState>();
            var secondaryLookups = new Dictionary<IBlockStatementParent, Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>>();
            var remainingToProcess = new Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>>();
            var decisionPoint = new LabelStatement(predictMethod, "DecisionPoint");
            var predictionDetails = ObtainReductionDetails(projectionAdapter, flowGraph);
            this.ReducePredictionDetails(predictionDetails);
            var multitargetLookup =
                SyntacticalDFAStateJumpData.ObtainStateJumpDetails(
                    flowGraph.PluralTargets.OrderBy(p => p.Value.StateValue).Select(pt => pt.Value),
                    predictMethod,
                    "PredictMultiTargetState_{{0:{0}}}",
                    new string('0', maxStateIndex.ToString().Length),
                    state => predictionDetails.Any(k => k.Value.Any(k2 => k2.FinalState == state)),
                    flowGraph.Singletons.Select(st => st.Value));
            var reductionHandlers = new MultikeyedDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState, ILocalMember>();
            foreach (var ksvp in predictionDetails)
            {
                if (ksvp.Value.All(k => k.ReductionType == PredictionReduceType.Simple))
                    continue;
                reductionHandlers.Add(
                    ksvp.Keys.Key1,
                    ksvp.Keys.Key2,
                    GetPredictionReductionLocal(predictMethod, maxStateIndex, ksvp));

            }
            var localGroups =
                (from localDetail in reductionHandlers
                 group localDetail by localDetail.Keys.Key2).ToDictionary(k => k.Key, v => v.ToArray());
            foreach (var localState in localGroups.Keys)
            {
                var localGroup = localGroups[localState];
                var declStatement = localGroup.FirstOrDefault().Value.GetDeclarationStatement(localGroup.Skip(1).Select(v => v.Value).ToArray());
                predictMethod.Add(declStatement);
            }
            predictMethod.Assign(this._StateImpl.GetReference(), projectionAdapter.AssociatedState.StateValue.ToPrimitive());
            var entryPointReference = multitargetLookup[projectionAdapter.AssociatedState].Label.Value;
            //bool nonGreedyContext = false;
            //if (projectionAdapter.AssociatedContext.LeftRecursiveType != ProductionRuleLeftRecursionType.None)
            //{
            //    nonGreedyContext = true;
            //    predictMethod.Parameters.Add(new TypedName("nonGreedy", RuntimeCoreType.Boolean, this._identityManager));
            //}
            var includeRuleContext =
                (projectionAdapter.AssociatedContext.RequiresLeftRecursiveCaution)
                ? predictMethod.Parameters[includeRuleContextName]
                : null;
            //var nonGreedyParam = nonGreedyContext ? predictMethod.Parameters["nonGreedy"] : null;
            var projectionLookup = projectionAdapter.All.ToDictionary(k => k.AssociatedState, v => v);
            CommonStateMachineBodyBuilder(
                topLevelState =>
                    GenerateRuleProjectionParseState(remainingToProcess,
                    topLevelState.BlockContainer,
                    topLevelState.State,
                    decisionPoint,
                    secondaryLookups,
                    multitargetLookup,
                    visited,
                    prExitLADepth,
                    prCurrentLADepth,
                    prExitState, (OilexerGrammarProductionRuleEntry)originatingNode.Rule, reductionHandlers,
                    projectionLookup,
                    predictionDetails,
                    includeRuleContext,
                    topLevelState,
                    projectionAdapter.AssociatedContext.RequiresLeftRecursiveCaution,
                    projectionAdapter.AssociatedContext.LeftRecursiveType/*,
                    nonGreedyParam*/),
                currentToProcess =>
                    GenerateRuleProjectionParseState(remainingToProcess,
                    currentToProcess.Item2,
                    currentToProcess.Item1,
                    decisionPoint,
                    secondaryLookups,
                    multitargetLookup,
                    visited,
                    prExitLADepth,
                    prCurrentLADepth,
                    prExitState, (OilexerGrammarProductionRuleEntry)originatingNode.Rule, reductionHandlers,
                    projectionLookup,
                    predictionDetails,
                    includeRuleContext,
                    currentToProcess.Item3,
                    projectionAdapter.AssociatedContext.RequiresLeftRecursiveCaution,
                    projectionAdapter.AssociatedContext.LeftRecursiveType/*,
                    nonGreedyParam*/),
                multitargetLookup,
                remainingToProcess,
                secondaryLookups,
                predictMethod);
            predictMethod.Add(decisionPoint);
            var exitPointSwitch = predictMethod.Switch(prExitState.GetReference());

            var destinations = (from edge in projectionAdapter.AssociatedState.ObtainEdges().ToArray()
                                  let decision = (from source in edge.Sources.Select(k => k.Item1)
                                                  where source is PredictionTreeDestination
                                                  select (PredictionTreeDestination)source).FirstOrDefault()
                                  where decision != null
                                  group new { EdgeState = edge, Decision = decisionPoint } by new { decision.DecidingFactor, decision.Target }).ToDictionary(k => k.Key, v => v.ToArray());

            var followCallerPoints = (from edge in projectionAdapter.AssociatedState.ObtainEdges().ToArray()
                              let decision = (from source in edge.Sources.Select(k => k.Item1)
                                              where source is PredictionTreeFollowCaller
                                              select (PredictionTreeFollowCaller)source).FirstOrDefault()
                              where decision != null
                              group new { EdgeState = edge, Decision = decisionPoint } by decision.DecidingFactor ?? GrammarVocabulary.NullInst).ToDictionary(k => k.Key, v => v.ToArray());

            foreach (var decidingFactor in destinations.Keys)
            {
                var edges = destinations[decidingFactor];
                var currentCase = exitPointSwitch.Case(edges.Select(k => (IExpression)k.EdgeState.StateValue.ToPrimitive()).ToArray());
                currentCase.Return(decidingFactor.Target.Veins.DFAOriginState.StateValue.ToPrimitive().RightComment(decidingFactor.DecidingFactor.ToString().Replace("{", "{{").Replace("}", "}}")));
            }
            bool hasFollow = false;
            foreach (var failingFactor in followCallerPoints.Keys)
            {
                if(!hasFollow)
                    hasFollow = true;
                var edges = followCallerPoints[failingFactor];
                var currentCase = exitPointSwitch.Case(edges.Select(k => (IExpression)k.EdgeState.StateValue.ToPrimitive()).ToArray());
                currentCase.Return((-1).ToPrimitive().RightComment(string.Format("Prediction failure for {0}", failingFactor.ToString().Replace("{", "{{").Replace("}", "}}"))));
            }
            if (projectionAdapter.AssociatedContext.IsLeftRecursiveProjection && projectionAdapter.AssociatedContext.RequiresLeftRecursiveCaution && !projectionAdapter.AssociatedContext.RequiresInnerRecursionSwap)
                predictMethod.Parameters.Remove(predictMethod.Parameters[nonGreedyName]);
            if (hasFollow && firstPrediction)
                predictMethod.Return(IntermediateGateway.NumberZero.RightComment("The prediction mechanism isn't necessary, so the owning state's normal execution can continue."));
            else if (firstPrediction)
            {
                predictMethod.Comment(
@"The prediction did not match a valid alt, so yield the longest look-ahead point as a negative to avoid accidentally looking like a viable alt.
-
This will ensure that the Expect function can identify the proper point of failure.");
                predictMethod.Return(prCurrentLADepth.GetReference().Negate());
            }
            else if (!hasFollow)
                predictMethod.Return(prCurrentLADepth.GetReference().Negate());
            else
                predictMethod.Return(IntermediateGateway.NumberZero);

        }

        private void GenerateRuleProjectionParseState(
            Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>>
                                                toInsert,
            IBlockStatementParent parentTarget,
            SyntacticalDFAState dfaState,
            ILabelStatement decisionPoint,
            Dictionary<IBlockStatementParent, Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>>
                                                secondaryLookups,
            IDictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>
                                                multitargetLookups,
            HashSet<SyntacticalDFAState> visited,
            ILocalMember exitLADepth,
            ILocalMember currentLADepth,
            ILocalMember exitState,
            OilexerGrammarProductionRuleEntry rule,
            MultikeyedDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState, ILocalMember>
                                                reductionHandlers,
            Dictionary<SyntacticalDFAState, PredictionTreeDFAdapter>
                                                adapterLookup,
            PredictionReduceDetails predictionDetails,
            IParameterMember includeRuleContext,
            SyntacticalDFAStateJumpData jumpData,
            bool cautionaryLeftRecursion,
            ProductionRuleLeftRecursionType leftRecursiveAware/*,
            IParameterMember nonGreedyParam*/)
        {
            if (!visited.Add(dfaState))
                return;
            var adapter = adapterLookup[dfaState];
            IEnumerable<IGrammarRuleSymbol> reductionsToWatchFor = this.compiler._GrammarSymbols.GetRuleSymbols(reductionHandlers.Where(k => k.Keys.Key2 == dfaState).Select(k => k.Keys.Key1));
            //BuildParseForestInRegion(parentTarget, dfaState);
            if (dfaState.OutTransitions.Count > 0)
            {
                var currentSwitch = GetLookAheadSwitch(parentTarget, currentLADepth);
                HashSet<IGrammarAmbiguousSymbol> seenAmbiguities = new HashSet<IGrammarAmbiguousSymbol>();
                var identityBlocks = new Dictionary<IGrammarSymbol, ISwitchCaseBlockStatement>();

                foreach (var transition in dfaState.OutTransitions.Keys)
                {
                    var targetOfTransition = dfaState.OutTransitions[transition];

                    var symbolicBreakdown = transition.SymbolicBreakdown(this.compiler);
                    foreach (var ambiguity in symbolicBreakdown.Ambiguities.Values)
                        seenAmbiguities.Add(ambiguity.Symbol);
                    var currentCase = currentSwitch.Case(
                        symbolicBreakdown.Tokens.Values.Select(tokDet => (IExpression)tokDet.Identity.GetReference())
                        .Concat(symbolicBreakdown.Ambiguities.Values.Select(ambigDet => (IExpression)ambigDet.Identity.GetReference()))
                        .Concat(
                        symbolicBreakdown.Rules.Values.Select(ruleDet => (IExpression)ruleDet.Identity.GetReference()))
                        .ToArray());
                    foreach (var tokenDet in symbolicBreakdown.Tokens.Values)
                        identityBlocks.Add(tokenDet.Symbol, currentCase);

                    HandleNextProjectionParseState(targetOfTransition, currentCase, currentSwitch, transition, symbolicBreakdown, toInsert, parentTarget, dfaState, decisionPoint, secondaryLookups, multitargetLookups, visited, exitLADepth, currentLADepth, exitState, rule, reductionHandlers, adapterLookup, predictionDetails, includeRuleContext, jumpData, cautionaryLeftRecursion, leftRecursiveAware/*, nonGreedyParam*/);
                }
                var lexAmbig =
                    (from source in dfaState.Sources
                     where source.Item1 is PredictionTree
                     let decisionPathSet = (PredictionTree)source.Item1
                     from path in decisionPathSet
                     let pathNode = path.CurrentNode
                     where pathNode.LexicalAmbiguities != null
                     from ambiguity in pathNode.LexicalAmbiguities
                     select ambiguity).Distinct().Except(seenAmbiguities).ToArray();
                foreach (var t in lexAmbig)
                {
                    var transitionLocks = (from tki in dfaState.OutTransitions.Keys
                                           where !tki.Intersect(t.AmbiguityKey).IsEmpty
                                           select tki.SymbolicBreakdown(this.Compiler)).ToArray();
                    Debug.Assert(transitionLocks.Length <= 1, "Ambiguity Assertion Failure");
                    var firstLock = transitionLocks.FirstOrDefault();
                    if (firstLock == null)
                        continue;
                    var targetDetail = firstLock;
                    var firstTokIdent = targetDetail.Tokens.Values.First();
                    var newCase = currentSwitch.Case(compiler.LexicalSymbolModel.GetIdentitySymbolField(t).GetReference());
                    newCase.Add(identityBlocks[firstTokIdent.Symbol].GetGoTo(newCase));
                }
                var defaultCase = currentSwitch.Case(true);
                defaultCase.GoTo(decisionPoint);
            }
            else
                parentTarget.GoTo(decisionPoint);
        }

        private void BuildParseForestInRegion(IBlockStatementParent parentTarget, SyntacticalDFAState originState)
        {
            var comments = new HashSet<string>();
            foreach (var sourcePathSet in (from source in originState.Sources
                                            where source.Item1 is PredictionTree
                                            let dPathSet = (PredictionTree)source.Item1
                                            from path in dPathSet
                                            select path).Distinct())
                comments.Add(sourcePathSet.ToString(false));
            if (comments.Count > 0)
            {
                var eslsBegin = new ExplicitStringLiteralStatement(parentTarget) { Literal = "#region Parse trace" };
                parentTarget.Add(eslsBegin);
                foreach (var comment in comments)
                    parentTarget.Comment(comment);
                 var eslsEnd = new ExplicitStringLiteralStatement(parentTarget) { Literal = "#endregion" };
                parentTarget.Add(eslsEnd);
            }
        }

        private ISwitchStatement GetLookAheadSwitch(IBlockStatementParent parentTarget, ILocalMember currentLADepth)
        {
            return GetLookAheadSwitch(parentTarget, currentLADepth.GetReference());
        }

        private ISwitchStatement GetLookAheadSwitch(IBlockStatementParent parentTarget, IParameterMember currentLADepth)
        {
            return GetLookAheadSwitch(parentTarget, currentLADepth.GetReference());
        }
        private ISwitchStatement GetLookAheadSwitch(IBlockStatementParent parentTarget, IExpression currentLADepthRef)
        {
            return parentTarget.Switch(this.compiler.SymbolStreamBuilder.LookAheadMethod.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(currentLADepthRef));
        }

        private void HandleNextProjectionParseState(
            SyntacticalDFAState                    targetState,
            IBlockStatement                      currentTarget,
            ISwitchStatement                      owningSwitch,
            GrammarVocabulary                       transition,
            GrammarVocabularySymbolicBreakdown      origIdents,
            Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>>
                                                      toInsert,
            IBlockStatementParent                 parentTarget,
            SyntacticalDFAState                       dfaState,
            ILabelStatement                      decisionPoint,
            Dictionary<IBlockStatementParent, Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>>
                                              secondaryLookups,
            IDictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>
                                            multitargetLookups,
            HashSet<SyntacticalDFAState>               visited,
            ILocalMember                           exitLADepth,
            ILocalMember                        currentLADepth,
            ILocalMember                             exitState,
            OilexerGrammarProductionRuleEntry             rule,
            MultikeyedDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState, ILocalMember>
                                             reductionHandlers,
            Dictionary<SyntacticalDFAState, PredictionTreeDFAdapter>
                                                 adapterLookup,
            PredictionReduceDetails          predictionDetails,
            IParameterMember                includeRuleContext,
            SyntacticalDFAStateJumpData               jumpData,
            bool                       cautionaryLeftRecursion,
            ProductionRuleLeftRecursionType leftRecursiveAware/*,
            IParameterMember nonGreedyParam*/)
        {
            //var anyOther = (from s in targetState.Sources
            //                let decision = s.Item1 as PredictionTreeDestination
            //                where decision != null
            //                let symbolicDecision = decision.DecidingFactor.SymbolicBreakdown(this.compiler)
            //                where symbolicDecision.Rules.Count > 0
            //                let firstRule = symbolicDecision.Rules.Values.First()
            //                let decisiveAdapter = this.compiler.AllRuleAdapters[firstRule.Rule, firstRule.DFAState]
            //                where decisiveAdapter.AssociatedContext.Node.ContainsKey(rule)
            //                select 1).Any();
            BuildParseForestInRegion(currentTarget, targetState);

            bool handleLeftRecursiveState = false;
            GrammarVocabularyRuleDetail peekDetail = null;
            handleLeftRecursiveState =
                 (leftRecursiveAware == ProductionRuleLeftRecursionType.Direct ||
                  leftRecursiveAware == ProductionRuleLeftRecursionType.DirectAndHidden ||
                  leftRecursiveAware == ProductionRuleLeftRecursionType.DirectAndIndirect) &&
                (from s in targetState.Sources
                 let decision = s.Item1 as PredictionTreeDestination
                 where decision != null
                 let symbolicDecision = decision.DecidingFactor.SymbolicBreakdown(this.compiler)
                 where symbolicDecision.Rules.Count > 0
                 let firstRule = symbolicDecision.Rules.Values.First()
                 let decisiveAdapter = this.compiler.AllRuleAdapters[firstRule.Rule, firstRule.DFAState]
                 where decisiveAdapter.AssociatedContext.Leaf.ContainsKey(rule)
                 select 1).Any();
            bool handlePeekSwap = false;
            if (jumpData.EnclosureType == SyntacticalDFAStateEnclosureHandling.Undecided && !handleLeftRecursiveState && cautionaryLeftRecursion)
            {
                if (dfaState.IsEdge)
                {

                    var edgesFromTarget =
                      (from e in dfaState.ObtainEdges()
                       where e != dfaState
                       select e).ToArray();
                    var currentDecisions =
                        dfaState.Sources
                            .Select(k => k.Item1 as PredictionTreeDestination)
                            .Where(k => k != null);

                    var currentDecision = currentDecisions.FirstOrDefault();
                    if (currentDecision != null)
                    {
                        var peekDetails = (from edge in edgesFromTarget
                                           from detailGroup in GetEdgeLeftRecursiveDetail(edge, rule)
                                           from detail in detailGroup
                                           group detail by detailGroup.Key).ToDictionary(k => k.Key, v => v.Distinct().ToArray());

                        foreach (var decisivePointDetailSet in peekDetails.Keys)
                            foreach (var decisivePoint in peekDetails[decisivePointDetailSet])
                                decisivePoint.Item4.PossibleLeftRecursiveDecision = true;
                    }
                }
                {
                    var peekDetails = (from detailGroup in GetEdgeLeftRecursiveDetail(targetState, rule)
                                       from detail in detailGroup
                                       group detail by detailGroup.Key).ToDictionary(k => k.Key, v => v.Distinct().ToArray());
                    if (peekDetails.Count > 0)
                    {
                        handleLeftRecursiveState = peekDetails.Values.Any(k => k.Any(v => v.Item4.PossibleLeftRecursiveDecision));
                        handlePeekSwap = true;
                        peekDetail = peekDetails.Keys.FirstOrDefault();
                    }
                }
            }
            SyntacticalDFAStateEnclosureHandling enclosureState =
                jumpData.EnclosureType == SyntacticalDFAStateEnclosureHandling.Undecided
                    ? handleLeftRecursiveState
                        ? cautionaryLeftRecursion
                            ? SyntacticalDFAStateEnclosureHandling.Encapsulate
                            : SyntacticalDFAStateEnclosureHandling.Encapsulate
                        : handlePeekSwap 
                            ? SyntacticalDFAStateEnclosureHandling.EncapsulatePeek 
                            : SyntacticalDFAStateEnclosureHandling.Undecided
                    : jumpData.EnclosureType;
            //if (enclosureState == SyntacticalDFAStateEnclosureHandling.Undecided &&
            //    noAltLeftRecursion)
            //    enclosureState = SyntacticalDFAStateEnclosureHandling.EncapsulatePeek;
            if (enclosureState == SyntacticalDFAStateEnclosureHandling.Encapsulate && includeRuleContext != null)
            {
                var newTarget = currentTarget.If(includeRuleContext.GetReference());
                newTarget.CreateNext();
                newTarget.Next.GoTo(decisionPoint);
                currentTarget = newTarget;
            }
            else if (enclosureState == SyntacticalDFAStateEnclosureHandling.EncapsulatePeek && peekDetail != null)
            {
                var advRuleMachine = this.compiler.RuleDetail[rule].ProjectionAdapter;
                if (!advRuleMachine.AssociatedContext.RequiresInnerRecursionSwap)
                    advRuleMachine.AssociatedContext.RequiresInnerRecursionSwap = true;
                var newTarget = currentTarget.If(this.compiler.RuleDetail[rule].PredictMethod.Parameters[nonGreedyName].GetReference().LogicalAnd(PeekInitialContextsForImpl.GetReference().Invoke(peekDetail.Identity.GetReference(), currentLADepth.GetReference())));
                //newTarget.Next.ToString();
                //newTarget.CreateNext();
                newTarget.Comment("Attempt to resolve 'Chicken before egg' issue with some left recursive rules.");
                var swapSymbol = newTarget.Locals.Add(new TypedName("swap{0}", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol, rule.Name), this.BorrowOuterContext.GetReference().Invoke(currentLADepth.GetReference(), this.Compiler.RuleDetail[rule].Identity.GetReference()));
                swapSymbol.AutoDeclare = false;
                newTarget.DefineLocal(swapSymbol);
                var swapNotNullCheck = newTarget.If(swapSymbol.InequalTo(IntermediateGateway.NullValue));
                var currentSymbol = swapNotNullCheck.Locals.Add(new TypedName("currentSymbol", this.compiler.CommonSymbolBuilder.ILanguageSymbol), this.compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this._SymbolStreamImpl.GetReference(), currentLADepth.GetReference()));
                currentSymbol.AutoDeclare = false;
                swapNotNullCheck.DefineLocal(currentSymbol);
                var currentSymbolNotNull = swapNotNullCheck.If(currentSymbol.InequalTo(IntermediateGateway.NullValue));
                var explicitCaptureCheck = currentSymbolNotNull.If(this.compiler.RuleSymbolBuilder.GetExplicitCapture.GetReference(swapSymbol.GetReference(), this._identityManager.ObtainTypeReference(RuntimeCoreType.Boolean)).Invoke("__Swapped".ToPrimitive()).Not());
                explicitCaptureCheck.CreateNext();
                explicitCaptureCheck.Next.Return(IntermediateGateway.NumberZero);
                explicitCaptureCheck.Call(this.compiler.RuleSymbolBuilder.DelineateCapture.GetReference(swapSymbol.GetReference()).Invoke("__Swapped".ToPrimitive(), IntermediateGateway.TrueValue));
                explicitCaptureCheck.Call(this.Compiler.SymbolStreamBuilder.SwapImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(currentSymbol.GetReference(), swapSymbol.GetReference()));
                explicitCaptureCheck.Return((-1).ToPrimitive());
                //newTarget.Next.GoTo(decisionPoint);
                //currentTarget = newTarget.Next;
            }

            IEnumerable<IGrammarRuleSymbol> reductionsToWatchFor = this.compiler._GrammarSymbols.GetRuleSymbols(reductionHandlers.Where(k => k.Keys.Key2 == targetState).Select(k => k.Keys.Key1));
            bool emittedExitStateCheck = false;
            
            var reductionPoints = targetState.Sources.Where(k => k.Item1 is ProductionRuleProjectionReduction).Select(k => k.Item1).Cast<ProductionRuleProjectionReduction>().ToArray();
            
            var reductionLocalsToAssignContext = reductionHandlers.Where(k => k.Keys.Key2 == targetState);
            /* If the start/end state are identical, then there's no look-ahead variable needed. */
            var reductionLocalsJoinContext = from rl in reductionLocalsToAssignContext
                                             join pDet in predictionDetails on new { rl.Keys.Key1, rl.Keys.Key2 } equals new { pDet.Keys.Key1, pDet.Keys.Key2 }
                                             let elementQuery = from element in pDet.Value
                                                                where element.InitialState != element.FinalState
                                                                select element
                                             where elementQuery.Any()
                                             select rl;

            var reductionLocalsToAssign = reductionLocalsJoinContext.Select(k => k.Value).OrderByDescending(k => k.Name).ToArray();
            if (reductionLocalsToAssign.Length > 0)
                currentTarget.Add(new ExpressionStatement(currentTarget, ChainAssign(reductionLocalsToAssign, currentLADepth)));

            var reduction = reductionPoints.FirstOrDefault();
            if (reduction != null)
            {
                var firstSymbol = reduction.ReducedRule.SymbolicBreakdown(this.compiler);
                if (firstSymbol.Rules.Count == 1)
                {
                    var ruleDetails = firstSymbol.Rules.Values.First();
                    var secondaryPredict =
                        (from kvp in predictionDetails
                         where kvp.Keys.Key1 == ruleDetails.Rule
                         from element in kvp.Value
                         where element.FinalState == targetState
                         where !element.FinalDiscriminator.Intersect(transition).IsEmpty
                         select element).ToArray();
                    var localReductionContext = (from reductionHandler in reductionHandlers
                                                 where reductionHandler.Keys.Key1 == ruleDetails.Rule
                                                 join secondaryPredictElement in secondaryPredict
                                                     on reductionHandler.Keys.Key2 equals secondaryPredictElement.InitialState
                                                 where secondaryPredictElement.InitialState != secondaryPredictElement.FinalState
                                                 select reductionHandler.Value).Distinct().ToArray();

                    if (localReductionContext.Length > 0)
                    {
                        if (ruleDetails.Leaf.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.None &&
                            ruleDetails.Leaf.Count == 1)
                            currentTarget = currentTarget.If(PeekInitialContextsForImpl.GetReference().Invoke(ruleDetails.Identity.GetReference(), currentLADepth.GetReference()).Not());
                        var maxContext = BuildMaxContext(localReductionContext);
                        IExpression assignContext = currentLADepth.GetReference().Assign(this.CurrentIfOtherNegative.GetReference().Invoke(currentLADepth.GetReference(), (INaryOperandExpression)maxContext));
                        currentTarget.Assign(_FollowStateImpl.GetReference(), targetState.StateValue.ToPrimitive());
                        //if (ruleDetails == null  || ruleDetails.Rule == null )
                        //{

                        //}
                        currentTarget.Comment(string.Format("Reduce {0}.", ruleDetails.Rule.Name));
                        currentTarget.Call(ruleDetails.InternalParseMethod.GetReference().Invoke(assignContext));
                        //currentTarget.Assign(this._StateImpl.GetReference(), targetState.StateValue.ToPrimitive());
                        ExitStateCheck(currentTarget, dfaState, exitState, targetState, true/*, nonGreedyParam, decisionPoint*/);
                        emittedExitStateCheck = true;
                    }
                    else if (secondaryPredict.Length > 0)
                    {
                        secondaryPredict = secondaryPredict.Where(k => k.ReductionType == PredictionReduceType.Simple).ToArray();

                        if (secondaryPredict.Length > 0)
                        {
                            if (ruleDetails.Leaf.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.None &&
                                ruleDetails.Leaf.Count == 1)
                                currentTarget = currentTarget.If(PeekInitialContextsForImpl.GetReference().Invoke(ruleDetails.Identity.GetReference(), currentLADepth.GetReference()).Not());
                            currentTarget.Assign(_FollowStateImpl.GetReference(), targetState.StateValue.ToPrimitive());
                            if (ruleDetails == null || ruleDetails.Rule == null)
                            {

                            }
                            currentTarget.Comment(string.Format("Reduce {0}.", ruleDetails.Rule.Name));
                            currentTarget.Call(ruleDetails.InternalParseMethod.GetReference().Invoke(currentLADepth.GetReference()));
                            //currentTarget.Assign(this._StateImpl.GetReference(), targetState.StateValue.ToPrimitive());
                            ExitStateCheck(currentTarget, dfaState, exitState, targetState, true/*, nonGreedyParam, decisionPoint*/);
                            emittedExitStateCheck = true;
                        }
                        else
                        {
                            //http://www.messletters.com/en/big-text/
                            currentTarget.Comment(@"  " +//Let's hope this doesn't happen!
@"NNNNNNNN        NNNNNNNN      OOOOOOOOO       !!! 
  N:::::::N       N::::::N    OO:::::::::OO    !!:!!
  N::::::::N      N::::::N  OO:::::::::::::OO  !:::!
  N:::::::::N     N::::::N O:::::::OOO:::::::O !:::!
  N::::::::::N    N::::::N O::::::O   O::::::O !:::!
  N:::::::::::N   N::::::N O:::::O     O:::::O !:::!
  N:::::::N::::N  N::::::N O:::::O     O:::::O !:::!
  N::::::N N::::N N::::::N O:::::O     O:::::O !:::!
  N::::::N  N::::N:::::::N O:::::O     O:::::O !:::!
  N::::::N   N:::::::::::N O:::::O     O:::::O !:::!
  N::::::N    N::::::::::N O:::::O     O:::::O !!:!!
  N::::::N     N:::::::::N O::::::O   O::::::O  !!! 
  N::::::N      N::::::::N O:::::::OOO:::::::O      
  N::::::N       N:::::::N  OO:::::::::::::OO   !!! 
  N::::::N        N::::::N    OO:::::::::OO    !!:!!
  NNNNNNNN         NNNNNNN      OOOOOOOOO       !!! ");
                            //...
                        }
                    }
                }
            }
            if (!emittedExitStateCheck)
                ExitStateCheck(currentTarget, dfaState, exitState, targetState, true/*, nonGreedyParam, decisionPoint*/);
            ExitLADepthCheck(currentTarget, exitLADepth, currentLADepth, targetState);
            if (multitargetLookups.ContainsKey(targetState))
            {
                var currentJumpInfo = multitargetLookups[targetState];
                currentTarget.GoTo(currentJumpInfo.Label.Value);
            }
            else if (targetState.OutTransitions.Count > 0)
                toInsert.Push(Tuple.Create(targetState, (IBlockStatementParent)currentTarget,
                    new SyntacticalDFAStateJumpData()
                    {
                        EnclosureType = enclosureState == SyntacticalDFAStateEnclosureHandling.Encapsulate || enclosureState == SyntacticalDFAStateEnclosureHandling.EncapsulatePeek
                        ? SyntacticalDFAStateEnclosureHandling.Handled
                        : SyntacticalDFAStateEnclosureHandling.Undecided
                    }));
            else
                currentTarget.GoTo(decisionPoint);
            //HandleNextProjectionTerminalEdge(toInsert, currentTarget, targetState, decisionPoint, secondaryLookups, multitargetLookups, visited, exitLADepth, currentLADepth, exitState, rule, reductionHandlers, adapterLookup, predictionDetails);
        }

        private IEnumerable<IGrouping<GrammarVocabularyRuleDetail, Tuple<GrammarVocabularyRuleDetail, PredictionTreeLeaf, PredictionTreeBranch, IPredictionTreeKnownDestination>>> GetEdgeLeftRecursiveDetail(SyntacticalDFAState dfaState, OilexerGrammarProductionRuleEntry rule)
        {
            return (from d in dfaState.Sources
                    let decision = d.Item1 as IPredictionTreeKnownDestination
                    where decision != null
                    let decisionBreakdown = decision.DecidingFactor.SymbolicBreakdown(this.compiler)
                    where decisionBreakdown.Rules.Count > 0
                    from ruleBreakdown in decisionBreakdown.Rules.Values
                    where ruleBreakdown.Leaf.Veins.DFAOriginState.OutTransitions.Count == 1
                    let rootNode = this.compiler.RuleDetail[rule].Leaf
                    from inPath in rootNode.IncomingPaths
                    let index = inPath.IndexOf(ruleBreakdown.Leaf)
                    let myIndex = inPath.IndexOf(rootNode)
                    where index != -1
                    where index < inPath.Depth && index < myIndex
                    where inPath.GetDeviationAt(index) == 0
                    group Tuple.Create(ruleBreakdown, rootNode, inPath, decision) by ruleBreakdown);
        }

        private object BuildMaxContext(ILocalMember[] localReductionContext)
        {
            IExpression maxExpression = null;
            foreach (var local in localReductionContext)
            {
                if (maxExpression == null)
                    maxExpression = local.GetReference();
                else
                    maxExpression = this._identityManager.ObtainTypeReference(typeof(Math)).GetTypeExpression().GetMethod("Max").Invoke(maxExpression, local.GetReference());
            }
            return maxExpression;
        }

        private IStatementExpression ChainAssign(ILocalMember[] set, ILocalMember currentLADepth)
        {
            IAssignmentExpression current = null;
            foreach (var element in set)
            {
                if (current == null)
                    current = element.GetReference().Assign(currentLADepth.GetReference());
                else
                    current = element.GetReference().Assign(current);
            }
            return current;
        }

        private ITypedLocalMember GetPredictionReductionLocal(IIntermediateClassMethodMember predictMethod, int maxStateIndex, KeysValuePair<MultikeyedDictionaryKeys<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState>, HashSet<PredictionReduceDetail>> ksvp)
        {
            var result = predictMethod.Locals.Add(
                new TypedName(
                    string.Format("la_{{0}}_At_{{1:{0}}}", new string('0', maxStateIndex.ToString().Length)),
                    this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32),
                    ksvp.Keys.Key1.Name,
                    ksvp.Keys.Key2.StateValue));
            result.InitializationExpression = (-1).ToPrimitive();
            result.AutoDeclare = false;
            return result;
        }

        private void ReducePredictionDetails(PredictionReduceDetails predictionDetails)
        {
            var initFinalSameGroupings = (from detKVP in predictionDetails
                                          from predictionDetail in detKVP.Value
                                          group new { Detail = predictionDetail, Keys = detKVP.Keys } by new { predictionDetail.InitialState, predictionDetail.FinalState, Rule = predictionDetail.Rule, FinalDiscriminator = predictionDetail.FinalDiscriminator })
                                          .ToDictionary(k => k.Key, v => v.ToArray());
            foreach (var grouping in initFinalSameGroupings.Keys)
            {
                var currentHashSet = predictionDetails[grouping.Rule, grouping.InitialState];
                var groupingDetail = initFinalSameGroupings[grouping];
                if (groupingDetail.Any(k => k.Detail.ReductionType == PredictionReduceType.VariableLookAhead ||
                                            k.Detail.ReductionType == PredictionReduceType.FixedLookAhead &&
                                            k.Detail.KnownDeviation > 0))
                {
                    if (currentHashSet.Count > 1)
                    {
                        var gathered = groupingDetail.Where(k =>
                            k.Detail.ReductionType == PredictionReduceType.VariableLookAhead ||
                            k.Detail.ReductionType == PredictionReduceType.FixedLookAhead &&
                            k.Detail.KnownDeviation > 0).ToArray();
                        foreach (var element in gathered)
                            currentHashSet.Remove(element.Detail);
                        currentHashSet.Add(new PredictionReduceDetail { FinalDiscriminator = grouping.FinalDiscriminator, Rule = grouping.Rule, InitialState = grouping.InitialState, FinalState = grouping.FinalState, ReductionType = PredictionReduceType.VariableLookAhead });
                    }
                }
            }
        }

        private static PredictionReduceDetails ObtainReductionDetails(PredictionTreeDFAdapter projectionAdapter, SyntacticalDFAFlowGraph flowGraph)
        {
            /*************************************************************************************************************************************************************************\
            | REWRITE IMMEDIATELY (When time permits :)                                                                                                                               |
            \*************************************************************************************************************************************************************************/
            var predictionDetails = new PredictionReduceDetails();
            var projectionPaths = (from state in flowGraph.PluralTargets.Select(k => k.Value).Concat(flowGraph.Singletons.Select(k => k.Value)).OrderBy(k => k.StateValue)
                                   from source in state.Sources
                                   where source.Item1 is PredictionTree
                                   group source.Item1 as PredictionTree by state).ToDictionary(k => k.Key, v => v.ToArray());
            var projectionPathsSlim = (from projectionPathSets in projectionPaths.Values
                                       from projectionPathSet in projectionPathSets
                                       select projectionPathSet).ToArray();
            /* All rule-node details. */
            var projectionNodes = (from projectionPathSets in projectionPaths.Values
                                   from projectionPathSet in projectionPathSets
                                   from path in projectionPathSet
                                   select path.CurrentNode).Distinct();
            /* node->PredictionTree */
            var projectionNodePaths = (from projectionPathSets in projectionPaths.Values
                                       from projectionPathSet in projectionPathSets
                                       from path in projectionPathSet
                                       group projectionPathSet by path.CurrentNode).ToDictionary(k => k.Key, v => v.ToArray());
            var singleStepSites = (from node in projectionNodes
                                   from ambiguousPoint in node.AmbiguousReduceContexts
                                   where projectionPathsSlim.Contains(ambiguousPoint)
                                   group new
                                   {
                                       Node = node,
                                       Rule = node.Rule,
                                       AssignmentPoints = (from state in projectionPaths.Keys
                                                           where projectionPaths[state].Contains(ambiguousPoint)
                                                           select state).ToArray(),
                                       ReductionStates = (from state in projectionPaths.Keys
                                                          where projectionPaths[state].Contains(ambiguousPoint)
                                                          select state).ToArray(),
                                       AssignmentSets = ambiguousPoint,
                                       ReductionSets = ambiguousPoint
                                   } by new
                                   {
                                       DPathSet = ambiguousPoint,
                                       Rule = node.Rule
                                   }).ToDictionary(k => k.Key, v =>
                                    (from detail in v
                                     from reduceState in detail.ReductionStates
                                     group detail by reduceState).ToDictionary(k => k.Key, v2 => v2.ToArray()));

            var reductionSites = (from node in projectionNodes
                                  from ambiguousPoint in node.AmbiguousReduceSteppedContexts
                                  where projectionPathsSlim.Contains(ambiguousPoint.ReductionPoint)
                                  let lookAheadDeclarationPathSet = ambiguousPoint.Entrypoint
                                  let lookAheadReductionSitePathSet = ambiguousPoint.ReductionPoint
                                  let assignmentStates = (from state in projectionPaths.Keys
                                                          where projectionPaths[state].Contains(lookAheadDeclarationPathSet)
                                                          select state).ToArray()
                                  let reductionStates = (from state in projectionPaths.Keys
                                                         where projectionPaths[state].Contains(lookAheadReductionSitePathSet)
                                                         select state).ToArray()
                                  where assignmentStates.Length > 0 && reductionStates.Length > 0
                                  group new
                                  {
                                      Node = node,
                                      Rule = node.Rule,
                                      AssignmentPoints = assignmentStates,
                                      AssignmentSets = lookAheadDeclarationPathSet,
                                      ReductionStates = reductionStates,
                                      ReductionSets = lookAheadReductionSitePathSet
                                  } by new
                                  {
                                      DPathSet = lookAheadDeclarationPathSet,
                                      Rule = node.Rule
                                  }).ToDictionary(k => k.Key, v =>
                                    (from detail in v
                                     from reduceState in detail.ReductionStates
                                     group detail by reduceState).ToDictionary(k => k.Key, v2 => v2.ToArray()));

            /* Find the recursive paths, these identify the state and transition (element n-2, zero-based) that
             * note recursion in the machine.  This is important for reductions.  If one of the sources of a reduction
             * is contained within the source set of one of the recursive states, then we need to track the look-ahead
             * of that rule using a local variable. */
            if (reductionSites.Count > 0)
            {
                var recursivePaths = SyntacticalDFAnalysis.FindRecursiveAvenues(projectionAdapter.AssociatedState);
                var allPaths = ObtainProjectionPaths(projectionAdapter, recursivePaths);
                var recursivePathRelevantNodes =
                    (from path in allPaths
                     let relativeReductionSite =
                         (from reduction in reductionSites.Keys
                          from reductionState in reductionSites[reduction].Keys
                          /* Now we concatenate the reduction states from the items that might be recursive 
                           * (because they have to consume input to make a decision.) */
                          let declarationState = from assignmentDetailSet in reductionSites[reduction].Values
                                                 from assignmentDetail in assignmentDetailSet
                                                 from state in assignmentDetail.AssignmentPoints
                                                 select state
                          /* *
                           * Of the recursive paths that pass through both the start and the end of the declaration of
                           * a reduction and the reduce point, we know this is a point where we need to handle
                           * tracking look-ahead through a local variable.
                           * */
                          where IsRecursiveBetweenDeclarationAndReduction(path, reductionState, declarationState)
                          group reductionState by reduction)
                     from reductionSiteGroup in relativeReductionSite.Distinct()
                     from reductionSite in reductionSiteGroup
                     group reductionSite by reductionSiteGroup.Key).Distinct().ToDictionary(k => k.Key, v => v.Distinct().ToArray());

                var nonRecursiveVariants =
                    (from reductionSite in reductionSites.Keys
                     join recursiveVariant in recursivePathRelevantNodes.Keys on reductionSite equals recursiveVariant into recursiveCheck
                     from recursiveVariant in recursiveCheck.DefaultIfEmpty()
                     let reductionStates = reductionSites[reductionSite].Keys
                     let recursiveKeys = recursiveVariant == null ? new SyntacticalDFAState[0] : recursivePathRelevantNodes[recursiveVariant]
                     let nonRecursiveSet = reductionStates.Except(recursiveKeys).ToArray()
                     from reductionState in nonRecursiveSet
                     group reductionState by reductionSite).ToDictionary(k => k.Key, v => v.ToArray());
                /*Down side of anonymous types.*/
                foreach (var ruleDetail in recursivePathRelevantNodes.Keys)
                {
                    var reductionDetails = reductionSites[ruleDetail];
                    foreach (var state in recursivePathRelevantNodes[ruleDetail])
                    {
                        foreach (var reductionState in reductionDetails.Keys)
                        {

                            var reductionDetailsEx = reductionDetails[reductionState];
                            foreach (var reductionDetail in reductionDetailsEx)
                            {
                                var currentDiscriminator = reductionDetail.ReductionSets.Discriminator;
                                foreach (var assignmentState in reductionDetail.AssignmentPoints)
                                {
                                    var currentDetail = new PredictionReduceDetail()
                                    {
                                        FinalDiscriminator = currentDiscriminator,
                                        ReductionType = PredictionReduceType.VariableLookAhead,
                                        InitialState = assignmentState,
                                        FinalState = state,
                                        Rule = ruleDetail.Rule,
                                    };
                                    HashSet<PredictionReduceDetail> currentSet;
                                    if (!predictionDetails.TryGetValue(ruleDetail.Rule, assignmentState, out currentSet))
                                        predictionDetails.Add(ruleDetail.Rule, assignmentState, currentSet = new HashSet<PredictionReduceDetail>());
                                    currentSet.Add(currentDetail);
                                }
                            }
                        }
                    }
                }
                foreach (var ruleDetail in nonRecursiveVariants.Keys)
                {
                    var reductionDetails = reductionSites[ruleDetail];
                    foreach (var state in nonRecursiveVariants[ruleDetail])
                    {
                        foreach (var reductionState in reductionDetails.Keys)
                        {

                            var reductionDetailsEx = reductionDetails[reductionState];
                            foreach (var reductionDetail in reductionDetailsEx)
                            {
                                var currentDiscriminator = reductionDetail.ReductionSets.Discriminator;
                                foreach (var assignmentState in reductionDetail.AssignmentPoints)
                                {
                                    var currentDetail = new PredictionReduceDetail()
                                    {
                                        FinalDiscriminator = currentDiscriminator,
                                        ReductionType = PredictionReduceType.FixedLookAhead,
                                        InitialState = assignmentState,
                                        FinalState = state,
                                        KnownDeviation = reductionDetail.ReductionSets[0].GetCurrentDeviation(),
                                        Rule = ruleDetail.Rule,
                                    };
                                    HashSet<PredictionReduceDetail> currentSet;
                                    if (!predictionDetails.TryGetValue(ruleDetail.Rule, assignmentState, out currentSet))
                                        predictionDetails.Add(ruleDetail.Rule, assignmentState, currentSet = new HashSet<PredictionReduceDetail>());
                                    currentSet.Add(currentDetail);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var singleStep in singleStepSites.Keys)
            {
                var singleStepDetails = singleStepSites[singleStep];
                foreach (var singleStepState in singleStepDetails.Keys)
                {
                    var singleStepDetailsEx = singleStepDetails[singleStepState];
                    foreach (var reductionDetail in singleStepDetailsEx)
                    {
                        var currentDiscriminator = reductionDetail.ReductionSets.Discriminator;
                        foreach (var assignmentState in reductionDetail.AssignmentPoints)
                        {
                            var currentDetail = new PredictionReduceDetail()
                            {
                                FinalDiscriminator = currentDiscriminator,
                                ReductionType = PredictionReduceType.Simple,
                                InitialState = assignmentState,
                                FinalState = singleStepState,
                                KnownDeviation = reductionDetail.ReductionSets[0].GetCurrentDeviation(),
                                Rule = singleStep.Rule,
                            };
                            HashSet<PredictionReduceDetail> currentSet;
                            if (!predictionDetails.TryGetValue(singleStep.Rule, assignmentState, out currentSet))
                                predictionDetails.Add(singleStep.Rule, assignmentState, currentSet = new HashSet<PredictionReduceDetail>());
                            currentSet.Add(currentDetail);
                        }
                    }
                }
            }
            return predictionDetails;

            /*
            var projectionPaths = (from state in flowGraph.PluralTargets.Select(k => k.Value).Concat(flowGraph.Singletons.Select(k => k.Value)).OrderBy(k => k.StateValue)
                                   from source in state.Sources
                                   where source.Item1 is PredictionTree
                                   group source.Item1 as PredictionTree by state).ToDictionary(k => k.Key, v => v.ToArray());
            var projectionPathsSlim = (from projectionPathSets in projectionPaths.Values
                                       from projectionPathSet in projectionPathSets
                                       orderby projectionPathSet.Instance
                                       group projectionPathSet by projectionPathSet.Instance).ToDictionary(k => k.Key, v => v.First()).Values.ToArray();

            var projectionPathsByInstance =
                projectionPathsSlim.ToDictionary(k => k.Instance, v => v);

            var reverseLookup =
              (from state in projectionPaths.Keys
               from pathSet in projectionPaths[state]
               group state by pathSet.Instance).ToDictionary(k => k.Key, v => v.ToArray());

            var reductions =
              (from key in projectionPathsSlim
               where key.ReductionType == LookAheadReductionType.CommonForwardSymbol
               where key.ReductionDetail != null
               let reduction = key.ReductionDetail
               let start = reduction.BranchPoint
               let end = reduction.ReducePoint
               let symbolicReducedRule = reduction.ReducedRule.SymbolicBreakdown(this.compiler)
               let rule = symbolicReducedRule.Rules.Values.First()
               select new
               {
                   Reduction = reduction,
                   Start = start,
                   StartStates = reverseLookup[start.Instance],
                   End = end,
                   EndStates = reverseLookup[end.Instance],
                   RuleDetail = rule
               }).ToArray();
            PredictionReduceDetails results = new PredictionReduceDetails();
            foreach (var reduction in reductions)
            {
                foreach (var startState in reduction.StartStates)
                {
                    
                    HashSet<PredictionReduceDetail> detail;
                    if (!results.TryGetValue(reduction.RuleDetail.Rule, startState, out detail))
                        results.Add(reduction.RuleDetail.Rule, startState, detail = new HashSet<PredictionReduceDetail>());
                    foreach (var endState in reduction.EndStates)
                    {
                        var newDetail = new PredictionReduceDetail
                        {
                            Rule = reduction.RuleDetail.Rule,
                            ReductionType = reduction.Reduction.LookAheadDepth == 0 ? PredictionReduceType.Simple : PredictionReduceType.VariableLookAhead,
                            FinalDiscriminator = reduction.End.Discriminator,
                            InitialState = startState,
                            FinalState = endState,
                            KnownDeviation = reduction.Reduction.LookAheadDepth,
                        };
                        detail.Add(newDetail);
                    }
                }
            }
            return results;
             */
        }

        private static List<Tuple<GrammarVocabulary, SyntacticalDFAState>>[] ObtainProjectionPaths(PredictionTreeDFAdapter projectionAdapter, List<List<Tuple<GrammarVocabulary, SyntacticalDFAState>>> recursivePaths)
        {
            return (from r in recursivePaths
                    where r.Count >= 2
                    let nextToLast = r[r.Count - 2]
                    /* Project into the non-recursive avenues from the current state */
                    let nonRecursivePaths = SyntacticalDFAnalysis.FindNonRecursiveAvenues(nextToLast.Item2)
                    from path in nonRecursivePaths
                    /* Then the full recursive path becomes the path to the end of the automation with recursive
                     * context padded in */
                    select r.Take(r.Count - 1).Concat(path.Skip(1)).ToList()).Concat(SyntacticalDFAnalysis.FindNonRecursiveAvenues(projectionAdapter.AssociatedState)).Distinct(SyntacticalDFAnalysis.ListComparer.Singleton).ToArray();
        }

        private static bool IsRecursiveBetweenDeclarationAndReduction(List<Tuple<GrammarVocabulary, SyntacticalDFAState>> path, SyntacticalDFAState reductionState, IEnumerable<SyntacticalDFAState> declarationStates)
        {
            int startOffset = -1;
            int endOffset = -1;
            for (int index = 0; index < path.Count; index++)
            {
                var currentNode = path[index];
                if (startOffset == -1)
                    startOffset = declarationStates.Contains(currentNode.Item2) ? index : -1;
                else
                {
                    if (endOffset == -1)
                    {
                        foreach (var transition in currentNode.Item2.OutTransitions.Keys)
                        {
                            var transitionTarget = currentNode.Item2.OutTransitions[transition];
                            int transitionOffset = -1;
                            for (int pathNodeIndex = 0; pathNodeIndex <= index; pathNodeIndex++)
                                if (path[pathNodeIndex].Item2 == transitionTarget)
                                    transitionOffset = pathNodeIndex;
                            if (currentNode.Item2 == reductionState)
                                endOffset = index;
                            if (transitionOffset != -1)
                            {
                                if (endOffset == -1)
                                    if (transitionOffset < startOffset)
                                        //False positive.
                                        continue;
                                return path.Skip(index).Any(k => k.Item2 == reductionState);
                            }

                        }
                        //endOffset = currentNode.Item2 == reductionState ? index : -1;
                    }
                }
            }
            return false;
        }

        private void BuildParseMethod(OilexerGrammarProductionRuleEntry rule)
        {
            bool noPop = false;
            var adapter = this.Compiler.RuleAdapters[rule];
            var parseMethod = this.parseInternalMethods[adapter];
            var lookAheadParam = parseMethod.Parameters["laDepth"];
            var projection = adapter.AssociatedContext.RequiresProjection ? adapter.AssociatedContext.GetPredictiveProjection() : null;
            if (projection != null && projection.AssociatedContext.LeftRecursiveType != ProductionRuleLeftRecursionType.None)
            {
                string originalName = parseMethod.Name;
                parseMethod.Name += "Continuous";

                var parseContinuous = parseMethod;
                parseMethod = this._parserClass.Methods.Add(
                    new TypedName(originalName, parseContinuous.ReturnType),
                    new TypedNameSeries(
                        new TypedName("laDepth", RuntimeCoreType.Int32, this._identityManager)));
                parseInternalOriginalMethods.Add(adapter, parseMethod);
                parseMethod.AccessLevel = AccessLevelModifiers.Private;
                var predictionMethod = this.predictMethods[projection];
                parseContinuous.Call(_LookAheadDepthsImpl.GetReference().GetMethod("Push").Invoke(lookAheadParam.GetReference()));
                BuildParseContinuousMethod(parseContinuous, parseMethod, predictionMethod, rule, lookAheadParam, projection);
                lookAheadParam = parseMethod.Parameters["laDepth"];
                noPop = true;
            }
            else
                parseMethod.Call(_LookAheadDepthsImpl.GetReference().GetMethod("Push").Invoke(lookAheadParam.GetReference()));
            var entryContext = parseMethod.Locals.Add(
                new TypedName("entryContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol),
                this._CurrentContextImpl.GetReference());
            var localContext = parseMethod.Locals.Add(
                new TypedName("localContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol),
                _CurrentContextImpl.GetReference().Assign(
                    this.compiler.RuleSymbolBuilder.LanguageRuleSymbol.GetNewExpression(
                        this._TokenStreamImpl.GetReference(),
                        this._CurrentContextImpl.GetReference(),
                        this.Compiler.SyntacticalSymbolModel.GetIdentitySymbolField(this.Compiler._GrammarSymbols.GetSymbolFromEntry(rule)).GetReference(),
                        this._StateImpl.GetReference(),
                        this._FollowStateImpl.GetReference())));

            //if (rule.IsRuleCollapsePoint)
            //    BuildCollapsePointRule(adapter, parseMethod, lookAheadParam, localContext);
            //else
            //{
            parseMethod.Assign(this._StateImpl.GetReference(), this.compiler.RuleDFAStates[rule].StateValue.ToPrimitive());
            var prCurrentLADepth = parseMethod.Locals.Add(new TypedName("currentLADepth", RuntimeCoreType.Int32, this._identityManager), lookAheadParam.GetReference());
            BuildNormalRule(rule, adapter, parseMethod, lookAheadParam, localContext, prCurrentLADepth);
            //}
            parseMethod.Assign(this._CurrentContextImpl.GetReference(), entryContext.GetReference());
            if (!noPop)
                parseMethod.Call(_LookAheadDepthsImpl.GetReference().GetMethod("Pop").Invoke());
            if (adapter.AssociatedState.CanBeEmpty)
                parseMethod.Return(localContext);
            else
            {
                var exitCheck = parseMethod.If(prCurrentLADepth.Subtract(lookAheadParam).GreaterThan(IntermediateGateway.NumberZero));
                exitCheck.Return(localContext);
                exitCheck.CreateNext();
                exitCheck.Next.Return(IntermediateGateway.NullValue);
            }
        }

        private void BuildParseContinuousMethod(
            IIntermediateClassMethodMember parseContinuousMethod,
            IIntermediateClassMethodMember parseMethod,
            IIntermediateClassMethodMember predictMethod,
            OilexerGrammarProductionRuleEntry rule,
            IIntermediateMethodParameterMember<IClassMethodMember, IIntermediateClassMethodMember, IClassType, IIntermediateClassType> lookAheadParam,
            PredictionTreeDFAdapter projectionAdapter)
        {
            bool needsRuleSwitchLogic = projectionAdapter.AssociatedContext.RequiresLeftRecursiveCaution;
            var currentLADepthParam = parseContinuousMethod.Parameters["laDepth"];
            var nonGreedyApproach = parseContinuousMethod.Locals.Add(
                new TypedName("nonGreedy", RuntimeCoreType.Boolean, this._identityManager), this.PeekInitialContextsForImplOvr.GetReference().Invoke(this.compiler.RuleDetail[rule].Identity.GetReference()));
            var lastContext = parseContinuousMethod.Locals.Add(new TypedName("lastContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol), IntermediateGateway.NullValue);

            var lastValidContext = parseContinuousMethod.Locals.Add(new TypedName("lastValidContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol), lastContext.GetReference());
            var initialState = parseContinuousMethod.Locals.Add(new TypedName("entryState", RuntimeCoreType.Int32, this._identityManager), this._StateImpl.GetReference());
            var initialFollow = parseContinuousMethod.Locals.Add(new TypedName("entryFollow", RuntimeCoreType.Int32, this._identityManager), this._FollowStateImpl.GetReference());

            initialState.AutoDeclare = initialFollow.AutoDeclare = false;
            //Issue with this: the follow details for a left-recursive rule might cause a prediction failure if the rule has a Symbol+ at the end, causing the first entry to consume it all.
            parseContinuousMethod.Add(initialState.GetDeclarationStatement(initialFollow));
#if ENABLEREATTEMPT
            ILabelStatement attemptStart = null;
            ILocalMember reattemptChecker = null;
            if (needsRuleSwitchLogic)
            {
                attemptStart = parseContinuousMethod.DefineLabel("AttemptStart");
                reattemptChecker = parseContinuousMethod.Locals.Add(new TypedName("secondAttempt", RuntimeCoreType.Boolean, this._identityManager), IntermediateGateway.FalseValue);
            }
#endif
            IIntermediateMethodParameterMember<IClassMethodMember, IIntermediateClassMethodMember, IClassType, IIntermediateClassType>
                    ruleIncludeRuleContextParam = null;
            IIntermediateMethodParameterMember<IClassMethodMember, IIntermediateClassMethodMember, IClassType, IIntermediateClassType>
                    projIncludeRuleContextParam = null;
            ILocalMember includeRuleContext = null;
            var parseMethodPredictedState = parseMethod.Parameters.Add(new TypedName(lastPredictionResultName, RuntimeCoreType.Int32, this._identityManager));
            var lastPredictionResult = parseContinuousMethod.Locals.Add(new TypedName(lastPredictionResultName, RuntimeCoreType.Int32, this._identityManager));
            if (needsRuleSwitchLogic)
            {
                ruleIncludeRuleContextParam = parseMethod.Parameters.Add(new TypedName(ParserBuilder.includeRuleContextName, RuntimeCoreType.Boolean, this._identityManager));
                projIncludeRuleContextParam = predictMethod.Parameters[ParserBuilder.includeRuleContextName];
                includeRuleContext = parseContinuousMethod.Locals.Add(new TypedName(includeRuleContextName, RuntimeCoreType.Boolean, this._identityManager), IntermediateGateway.FalseValue);
            }

            var predictInvoke = predictMethod.GetReference().Invoke(lookAheadParam.GetReference());
            if (needsRuleSwitchLogic)
            {
                predictInvoke.Arguments.Add(includeRuleContext.GetReference());
                if (projectionAdapter.AssociatedContext.RequiresInnerRecursionSwap)
                    predictInvoke.Arguments.Add(nonGreedyApproach.GetReference());
            }
            //predictInvoke.Parameters.Add(nonGreedyApproach.GetReference());
            var whileBlock = parseContinuousMethod.While((lastPredictionResult.GetReference().Assign(predictInvoke)).GreaterThan(IntermediateGateway.NumberZero));
            if (projectionAdapter.AssociatedContext.RequiresInnerRecursionSwap)
            {
                var yieldTopLevelOuter = whileBlock.If(lastPredictionResult.EqualTo((-1).ToPrimitive()));
                yieldTopLevelOuter.Assign(lastContext.GetReference(), new CSharpConditionalExpression((ICSharpLogicalOrExpression)nonGreedyApproach.GetReference().AffixTo(CSharpOperatorPrecedences.LogicalOrOperation), (ICSharpConditionalExpression)this.BorrowOuterContext.GetReference().Invoke(currentLADepthParam.GetReference(), this.compiler.RuleDetail[rule].Identity.GetReference()).AffixTo(CSharpOperatorPrecedences.ConditionalOperation), (ICSharpConditionalExpression)lastValidContext.GetReference().AffixTo(CSharpOperatorPrecedences.ConditionalOperation)));
                yieldTopLevelOuter.Break();
            }
            whileBlock.Assign(this._StateImpl.GetReference(), initialState.GetReference());
            whileBlock.Assign(this._FollowStateImpl.GetReference(), initialFollow.GetReference());
            if (needsRuleSwitchLogic)
                whileBlock.Assign(lastContext.GetReference(), parseMethod.GetReference().Invoke(currentLADepthParam.GetReference(), lastPredictionResult.GetReference(), ruleIncludeRuleContextParam.GetReference()));
            else
                whileBlock.Assign(lastContext.GetReference(), parseMethod.GetReference().Invoke(currentLADepthParam.GetReference(), lastPredictionResult.GetReference()));
            var hasErrorCheck = whileBlock.If(this.compiler.RuleSymbolBuilder.HasError.GetReference(lastContext.GetReference()).Not());
            hasErrorCheck.Assign(lastValidContext.GetReference(), lastContext.GetReference());

            hasErrorCheck.If(nonGreedyApproach.GetReference())
                .Break();
            if (needsRuleSwitchLogic)
                hasErrorCheck.If(includeRuleContext.GetReference().Not())
                    .Assign(includeRuleContext.GetReference(), IntermediateGateway.TrueValue);
            hasErrorCheck.CreateNext();
            var recoveryCheck = hasErrorCheck.Next.If(lastValidContext.GetReference().InequalTo(IntermediateGateway.NullValue));
            recoveryCheck.If(this.compiler.RuleSymbolBuilder.Count.GetReference(lastContext.GetReference()).GreaterThan(0))
                .Call(this.Compiler.SymbolStreamBuilder.SwapImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(lastContext.GetReference(), lastValidContext.GetReference()));

            hasErrorCheck.Next.Break();

            parseContinuousMethod.Call(_LookAheadDepthsImpl.GetReference().GetMethod("Pop").Invoke());
            var nullValidCheck = parseContinuousMethod.If(lastValidContext.GetReference().EqualTo(IntermediateGateway.NullValue));

            nullValidCheck.Return(lastContext.GetReference());
            var currentSymbol = parseContinuousMethod.Locals.Add(new TypedName("currentSymbolOnStack", compiler.CommonSymbolBuilder.ILanguageSymbol), this.Compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this._SymbolStreamImpl.GetReference(), currentLADepthParam.GetReference()));
            currentSymbol.AutoDeclare = false;
            parseContinuousMethod.DefineLocal(currentSymbol);

            parseContinuousMethod.If(lastValidContext.InequalTo(currentSymbol))
                .Call(this.Compiler.SymbolStreamBuilder.SwapImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(currentSymbol.GetReference(), lastValidContext.GetReference()));

            parseContinuousMethod.Return(lastValidContext.GetReference());
        }

        private void BuildNormalRule(OilexerGrammarProductionRuleEntry rule, ProductionRuleNormalAdapter adapter, IIntermediateClassMethodMember parseMethod, IIntermediateMethodParameterMember<Abstract.Members.IClassMethodMember, IIntermediateClassMethodMember, IClassType, IIntermediateClassType> lookAheadParam, ITypedLocalMember localContext, ILocalMember prCurrentLADepth)
        {
            var flowGraph = SyntacticalDFAFlowGraph.CreateFlowGraph(this.Compiler.RuleDFAStates[rule]);
            var prExitLADepth = parseMethod.Locals.Add(new TypedName("exitLADepth", RuntimeCoreType.Int32, this._identityManager), adapter.AssociatedState.CanBeEmpty ? (IExpression)lookAheadParam.GetReference() : (-1).ToPrimitive());

            var prExitState = parseMethod.Locals.Add(new TypedName("exitState", RuntimeCoreType.Int32, this._identityManager), adapter.AssociatedState.CanBeEmpty ? adapter.AssociatedState.StateValue.ToPrimitive() : (-1).ToPrimitive());
            var maxStateIndex = flowGraph.PluralTargets.Concat(flowGraph.Singletons).Select(k => k.Value.StateValue).Max();
            var visited = new HashSet<SyntacticalDFAState>();
            var secondaryLookups = new Dictionary<IBlockStatementParent, Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>>();
            var laDepth = parseMethod.Parameters["laDepth"];

            var multitargetLookup =
                SyntacticalDFAStateJumpData.ObtainStateJumpDetails(
                    flowGraph.PluralTargets.OrderBy(p => p.Value.StateValue).Select(pt => pt.Value),
                    parseMethod,
                    "RuleMultiTargetState_{{0:{0}}}",
                    new string('0', maxStateIndex.ToString().Length), k => TargetedByAnyFollowPredictions(k, this.Compiler.RuleDetail[rule]), flowGraph.Singletons.Select(k=>k.Value));
            var explicitCaptures = rule.CaptureStructure.Values.Distinct().ToArray();

            var explicitCaptureLookup = new Dictionary<IProductionRuleCaptureStructuralItem, ILocalMember>();
            if (explicitCaptures.Length > 0 && rule.Name == this.compiler.Source.Options.StartEntry)
                parseMethod.Comment("Explicit captures are all only symbols, because the actual AST (stripped of un-necessary symbols) is built after we have a full graph.");
            foreach (var importElement in explicitCaptures)
            {
                switch (importElement.ResultType)
                {
                    case ResultedDataType.ComplexType:
                        break;
                    case ResultedDataType.Flag:
                        explicitCaptureLookup.Add(importElement, parseMethod.Locals.Add(new TypedName(importElement.BucketName.LowerFirstCharacter(), this._identityManager.ObtainTypeReference(RuntimeCoreType.Boolean).MakeNullable()), IntermediateGateway.NullValue));
                        break;
                    case ResultedDataType.Counter:
                        explicitCaptureLookup.Add(importElement, parseMethod.Locals.Add(new TypedName(importElement.BucketName.LowerFirstCharacter(), this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32).MakeNullable()), IntermediateGateway.NullValue));
                        break;
                    case ResultedDataType.Character:
                    case ResultedDataType.String:
                    case ResultedDataType.ImportType:
                    case ResultedDataType.Enumeration:
                    case ResultedDataType.EnumerationItem:
                        explicitCaptureLookup.Add(importElement, parseMethod.Locals.Add(new TypedName(importElement.BucketName.LowerFirstCharacter(), this.compiler.CommonSymbolBuilder.ILanguageSymbol), IntermediateGateway.NullValue));
                        break;
                    case ResultedDataType.FlagEnumerationItem:
                        if (importElement.Sources.AllSourcesAreSameIdentity())
                            goto case ResultedDataType.Flag;
                        else
                            goto case ResultedDataType.ImportTypeList;

                    case ResultedDataType.ImportTypeList:
                        explicitCaptureLookup.Add(importElement, parseMethod.Locals.Add(
                            new TypedName(importElement.BucketName.LowerFirstCharacter(),
                                ((IInterfaceType)this._identityManager.ObtainTypeReference(typeof(IList<>)))
                                    .MakeGenericClosure(this.compiler.CommonSymbolBuilder.ILanguageSymbol)),
                                ((IClassType)this._identityManager.ObtainTypeReference(typeof(List<>)))
                                    .MakeGenericClosure(this.compiler.CommonSymbolBuilder.ILanguageSymbol).GetNewExpression()));
                        break;
                    default:
                        break;
                }
            }
            var entryPointReference = multitargetLookup[adapter.AssociatedState].Label.Value;
            var remainingToProcess = new Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>>();
            var decisionPoint = new LabelStatement(parseMethod, "DecisionPoint");

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
             * This should be easier to marshal than the Lexical construct due to the lack of UnicodeTargets.   *
             * But more difficult in that the transition into a state will determine the type of action taken.  *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            //multitargetLookup[adapter.AssociatedState].BlockContainer.Assign(this._StateImpl.GetReference(), adapter.AssociatedState.StateValue.ToPrimitive());

            var lastParseResult = parseMethod.Locals.Add(new TypedName("lastParseResult", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol), IntermediateGateway.NullValue);

            CommonStateMachineBodyBuilder(topLevelState =>
                GenerateNormalRuleParseState(
                    remainingToProcess,
                    topLevelState.BlockContainer,
                    topLevelState.State,
                    decisionPoint,
                    secondaryLookups,
                    multitargetLookup,
                    visited,
                    prExitLADepth, prCurrentLADepth, prExitState, rule, explicitCaptureLookup, lastParseResult, localContext),
                currentToProcess =>
                GenerateNormalRuleParseState(
                    remainingToProcess,
                    currentToProcess.Item2,
                    currentToProcess.Item1,
                    decisionPoint,
                    secondaryLookups,
                    multitargetLookup,
                    visited,
                    prExitLADepth, prCurrentLADepth, prExitState, rule, explicitCaptureLookup, lastParseResult, localContext),
                multitargetLookup, remainingToProcess, secondaryLookups, parseMethod);


            parseMethod.Add(decisionPoint);
            foreach (var importElement in explicitCaptures)
            {
                switch (importElement.ResultType)
                {
                    case ResultedDataType.ComplexType:
                        break;
                    case ResultedDataType.FlagEnumerationItem:
                        if (importElement.Sources.AllSourcesAreSameIdentity())
                            goto case ResultedDataType.EnumerationItem;
                        else
                            goto case ResultedDataType.ImportTypeList;
                    case ResultedDataType.EnumerationItem:
                    case ResultedDataType.Flag:
                    case ResultedDataType.Counter:
                    case ResultedDataType.Character:
                    case ResultedDataType.String:
                    case ResultedDataType.ImportType:
                    case ResultedDataType.Enumeration:
                        parseMethod.If(explicitCaptureLookup[importElement].InequalTo(IntermediateGateway.NullValue)).Call(this.compiler.RuleSymbolBuilder.DelineateCapture.GetReference(localContext.GetReference()).Invoke(importElement.BucketName.ToPrimitive(), explicitCaptureLookup[importElement].GetReference()));
                        break;
                    case ResultedDataType.ImportTypeList:
                        parseMethod.If(explicitCaptureLookup[importElement].GetReference().GetProperty("Count").GreaterThan(0)).Call(this.compiler.RuleSymbolBuilder.DelineateCapture.GetReference(localContext.GetReference()).Invoke(importElement.BucketName.ToPrimitive(), explicitCaptureLookup[importElement].GetReference()));
                        break;
                }
            }
            var edges = adapter.AssociatedState.ObtainEdges().ToList();
            if (adapter.AssociatedState.CanBeEmpty)
                edges.Add(adapter.AssociatedState);
            var edgeStates = edges.Where(k => k.OutTransitions.Count == 0).Select(k => k.StateValue).Distinct().Select(k => k.ToPrimitive()).ToArray();
            var edgeStatesWithTransition =
                edges.Where(k => k.OutTransitions.Count > 0).Select(k => k.StateValue).Distinct().Select(k => k.ToPrimitive()).ToArray();

            var yieldSwitch = parseMethod.Switch(prExitState.GetReference());
            if (edgeStatesWithTransition.Any())
            {
                var withTransitionCases = yieldSwitch.Case(edgeStatesWithTransition);
                withTransitionCases.Comment("If an owner context errors out, the location here will identify the greater context at large of what's valid.");
                withTransitionCases.Call(this.ExpectImpl.GetReference(), prExitLADepth.GetReference(), localContext.GetReference(), IntermediateGateway.FalseValue);
                if (!adapter.AssociatedState.CanBeEmpty)
                    withTransitionCases.Call(this.compiler.RuleSymbolBuilder.DoReductionImpl.GetReference(localContext.GetReference()).Invoke(this._SymbolStreamImpl.GetReference(), lookAheadParam.GetReference(), prExitLADepth.Subtract(lookAheadParam)));
                else
                {
                    var wtcIf = withTransitionCases.If((prCurrentLADepth.Subtract(lookAheadParam)).GreaterThan(IntermediateGateway.NumberZero));

                    wtcIf.Call(this.compiler.RuleSymbolBuilder.DoReductionImpl.GetReference(localContext.GetReference()).Invoke(this._SymbolStreamImpl.GetReference(), lookAheadParam.GetReference(), prExitLADepth.Subtract(lookAheadParam)));
                    wtcIf.CreateNext();
                    wtcIf.Next.Call(this.Compiler.RuleSymbolBuilder.DoBlankReduction.GetReference(localContext.GetReference()).Invoke(this.compiler.SymbolStreamBuilder.GetTokenIndexImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(laDepth.GetReference())));
                }
            }
            if (edgeStates.Any())
            {
                var currentEdgeCase = yieldSwitch.Case(edgeStates);
                if (!adapter.AssociatedState.CanBeEmpty)
                    currentEdgeCase.Call(this.compiler.RuleSymbolBuilder.DoReductionImpl.GetReference(localContext.GetReference()).Invoke(this._SymbolStreamImpl.GetReference(), lookAheadParam.GetReference(), prExitLADepth.Subtract(lookAheadParam)));
                else
                {
                    var currentEdgeIf = currentEdgeCase.If((prCurrentLADepth.Subtract(lookAheadParam)).GreaterThan(IntermediateGateway.NumberZero));
                    currentEdgeIf.Call(this.compiler.RuleSymbolBuilder.DoReductionImpl.GetReference(localContext.GetReference()).Invoke(this._SymbolStreamImpl.GetReference(), lookAheadParam.GetReference(), prExitLADepth.Subtract(lookAheadParam)));
                    currentEdgeIf.CreateNext();
                    currentEdgeIf.Next.Call(this.Compiler.RuleSymbolBuilder.DoBlankReduction.GetReference(localContext.GetReference()).Invoke(this.compiler.SymbolStreamBuilder.GetTokenIndexImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(laDepth.GetReference())));
                }
            }

            /* Yield an error, but first check to see that there isn't already one on the stack from the lexer. */
            var yieldError = yieldSwitch.Case(true);
            yieldError.Assign(this.compiler.RuleSymbolBuilder.HasError.GetReference(localContext.GetReference()), IntermediateGateway.TrueValue);
            yieldError.Call(this.ExpectImpl.GetReference(), prCurrentLADepth.GetReference(), localContext.GetReference(), IntermediateGateway.TrueValue);
            var yieldErrorCheck = yieldError.If((prCurrentLADepth.Subtract(lookAheadParam)).GreaterThan(IntermediateGateway.NumberZero));
            yieldErrorCheck.Call(this.compiler.RuleSymbolBuilder.DoReductionImpl.GetReference(localContext.GetReference()).Invoke(this._SymbolStreamImpl.GetReference(), lookAheadParam.GetReference(), prCurrentLADepth.Subtract(lookAheadParam)));
            yieldErrorCheck.CreateNext();
            yieldErrorCheck.Next.Call(this.Compiler.RuleSymbolBuilder.DoBlankReduction.GetReference(localContext.GetReference()).Invoke(this.compiler.SymbolStreamBuilder.GetTokenIndexImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(laDepth.GetReference())));
        }

        private bool TargetedByAnyFollowPredictions(SyntacticalDFAState state, GrammarVocabularyRuleDetail ruleDetail)
        {
            List<SyntacticalDFAState> ruleStates = new List<SyntacticalDFAState>();
            SyntacticalDFAState.FlatlineState(ruleDetail.DFAState, ruleStates);
            var stateContext = this.compiler.AllRuleAdapters[ruleDetail.Rule, state].AssociatedContext;
            var ruleAdapters = (from ruleState in ruleStates
                                let adapterContext = this.compiler.AllRuleAdapters[ruleDetail.Rule, ruleState].AssociatedContext
                                where adapterContext.RequiresFollowProjection
                                select new { State = ruleState, AdapterContext = adapterContext }).ToDictionary(k => k.State, v => v.AdapterContext);
            var targeted =
                (from rs in ruleAdapters.Values
                 from follow in rs.Leaf.FollowAmbiguities
                 from followAdapt in follow.Adapter.All
                 from source in followAdapt.AssociatedState.Sources
                 where source.Item1 is PredictionTreeDestination
                 let decision = (PredictionTreeDestination)source.Item1
                 where decision.Target == stateContext.Leaf
                 select 1).Any();
            /* If a follow prediction targets a given state, eject it from the inlined set to reduce repeating code.
             * Smaller code is usually faster code. */
            return targeted;
        }

        private static void CommonStateMachineBodyBuilder(
            Action<SyntacticalDFAStateJumpData> stateProcessor,
            Action<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>> remainingProcessor,
            Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData> multitargetLookup,
            Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>> remainingToProcess,
            Dictionary<IBlockStatementParent, Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>> secondaryLookups,
            IIntermediateClassMethodMember targetMethod)
        {
            foreach (var topLevelState in multitargetLookup.Values)
                stateProcessor(topLevelState);
            while (remainingToProcess.Count > 0)
                remainingProcessor(remainingToProcess.Pop());

            foreach (var parent in secondaryLookups.Keys.Reverse())
                HandleStatementInjections(parent, secondaryLookups[parent]);
            HandleStatementInjections(targetMethod, multitargetLookup);
        }

        public IExpression GetPredictionInvokeExpression(IMemberReferenceExpression reference, PredictionTreeDFAdapter projection, IParameterMember lastPredictionResult)
        {

            var predictMethod = this.predictMethods[projection];
            if (lastPredictionResult != null)
                return lastPredictionResult.GetReference();//_StateImpl.GetReference().Assign(lastPredictionResult.GetReference());
            else
                return predictMethod.GetReference().Invoke(reference);//_StateImpl.GetReference().Assign(predictMethod.GetReference().Invoke(reference));
        }

        private void GenerateNormalRuleParseState(
            Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>> toInsert,
            IBlockStatementParent parentTarget,
            SyntacticalDFAState dfaState,
            ILabelStatement decisionPoint,
            Dictionary<IBlockStatementParent, Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>> secondaryLookups,
            IDictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData> multitargetLookups,
            HashSet<SyntacticalDFAState> visited,
            ILocalMember exitLADepth,
            ILocalMember currentLADepth,
            ILocalMember exitState,
            OilexerGrammarProductionRuleEntry rule,
            Dictionary<IProductionRuleCaptureStructuralItem, ILocalMember> explicitCaptures,
            ILocalMember lastParseResult,
            ILocalMember localContext)
        {
            if (!visited.Add(dfaState) && multitargetLookups.ContainsKey(dfaState))
                return;
            var adapter = this.Compiler.AllRuleAdapters[rule, dfaState];
            var node = this.compiler.AllProjectionNodes[dfaState];
            if (adapter.AssociatedContext.RequiresFollowProjection)
                /* Follow ambiguities might have a *normal* prediction also */
            {
                var discriminator = this.followDiscriminatorMethods[node];
                parentTarget.Comment("Follow ambiguity determination.  Do we get to continue?");
                var discriminatorSwitch =
                    parentTarget.Switch(this.compiler.SymbolStreamBuilder.LookAheadMethod.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(currentLADepth.GetReference()));

                /* *
                 * Pull all of the decisions out of every possible state-machine for this follow ambiguity.
                 * *
                 * We'll use this to build a state value switch which we'll use to transition into the
                 * appropriate state.
                 * */
                var decisionPoints =
                    (from followAmbiguitiesKSVP in this.followDiscriminatorContext.FilterToDictionary(node)
                     from followAmbiguity in followAmbiguitiesKSVP.Value
                     from edgeState in followAmbiguity.DFAState.ObtainEdges()
                     from source in edgeState.Sources
                     let decisiveSource = source.Item1 as PredictionTreeDestination
                     let decisiveFail = source.Item1 as PredictionTreeFollowCaller
                     where decisiveSource != null || decisiveFail != null
                     let targetState = decisiveSource == null ? null : decisiveSource.Target
                     let subQueryAdapterCheck = from adapterDetail in targetState == null ? (IEnumerable<KeysValuePair<MultikeyedDictionaryKeys<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState>, ProductionRuleNormalAdapter>>)new KeysValuePair<MultikeyedDictionaryKeys<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState>, ProductionRuleNormalAdapter>[0] : this.Compiler.AllRuleAdapters
                                                where targetState.Veins.DFAOriginState == adapterDetail.Keys.Key2
                                                select adapterDetail
                     from adapterDetail in subQueryAdapterCheck.DefaultIfEmpty()
                     where adapterDetail.Keys.Key1 == null || adapterDetail.Keys.Key1 == rule
                     group targetState by followAmbiguity.InitialPaths.Discriminator).ToDictionary(k => k.Key, v => v.Distinct().ToArray());
                //Console.WriteLine("{0} - {1} decision points", rule.Name, destinations.Sum(k => k.Value.Length));
                foreach (var transition in decisionPoints.Keys)
                {
                    var targetStates = decisionPoints[transition];
                    var symbolicBreakdown = transition.SymbolicBreakdown(this.compiler);
                    var discriminatorSymbols =
                        symbolicBreakdown.Rules.Values.Select(r => r.Identity.GetReference())
                        .Concat(
                        symbolicBreakdown.Tokens.Values.Select(t => t.Identity.GetReference()))
                        .Concat(
                        symbolicBreakdown.Ambiguities.Values.Select(a => a.Identity.GetReference()))
                        .Cast<IExpression>().ToArray();
                    var currentCase =
                        discriminatorSwitch.Case(discriminatorSymbols);
                    var targetStateSwitch = currentCase.Switch(discriminator.GetReference().Invoke(currentLADepth.GetReference()));

                    foreach (var targetNode in targetStates.Where(k => k != null))
                    {
                        var currentTargetCase = targetStateSwitch.Case(targetNode.Veins.DFAOriginState.StateValue.ToPrimitive());
                        HandleNextRuleStateTarget(
                            targetNode, currentTargetCase, symbolicBreakdown, toInsert, parentTarget,
                            dfaState, decisionPoint, secondaryLookups, multitargetLookups, visited,
                            exitLADepth, currentLADepth, exitState, rule, explicitCaptures, lastParseResult, localContext,
                            false, ParserRuleHandlingType.AsEither);
                    }
                    targetStateSwitch.Case((-1).ToPrimitive().RightComment("Finish, the owning context needs the symbol to parse properly."))
                        .GoTo(decisionPoint);
                    parentTarget.Comment("It was determined that the owning context doesn't need our symbols.");
                }
            }
            GenerateNormalRuleParseStateAfterDiscriminator(toInsert, parentTarget, dfaState, decisionPoint, secondaryLookups, multitargetLookups, visited, exitLADepth, currentLADepth, exitState, rule, explicitCaptures, lastParseResult, localContext, adapter, node);
        }

        private void GenerateNormalRuleParseStateAfterDiscriminator(Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>> toInsert, IBlockStatementParent parentTarget, SyntacticalDFAState dfaState, ILabelStatement decisionPoint, Dictionary<IBlockStatementParent, Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>> secondaryLookups, IDictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData> multitargetLookups, HashSet<SyntacticalDFAState> visited, ILocalMember exitLADepth, ILocalMember currentLADepth, ILocalMember exitState, OilexerGrammarProductionRuleEntry rule, Dictionary<IProductionRuleCaptureStructuralItem, ILocalMember> explicitCaptures, ILocalMember lastParseResult, ILocalMember localContext, ProductionRuleNormalAdapter adapter, PredictionTreeLeaf node)
        {
            if (adapter.AssociatedContext.RequiresProjection)
                GenerateNormalRulePredictionState(toInsert, parentTarget, dfaState, decisionPoint, secondaryLookups, multitargetLookups, visited, exitLADepth, currentLADepth, exitState, rule, explicitCaptures, lastParseResult, localContext, adapter, node);
            else
                GenerateNormalRuleRegularState(toInsert, parentTarget, dfaState, decisionPoint, secondaryLookups, multitargetLookups, visited, exitLADepth, currentLADepth, exitState, rule, explicitCaptures, lastParseResult, localContext, node);
        }

        private void BuildDiscriminatorScaffolding(PredictionTreeLeaf node, int prediction, int predictionCount)
        {
            /* Setup, start by giving each prediction method a unique identifier, then those will be the result of the first switch, 
             * which will kick off the next step which will determine which states are targeted. */
            var followsByGrammar = (from fa in node.FollowAmbiguities
                                    group fa by fa.InitialPaths.Discriminator)
                                    .ToDictionary(
                                        key => key.Key,
                                        value => (from followAmbiguity in value
                                                  let predictMethod = this.followPredictMethods[followAmbiguity.Adapter]
                                                  group followAmbiguity by predictMethod).ToDictionary(k => k.Key, v => v.ToArray()));
            var predictDiscriminator =
                this.ParserClass.Methods.Add(
                new TypedName("__Predict{0}DiscriminatorFollowing{1}", this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32), string.Format(string.Format("{{0:{0}}}", new string('0', predictionCount.ToString().Length)), prediction), node.Rule.Name),
                new TypedNameSeries(
                    new TypedName("laDepth",RuntimeCoreType.Int32, this._identityManager)));
            this.followDiscriminatorMethods.Add(node, predictDiscriminator);
            foreach (var key in followsByGrammar.Keys)
                foreach (var key2 in followsByGrammar[key].Keys)
                    this.followDiscriminatorContext.Add(node, key, key2, followsByGrammar[key][key2]);
            predictDiscriminator.AccessLevel = AccessLevelModifiers.Private;

        }

        private void GenerateNormalRulePredictionState(
            Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>> toInsert, 
            IBlockStatementParent parentTarget, SyntacticalDFAState dfaState,
            ILabelStatement decisionPoint, Dictionary<IBlockStatementParent, 
            Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>> secondaryLookups,
            IDictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData> multitargetLookups,
            HashSet<SyntacticalDFAState> visited,
            ILocalMember exitLADepth, 
            ILocalMember currentLADepth, 
            ILocalMember exitState,
            OilexerGrammarProductionRuleEntry rule,
            Dictionary<IProductionRuleCaptureStructuralItem,
            ILocalMember> explicitCaptures,
            ILocalMember lastParseResult,
            ILocalMember localContext,
            ProductionRuleNormalAdapter adapter,
            PredictionTreeLeaf node)
        {
            //bool recursive = adapter.AssociatedContext.RequiresProjection && this.compiler.AdvanceMachines[node].AssociatedContext.LeftRecursiveType == ProductionRuleLeftRecursionType.Direct;
            IParameterMember lastPredictionResult = null;
            bool recursive = false;
            if (this.parseInternalOriginalMethods.ContainsKey(adapter))
                recursive = true;
            if (dfaState is SyntacticalDFARootState && recursive)
                lastPredictionResult = this.parseInternalOriginalMethods[adapter].Parameters[ParserBuilder.lastPredictionResultName];

            var projectionAdapter = this.Compiler.AdvanceMachines[node];
            ILocalMember lastPredictionValue = null;
            if (lastPredictionResult == null)
                if (!adapter.AssociatedContext.ParseInternalMethod.Locals.TryGetValue(TypeSystemIdentifiers.GetMemberIdentifier("lastPrediction"), out lastPredictionValue))
                    lastPredictionValue = adapter.AssociatedContext.ParseInternalMethod.Locals.Add(new TypedName("lastPrediction", RuntimeCoreType.Int32, this._identityManager), (-1).ToPrimitive());


            var destinations = (from edge in projectionAdapter.AssociatedState.ObtainEdges().ToArray()
                                let decision = (from source in edge.Sources.Select(k => k.Item1)
                                                where source is PredictionTreeDestination
                                                select (PredictionTreeDestination)source).FirstOrDefault()
                                where decision != null
                                group new { EdgeState = edge, Decision = decisionPoint } by new { decision.DecidingFactor, decision.Target }).ToDictionary(k => k.Key, v => v.ToArray());

            var predictionExpression = (INaryOperandExpression)GetPredictionInvokeExpression(currentLADepth.GetReference(), projectionAdapter, lastPredictionResult);
            var currentSwitch = parentTarget.Switch(lastPredictionResult == null ? lastPredictionValue.GetReference().Assign(predictionExpression) : predictionExpression);
            foreach (var destination in destinations.Keys)
            {
                var targetTransition = destination.DecidingFactor;
                var destinationDetail = destinations[destination];

                ProductionRuleNormalAdapter targetAdapter = this.Compiler.AllRuleAdapters[rule, destination.Target.Veins.DFAOriginState];// adapter.OutgoingTransitions[targetTransition];
                var currentStateCase = currentSwitch.Case(targetAdapter.AssociatedState.StateValue.ToPrimitive().LeftComment(targetTransition.ToString().Replace("{", "{{").Replace("}", "}}")));
                currentStateCase.Assign(this._StateImpl, dfaState.StateValue.ToPrimitive());
                //currentStateCase.Assign(this._StateImpl.GetReference(), targetAdapter.AssociatedState.StateValue.ToPrimitive());
                var symbolicBreakdown = targetTransition.SymbolicBreakdown(this.compiler);
                bool isPossiblySkippingCaptures =
                    !dfaState.OutTransitions.ContainsKey(destination.DecidingFactor);
                if (isPossiblySkippingCaptures)
                {
                    var nullableDfaFactors =
                        dfaState.OutTransitions.Keys.Where(t =>
                        {
                            if (t.GetRuleVariant().IsEmpty)
                                return false;
                            return t.Breakdown.Rules.Any(r => this.compiler.RuleDetail[r.Source].DFAState.CanBeEmpty) && !dfaState.OutTransitions[t].OutTransitions.FullCheck.Intersect(targetTransition).IsEmpty;
                        }).ToArray();
                    if (nullableDfaFactors.Length > 0)
                    {
                        foreach (var factor in nullableDfaFactors)
                        {
                            var dfaTargetState = dfaState.OutTransitions[factor];
                            var targetCaptures =
                                (from source in dfaTargetState.Sources
                                 where source.Item2 == FiniteAutomata.FiniteAutomationSourceKind.Final &&
                                       source.Item1 is IProductionRuleCaptureStructuralItem
                                 select (IProductionRuleCaptureStructuralItem)source.Item1).Where(k => !string.IsNullOrEmpty(k.BucketName) && explicitCaptures.ContainsKey(k)).ToArray();
                            var currentRuleTarget = factor.Breakdown.Rules.FirstOrDefault();
                            var ruleDet = this.Compiler.RuleDetail[currentRuleTarget.Source];
                            ruleDet.InternalParseMethod.GetReference();
                            if (targetCaptures.Length > 0)
                                InjectCaptureLogic(currentStateCase, explicitCaptures, targetCaptures, ruleDet.InternalParseMethod.GetReference().Invoke(currentLADepth.GetReference()));
                        }
                    }
                    //var potentiallyRelevantIncomingStates = destination.Target.Veins.DFAOriginState.InTransitions.Keys.Where(k=>k.Intersect())
                }
                HandleNextRuleStateTarget(targetAdapter.AssociatedContext.Leaf, currentStateCase, symbolicBreakdown, toInsert, parentTarget, dfaState, decisionPoint, secondaryLookups, multitargetLookups, visited, exitLADepth, currentLADepth, exitState, rule, explicitCaptures, lastParseResult, localContext, false, ParserRuleHandlingType.AsEither/*, symbolicBreakdown.Rules.Count == 1 && symbolicBreakdown.Tokens.Count == 0*/);
            }
            if (lastPredictionResult == null)
            {
                var defaultCase = currentSwitch.Case(true);
                defaultCase.Assign(currentLADepth, lastPredictionValue.GetReference().Negate());
                defaultCase.GoTo(decisionPoint);
            }
        }

        private void GenerateNormalRuleRegularState(
            Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>> toInsert, 
            IBlockStatementParent parentTarget, 
            SyntacticalDFAState dfaState, 
            ILabelStatement decisionPoint, 
            Dictionary<IBlockStatementParent, Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>> secondaryLookups,
            IDictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData> multitargetLookups,
            HashSet<SyntacticalDFAState> visited,
            ILocalMember exitLADepth, 
            ILocalMember currentLADepth, 
            ILocalMember exitState, 
            OilexerGrammarProductionRuleEntry rule, 
            Dictionary<IProductionRuleCaptureStructuralItem, ILocalMember> explicitCaptures,
            ILocalMember lastParseResult,
            ILocalMember localContext, 
            PredictionTreeLeaf node)
        {
            if (dfaState.OutTransitions.Count > 0)
            {
                ILocalMember lastLookAhead = null;
                bool singleTarget = dfaState.OutTransitions.Count == 1;
                var invokeCall = this.compiler.SymbolStreamBuilder.LookAheadMethod.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(currentLADepth.GetReference());
                var nullableRuleTransitions = dfaState.OutTransitions.Where(k =>
                {
                    var rulesForBreakdown = k.Key.GetRuleVariant();
                    if (rulesForBreakdown.IsEmpty)
                        return false;
                    var ruleBreakdownCheck = rulesForBreakdown.Breakdown.Rules.Any(r =>
                    {
                        var dfa = this.Compiler.RuleDFAStates[(IOilexerGrammarProductionRuleEntry)r.Source];
                        if (dfa.CanBeEmpty)
                            return true;
                        return false;
                    });
                    var target = dfaState.OutTransitions[k.Key];
                    return target.IsEdge && ruleBreakdownCheck;
                }).ToArray();
                if (nullableRuleTransitions.Length > 0)
                    lastLookAhead = this.GetNewLocal(exitLADepth, "lastLookAhead", this.Compiler.SymbolStoreBuilder.Identities);

                var currentSwitch = parentTarget.Switch(lastLookAhead == null ? (INaryOperandExpression)invokeCall : lastLookAhead.GetReference().Assign(invokeCall));

                var transitionsGroupedByOriginalTarget =
                    (from transitionKey in node.LookAhead.Keys
                     let decisionPathSet = node.LookAhead[transitionKey]
                     let originalTransition =
                        singleTarget
                            ? dfaState.OutTransitions.Keys.First()
                            : decisionPathSet.ProjectedRootTransition
                     let transitionTarget =
                        singleTarget
                            ? this.Compiler.AllProjectionNodes[dfaState.OutTransitions.Values.First()]
                            : decisionPathSet.ProjectedRootTarget
                     where Assert(string.Format("Parse{0}", rule.Name), transitionKey, node, originalTransition, transitionTarget)
                     group transitionKey
                     by new
                     {
                         TargetState = transitionTarget,
                         TransitionKey = originalTransition
                     }).ToDictionary(k => new
                     {
                         Target = k.Key.TargetState,
                         TransitionKey = k.Key.TransitionKey.SymbolicBreakdown(this.compiler),
                         OriginalVocabulary = k.Key.TransitionKey,
                     }, v =>
                     {
                         var aggregate = v.Aggregate(GrammarVocabulary.UnionAggregateDelegate);
                         var tokens = aggregate.GetTokenVariant().SymbolicBreakdown(this.compiler);
                         var rules = aggregate.GetRuleVariant().SymbolicBreakdown(this.compiler);
                         return new { Rules = rules, Tokens = tokens, Vocabulary = aggregate };
                     });
                HashSet<IGrammarAmbiguousSymbol> seenAmbiguities = new HashSet<IGrammarAmbiguousSymbol>();
                var identityBlocks = new Dictionary<IGrammarSymbol, ISwitchCaseBlockStatement>();
                foreach (var transitionKeyInfo in transitionsGroupedByOriginalTarget.Keys)
                {
                    var detail = transitionsGroupedByOriginalTarget[transitionKeyInfo];
                    if (detail.Tokens.Tokens.Count > 0 || detail.Tokens.Ambiguities.Count > 0)
                    {
                        foreach (var ambiguity in detail.Tokens.Ambiguities.Values)
                            seenAmbiguities.Add(ambiguity.Symbol);
                        var currentCase = currentSwitch.Case(
                            detail.Tokens.Tokens.Values
                            .Select(tokDet => (IExpression)tokDet.Identity.GetReference())
                            .Concat(
                                detail.Tokens.Ambiguities.Values
                                .Select(ambigDet => (IExpression)ambigDet.Identity.GetReference())).ToArray());
                        foreach (var tokenDet in detail.Tokens.Tokens.Values)
                            identityBlocks.Add(tokenDet.Symbol, currentCase);
                        HandleNextRuleStateTarget(
                            transitionKeyInfo.Target, currentCase, transitionKeyInfo.TransitionKey, toInsert, parentTarget, dfaState, decisionPoint, secondaryLookups, multitargetLookups, visited, exitLADepth, currentLADepth, exitState, rule, explicitCaptures, lastParseResult, localContext, methodInvocationHandlingType: ParserRuleHandlingType.AsRule);
                    }
                    if (detail.Rules.Rules.Count > 0)
                    {
                        var currentCase = currentSwitch.Case(
                            detail.Rules.Rules.Values.Select(ruleDet => (IExpression)ruleDet.Identity.GetReference()).ToArray());
                        HandleNextRuleStateTarget(
                            transitionKeyInfo.Target, currentCase, transitionKeyInfo.TransitionKey, toInsert, parentTarget, dfaState, decisionPoint, secondaryLookups, multitargetLookups, visited, exitLADepth, currentLADepth, exitState, rule, explicitCaptures, lastParseResult, localContext, methodInvocationHandlingType: ParserRuleHandlingType.AsEither,
                            fromRuleLeadIn: detail.Rules.Rules.Count == 1);
                    }
                }
                if (node.LexicalAmbiguities != null)
                {
                    var ambiguityPoints = node.LexicalAmbiguities.Except(seenAmbiguities);
                    foreach (var t in ambiguityPoints)
                    {
                        var transitionLocks = (from tki in transitionsGroupedByOriginalTarget.Values
                                               where !tki.Vocabulary.Intersect(t.AmbiguityKey).IsEmpty
                                               select tki).ToArray();
                        Debug.Assert(transitionLocks.Length == 1, "Ambiguity Assertion Failure");
                        var firstLock = transitionLocks.Single();
                        var targetDetail = firstLock;
                        var firstTokIdent = targetDetail.Tokens.Tokens.Values.First();
                        var newCase = currentSwitch.Case(compiler.LexicalSymbolModel.GetIdentitySymbolField(t).GetReference());
                        newCase.Add(identityBlocks[firstTokIdent.Symbol].GetGoTo(newCase));
                    }
                }
                var defaultCase = currentSwitch.Case(true);
                /* Handle transitions which yield an empty (blank) rule here. */
                /* Example: A ::= (a | b | );  <-- without this, the '|' after b, fails. */
                if (nullableRuleTransitions.Length > 0)
                {
                    var targetTransitionDetail = nullableRuleTransitions.FirstOrDefault();
                    var target = transitionsGroupedByOriginalTarget.Where(k => k.Key.Target.Veins.DFAOriginState == targetTransitionDetail.Value).FirstOrDefault();
                    if (target.Key != null)
                    {
                        ILocalMember symbolAsValidSymbol = GetNewLocal(exitLADepth, "symbolAsValidSyntax", this.Compiler.LexicalSymbolModel.ValidSymbols);
                        ILocalMember validSyntaxFor = GetNewLocal(exitLADepth, "validSyntaxFor", this.Compiler.LexicalSymbolModel.ValidSymbols);
                        defaultCase.Assign(symbolAsValidSymbol, lastLookAhead.GetReference().Cast(this.Compiler.LexicalSymbolModel.ValidSymbols));
                        defaultCase.Assign(validSyntaxFor, this.Compiler.ExtensionsBuilder.GetValidSyntaxMethodInternalImpl.GetReference().Invoke(targetTransitionDetail.Value.StateValue.ToPrimitive(), localContext.GetReference(), IntermediateGateway.TrueValue));
                        var targetRule = targetTransitionDetail.Key.Breakdown.Rules.First().Source;
                        var firstTokenRef = target.Value.Tokens.Tokens.Keys.FirstOrDefault();
                        if (firstTokenRef == null)
                            defaultCase.Comment("??");
                        else
                        {
                            var ifStatement = defaultCase.If(symbolAsValidSymbol.BitwiseAnd(validSyntaxFor).InequalTo(this.Compiler.LexicalSymbolModel.NoIdentityField));
                            ifStatement.Comment(string.Format("Most likely {0} is empty.", targetRule.Name));
                            ifStatement.Add(identityBlocks[firstTokenRef].GetGoTo(defaultCase));
                        }
                    }
                }
                defaultCase.GoTo(decisionPoint);
            }
            else
                parentTarget.GoTo(decisionPoint);
        }

        private ILocalMember GetNewLocal(ILocalMember exitLADepth, string localName, IType localType)
        {
            ILocalMember symbolAsValidSymbol;

            if (!exitLADepth.Parent.Locals.TryGetValue(TypeSystemIdentifiers.GetMemberIdentifier(localName), out symbolAsValidSymbol))
                symbolAsValidSymbol = exitLADepth.Parent.Locals.Add(localType.WithName(localName));
            return symbolAsValidSymbol;
        }


        private static bool IsIntersectionNonEmpty(GrammarVocabulary transitionKey, GrammarVocabulary transition)
        {
            return !transition.Intersect(transitionKey).IsEmpty;
        }

        private bool Assert(string parseMethodName, GrammarVocabulary transitionKey, PredictionTreeLeaf node, GrammarVocabulary originalTransition, PredictionTreeLeaf transitionTarget)
        {
            bool tKeyAssert = transitionKey != null;
            bool nodeAssert = node != null;
            bool originalTransitionAssert = originalTransition != null;
            bool transitionTargetAssert = transitionTarget != null;
            if (!(tKeyAssert && nodeAssert && originalTransitionAssert && transitionTargetAssert))
            {
                var sb = new StringBuilder();
                sb.Append(parseMethodName);
                sb.Append(": ");
                if (!tKeyAssert)
                    sb.Append("transitionKey was null");
                else
                    sb.Append(transitionKey);
                sb.Append(", ");
                if (!nodeAssert)
                    sb.Append("node was null");
                else
                    sb.Append(node);
                sb.Append(", ");
                if (!originalTransitionAssert)
                    sb.Append("originalTransition was null");
                else
                    sb.Append(originalTransition);
                sb.Append(", ");
                if (!transitionTargetAssert)
                    sb.Append("transitionTarget was null");
                else
                    sb.AppendFormat("{0}: {2}State {1}", transitionTarget.Rule.Name, transitionTarget.Veins.DFAOriginState.StateValue, transitionTarget.Veins.DFAOriginState.IsEdge ? "Edge " : string.Empty);
                Debug.Assert(false, sb.ToString());
            }

            return tKeyAssert && nodeAssert && originalTransitionAssert && transitionTargetAssert;
        }

        private void HandleNextRuleStateTarget(
            PredictionTreeLeaf target,
            ISwitchCaseBlockStatement currentCase,
            GrammarVocabularySymbolicBreakdown origIdents,
            Stack<Tuple<SyntacticalDFAState, IBlockStatementParent, SyntacticalDFAStateJumpData>> toInsert,
            IBlockStatementParent parentTarget,
            SyntacticalDFAState dfaState,
            ILabelStatement decisionPoint,
            Dictionary<IBlockStatementParent, Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData>> secondaryLookups,
            IDictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData> multitargetLookups,
            HashSet<SyntacticalDFAState> visited,
            ILocalMember exitLADepth,
            ILocalMember currentLADepth,
            ILocalMember exitState,
            OilexerGrammarProductionRuleEntry rule,
            Dictionary<IProductionRuleCaptureStructuralItem, ILocalMember> explicitCaptures,
            ILocalMember lastParseResult,
            ILocalMember localContext,
            bool assignState = true, ParserRuleHandlingType methodInvocationHandlingType = ParserRuleHandlingType.AsRule,
            bool fromRuleLeadIn = false)
        {
            var originalState = target.Veins.DFAOriginState;
            var targetCaptures =
                (from source in originalState.Sources
                 where source.Item2 == FiniteAutomata.FiniteAutomationSourceKind.Final &&
                       source.Item1 is IProductionRuleCaptureStructuralItem
                 select (IProductionRuleCaptureStructuralItem)source.Item1).Where(k => !string.IsNullOrEmpty(k.BucketName) && explicitCaptures.ContainsKey(k)).ToArray();
            if (origIdents.Rules.Count > 1)
                Debug.Assert(false, "Error: Deterministic failure.");
            else if (origIdents.Rules.Count == 1)
            {
                var ruleDet = origIdents.Rules.Values.First();

                var targetRuleNode = this.compiler.RuleAdapters[ruleDet.Rule].AssociatedContext.Leaf;
                var ruldIdentity = ruleDet.Identity;

                //if (targetRuleNode.HasBeenReduced)
                //{
                switch (methodInvocationHandlingType)
                {
                    case ParserRuleHandlingType.AsSymbol:
                        currentCase.Comment(string.Format("{0} was reduced.", ruleDet.Identity.Name));
                        InjectCaptureLogic(currentCase, explicitCaptures, targetCaptures, this._SymbolStreamImpl.GetReference().GetIndexer(currentLADepth.GetReference()));
                        ExitLADepthCheck(currentCase, exitLADepth, currentLADepth, originalState);
                        break;
                    case ParserRuleHandlingType.AsRule:
                        {
                            IBlockStatement currentTarget = currentCase;
                            currentTarget.Assign(_FollowStateImpl.GetReference(), originalState.StateValue.ToPrimitive());
                            if (ruleDet.Leaf.Veins.LeftRecursionType != ProductionRuleLeftRecursionType.None &&
                                ruleDet.Leaf.Count == 1)
                                currentTarget = currentCase.If(PeekInitialContextsForImpl.GetReference().Invoke(ruleDet.Identity.GetReference(), currentLADepth.GetReference()).Not());
                            assignState = true;
                            InjectCaptureLogic(currentTarget, explicitCaptures, targetCaptures, lastParseResult.GetReference().Assign(ruleDet.InternalParseMethod.GetReference().Invoke(currentLADepth.GetReference())));
                            if (ruleDet.DFAState.CanBeEmpty)
                                ZeroLengthCheck(exitLADepth, currentLADepth, lastParseResult, originalState, currentTarget);
                            else
                            {
                                var ifNullCheck = currentTarget.If(lastParseResult.GetReference().EqualTo(IntermediateGateway.NullValue));
                                ifNullCheck.Assign(this.compiler.RuleSymbolBuilder.HasError.GetReference(localContext.GetReference()), IntermediateGateway.TrueValue);
                                ExitLADepthCheck(currentCase, exitLADepth, currentLADepth, originalState);
                            }
                            break;
                        }
                    case ParserRuleHandlingType.AsEither:
                        {
                            IBlockStatement currentTarget = currentCase;
                            if (!this.Compiler.ReducedRules.Contains(ruleDet.Rule) && ruleDet.Leaf.Veins.LeftRecursionType == ProductionRuleLeftRecursionType.None)
                                goto case ParserRuleHandlingType.AsRule;
                            bool addExitLADepthCheck = false;
                            if (!fromRuleLeadIn)// && ruleDet.Leaf.Veins.LeftRecursionType == ProductionRuleLeftRecursionType.None)
                            {
                                var ifCheck = new ConditionBlockStatement(currentTarget);
                                ifCheck.Condition =
                                    this.compiler.GenericSymbolStreamBuilder.CountImpl.GetReference(this._SymbolStreamImpl.GetReference())
                                        .GreaterThan(currentLADepth)
                                        .LogicalAnd(this.SymbolStreamBuilder.LookAheadMethodImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(currentLADepth.GetReference())
                                        .EqualTo(ruldIdentity.GetReference()));
                                InjectCaptureLogic(ifCheck, explicitCaptures, targetCaptures, this._SymbolStreamImpl.GetReference().GetIndexer(currentLADepth.GetReference()));
                                if (ifCheck.Count > 0)
                                {

                                    ifCheck.CreateNext();
                                    IBlockStatement currentParseTarget = ifCheck.Next;
                                    //if (ruleDet.Node.Value.LeftRecursionType != ProductionRuleLeftRecursionType.None &&
                                    //    ruleDet.Node.Count == 1)
                                    //    currentParseTarget = currentParseTarget.If(PeekInitialContextsForImpl.GetReference().Invoke(ruleDet.Identity.GetReference(), currentLADepth.GetReference()).Not());
                                    assignState = true;
                                    currentParseTarget.Assign(_FollowStateImpl.GetReference(), originalState.StateValue.ToPrimitive());
                                    InjectCaptureLogic(currentParseTarget, explicitCaptures, targetCaptures, lastParseResult.GetReference().Assign(ruleDet.InternalParseMethod.GetReference().Invoke(currentLADepth.GetReference())));
                                    currentParseTarget.Assign(this._StateImpl.GetReference(), originalState.StateValue.ToPrimitive());
                                    if (ruleDet.DFAState.CanBeEmpty)
                                        ZeroLengthCheck(exitLADepth, currentLADepth, lastParseResult, originalState, currentParseTarget);
                                    else
                                    {
                                        var ifNullCheck = currentParseTarget.If(lastParseResult.GetReference().EqualTo(IntermediateGateway.NullValue));
                                        ifNullCheck.Assign(this.compiler.RuleSymbolBuilder.HasError.GetReference(localContext.GetReference()), IntermediateGateway.TrueValue);
                                        addExitLADepthCheck = true;
                                    }
                                }
                                else
                                {
                                    ifCheck.Assign(_FollowStateImpl.GetReference(), originalState.StateValue.ToPrimitive());
                                    ifCheck.Condition = new ParenthesizedExpression(ifCheck.Condition).Not();
                                    assignState = true;
                                    InjectCaptureLogic(ifCheck, explicitCaptures, targetCaptures, lastParseResult.GetReference().Assign(ruleDet.InternalParseMethod.GetReference().Invoke(currentLADepth.GetReference())));
                                    if (ruleDet.DFAState.CanBeEmpty)
                                        ZeroLengthCheck(exitLADepth, currentLADepth, lastParseResult, originalState, ifCheck);
                                    else
                                    {
                                        var ifNullCheck = ifCheck.If(lastParseResult.GetReference().EqualTo(IntermediateGateway.NullValue));
                                        ifNullCheck.Assign(this.compiler.RuleSymbolBuilder.HasError.GetReference(localContext.GetReference()), IntermediateGateway.TrueValue);
                                        addExitLADepthCheck = true;
                                    }
                                }
                                currentTarget.Add(ifCheck);
                            }
                            else
                            {
                                int preInjectCount = currentTarget.Count;

                                InjectCaptureLogic(currentTarget, explicitCaptures, targetCaptures, this._SymbolStreamImpl.GetReference().GetIndexer(currentLADepth.GetReference()));
                                if (currentTarget.Count == preInjectCount)
                                {
                                    currentTarget.Assign(_FollowStateImpl.GetReference(), originalState.StateValue.ToPrimitive());
                                    assignState = true;
                                    InjectCaptureLogic(currentTarget, explicitCaptures, targetCaptures, lastParseResult.GetReference().Assign(ruleDet.InternalParseMethod.GetReference().Invoke(currentLADepth.GetReference())));
                                    if (ruleDet.DFAState.CanBeEmpty)
                                        ZeroLengthCheck(exitLADepth, currentLADepth, lastParseResult, originalState, currentTarget);
                                    else
                                    {
                                        var ifNullCheck = currentTarget.If(lastParseResult.GetReference().EqualTo(IntermediateGateway.NullValue));
                                        ifNullCheck.Assign(this.compiler.RuleSymbolBuilder.HasError.GetReference(localContext.GetReference()), IntermediateGateway.TrueValue);
                                        addExitLADepthCheck = true;
                                    }
                                }
                                else
                                    addExitLADepthCheck = true;
                            }
                            if (addExitLADepthCheck)
                                ExitLADepthCheck(currentCase, exitLADepth, currentLADepth, originalState);
                            break;
                        }
                }
            }
            ExitStateCheck(currentCase, dfaState, exitState, originalState, assignState);
            if (origIdents.Tokens.Count > 0 || origIdents.Ambiguities.Count > 0)
            {
                InjectCaptureLogic(currentCase, explicitCaptures, targetCaptures, this._SymbolStreamImpl.GetReference().GetIndexer(currentLADepth.GetReference()));
                ExitLADepthCheck(currentCase, exitLADepth, currentLADepth, originalState);
            }
            if (multitargetLookups.ContainsKey(originalState))
            {
                var currentJumpInfo = multitargetLookups[originalState];
                currentCase.GoTo(currentJumpInfo.Label.Value);
            }
            else if (originalState.OutTransitions.Count > 0)
            {
                ProductionRuleNormalAdapter normalTargetAdapter;

                if (!this.Compiler.AllRuleAdapters.TryGetValue(rule, originalState, out normalTargetAdapter))
                {

                }
                toInsert.Push(Tuple.Create(originalState, (IBlockStatementParent)currentCase, (SyntacticalDFAStateJumpData)null));
            }
            else
            {
                currentCase.GoTo(decisionPoint);
            }
        }

        private void ZeroLengthCheck(ILocalMember exitLADepth, ILocalMember currentLADepth, ILocalMember lastParseResult, SyntacticalDFAState originalState, IBlockStatement currentTarget)
        {
            var ifZeroLengthCheck = currentTarget.If(this.compiler.CommonSymbolBuilder.Length.GetReference(lastParseResult.GetReference()).EqualTo(0));
            ExitLADepthCheck(ifZeroLengthCheck, exitLADepth, currentLADepth, originalState, false);
            if (ifZeroLengthCheck.Count == 0)
            {
                ifZeroLengthCheck.Condition = this.compiler.CommonSymbolBuilder.Length.GetReference(lastParseResult.GetReference()).InequalTo(0);
                ExitLADepthCheck(ifZeroLengthCheck, exitLADepth, currentLADepth, originalState, true);
                if (ifZeroLengthCheck.Count == 0)
                    currentTarget.Remove(ifZeroLengthCheck);
            }
            else
            {
                ifZeroLengthCheck.CreateNext();
                ExitLADepthCheck(ifZeroLengthCheck.Next, exitLADepth, currentLADepth, originalState, true);
            }
        }

        private void ExitStateCheck(IBlockStatement currentTarget, SyntacticalDFAState dfaState, ILocalMember exitState, SyntacticalDFAState targetState, bool assignState/*, IParameterMember nonGreedyParam = null, ILabelStatement decisionPoint = null*/)
        {
            //var decision = (from source in targetState.Sources.Select(k => k.Item1)
            //                 where source is PredictionTreeDestination
            //                 select (PredictionTreeDestination)source).FirstOrDefault();
            //if (decision != null)
            //{
            //    var symbolicDecision = decision.DecidingFactor.SymbolicBreakdown(this.compiler);
            //    if (symbolicDecision.Rules.Count > 0)
            //    {
            //        var ruleDet = symbolicDecision.Rules.Values.First();
            //        if (ruleDet.Node.Value.LeftRecursionType != ProductionRuleLeftRecursionType.None &&
            //            ruleDet.Node.Count == 1)
            //        {
            //            var ruleTarget = ruleDet.Node.Values.First();
            //            var ruleTargetDet = this.compiler.RuleDetail[ruleTarget.Rule];

            //            currentTarget = currentTarget.If(PeekInitialContextsForImpl.GetReference().Invoke(ruleTargetDet.Identity.GetReference()).Not());

            //        }
            //    }
            //}


            if (targetState != dfaState || assignState)
            {
                if (targetState.IsEdge)
                {
                    currentTarget.Assign(exitState.GetReference(), this._StateImpl.GetReference().Assign(targetState.StateValue.ToPrimitive()));
                    /*
                    if (nonGreedyParam != null)
                        currentTarget.If(nonGreedyParam.GetReference()).GoTo(decisionPoint);*/
                }
                else
                    currentTarget.Assign(this._StateImpl.GetReference(), targetState.StateValue.ToPrimitive());
            }
            else if (targetState.IsEdge)
            {
                currentTarget.Assign(exitState.GetReference(), this._StateImpl.GetReference().Assign(targetState.StateValue.ToPrimitive()));
                /*if (nonGreedyParam != null)
                    currentTarget.If(nonGreedyParam.GetReference()).GoTo(decisionPoint);*/
            }
        }

        private static void InjectCaptureLogic(
            IBlockStatement currentTarget,
            Dictionary<IProductionRuleCaptureStructuralItem, ILocalMember> explicitCaptures,
            IProductionRuleCaptureStructuralItem[] targetCaptures,
            IExpression captureExpression)
        {
            StringBuilder captureBuilder = new StringBuilder();
            if (targetCaptures.Length > 0)
            {
                bool firstCapture = true;
                foreach (var element in targetCaptures)
                {
                    var currentLocal = explicitCaptures[element];
                    if (firstCapture)
                        firstCapture = false;
                    else
                        captureBuilder.Append(", ");
                    switch (element.ResultType)
                    {
                        case ResultedDataType.ComplexType:
                            break;
                        case ResultedDataType.Flag:
                        case ResultedDataType.Counter:
                            var counterFlagNullCheck = currentTarget.If(currentLocal.EqualTo(IntermediateGateway.NullValue));
                            if (element.ResultType == ResultedDataType.Counter)
                            {
                                counterFlagNullCheck.Assign(currentLocal.GetReference(), 1.ToPrimitive());
                                counterFlagNullCheck.CreateNext();
                                counterFlagNullCheck.Next.Increment(currentLocal.GetReference());
                            }
                            else
                                counterFlagNullCheck.Assign(currentLocal.GetReference(), IntermediateGateway.TrueValue);
                            break;
                        case ResultedDataType.FlagEnumerationItem:
                            if (element.Sources.AllSourcesAreSameIdentity())
                            {
                                goto case ResultedDataType.Counter;
                            }
                            else
                                goto case ResultedDataType.ImportTypeList;
                        case ResultedDataType.EnumerationItem:
                        case ResultedDataType.Enumeration:
                        case ResultedDataType.Character:
                        case ResultedDataType.String:
                            if (captureExpression is INaryOperandExpression)
                                currentTarget.Assign(currentLocal.GetReference(), (INaryOperandExpression)captureExpression);
                            break;
                        case ResultedDataType.ImportType:
                            if (captureExpression is INaryOperandExpression)
                                currentTarget.Assign(currentLocal.GetReference(), (INaryOperandExpression)captureExpression);
                            break;
                        case ResultedDataType.ImportTypeList:
                            currentTarget.Call(currentLocal.GetReference().GetMethod("Add").Invoke(captureExpression));
                            break;
                    }
                    captureBuilder.AppendFormat("{0} ({1})", element.BucketName, element.ResultType);
                }

                currentTarget.Comment(string.Format("{1}Captures: {0}", captureBuilder, targetCaptures.Length > 1 ? "Multiple " : string.Empty));
            }
            else if (captureExpression is IStatementExpression)
                currentTarget.Add(new ExpressionStatement(currentTarget, (IStatementExpression)captureExpression));
        }

        private static void ExitLADepthCheck(IBlockStatement currentCase, ILocalMember exitLADepth, ILocalMember currentLADepth, SyntacticalDFAState originalState, bool increment = true)
        {
            if (originalState.IsEdge)
                if (increment)
                    currentCase.Assign(exitLADepth.GetReference(), currentLADepth.GetReference().Increment(false));
                else
                    currentCase.Assign(exitLADepth.GetReference(), currentLADepth.GetReference());
            else if (increment)
                currentCase.Increment(currentLADepth.GetReference());
        }
        private static void HandleStatementInjections(
            IBlockStatementParent parentTarget,
            Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData> lookupData)
        {
            foreach (var localItem in lookupData.Values)
            {
                var container = localItem.BlockContainer;
                HandleStatementInjections(parentTarget, container, localItem.Label.Value);
            }
        }

        private static void HandleStatementInjections(
            IBlockStatementParent parentTarget,
            BlockStatementParentContainer container,
            IStatement lastStatement)
        {
            foreach (var localKey in container.Locals.Keys)
            {
                var local = container.Locals[localKey];
                ILocalMember localCopy = null;
                switch (local.TypingMethod)
                {
                    case LocalTypingKind.Dynamic:
                        localCopy = parentTarget.Locals.Add(localKey.Name, local.InitializationExpression, LocalTypingKind.Dynamic);
                        break;
                    case LocalTypingKind.Explicit:
                        var typedLocal = local as ITypedLocalMember;
                        if (typedLocal == null)
                            break;
                        localCopy = parentTarget.Locals.Add(new TypedName(localKey.Name, typedLocal.LocalType), local.InitializationExpression);
                        break;
                    case LocalTypingKind.Implicit:
                        localCopy = parentTarget.Locals.Add(localKey.Name, local.InitializationExpression, LocalTypingKind.Implicit);
                        break;
                    default:
                        break;
                }
                if (localCopy != null)
                    localCopy.AutoDeclare = local.AutoDeclare;
            }
            foreach (var statement in container)
                if (!parentTarget.AddAfter(lastStatement, lastStatement = statement))
                {
                    Debug.Assert(false, "Statement injection failed.");
                    break;
                }
        }

        private void BuildRuleScaffolding(OilexerGrammarProductionRuleEntry rule)
        {
            var adapter = this.Compiler.RuleAdapters[rule];
            var ruleResultsType = this.Compiler.ParseResultsBuilder.InterfaceDetail.Type.MakeGenericClosure(adapter.AssociatedContext.ModelInterface);
            var parseInternalMethod = this.ParserClass.Methods.Add(
                new TypedName("_Parse{0}Internal", this.Compiler.RuleSymbolBuilder.ILanguageRuleSymbol, rule.Name),
                new TypedNameSeries(
                    new TypedName("laDepth", RuntimeCoreType.Int32, this._identityManager)));
            parseInternalMethod.AccessLevel = AccessLevelModifiers.Private;
            adapter.AssociatedContext.ParseInternalMethod = parseInternalMethod;
            this.parseInternalMethods.Add(adapter, parseInternalMethod);

            if (this.Compiler.Source.Options.StartEntry != rule.Name)
                return;

            var parseMethod = this.ParserClass.Methods.Add(
                new TypedName("Parse{0}", ruleResultsType, rule.Name));
            parseMethod.AccessLevel = AccessLevelModifiers.Public;
            this.parseMethods.Add(adapter, parseMethod);
            var results = parseMethod.Locals.Add(this.Compiler.ParseResultsBuilder.ClassDetail.Type.MakeGenericClosure(adapter.AssociatedContext.ModelInterface).WithName("resultsOfParse"));
            results.InitializationExpression = results.LocalType.GetNewExpression();
            parseMethod.Call(SpinErrorContextImpl.GetReference().Invoke());
            var tryBlock = parseMethod.TryCatch();

            var resultRuleLocal = tryBlock.Locals.Add(new TypedName("resultRule", adapter.AssociatedContext.ModelInterface));
            var resultSymbolLocal = tryBlock.Locals.Add(new TypedName("resultParseTree", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol), parseInternalMethod.GetReference().Invoke(IntermediateGateway.NumberZero));
            tryBlock.DefineLocal(resultRuleLocal);
            tryBlock.DefineLocal(resultSymbolLocal);
            resultSymbolLocal.AutoDeclare = false;

            tryBlock.Assign(resultRuleLocal.GetReference(), this.compiler.RuleSymbolBuilder.CreateRule.GetReference(resultSymbolLocal.GetReference()).Invoke().Cast(resultRuleLocal.LocalType));

            tryBlock.Call(HandleErrorContextImpl.GetReference().Invoke(results.GetReference(), resultSymbolLocal.GetReference().Cast(this.Compiler.RuleSymbolBuilder.LanguageRuleSymbol)));
            //tryBlock.If(this._ErrorContextImpl.GetReference().GetProperty("Count").GreaterThan(IntermediateGateway.NumberZero))
            //    .Assign(this.Compiler.ParseResultsBuilder.ClassDetail.SyntaxErrors.GetReference(results.GetReference()), this._ErrorContextImpl.GetReference());
            tryBlock.Assign(this.Compiler.ParseResultsBuilder.ClassDetail.Result.GetReference(results.GetReference()), resultRuleLocal);
            tryBlock.Assign(this.Compiler.ParseResultsBuilder.ClassDetail.Successful.GetReference(results.GetReference()), IntermediateGateway.TrueValue);
            tryBlock.CatchAll.Assign(this.Compiler.ParseResultsBuilder.ClassDetail.Successful.GetReference(results.GetReference()), IntermediateGateway.FalseValue);
            tryBlock.CatchAll.Assign(this.Compiler.ParseResultsBuilder.ClassDetail.SyntaxErrors.GetReference(results.GetReference()), this._ErrorContextImpl.GetReference());
            parseMethod.Return(results.GetReference());
        }

        private IIntermediateClassMethodMember CreatePredictParseMethod(PredictionTreeLeaf projectionPoint, int predictIndex, int predictCount)
        {
            var normalAdapter = this.compiler.AllRuleAdapters[projectionPoint.Rule, projectionPoint.Veins.DFAOriginState];

            var predictMethod = this.ParserClass.Methods.Add(
                new TypedName(string.Format("_Predict{{1:{0}}}On{{0}}", new string('0', predictCount.ToString().Length)), this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32), normalAdapter.AssociatedContext.Leaf.Rule.Name, predictIndex),
                new TypedNameSeries(
                        new TypedName("laDepth", RuntimeCoreType.Int32, this._identityManager)));
            var advMachine = this.compiler.AdvanceMachines[projectionPoint];
            if (projectionPoint.Veins.DFAOriginState is SyntacticalDFARootState)
            {
                if (advMachine.AssociatedContext.IsLeftRecursiveProjection && advMachine.AssociatedContext.RequiresLeftRecursiveCaution)
                {
                    predictMethod.Parameters.Add(new TypedName(includeRuleContextName, RuntimeCoreType.Boolean, this._identityManager));
                    predictMethod.Parameters.Add(new TypedName(nonGreedyName, RuntimeCoreType.Boolean, this._identityManager));
                }
            }
            var parseParent = predictMethod.Parent;
            predictMethod.AccessLevel = AccessLevelModifiers.Private;
            return predictMethod;
        }

        private IIntermediateClassMethodMember CreateFollowPredictParseMethod(PredictionTreeFollow follow, int predictIndex, int predictCount)
        {

            var normalAdapter = this.compiler.AllRuleAdapters[follow.Rule, follow.EdgeNode.Veins.DFAOriginState];
            var followPredictMethod = this.ParserClass.Methods.Add(
                new TypedName(string.Format("_Predict{{1:{0}}}Following{{0}}_", new string('0', predictCount.ToString().Length)), this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32), normalAdapter.AssociatedContext.Leaf.Rule.Name, predictIndex),
                new TypedNameSeries(
                        new TypedName("laDepth", RuntimeCoreType.Int32, this._identityManager)));
            followPredictMethod.AccessLevel = AccessLevelModifiers.Private;
            return followPredictMethod;
        }

        internal enum PredictionReduceType
        {
            /// <summary>
            /// Denotes a simple reduction that doesn't consume any stack-space prior to reaching
            /// the reduction point.
            /// </summary>
            Simple,
            /// <summary>
            /// Denotes a reduction that consumes stack-space prior to reaching the reduction point,
            /// but the 'k' for the reduction is always fixed.  The to/from of the node will denote the
            /// details of this reduction, as this context might change depending on the originating state.
            /// </summary>
            FixedLookAhead,
            /// <summary>
            /// Denotes a reduction that consumes stack-space prior to reaching the reduction point,
            /// and the 'k' for the reduction is unbound due to a deterministic loop noted in the
            /// automation.
            /// </summary>
            VariableLookAhead,
        }

        internal class PredictionReduceDetail :
            IEquatable<PredictionReduceDetail>
        {
            public PredictionReduceType ReductionType { get; set; }
            public SyntacticalDFAState InitialState { get; set; }
            public SyntacticalDFAState FinalState { get; set; }
            public int? KnownDeviation { get; set; }
            public GrammarVocabulary FinalDiscriminator { get; set; }
            public IOilexerGrammarProductionRuleEntry Rule { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is PredictionReduceDetail)
                    return this.Equals((PredictionReduceDetail)(obj));
                return false;
            }

            public bool Equals(PredictionReduceDetail other)
            {
                if (object.ReferenceEquals(other, null))
                    return false;
                if (object.ReferenceEquals(other, this))
                    return true;
                return other.ReductionType == this.ReductionType &&
                       other.InitialState == this.InitialState &&
                       other.FinalState == this.FinalState &&
                       other.KnownDeviation == this.KnownDeviation &&
                       other.FinalDiscriminator.Equals(this.FinalDiscriminator);
            }

            public override int GetHashCode()
            {
                return this.ReductionType.GetHashCode() ^
                    this.InitialState.GetHashCode() ^
                    this.FinalState.GetHashCode() ^
                    this.FinalDiscriminator.GetHashCode() ^
                   (this.KnownDeviation == null
                        ? 0
                        : this.KnownDeviation.Value.GetHashCode());
            }

            public override string ToString()
            {
                StringBuilder result = new StringBuilder();
                result.AppendFormat("Reduce {0} on {1}", this.Rule.Name, this.FinalDiscriminator);
                switch (this.ReductionType)
                {
                    case PredictionReduceType.FixedLookAhead:
                        result.AppendFormat(" after {0} steps{1}.", this.KnownDeviation, this.KnownDeviation != null && this.KnownDeviation.Value > 0 ? " using look-ahead variable" : string.Empty);
                        break;
                    case PredictionReduceType.VariableLookAhead:
                        result.Append(" after '?' steps using look-ahead variable.");
                        break;
                    default:
                        goto yieldResult;
                }
            yieldResult:
                return result.ToString();
            }
        }

        internal class PredictionReduceDetails :
            MultikeyedDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState, HashSet<PredictionReduceDetail>>
        {

        }

        public IIntermediateClassFieldMember _LookAheadDepthsImpl { get; set; }

        public IIntermediateClassMethodMember PeekInitialContextsForImpl { get; set; }

        public IIntermediateClassMethodMember BorrowOuterContext { get; set; }

        public IIntermediateClassMethodMember PeekInitialContextsForImplOvr { get; set; }
    }

    public enum ParserRuleHandlingType
    {
        AsSymbol,
        AsRule,
        AsEither,
    }
}
