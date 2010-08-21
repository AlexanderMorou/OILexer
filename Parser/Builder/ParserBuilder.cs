using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Oilexer._Internal;
using Oilexer._Internal.Inlining;
using Oilexer.Expression;
using Oilexer.FiniteAutomata.Rules;
using Oilexer.FiniteAutomata.Tokens;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Statements;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Utilities.Collections;
namespace Oilexer.Parser.Builder
{
    /// <summary>
    /// Provides a builder for the parser that results
    /// from a grammar description file.
    /// </summary>
    public class ParserBuilder :
        IEnumerable<ParserBuilderPhase>
    {
        /// <summary>
        /// Returns the <see cref="ParserBuildPhase"/> associated to the current
        /// step in the parse.
        /// </summary>
        public ParserBuilderPhase Phase { get; private set; }
        /// <summary>
        /// Returns the <see cref="GDFile"/> which describes the structure
        /// of the grammar to build a parser for.
        /// </summary>
        public GDFile Source { get; private set; }

        /// <summary>
        /// Returns a series of <see cref="CompilerError"/> instances
        /// which denote where errors occur in the grammar description.
        /// </summary>
        public CompilerErrorCollection Errors { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateProject"/> which represents
        /// the resulted parser.
        /// </summary>
        public IIntermediateProject Project { get; private set; }

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

        private CharStreamClass bitStream;

        public ParserBuilder(IGDFile source, CompilerErrorCollection errors, List<string> streamAnalysisFiles)
        {
            this.PhaseTimes = new Dictionary<ParserBuilderPhase, TimeSpan>();
            this.Source = (GDFile)source;
            this.Errors = errors;
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
            this.Source.GetTokens().AsParallel().ForAll(token =>
                token.BuildNFA());
            //foreach (var token in this.Source.GetTokens())
            //    token.BuildNFA();
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
            var idToken = this.Source.GetTokens().FirstOrDefault(t => t.Name == "Identifier");
            if (idToken != null)
            {
                var second = idToken.DFAState.OutTransitions.Checks.Skip(1).First();
                var secondCount = second.CountTrue();
                var unicodeRanges = Breakdown(second, RegularLanguageSet.CompleteSet);
            }
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
            this.Source.GetRules().AsParallel().ForAll(rule =>
                {
                    lock (this.ruleNFAStates)
                        this.ruleNFAStates.Add(rule, rule.BuildNFA(new SyntacticalNFARootState(rule, this), this.GrammarSymbols, this));
                });
        }

        private void ConstructRuleDFA()
        {
            this.ruleDFAStates = new Dictionary<IProductionRuleEntry, SyntacticalDFARootState>();
            this.Source.GetRules().AsParallel().ForAll(rule =>
                {
                    var dfa = (SyntacticalDFARootState)this.ruleNFAStates[rule].DeterminateAutomata();
                    lock (this.ruleDFAStates)
                    {
                        this.ruleDFAStates.Add(rule, dfa);
                    }
                });
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

        private void BuildCodeModel()
        {
            IIntermediateProject project = new IntermediateProject(Source.Options.AssemblyName, Source.Options.Namespace == null ? "OILexer.DefaultNamespace" : Source.Options.Namespace);
            this.bitStream = BitStreamCreator.CreateBitStream(project.DefaultNameSpace);
            this.Project = project;
        }

        private void BuildStateMachine(InlinedTokenEntry token, IIntermediateProject project, CharStreamClass charStream)
        {
            var tokenDFA = token.DFAState;
            var tokenName = token.Name;
            if (tokenDFA == null)
                return;
            BuildStateMachine(project, charStream, tokenDFA, tokenName);
        }

        private void BuildStateMachine(IIntermediateProject project, CharStreamClass charStream, RegularLanguageDFAState tokenDFA, string tokenName)
        {
            //Setup the basic class, access level, and base-type.
            INameSpaceDeclaration targetNamespace;
            lock (project.DefaultNameSpace.Partials)
                targetNamespace = project.DefaultNameSpace.Partials.AddNew();
            IClassType targetType;
            lock (targetNamespace.Classes)
                targetType = targetNamespace.Classes.AddNew(string.Format("{0}StateMachine", tokenName));
            if (this.Project.Modules.ContainsKey("Lexer"))
                targetType.Module = this.Project.Modules["Lexer"];
            else
                targetType.Module = this.Project.Modules.AddNew("Lexer");
            var stateMachine = targetType;
            stateMachine.AccessLevel = DeclarationAccessLevel.Internal;
            stateMachine.BaseType = charStream.BitStream.GetTypeReference();

            /* *
             * Setup the 'next character' state movement method,
             * the state and exit-length variables.
             * */
            
            var nextMethod = stateMachine.Methods.AddNew(new TypedName("Next", typeof(bool)));
            nextMethod.AccessLevel = DeclarationAccessLevel.Public;
            var nextChar = nextMethod.Parameters.AddNew(new TypedName("currentChar", typeof(char)));
            nextMethod.Summary = string.Format("Moves the state machine into its next state with the @p:{0};.", nextChar.Name);
            nextChar.DocumentationComment = "The next character used as the condition for state->state transitions.";
            var stateField = stateMachine.Fields.AddNew(new TypedName("state", typeof(int)));
            stateField.InitializationExpression = PrimitiveExpression.NumberZero;
            stateField.Summary = "The state machine's current state, determining the logic path to follow for the next character.";
            var exitLength = stateMachine.Fields.AddNew(new TypedName("exitLength", typeof(int)));

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
            var stateSwitch = nextMethod.SelectCase(stateField.GetReference());
            IUnicodeCollectiveTargetGraph collectiveGraph = new UnicodeCollectiveTargetGraph();
            nextMethod.Return(PrimitiveExpression.FalseValue);
            var stateCodeData = GetDictionary((RegularLanguageDFAState)null,
                new
                {
                    SwitchCase = (ISwitchStatementCase)null,
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
                ISwitchStatementCase stateCase = null;
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
                        graphLabels.Add(graph, graphLabel = new LabelStatement(string.Format("UnicodeGraph_{0}", ++unicodeGraphCount)));
                    }
                    else
                    {
                        graph = findResults;
                        graphLabel = graphLabels[findResults];
                    }
                }
                else
                    graph = null;
                stateCase = stateSwitch.Cases.AddNew(new PrimitiveExpression(state.StateValue));
                //if (activeNamedSources.Count > 0)
                //{
                //    stateCase.Statements.Add(new CommentStatement("Sources: "));
                //    foreach (var source in activeNamedSources)
                //        stateCase.Statements.Add(new CommentStatement(source.Key.Name));
                //}
            skipGraph:
                if (state.InTransitions.Count > 0)
                    if (!(state.InTransitions.Count == 1 && state.InTransitions[0].Value.Count == 1 && state.InTransitions[0].Value[0] == state))
                        label = new LabelStatement(string.Format("MoveToState_{0}", state.StateValue));
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
            foreach (var state in stateCodeData.Keys)
            {
                var stateData = stateCodeData[state];
                var stateCase = stateData.SwitchCase;
                var stateGraph = stateData.Graph;
                var graphLabel = stateData.GraphLabel;
                var transitionTable = stateData.TransitionTable;
                var currentTarget = (IStatementBlockInsertBase)stateCase;
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
                                case RegularLanguageSet.ABSelect.A:
                                    currentExpression = new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.IdentityEquality, new PrimitiveExpression(rangeElement.A.Value));
                                    break;
                                case RegularLanguageSet.ABSelect.B:
                                    currentExpression = new BinaryOperationExpression(new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.GreaterThanOrEqual, new PrimitiveExpression(rangeElement.B.Value.Start)), CodeBinaryOperatorType.BooleanAnd, new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.LessThanOrEqual, new PrimitiveExpression(rangeElement.B.Value.End)));
                                    break;
                                default:
                                    break;
                            }
                            if (finalExpression == null)
                                finalExpression = currentExpression;
                            else
                                finalExpression = new BinaryOperationExpression(finalExpression, CodeBinaryOperatorType.BooleanOr, currentExpression);
                        }
                        var currentCondition = currentTarget.IfThen(finalExpression);
                        currentCondition.Statements.Add(targetLabel.GetGoTo(currentCondition.Statements));
                        currentTarget = currentCondition.FalseBlock;
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
                                currentTarget = currentTarget.IfThen(ObtainNegativeAssertion(nextChar, negativeSet));
                            currentTarget.Add(targetLabel.GetGoTo(nextMethod.Statements));

