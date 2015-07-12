using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Utilities;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Cli;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Utilities;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Utilities.Collections;
using System.Diagnostics;
using System.Globalization;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Ast.Cli;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Builds the lexer for a given language.
    /// </summary>
    public class LexerBuilder :
        IConstructBuilder<Tuple<ParserCompiler, ParserBuilder, IIntermediateAssembly, TokenSymbolBuilder, bool, GenericSymbolStreamBuilder, RegularLanguageDFAState>, Tuple<IIntermediateInterfaceType, IIntermediateClassType, IIntermediateMethodMember, TokenStreamBuilder>>
    {
        private IIntermediateInterfaceType _lexerInterface;
        private IIntermediateClassType lexerClass;
        private IIntermediateAssembly _initialAssembly;
        private ParserBuilder _parserBuilder;
        private ParserCompiler _compiler;
        private TokenStreamBuilder _tokenStreamBuilder;
        private TokenSymbolBuilder _tokenSymbolBuilder;
        private RegularLanguageDFAState _lexerCore;
        private ITypedLocalMember _ntCurrentState;
        private ITypedLocalMember _ntCurrentLADepth;
        private ITypedLocalMember _ntExitState;
        private ITypedLocalMember _ntExitLADepth;
        private ITypedLocalMember _ntCurrentChar;
        private UnicodeCollectiveTargetGraph _collectiveUnicodeGraph;
        private string _statePattern;
        private IMethodReferenceStub _charGetUnicodeCategoryExpr;
        private Dictionary<RegularLanguageDFAState, RegularLanguageDFAStateJumpData> _multitargetLookup;
        private Dictionary<IBlockStatementParent, Dictionary<RegularLanguageDFAState, RegularLanguageDFAStateJumpData>> _secondaryLookups = new Dictionary<IBlockStatementParent, Dictionary<RegularLanguageDFAState, RegularLanguageDFAStateJumpData>>();
        private HashSet<RegularLanguageDFAState> _visited = new HashSet<RegularLanguageDFAState>();
        private IIntermediateInterfacePropertyMember _lexerParserReference;
        private IIntermediateClassFieldMember _lexerParserFieldReference;
        private IIntermediateClassPropertyMember _lexerParserReferenceImpl;
        private IIntermediateCliManager _identityManager;

        public Tuple<IIntermediateInterfaceType, IIntermediateClassType, IIntermediateMethodMember, TokenStreamBuilder> Build(Tuple<ParserCompiler, ParserBuilder, IIntermediateAssembly, TokenSymbolBuilder, bool, GenericSymbolStreamBuilder, RegularLanguageDFAState> input)
        {
            this._charGetUnicodeCategoryExpr = input.Item3.IdentityManager.ObtainTypeReference(RuntimeCoreType.Char).GetTypeExpression().GetMethod("GetUnicodeCategory");
            this._compiler = input.Item1;
            this._parserBuilder = input.Item2;
            this._initialAssembly = input.Item3;
            this._identityManager = (IIntermediateCliManager)this._initialAssembly.IdentityManager;
            this.ScannerNested = input.Item5;
            this._lexerCore = input.Item7;
            this._lexerInterface = _initialAssembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}", _compiler.Source.Options.LexerName);
            if (ScannerNested)
            {
                var nameLeftDiff = _compiler.Source.Options.LexerName.LeftDiff(_compiler.Source.Options.ParserName);

                this.lexerClass = this._parserBuilder.ParserClass.Parts.Add().Classes.Add(_compiler.Source.Options.LexerName.Substring(nameLeftDiff));
            }
            else
                this.lexerClass = this._initialAssembly.DefaultNamespace.Parts.Add().Classes.Add(_compiler.Source.Options.LexerName);
            this._tokenStreamBuilder = new TokenStreamBuilder();
            this._tokenSymbolBuilder = input.Item4;
            _tokenStreamBuilder.Build(Tuple.Create(this._compiler, input.Item4, this._initialAssembly, input.Item6));


            this.BuildLexerInterface();
            this.BuildLexerClass();
            return Tuple.Create(this.LexerInterface, this.LexerClass, (IIntermediateMethodMember)null, this._tokenStreamBuilder);
        }


        #region Major Exports

        /// <summary>
        /// Returns the <see cref="IIntermediateInterfaceType"/> which results
        /// from building the lexer for the language.
        /// </summary>
        public IIntermediateInterfaceType LexerInterface { get { return this._lexerInterface; } }

        /// <summary>
        /// Returns the <see cref="IIntermediateClassType"/> which results
        /// from building the lexer for the language.
        /// </summary>
        public IIntermediateClassType LexerClass { get { return this.lexerClass; } }
        #endregion

        #region Imports
        /// <summary>
        /// Returns the <see cref="IIntermediateAssembly"/> in which the
        /// <see cref="LexerInterface"/> and <see cref="LexerClass"/> 
        /// are created within.
        /// </summary>
        public IIntermediateAssembly OwningAssembly { get { return this._initialAssembly; } }

        /// <summary>
        /// Returns the <see cref="ParserBuilder"/> in which the context sensitive 
        /// <see cref="LexerClass"/> may be nested within.
        /// </summary>
        public ParserBuilder ParserBuilder { get { return this._parserBuilder; } }

        /// <summary>
        /// Returns the <see cref="ParserCompiler"/> instance which contains the internal
        /// state for handling the compilation of the lexer/parser.
        /// </summary>
        public ParserCompiler Compiler { get { return this._compiler; } }

        /// <summary>
        /// Returns whether or not the <see cref="LexerClass"/> is nested within the 
        /// <see cref="Parser"/>.
        /// </summary>
        public bool ScannerNested { get; private set; }


        public IIntermediateInterfaceType Parser { get { return this._parserBuilder.ParserInterface; } }
        public IIntermediateClassType ParserImpl { get { return this._parserBuilder.ParserClass; } }

        
        #endregion

        #region Interface Exports

        private void BuildLexerInterface()
        {
            this.LexerInterface.SummaryText = string.Format(@"Defines properties and methods for working with the tokenizer for the {0}{1}.", this.Compiler.Source.Options.GrammarName, this.Compiler.Source.Options.GrammarName.ToLower().EndsWith("language") ? string.Empty : " language");
            this.LexerInterface.ImplementedInterfaces.Add(typeof(IDisposable).ObtainCILibraryType<IInterfaceType>(this._initialAssembly.IdentityManager));
            this.TokenStream = CreateTokenStream();
            this.EndOfStreamReached  = CreateEosReached();
            this.Position = CreatePositionProperty();
            this.LexerInterface.AccessLevel = AccessLevelModifiers.Public;
        }

        private IIntermediateInterfacePropertyMember CreateTokenStream()
        {
            var tokenStream = this.LexerInterface.Properties.Add(new TypedName("TokenStream", this._tokenStreamBuilder.ResultInterface), true, false);
            tokenStream.SummaryText = string.Format("Returns the @s:{0}; which represents the current series of tokens that have been read from the @s:TextStream;.", this._tokenStreamBuilder.ResultInterface.Name);
            return tokenStream;
        }


        private IIntermediateInterfacePropertyMember CreateEosReached()
        {
            var endOfStreamReached = this.LexerInterface.Properties.Add(new TypedName("EndOfStreamReached", this._initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Boolean)), true, false);

            endOfStreamReached.SummaryText = @"Returns a @s:Boolean; value denoting whether the end of @s:TextStream; has been reached.";
            endOfStreamReached.RemarksText = string.Format(@"This value does not necessarily determine the value of @s:{0}.EndOfFilePresent;, but merely that the buffer has reached the end of the stream.", this._tokenStreamBuilder.ResultInterface.Name);
            return endOfStreamReached;
        }

        private IIntermediateInterfacePropertyMember CreatePositionProperty()
        {
            var position = this.LexerInterface.Properties.Add(new TypedName("Position", this._initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Int64)), true, true);
            position.SummaryText = @"Returns the @s:Int64; value denoting the position within the @s:TextStream;.";
            position.RemarksText = "Zero based.";
            return position;
        }

        private IIntermediateInterfacePropertyMember CreateCurrentLineProperty(IIntermediateInterfacePropertyMember charStream)
        {
            var currentLine = this.LexerInterface.Properties.Add(new TypedName("CurrentLine", this._initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Int32)), true, false);
            currentLine.SummaryText = string.Format(@"Returns the @s:Int32; value denoting the line within the @s:{1}; the @s:{0}; is at.", this.LexerInterface.Name, charStream.Name);
            return currentLine;
        }

        private IIntermediateInterfacePropertyMember CreateCurrentColumnProperty()
        {
            var currentColumn = this._lexerInterface.Properties.Add(new TypedName("CurrentColumn", this._initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Int32)), true, false);
            currentColumn.SummaryText = string.Format(@"Returns the @s:Int32; value denoting the column within the @s:CurrentLine; the @s:{0}; is at.", this.LexerInterface.Name);
            return currentColumn;
        }

        /// <summary>
        /// Returns the <see cref="IIntermediateInterfacePropertyMember"/> which
        /// describes the initial definition of the token stream for the lexer
        /// to expose to the implementing <see cref="Parser"/>.
        /// </summary>
        public IIntermediateInterfacePropertyMember TokenStream { get; set; }
        /// <summary>
        /// Returns the <see cref="IIntermediateInterfacePropertyMember"/> which
        /// describes whether or not the end of the stream has been reached.
        /// </summary>
        /// <remarks>
        /// This is determined by the results from an internal
        /// character buffer read yielding a value less than expected on
        /// available characters.
        /// </remarks>
        public IIntermediateInterfacePropertyMember EndOfStreamReached { get; set; }
        /// <summary>
        /// Returns the <see cref="IIntermediateInterfacePropertyMember"/> which 
        /// describes the <see cref="UInt64"/> value that notes where within
        /// the <see cref="TextStream"/> the <see cref="LexerInterface"/> is.
        /// </summary>
        /// <remarks>Zero based.</remarks>
        public IIntermediateInterfacePropertyMember Position { get; set; }
        /// <summary>
        /// Returns a <see cref="IIntermediateInterfacePropertyMember"/> instance
        /// which describes the <see cref="Int32"/> value which denotes 
        /// the current line within the <see cref="TextStream"/>.
        /// </summary>
        /// <remarks>One based.</remarks>
        public IIntermediateInterfacePropertyMember CurrentLine { get; set; }
        /// <summary>
        /// Returns a <see cref="IIntermediateInterfacePropertyMember"/> instance
        /// which describes the <see cref="Int32"/> value which denotes
        /// the current column of the <see cref="CurrentLine"/>.
        /// </summary>
        /// <remarks>One based.</remarks>
        public IIntermediateInterfacePropertyMember CurrentColumn { get; set; }
        #endregion

        #region Class Exports

        private void BuildLexerClass()
        {

            /* ACC - Update June 30, 2015: Please update this to reflect the new build pattern, this is a mess. */
            this.LexerClass.SummaryText = string.Format(@"Provides a base implementation of @s:{2}; which defines properties and methods for working with the tokenizer for the {0}{1}.", this.Compiler.Source.Options.GrammarName, this.Compiler.Source.Options.GrammarName.ToLower().EndsWith("language") ? string.Empty : " language", this.LexerInterface.Name);
            this.LexerClass.ImplementedInterfaces.ImplementInterfaceQuick(this.LexerInterface);
            var tokenStream = this.LexerClass.Properties.Add(new TypedName("TokenStream", this._tokenStreamBuilder.ResultInterface), true, false);
            tokenStream.SummaryText = this.TokenStream.SummaryText;
            //var textStream = this.LexerClass.Properties.Add(new TypedName("TextStream", typeof(TextReader).ObtainCILibraryType<IClassType>(this._initialAssembly.IdentityManager)), true, false);
            //textStream.SummaryText = this.TextStream.SummaryText;
            var endOfStreamReached = this.LexerClass.Properties.Add(new TypedName("EndOfStreamReached", this._initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Boolean)), true, false);
            endOfStreamReached.SummaryText = this.EndOfStreamReached.SummaryText;
            endOfStreamReached.RemarksText = this.EndOfStreamReached.RemarksText;
            var position = this.LexerClass.Properties.Add(new TypedName("Position", this._initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Int64)), true, true);
            position.SummaryText = this.Position.SummaryText;
            position.RemarksText = this.Position.RemarksText;
            var currentLine = this.LexerClass.Properties.Add(new TypedName("CurrentLine", this._initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Int32)), true, false);
            var currentColumn = this.LexerClass.Properties.Add(new TypedName("CurrentColumn", this._initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Int32)), true, false);
            currentColumn.GetMethod.Return(IntermediateGateway.NumberZero);
            currentLine.GetMethod.Return(IntermediateGateway.NumberZero);
            this.TokenStreamImpl = tokenStream;
            //this.TextStreamImpl = textStream;
            this.PositionImpl = position;
            this.EndOfStreamReachedImpl = endOfStreamReached;
            this.CurrentLineImpl = currentLine;
            this.CurrentColumnImpl = currentColumn;
            var positionField = this.LexerClass.Fields.Add(new TypedName("position", this.PositionImpl.PropertyType));
            this.PositionImpl.GetMethod.Return(positionField.GetReference());
            positionField.SummaryText = string.Format("Data member for @s:{0};.", PositionImpl.Name);
            this.PositionImpl.SetMethod.Assign(positionField.GetReference(), this.PositionImpl.SetMethod.ValueParameter.GetReference());
            var tokenStreamField = this.LexerClass.Fields.Add(new TypedName("tokenStream", this._tokenStreamBuilder.ResultClass));
            
            this.TokenStreamImpl.GetMethod.Return(tokenStreamField.GetReference());

            var eosReached = this.lexerClass.Fields.Add(new TypedName("endOfStreamReached", _initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Boolean)));
            EndOfStreamReachedImpl.GetMethod.Return(eosReached.GetReference());
            eosReached.SummaryText = string.Format("Data member for @s:{0}; which denotes whether the stream's end has been encountered during buffering.", EndOfStreamReachedImpl.Name);

            CreateInitializeMethod(CreateDisposeMethod(eosReached));
            this._lexerParserReference = this.LexerInterface.Properties.Add(new TypedName("Parser", this.Parser), true, false);
            this._lexerParserReferenceImpl = this.LexerClass.Properties.Add(new TypedName("Parser", this.Parser), true, false);
            this._lexerParserFieldReference = this.LexerClass.Fields.Add(new TypedName("_parser", this.ParserImpl));
            this._lexerParserReferenceImpl.GetMethod.Return(this._lexerParserFieldReference.GetReference());
            this._lexerParserFieldReference.AccessLevel = AccessLevelModifiers.Private;
            this._lexerParserReferenceImpl.AccessLevel = AccessLevelModifiers.Public;
            this.LexerClass.AccessLevel = AccessLevelModifiers.Internal;
            BuildCtor();

        }

        private void BuildCtor()
        {
            this.LexerCtor = this.LexerClass.Constructors.Add(new TypedName("parser", this.ParserBuilder.ParserClass));
            this.LexerCtor.Assign(this._lexerParserFieldReference.GetReference(), this.LexerCtor.Parameters["parser"].GetReference());
            this.LexerCtor.AccessLevel = AccessLevelModifiers.Public;
        }

        public void Build2()
        {
            /* ACC Update June 03, 2015: NextToken method requires context from the parse phase, so it had to be pushed back to a separate Build method. */
            var charStream = this.LexerInterface.Properties.Add(new TypedName("CharStream", this._compiler.CharStreamBuilder.ICharStream), true, false);
            var charStreamImpl = this.LexerClass.Properties.Add(new TypedName("CharStream", this._compiler.CharStreamBuilder.ICharStream), true, false);
            var _charStreamImpl = this.LexerClass.Fields.Add(new TypedName("_charStream", this._compiler.CharStreamBuilder.CharStream));
            charStreamImpl.GetMethod.Return(_charStreamImpl);
            charStreamImpl.AccessLevel = AccessLevelModifiers.Public;
            this.CharStream = charStream;
            this.CharStreamImpl = charStreamImpl;
            this._CharStreamImpl = _charStreamImpl;
            this.NextTokenImpl = this.BuildNextTokenMethodImpl();
            this.NextToken = this.BuildNextTokenMethod();
            _charStreamImpl.AccessLevel = AccessLevelModifiers.Private;
            this.CurrentLine = CreateCurrentLineProperty(this.CharStream);
            this.CurrentColumn = CreateCurrentColumnProperty();
            CurrentLine.SummaryText = string.Format(@"Returns the @s:Int32; value denoting the line within the @s:{1}; the @s:{0}; is at.", this.LexerClass.Name, this.CharStream.Name);
            CurrentColumn.SummaryText       = string.Format(@"Returns the @s:Int32; value denoting the column within the @s:CurrentLine; the @s:{0}; is at.", this.LexerClass.Name);
            CurrentLineImpl.SummaryText = string.Format(@"Returns the @s:Int32; value denoting the line within the @s:{1}; the @s:{0}; is at.", this.LexerClass.Name, this.CharStream.Name);
            CurrentColumnImpl.SummaryText   = string.Format(@"Returns the @s:Int32; value denoting the column within the @s:CurrentLine; the @s:{0}; is at.", this.LexerClass.Name);
            this.FinishDisposeMethodImpl();
            this.FinishInitializeMethodImpl();
            SetClassMembersAccessLevel();
        }

        private void FinishInitializeMethodImpl()
        {
            var streamParam = this.InitializeMethod.Parameters["stream"];
            this.InitializeMethod.Assign(this._CharStreamImpl, this._CharStreamImpl.FieldType.GetNewExpression(streamParam.GetReference()));
        }

        private void FinishDisposeMethodImpl()
        {
            var charStreamNullCheck = this.DisposeMethodImpl.If(this._CharStreamImpl.InequalTo(IntermediateGateway.NullValue));
            charStreamNullCheck
                .Call(this._compiler.CharStreamBuilder.DisposeMethodImpl.GetReference(this._CharStreamImpl.GetReference())).FollowedBy()
                .Assign(this._CharStreamImpl, IntermediateGateway.NullValue);
        }

        public IIntermediateInterfacePropertyMember LexerParser { get { return this._lexerParserReference; } }
        public IIntermediateClassPropertyMember LexerParserImpl { get { return this._lexerParserReferenceImpl; } }

        public IIntermediateClassFieldMember LexerParserImplField { get { return this._lexerParserFieldReference; } }

        private IIntermediateInterfaceMethodMember BuildNextTokenMethod()
        {
            return this.LexerInterface.Methods.Add(new TypedName("NextToken", this.Compiler.TokenSymbolBuilder.ILanguageToken));
        }

        private IIntermediateClassMethodMember BuildNextTokenMethodImpl()
        {
            /*ToDo: Utilize the flow graph. */
            //var mued = RegularLanguageDFAFlowGraph.CreateFlowGraph(this._lexerCore);
            var flatline = new List<RegularLanguageDFAState>();
            RegularLanguageDFAState.FlatlineState(this._lexerCore, flatline);

            if (!flatline.Contains(this._lexerCore))
                flatline.Insert(0, this._lexerCore);

            flatline = (from state in flatline
                        orderby state.StateValue
                        select state).ToList();

            this._statePattern = '0'.Repeat(flatline.Select(k=>k.StateValue).Max().ToString().Length);

            /* *
             * We build a set of top level states to indicate those which must have a label indicating their position.
             * *
             * All else is nested inside the owning state.  This will yield VERY LARGE state chains.
             * */
            var multiTargetStates = (from state in flatline
                                     let transitionCount = state.InTransitions.Sum(k => k.Value.Count)
                                     where state == this._lexerCore ||
                                           transitionCount > 1
                                     select state).ToArray();

            var result = this.LexerClass.Methods.Add(new TypedName("NextToken", this.Compiler.TokenSymbolBuilder.ILanguageToken));
            /* The state of _ntExitLADepth also denotes whether we've hit a termianl edge. */
            RuntimeCoreType buildStateType = 
#if x64
                RuntimeCoreType.Int64;
#elif x86
#if !HalfWord
                RuntimeCoreType.Int32;
#else
                RuntimeCoreType.Int16;            
#endif
#endif
            const int negativeOne = -1;
            this._ntExitLADepth = result.Locals.Add(new TypedName("exitLADepth", RuntimeCoreType.Int32, this._initialAssembly.IdentityManager), negativeOne.ToPrimitive());
            this._ntCurrentLADepth = result.Locals.Add(new TypedName("currentLADepth", RuntimeCoreType.Int32, this._initialAssembly.IdentityManager), this.PositionImpl.Subtract(1).Cast(this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32)));
            this._ntExitState = result.Locals.Add(new TypedName("exitState", buildStateType, this._initialAssembly.IdentityManager), negativeOne.ToPrimitive());
            this._ntCurrentState = result.Locals.Add(new TypedName("currentState", buildStateType, this._initialAssembly.IdentityManager), this._lexerCore.StateValue.ToPrimitive());
            this._ntCurrentChar = result.Locals.Add(new TypedName("char", RuntimeCoreType.Int32, this._initialAssembly.IdentityManager), negativeOne.ToPrimitive());
            this._collectiveUnicodeGraph = new UnicodeCollectiveTargetGraph();
            var globalUnicodeGraphs = new Dictionary<IUnicodeTargetGraph, ILabelStatement>();
            this._multitargetLookup = GetTargetStateData(multiTargetStates, result, "MultitargetState_{{0:{0}}}").ToDictionary(k => k.State, v => v);
            /* *
             * Obtain a reference to the entry-point label to avoid an un-necessary jump to it.  We lazily create the labels due to the out-of-order process involved
             * in generating the state machine code building blocks.
             * */
            var entryLabelReference = this._multitargetLookup[this._lexerCore].Label.Value;
            /* It's necessary to specify the decision point last, to ensure it's entered only by fall through from the last MultitargetState and by explicit jumps. */
            var decisionPoint = new LabelStatement(result, "DecisionPoint");
            var remainingToProcess = new Stack<Tuple<RegularLanguageDFAState, IBlockStatementParent, RegularLanguageDFAStateJumpData>>();
            Func<IUnicodeTargetGraph, ILabelStatement> globalGraphHandler = g =>
                {
                    ILabelStatement r;
                    if (!globalUnicodeGraphs.TryGetValue(g, out r))
                        globalUnicodeGraphs.Add(g, r = GenerateUnicodeGraph(result, g, _multitargetLookup, decisionPoint, "UnicodeGraph_{0}", globalUnicodeGraphs.Count + 1, true));
                    return r;
                };
            foreach (var topLevelState in _multitargetLookup.Values)
                GenerateNextTokenState(remainingToProcess, topLevelState.LocalizedContainer, topLevelState.State, decisionPoint, globalGraphHandler);
            while (remainingToProcess.Count > 0)
            {
                var currentSet = remainingToProcess.Pop();
                GenerateNextTokenState(remainingToProcess, currentSet.Item2, currentSet.Item1, decisionPoint, globalGraphHandler);
            }
            foreach (var parent in _secondaryLookups.Keys.Reverse())
                HandleStatementInjections(parent, _secondaryLookups[parent]);
            HandleStatementInjections(result, _multitargetLookup);
            result.Add(decisionPoint);
            var exitLength = result.Locals.Add(new TypedName("exitLength", _ntExitLADepth.LocalType), _ntExitLADepth.Subtract(this.PositionImpl).Add(1).Cast(_identityManager.ObtainTypeReference(RuntimeCoreType.Int32)));
            var lexerAmbiguityIdentity = result.Locals.Add(new TypedName("lexerAmbiguityIdentity", this.Compiler.LexicalSymbolModel.ValidSymbols));
            var ruleAmbiguityIdentity = result.Locals.Add(new TypedName("ruleAmbiguityIdentity", this.Compiler.LexicalSymbolModel.ValidSymbols));
            var resultIdentity = result.Locals.Add(new TypedName("resultIdentity", this.Compiler.LexicalSymbolModel.IdentityEnum), this._compiler.LexicalSymbolModel.NoIdentityField.GetReference());
            lexerAmbiguityIdentity.AutoDeclare =
             ruleAmbiguityIdentity.AutoDeclare =
                    resultIdentity.AutoDeclare =
                        exitLength.AutoDeclare = false;//Ensure it shows up at the end like we want it to.
            var resultIdentityLocalDef = result.DefineLocal(resultIdentity);

            var exitSwitch = result.Switch(this._ntExitState.GetReference());
            var variableResult = result.DefineLabel("VariableResult");
            result.DefineLocal(exitLength);
            result.Return(this._compiler.VariableTokenBaseBuilder.LanguageVariableToken.GetNewExpression(this.CharStreamImpl.GetReference(), this.PositionImpl.GetReference().Cast(this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32)), this.PositionImpl.Add(exitLength).Cast(this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32)), resultIdentity.GetReference()));
            var fixedResult = result.DefineLabel("FixedResult");
            result.Return(this._compiler.FixedTokenBaseBuilder.LanguageFixedToken.GetNewExpression(resultIdentity.GetReference(), this.PositionImpl.GetReference().Cast(this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32))));
            var terminalEdges =
                (from edgeState in this._lexerCore.ObtainEdges()
                 orderby edgeState.StateValue
                 select new { EdgeState = edgeState, Sources = edgeState.ObtainEdgeSourceTokenItems().FilterSourceTokenItems(this.Compiler.GrammarSymbols).ToArray() }).ToDictionary(k => k.EdgeState, v => v);
            
            var terminalEdgeGrouping =
                (from edgeInfo in terminalEdges
                 group edgeInfo by new HashList<ITokenSource>(edgeInfo.Value.Sources)).ToDictionary(k => k.Key, v => GetGroupDFAState(v.Select(elementInfo => elementInfo.Value.EdgeState).ToArray(), exitSwitch));

            bool ambiguityHit = false;
            bool multipleTargetEdges = false;
            foreach (var edgeStateInfo in terminalEdgeGrouping.Keys)
            {
                var currentCaseElement = terminalEdgeGrouping[edgeStateInfo].Item1;
                var sourceItemNames = string.Join(", ", edgeStateInfo.Select(k => (k is IOilexerGrammarTokenEntry ? ((IOilexerGrammarTokenEntry)k).Name : ((ITokenItem)k).Name)).Distinct());
                var symbols = this.Compiler._GrammarSymbols.SymbolsFromSources(edgeStateInfo);
                var preFilterSymbols = symbols.ToArray();
                var tokens = (from symbol in symbols
                              group symbol by symbol.Source).ToDictionary(k => (OilexerGrammarTokenEntry)k.Key, v => v.ToArray());
                /* Before we start saying there's ambiguities, we need to ogranize things based on their precedence, this will enable us to provide an accurate answer further down.  Usually
                 * most grammars will have their keywords filter out the Identifiers. */
                var tokenPrecedenceTable = new TokenPrecedenceTable(tokens.Keys);
                GrammarVocabulary prefilterVocabulary;
                if (tokenPrecedenceTable.Count > 1)
                {
                    tokenPrecedenceTable.FilterByPrecedence(tokens);
                    symbols = (from tSet in tokens.Values
                               from t in tSet
                               orderby t.ToString()
                               select t).ToArray();
                    prefilterVocabulary = new GrammarVocabulary(this.Compiler.GrammarSymbols, preFilterSymbols.Except(symbols).ToArray());
                }
                else
                    prefilterVocabulary = null;

                var vocabulary = new GrammarVocabulary(this.Compiler.GrammarSymbols, symbols.ToArray());
                IIntermediateClassPropertyMember lexerEdgeProperty = null;
                IIntermediateClassPropertyMember lexerAmbiguityEdgeProperty = null;
                IGrammarAmbiguousSymbol[] ambiguities = new IGrammarAmbiguousSymbol[0];
                GrammarVocabulary ambiguityVocab = null;
                if (vocabulary.TrueCount > 1)
                {
                    string lexerClassFullName = this.ScannerNested ? string.Format("{0}.{1}", ((IClassType)this.LexerClass.Parent).Name, this.LexerClass.Name) : this.LexerClass.Name;
                    ambiguities = (from ambiguity in this.Compiler._GrammarSymbols.AmbiguousSymbols
                                   let intersection = ambiguity.AmbiguityKey & vocabulary
                                   where intersection.Equals(ambiguity.AmbiguityKey)
                                   select ambiguity).ToArray();
                    if (!ambiguityHit)
                        ambiguityHit = ambiguities.Length > 0;
                    ambiguityVocab = new GrammarVocabulary(this.Compiler.GrammarSymbols, ambiguities.ToArray());
                    multipleTargetEdges = true;
                    if (ambiguities.Length > 0)
                    {
                        lexerEdgeProperty = this.GrammarVocabularyModel.GenerateSymbolstoreVariation(vocabulary | ambiguityVocab, "LexerEdge", string.Format("Denotes an ambiguous edge of the @s:{0}; which represents multiple possible terminals due to ambiguity.", lexerClassFullName), string.Format("Represents an ambiguous edge of the grammar, for the following terminal symbols: {0}", vocabulary));
                        lexerAmbiguityEdgeProperty = this.GrammarVocabularyModel.GenerateSymbolstoreVariation(ambiguityVocab, "LexerAmbiguityEdge", string.Format("Denotes an ambiguous edge of the @s:{0}; which outlines the specifics of the ambiguity.", lexerClassFullName), string.Format("Represents an ambiguous edge of the grammar, for the following terminal symbols: {0}", vocabulary));
                    }
                    else
                        lexerEdgeProperty = this.GrammarVocabularyModel.GenerateSymbolstoreVariation(vocabulary, "LexerEdge", string.Format("Denotes an edge of the @s:{0}; which represents multiple possible terminals due to ambiguity.", lexerClassFullName), string.Format("Represents an ambiguous edge of the grammar, for the following terminal symbols: {0}", vocabulary));

                }

                currentCaseElement.Comment(string.Format("Edge for: {{ {0} }}{2}{1}", vocabulary, ambiguities.Length > 0 ? string.Format("\r\n\tAmbiguityContext: {0}", ambiguityVocab) : string.Empty, prefilterVocabulary != null ? string.Format(", precedence obviated terminals: {0}", prefilterVocabulary) : string.Empty));
                if (ambiguities.Length > 0 || vocabulary.TrueCount > 1)
                {
                    currentCaseElement.Assign(ruleAmbiguityIdentity.GetReference(), this.Compiler.ExtensionsBuilder.GetValidSyntaxMethod.GetReference(this.LexerParserImpl.GetReference()).Invoke());
                    currentCaseElement.Assign(lexerAmbiguityIdentity.GetReference(), lexerEdgeProperty.GetReference().BitwiseAnd(ruleAmbiguityIdentity));
                    if (ambiguities.Length > 0)
                    {
                        var ambiguityCheck = currentCaseElement.If(lexerAmbiguityIdentity.GetReference().GetProperty("HasAmbiguity"));
                        ambiguityCheck.Assign(resultIdentity.GetReference(), lexerAmbiguityIdentity.BitwiseAnd(lexerAmbiguityEdgeProperty).Cast(resultIdentity.LocalType));
                        ambiguityCheck.CreateNext();
                        ambiguityCheck.Next.Assign(resultIdentity.GetReference(), lexerAmbiguityIdentity.GetReference().Cast(resultIdentity.LocalType));

                    }
                    else
                    {
                        currentCaseElement.Assign(resultIdentity.GetReference(), lexerAmbiguityIdentity.GetReference().Cast(resultIdentity.LocalType));
                        var variableVariants =
                            symbols.Where(k => !(k is IGrammarConstantEntrySymbol || k is IGrammarConstantItemSymbol)).ToArray();
                        var fixedVariants = symbols.Except(variableVariants).ToArray();
                        if (fixedVariants.Length == 0 || variableVariants.Length == 0)
                        {
                            if (fixedVariants.Length > 0)
                                currentCaseElement.GoTo(fixedResult);
                            else
                                currentCaseElement.GoTo(variableResult);
                        }
                        else
                        {
                            var subResultSwitch = currentCaseElement.Switch(resultIdentity.GetReference());
                            var fixedIdentities =
                                this._compiler.LexicalSymbolModel.GetIdentitySymbolsFieldReferences(fixedVariants).ToArray();

                            var variableIdentities =
                                variableVariants.Select(id => (IExpression)this._compiler.LexicalSymbolModel.GetIdentitySymbolField(id).GetReference()).ToArray();
                            subResultSwitch.Case(fixedIdentities).GoTo(fixedResult);
                            subResultSwitch.Case(true, variableIdentities).GoTo(variableResult);
                        }
                    }
                }
                else if (vocabulary.TrueCount == 1)
                {
                    var symbol = vocabulary.GetSymbols()[0];
                    currentCaseElement.Assign(resultIdentity.GetReference(), this.Compiler.LexicalSymbolModel.GetIdentitySymbolField(symbol).GetReference());
                    if (symbol is IGrammarConstantEntrySymbol || symbol is IGrammarConstantItemSymbol)
                        currentCaseElement.GoTo(fixedResult);
                    else
                        currentCaseElement.GoTo(variableResult);
                }
            }

            var defaultCase = exitSwitch.Case(true);
            var eosCheck = defaultCase.If(_ntCurrentChar.EqualTo(-1));
            var firstWasEoS = eosCheck.If(
                _ntCurrentState
                    .EqualTo(
                        _lexerCore.StateValue.ToPrimitive())
                .LogicalAnd(
                    _ntCurrentLADepth.Subtract(this.PositionImpl)
                        .EqualTo(
                            IntermediateGateway.NumberZero)));
            defaultCase.GoTo(fixedResult);
            firstWasEoS.CreateNext(_ntExitState.EqualTo(-1));
            var neverEnteredValidExitCheck = firstWasEoS.Next;
            neverEnteredValidExitCheck.Comment("We never entered a valid exit state.");
            neverEnteredValidExitCheck.Assign(resultIdentity.GetReference(), this._compiler.LexicalSymbolModel.NoIdentityField.GetReference());
            firstWasEoS.Comment("Method was entered, and we're immediately at the end of the stream.");
            firstWasEoS.Assign(resultIdentity.GetReference(), this._compiler.LexicalSymbolModel.GetEofIdentityField().GetReference());
            RegularLanguageAmbiguityState ambiguityState;
            if (!ambiguityHit && multipleTargetEdges)
                ambiguityState = RegularLanguageAmbiguityState.AmbiguousButDeterministic;
            else if (ambiguityHit)
                ambiguityState = RegularLanguageAmbiguityState.Ambiguous;
            else
                ambiguityState = RegularLanguageAmbiguityState.Unambiguous;
            switch (ambiguityState)
            {
                case RegularLanguageAmbiguityState.Ambiguous:
                    //We needed these locals to ensure we had a proper identity to refer to.  
                    //The determinism of ambiguous vs unambiguous could've been handled separately, but that's a future task.
                    result.AddAfter(resultIdentityLocalDef, lexerAmbiguityIdentity.GetDeclarationStatement());
                    result.AddAfter(resultIdentityLocalDef, ruleAmbiguityIdentity.GetDeclarationStatement());
                    /* ToDo: Write code here to handle ambiguous states. */
                    result.AddAfter(decisionPoint, new CommentStatement(result,
@"There are ambiguities within this grammar.
-
Due to this, the parser's state will be necessary in identifying the specific ambiguous identity intended by a given token.  Other states, while ambiguous, appear unambiguously within the grammar."));
                    break;
                case RegularLanguageAmbiguityState.AmbiguousButDeterministic:
                    result.AddAfter(resultIdentityLocalDef, lexerAmbiguityIdentity.GetDeclarationStatement());
                    result.AddAfter(resultIdentityLocalDef, ruleAmbiguityIdentity.GetDeclarationStatement());
                    result.AddAfter(decisionPoint, new CommentStatement(result,
@"You'll notice some states have multiple identities on them, these have been analyzed and it appears as though none of them appear ambiguously within the result grammar.
-
Due to this, the parser's state will be necessary in identifying the specific identity intended by a given token."));
                    break;
                case RegularLanguageAmbiguityState.Unambiguous:
                    
                    result.AddAfter(decisionPoint, new CommentStatement(result,
@"This grammar is wholly unambiguous on the lexical front."));
                    break;
                default:
                    break;
            }
            if (!ambiguityHit && multipleTargetEdges)
            {
            }
            else if (ambiguityHit)
            {
            }
            else
            {
            }
            return (IIntermediateClassMethodMember)result;
        }

        public GrammarVocabularyModelBuilder GrammarVocabularyModel { get { return this.Compiler.LexicalSymbolModel; } }

        private Tuple<ISwitchCaseBlockStatement, IEnumerable<RegularLanguageDFAState>> GetGroupDFAState(IEnumerable<RegularLanguageDFAState> series, ISwitchStatement owningSwitch)
        {
            series = series.ToArray();
            var stateValues = series.Select(k=>k.StateValue.ToPrimitive()).ToArray();
            return new Tuple<ISwitchCaseBlockStatement, IEnumerable<RegularLanguageDFAState>>(owningSwitch.Case(stateValues), series);
        }

        private IEnumerable<RegularLanguageDFAStateJumpData> GetTargetStateData(IEnumerable<RegularLanguageDFAState> statesNeedingLabels, IBlockStatementParent owner, string labelPattern, params object[] furtherReplacements)
        {
            return (from stateNeedingLabel in statesNeedingLabels
                    select new
                    RegularLanguageDFAStateJumpData
                    {
                        State = stateNeedingLabel,
                        Label = new Lazy<ILabelStatement>(() => owner.DefineLabel(string.Format(string.Format(labelPattern, this._statePattern), new object[1] { stateNeedingLabel.StateValue }.Concat(furtherReplacements).ToArray()))),
                        LocalizedContainer = new BlockStatementParentContainer(owner),
                    });
        }

        private void GenerateNextTokenState(Stack<Tuple<RegularLanguageDFAState, IBlockStatementParent, RegularLanguageDFAStateJumpData>> toInsert, IBlockStatementParent parentTarget, RegularLanguageDFAState dfaState, ILabelStatement decisionPoint, Func<IUnicodeTargetGraph, ILabelStatement> graphLabeler)
        {
            if (!this._visited.Add(dfaState))
                return;
            var sources = (from src in dfaState.Sources
                           group src by src.Item2).ToDictionary(k => k.Key, v => v.GetActualTokenSources().ToArray()).Where(kvp => kvp.Value.Length > 0).ToDictionary(k => k.Key, v => v.Value);
            foreach (var k in sources.Keys)
                parentTarget.Comment(string.Format("{0} - {1}", k, string.Join<object>(", ", sources[k].Select(j => j is IOilexerGrammarTokenEntry ? ((InlinedTokenEntry)(j)).ToLocationDetails(this.Compiler.Source) : ((IInlinedTokenItem)(j)).ToLocationDetails(this.Compiler.Source)).Distinct())));
            if (!(dfaState is RegularLanguageDFARootState))
                parentTarget.Assign(_ntCurrentState.GetReference(), dfaState.StateValue.ToPrimitive());
            if (dfaState.IsEdge)
            {
                parentTarget.Assign(_ntExitState.GetReference(), _ntCurrentState.GetReference());
                parentTarget.Assign(_ntExitLADepth.GetReference(), _ntCurrentLADepth.GetReference());
            }
            if (dfaState.OutTransitions.Count > 0)
                parentTarget.Assign(this._ntCurrentChar.GetReference(), this.Compiler.CharStreamBuilder.CharIndexer.GetReference(this._CharStreamImpl.GetReference(), this._ntCurrentLADepth.Increment(false)));
            else
                parentTarget.GoTo(decisionPoint);
            var dfaTransitionTable = dfaState.BuildUnicodeGraph();
            bool useGlobalUnicodeGraph =
                dfaTransitionTable.UnicodeGraph.Values
                    .All(k => this._multitargetLookup.ContainsKey(k.Target));
            bool needCharSwitch = dfaTransitionTable.Count > 0;
            bool needUnicodeCategorySwitch = dfaTransitionTable.UnicodeGraph.Count > 0;
            if (useGlobalUnicodeGraph)
                HandleGlobalUnicodeGraphInsertion(dfaTransitionTable, graphLabeler);
            if (dfaTransitionTable != null && (dfaTransitionTable.Count > 0 || (dfaTransitionTable.UnicodeGraph != null && dfaTransitionTable.UnicodeGraph.Count > 0)))
            {
                var dfaHandlingMechanisms = ParserCompilerExtensions.DetermineStateHandlingTypes(this._multitargetLookup, dfaTransitionTable);
                var statesNeedingLocalJumps =
                    dfaHandlingMechanisms.Where(kvp => kvp.Value == RegularLanguageDFAHandlingType.LocalJump).Select(s => s.Key);
                var localData = GetTargetStateData(statesNeedingLocalJumps, parentTarget, "SubState_{{1:{0}}}_{{0:{0}}}", dfaState.StateValue).ToDictionary(k => k.State, v => v);
                /* Assert our logic is spot on... */
                Debug.Assert(localData.Count == 0 || (localData.Count > 0 && (needCharSwitch || (needUnicodeCategorySwitch && !useGlobalUnicodeGraph))));
                IBlockStatementParent unicodeSwitchParent = parentTarget;
                if (needCharSwitch)
                {
                    IBlockStatementParent charIfParent = parentTarget;
                    bool checkNeedsIf = false,
                         checkNeedsSwitch = false;
                    var switchConditions = new Dictionary<RegularLanguageDFAState, RegularLanguageDFAStateCodeData>();
                    foreach (var check in dfaTransitionTable.Keys)
                    {
                        var target = dfaTransitionTable[check];
                        var ifable = check.GetInefficientSwitchCases().ToArray();
                        var switchable = check.GetRange().Except(ifable).ToArray();
                        if (!checkNeedsIf && ifable.Length > 0)
                            checkNeedsIf = true;
                        if (!checkNeedsSwitch && switchable.Length > 0)
                            checkNeedsSwitch = true;
                        switchConditions.Add(target, new RegularLanguageDFAStateCodeData() { PairsForSwitch = switchable, PairsForIf = ifable });
                    }
                    if (switchConditions.Count == 1 && checkNeedsSwitch)
                    {
                        checkNeedsSwitch = false;
                        checkNeedsIf = true;
                        var firstItem = switchConditions.Values.First();
                        firstItem.PairsForIf = firstItem.PairsForSwitch.Concat(firstItem.PairsForIf).ToArray();
                        firstItem.PairsForSwitch = new RegularLanguageSet.SwitchPair<char, RegularLanguageSet.RangeSet>[0];
                    }
                    if (checkNeedsSwitch)
                    {
                        var charSwitch = charIfParent.Switch(this._ntCurrentChar.GetReference());
                        ISwitchCaseBlockStatement defaultCase = null;
                        foreach (var target in switchConditions.Keys.Where(k => switchConditions[k].PairsForSwitch.Length > 0))
                        {
                            if (dfaHandlingMechanisms[target] == RegularLanguageDFAHandlingType.Unknown)
                                SwitchOrIfAssertionFailure();
                            var switchData = switchConditions[target].PairsForSwitch;
                            List<IExpression> switchExpressions = new List<IExpression>();
                            /* Very basic analysis was done on these to determine if they were 'switchable', or more efficiently handled through a switch. */
                            foreach (var switchPair in switchData)
                            {
                                switch (switchPair.Which)
                                {
                                    case RegularLanguageSet.SwitchPairElement.SingleCharacter:
                                        switchExpressions.Add(switchPair.A.Value.ToPrimitive());
                                        break;
                                    case RegularLanguageSet.SwitchPairElement.CharacterRange:
                                        for (char c = switchPair.B.Value.Start; c <= switchPair.B.Value.End; c++)
                                            switchExpressions.Add(c.ToPrimitive());
                                        break;
                                }
                            }
                            ISwitchCaseBlockStatement currentCase = charSwitch.Case(switchExpressions.ToArray());
                            HandleTargetJumpCondition(toInsert, this._multitargetLookup, decisionPoint, dfaHandlingMechanisms, localData, currentCase, target, graphLabeler);
                        }
                        defaultCase = charSwitch.Case(true);
                        if (checkNeedsIf)
                            charIfParent = defaultCase;
                        else
                            unicodeSwitchParent = defaultCase;
                    }
                    if (checkNeedsIf)
                    {
                        foreach (var target in switchConditions.Keys.Where(k => switchConditions[k].PairsForIf.Length > 0))
                        {
                            if (dfaHandlingMechanisms[target] == RegularLanguageDFAHandlingType.Unknown)
                                SwitchOrIfAssertionFailure();
                            var charIfData = switchConditions[target].PairsForIf;
                            var charIfBlock = charIfParent.If(BuildRegularLanguageSetIfCondition(charIfData));
                            charIfParent = charIfBlock;
                            HandleTargetJumpCondition(toInsert, this._multitargetLookup, decisionPoint, dfaHandlingMechanisms, localData, charIfParent, target, graphLabeler);
                            unicodeSwitchParent = charIfParent = charIfBlock.Next;
                        }
                    }
                }
                if (needUnicodeCategorySwitch)
                {
                    if (useGlobalUnicodeGraph)
                        unicodeSwitchParent.GoTo(graphLabeler(dfaTransitionTable.UnicodeGraph));
                    else
                        GenerateUnicodeGraph(unicodeSwitchParent, dfaTransitionTable.UnicodeGraph, localData, decisionPoint);
                }
                else
                    unicodeSwitchParent.GoTo(decisionPoint);
                foreach (var element in localData.Keys)
                {
                    var currentData = localData[element];
                    toInsert.Push(Tuple.Create(element, (IBlockStatementParent)currentData.LocalizedContainer, currentData));
                }
                this._secondaryLookups.Add(parentTarget, localData);
            }
        }

        private static void HandleStatementInjections(IBlockStatementParent parentTarget, Dictionary<RegularLanguageDFAState, RegularLanguageDFAStateJumpData> lookupData)
        {
            foreach (var localItem in lookupData.Values)
            {
                var container = localItem.LocalizedContainer;
                HandleStatementInjections(parentTarget, container, localItem.Label.Value);
            }
        }

        private static void HandleStatementInjections(IBlockStatementParent parentTarget, BlockStatementParentContainer container, IStatement lastStatement)
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

        private ILabelStatement GenerateUnicodeGraph(IBlockStatementParent target, IUnicodeTargetGraph graph, Dictionary<RegularLanguageDFAState, RegularLanguageDFAStateJumpData> jumpData, ILabelStatement decisionPoint, string pattern = null, int patternCount = -1, bool defineLabel = false)
        {
            ILabelStatement result = null;
            if (defineLabel)
                result = target.DefineLabel(string.Format(pattern, ++patternCount));
            IConditionBlockStatement charRangeCheck = target.If(this._ntCurrentChar.GetReference().InequalTo((-1).ToPrimitive()));
            target = charRangeCheck;
            charRangeCheck.Next.GoTo(decisionPoint);
            if (graph.Keys.Count == 1 && graph[graph.Keys[0]].Values.All(k => !(k is IUnicodeTargetPartialCategory)) && graph[graph.Keys[0]].Values.Select(k => k.TargetedCategory).Distinct().OrderBy(tc => tc).SequenceEqual(ParserCompilerExtensions.unicodeCategoryData.Keys.OrderBy(uc => uc)))
            {
                var targetState = graph.Keys[0];
                ILabelStatement targetStateLabel = jumpData.ContainsKey(targetState) ? jumpData[targetState].Label.Value : this._multitargetLookup[targetState].Label.Value;
                target.Comment("The remaining characters not captured, from the states that target this graph, covered the entire unicode spectrum.");
                target.GoTo(targetStateLabel);
                return result;
            }
            var unicodeSwitch =
                target.Switch(_charGetUnicodeCategoryExpr.Invoke(this._ntCurrentChar.GetReference().Cast(_initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Char))));

            foreach (var targetState in graph.Keys)
            {
                var currentGraphTarget = graph[targetState];
                List<IUnicodeTargetCategory> fullCategories = new List<IUnicodeTargetCategory>();
                List<IUnicodeTargetPartialCategory> partialCategories = new List<IUnicodeTargetPartialCategory>();
                foreach (var category in currentGraphTarget.Keys)
                    if (currentGraphTarget[category] is IUnicodeTargetPartialCategory)
                        partialCategories.Add(((IUnicodeTargetPartialCategory)(currentGraphTarget[category])));
                    else
                        fullCategories.Add(currentGraphTarget[category]);
                ILabelStatement targetStateLabel = jumpData.ContainsKey(targetState) ? jumpData[targetState].Label.Value : this._multitargetLookup[targetState].Label.Value;
                var fullCategory = unicodeSwitch.Case();
                fullCategory.IsDefault = false;

                foreach (var category in fullCategories)
                    fullCategory.Cases.Add(typeof(UnicodeCategory).GetTypeExpression((ICliManager)this._initialAssembly.IdentityManager).GetField(category.TargetedCategory.ToString()));
                fullCategory.GoTo(targetStateLabel);

                foreach (var category in partialCategories)
                {
                    IExpression finalExpression = ObtainNegativeAssertion(category.NegativeAssertion);
                    var currentCategory = unicodeSwitch.Case(typeof(UnicodeCategory).GetTypeExpression((ICliManager)this._initialAssembly.IdentityManager).GetField(category.TargetedCategory.ToString()));
                    var currentCondition = currentCategory.If(finalExpression);
                    currentCondition.GoTo(targetStateLabel);
                    currentCategory.GoTo(decisionPoint);
                }
            }
            unicodeSwitch.Case(true).GoTo(decisionPoint);
            return result;
        }

        private IExpression ObtainNegativeAssertion(RegularLanguageSet regularSet)
        {
            /* *
             * Constructs a negative assertion simply as: [^0-9a-z] would yield:
             * ('0' > nextChar || nextchar > '9') && ('a' > this._ntCurrentChar || this._ntCurrentChar < 'z')
             * */
            IExpression finalExpression = null;
            foreach (var rangeElement in regularSet.GetRange())
            {
                IExpression currentExpression = null;
                switch (rangeElement.Which)
                {
                    case RegularLanguageSet.SwitchPairElement.SingleCharacter:
                        var value = rangeElement.A.Value;
                        //this._ntCurrentChar != 'value'
                        currentExpression = this._ntCurrentChar.InequalTo(value);
                        break;
                    case RegularLanguageSet.SwitchPairElement.CharacterRange:
                        var start = rangeElement.B.Value.Start;
                        var end = rangeElement.B.Value.End;
                        /* *
                         * 'start' > this._ntCurrentChar || this._ntCurrentChar > 'end'
                         * */
                        currentExpression = (start.GreaterThan(this._ntCurrentChar)).LogicalOr(this._ntCurrentChar.GreaterThan(end));
                        break;
                    default:
                        break;
                }
                if (finalExpression == null)
                    finalExpression = currentExpression;
                else
                    finalExpression = finalExpression.LogicalAnd(rangeElement.Which == RegularLanguageSet.SwitchPairElement.CharacterRange ? currentExpression.LeftNewLine() : currentExpression);
            }
            return finalExpression;
        }

        private void HandleGlobalUnicodeGraphInsertion(RegularLanguageDFAUnicodeGraph dfaTransitionTable, Func<IUnicodeTargetGraph, ILabelStatement> graphLabeler)
        {
            /* *
             * The unicode graph used targets only multi-target transitions, so it's safe to jump to a point
             * later in the method to identify the next jump.
             * *
             * Rationale: if the same unicode graph is used ten times, it's silly to repeat it over and over.
             * In areas where the jump table only targets local states, it's safe to inline the switch because
             * the targets will all be within the current block.
             * */
            var collectiveGraph = _collectiveUnicodeGraph.Find(dfaTransitionTable.UnicodeGraph, true);
            if (collectiveGraph == null)
                _collectiveUnicodeGraph.Add(collectiveGraph = dfaTransitionTable.UnicodeGraph);
            dfaTransitionTable.UnicodeGraph = collectiveGraph;
        }

        private void HandleTargetJumpCondition(Stack<Tuple<RegularLanguageDFAState, IBlockStatementParent, RegularLanguageDFAStateJumpData>> toInsert, Dictionary<RegularLanguageDFAState, RegularLanguageDFAStateJumpData> multitargetLookup, ILabelStatement decisionPoint, Dictionary<RegularLanguageDFAState, RegularLanguageDFAHandlingType> dfaHandlingMechanisms, Dictionary<RegularLanguageDFAState, RegularLanguageDFAStateJumpData> localData, IBlockStatementParent targetOfInsertion, RegularLanguageDFAState target, Func<IUnicodeTargetGraph, ILabelStatement> graphLabeler)
        {
            switch (dfaHandlingMechanisms[target])
            {
                case RegularLanguageDFAHandlingType.Inline:
                    toInsert.Push(Tuple.Create(target, targetOfInsertion, (RegularLanguageDFAStateJumpData)null));
                    break;
                case RegularLanguageDFAHandlingType.LocalJump:
                    var currentLocalData = localData[target];
                    targetOfInsertion.GoTo(currentLocalData.Label.Value);
                    break;
                case RegularLanguageDFAHandlingType.GlobalJump:
                    var currentGlobalData = multitargetLookup[target];
                    targetOfInsertion.GoTo(currentGlobalData.Label.Value);
                    break;
                default:
                    SwitchOrIfAssertionFailure();
                    break;
            }
        }

        private static void SwitchOrIfAssertionFailure()
        {
            Debug.Assert(false, "Assertion failure, deterministic automata state entered into which exists in an indeterminate object state.");
        }

        private IExpression BuildRegularLanguageSetIfCondition(IEnumerable<RegularLanguageSet.SwitchPair<char, RegularLanguageSet.RangeSet>> relevantElements)
        {
            IExpression finalExpression = null;
            foreach (var rangeElement in relevantElements)
            {
                IExpression currentExpression = null;
                switch (rangeElement.Which)
                {
                    case RegularLanguageSet.SwitchPairElement.SingleCharacter:
                        currentExpression = _ntCurrentChar.EqualTo(rangeElement.A.Value);
                        break;
                    case RegularLanguageSet.SwitchPairElement.CharacterRange:
                        var start = rangeElement.B.Value.Start;
                        var end = rangeElement.B.Value.End;
                        /* *
                         * 'start' <= @char && @char <= 'end'
                         * */
                        currentExpression = start.LessThanOrEqualTo(_ntCurrentChar).LogicalAnd(_ntCurrentChar.LessThanOrEqualTo(end));
                        break;
                    default:
                        break;
                }
                if (finalExpression == null)
                    finalExpression = currentExpression;
                else
                    finalExpression = finalExpression.LogicalOr(rangeElement.Which == RegularLanguageSet.SwitchPairElement.CharacterRange ? currentExpression.LeftNewLine() : currentExpression);
            }
            return finalExpression;
        }


        private void CreateInitializeMethod(IIntermediateClassMethodMember disposeMethod)
        {
            var initializeMethod = this.lexerClass.Methods.Add(new TypedName("Initialize", _initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.VoidType)), new TypedNameSeries(new TypedName("stream", typeof(Stream).ObtainCILibraryType<IClassType>(this._initialAssembly.IdentityManager))));
            initializeMethod.Call(disposeMethod.GetReference().Invoke());
            initializeMethod.SummaryText = string.Format("Initializes the @s:{0}; with the @p:stream; provided.", this.LexerClass.Name);
            var initStream = initializeMethod.Parameters[TypeSystemIdentifiers.GetMemberIdentifier("stream")];
            initStream.SummaryText = "The @s:Stream; from which to create the internal @s:TextStream; from.";
            this.InitializeMethod = initializeMethod;

            //initializeMethod.Assign(textStreamField.GetReference(), typeof(StreamReader).ObtainCILibraryType<IClassType>(this._initialAssembly.IdentityManager).GetNewExpression(initStream.GetReference()));
        }

        private IIntermediateClassMethodMember CreateDisposeMethod(IIntermediateClassFieldMember eosReached)
        {
            var disposeMethod = this.LexerClass.Methods.Add(new TypedName("Dispose", this._initialAssembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.VoidType)));
            disposeMethod.Assign(eosReached.GetReference(), IntermediateGateway.FalseValue);
            disposeMethod.AccessLevel = AccessLevelModifiers.Public;
            this.DisposeMethodImpl = disposeMethod;
            return disposeMethod;
        }

        private void SetClassMembersAccessLevel()
        {
            this.TokenStreamImpl.AccessLevel =
                this.PositionImpl.AccessLevel =
                this.EndOfStreamReachedImpl.AccessLevel =
                this.CurrentLineImpl.AccessLevel =
                this.CurrentColumnImpl.AccessLevel =
                this.DisposeMethodImpl.AccessLevel =
                this.CharStreamImpl.AccessLevel =
                this.InitializeMethod.AccessLevel =
                this.NextTokenImpl.AccessLevel = 
                    AccessLevelModifiers.Public;
        }

        /// <summary>
        /// Returns the <see cref="IIntermediateClassPropertyMember"/> which
        /// describes the initial definition of the token stream for the lexer
        /// to expose to the implementing <see cref="ParserImpl"/>.
        /// </summary>
        public IIntermediateClassPropertyMember TokenStreamImpl { get; private set; }
        /// <summary>
        /// Returns the <see cref="IIntermediateClassPropertyMember"/> which
        /// describes whether or not the end of the stream has been reached.
        /// </summary>
        /// <remarks>
        /// This is determined by the results from an internal
        /// character buffer read yielding a value less than expected on
        /// available characters.
        /// </remarks>
        public IIntermediateClassPropertyMember EndOfStreamReachedImpl { get; private set; }
        /// <summary>
        /// Returns the <see cref="IIntermediateClassPropertyMember"/> which 
        /// describes the <see cref="UInt64"/> value that notes where within
        /// the <see cref="TextStreamImpl"/> the <see cref="LexerClass"/> is.
        /// </summary>
        /// <remarks>Zero based.</remarks>
        public IIntermediateClassPropertyMember PositionImpl { get; private set; }
        /// <summary>
        /// Returns a <see cref="IIntermediateClassPropertyMember"/> instance
        /// which describes the <see cref="Int32"/> value which denotes 
        /// the current line within the <see cref="TextStreamImpl"/>.
        /// </summary>
        /// <remarks>One based.</remarks>
        public IIntermediateClassPropertyMember CurrentLineImpl { get; private set; }
        /// <summary>
        /// Returns a <see cref="IIntermediateClassPropertyMember"/> instance
        /// which describes the <see cref="Int32"/> value which denotes
        /// the current column of the <see cref="CurrentLineImpl"/>.
        /// </summary>
        /// <remarks>One based.</remarks>
        public IIntermediateClassPropertyMember CurrentColumnImpl { get; private set; }
        /// <summary>
        /// Returns the <see cref="IIntermediateClassMethodMember"/> which denotes
        /// the method used to dispose the resources used by the 
        /// <see cref="LexerClass"/>.
        /// </summary>
        public IIntermediateClassMethodMember DisposeMethodImpl { get; private set; }
        /// <summary>
        /// Returns the <see cref="IIntermediateClassMethodMember"/> which yields the next token in the stream.
        /// </summary>
        public IIntermediateClassMethodMember NextTokenImpl { get; private set; }
        public IIntermediateInterfaceMethodMember NextToken{ get; private set; }
        #endregion


        public IIntermediateInterfacePropertyMember CharStream { get; set; }

        public IIntermediateClassPropertyMember CharStreamImpl { get; set; }

        public IIntermediateClassFieldMember _CharStreamImpl { get; set; }

        public IIntermediateClassMethodMember InitializeMethod { get; set; }

        public TokenStreamBuilder TokenStreamBuilder { get { return this._tokenStreamBuilder; } }

        public IIntermediateClassCtorMember LexerCtor { get; set; }
    }
    public class RegularLanguageDFAStateJumpData
    {
        public RegularLanguageDFAState State { get; set; }
        public Lazy<ILabelStatement> Label { get; set; }
        public BlockStatementParentContainer LocalizedContainer { get; set; }
    }

    public enum RegularLanguageDFAHandlingType
    {
        Unknown,
        Inline,
        LocalJump,
        GlobalJump,
    }

    public enum RegularLanguageAmbiguityState
    {
        Ambiguous,
        AmbiguousButDeterministic,
        Unambiguous
    }

    internal class RegularLanguageDFAStateCodeData
    {
        //public IExpression[] SwitchExpressions { get; set; }
        public RegularLanguageSet.SwitchPair<char, RegularLanguageSet.RangeSet>[] PairsForSwitch { get; set; }
        public RegularLanguageSet.SwitchPair<char, RegularLanguageSet.RangeSet>[] PairsForIf { get; set; }
    }
}
