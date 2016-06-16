using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf.Cli;
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
using AllenCopeland.Abstraction.Slf.Ast.Expressions.Linq;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Provides a base implementation of a <see cref="IConstructBuilder{TInput, TOutput}"/>
    /// which yields a given language's parser.
    /// </summary>
    public partial class ParserBuilder :
        IConstructBuilder<Tuple<ParserCompiler, TokenSymbolBuilder, IIntermediateAssembly, RegularLanguageDFAState>, Tuple<IIntermediateInterfaceType, IIntermediateClassType, LexerBuilder>>
    {
        /// <summary>
        /// Data member for <see cref="Compiler"/>.
        /// </summary>
        private ParserCompiler compiler;
        /// <summary>
        /// Data member for <see cref="LexerBuilder"/>.
        /// </summary>
        private LexerBuilder _lexerBuilder;
        /// <summary>
        /// Data member for <see cref="ParserInterface"/>
        /// </summary>
        private IIntermediateInterfaceType _parserInterface;
        /// <summary>
        /// Data member for <see cref="ParserClass"/>.
        /// </summary>
        private IIntermediateClassType _parserClass;
        /// <summary>
        /// Data member maintaining a reference to the result assembly
        /// on which the <see cref="ParserClass"/> and 
        /// <see cref="ParserInterface"/> are constructed.
        /// </summary>
        private IIntermediateAssembly _assembly;
        private SymbolStreamBuilder _symbolStreamBuilder;
        private GenericSymbolStreamBuilder _genericSymbolStreamBuilder;
        private TokenSymbolBuilder _tokenSymbolBuilder;
        private RegularLanguageDFAState _lexerCore;
        private IIntermediateClassFieldMember _parserState;
        private IIntermediateClassPropertyMember _parserStatePropImpl;
        private IIntermediateInterfacePropertyMember _parserStateProp;
        private IIntermediateCliManager _identityManager;

        /// <summary>
        /// Creates a new <see cref="ParserBuilder"/>.
        /// </summary>
        public ParserBuilder()
        {
        }

        /// <summary>
        /// Handles construction of the elements yielded by the
        /// <see cref="ParserBuilder"/> with the <paramref name="input"/>
        /// provided.
        /// </summary>
        /// <param name="input">The <see cref="Tuple{T1, T2, T3}"/> which provides the
        /// <see cref="ParserCompiler"/>, <see cref="TokenSymbolBuilder"/> and 
        /// <see cref="IIntermediateAssembly"/> necessary to perform the operation.</param>
        /// <returns>A <see cref="Tuple{T1, T2, T3}"/> which yields the
        /// <see cref="IIntermediateInterfaceType"/>, <see cref="IIntermediateClassType"/> 
        /// for the result parser of the language, and 
        /// <see cref="LexerBuilder"/> of the language which denotes the class
        /// and interface of the tokenizer.</returns>
        public Tuple<IIntermediateInterfaceType, IIntermediateClassType, LexerBuilder> Build(Tuple<ParserCompiler, TokenSymbolBuilder, IIntermediateAssembly, RegularLanguageDFAState> input)
        {
            this.compiler = input.Item1;
            this._tokenSymbolBuilder = input.Item2;
            this._assembly = input.Item3;
            this._identityManager = (IIntermediateCliManager)this._assembly.IdentityManager;
            this._parserInterface = _assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}", compiler.Source.Options.ParserName);
            this._parserClass = _assembly.DefaultNamespace.Parts.Add().Classes.Add(compiler.Source.Options.ParserName);
            this._parserClass.Assembly.ScopeCoercions.Add("System.Linq");

            this._genericSymbolStreamBuilder = new GenericSymbolStreamBuilder();
            this._symbolStreamBuilder = new SymbolStreamBuilder();
            this._lexerBuilder = new LexerBuilder();
            this._parserClass.AccessLevel = AccessLevelModifiers.Internal;
            this._parserInterface.AccessLevel = AccessLevelModifiers.Public;
            this._lexerCore = input.Item4;
            this._parserState = this._parserClass.Fields.Add(new TypedName("_state", _assembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.Int32)));
            this._parserState.AccessLevel = AccessLevelModifiers.Private;
            this._parserStateProp = this._parserInterface.Properties.Add(new TypedName("State", this._parserState.FieldType), true, false);
            this._parserStatePropImpl = this._parserClass.Properties.Add(new TypedName("State", this._parserState.FieldType), true, true);
            this._parserStatePropImpl.AccessLevel = AccessLevelModifiers.Public;
            this._parserStatePropImpl.SetMethod.AccessLevel = AccessLevelModifiers.Private;
            this._parserStatePropImpl.GetMethod.Return(this._parserState.GetReference());
            this._parserStatePropImpl.SetMethod.Assign(this._parserState.GetReference(), this._parserStatePropImpl.SetMethod.ValueParameter.GetReference());
            this._parserClass.ImplementedInterfaces.ImplementInterfaceQuick(this._parserInterface);
            return Tuple.Create(this.ParserInterface, this.ParserClass, this._lexerBuilder);
        }

        public IIntermediateClassPropertyMember StateImpl { get { return this._parserStatePropImpl; } }
        public IIntermediateClassFieldMember _StateImpl { get { return this._parserState; } }
        public IIntermediateClassFieldMember StateImplField { get { return this._parserState; } }
        public IIntermediateInterfacePropertyMember State { get { return this._parserStateProp; } }

        public void Build2()
        {
            /* ACC - Update May 24, 2015: Moved Lexer build into a separate method due to pruning of unused ambiguities being a necessity prior to Lexer build. */
            this._genericSymbolStreamBuilder.Build(Tuple.Create(this.compiler, this._tokenSymbolBuilder.CommonSymbolBuilder, this._assembly));
            this._symbolStreamBuilder.Build(Tuple.Create(this.compiler, this._tokenSymbolBuilder.CommonSymbolBuilder, this._assembly, this._genericSymbolStreamBuilder));
            this._lexerBuilder.Build(Tuple.Create(compiler, this, _assembly, this._tokenSymbolBuilder, true, this._genericSymbolStreamBuilder, this._lexerCore));
            this.BuildSymbolStreamImpl();
            this.BuildTokenStreamImpl();
            this.BuildTokenizerImpl();
            this.Create_CurrentContextImpl();
            this.BuildConstructor();
            this.BuildCurrentIfNegative();
            this.BuildGetMasterContext();
            this.BuildSpinErrorContext();
            this.BuildFollowState();
            this.BuildReset();
        }

        private void BuildExpect()
        {
            this.ExpectImpl = this._parserClass.Methods.Add(
                new TypedName("Expect", RuntimeCoreType.VoidType, this._identityManager),
                new TypedNameSeries(
                    new TypedName("lookAheadDepth", RuntimeCoreType.Int32, this._identityManager),
                    new TypedName("erroneousContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol),
                    new TypedName("fromError", RuntimeCoreType.Boolean, this._identityManager)));
            var lookAheadDepth = this.ExpectImpl.Parameters["lookAheadDepth"];
            var erroneousContext = this.ExpectImpl.Parameters["erroneousContext"];
            var fromError = this.ExpectImpl.Parameters["fromError"];

            var tokenIndex = this.ExpectImpl.Locals.Add(new TypedName("tokenIndex", RuntimeCoreType.Int32, this._identityManager), this.SymbolStreamBuilder.GetTokenIndexImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke(lookAheadDepth.GetReference()));
            var position = this.ExpectImpl.Locals.Add(new TypedName("position", RuntimeCoreType.Int32, this._identityManager));
            var tiCheck = this.ExpectImpl.If(tokenIndex.LessThan(this.Compiler.GenericSymbolStreamBuilder.CountImpl.GetReference(this.TokenStreamImpl.GetReference())));
            tiCheck.CreateNext(this.Compiler.GenericSymbolStreamBuilder.CountImpl.GetReference(this.TokenStreamImpl.GetReference()).EqualTo(IntermediateGateway.NumberZero));
            tiCheck.Assign(position, this.compiler.TokenSymbolBuilder.StartPosition.GetReference(this.compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this.TokenStreamImpl.GetReference(), tokenIndex.GetReference())));
            tiCheck.Next.Assign(position, IntermediateGateway.NumberZero);
            IConditionBlockStatement tiCheckNext = (IConditionBlockStatement)tiCheck.Next;
            tiCheckNext.CreateNext();
            tiCheckNext.Next.Assign(position, this.compiler.TokenSymbolBuilder.StartPosition.GetReference(this.compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this.TokenStreamImpl.GetReference(), this.Compiler.GenericSymbolStreamBuilder.CountImpl.GetReference(this.TokenStreamImpl.GetReference()).Subtract(1))));
            var errorContext = this.ExpectImpl.Locals.Add(new TypedName("errorContext", this.compiler.ErrorContextBuilder.LanguageErrorContext), this.compiler.ErrorContextBuilder.LanguageErrorContext.GetNewExpression(this.compiler.ErrorContextBuilder.CauseExpected.GetReference(), this.StateImpl.GetReference(), this.Compiler.ExtensionsBuilder.GetValidSyntaxMethodImpl.GetReference().Invoke(), position.GetReference()));
            errorContext.AutoDeclare = false;
            this.ExpectImpl.DefineLocal(errorContext);
            var listTypeGeneric = typeof(List<int>).GetTypeReference<IClassType>(this._identityManager);
            var errorIndicesList = this.ExpectImpl.Locals.Add(listTypeGeneric.WithName("errorIndices"), listTypeGeneric.GetNewExpression());

            errorIndicesList.AutoDeclare = false;
            this.ExpectImpl.DefineLocal(errorIndicesList);
            this.ExpectImpl.Call(errorIndicesList.GetReference().GetMethod("Add").Invoke(this.Compiler.ErrorContextBuilder.IndexImpl.GetReference(errorContext.GetReference()).Assign(this._ErrorContextImpl.GetReference().GetProperty("Count"))));
            this.ExpectImpl.Call(this._ErrorContextImpl.GetReference().GetMethod("Add").Invoke(errorContext.GetReference()));
            this.ExpectImpl.Call(this.Compiler.RuleSymbolBuilder.DelineateCaptureImpl.GetReference(erroneousContext.GetReference()).Invoke(ParserCompiler.ErrorCaptureName.ToPrimitive(), errorIndicesList.GetReference()));
            this.Compiler.RuleSymbolBuilder.DelineateCapture.GetReference(erroneousContext.GetReference()).Invoke(ParserCompiler.ErrorCaptureName.ToPrimitive(), errorIndicesList.GetReference());
            this.ExpectImpl.AccessLevel = AccessLevelModifiers.Public;
        }

        private void BuildFollowState()
        {
            var followStateImpl = this._parserClass.Fields.Add(new TypedName("followState", RuntimeCoreType.Int32, this._identityManager));
            this._FollowStateImpl = followStateImpl;
            this._FollowStateImpl.AccessLevel = AccessLevelModifiers.Private;
        }

        public void Build3()
        {
            this.FinishResetMethod();
        }

        public void Build4()
        {
            this.BuildExpect();
            this.BuildHandleErrorContext();
        }

        private void BuildHandleErrorContext()
        {
            this.HandleErrorContextImpl = 
                this.ParserClass.Methods.Add(
                    new TypedName("HandleErrorContext", RuntimeCoreType.VoidType, this._identityManager),
                    new TypedNameSeries(
                        this.Compiler.ParseResultsBuilder.ClassDetail.Type.MakeGenericClosure("TResult".GetSymbolType()).WithName("resultsOfParse"),
                        this.Compiler.RuleSymbolBuilder.LanguageRuleSymbol.WithName("resultParseTree")),
                    new GenericParameterData(
                        "TResult",
                        new [] { this.compiler.RootRuleBuilder.ILanguageRule }) { SpecialConstraint = GenericTypeParameterSpecialConstraint.Class });
            this.HandleErrorContextImpl.AccessLevel = AccessLevelModifiers.Internal;
            var resultsOfParse = this.HandleErrorContextImpl.Parameters["resultsOfParse"];
            var resultParseTree = this.HandleErrorContextImpl.Parameters["resultParseTree"];
            var ifParseTreeHasErrorCheck = this.HandleErrorContextImpl.If(this._ErrorContextImpl.GetReference().GetProperty("Count").GreaterThan(0).LogicalAnd(this.Compiler.RuleSymbolBuilder.HasErrorImpl.GetReference(resultParseTree.GetReference())));
            var listGenericTypeOfInt = typeof(List<int>).GetTypeReference<IClassType>(this._identityManager);
            var listType = listGenericTypeOfInt.ElementType;
            var errors = ifParseTreeHasErrorCheck.Locals.Add(listGenericTypeOfInt.WithName("errorIndices"), this.Compiler.RuleSymbolBuilder.GetExplicitCaptureImpl.GetReference(resultParseTree.GetReference(), listGenericTypeOfInt).Invoke(ParserCompiler.ErrorCaptureName.ToPrimitive()));
            var errorsNonNullCheck = ifParseTreeHasErrorCheck.If(errors.InequalTo(IntermediateGateway.NullValue));
            var relevantTopLevels = errorsNonNullCheck.Locals.Add(this.Compiler.ErrorContextBuilder.ILanguageErrorContext.MakeArray().WithName("relevantTopLevels"),
                new ParenthesizedExpression(LinqHelper.From("errorIndex", errors.GetReference())
                                            .Select(this._ErrorContextImpl.GetReference().GetIndexer("errorIndex".GetSymbolExpression()))
                                            .Build())
                .GetMethod("ToArray").Invoke());
            var errorSymbol = "error".GetSymbolExpression();
            var topLevel = "topLevel".GetSymbolExpression();
            var errorSet = "errorSet".GetSymbolExpression();
            var errorDetail = 
                errorsNonNullCheck.Locals.Add(
                    listType.MakeGenericClosure(this.Compiler.ErrorContextBuilder.ILanguageErrorContext).WithName("errorDetail"),
                    new ParenthesizedExpression(
                        LinqHelper.From(errorSymbol.Symbol, this._ErrorContextImpl.GetReference().GetMethod("OfType", this.Compiler.ErrorContextBuilder.LanguageErrorContext).Invoke())
                                  .OrderBy(errorSymbol.GetProperty("Index"))
                                  .GroupBy(errorSymbol, errorSymbol.GetProperty("Location")).Into("errorSet")
                                  .Let(errorSymbol.Symbol, errorSet.GetMethod("FirstOrDefault").Invoke())
                                  .Join(topLevel.Symbol, relevantTopLevels.GetReference(), errorSymbol.GetProperty("Location"), topLevel.GetProperty("Location"))
                                  .Select(errorSymbol)
                                  .Build())
                                  .GetMethod("Distinct").Invoke()
                                  .GetMethod("Cast", this.Compiler.ErrorContextBuilder.ILanguageErrorContext).Invoke()
                                  .GetMethod("ToList").Invoke());
            errorsNonNullCheck.Call(this.Compiler.RuleSymbolBuilder.ClearErrorContextImpl.GetReference(resultParseTree.GetReference()).Invoke());
            errorsNonNullCheck.Assign(this.Compiler.ParseResultsBuilder.ClassDetail.SyntaxErrors.GetReference(resultsOfParse.GetReference()), errorDetail);
        }

        private void FinishResetMethod()
        {
            this.ResetMethodImpl.Call(this.compiler.SymbolStreamBuilder.ResetMethodImpl.GetReference(this._SymbolStreamImpl.GetReference()).Invoke());
        }

        private void BuildReset()
        {
            var resetMethod = this._parserClass.Methods.Add(new TypedName("Reset", RuntimeCoreType.VoidType, this._identityManager));
            resetMethod.Call(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference(this._TokenStreamImpl.GetReference()).GetMethod("Clear").Invoke());
            resetMethod.AccessLevel = AccessLevelModifiers.Public;
            this.ResetMethodImpl = resetMethod;
        }

        private void BuildConstructor()
        {
            var ctor = this.ParserClass.Constructors.Add(new TypedName("stream", this._identityManager.ObtainTypeReference(typeof(Stream))));
            var streamParam = ctor.Parameters["stream"];
            ctor.Assign(this._TokenizerImpl.GetReference(), this.LexerBuilder.LexerClass.GetNewExpression(new SpecialReferenceExpression(SpecialReferenceKind.This)));
            ctor.Call(this.LexerBuilder.InitializeMethod.GetReference(this._TokenizerImpl.GetReference()).Invoke(streamParam.GetReference()));
            ctor.Assign(this._SymbolStreamImpl.GetReference(), this.Compiler.SymbolStreamBuilder.ResultClass.GetNewExpression(this._TokenStreamImpl.GetReference().Assign(this.Compiler.TokenStreamBuilder.ResultClass.GetNewExpression(this._TokenizerImpl.GetReference()))));
            ctor.AccessLevel = AccessLevelModifiers.Public;
        }

        private void BuildSymbolStreamImpl()
        {
            var symbolStreamImpl = this.ParserClass.Fields.Add(new TypedName("_symbolStream", this.Compiler.SymbolStreamBuilder.ResultClass));
            symbolStreamImpl.AccessLevel = AccessLevelModifiers.Private;
            this._SymbolStreamImpl = symbolStreamImpl;
        }

        private void BuildTokenStreamImpl()
        {
            var tokenStream = this.ParserClass.Fields.Add(new TypedName("_tokenStream", this.Compiler.TokenStreamBuilder.ResultClass));
            var tokenStreamImpl = this.ParserClass.Properties.Add(new TypedName("TokenStream", this.compiler.TokenStreamBuilder.ResultInterface), true, false);
            tokenStreamImpl.AccessLevel = AccessLevelModifiers.Public;
            tokenStreamImpl.GetMethod.Return(tokenStream.GetReference());
            tokenStream.AccessLevel = AccessLevelModifiers.Private;
            this._TokenStreamImpl = tokenStream;
            this.TokenStreamImpl = tokenStreamImpl;
        }

        private void BuildTokenizerImpl()
        {
            var tokenizer = this.ParserClass.Fields.Add(new TypedName("_tokenizer", this.LexerBuilder.LexerClass));
            tokenizer.AccessLevel = AccessLevelModifiers.Private;
            this._TokenizerImpl = tokenizer;
        }

        private void Create_CurrentContextImpl()
        {
            var _currentContextImpl = this._parserClass.Fields.Add(new TypedName("_currentContext", this.Compiler.RuleSymbolBuilder.ILanguageRuleSymbol));
            _currentContextImpl.AccessLevel = AccessLevelModifiers.Private;
            this._CurrentContextImpl = _currentContextImpl;
        }

        private void BuildGetMasterContext()
        {
            var getMasterContextImpl = this.ParserClass.Methods.Add(new TypedName("GetMasterContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol));
            var currentContextLocal = getMasterContextImpl.Locals.Add(new TypedName("currentContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol), this._CurrentContextImpl.GetReference());
            var whileLoop = getMasterContextImpl.While(currentContextLocal.InequalTo(IntermediateGateway.NullValue));
            whileLoop.If(this.compiler.RuleSymbolBuilder.ParentImpl.GetReference(currentContextLocal.GetReference()).InequalTo(IntermediateGateway.NullValue))
                .Assign(currentContextLocal.GetReference(), this.compiler.RuleSymbolBuilder.ParentImpl.GetReference(currentContextLocal.GetReference()));
            getMasterContextImpl.Return(currentContextLocal.GetReference());
            this.GetMasterContextImpl = getMasterContextImpl;
            GetMasterContextImpl.AccessLevel = AccessLevelModifiers.Private;
        }

        private void BuildCurrentIfNegative()
        {
            var currentIfOtherNegative = this.ParserClass.Methods.Add(new TypedName("CurrentIfOtherNegative", RuntimeCoreType.Int32, this._identityManager),
                new TypedNameSeries(
                    new TypedName("current", RuntimeCoreType.Int32, this._identityManager), 
                    new TypedName("other", RuntimeCoreType.Int32, this._identityManager)));
            var current = currentIfOtherNegative.Parameters["current"];
            var other = currentIfOtherNegative.Parameters["other"];
            currentIfOtherNegative.If(other.GetReference().LessThan(IntermediateGateway.NumberZero))
                .Return(current.GetReference());
            currentIfOtherNegative.Return(other.GetReference());
            this.CurrentIfOtherNegative = currentIfOtherNegative;
        }

        private void BuildSpinErrorContext()
        {
            var errorContextImpl = this.ParserClass.Fields.Add(new TypedName("currentErrorContext", ((IClassType)(this._identityManager.ObtainTypeReference(typeof(List<>)))).MakeGenericClosure(this.compiler.ErrorContextBuilder.ILanguageErrorContext)));
            var spinErrorContext = this.ParserClass.Methods.Add(new TypedName("SpinUpErrorContext", RuntimeCoreType.VoidType, this._identityManager));
            spinErrorContext.Assign(errorContextImpl.GetReference(), errorContextImpl.FieldType.GetNewExpression());
            spinErrorContext.AccessLevel = AccessLevelModifiers.Private;
            this.SpinErrorContextImpl = spinErrorContext;
            this._ErrorContextImpl = errorContextImpl;
        }

        /// <summary>
        /// Returns the <see cref="ParserCompiler"/> instance from which the
        /// <see cref="ParserBuilder"/> was derived.
        /// </summary>
        public ParserCompiler Compiler { get { return this.compiler; } }

        /// <summary>
        /// Returns the <see cref="IIntermediateInterfaceType"/> which denotes the the parser interface of the
        /// result language.
        /// </summary>
        public IIntermediateInterfaceType ParserInterface { get { return this._parserInterface; } }

        /// <summary>
        /// Returns the <see cref="IIntermediateClassType"/> which denotes the parser class of the result language.
        /// </summary>
        public IIntermediateClassType ParserClass { get { return this._parserClass; } }

        /// <summary>
        /// Returns the <see cref="LexerBuilder"/> class which is responsible for building the resultant lexer
        /// interface and class for the current language
        /// </summary>
        public LexerBuilder LexerBuilder { get { return this._lexerBuilder; } }

        /// <summary>
        /// Returns the <see cref="IControlledDictionary{TKey, TValue}"/> which denotes the series of
        /// <see cref="IIntermediateClassMethodMember"/> instances which provide static LL(*) predictions for a
        /// given state within a language.
        /// </summary>
        public IControlledDictionary<PredictionTreeDFAdapter, IIntermediateClassMethodMember> PredictionMethods { get; private set; }

        /// <summary>
        /// Returns the <see cref="IControlledDictionary{TKey, TValue}"/> which denotes the series of
        /// <see cref="IIntermediateClassMethodMember"/> instances which provide the standard parse method
        /// implementations of a given production rule of a language relative to the internal class of the parser.
        /// </summary>
        public IControlledDictionary<ProductionRuleNormalAdapter, IIntermediateClassMethodMember> ParseMethods { get; private set; }

        /// <summary>
        /// Returns the <see cref="IControlledDictionary{TKey, TValue}"/> which denotes the series of
        /// <see cref="IIntermediateInterfaceMethodMember"/> instances which provide the standard parse method of
        /// a given production of a rule of a language relative to the public interface of the parser.
        /// </summary>
        public IControlledDictionary<ProductionRuleNormalAdapter, IIntermediateInterfaceMethodMember> InterfaceParseMethods { get; private set; }

        public SymbolStreamBuilder SymbolStreamBuilder { get { return this._symbolStreamBuilder; } }
        public GenericSymbolStreamBuilder GenericSymbolStreamBuilder { get { return this._genericSymbolStreamBuilder; } }

        private Dictionary<ProductionRuleNormalAdapter, IIntermediateClassMethodMember> parseMethods = new Dictionary<ProductionRuleNormalAdapter, IIntermediateClassMethodMember>();

        private Dictionary<ProductionRuleNormalAdapter, IIntermediateClassMethodMember> parseInternalMethods = new Dictionary<ProductionRuleNormalAdapter, IIntermediateClassMethodMember>();
        private Dictionary<ProductionRuleNormalAdapter, IIntermediateClassMethodMember> parseInternalOriginalMethods = new Dictionary<ProductionRuleNormalAdapter, IIntermediateClassMethodMember>();

        private Dictionary<PredictionTreeDFAdapter, IIntermediateClassMethodMember> predictMethods = new Dictionary<PredictionTreeDFAdapter, IIntermediateClassMethodMember>();

        private Dictionary<PredictionTreeDFAdapter, IIntermediateClassMethodMember> followPredictMethods = new Dictionary<PredictionTreeDFAdapter, IIntermediateClassMethodMember>();
        private Dictionary<PredictionTreeLeaf, IIntermediateClassMethodMember> followDiscriminatorMethods = new Dictionary<PredictionTreeLeaf, IIntermediateClassMethodMember>();

        internal IIntermediateClassMethodMember GetProjectionPredictMethod(PredictionTreeDFAdapter adapter)
        {
            return this.predictMethods[adapter];
        }

        internal IIntermediateClassMethodMember GetEntryParseMethod(IOilexerGrammarProductionRuleEntry entry)
        {
            IIntermediateClassMethodMember result;
            var adapter = this.Compiler.RuleAdapters[entry];
            this.parseMethods.TryGetValue(adapter, out result);
            return result;
        }
        internal IIntermediateClassMethodMember GetEntryInternalParseMethod(IOilexerGrammarProductionRuleEntry entry)
        {
            IIntermediateClassMethodMember result;
            var adapter = this.Compiler.RuleAdapters[entry];
            this.parseInternalMethods.TryGetValue(adapter, out result);
            return result;
        }
        private static IEnumerable<Tuple<IGrammarSymbol, IIntermediateEnumFieldMember>> GetVocabularyIdentities(GrammarVocabulary vocabulary, ParserCompiler compiler)
        {
            var symbols = vocabulary.GetSymbols();
            var tokens = symbols.Where(k => k is IGrammarTokenSymbol && (!(((IGrammarTokenSymbol)k).Source is IOilexerGrammarTokenEofEntry)));
            var rules = symbols.Where(k => k is IGrammarRuleSymbol);
            foreach (var token in tokens)
                yield return Tuple.Create(token, compiler.LexicalSymbolModel.GetIdentitySymbolField(token));
            foreach (var rule in rules)
            {
                yield return Tuple.Create(rule, compiler.SyntacticalSymbolModel.GetIdentitySymbolField(rule));
            }
        }

        public IIntermediateClassFieldMember _CurrentContextImpl { get; set; }

        public IIntermediateClassFieldMember _TokenizerImpl { get; set; }

        public IIntermediateClassFieldMember _TokenStreamImpl { get; set; }

        public IIntermediateClassFieldMember _SymbolStreamImpl { get; set; }

        public IIntermediateClassMethodMember ResetMethodImpl { get; set; }


        public IIntermediateClassMethodMember GetMasterContextImpl { get; set; }

        public IIntermediateClassMethodMember SpinErrorContextImpl { get; set; }

        public IIntermediateClassFieldMember _ErrorContextImpl { get; set; }

        public IIntermediateClassMethodMember CurrentIfOtherNegative { get; set; }

        public IIntermediateClassFieldMember _FollowStateImpl { get; set; }

        public IIntermediateClassMethodMember ExpectImpl { get; set; }

        public IIntermediateClassPropertyMember TokenStreamImpl { get; set; }

        public IIntermediateClassMethodMember HandleErrorContextImpl { get; set; }
    }

    public enum SyntacticalDFAStateEnclosureHandling
    {
        Undecided,
        Encapsulate,
        EncapsulatePeek,
        Handled,
    }
    public class SyntacticalDFAStateJumpData
    {
        public SyntacticalDFAState                  State              { get; set; }
        public Lazy<ILabelStatement>                Label              { get; set; }
        public BlockStatementParentContainer        BlockContainer     { get; set; }
        public SyntacticalDFAStateEnclosureHandling EnclosureType      { get; set; }
        private static IEnumerable<SyntacticalDFAStateJumpData> ObtainStateJumpDetailsInternal(
            IEnumerable<SyntacticalDFAState>         statesNeedingLabels, 
            IBlockStatementParent                    owner, 
            string                                   labelPattern, 
            string                                   statePattern, 
            Func<SyntacticalDFAState, bool>          isMultiOverride,
            IEnumerable<SyntacticalDFAState>         singularStates, 
            params object[]                          furtherReplacements)
        {
            if (singularStates != null && isMultiOverride != null)
                foreach (var state in singularStates)
                    if (isMultiOverride(state))
                        yield return new SyntacticalDFAStateJumpData()
                        {
                            State = state,
                            Label = new Lazy<ILabelStatement>(() =>
                                owner.DefineLabel(string.Format(string.Format(labelPattern, statePattern), ((object)state.StateValue).AsEnumerable().Concat(furtherReplacements).ToArray()))),
                            BlockContainer = new BlockStatementParentContainer(owner)
                        };
            foreach (var state in statesNeedingLabels.OrderBy(k => k.StateValue))
            {
                yield return new SyntacticalDFAStateJumpData()
                    {
                        State = state,
                        Label = new Lazy<ILabelStatement>(()=>
                            owner.DefineLabel(string.Format(string.Format(labelPattern, statePattern), ((object)state.StateValue).AsEnumerable().Concat(furtherReplacements).ToArray()))),
                        BlockContainer = new BlockStatementParentContainer(owner)
                    };
            }
        }

        public static Dictionary<SyntacticalDFAState, SyntacticalDFAStateJumpData> ObtainStateJumpDetails(
            IEnumerable<SyntacticalDFAState>         statesNeedingLabels, 
            IBlockStatementParent                    owner, 
            string                                   labelPattern, 
            string                                   statePattern, 
            Func<SyntacticalDFAState, bool>          isMultiOverride,
            IEnumerable<SyntacticalDFAState>         singularStates, 
            params object[]                          furtherReplacements)
        {
            return ObtainStateJumpDetailsInternal(statesNeedingLabels, owner, labelPattern, statePattern, isMultiOverride, singularStates, furtherReplacements)
                .OrderBy(k=>k.State.StateValue)
                .ToDictionary(k => k.State, v => v);
        }
    }
}
