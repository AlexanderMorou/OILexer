using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Cli;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages;
using AllenCopeland.Abstraction.Slf.Languages.CSharp;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities;
using AllenCopeland.Abstraction.Utilities.Arrays;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if x64
#if HalfWord
using SlotType = System.UInt32;
#else
using SlotType = System.UInt64;
#endif
#elif x86
#if HalfWord
using SlotType = System.UInt16;
#else
using SlotType = System.UInt32;
#endif
#endif

/*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
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
    public class ParserCompiler
    {
        /* *
         * The purpose of this class should be to only synthesize the phases together in a logical, straight forward, manner.
         * If a class, method or other logical structure needs created, this is not the place for it.  Use the builder pattern
         * shown below.
         * */
        private IOilexerGrammarFileObjectRelationalMap _relationalModelMapping;
        internal const string ErrorCaptureName = "__Errors";
        internal const string HasErrorsCaptureName = "__HasError";
        internal static bool ReductionSignalTriggered = false;
        private VariableTokenBaseBuilder _variableTokenBaseBuilder;


        private Dictionary<IGrammarAmbiguousSymbol, GrammarVocabularyAmbiguitySymbolBreakdown> _ambiguityBreakdowns = new Dictionary<IGrammarAmbiguousSymbol, GrammarVocabularyAmbiguitySymbolBreakdown>();
        private ParseResultsBuilder _parseResultsBuilder;
        private ExtensionsBuilder _extensionsBuilder;
        private CommonSymbolBuilder _commonSymbolBuilder;
        private StackContextBuilder _stackContextBuilder;
        private RuleSymbolBuilder _ruleSymbolBuilder;
        private RootRuleBuilder _rootRuleBuilder;
        private SymbolStoreBuilder _symbolStoreBuilder;
        private ErrorContextBuilder _errorContextBuilder;
        private Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabularyRuleDetail> aggregateRuleDetails = new Dictionary<IOilexerGrammarProductionRuleEntry,GrammarVocabularyRuleDetail>();
        private Dictionary<IGrammarTokenSymbol, GrammarVocabularyTokenSymbolBreakdown> aggregateTokenSymbolDetails = new Dictionary<IGrammarTokenSymbol, GrammarVocabularyTokenSymbolBreakdown>();
        /// <summary>
        /// Returns the <see cref="OilexerGrammarFile"/> which describes the structure
        /// of the grammar to build a parser for.
        /// </summary>
        public OilexerGrammarFile Source { get; private set; }

        public ExtensionsBuilder ExtensionsBuilder { get { return this._extensionsBuilder; } }

        internal Dictionary<IGrammarAmbiguousSymbol, GrammarVocabularyAmbiguitySymbolBreakdown> AmbiguityDetail
        {
            get
            {
                return this._ambiguityBreakdowns;
            }
        }

        public ParseResultsBuilder ParseResultsBuilder { get { return this._parseResultsBuilder; } }

        /// <summary>
        /// Returns a series of <see cref="ICompilerErrorCollection"/> instances
        /// which denote where errors occur in identity resolution,
        /// usage of templates, or other base compiler errors.
        /// </summary>
        public ICompilerErrorCollection CompilationErrors { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateAssembly"/> which represents
        /// the results of the compile.
        /// </summary>
        public IIntermediateAssembly ResultAssembly { get; private set; }

        /// <summary>
        /// Returns a dictionary containing timing information for each phase
        /// of the build process.
        /// </summary>
        public Dictionary<ParserCompilerPhase, TimeSpan> PhaseTimes { get; private set; }

        public ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalNFARootState> RuleNFAStates { get; private set; }

        private Dictionary<IOilexerGrammarProductionRuleEntry, SyntacticalNFARootState> ruleNFAStates { get; set; }

        public ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> RuleDFAStates { get; private set; }

        /// <summary>
        /// Returns the <see cref="ControlledDictionary{TKey, TValue}"/> 
        /// which denotes the <see cref="ProductionRuleNormalAdapter"/>
        /// for the rules of the language defined by <see cref="Source"/>.
        /// </summary>
        /// <remarks><see cref="ProductionRuleNormalAdapter"/>s augment the
        /// <see cref="SyntacticalDFARootState"/> instances derived through
        /// <see cref="RuleDFAStates"/>.
        /// </remarks>
        public ControlledDictionary<IOilexerGrammarProductionRuleEntry, ProductionRuleNormalAdapter> RuleAdapters { get; private set; }

        public ReadonlyMultikeyedDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState, ProductionRuleNormalAdapter> AllRuleAdapters { get; private set; }
        private MultikeyedDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState, ProductionRuleNormalAdapter> allRuleAdapters { get; set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleEntry"/> which is the root
        /// of the grammar defined by <see cref="Source"/>.
        /// </summary>
        public IOilexerGrammarProductionRuleEntry StartEntry { get; private set; }

        /// <summary>
        /// Returns a series of token precedences associated to the tokens.
        /// </summary>
        internal TokenPrecedenceTable Precedences { get; private set; }

        /// <summary>
        /// Returns the <see cref="IGrammarSymbolSet"/> which denotes the full set of token/rule symbols of a given grammar.
        /// </summary>
        public IGrammarSymbolSet GrammarSymbols { get; private set; }
        internal GrammarSymbolSet _GrammarSymbols { get { return (GrammarSymbolSet)this.GrammarSymbols; } }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> which denotes the 
        /// generated token representing the end of the file/stream.
        /// </summary>
        public IOilexerGrammarTokenEntry EOFToken { get; private set; }

        /// <summary>
        /// Returns the <see cref="IControlledCollection{T}"/> of
        /// <see cref="String"/> values denoting the files to use
        /// to analyze the language.
        /// </summary>
        public IControlledCollection<string> StreamAnalysisFiles { get; private set; }

        private IIntermediateClassType _multikeyedDictionary;
        private IIntermediateInterfaceType _imultikeyedDictionary;
        private IIntermediateStructType _mkdKeys;

        /// <summary>
        /// Occurs when a phase of the compiler changes.
        /// </summary>
        public event EventHandler<ParserBuilderPhaseChangeEventArgs> PhaseChange;

        private bool buildObjectModel;

        public ParserCompiler(IOilexerGrammarFile source, List<string> streamAnalysisFiles, bool buildObjectModel, bool expandTemplates)
            : this()
        {
            this.RuleDFAStates = new ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState>();
            Initialize(source, streamAnalysisFiles, buildObjectModel, expandTemplates);
        }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarFileObjectRelationalMap"/> for the model
        /// described by the language derived from <see cref="Source"/>.
        /// </summary>
        public IOilexerGrammarFileObjectRelationalMap RelationalModelMapping { get { return this._relationalModelMapping; } }

        protected ParserCompiler()
        {
            this.ReducedRules = new HashSet<IOilexerGrammarProductionRuleEntry>();
        }


        protected void Initialize(IOilexerGrammarFile source, List<string> streamAnalysisFiles, bool buildObjectModel, bool expandTemplates)
        {
            this.buildObjectModel = buildObjectModel;
            this.expandTemplates = expandTemplates;
            this.CompilationErrors = new CompilerErrorCollection();
            this.PhaseTimes = new Dictionary<ParserCompilerPhase, TimeSpan>();
            this.Source = (OilexerGrammarFile)source;
            this.StreamAnalysisFiles = new ControlledCollection<string>(streamAnalysisFiles);
        }


        private void ConstructTokenNFA()
        {
            this.EOFToken = (IOilexerGrammarTokenEofEntry)this.Source.FirstOrDefault(f => f is IOilexerGrammarTokenEofEntry);
            /* *
             * Overview:
             *      1. Determine each token type and construct 
             *         appropriate data model for each token.
             *      2. Order tokens via precedence. 
             *      3. Construct general NFA from inlined tokens.
             * */
            foreach (var token in this.Source.GetInlinedTokens())
                token.BuildNFA(this.Source);
        }

        private void ConstructTokenDFA()
        {
            /* *
             * Overview:
             *      Construct the appropriate Deterministic 
             *      finite automata for each kind of token using
             *      the information gained in the previous step,
             *      to decide how much reduction to perform 
             *      post-DFA construction.
             * */
            this.Source.GetInlinedTokens().AsParallel()
              .OnAllP(token =>
                token.BuildDFA());
        }


        private void ConstructRuleNFA()
        {
            var symbols = _GrammarSymbols;
            this.ruleNFAStates = new Dictionary<IOilexerGrammarProductionRuleEntry, SyntacticalNFARootState>();
            var rules = (from rule in Source.GetRules()
                         orderby rule.Name
                         select rule).ToArray();
            var result = new SyntacticalNFARootState[rules.Length];
            int i = 0;
            var translatedRules =
                (from OilexerGrammarProductionRuleEntry rule in rules
                 select new
                     {
                         Rule = rule,
                         Index = i++,
                         CaptureStructure = rule.CaptureStructure = ProductionRuleStructuralExtractionCore.BuildStructureFor(rule, this.Source),
                         StructureReplacements = ProductionRuleStructuralExtractionCore.ObtainReplacements(rule.captureStructure)
                     }).ToArray();
            translatedRules
#if ParallelProcessing
                /* Auto-formatting fix. */
                .AsParallel()
                .ForAll(ruleInfo =>
#else
                /* Auto-formatting fix. */
                .OnAll(ruleInfo =>
#endif
                    /* Auto-formatting fix. */

                result[ruleInfo.Index] = (SyntacticalNFARootState)ruleInfo.Rule.BuildNFA(new SyntacticalNFARootState(ruleInfo.Rule, this.RuleDFAStates, symbols), symbols, RuleDFAStates, ruleInfo.StructureReplacements));
            for (i = 0; i < result.Length; i++)
            {
                var current = result[i];
                this.ruleNFAStates.Add(current.Source, current);
            }
            this.RuleNFAStates = new ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalNFARootState>(this.ruleNFAStates);
        }

        private void ConstructRuleDFA()
        {
            var ruleDFAStates = new ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState>();
            int i = 0;
            var rules = (from rule in Source.GetRules()
                         orderby rule.Name
                         select rule).ToArray();
            SyntacticalDFARootState[] result = new SyntacticalDFARootState[rules.Length];
            rules.OnAll(p => new { Rule = p, Index = i++ }).AsParallel().ForAll(ruleAndIndex => result[ruleAndIndex.Index] = (SyntacticalDFARootState)this.ruleNFAStates[ruleAndIndex.Rule].DeterminateAutomata());
            this.RuleAdapters = new ControlledDictionary<IOilexerGrammarProductionRuleEntry, ProductionRuleNormalAdapter>();
            for (i = 0; i < result.Length; i++)
            {
                var current = result[i];
                RuleDFAStates._Add(current.Entry, current);
                aggregateRuleDetails.Add(current.Entry, new GrammarVocabularyRuleDetail() { DFAState = current, Compiler = this });
            }
        }

        private void ReduceTokenDFA()
        {
            /* *
             * Overview:
             *      Reduce each token's deterministic
             *      state machine to its minimal form.
             * */
            var tokensArr = this.Source.GetInlinedTokens().ToArray();
            tokensArr.OnAllP(token =>
            {
                token.ReduceDFA();
                if (token.DFAState != null)
                    token.DFAState.Enumerate();
            });

        }

        private void ReduceRuleDFA()
        {
            /* * * * * * * * * * * * * * * * * * * *\
             * Create a basic flat list of the      *
             * rules that will be used frequently.  *
             * * * * * * * * * * * * * * * * * * * */
            var rulesArr = (from rule in this.Source.GetRules()
                            select this.RuleDFAStates[rule]).ToArray();
            rulesArr.OnAll(rule =>
            {
                lock (rule)
                    rule.ReduceDFA();
            });
            int stateIndex = 1;
            this.allRuleAdapters = new MultikeyedDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState, ProductionRuleNormalAdapter>();
            foreach (var rule in rulesArr)
            {
                rule.Enumerate(ref stateIndex);
                /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
                 * Create adapters for every state within the deterministic automation.     *
                 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
                 * The root adapter is provided this context so lookups can be performed.   *
                 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
                 * ToDo: Create a 'RootAdapter' concept in the base implementation, due to  *
                 * the frequency at which it is used. This will auto-link this root adapter *
                 * lookup.                                                                  *
                 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
                Dictionary<SyntacticalDFAState, ProductionRuleNormalAdapter> lookupContext = new Dictionary<SyntacticalDFAState, ProductionRuleNormalAdapter>();
                this.RuleAdapters._Add(rule.Entry, ProductionRuleNormalAdapter.Adapt(rule, ref lookupContext, this));
                this.RuleAdapters[rule.Entry].AssociatedContext.StateAdapterLookup = new ControlledDictionary<SyntacticalDFAState, ProductionRuleNormalAdapter>(lookupContext);
                foreach (var key in lookupContext)
                {
                    /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
                     * Using the lookup context, add them to the flattened form of the  *
                     * 'all adapters'.                                                  *
                     * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
                    allRuleAdapters.Add(rule.Entry, key.Key, key.Value);
                }
            }

            /* * * * * * * * * * * * * * * * * *\
             * Light-weight read-only wrapper.  *
             * * * * * * * * * * * * * * * * * */
            this.AllRuleAdapters = new ReadonlyMultikeyedDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFAState, ProductionRuleNormalAdapter>(this.allRuleAdapters);
            //Update the max state for the next phase.
            this.ParserMaxState = stateIndex;
        }

        private void ConstructProjectionNodes()
        {
            /* * * * * * * * * * * * * * * * * * * * * * *\
             * Create the nodes for the root rule states. *
             * * * * * * * * * * * * * * * * * * * * * * */
            this._ruleProjectionNodes = SyntacticAnalysisCore.ConstructProjectionNodes(this);
            /* * * * * * * * * * * * * * * * * * * * * * * * * *\
             * Create the nodes for the sub-states of the rule  *
             * automations.                                     *
             * * * * * * * * * * * * * * * * * * * * * * * * * **
             * Adapters were implemented after this phase.      *
             * Hence the disconnect.                            *
             * * * * * * * * * * * * * * * * * * * * * * * * * */
            this._allProjectionNodes = SyntacticAnalysisCore.ConstructRemainingNodes(_ruleProjectionNodes, this);
        }

        private void ProjectLookaheadInitial()
        {
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
             * Link the individual nodes together using the production    *
             * rule as a key, this will lead to a forest type structure   *
             * that can be used to calculate look-ahead.                  *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * Then project the initial look-ahead context from that.     *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            if (!_linkedProjectionNodes)
            {
                SyntacticAnalysisCore.ConstructProjectionLinks(_allProjectionNodes, _ruleProjectionNodes);
                _linkedProjectionNodes = true;
            }
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
             * Cycle Depth determines the level to which call trees will  *
             * be expanded, which is most notably important for           *
             * left-recursive rules as some ambiguities stay hidden until *
             * you explore them further.                                  *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * Use caution when using cycle depths > 5, the memory        *
             * requirements skyrocket beyond this point (exponential.)    *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            this.RuleGrammarLookup =
                (from rule in this.RuleDFAStates.Keys
                 select new { Rule = rule, GrammarVocabulary = new GrammarVocabulary(this.GrammarSymbols, this.GrammarSymbols.First(k => (k is IGrammarRuleSymbol) && ((IGrammarRuleSymbol)k).Source == rule)) }).ToDictionary(k => k.Rule, v => v.GrammarVocabulary);
            SyntacticAnalysisCore.PerformLookAheadProjection(this._allProjectionNodes, this.RuleGrammarLookup, this.CycleDepth);/* OILexer command line arg: -cycledepth:int */
        }

        private void ProjectLookaheadExpanded()
        {

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
             * Flesh out the forest further, noting transitions interna-  *
             * -lly from state->state for each avenue.                    *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * The forest structure is used to enable a deeper insight    *
             * beyond just building a simple table of each state's        *
             * look-ahead.  This is because all possible avenues can be   *
             * observed simultaneously and reductions can occur when mul- *
             * -tiple branches target the same entry state for a given    *
             * rule.                                                      *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * Reduce-Reduce conflicts are the only style of ambiguity    *
             * that is presumed to be unhandled by OILexer.  ToDo: Case   *
             * Studies that validate this.                                *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            SyntacticAnalysisCore.PerformExpandedLookAheadProjection(_allProjectionNodes, this.RuleGrammarLookup, this.CompilationErrors, this._GrammarSymbols);
        }

        private void BuildSymbolStore(IIntermediateAssembly resultAssembly)
        {
            this.LexicalSymbolModel = new GrammarVocabularyModelBuilder(this.GrammarSymbols, resultAssembly, this.Source, this, "LexicalSymbols", t => t is IGrammarTokenSymbol || t is IGrammarAmbiguousSymbol, true);
            this.SyntacticalSymbolModel = new GrammarVocabularyModelBuilder(this.GrammarSymbols, resultAssembly, this.Source, this, "SyntaxSymbols", r=>r is IGrammarRuleSymbol, false);
        }

        public void BuildProject()
        {
            if (this.phase != ParserCompilerPhase.None)
                throw new InvalidOperationException("Build in progress.");
            try
            {
                const int ACTION_FAIL = 0,
                          ACTION_CONTINUE = 1,
                          ACTION_SKIP = 2;

                IIntermediateAssembly resultAssembly = null;
                IIntermediateAssembly vsixProject = null;
                if (this.Source == null)
                    goto finished;
                IIntermediateCliManager identityManager = IntermediateCliGateway.CreateIdentityManager(CliGateway.CurrentPlatform, CliGateway.CurrentVersion);
                resultAssembly = (IIntermediateAssembly)LanguageVendors.AllenCopeland.GetOilexerLanguage().GetProvider(identityManager).CreateAssembly(Source.Options.AssemblyName);
                if (Source.Options.Namespace == null)
                    resultAssembly.DefaultNamespace = resultAssembly.Namespaces.Add("OILexer.DefaultNamespace");
                else
                    resultAssembly.DefaultNamespace = resultAssembly.Namespaces.Add(Source.Options.Namespace);
                Stopwatch timer = new Stopwatch();
                Dictionary<ParserCompilerPhase, Action> compilerPhases = new Dictionary<ParserCompilerPhase, Action>();
                Dictionary<ParserCompilerPhase, Func<int>> compilerBreakpoints = new Dictionary<ParserCompilerPhase, Func<int>>();

                string projectGuidText;
                if (this.Source.DefinedSymbols.TryGetValue("ProjectGuid", out projectGuidText) && !string.IsNullOrEmpty(projectGuidText))
                {
                    Guid projectGuid;
                    if (Guid.TryParse(projectGuidText, out projectGuid))
                        resultAssembly.PrivateImplementationDetails.DetailGuid = projectGuid;
                }
                if (this.Source.DefinedSymbols.TryGetValue("VsixProjectGuid", out projectGuidText) && !string.IsNullOrEmpty(projectGuidText))
                {
                    vsixProject = ((IIntermediateAssembly)(LanguageVendors.Microsoft.GetCSharpLanguage().GetProvider(identityManager)).CreateAssembly(string.Format("Vsix{0}", Source.Options.AssemblyName)));
                    Guid projectGuid;
                    if (Guid.TryParse(projectGuidText, out projectGuid))
                        vsixProject.PrivateImplementationDetails.DetailGuid = projectGuid;
                    vsixProject.References.Add(resultAssembly);
                }
                #region Linking - Stage 1 - Identity Resolution
                /* *
                 * Identity resolution phase. 
                 * *
                 * This is done first to avoid high cost
                 * of identity resolution that would occur
                 * after template resolution.
                 * */
                compilerPhases.Add(ParserCompilerPhase.Linking, () =>
                {
                    Source.InitLookups();
                    ((OilexerGrammarFile)Source).Add(new OilexerGrammarTokenEofEntry(Source.GetTokenEnumerator().ToArray()));
                    this.Source.IdentityResolution(this.CompilationErrors);
                });
                #endregion
                #region Linking - Stage 2 - Expanding Templates
                /* *
                 * The process of expanding templates works like so:
                 * ABCAny ::=
                 *  Tuple<'a', 'b', 'c'>;
                 *  
                 * Tuple<N1, N2, N3> ::=
                 *  N1 N2 N3 |
                 *  N1 N3 N2 |
                 *  N2 N1 N3 |
                 *  N2 N3 N1 |
                 *  N3 N1 N2 |
                 *  N3 N2 N1 ;
                 * Would be rewritten as:
                 * ABCAny ::=
                 *  'a' 'b' 'c' |
                 *  'a' 'c' 'b' |
                 *  'b' 'a' 'c' |
                 *  'b' 'c' 'a' |
                 *  'c' 'a' 'b' |
                 *  'c' 'b' 'a' ;
                 * *
                 * Templates are then removed from the model as they
                 * have served their purpose. Conditions are possible
                 * within templates, but that's not covered here.
                 * *
                 * The next step's 'Deliteralization' process will
                 * take place on the parameters *prior* to expansion
                 * *
                 * This is due to the overhead of doing so.
                 * */
                compilerBreakpoints.Add(ParserCompilerPhase.ExpandingTemplates, () => expandTemplates ? ACTION_CONTINUE : ACTION_FAIL);
                compilerPhases.Add(ParserCompilerPhase.ExpandingTemplates, () =>
                {
                    this.Source.Where(entry => entry is IOilexerGrammarProductionRuleEntry).Cast<IOilexerGrammarProductionRuleEntry>().OnAll(entry =>
                        entry.CreatePreexpansionText());
                    this.Source.ExpandTemplates(this.CompilationErrors);
                    var precedences = new TokenPrecedenceTable(this.Source.GetSortedTokens());
                    var sourceSorted = ((from grouping in
                                             (from equalityLevel in precedences
                                              from tokenEntry in equalityLevel
                                              orderby equalityLevel.Index,
                                                      tokenEntry.Name
                                              group tokenEntry by equalityLevel)
                                         from tokenEntry in grouping
                                         select tokenEntry).ToArray()).Concat(from fileEntry in Source.Except(this.Source.GetTokens())
                                                                              where fileEntry is IOilexerGrammarNamedEntry
                                                                              let namedEntry = (IOilexerGrammarNamedEntry)fileEntry
                                                                              orderby namedEntry.Name
                                                                              select fileEntry).ToArray();
                    this.Source.Clear();
                    foreach (var k in sourceSorted)
                        this.Source.Add(k);
                });
                #endregion
                #region Linking - Stage 3 - Deliteralization
                /* *
                 * Deliteralization is a process by which:
                 * IfStatement ::=
                 *  "if" '(' BooleanExpression ')' ...;
                 * Operators :=
                 *  ...
                 *  '(':LeftParenthesis
                 *  ...
                 *  ')':RightParenthesis
                 *  ...;
                 * Keywords  :=
                 *  ...
                 *  "if":If
                 *  ...;
                 * *
                 * Would be rewritten as:
                 * IfStatement ::=
                 *  Keywords.If Operators.LeftParenthesis BooleanExpression Operators.RightParenthesis ...;
                 * *
                 * This process also expunges comments and 
                 * unreferenced tokens as they could introduce
                 * ambiguity.
                 * */
                compilerPhases.Add(ParserCompilerPhase.LiteralLookup, () => this.Source.FinalLink(this.CompilationErrors));
                #endregion

                #region Token Inlining
                /* *
                 * Inlining is a process by which:
                 * Token1 :=
                 *  Token2 'c';
                 * Token2 :=
                 *  'a' 'b'
                 * *
                 * Token1 would be rewritten so that:
                 * Token1 :=
                 *  ('a' 'b') 'c';
                 * *
                 * Tokens are deterministic machines which 
                 * cannot be recursive nor call other token
                 * parse methods.
                 * *
                 * This inlining process simplifies the overall analysis done on any given token.
                 * *
                 * The imported tokens are akin to 'fragments' in another popular tool.
                 * */
                bool inlineSuccess = false;
                compilerPhases.Add(ParserCompilerPhase.InliningTokens, () =>
                {
                    inlineSuccess = ParserCompilerExtensions.InlineTokens(Source);
                    if (inlineSuccess)
                        this.Precedences = new TokenPrecedenceTable(this.Source.GetSortedTokens());
                });
                compilerBreakpoints.Add(ParserCompilerPhase.TokenNFAConstruction, () =>
                {
                    FindStartRule();
                    if (!(inlineSuccess && buildObjectModel) || this.StartEntry == null)
                        return ACTION_FAIL;
                    ParserCompilerExtensions.CleanupRules();
                    return ACTION_CONTINUE;
                });
                #endregion

                compilerPhases.Add(ParserCompilerPhase.TokenNFAConstruction, this.ConstructTokenNFA);
                compilerPhases.Add(ParserCompilerPhase.TokenDFAConstruction, this.ConstructTokenDFA);
                compilerPhases.Add(ParserCompilerPhase.TokenDFAReduction, this.ReduceTokenDFA);
                compilerPhases.Add(ParserCompilerPhase.RuleDuplicationCheck, this.RuleReplicationCheck);
                /* *
                 * - Change April 12, 2015 -
                 * ToDo: Moved lexical analysis phase before syntactic phase to enable
                 *       ambiguities to be detected prior to the syntax analysis.
                 * */
                compilerPhases.Add(ParserCompilerPhase.CreateCoreLexerAndEstablishFullAmbiguitySet, () => this.IdentifyLexerAmbiguities(resultAssembly));
                compilerPhases.Add(ParserCompilerPhase.RuleNFAConstruction, this.ConstructRuleNFA);
                compilerPhases.Add(ParserCompilerPhase.RuleDFAConstruction, this.ConstructRuleDFA);
                compilerPhases.Add(ParserCompilerPhase.RuleDFAReduction, this.ReduceRuleDFA);
                compilerPhases.Add(ParserCompilerPhase.ConstructProjectionNodes, this.ConstructProjectionNodes);
                compilerPhases.Add(ParserCompilerPhase.ProjectLookaheadInitial, this.ProjectLookaheadInitial);
                compilerPhases.Add(ParserCompilerPhase.ProjectLookaheadExpanded, this.ProjectLookaheadExpanded);
                compilerPhases.Add(ParserCompilerPhase.ProjectionStateMachines, this.CreateProjectionStateMachines);
                compilerPhases.Add(ParserCompilerPhase.FollowProjectionsAndMachines, this.CreateFollowProjectionsAndMachines);
                compilerPhases.Add(ParserCompilerPhase.ObjectModelScaffolding, () => this.BuildObjectModelScaffolding(resultAssembly));
                compilerPhases.Add(ParserCompilerPhase.LiftAmbiguitiesAndGenerateLexerCore, () => HandleAmbiguitiesAndLexerCore(resultAssembly));
                compilerPhases.Add(ParserCompilerPhase.BuildExtensions, () => this.BuildExtensions(resultAssembly));
                compilerPhases.Add(ParserCompilerPhase.ObjectModelParser, () => this.BuildParser());
                /* *
                 * ToDo: Evaluate whether these phases are necessary.
                 * */
                compilerPhases.Add(ParserCompilerPhase.ObjectModelRuleStructureConstruction, null);
                compilerPhases.Add(ParserCompilerPhase.ObjectModelFinalTypesConstruction, null);
                int phaseIndex = 0;
                var keys = compilerPhases.Keys.ToList();
                Func<int> ReductionSignalTriggeredAction = () =>
                {
                    if (ReductionSignalTriggered)
                    {
                        foreach (var node in this.AllProjectionNodes.Values)
                            node.ClearCache();
                        ReductionSignalTriggered = false;
                        phaseIndex = keys.IndexOf(ParserCompilerPhase.ProjectLookaheadInitial) - 1;
                        /* Go back one, if we have reductions it can greatly change the face of the result behavior. */
                        return ACTION_SKIP;
                    }

                    return ACTION_CONTINUE;
                };
                compilerBreakpoints.Add(ParserCompilerPhase.ProjectLookaheadExpanded, ReductionSignalTriggeredAction);
                compilerBreakpoints.Add(ParserCompilerPhase.ProjectionStateMachines, ReductionSignalTriggeredAction);
                compilerBreakpoints.Add(ParserCompilerPhase.FollowProjectionsAndMachines, ReductionSignalTriggeredAction);
                /* *
                 * Simpler way, for me, to inject new steps into the process.
                 * */
                ParserCompilerPhase phase;
                for (; phaseIndex < keys.Count; phaseIndex++)
                {
                    phase = keys[phaseIndex];
                    if (compilerBreakpoints.ContainsKey(phase))
                        switch (compilerBreakpoints[phase]())
                        {
                            case ACTION_CONTINUE:
                                break;
                            case ACTION_FAIL:
                                goto finished;
                            case ACTION_SKIP:
                                continue;
                        }
                    if (CompilationErrors.HasErrors)
                        goto finished;
                    this.Phase = phase;
                    timer.Reset();
                    timer.Start();
                    Action phaseAction = compilerPhases[phase];
                    if (phaseAction != null)
                        phaseAction();
                    timer.Stop();
                    if (PhaseTimes.ContainsKey(phase))
                        PhaseTimes[phase]+=timer.Elapsed;
                    else
                        PhaseTimes.Add(phase, timer.Elapsed);
                }
            finished:
                /* *
                 * Change March 17, 2015:
                 * *
                 * Used 'HasErrors' because .Count may be greater than
                 * zero in the event that there are warnings. Obvious,
                 * but it was missed!
                 * */
                if (!this.CompilationErrors.HasErrors)
                {
                    this.ResultAssembly = resultAssembly;
                    this.ResultVsixAssembly = vsixProject;
                }
            }
            finally
            {
                OilexerGrammarLinkerCore.errorEntries = null;
                OilexerGrammarLinkerCore.tokenEntries = null;
                OilexerGrammarLinkerCore.ruleEntries = null;
                OilexerGrammarLinkerCore.ruleTemplEntries = null;
                this.Phase = ParserCompilerPhase.None;
            }
        }

        private void BuildExtensions(IIntermediateAssembly resultAssembly)
        {
            this.ExtensionsBuilder.Build2(this, this._parserBuilder, (IIntermediateCliManager)resultAssembly.IdentityManager);
        }

        private void RuleReplicationCheck()
        {
            List<IOilexerGrammarProductionRuleEntry> processed = new List<IOilexerGrammarProductionRuleEntry>();
            List<Tuple<IOilexerGrammarProductionRuleEntry, IOilexerGrammarProductionRuleEntry>> found = new List<Tuple<IOilexerGrammarProductionRuleEntry, IOilexerGrammarProductionRuleEntry>>();
            foreach (var primary in this.Source.GetRules())
            {
                if (processed.Contains(primary))
                    continue;
                processed.Add(primary);
                foreach (var secondary in this.Source.GetRules().Except(processed.ToArray()))
                {
                    if (primary.IsEqualTo(secondary))
                    {
                        processed.Add(secondary);
                        found.Add(Tuple.Create(primary, secondary));
                    }
                }
            }
            var duplications = (from d in found
                                group d.Item2 by d.Item1).ToDictionary(k => k.Key, v => v.ToArray());
            foreach (var duplication in duplications)
                this.CompilationErrors.SourceModelWarning(
                    OilexerGrammarCore.CompilerWarnings.DuplicateDefinition,
                    new LineColumnPair(duplication.Key.Line, duplication.Key.Column),
                    new LineColumnPair(duplication.Key.Line, duplication.Key.Column),
                    new Uri(duplication.Key.FileName, UriKind.RelativeOrAbsolute),
                    duplication.Key,
                    duplication.Value,
                    replacements:
                        string.Format(
                            "\r\n\r\n{0}:\r\n{1}",
                            duplication.Key.Name,
                            string.Join(Environment.NewLine, from v in duplication.Value
                                                             select string.Format("\t{0}", v.Name))));
        }

        private ParserCompilerPhase phase;
        private bool expandTemplates;
        private Dictionary<SyntacticalDFAState, PredictionTreeLeaf> _allProjectionNodes;
        private Dictionary<IOilexerGrammarProductionRuleEntry, PredictionTreeLeaf> _ruleProjectionNodes;
        private ParserBuilder _parserBuilder;
        private TokenSymbolBuilder _tokenSymbolBuilder;
        private InlinedTokenEntry _coreLexer;
        private IIntermediateClassType _parserClass;

        internal Dictionary<SyntacticalDFAState, PredictionTreeLeaf> AllProjectionNodes
        {
            get
            {
                return this._allProjectionNodes;
            }
        }

        public ParserCompilerPhase Phase
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

        private void OnPhaseChange(ParserCompilerPhase phase)
        {
            if (this.PhaseChange != null)
                this.PhaseChange(this, new ParserBuilderPhaseChangeEventArgs(phase));
        }

        private void FindStartRule()
        {
            IOilexerGrammarProductionRuleEntry startRule;
            if (Source.Options.StartEntry == null || Source.Options.StartEntry == string.Empty)
            {
                CompilationErrors.SourceModelError<OilexerGrammarFile>(OilexerGrammarCore.CompilerErrors.NoStartDefined, LineColumnPair.Zero, LineColumnPair.Zero, new Uri(Source.Files[0], UriKind.RelativeOrAbsolute), Source, Source.Options.GrammarName);
                return;
            }
            if ((startRule = (this.Source.GetRules()).OilexerGrammarFindScannableEntry(Source.Options.StartEntry)) == null)
            {
                CompilationErrors.SourceModelError<OilexerGrammarFile>(OilexerGrammarCore.CompilerErrors.InvalidStartDefined, LineColumnPair.Zero, LineColumnPair.Zero, new Uri(Source.Files[0], UriKind.RelativeOrAbsolute), Source, Source.Options.StartEntry, Source.Options.GrammarName);
                return;
            }
            this.StartEntry = startRule;
        }

        /* *
         * Melt all tokens together in a pot, find out where the edges represent
         * multiple tokens, use this information to find the ambiguities during
         * the syntax stage.
         * */
        private void IdentifyLexerAmbiguities(IIntermediateAssembly resultAssembly)
        {
            
            this.GrammarSymbols = new GrammarSymbolSet(this.Source);
            foreach (var tokenSymbol in this._GrammarSymbols.TokenSymbols)
                this.TokenSymbolDetail.Add(tokenSymbol, new GrammarVocabularyTokenSymbolBreakdown() { Symbol = tokenSymbol });
            /* Create the unified DFA of all tokens. */
            this._coreLexer = CreateLexerCore();
            /* Obtain all edges from the unified deterministic automata. */
            var edges = _coreLexer.DFAState.ObtainEdges();
            var ambiguitySets =
                /* Of the work done below, get just the unique sets. */
                (from edge in edges
                 /* In yielding the symbols from each edge... */
                 let ambiguityQuery = YieldEdgeSymbols(edge)
                 /* Filter the edge symbols based on token precedence. */
                 from ambiguityVariation in FilterEdgeSymbols(ambiguityQuery)
                     /* Make an IEnumerable<T> out of each element */
                    .Splay()
                     /* and find all permutations of those items where there are >= 2 items per set */
                    .GetAllPermutations(2)
                 let ambiguityVariationArray = ambiguityVariation.ToArray()
                 orderby string.Join<IGrammarTokenSymbol>(", ", ambiguityVariationArray)
                 let hashArray = new HashList<IGrammarTokenSymbol>(ambiguityVariationArray)
                 group edge by hashArray).Distinct().ToDictionary(k => k.Key.ToArray(), v => v.ToArray());



            var ambiguityCount =
                ambiguitySets.Sum(k => k.Key.Length);



            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
             * For each ambiguous permutation, generate a symbol for the language which represents this symbol.       *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * These will be reduced down to the relevant set once the language has had its paths evaluated, because  *
             * the references targeting a given symbol are tracked through path analysis.                             *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            List<IGrammarAmbiguousSymbol> ambiguitySymbols = new List<IGrammarAmbiguousSymbol>();
            int index = 0;
            foreach (var distinctSet in ambiguitySets.Keys)
                ambiguitySymbols.Add(new GrammarAmbiguousSymbol(this.GrammarSymbols, distinctSet, ++index, ambiguityCount, ambiguitySets[distinctSet]));
            //foreach (var edgeState in ambiguitySets.Keys)
            //{
            //    var currentSets = ambiguitySets[edgeState];
            //    foreach (var set in currentSets)
            //        ambiguitySymbols.Add(new GrammarAmbiguousSymbol(this.GrammarSymbols, set, ++index, ambiguityCount, edgeState));
            //}
            _GrammarSymbols.GenerateAmbiguityContext(ambiguitySymbols.OrderByDescending(ambiguity => ambiguity.Count).ToList());
        }

        private IEnumerable<IGrammarTokenSymbol> FilterEdgeSymbols(IEnumerable<IGrammarTokenSymbol> set)
        {
            /* Establish a proper conceptual view of token precedence, if Keyword > Identifier > IdentifierNoExpect, then IdentifierNoExpect should not be ambiguous with Identifier. */
            set = set.Distinct().ToArray();
            var setD = (IGrammarTokenSymbol[])set;
            var tokens = (from symbol in set
                          group symbol by symbol.Source).ToDictionary(k => (OilexerGrammarTokenEntry)k.Key, v => v.ToArray());
            var tpt = new TokenPrecedenceTable(tokens.Keys);
            tpt.FilterByPrecedence(tokens);
            return from tSet in tokens.Values
                   from t in tSet
                   orderby t.ToString()
                   select t;
        }

        private IEnumerable<IGrammarTokenSymbol> YieldEdgeSymbols(RegularLanguageDFAState state)
        {
            var sources = state.ObtainEdgeSourceTokenItems().ToArray();
            var midStep = this._GrammarSymbols.SymbolsFromSources(sources).ToArray();
            return midStep;
        }

        private InlinedTokenEntry CreateLexerCore()
        {
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
             * This 'CoreLexer' token is used to isolate ambiguities within     *
             * the deterministic structure of all tokens.                       *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * Using this, for every token k that has all of its edge states    *
             * shared with another token j, k<=j.                               *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **
             * This will be useful for quickly determining the most optimal     *
             * lexing mechanisms.  In cases where x characters of look-ahead    *
             * are insufficient in disambiguating, a unified parse method for   *
             * those locking tokens will be generated.                          *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            var lexerCoreToken = new InlinedTokenEntry(new OilexerGrammarTokenEntry("CoreLexer", new TokenExpressionSeries(new ITokenExpression[0], 0, 0, 0, this.Source.Files.First()), EntryScanMode.Inherited, this.Source.Files.First(), 0, 0, 0, false, new IOilexerGrammarTokenEntry[0], true), this.Source);
            RegularLanguageNFAState lexerCoreNFA = new RegularLanguageNFARootState(lexerCoreToken);

            Dictionary<RegularLanguageNFAState, Tuple<InlinedTokenEntry, RegularLanguageNFAState>> replications = new Dictionary<RegularLanguageNFAState, Tuple<InlinedTokenEntry, RegularLanguageNFAState>>();
            var inlinedTokens = this.Source.GetInlinedTokens();
            foreach (var token in inlinedTokens)
            {
                if (token.NFAState == null)
                    continue;
                List<RegularLanguageNFAState> flatlined = new List<RegularLanguageNFAState>();
                var tokenNFA = token.NFAState;
                RegularLanguageNFAState.FlatlineState(tokenNFA, flatlined);
                if (flatlined.Contains(tokenNFA))
                    flatlined.Remove(tokenNFA);
                foreach (var state in flatlined)
                    replications.Add(state, Tuple.Create(token, new RegularLanguageNFAState()));
                replications.Add(tokenNFA, Tuple.Create(token, lexerCoreNFA));
            }

            foreach (var stateKey in replications.Keys)
            {
                var tknState = replications[stateKey];
                var targetOfTransition = tknState.Item2;
                foreach (var transition in stateKey.OutTransitions.Keys)
                {
                    var movedTo = stateKey.OutTransitions[transition];
                    foreach (var target in movedTo)
                    {
                        var targetTknState = replications[target];
                        var replication = targetTknState.Item2;
                        if (target.IsEdge)
                            replication.IsEdge = true;
                        target.IterateSources((item, type) =>
                        {
                            switch (type)
                            {
                                case FiniteAutomationSourceKind.Final:
                                    if (target.IsEdge)
                                        replication.SetFinal(tknState.Item1);
                                    replication.SetFinal(item);
                                    break;
                                case FiniteAutomationSourceKind.Initial:
                                    replication.SetInitial(item);
                                    break;
                                case FiniteAutomationSourceKind.Intermediate:
                                    replication.SetIntermediate(item);
                                    break;
                                case FiniteAutomationSourceKind.RepeatPoint:
                                    replication.SetRepeat(item);
                                    break;
                            }
                        });
                        targetOfTransition.MoveTo(transition, replication);
                    }
                }
            }
            var lexerCoreDFA = lexerCoreNFA.DeterminateAutomata();
            lexerCoreToken.DFAState = (RegularLanguageDFARootState)lexerCoreDFA;
            lexerCoreDFA.Reduce(RegularCaptureType.ContextfulTransducer
#if ReduceGlobalLexer
                , (currentState, targetState) =>
                {
                    if (currentState.OutTransitions.Count != targetState.OutTransitions.Count)
                        return false;
                    if (!currentState.OutTransitions.Checks.All(
                        k => targetState.OutTransitions.Checks.Contains(k) && currentState.OutTransitions[k] == targetState.OutTransitions[k]))
                        return false;
                    if (currentState.IsEdge != targetState.IsEdge)
                        return false;
                    var originalSources = GetDFATokenSources(currentState);
                    var dupSources = GetDFATokenSources(targetState);
                    /* We need to make sure that we don't merge two separate tokens' paths.  Identity determination needs to be distinct if that's the case. */
                    if (originalSources.Length > 1 ||
                        dupSources.Length > 1)
                        return false;
                    if (originalSources.Length == 0 || /* This shouldn't happen, but I'm sure we don't want to merge an unknown identity with a token. */
                        dupSources.Length == 0)
                        return false;
                    var targetToken = originalSources[0];
                    if (targetToken != dupSources[0])
                        return false;
                    /* We don't want to unify the states of a set of keywords, you'd get weird results, like "lol" and "lal" having the same start and end state!  Which did we match? */
                    if (targetToken.CaptureKind == RegularCaptureType.ContextfulTransducer || 
                        targetToken.CaptureKind == RegularCaptureType.Transducer)
                        return false;

                    return currentState != targetState && currentState.IsReductionSite == targetState.IsReductionSite;
                }
#endif
                );
            lexerCoreDFA.Enumerate();

            return lexerCoreToken;
        }

        private static InlinedTokenEntry[] GetDFATokenSources(RegularLanguageDFAState original)
        {
            return (from t in original.Sources.ObtainEdgeSourceTokenItems()
                    select t is IOilexerGrammarTokenEntry ? (InlinedTokenEntry)t : (((IInlinedTokenItem)(t)).Root)).Distinct().ToArray();
        }

        public void BuildParser()
        {
            this.ParserBuilder.BuildParser();
        }

        private void HandleAmbiguitiesAndLexerCore(IIntermediateAssembly resultAssembly)
        {
            this._GrammarSymbols.PruneUnusedAmbiguities(this.CompilationErrors, this);
            this._GrammarSymbols.GenerateAmbiguitySymbolStoreVariations(this.LexicalSymbolModel, this);

            this.LexicalSymbolModel.CheckValidSymbols();
            this.SyntacticalSymbolModel.CheckValidSymbols();
            foreach (var ambiguity in this._GrammarSymbols.AmbiguousSymbols)
            {
                var currentAmbiguityBreakdown = this.AmbiguityDetail[ambiguity];
                currentAmbiguityBreakdown.Identity = this.LexicalSymbolModel.GetIdentitySymbolField(ambiguity);
                currentAmbiguityBreakdown.AmbiguityReference = this.LexicalSymbolModel.GetSingletonReference(ambiguity);
                currentAmbiguityBreakdown.ValidIdentity = this.LexicalSymbolModel.GetValidSymbolField(ambiguity);
            }
            this._relationalModelMapping.ConnectConstructs(this.RuleDFAStates);
            this._parseResultsBuilder.Build2();
            this.ParserBuilder.Build2();
            this.ErrorContextBuilder.Build2();
            this.TokenStreamBuilder.Build2();
            this.SymbolStreamBuilder.Build2();
            this.CharStreamSegmentBuilder.Build2(this._charStreamBuilder);
            this.VariableTokenBaseBuilder.Build(this, resultAssembly);
            this.FixedTokenBaseBuilder.Build(this, resultAssembly);
            this.ParserBuilder.Build3();
            this.RuleSymbolBuilder.Build2();
            this.ExtensionsBuilder.Build(this, this._parserBuilder, (IIntermediateCliManager)resultAssembly.IdentityManager);
            this.ParserBuilder.Build4();
            this.ParserBuilder.LexerBuilder.Build2();
            this.RootRuleBuilder.Build2();
            this.TokenStreamBuilder.Build3();
            this.RuleSymbolBuilder.Build3();
            foreach (var symbol in this._GrammarSymbols.TokenSymbols)
            {
                var tokenDetail = this.TokenSymbolDetail[symbol];
                tokenDetail.Identity = this.LexicalSymbolModel.GetIdentitySymbolField(symbol);
                tokenDetail.ValidIdentity = this.LexicalSymbolModel.GetValidSymbolField(symbol);
            }
            foreach (var rule in this.Source.GetRules())
                this.RuleDetail[rule].Symbol = this._GrammarSymbols.GetSymbolFromEntry(rule);
            this._relationalModelMapping.BuildPrimaryMembers((IIntermediateCliManager)resultAssembly.IdentityManager);

        }

        private void CreateProjectionStateMachines()
        {
            int maxState = this.ParserMaxState;
            this.AdvanceMachines = SyntacticAnalysisCore.ConstructLookAheadProjections(this, _allProjectionNodes, this.RuleDFAStates, this.RuleGrammarLookup, this._GrammarSymbols, this.CompilationErrors, ref maxState);
            this.AdvanceMachinesReverse = this.AdvanceMachines.ToDictionary(k => k.Value, v => v.Key);
            SyntacticAnalysisCore.AnalyzeDFARepetitions((from ad in this.AdvanceMachines.Values
                                                         from ad2 in ad.All
                                                         select ad2.AssociatedContext).ToArray(), this.AdvanceMachines);
            this.ParserMaxState = maxState;
        }

        private void CreateFollowProjectionsAndMachines()
        {
            this.FollowAmbiguousNodes = SyntacticAnalysisCore.PerformEdgePredictions(this, _allProjectionNodes, RuleDFAStates, RuleGrammarLookup, this._GrammarSymbols, this.CompilationErrors).ToArray();

            this._FollowAdapters = new MultikeyedDictionary<PredictionTreeLeaf, PredictionTreeFollow, SyntacticalDFAState, PredictionTreeDFAdapter>();
            foreach (var node in FollowAmbiguousNodes)
                foreach (var followAmbiguity in node.FollowAmbiguities)
                    foreach (var adapter in followAmbiguity.Adapter.All)
                        _FollowAdapters.Add(node, followAmbiguity, adapter.AssociatedState, adapter);
        }

        private void BuildObjectModelScaffolding(IIntermediateAssembly resultAssembly)
        {
            if (this.scaffoldingBuilt)
                return;
            this.scaffoldingBuilt = true;
            this.BuildSymbolStore(resultAssembly);
            /* *
             * ACC - Update June 02, 2005: Builder classes simplified the synthesis of the
             * code, previously it was ALL done inside the compiler class.  That's insane. 
             * */
            /* *
             * ACC - Update June 09, 2015: Original builder classes used a tuple of 
             * parameters, that wasn't so much insane as it was inane.
             * */
            this._commonSymbolBuilder       = new CommonSymbolBuilder     ();
            this._stackContextBuilder       = new StackContextBuilder     ();
            this._ruleSymbolBuilder         = new RuleSymbolBuilder       ();
            this._rootRuleBuilder           = new RootRuleBuilder         ();
            this._parserBuilder             = new ParserBuilder           ();
            this._tokenSymbolBuilder        = new TokenSymbolBuilder      ();
            this._extensionsBuilder         = new ExtensionsBuilder       ();
            this._parseResultsBuilder      = new ParseResultsBuilder    ();
            this._symbolStoreBuilder        = new SymbolStoreBuilder      ();
            this._charStreamBuilder         = new CharStreamBuilder       ();
            this._charStreamSegmentBuilder  = new CharStreamSegmentBuilder();
            this._variableTokenBaseBuilder  = new VariableTokenBaseBuilder();
            this._fixedTokenBaseBuilder     = new FixedTokenBaseBuilder   ();
            this._errorContextBuilder       = new ErrorContextBuilder     (this, ((IIntermediateCliManager)(resultAssembly.IdentityManager)), resultAssembly);
            var mkdData                     = MultikeyedDictionaryBuilder.CreateNormalVariation(resultAssembly, (IIntermediateCliManager)resultAssembly.IdentityManager, this, 2);
            /* Multi-keyed dictionaries were adapted from an old code generator, so they're not as neatly packaged. */
            this._imultikeyedDictionary     = (IIntermediateInterfaceType)mkdData.Item1[0];
            this._multikeyedDictionary      =     (IIntermediateClassType)mkdData.Item2[0];
            this._mkdKeys                   =    (IIntermediateStructType)mkdData.Item3[0];
            this._mkdKeysValuePair          =    (IIntermediateStructType)mkdData.Item4;
            this._symbolStoreBuilder        .Build(                                                                                 this                             , resultAssembly );
            this._charStreamBuilder         .Build(                                                                                 this                             , resultAssembly );
            this._charStreamSegmentBuilder  .Build(                                                                                 this                             , resultAssembly );
            this._commonSymbolBuilder       .Build(new Tuple<ParserCompiler, GrammarVocabularyModelBuilder, IIntermediateAssembly> (this, this.LexicalSymbolModel    , resultAssembly));
            this._stackContextBuilder       .Build(new Tuple<ParserCompiler, CommonSymbolBuilder          , IIntermediateAssembly> (this, this._commonSymbolBuilder  , resultAssembly));
            this._ruleSymbolBuilder         .Build(new Tuple<ParserCompiler, StackContextBuilder          , IIntermediateAssembly> (this, this._stackContextBuilder  , resultAssembly));
            this._rootRuleBuilder           .Build(new Tuple<ParserCompiler, RuleSymbolBuilder            , IIntermediateAssembly> (this, this._ruleSymbolBuilder    , resultAssembly));
            this._tokenSymbolBuilder        .Build(new Tuple<ParserCompiler, CommonSymbolBuilder          , IIntermediateAssembly> (this, this._commonSymbolBuilder  , resultAssembly));
            this._errorContextBuilder       .Build();
            this._parseResultsBuilder      .Build(Tuple.Create(this, resultAssembly));
            this._relationalModelMapping   = new OilexerGrammarFileObjectRelationalMap(this.Source, this.RuleDFAStates, resultAssembly, this._rootRuleBuilder);
            if (this.CompilationErrors.HasErrors)
                return;

            this._parserBuilder          .Build(Tuple.Create(this, this._tokenSymbolBuilder, resultAssembly, (RegularLanguageDFAState)this._coreLexer.DFAState));
            this._parserClass            = this._parserBuilder.ParserClass;

        }

        public IIntermediateClassMethodMember CreateRuleParseMethod(IOilexerGrammarProductionRuleEntry entry, IIntermediateClassType targetType)
        {

            var ormInfo          = this._relationalModelMapping[entry];
            var rootAdapter      = this.RuleAdapters[entry];
            var methodResultType = rootAdapter.AssociatedContext.ModelInterface;
            return targetType.Methods.Add(new TypedName("Parse{0}", methodResultType, entry.Name));
        }

        private static List<PredictionTree> GetAdapterDPathSets<TAdapter, TAdapterContext>(TAdapter adapter)
            where TAdapter :
                DFAAdapter<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource, TAdapter, TAdapterContext>,
                new()
            where TAdapterContext :
                new()
        {
            return (from    s in adapter.AssociatedState.Sources
                    where   s.Item1 is PredictionTree
                    let     dps = (PredictionTree)s.Item1
                    select  dps).ToList();
        }

        private IEnumerable<IPredictionTreeDestination> GetAdapterDecisions(PredictionTreeDFAdapter adapter)
        {
            return (from    source in adapter.AssociatedState.Sources
                    where   source.Item1 is IPredictionTreeDestination
                    select (IPredictionTreeDestination)source.Item1);
        }

        private static Tuple<List<string>, List<PredictionTreeBranch>> GetAdapterDPathSets(ProductionRuleNormalAdapter adapter)
        {
            var initialSet = GetAdapterDPathSets<ProductionRuleNormalAdapter, ProductionRuleNormalContext>(adapter);
            var distinctPaths = (from   pSet in initialSet
                                 from   p in pSet
                                 select p).Distinct().ToList();
            var distinctTransitions =
                (from   pSet in initialSet
                 select pSet.CurrentLookAheadBranches).ToList();
            return Tuple.Create(distinctTransitions, distinctPaths);
        }

        private static Tuple<List<string>, List<PredictionTreeBranch>> GetAdapterDPathSets(PredictionTreeDFAdapter adapter)
        {
            var initialSet = GetAdapterDPathSets<PredictionTreeDFAdapter, PredictionTreeDFAContext>(adapter);
            var distinctPaths = (from   pSet in initialSet
                                 from   p in pSet
                                 select p).Distinct().ToList();
            var distinctTransitions =
                (from pSet in initialSet
                 select pSet.CurrentLookAheadBranches).ToList();
            return Tuple.Create(distinctTransitions, distinctPaths);
        }


        /// <summary>
        /// Returns/sets the <see cref="Int32"/> value denoting the maximum value of 
        /// the states within all automations of the parser.
        /// </summary>
        public int ParserMaxState { get; set; }

        /// <summary>
        /// Returns the <see cref="Dictionary{TKey, TValue}"/> of deterministic
        /// automata lookups which outline the structure for predicting a non 
        /// LL(1) decision point within a lexer.
        /// </summary>
        internal Dictionary<PredictionTreeLeaf, PredictionTreeDFAdapter> AdvanceMachines { get; set; }

        /// <summary>
        /// Returns the <see cref="Dictionary{TKey, TValue}"/> of deterministic
        /// automata lookups in reverse to the <see cref="PredictionTreeLeaf"/> 
        /// from which the machine was derived.
        /// </summary>
        internal Dictionary<PredictionTreeDFAdapter, PredictionTreeLeaf> AdvanceMachinesReverse{ get; set; }

        /// <summary>
        /// Returns the <see cref="Dictionary{TKey, TValue}"/> lookup of
        /// <see cref="IOilexerGrammarProductionRuleEntry"/> instances to <see cref="GrammarVocabulary"/>
        /// instances which denote the singleton transition of a given language's rules.
        /// </summary>
        public Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabulary> RuleGrammarLookup { get; private set; }

        /// <summary>
        /// Denotes the depth at which to evaluate recursive rules
        /// to detect anomalies.
        /// </summary>
        public int CycleDepth { get; internal set; }

        /// <summary>
        /// Returns the <see cref="GrammarVocabularyModelBuilder"/> which denotes the object model
        /// elements generated from the language elements defined by the all of the symbols 
        /// within <see cref="GrammarSymbols"/>.
        /// </summary>
        public GrammarVocabularyModelBuilder LexicalSymbolModel { get; private set; }

        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> of <see cref="PredictionTreeLeaf"/>
        /// elements that denote ambiguous edge states of a language's non-terminals
        /// relative to the paths that were taken to reach a given rule.
        /// </summary>
        /// <remarks>
        /// Refer to: <seealso cref="PredictionTreeLeaf.FollowAmbiguities"/> for more
        /// information.
        /// </remarks>
        public PredictionTreeLeaf[] FollowAmbiguousNodes { get; set; }
        private IReadonlyMultikeyedDictionary<PredictionTreeLeaf, PredictionTreeFollow, SyntacticalDFAState, PredictionTreeDFAdapter> _followAdapters;
        public GrammarVocabularyModelBuilder SyntacticalSymbolModel { get; private set; }
        private CharStreamBuilder _charStreamBuilder;
        private CharStreamSegmentBuilder _charStreamSegmentBuilder;
        private IIntermediateStructType _mkdKeysValuePair;
        private Oilexer.FixedTokenBaseBuilder _fixedTokenBaseBuilder;
        private bool _linkedProjectionNodes;
        public IReadonlyMultikeyedDictionary<PredictionTreeLeaf, PredictionTreeFollow, SyntacticalDFAState, PredictionTreeDFAdapter> FollowAdapters
        {
            get {
                if (_FollowAdapters == null)
                    return null;
                return this._followAdapters ?? (this._followAdapters = new ReadonlyMultikeyedDictionary<PredictionTreeLeaf, PredictionTreeFollow, SyntacticalDFAState, PredictionTreeDFAdapter>(_FollowAdapters));
            }
        }
        private MultikeyedDictionary<PredictionTreeLeaf, PredictionTreeFollow, SyntacticalDFAState, PredictionTreeDFAdapter> _FollowAdapters { get; set; }


        public SymbolStoreBuilder SymbolStoreBuilder { get { return this._symbolStoreBuilder; } }

        public IIntermediateClassType MultikeyedDictionary { get { return this._multikeyedDictionary; } }
        public IIntermediateInterfaceType IMultikeyedDictionary { get { return this._imultikeyedDictionary; } }
        public IIntermediateStructType MultikeyedDictionaryKeys { get { return this._mkdKeys; } }
        public IIntermediateStructType MultikeyedDictionaryKeysValuePair { get { return this._mkdKeysValuePair; } }


        public TokenSymbolBuilder TokenSymbolBuilder { get { return this._tokenSymbolBuilder; } }
        public TokenStreamBuilder TokenStreamBuilder { get { return this._parserBuilder.LexerBuilder.TokenStreamBuilder; } }

        public CharStreamSegmentBuilder CharStreamSegmentBuilder { get { return this._charStreamSegmentBuilder; } }
        public CharStreamBuilder CharStreamBuilder { get { return this._charStreamBuilder; } }
        public SymbolStreamBuilder SymbolStreamBuilder { get { return this._parserBuilder.SymbolStreamBuilder; } }
        public CommonSymbolBuilder CommonSymbolBuilder { get { return this._commonSymbolBuilder; } }
        public GenericSymbolStreamBuilder GenericSymbolStreamBuilder { get { return this._parserBuilder.GenericSymbolStreamBuilder; } }
        public ParserBuilder ParserBuilder { get { return this._parserBuilder; } }
        public LexerBuilder LexerBuilder { get { return this._parserBuilder.LexerBuilder; } }
        public VariableTokenBaseBuilder VariableTokenBaseBuilder { get { return this._variableTokenBaseBuilder; } }
        public FixedTokenBaseBuilder FixedTokenBaseBuilder { get { return this._fixedTokenBaseBuilder; } }
        public RuleSymbolBuilder RuleSymbolBuilder { get { return this._ruleSymbolBuilder; } }
        public RootRuleBuilder RootRuleBuilder { get { return this._rootRuleBuilder; } }
        public ErrorContextBuilder ErrorContextBuilder { get { return this._errorContextBuilder; } }
        internal List<InlinedTokenEntry> TokensCastAsRules { get; set; }

        internal InlinedTokenEntry TokenCastAsRule { get; set; }

        public StackContextBuilder StackContextBuilder { get { return this._stackContextBuilder; } }

        internal Dictionary<IOilexerGrammarProductionRuleEntry, GrammarVocabularyRuleDetail> RuleDetail { get { return this.aggregateRuleDetails; } }
        internal Dictionary<IGrammarTokenSymbol, GrammarVocabularyTokenSymbolBreakdown> TokenSymbolDetail { get { return this.aggregateTokenSymbolDetails; } }

        public bool scaffoldingBuilt { get; set; }

        public IIntermediateAssembly ResultVsixAssembly { get; set; }

        public HashSet<IOilexerGrammarProductionRuleEntry> ReducedRules { get; private set; }

        internal void DenoteReduction(IOilexerGrammarProductionRuleEntry rule)
        {
            this.ReducedRules.Add(rule);
        }
    }
}