                            goto FullUnicodeCoverage;
                        }
                    }
                    currentTarget.Add(graphLabel.GetGoTo(nextMethod.Statements));
                    if (!insertedGraphs.Contains(stateGraph))
                    {
                        currentTarget = nextMethod;
                        nextMethod.Statements.Add(graphLabel);
                        var categoryGetExpr = typeof(char).GetTypeReferenceExpression().GetMethod("GetUnicodeCategory").Invoke(nextChar.GetReference());
                        var graphSwitch = nextMethod.SelectCase(categoryGetExpr);
                        nextMethod.Return(PrimitiveExpression.FalseValue);
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
                            var fullCategory = graphSwitch.Cases.AddNew();
                            fullCategory.LastIsDefaultCase = false;

                            foreach (var category in fullCategories)
                                fullCategory.Cases.Add(typeof(UnicodeCategory).GetTypeReferenceExpression().GetField(category.TargetedCategory.ToString()));
                            fullCategory.Statements.Add(targetLabel.GetGoTo(fullCategory.Statements));

                            foreach (var category in partialCategories)
                            {
                                IExpression finalExpression = ObtainNegativeAssertion(nextChar, category.NegativeAssertion);
                                var currentCategory = graphSwitch.Cases.AddNew(typeof(UnicodeCategory).GetTypeReferenceExpression().GetField(category.TargetedCategory.ToString()));
                                var currentCondition = currentCategory.IfThen(finalExpression);
                                currentCondition.Statements.Add(targetLabel.GetGoTo(fullCategory.Statements));
                            }
                        }
                        insertedGraphs.Add(stateGraph);
                    }
                FullUnicodeCoverage: ;
                }
            }
        }

        private void DiscernForwardTarget(CharStreamClass charStream, IMethodMember nextMethod, IMethodParameterMember nextChar, IFieldMember stateField, IFieldMember exitLength, ref ILabelStatement terminalExit, ref ILabelStatement nominalExit, ref ILabelStatement commonMove, List<RegularLanguageDFAState> insertedStates, RegularLanguageDFAState activeState, RegularLanguageDFAState destinationState, ref ILabelStatement targetLabel)
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

        private static IExpression ObtainNegativeAssertion(IMethodParameterMember nextChar, RegularLanguageSet regularSet)
        {
            IExpression finalExpression = null;
            foreach (var rangeElement in regularSet.GetRange())
            {
                IExpression currentExpression = null;
                switch (rangeElement.Which)
                {
                    case RegularLanguageSet.ABSelect.A:
                        currentExpression = new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.IdentityInequality, new PrimitiveExpression(rangeElement.A.Value));
                        break;
                    case RegularLanguageSet.ABSelect.B:
                        currentExpression = new BinaryOperationExpression(new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.LessThan, new PrimitiveExpression(rangeElement.B.Value.Start)), CodeBinaryOperatorType.BooleanOr, new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.GreaterThan, new PrimitiveExpression(rangeElement.B.Value.End)));
                        break;
                    default:
                        break;
                }
                if (finalExpression == null)
                    finalExpression = currentExpression;
                else
                    finalExpression = new BinaryOperationExpression(finalExpression, CodeBinaryOperatorType.BooleanAnd, currentExpression);
            }
            return finalExpression;
        }


        private void ImplementStateMoveCheck(CharStreamClass charStream, IMethodMember nextMethod, IMethodParameterMember nextChar, ILabelStatement targetLabel, List<RegularLanguageDFAState> insertedStates, RegularLanguageDFAState target, ref ILabelStatement commonMove, ref ILabelStatement nominalExit, ref ILabelStatement terminalExit, IFieldMember exitLength, IFieldMember stateField)
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
                nextMethod.Statements.Add(targetLabel);
                nextMethod.Assign(stateField.GetReference(), new PrimitiveExpression(target.StateValue));
                nextMethod.Statements.Add(followUp.GetGoTo(nextMethod.Statements));
                insertedStates.Add(target);
            }
        }

        private static void ImplementNominalExitCheck(CharStreamClass charStream, IMethodMember nextMethod, IMethodParameterMember nextChar, ref ILabelStatement nominalExit, IFieldMember exitLength)
        {
            if (nominalExit == null)
            {
                nominalExit = new LabelStatement("NominalExit");
                nextMethod.Statements.Add(nominalExit);
                nextMethod.CallMethod(charStream.PushCharMethod.GetReference().Invoke(nextChar.GetReference()));
                nextMethod.Assign(exitLength.GetReference(), charStream.ActualSize.GetReference());
                nextMethod.Return(PrimitiveExpression.TrueValue);
            }
        }

        private static void ImplementTerminalExitCheck(CharStreamClass charStream, IMethodMember nextMethod, IMethodParameterMember nextChar, ref ILabelStatement terminalExit, IFieldMember exitLength)
        {
            if (terminalExit == null)
            {
                terminalExit = new LabelStatement("TerminalExit");
                nextMethod.Statements.Add(terminalExit);
                nextMethod.CallMethod(charStream.PushCharMethod.GetReference().Invoke(nextChar.GetReference()));
                nextMethod.Assign(exitLength.GetReference(), charStream.ActualSize.GetReference());
                nextMethod.Return(PrimitiveExpression.FalseValue);
            }
        }

        private static void ImplementCommonMoveCheck(CharStreamClass charStream, IMethodMember nextMethod, IMethodParameterMember nextChar, ref ILabelStatement commonMove)
        {
            if (commonMove == null)
            {
                commonMove = new LabelStatement("CommonMove");
                nextMethod.Statements.Add(commonMove);
                nextMethod.CallMethod(charStream.PushCharMethod.GetReference().Invoke(nextChar.GetReference()));
                nextMethod.Return(PrimitiveExpression.TrueValue);
            }
        }

        private static IStatementBlockInsertBase EmitCategoryLogic(IMethodParameterMember nextChar, IFieldMember stateField, IStatementBlockInsertBase currentTarget, RegularLanguageDFAState targetState, RegularLanguageSet negativeAssertion)
        {
            if (negativeAssertion == null)
                currentTarget.Assign(stateField.GetReference(), new PrimitiveExpression(targetState.StateValue));
            else
            {
                IExpression finalExpression = null;
                var currentRange = negativeAssertion.GetRange();

                foreach (var rangeElement in currentRange)
                {
                    IExpression currentExpression = null;
                    switch (rangeElement.Which)
                    {
                        case RegularLanguageSet.ABSelect.A:
                            currentExpression = new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.IdentityInequality, new PrimitiveExpression(rangeElement.A.Value));
                            break;
                        case RegularLanguageSet.ABSelect.B:
                            currentExpression = new BinaryOperationExpression(new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.LessThan, new PrimitiveExpression(rangeElement.B.Value.Start)), CodeBinaryOperatorType.BooleanOr, new BinaryOperationExpression(nextChar.GetReference(), CodeBinaryOperatorType.GreaterThan, new PrimitiveExpression(rangeElement.B.Value.End)));
                            break;
                    }
                    if (finalExpression == null)
                        finalExpression = currentExpression;
                    else
                        finalExpression = new BinaryOperationExpression(finalExpression, CodeBinaryOperatorType.BooleanAnd, currentExpression);
                }
                var currentCondition = currentTarget.IfThen(finalExpression);
                currentCondition.Assign(stateField.GetReference(), new PrimitiveExpression(targetState.StateValue));
                currentTarget = currentCondition.FalseBlock;
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


        /// <summary>
        /// Obtains an enumerator for stepping through the build process.
        /// </summary>
        /// <returns>A new <see cref="IEnumerator{T}"/> which steps the builder 
        /// through its phases.</returns>
        public IEnumerator<ParserBuilderPhase> GetEnumerator()
        {
            if (this.Source == null)
                yield break;
            IntermediateProject result = new IntermediateProject(Source.Options.AssemblyName, string.Format("Oilexer.Parsers.{0}", Source.Options.AssemblyName));
            Stopwatch timer = new Stopwatch();

            Source.InitLookups();
            while (true)
            {
                switch (Phase)
                {
                    case ParserBuilderPhase.None:
                        ((GDFile)Source).Add(new TokenEofEntry(Source.GetTokenEnumerator().ToArray()));
                        Phase = ParserBuilderPhase.Linking;
                        break;
                    case ParserBuilderPhase.Linking:
                        yield return ParserBuilderPhase.Linking;
                        timer.Start();
                        this.Source.ResolveTemplates(this.Errors);
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.Linking, timer.Elapsed);
                        timer.Reset();
                        if (Errors.HasErrors)
                            goto finished;
                        Phase = ParserBuilderPhase.ExpandingTemplates;
                        break;
                    case ParserBuilderPhase.ExpandingTemplates:
                        yield return ParserBuilderPhase.ExpandingTemplates;
                        timer.Start();
                        this.Source.ExpandTemplates(this.Errors);
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.ExpandingTemplates, timer.Elapsed);
                        timer.Reset();
                        if (Errors.HasErrors)
                            goto finished;
                        Phase = ParserBuilderPhase.LiteralLookup;
                        break;
                    case ParserBuilderPhase.LiteralLookup:
                        yield return ParserBuilderPhase.LiteralLookup;
                        timer.Start();
                        this.Source.FinalLink(this.Errors);
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.LiteralLookup, timer.Elapsed);
                        timer.Reset();
                        if (Errors.HasErrors)
                            goto finished;
                        Phase = ParserBuilderPhase.InliningTokens;
                        break;
                    case ParserBuilderPhase.InliningTokens:
                        yield return ParserBuilderPhase.InliningTokens;
                        timer.Start();
                        bool inlineSuccess = ParserCompilerExtensions.InlineTokens(Source, Errors);
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
                            yield break;
                        Phase = ParserBuilderPhase.TokenNFAConstruction;
                        break;
                    case ParserBuilderPhase.TokenNFAConstruction:
                        yield return ParserBuilderPhase.TokenNFAConstruction;
                        timer.Start();
                        ConstructTokenNFA();
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.TokenNFAConstruction, timer.Elapsed);
                        timer.Reset();
                        Phase = ParserBuilderPhase.TokenDFAConstruction;
                        break;
                    case ParserBuilderPhase.TokenDFAConstruction:
                        yield return ParserBuilderPhase.TokenDFAConstruction;
                        timer.Start();
                        ConstructTokenDFA();
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.TokenDFAConstruction, timer.Elapsed);
                        timer.Reset();
                        Phase = ParserBuilderPhase.TokenDFAReduction;
                        break;
                    case ParserBuilderPhase.TokenDFAReduction:
                        yield return ParserBuilderPhase.TokenDFAReduction;
                        timer.Start();
                        this.ReduceTokenDFA();
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.TokenDFAReduction, timer.Elapsed);
                        timer.Reset();
                        Phase = ParserBuilderPhase.RuleNFAConstruction;
                        break;
                    case ParserBuilderPhase.RuleNFAConstruction:
                        yield return ParserBuilderPhase.RuleNFAConstruction;
                        timer.Start();
                        this.ConstructRuleNFA();
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.RuleNFAConstruction, timer.Elapsed);
                        timer.Reset();
                        Phase = ParserBuilderPhase.RuleDFAConstruction;
                        break;
                    case ParserBuilderPhase.RuleDFAConstruction:
                        yield return ParserBuilderPhase.RuleDFAConstruction;
                        timer.Start();
                        this.ConstructRuleDFA();
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.RuleDFAConstruction, timer.Elapsed);
                        timer.Reset();
                        Phase = ParserBuilderPhase.CallTreeAnalysis;
                        break;
                    case ParserBuilderPhase.CallTreeAnalysis:
                        yield return ParserBuilderPhase.CallTreeAnalysis;
                        timer.Start();
                        this.PerformStreamAnalysis();
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.CallTreeAnalysis, timer.Elapsed);
                        timer.Reset();
                        Phase = ParserBuilderPhase.ObjectModelRootTypesConstruction;
                        break;
                    case ParserBuilderPhase.ObjectModelTokenCaptureConstruction:
                        yield return ParserBuilderPhase.ObjectModelTokenCaptureConstruction;
                        this.BuildCodeModelCaptures();
                        Phase = ParserBuilderPhase.ObjectModelTokenEnumConstruction;
                        break;
                    case ParserBuilderPhase.ObjectModelTokenEnumConstruction:
                        yield return ParserBuilderPhase.ObjectModelTokenEnumConstruction;
                        this.BuildCodeModelEnums();
                        Phase = ParserBuilderPhase.ObjectModelRuleStructureConstruction;
                        break;
                    case ParserBuilderPhase.ObjectModelRuleStructureConstruction:
                        yield return ParserBuilderPhase.ObjectModelRuleStructureConstruction;
                        Phase = ParserBuilderPhase.ObjectModelFinalTypesConstruction;
                        break;
                    case ParserBuilderPhase.ObjectModelRootTypesConstruction:
                        yield return ParserBuilderPhase.ObjectModelRootTypesConstruction;
                        timer.Start();
                        BuildCodeModel();
                        timer.Stop();
                        PhaseTimes.Add(ParserBuilderPhase.ObjectModelRootTypesConstruction, timer.Elapsed);
                        timer.Reset();
                        Phase = ParserBuilderPhase.ObjectModelTokenCaptureConstruction;
                        break;
                    case ParserBuilderPhase.ObjectModelFinalTypesConstruction:
                        yield return ParserBuilderPhase.ObjectModelFinalTypesConstruction;
                        goto finished;
                    default:
                        goto finished;
                }
            }
        finished:
            //if (this.Errors.Count == 0)
            //     this.Project = result;
            LinkerCore.errorEntries = null;
            LinkerCore.tokenEntries = null;
            LinkerCore.ruleEntries = null;
            LinkerCore.ruleTemplEntries = null;
            yield break;
        }

        private void FindStartRule()
        {
            IProductionRuleEntry startRule;
            if (Source.Options.StartEntry == null || Source.Options.StartEntry == string.Empty)
            {
                Errors.Add(GrammarCore.GetParserError(Source.Files[0], 0, 0, GDParserErrors.NoStartDefined, Source.Options.GrammarName));
                return;
            }
            if ((startRule = (this.Source.GetRules()).FindScannableEntry(Source.Options.StartEntry)) == null)
            {
                Errors.Add(GrammarCore.GetParserError(Source.Files[0], 0, 0, GDParserErrors.InvalidStartDefined, Source.Options.StartEntry));
                return;
            }
            this.StartEntry = startRule;
        }

        private void BuildCodeModelEnums()
        {
        }

        private void BuildCodeModelCaptures()
        {
            foreach (var token in this.Source.GetTokens())
                BuildStateMachine(token, this.Project, bitStream);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
