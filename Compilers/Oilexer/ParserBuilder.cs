using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using AllenCopeland.Abstraction.Slf.Cli;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Oil;
using AllenCopeland.Abstraction.Slf.Oil.Expressions.CSharp;
using AllenCopeland.Abstraction.Slf.Oil.Expressions;
using AllenCopeland.Abstraction.Slf.Oil.Members;
using AllenCopeland.Abstraction.Slf.Oil.Statements;
using AllenCopeland.Abstraction.Slf.Parsers;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Provides a builder for the parser that results
    /// from a grammar description file.
    /// </summary>
    public class ParserBuilder
    {
        /// <summary>
        /// Returns the <see cref="GDFile"/> which describes the structure
        /// of the grammar to build a parser for.
        /// </summary>
        public GDFile Source { get; private set; }

        /// <summary>
        /// Returns a series of <see cref="ICompilerErrorCollection"/> instances
        /// which denote where errors occur in identity resolution,
        /// usage of templates, or other base compiler errors.
        /// </summary>
        public ICompilerErrorCollection CompilationErrors { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateAssembly"/> which represents
        /// the resulted parser.
        /// </summary>
        public IIntermediateAssembly Project { get; private set; }

        /// <summary>
        /// Returns a dictionary containing timing information for each phase
        /// of the build process.
        /// </summary>
        public Dictionary<ParserBuilderPhase, TimeSpan> PhaseTimes { get; private set; }

        public ReadOnlyDictionary<IProductionRuleEntry, SyntacticalDFARootState> RuleDFAStates { get; private set; }

        public IProductionRuleEntry StartEntry { get; private set; }

        /// <summary>
        /// Returns a series of token precedences associated to the tokens.
        /// </summary>
        internal TokenPrecedenceTable Precedences { get; private set; }

        internal RegularLanguageLexer LexicalAnalyzer { get; private set; }

        internal SyntacticalParser SyntaxParser { get; private set; }

        public IGrammarSymbolSet GrammarSymbols { get; private set; }

        public ITokenEntry EOFToken { get; private set; }

        private Dictionary<IProductionRuleEntry, SyntacticalNFAState> ruleNFAStates { get; set; }

        private Dictionary<IProductionRuleEntry, SyntacticalDFARootState> ruleDFAStates { get; set; }

        public IReadOnlyCollection<string> StreamAnalysisFiles { get; private set; }

        private IGDFileObjectRelationalMap fileRelationalMap;

        private CharStreamClass bitStream;

        public event EventHandler<ParserBuilderPhaseChangeEventArgs> PhaseChange;


        public ParserBuilder(IGDFile source, List<string> streamAnalysisFiles)
        {
            this.CompilationErrors = new CompilerErrorCollection();
            this.PhaseTimes = new Dictionary<ParserBuilderPhase, TimeSpan>();
            this.Source = (GDFile)source;
            this.StreamAnalysisFiles = new ReadOnlyCollection<string>(streamAnalysisFiles);
        }

        private void ConstructTokenNFA()
        {
            this.EOFToken = (ITokenEofEntry)this.Source.FirstOrDefault(f => f is ITokenEofEntry);
            /* *
             * Overview:
             *      1. Determine each token type and construct 
             *         appropriate data model for each token.
             *      2. Order tokens via precedence.
             *      3. Construct general NFA from inlined tokens.
             * */
            //this.Source.GetTokens().OnAll(token =>
            //    token.BuildNFA());
            foreach (var token in this.Source.GetTokens())
                token.BuildNFA();
        }

        private void ConstructTokenDFA()
        {
            /* *
             * Overview:
             *      Construct the appropriate Deterministic 
             *      state machine for each kind of token using
             *      the information gained in the previous step,
             *      to decide how much reduction to perform 
             *      post-DFA construction.
             * */
            this.Source.GetTokens().AsParallel()
              .ForAll(token =>
                token.BuildDFA());
        }

        private void ReduceTokenDFA()
        {
            /* *
             * Overview:
             *      Reduce each token's deterministic
             *      state machine to its minimal form.
             * */
            var tokensArr = this.Source.GetTokens().ToArray();
            tokensArr.AsParallel().ForAll(token =>
                token.ReduceDFA());
        }

        private void ReduceRuleDFA()
        {
            var rulesArr = (from rule in this.Source.GetRules()
                           select this.ruleDFAStates[rule]).ToArray();
            rulesArr.AsParallel().ForAll(rule => { lock (rule) rule.ReduceDFA(); });

            this.LexicalAnalyzer = new RegularLanguageLexer(this);
        }

        private void ConstructRuleNFA()
        {
            this.GrammarSymbols = new GrammarSymbolSet(this.Source);
            this.ruleNFAStates = new Dictionary<IProductionRuleEntry, SyntacticalNFAState>();
            var rules = (from rule in Source.GetRules()
                         orderby rule.Name
                         select rule).ToArray();
            var result = new SyntacticalNFARootState[rules.Length];
            int i = 0;
            rules.OnAll(p => new { Rule = p, Index = i++ }).AsParallel().ForAll(ruleAndIndex =>
                result[ruleAndIndex.Index] = (SyntacticalNFARootState)ruleAndIndex.Rule.BuildNFA(new SyntacticalNFARootState(ruleAndIndex.Rule, this), this.GrammarSymbols, this));
            for (i = 0; i < result.Length; i++)
            {
                var current = result[i];
                this.ruleNFAStates.Add(current.Source, current);
            }
        }

        private void ConstructRuleDFA()
        {
            this.ruleDFAStates = new Dictionary<IProductionRuleEntry, SyntacticalDFARootState>();
            int i = 0;
            var rules = (from rule in Source.GetRules()
                         orderby rule.Name
                         select rule).ToArray();
            SyntacticalDFARootState[] result = new SyntacticalDFARootState[rules.Length];
            rules.OnAll(p => new { Rule = p, Index = i++ }).AsParallel().ForAll(ruleAndIndex => result[ruleAndIndex.Index] = (SyntacticalDFARootState)this.ruleNFAStates[ruleAndIndex.Rule].DeterminateAutomata());
            for (i = 0; i < result.Length; i++)
            {
                var current = result[i];
                ruleDFAStates.Add(current.Entry, current);
            }
            this.RuleDFAStates = new ReadOnlyDictionary<IProductionRuleEntry, SyntacticalDFARootState>(this.ruleDFAStates);
        }

        private void PerformStreamAnalysis()
        {
            this.ReduceRuleDFA();
            this.RuleDFAStates[this.StartEntry].Connect();
            this.SyntaxParser = new SyntacticalParser(this);
            foreach (var file in this.StreamAnalysisFiles)
                this.SyntaxParser.Parse(file);
        }

        private void BuildCodeModel(IIntermediateAssembly project)
        {
            this.bitStream = BitStreamCreator.CreateBitStream(project.DefaultNamespace);
            this.fileRelationalMap = new GDFileObjectRelationalMap(this.Source, this.RuleDFAStates, project);
        }

        private void BuildStateMachine(InlinedTokenEntry token, IIntermediateAssembly project, CharStreamClass charStream)
        {
            var tokenDFA = token.DFAState;
            var tokenName = token.Name;
            if (tokenDFA == null)
                return;

            switch (token.CaptureKind)
            {
                case RegularCaptureType.Recognizer:
                    BuildRecognizerMachine(project, charStream, tokenDFA, tokenName);
                    break;
                case RegularCaptureType.Capturer:
                    BuildRecognizerMachine(project, charStream, tokenDFA, tokenName);
                    break;
                case RegularCaptureType.Transducer:
                    BuildRecognizerMachine(project, charStream, tokenDFA, tokenName);
                    break;
                case RegularCaptureType.Undecided:
                    break;
            }
        }

        private void BuildTransducerMachine(IIntermediateAssembly project, RegularLanguageDFARootState tokenDFA, string tokenName)
        {
            
        }

        private void BuildRecognizerMachine(IIntermediateAssembly project, CharStreamClass charStream, RegularLanguageDFAState tokenDFA, string tokenName)
        {
            //Setup the basic class, access level, and base-type.
            IIntermediateNamespaceDeclaration targetNamespace = project.DefaultNamespace.Parts.Add();
            IIntermediateClassType targetType;
            targetType = targetNamespace.Classes.Add(string.Format("{0}StateMachine", tokenName));
            var stateMachine = targetType;
            stateMachine.AccessLevel = AccessLevelModifiers.Internal;
            stateMachine.BaseType = charStream.BitStream;

            /* *
             * Setup the 'next character' state movement method,
             * the state and exit-length variables.
             * */
            
            var nextMethod = stateMachine.Methods.Add(new TypedName("Next", typeof(bool).GetTypeReference()));
            nextMethod.AccessLevel = AccessLevelModifiers.Public;
            var nextChar = nextMethod.Parameters.Add(new TypedName("currentChar", typeof(char).GetTypeReference()));
            //nextMethod.Summary = string.Format("Moves the state machine into its next state with the @p:{0};.", nextChar.Name);
            //nextChar.DocumentationComment = "The next character used as the condition for state->state transitions.";
            var stateField = stateMachine.Fields.Add(new TypedName("state", typeof(int).GetTypeReference()));
            stateField.InitializationExpression = IntermediateGateway.NumberZero;
            //stateField.Summary = "The state machine's current state, determining the logic path to follow for the next character.";
            var exitLength = stateMachine.Fields.Add(new TypedName("exitLength", typeof(int).GetTypeReference()));

            /* *
             * Obtain all the states in the deterministic automation.
             * */
            var flatForm = new List<RegularLanguageDFAState>();
            RegularLanguageDFAState.FlatlineState(tokenDFA, flatForm);
            if (!flatForm.Contains(tokenDFA))
                flatForm.Add(tokenDFA);
            flatForm = (from f in flatForm
                        orderby f.StateValue
                        select f).ToList();
            /* *
             * Setup the main state switch, and a basic anonymous-type 
             * state-data construct.
             * */
            var stateSwitch = nextMethod.Switch(stateField.GetReference());
            IUnicodeCollectiveTargetGraph collectiveGraph = new UnicodeCollectiveTargetGraph();
            nextMethod.Return(IntermediateGateway.FalseValue);
            var stateCodeData = GetDictionary((RegularLanguageDFAState)null,
                new
                {
                    SwitchCase = (ISwitchCaseBlockStatement)null,
                    Label = (ILabelStatement)null,
                    Graph = (IUnicodeTargetGraph)null,
                    GraphLabel = (ILabelStatement)null,
                    TransitionTable = (Dictionary<RegularLanguageSet, RegularLanguageDFAState>)null,
                });
            ILabelStatement terminalExit = null;
            ILabelStatement nominalExit = null;
            ILabelStatement commonMove = null;
            /* *
             * Make the initial pass over the states of the machine, convert the
             * transition tables into unicode-aware variants.
             * *
             * This ensures that massive thousand character sets aren't generated,
             * while also keeping in mind partial unicode sets.
             * */
            var graphLabels = new Dictionary<IUnicodeTargetGraph, ILabelStatement>();
            var insertedGraphs = new List<IUnicodeTargetGraph>();
            var insertedStates = new List<RegularLanguageDFAState>();
            int unicodeGraphCount = 0;
            var sourceSet = tokenDFA.SourceSet.Cast<ITokenItem>().ToArray();
            var sourceNames = (from s in sourceSet
                               select s.Name).Distinct().ToArray();
            var sources = sourceNames.ToDictionary(p => p, p => (from s in sourceSet
                                                                 where s.Name == p
                                                                 select s).ToArray());
            foreach (var state in flatForm)
            {
                ISwitchCaseBlockStatement stateCase = null;
                ILabelStatement graphLabel = null;
                ILabelStatement label = null;
                IUnicodeTargetGraph graph = null;
                //var activeNamedSources = (from s in state.Sources
                //                          where sourceSet.Contains(s.Item1)
                //                          select s).ToDictionary(p => (ITokenItem)p.Item1, p => p.Item2);

                Dictionary<RegularLanguageSet, RegularLanguageDFAState> finalTransitionTable = null;
                if (state.OutTransitions.Count == 0)
                    goto skipGraph;
                graph = new UnicodeTargetGraph();
                var fullSet = state.OutTransitions.FullCheck;
                finalTransitionTable = new Dictionary<RegularLanguageSet, RegularLanguageDFAState>();
                foreach (var transition in state.OutTransitions)
                {
                    /* *
                     * Send in the current transition's requirement, along with the full set
                     * to breakdown the unicode subsets contained within.
                     * */
                    var breakdown = Breakdown(transition.Key, fullSet);
                    /* *
                     * If the remainder of the unicode breakdown does not overlap enough
                     * of a category to include it, denote the remainder.
                     * */
                    if (breakdown.Item1 != null && !breakdown.Item1.IsEmpty)
                        finalTransitionTable.Add(breakdown.Item1, transition.Value);
                    /* *
                     * If there are partial and full unicode sets,
                     * push them into the unicode target logic graph.
                     * */
                    if (breakdown.Item2.Length > 0 ||
                        breakdown.Item3.Count > 0)
                    {
                        IUnicodeTarget target = null;
                        if (!graph.TryGetValue(transition.Value, out target))
                            target = graph.Add(transition.Value, transition.Value == state);
                        //Full sets are simple.
                        foreach (var category in breakdown.Item2)
                            target.Add(category);
                        var item3 = breakdown.Item3;
                        /* *
                         * Partial sets are a bit more interesting.
                         * */
                        foreach (var partialCategory in item3.Keys)
                        {
                            /* *
                             * If the partial set doesn't contain a remainder,
                             * the original remainder was consumed by the overall 
                             * checks that occur before it.
                             * *
                             * As an example, if the category is Ll, assuming there
                             * are other paths that utilize a-z, the original check used to
                             * construct the unicode breakdown would note this, but
                             * the full set sent into the breakdown method would negate
                             * the negative set (if a-z are already checked,
                             * there is no need to check that the character -isn't- 
                             * in that range).
                             * */
                            if (item3[partialCategory] == null)
                                target.Add(partialCategory);
                            else
                                target.Add(partialCategory, item3[partialCategory]);
                        }
                    }
                }
                if (graph.Count > 0)
                {
                    /* *
                     * Based upon the full groups and partials above for the current
                     * state's output, check the collective graph to see if a duplicate
                     * exists.
                     * *
                     * If the target of the transition is identical to the current state,
                     * the graph will be unique, this is to avoid unnecessary assignments
                     * to the current state.
                     * */
                    IUnicodeTargetGraph findResults = collectiveGraph.Find(graph);
                    if (findResults == null)
                    {
                        collectiveGraph.Add(graph);
                        graphLabels.Add(graph, graphLabel = new LabelStatement(nextMethod, string.Format("UnicodeGraph_{0}", ++unicodeGraphCount)));
                    }
                    else
                    {
                        graph = findResults;
                        graphLabel = graphLabels[findResults];
                    }
                }
                else
                    graph = null;
                stateCase = stateSwitch.Case(state.StateValue.ToPrimitive());
            skipGraph:
                if (state.InTransitions.Count > 0)
                    /* *
                     * No need to provide a move-to label if the state,
                     * which designates the origin, is the state itself.
                     * *
                     * Such a case uses a common move or nominal exit depending
                     * on whether the state is an edge.
                     * */
                    if (!(state.InTransitions.Count == 1 && state.InTransitions[0].Value.Count == 1 && state.InTransitions[0].Value[0] == state))
                        label = new LabelStatement(nextMethod, string.Format("MoveToState_{0}", state.StateValue));
                stateCodeData.Add(state,
                    new
                    {
                        SwitchCase = stateCase,
                        Label = label,
                        Graph = graph,
                        GraphLabel = graphLabel,
                        TransitionTable = finalTransitionTable,
                    });
            }
            /* *
             * Build the individual functional areas relative to the
             * information provided within their transition data.
             * */
            foreach (var state in stateCodeData.Keys)
            {
                var stateData = stateCodeData[state];
                var stateCase = stateData.SwitchCase;
                var stateGraph = stateData.Graph;
                var graphLabel = stateData.GraphLabel;
                var transitionTable = stateData.TransitionTable;
                var currentTarget = (IBlockStatementParent)stateCase;
                /* *
                 * Assuming the entire transition table wasn't consumed by 
                 * the unicode graph translation, emit the necessary
                 * transitional logic for those remaining.
                 * */
                if (transitionTable != null)
                    foreach (var check in transitionTable.Keys)
                    {
                        IExpression finalExpression = null;
                        var target = transitionTable[check];
                        var targetLabel = stateCodeData[target].Label;
                        DiscernForwardTarget(charStream, nextMethod, nextChar, stateField, exitLength, ref terminalExit, ref nominalExit, ref commonMove, insertedStates, state, target, ref targetLabel);
                        foreach (var rangeElement in check.GetRange())
                        {
                            IExpression currentExpression = null;
                            switch (rangeElement.Which)
                            {
                                case RegularLanguageSet.SwitchPairElement.A:
                                    currentExpression = nextChar.EqualTo(rangeElement.A.Value);
                                    break;
                                case RegularLanguageSet.SwitchPairElement.B:
                                    var start = rangeElement.B.Value.Start;
                                    var end = rangeElement.B.Value.End;
                                    /* *
                                     * 'start' <= nextChar && nextChar <= 'end'
                                     * */
                                    currentExpression = start.LessThanOrEqualTo(nextChar).LogicalAnd(nextChar.LessThanOrEqualTo(end));
                                    break;
                                default:
                                    break;
                            }
                            if (finalExpression == null)
                                finalExpression = currentExpression;
                            else
                                finalExpression = finalExpression.LogicalOr(currentExpression);
                        }
                        var currentCondition = currentTarget.If(finalExpression);
                        currentCondition.GoTo(targetLabel);
                        currentTarget = currentCondition.Next;
                    }
                if (stateGraph != null)
                {
                    if (stateGraph.Count == 1)
                    {
                        var first = stateGraph[0].Key;
                        var firstTarget = stateGraph[first];
                        if (ParserCompilerExtensions.unicodeCategoryData.Keys.All(
                            p => firstTarget.Keys.Contains(p)))
                        {
                            ILabelStatement targetLabel = stateCodeData[first].Label;
                            DiscernForwardTarget(charStream, nextMethod, nextChar, stateField, exitLength, ref terminalExit, ref nominalExit, ref commonMove, insertedStates, state, first, ref targetLabel);

                            RegularLanguageSet negativeSet = null;
                            foreach (var category in firstTarget.Values)
                            {
                                if (category is IUnicodeTargetPartialCategory)
                                {
                                    var partialCategory = (IUnicodeTargetPartialCategory)category;
                                    if (negativeSet == null)
                                        negativeSet = partialCategory.NegativeAssertion;
                                    else
                                        negativeSet |= partialCategory.NegativeAssertion;
                                }
                            }
                            if (negativeSet != null)
                                currentTarget = currentTarget.If(ObtainNegativeAssertion(nextChar, negativeSet));
                            currentTarget.GoTo(targetLabel);

                            goto FullUnicodeCoverage;
                        }
                    }
                    currentTarget.GoTo(graphLabel);
                    if (!insertedGraphs.Contains(stateGraph))
                    {
                        currentTarget = nextMethod;
                        nextMethod.DefineLabel(graphLabel);
                        var categoryGetExpr = typeof(char).GetTypeExpression().GetMethod("GetUnicodeCategory").Invoke(nextChar.GetReference());
                        var graphSwitch = nextMethod.Switch(categoryGetExpr);
                        nextMethod.Return(IntermediateGateway.FalseValue);
                        foreach (var target in stateGraph.Keys)
                        {
                            var currentGraphTarget = stateGraph[target];
                            List<IUnicodeTargetCategory> fullCategories = new List<IUnicodeTargetCategory>();
                            List<IUnicodeTargetPartialCategory> partialCategories = new List<IUnicodeTargetPartialCategory>();
                            foreach (var category in currentGraphTarget.Keys)
                                if (currentGraphTarget[category] is IUnicodeTargetPartialCategory)
                                    partialCategories.Add(((IUnicodeTargetPartialCategory)(currentGraphTarget[category])));
                                else
                                    fullCategories.Add(currentGraphTarget[category]);
                            ILabelStatement targetLabel = stateCodeData[target].Label;
                            DiscernForwardTarget(charStream, nextMethod, nextChar, stateField, exitLength, ref terminalExit, ref nominalExit, ref commonMove, insertedStates, state, target, ref targetLabel);
                            var fullCategory = graphSwitch.Case();
                            fullCategory.IsDefault = false;

                            foreach (var category in fullCategories)
                                fullCategory.Cases.Add(typeof(UnicodeCategory).GetTypeExpression().GetField(category.TargetedCategory.ToString()));
                            fullCategory.GoTo(targetLabel);

                            foreach (var category in partialCategories)
                            {
                                IExpression finalExpression = ObtainNegativeAssertion(nextChar, category.NegativeAssertion);
                                var currentCategory = graphSwitch.Case(typeof(UnicodeCategory).GetTypeExpression().GetField(category.TargetedCategory.ToString()));
                                var currentCondition = currentCategory.If(finalExpression);
                                currentCondition.GoTo(targetLabel);
                            }
                        }
                        insertedGraphs.Add(stateGraph);
                    }
                FullUnicodeCoverage: ;
                }
            }
        }

        private void DiscernForwardTarget(CharStreamClass charStream, IIntermediateMethodMember nextMethod, IIntermediateMethodParameterMember nextChar, IIntermediateFieldMember stateField, IIntermediateFieldMember exitLength, ref ILabelStatement terminalExit, ref ILabelStatement nominalExit, ref ILabelStatement commonMove, List<RegularLanguageDFAState> insertedStates, RegularLanguageDFAState activeState, RegularLanguageDFAState destinationState, ref ILabelStatement targetLabel)
        {
            /* *
             * The target transition label is normal, it targets 
             * a known state.
             * */
            const int TARGET_NORMAL = 0;
            /* *
             * The target transition label is the active state,
             * which is also an edge, so the nominal (satisfactory)
             * label is targeted, since it targets itself, no state
             * change need be made, but the exit-length is changed.
             * */
            const int TARGET_NOMINAL = 1;
            /* *
             * The target transition label is the active state,
             * which is not an edge, so the common state
             * move is targeted, which does not alter the state,
             * but doesn't alter the exit length, either.
             * */
            const int TARGET_COMMON = 2;
            int targetLabelKind = TARGET_NORMAL;
            if (destinationState == activeState)
                if (destinationState.IsEdge)
                    targetLabelKind = TARGET_NOMINAL;
                else
                    targetLabelKind = TARGET_COMMON;
            switch (targetLabelKind)
            {
                case TARGET_NORMAL:
                    ImplementStateMoveCheck(charStream, nextMethod, nextChar, targetLabel, insertedStates, destinationState, ref commonMove, ref nominalExit, ref terminalExit, exitLength, stateField);
                    break;
                case TARGET_COMMON:
                    ImplementCommonMoveCheck(charStream, nextMethod, nextChar, ref commonMove);
                    targetLabel = commonMove;
                    break;
                case TARGET_NOMINAL:
                    ImplementNominalExitCheck(charStream, nextMethod, nextChar, ref nominalExit, exitLength);
                    targetLabel = nominalExit;
                    break;
            }
        }

        private static IExpression ObtainNegativeAssertion(IIntermediateMethodParameterMember nextChar, RegularLanguageSet regularSet)
        {
            IExpression finalExpression = null;
            foreach (var rangeElement in regularSet.GetRange())
            {
                IExpression currentExpression = null;
                switch (rangeElement.Which)
                {
                    case RegularLanguageSet.SwitchPairElement.A:
                        var value = rangeElement.A.Value;
                        //nextChar != 'value'
                        currentExpression = nextChar.InequalTo(value);
                        /* *
                         * Old version:
                         * *
                         * new BinaryOperationExpression(
                         *     nextChar.GetReference(), 
                         *     CodeBinaryOperatorType.IdentityInequality, 
                         *     new PrimitiveExpression(value));
                         * */
                        break;
                    case RegularLanguageSet.SwitchPairElement.B:
                        var start = rangeElement.B.Value.Start;
                        var end = rangeElement.B.Value.End;
                        /* *
                         * 'start' > nextChar || nextChar > 'end'
                         * */
                        currentExpression = start.GreaterThan(nextChar).LogicalOr(nextChar.GreaterThan(end));
                        /* *
                         * Old variant below makes less sense due to the 'type verbosity'.
                         * *
                         * new BinaryOperationExpression(
                         *     new BinaryOperationExpression(
                         *         nextChar.GetReference(), 
                         *         CodeBinaryOperatorType.LessThan, 
                         *         new PrimitiveExpression(start)), 
                         *      CodeBinaryOperatorType.BooleanOr, 
                         *      new BinaryOperationExpression(
                         *          nextChar.GetReference(), 
                         *          CodeBinaryOperatorType.GreaterThan, 
                         *          new PrimitiveExpression(end)));
                         * */
                        break;
                    default:
                        break;
                }
                if (finalExpression == null)
                    finalExpression = currentExpression;
                else
                    finalExpression = finalExpression.LogicalAnd(currentExpression);
            }
            return finalExpression;
        }


        private void ImplementStateMoveCheck(CharStreamClass charStream, IIntermediateMethodMember nextMethod, IIntermediateMethodParameterMember nextChar, ILabelStatement targetLabel, List<RegularLanguageDFAState> insertedStates, RegularLanguageDFAState target, ref ILabelStatement commonMove, ref ILabelStatement nominalExit, ref ILabelStatement terminalExit, IIntermediateFieldMember exitLength, IIntermediateFieldMember stateField)
        {
            if (!insertedStates.Contains(target))
            {
                ILabelStatement followUp;
                if (target.IsEdge)
                {
                    if (target.OutTransitions.Count == 0)
                    {
                        ImplementTerminalExitCheck(charStream, nextMethod, nextChar, ref terminalExit, exitLength);
                        followUp = terminalExit;
                    }
                    else
                    {
                        ImplementNominalExitCheck(charStream, nextMethod, nextChar, ref nominalExit, exitLength);
                        followUp = nominalExit;
                    }
                }
                else
                {
                    ImplementCommonMoveCheck(charStream, nextMethod, nextChar, ref commonMove);
                    followUp = commonMove;
                }
                nextMethod.DefineLabel(targetLabel);
                nextMethod.Assign(stateField.GetReference(), target.StateValue.ToPrimitive());
                nextMethod.GoTo(followUp);
                insertedStates.Add(target);
            }
        }

        private static void ImplementNominalExitCheck(CharStreamClass charStream, IIntermediateMethodMember nextMethod, IIntermediateMethodParameterMember nextChar, ref ILabelStatement nominalExit, IIntermediateFieldMember exitLength)
        {
            if (nominalExit == null)
            {
                nominalExit = nextMethod.DefineLabel("NominalExit");
                nextMethod.Call(charStream.PushCharMethod.GetReference().Invoke(nextChar.GetReference()));
                nextMethod.Assign(exitLength.GetReference(), charStream.ActualSize.GetReference());
                nextMethod.Return(IntermediateGateway.TrueValue);
            }
        }

        private static void ImplementTerminalExitCheck(CharStreamClass charStream, IIntermediateMethodMember nextMethod, IIntermediateMethodParameterMember nextChar, ref ILabelStatement terminalExit, IIntermediateFieldMember exitLength)
        {
            if (terminalExit == null)
            {
                terminalExit = nextMethod.DefineLabel("TerminalExit");// new LabelStatement("TerminalExit");
                nextMethod.Call(charStream.PushCharMethod.GetReference().Invoke(nextChar.GetReference()));
                nextMethod.Assign(exitLength.GetReference(), charStream.ActualSize.GetReference());
                nextMethod.Return(IntermediateGateway.FalseValue);
            }
        }

        private static void ImplementCommonMoveCheck(CharStreamClass charStream, IIntermediateMethodMember nextMethod, IIntermediateMethodParameterMember nextChar, ref ILabelStatement commonMove)
        {
            if (commonMove == null)
            {
                commonMove = nextMethod.DefineLabel("CommonMove");
                nextMethod.Call(charStream.PushCharMethod.GetReference().Invoke(nextChar.GetReference()));
                nextMethod.Return(IntermediateGateway.TrueValue);
            }
        }

        private static IBlockStatement EmitCategoryLogic(IIntermediateMethodParameterMember nextChar, IIntermediateFieldMember stateField, IBlockStatement currentTarget, RegularLanguageDFAState targetState, RegularLanguageSet negativeAssertion)
        {
            if (negativeAssertion == null)
                currentTarget.Assign(stateField.GetReference(), targetState.StateValue.ToPrimitive());
            else
            {
                IExpression finalExpression = null;
                var currentRange = negativeAssertion.GetRange();

                foreach (var rangeElement in currentRange)
                {
                    IExpression currentExpression = null;
                    switch (rangeElement.Which)
                    {
                        case RegularLanguageSet.SwitchPairElement.A:
                            currentExpression = nextChar.EqualTo(rangeElement.A.Value);
                            //currentExpression = new CSharpInequalityExpression(nextChar.GetReference(), false, rangeElement.A.Value.ToPrimitive());
                            break;
                        case RegularLanguageSet.SwitchPairElement.B:
                            var start=rangeElement.B.Value.Start;
                            var end=rangeElement.B.Value.End;
                            currentExpression = start.LessThan(nextChar).LogicalOr(nextChar.GreaterThan(end));
                            //currentExpression = new CSharpLogicalOrExpression(new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.LessThan, new PrimitiveExpression(rangeElement.B.Value.Start)), CodeBinaryOperatorType.BooleanOr, new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.GreaterThan, new PrimitiveExpression(rangeElement.B.Value.End)));
                            break;
                    }
                    if (finalExpression == null)
                        finalExpression = currentExpression;
                    else
                        finalExpression = finalExpression.LogicalAnd(currentExpression);
                        //finalExpression = new BinaryOperationExpression(finalExpression, CodeBinaryOperatorType.BooleanAnd, currentExpression);
                }
                var currentCondition = currentTarget.If(finalExpression);
                currentCondition.Assign(stateField.GetReference(), targetState.StateValue.ToPrimitive());
                currentTarget = currentCondition.Next;
            }
            return currentTarget;
        }

        private static List<T> GetList<T>(T inst)
        {
            return new List<T>();
        }

        private static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(TKey key,TValue value)
        {
            return new Dictionary<TKey, TValue>();
        }

        private static Tuple<RegularLanguageSet, UnicodeCategory[], Dictionary<UnicodeCategory, RegularLanguageSet>> Breakdown(RegularLanguageSet check, RegularLanguageSet fullCheck)
        {
            //if (check.IsNegativeSet)
            //    return new Tuple<RegularLanguageSet, UnicodeCategory[], Dictionary<UnicodeCategory, RegularLanguageSet>>(check, new UnicodeCategory[0], new Dictionary<UnicodeCategory, RegularLanguageSet>());
            var noise = fullCheck.RelativeComplement(check);
            UnicodeCategory[] unicodeCategories;
            Dictionary<UnicodeCategory, RegularLanguageSet> partialCategories = new Dictionary<UnicodeCategory, RegularLanguageSet>();
            ParserCompilerExtensions.PropagateUnicodeCategoriesAndPartials(ref check, noise, out unicodeCategories, out partialCategories);
            return new Tuple<RegularLanguageSet, UnicodeCategory[], Dictionary<UnicodeCategory, RegularLanguageSet>>(check, unicodeCategories, partialCategories);
        }

        public void BuildProject()
        {
            if (this.phase != ParserBuilderPhase.None)
                throw new InvalidOperationException("Build in progress.");
            try
            {
                IIntermediateAssembly project = null;
                if (this.Source == null)
                    goto finished;
                project = IntermediateGateway.CreateAssembly(Source.Options.AssemblyName);
                if (Source.Options.Namespace == null)
                    project.DefaultNamespace = project.Namespaces.Add("OILexer.DefaultNamespace");
                else
                    project.DefaultNamespace = project.Namespaces.Add(Source.Options.Namespace);
                Stopwatch timer = new Stopwatch();

                Source.InitLookups();
                ((GDFile)Source).Add(new TokenEofEntry(Source.GetTokenEnumerator().ToArray()));
                Phase = ParserBuilderPhase.Linking;
                timer.Start();
                this.Source.ResolveTemplates(this.CompilationErrors);
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.Linking, timer.Elapsed);
                timer.Reset();
                if (CompilationErrors.HasErrors)
                    goto finished;
                Phase = ParserBuilderPhase.ExpandingTemplates;
                timer.Start();
                this.Source.ExpandTemplates(this.CompilationErrors);
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.ExpandingTemplates, timer.Elapsed);
                timer.Reset();
                if (CompilationErrors.HasErrors)
                    goto finished;
                Phase = ParserBuilderPhase.LiteralLookup;
                timer.Start();
                this.Source.FinalLink(this.CompilationErrors);
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.LiteralLookup, timer.Elapsed);
                timer.Reset();
                if (CompilationErrors.HasErrors)
                    goto finished;
                Phase = ParserBuilderPhase.InliningTokens;
                timer.Start();
                bool inlineSuccess = ParserCompilerExtensions.InlineTokens(Source);
                if (inlineSuccess)
                    this.Precedences = new TokenPrecedenceTable(this.Source.GetTokens());
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.InliningTokens, timer.Elapsed);
                timer.Reset();
                if (!inlineSuccess)
                    goto finished;
                ParserCompilerExtensions.CleanupRules();
                FindStartRule();
                if (this.StartEntry == null)
                    goto finished;
                Phase = ParserBuilderPhase.TokenNFAConstruction;
                timer.Start();
                ConstructTokenNFA();
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.TokenNFAConstruction, timer.Elapsed);
                timer.Reset();
                Phase = ParserBuilderPhase.TokenDFAConstruction;
                timer.Start();
                ConstructTokenDFA();
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.TokenDFAConstruction, timer.Elapsed);
                timer.Reset();
                Phase = ParserBuilderPhase.TokenDFAReduction;
                timer.Start();
                this.ReduceTokenDFA();
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.TokenDFAReduction, timer.Elapsed);
                timer.Reset();
                Phase = ParserBuilderPhase.RuleNFAConstruction;
                timer.Start();
                this.ConstructRuleNFA();
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.RuleNFAConstruction, timer.Elapsed);
                timer.Reset();
                Phase = ParserBuilderPhase.RuleDFAConstruction;
                timer.Start();
                this.ConstructRuleDFA();
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.RuleDFAConstruction, timer.Elapsed);
                timer.Reset();
                Phase = ParserBuilderPhase.CallTreeAnalysis;
                timer.Start();
                this.PerformStreamAnalysis();
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.CallTreeAnalysis, timer.Elapsed);
                timer.Reset();
                Phase = ParserBuilderPhase.ObjectModelRootTypesConstruction;
                timer.Start();
                BuildCodeModel(project);
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.ObjectModelRootTypesConstruction, timer.Elapsed);
                timer.Reset();
                Phase = ParserBuilderPhase.ObjectModelTokenCaptureConstruction;
                timer.Start();
                this.BuildCodeModelCaptures(project);
                timer.Stop();
                PhaseTimes.Add(ParserBuilderPhase.ObjectModelTokenCaptureConstruction, timer.Elapsed);
                timer.Reset();
                Phase = ParserBuilderPhase.ObjectModelTokenEnumConstruction;
                this.BuildCodeModelEnums();
                Phase = ParserBuilderPhase.ObjectModelRuleStructureConstruction;
                Phase = ParserBuilderPhase.ObjectModelFinalTypesConstruction;
                goto finished;
            finished:
                if (this.CompilationErrors.Count == 0)
                    this.Project = project;
            }
            finally
            {
                LinkerCore.errorEntries = null;
                LinkerCore.tokenEntries = null;
                LinkerCore.ruleEntries = null;
                LinkerCore.ruleTemplEntries = null;
                this.Phase = ParserBuilderPhase.None;
            }
        }

        private ParserBuilderPhase phase;
        public ParserBuilderPhase Phase
        {
            get { return this.phase; }
            set
            {
                if (value == phase)
                    return;
                this.phase = value;
                this.OnPhaseChange(value);
            }
        }

        private void OnPhaseChange(ParserBuilderPhase phase)
        {
            if (this.PhaseChange != null)
                this.PhaseChange(this, new ParserBuilderPhaseChangeEventArgs(phase));
        }

        private void FindStartRule()
        {
            IProductionRuleEntry startRule;
            if (Source.Options.StartEntry == null || Source.Options.StartEntry == string.Empty)
            {
                CompilationErrors.SourceModelError<GDFile>(GrammarCore.CompilerErrors.NoStartDefined, 0, 0, Source.Files[0], Source, Source.Options.GrammarName);
                return;
            }
            if ((startRule = (this.Source.GetRules()).FindScannableEntry(Source.Options.StartEntry)) == null)
            {
                CompilationErrors.SourceModelError<GDFile>(GrammarCore.CompilerErrors.InvalidStartDefined, 0, 0, Source.Files[0], Source, Source.Options.StartEntry, Source.Options.GrammarName);
                return;
            }
            this.StartEntry = startRule;
        }

        private void BuildCodeModelEnums()
        {
        }

        private void BuildCodeModelCaptures(IIntermediateAssembly project)
        {
            foreach (var token in this.Source.GetTokens())
                BuildStateMachine(token, project, bitStream);
        }



    }
}
