using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.CSharp;
using AllenCopeland.Abstraction.Utilities.Arrays;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Slf.Ast.Expressions.Linq;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Ast.Statements;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    using RuleTree = KeyedTree<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using RuleTreeNode = KeyedTreeNode<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using System.Collections;
    using System.Diagnostics;
    using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
    using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
    public class RuleSymbolBuilder :
        IConstructBuilder<Tuple<ParserCompiler, StackContextBuilder, IIntermediateAssembly>, IIntermediateInterfaceType>
    {
        private ParserCompiler _compiler;
        private IIntermediateAssembly _assembly;
        private IIntermediateInterfaceType _resultInterface;
        private IIntermediateInterfacePropertyMember _endTokenIndex;
        private IIntermediateInterfaceMethodMember _getExplicitCapture;
        private IIntermediateInterfacePropertyMember _previous;
        private IIntermediateInterfacePropertyMember _next;
        private IIntermediateInterfacePropertyMember _parent;
        private IIntermediateInterfacePropertyMember _firstChild;
        private IIntermediateInterfacePropertyMember _children;
        private StackContextBuilder _stackContext;
        private IIntermediateCliManager _identityManager;
        private IIntermediateClassType _resultClass;
        private IIntermediateClassPropertyMember _endTokenIndexImpl;
        private IIntermediateClassMethodMember _getExplicitCaptureImpl;
        private IIntermediateClassPropertyMember _previousImpl;
        private IIntermediateClassPropertyMember _nextImpl;
        private IIntermediateClassPropertyMember _firstChildImpl;
        private IIntermediateClassPropertyMember _childrenImpl;
        private IIntermediateClassPropertyMember _parentImpl;
        private IIntermediateClassFieldMember __parentImpl;
        private IIntermediateClassFieldMember _bodyContextImpl;
        private IIntermediateClassFieldMember _captureContextImpl;
        private IIntermediateClassFieldMember __endTokenIndexImpl;
        private IIntermediateClassPropertyMember _startTokenIndexImpl;
        private IIntermediateClassMethodMember _getNextOfTImpl;
        private IIntermediateClassMethodMember _getPreviousOfTImpl;
        private IIntermediateClassFieldMember __indexImpl;
        private IIntermediateClassFieldMember __startTokenIndexImpl;
        private IIntermediateClassFieldMember _RuleImpl;
        private IIntermediateInterfaceMethodMember _delineateCapture;
        private IIntermediateClassMethodMember _delineateCaptureImpl;
        public IIntermediateInterfaceType Build(Tuple<ParserCompiler, StackContextBuilder, IIntermediateAssembly> input)
        {
            this._compiler = input.Item1;
            this._stackContext = input.Item2;
            this._assembly = input.Item3;
            this._identityManager = ((IIntermediateCliManager)(this._assembly.IdentityManager));
            this.BuildInterface();
            this.BuildClass();
            return this._resultInterface;
        }

        private void BuildClass()
        {
            this._resultClass = _assembly.DefaultNamespace.Parts.Add().Classes.Add("{0}RuleContext", this._compiler.Source.Options.AssemblyName);
            this._bodyContextImpl = this.BuildBodyContextImpl();
            this.__indexImpl = this.Build_IndexImpl();
            this._captureContextImpl = this.BuildCaptureContextImpl();
            this.__parentImpl = this.Build_ParentImpl();
            this._parentImpl = this.BuildParentImpl();
            this._getNextOfTImpl = this.BuildNextOfTImpl();
            this._getPreviousOfTImpl = this.BuildPreviousOfTImpl();
            this._endTokenIndexImpl = this.BuildEndTokenIndexImpl();
            this._startTokenIndexImpl = this.BuildStartTokenIndex();
            this._getExplicitCaptureImpl = this.BuildGetExplicitCaptureImpl();
            this._previousImpl = this.BuildPreviousImpl();
            this._nextImpl = this.BuildNextImpl();
            this._firstChildImpl = this.BuildFirstChildImpl();
            this._childrenImpl = this.BuildChildrenImpl();
            this._resultClass.ImplementedInterfaces.ImplementInterfaceQuick(this._resultInterface);
        }

        private void BuildRuleField()
        {
            this._RuleImpl = this._resultClass.Fields.Add(new TypedName("_rule", this._compiler.RootRuleBuilder.ILanguageRule));
        }

        private void BuildRuleCreatedImpl()
        {
            var ruleCreatedImpl = this._resultClass.Properties.Add(new TypedName("RuleCreated", RuntimeCoreType.Boolean, this._identityManager), true, false);
            ruleCreatedImpl.GetMethod.Return(this._RuleImpl.InequalTo(IntermediateGateway.NullValue));
            ruleCreatedImpl.AccessLevel = AccessLevelModifiers.Public;
            this.RuleCreatedImpl = RuleCreated;
        }

        private IIntermediateClassFieldMember Build_ParentImpl()
        {
            var _parentImpl = this._resultClass.Fields.Add(new TypedName("_parent", this._resultInterface));
            _parentImpl.AccessLevel = AccessLevelModifiers.Private;
            return _parentImpl;
        }

        private IIntermediateClassFieldMember Build_IndexImpl()
        {
            var _indexImpl = this._resultClass.Fields.Add(new TypedName("_index", RuntimeCoreType.Int32, this._identityManager));
            _indexImpl.AccessLevel = AccessLevelModifiers.Private;
            return _indexImpl;
        }
        private IIntermediateInterfaceMethodMember BuildDoReduction()
        {
            var doReductionImpl = this._resultInterface.Methods.Add(
                new TypedName("DoReduction", RuntimeCoreType.VoidType, this._identityManager),
                new TypedNameSeries(
                    new TypedName("symbolStream", this._compiler.SymbolStreamBuilder.ResultInterface),
                    new TypedName("symbolStart", RuntimeCoreType.Int32, this._identityManager),
                    new TypedName("symbolCount", RuntimeCoreType.Int32, this._identityManager)));
            return doReductionImpl;
        }

        private IIntermediateClassMethodMember BuildDoReductionImpl()
        {
            var doReductionImpl = this._resultClass.Methods.Add(
                new TypedName("DoReduction", RuntimeCoreType.VoidType, this._identityManager), 
                new TypedNameSeries(
                    new TypedName("symbolStream", this._compiler.SymbolStreamBuilder.ResultInterface), 
                    new TypedName("symbolStart", RuntimeCoreType.Int32, this._identityManager),
                    new TypedName("symbolCount", RuntimeCoreType.Int32, this._identityManager)));

            var symbolStreamParam = doReductionImpl.Parameters["symbolStream"];
            var symbolStart = doReductionImpl.Parameters["symbolStart"];
            var symbolCount = doReductionImpl.Parameters["symbolCount"];
            doReductionImpl.Assign(this._bodyContextImpl.GetReference(), this._compiler.SymbolStreamBuilder
                .ReduceMethod.GetReference(
                    symbolStreamParam.GetReference())
                    .Invoke(symbolStart.GetReference(), symbolCount.GetReference(), new SpecialReferenceExpression(SpecialReferenceKind.This)));
            doReductionImpl.If(symbolCount.GetReference().LessThanOrEqualTo(IntermediateGateway.NumberZero))
                .Return();

            var first = doReductionImpl.Locals.Add(new TypedName("first", this._compiler.CommonSymbolBuilder.ILanguageSymbol), _bodyContextImpl.GetReference().GetIndexer(IntermediateGateway.NumberZero));
            var last = doReductionImpl.Locals.Add(new TypedName("last", this._compiler.CommonSymbolBuilder.ILanguageSymbol), _bodyContextImpl.GetReference().GetIndexer(symbolCount.Subtract(1)));
            first.AutoDeclare = false;
            last.AutoDeclare = false;
            doReductionImpl.DefineLocal(first);
            doReductionImpl.DefineLocal(last);
            var firstCheck = doReductionImpl.If(first.GetReference().Is(this._compiler.TokenSymbolBuilder.ILanguageToken));
            firstCheck.Assign(this.__startTokenIndexImpl.GetReference(), this._compiler.CommonSymbolBuilder.StartTokenIndex.GetReference(first.GetReference().Cast(this._compiler.TokenSymbolBuilder.ILanguageToken)));
            firstCheck.CreateNext(first.GetReference().Is(this._resultInterface));
            firstCheck.Next.Assign(this.__startTokenIndexImpl.GetReference(), this._compiler.CommonSymbolBuilder.StartTokenIndex.GetReference(first.GetReference().Cast(this.ILanguageRuleSymbol)));


            var lastCheck = doReductionImpl.If(last.GetReference().Is(this._compiler.TokenSymbolBuilder.ILanguageToken));
            lastCheck.Assign(this.__endTokenIndexImpl.GetReference(), this._compiler.CommonSymbolBuilder.StartTokenIndex.GetReference(last.GetReference().Cast(this._compiler.TokenSymbolBuilder.ILanguageToken)));
            lastCheck.CreateNext(last.GetReference().Is(this._resultInterface));
            lastCheck.Next.Assign(this.__endTokenIndexImpl.GetReference(), this.EndTokenIndex.GetReference(last.GetReference().Cast(this.ILanguageRuleSymbol)));

            doReductionImpl.Comment("Set each of their indices, so get first/last handling can be done.");
            var symbolIndex = doReductionImpl.Locals.Add(new TypedName("symbolIndex", RuntimeCoreType.Int32, this._identityManager), IntermediateGateway.NumberZero);
            symbolIndex.AutoDeclare = false;
            var forLoop = doReductionImpl.Iterate(symbolIndex.GetDeclarationStatement(), symbolIndex.LessThan(symbolCount), symbolIndex.Increment().AsEnumerable());
            var currentSymbol = forLoop.Locals.Add(new TypedName("currentSymbol", this._compiler.CommonSymbolBuilder.ILanguageSymbol), _bodyContextImpl.GetReference().GetIndexer(symbolIndex.GetReference()));
            currentSymbol.AutoDeclare = false;
            forLoop.DefineLocal(currentSymbol);
            var forRuleCheck = forLoop.If(currentSymbol.GetReference().Is(this.LanguageRuleSymbol));
            var ruleSymbol = forRuleCheck.Locals.Add(new TypedName("ruleSymbol", LanguageRuleSymbol), currentSymbol.GetReference().As(LanguageRuleSymbol));

            ruleSymbol.AutoDeclare = false;
            forRuleCheck.DefineLocal(ruleSymbol);
            forRuleCheck.Assign(this.__indexImpl.GetReference(ruleSymbol.GetReference()), symbolIndex.GetReference());
            forRuleCheck.Assign(this.ParentImpl.GetReference(ruleSymbol.GetReference()), new SpecialReferenceExpression(SpecialReferenceKind.This));
            forRuleCheck.If(this.HasErrorImpl.GetReference().Not().LogicalAnd(this.HasError.GetReference(ruleSymbol.GetReference())))
                .Assign(this.HasErrorImpl.GetReference(),IntermediateGateway.TrueValue);
            doReductionImpl.AccessLevel = AccessLevelModifiers.Public;
            return doReductionImpl;
        }

        private IIntermediateClassMethodMember BuildPreviousOfTImpl()
        {
            string symbolTParamName = "TSymbol";
            var getPreviousOfTImpl = this._resultClass.Methods.Add(new TypedName("GetPrevious", symbolTParamName), new TypedNameSeries(new TypedName("parent", this._resultInterface), new TypedName("startIndex", RuntimeCoreType.Int32, this._identityManager)), new GenericParameterData(symbolTParamName, new IType[1] { this._compiler.CommonSymbolBuilder.ILanguageSymbol }));
            getPreviousOfTImpl.AccessLevel = AccessLevelModifiers.Private;
            var startIndexParam = getPreviousOfTImpl.Parameters["startIndex"];
            var symbolTParam = getPreviousOfTImpl.TypeParameters[symbolTParamName];
            var parentParam = getPreviousOfTImpl.Parameters["parent"];
            getPreviousOfTImpl.IsStatic = true;
            var symbolIndexLocal = getPreviousOfTImpl.Locals.Add(new TypedName("symbolIndex", RuntimeCoreType.Int32, this._identityManager), startIndexParam.GetReference());
            symbolIndexLocal.AutoDeclare = false;
            var iteration = getPreviousOfTImpl.Iterate(symbolIndexLocal.GetDeclarationStatement(), symbolIndexLocal.GreaterThanOrEqualTo(0), symbolIndexLocal.Decrement().AsEnumerable());
            var currentSymbolLocal = iteration.Locals.Add(new TypedName("currentSymbol", this._compiler.CommonSymbolBuilder.ILanguageSymbol), parentParam.GetReference().GetIndexer(symbolIndexLocal.GetReference()));
            currentSymbolLocal.AutoDeclare = false;
            iteration.DefineLocal(currentSymbolLocal);
            iteration.If(currentSymbolLocal.GetReference().Is(symbolTParam))
                .Return(currentSymbolLocal.GetReference().Cast(symbolTParam));
            getPreviousOfTImpl.Return(new DefaultValueExpression(symbolTParam));
            return getPreviousOfTImpl;
        }

        private IIntermediateClassMethodMember BuildNextOfTImpl()
        {
            string symbolTParamName = "TSymbol";
            var getNextOfTImpl = this._resultClass.Methods.Add(new TypedName("GetNext", symbolTParamName), new TypedNameSeries(new TypedName("parent", this._resultInterface), new TypedName("startIndex", RuntimeCoreType.Int32, this._identityManager)), new GenericParameterData(symbolTParamName, new IType[1] { this._compiler.CommonSymbolBuilder.ILanguageSymbol }));
            getNextOfTImpl.AccessLevel = AccessLevelModifiers.Private;
            var symbolTParam = getNextOfTImpl.TypeParameters[symbolTParamName];
            var parentParam = getNextOfTImpl.Parameters["parent"];
            var startIndexParam = getNextOfTImpl.Parameters["startIndex"];
            getNextOfTImpl.IsStatic = true;
            var symbolIndexLocal = getNextOfTImpl.Locals.Add(new TypedName("symbolIndex", RuntimeCoreType.Int32, this._identityManager), startIndexParam.GetReference());
            symbolIndexLocal.AutoDeclare = false;
            var iteration = getNextOfTImpl.Iterate(symbolIndexLocal.GetDeclarationStatement(), symbolIndexLocal.LessThan(parentParam.GetReference().GetProperty("Count")), symbolIndexLocal.Increment().AsEnumerable());
            var currentSymbolLocal = iteration.Locals.Add(new TypedName("currentSymbol", this._compiler.CommonSymbolBuilder.ILanguageSymbol), parentParam.GetReference().GetIndexer(symbolIndexLocal.GetReference()));
            currentSymbolLocal.AutoDeclare = false;
            iteration.DefineLocal(currentSymbolLocal);
            iteration.If(currentSymbolLocal.GetReference().Is(symbolTParam))
                .Return(currentSymbolLocal.GetReference().Cast(symbolTParam));
            getNextOfTImpl.Return(new DefaultValueExpression(symbolTParam));
            return getNextOfTImpl;
        }

        private IIntermediateClassFieldMember BuildCaptureContextImpl()
        {
            var captureContext = this._resultClass.Fields.Add(new TypedName("_captureContext", ((IClassType)(this._identityManager.ObtainTypeReference(typeof(Dictionary<,>)))).MakeGenericClosure(this._identityManager.ObtainTypeReference(RuntimeCoreType.String), this._identityManager.ObtainTypeReference(RuntimeCoreType.RootType))));
            captureContext.AccessLevel = AccessLevelModifiers.Private;
            return captureContext;
        }

        private IIntermediateClassFieldMember BuildBodyContextImpl()
        {
            var bodyContext = this._resultClass.Fields.Add(new TypedName("_bodyContext", ((IInterfaceType)(_identityManager.ObtainTypeReference(typeof(IReadOnlyList<>)))).MakeGenericClosure(this._compiler.CommonSymbolBuilder.ILanguageSymbol)));
            bodyContext.AccessLevel = AccessLevelModifiers.Private;
            return bodyContext;
        }

        private IIntermediateClassPropertyMember BuildStartTokenIndex()
        {
            this.__startTokenIndexImpl = this._resultClass.Fields.Add(new TypedName("_startTokenIndex", RuntimeCoreType.Int32, this._identityManager));
            __startTokenIndexImpl.AccessLevel = AccessLevelModifiers.Private;
            var startTokenIndexImpl = this._resultClass.Properties.Add(new TypedName("StartTokenIndex", RuntimeCoreType.Int32, this._identityManager), true, false);
            startTokenIndexImpl.AccessLevel = AccessLevelModifiers.Public;
            startTokenIndexImpl.GetMethod.Return(this.__startTokenIndexImpl.GetReference());
            return startTokenIndexImpl;
        }

        private IIntermediateClassPropertyMember BuildEndTokenIndexImpl()
        {
            this.__endTokenIndexImpl = this._resultClass.Fields.Add(new TypedName("_endTokenIndex", RuntimeCoreType.Int32, this._identityManager));
            __endTokenIndexImpl.AccessLevel = AccessLevelModifiers.Private;
            var endTokenIndexImpl = this._resultClass.Properties.Add(new TypedName("EndTokenIndex", RuntimeCoreType.Int32, this._identityManager), true, false);
            endTokenIndexImpl.AccessLevel = AccessLevelModifiers.Public;
            endTokenIndexImpl.GetMethod.Return(this.__endTokenIndexImpl.GetReference());
            return endTokenIndexImpl;
        }

        private IIntermediateClassMethodMember BuildGetExplicitCaptureImpl()
        {
            const string captureTParamName = "TCapture";
            var getExplicitCaptureImpl = this._resultClass.Methods.Add(new TypedName("GetExplicitCapture", captureTParamName), new TypedNameSeries(new TypedName("name", RuntimeCoreType.String, this._identityManager)), new GenericParameterData(captureTParamName));
            getExplicitCaptureImpl.AccessLevel = AccessLevelModifiers.Public;
            getExplicitCaptureImpl.SummaryText = this._getExplicitCapture.SummaryText;

            var captureTParam = getExplicitCaptureImpl.TypeParameters[captureTParamName];
            captureTParam.SummaryText = this._getExplicitCapture.TypeParameters[captureTParam.Name].SummaryText;
            var nameParam = getExplicitCaptureImpl.Parameters["name"];
            nameParam.SummaryText = this._getExplicitCapture.Parameters[nameParam.Name].SummaryText;
            var captureResult = getExplicitCaptureImpl.Locals.Add(new TypedName("currentCapture", RuntimeCoreType.RootType, this._identityManager));
            getExplicitCaptureImpl.If(_captureContextImpl.GetReference().GetMethod("TryGetValue").Invoke(nameParam.GetReference(), captureResult.GetReference().Direct(ParameterCoercionDirection.Out)).Not())
                .Return(new DefaultValueExpression(captureTParam));
            getExplicitCaptureImpl.If(captureResult.GetReference().Is(captureTParam))
                .Return(captureResult.GetReference().Cast(captureTParam));
            getExplicitCaptureImpl.Return(new DefaultValueExpression(captureTParam));
            return getExplicitCaptureImpl;
        }

        private IIntermediateClassPropertyMember BuildPreviousImpl()
        {
            var previousImpl = this._resultClass.Properties.Add(new TypedName("Previous", this._resultInterface), true, false);
            previousImpl.AccessLevel = AccessLevelModifiers.Public;
            previousImpl.GetMethod.Return(_getPreviousOfTImpl.GetReference(null, this._resultInterface).Invoke(this.__parentImpl.GetReference(), this.__indexImpl.Subtract(1)));
            return previousImpl;
        }

        private IIntermediateClassPropertyMember BuildNextImpl()
        {
            var nextImpl = this._resultClass.Properties.Add(new TypedName("Next", this._resultInterface), true, false);
            nextImpl.AccessLevel = AccessLevelModifiers.Public;
            nextImpl.GetMethod.Return(_getNextOfTImpl.GetReference(null, this._resultInterface).Invoke(this.__parentImpl.GetReference(), this.__indexImpl.Add(1)));
            return nextImpl;
        }

        private IIntermediateClassPropertyMember BuildParentImpl()
        {
            var parentImpl = this._resultClass.Properties.Add(new TypedName("Parent", this._resultInterface), true, true);
            parentImpl.AccessLevel = AccessLevelModifiers.Public;
            parentImpl.GetMethod.Return(this.__parentImpl.GetReference());
            parentImpl.SetMethod.Assign(this.__parentImpl.GetReference(), parentImpl.SetMethod.ValueParameter.GetReference());
            parentImpl.SetMethod.AccessLevel = AccessLevelModifiers.Internal;
            return parentImpl;
        }

        private IIntermediateClassPropertyMember BuildFirstChildImpl()
        {
            var nextImpl = this._resultClass.Properties.Add(new TypedName("FirstChild", this._resultInterface), true, false);
            nextImpl.AccessLevel = AccessLevelModifiers.Public;
            nextImpl.GetMethod.Return(_getNextOfTImpl.GetReference(null, this._resultInterface).Invoke(new SpecialReferenceExpression(SpecialReferenceKind.This), IntermediateGateway.NumberZero));
            return nextImpl;
        }

        private IIntermediateClassPropertyMember BuildChildrenImpl()
        {
            var childrenImpl = this._resultClass.Properties.Add(new TypedName("Children", ((IInterfaceType)this._identityManager.ObtainTypeReference(typeof(IEnumerable<>))).MakeGenericClosure(this._resultInterface)),true, false);
            var childSymbolSymbol = new SymbolExpression("childSymbol");
            childrenImpl.GetMethod
                .Return(LinqHelper
                    .From("childSymbol", this._bodyContextImpl.GetReference())
                    .Where(childSymbolSymbol.Is(this._resultInterface))
                    .Select(childSymbolSymbol.Cast(this._resultInterface)).Build());
            childrenImpl.AccessLevel = AccessLevelModifiers.Public;
            return childrenImpl;
        }

        private void BuildInterface()
        {
            this._resultInterface = _assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}RuleContext", this._compiler.Source.Options.AssemblyName);
            this.RuleCreated = _resultInterface.Properties.Add(new TypedName("RuleCreated", RuntimeCoreType.Boolean, this._identityManager), true, false);
            this._resultInterface.AccessLevel = AccessLevelModifiers.Public;
            this._endTokenIndex = this.BuildEndTokenIndex();
            this._getExplicitCapture = this.BuildGetExplicitCapture();
            this._previous = this.BuildPrevious();
            this._next = this.BuildNext();
            this._parent = this.BuildParent();
            this._firstChild = this.BuildFirstChild();
            this._children = this.BuildChildren();
            this._resultInterface.ImplementedInterfaces.Add(this._stackContext.ILanguageStackContext);
        }

        public void Build2()
        {
            this.BuildHasError();
            this.BuildHasErrorImpl();
            this.BuildRuleField();
            this.BuildRuleCreatedImpl();
            this.BuildIndexer();
            this.BuildGetFirstViableContext();
            this.BuildGetFirstViableParentContext();
            this.BuildCreateRule();
            this.BuildCount();
            this.BuildCountImpl();
            this.BuildCreateRuleImpl();
            this.DoReductionImpl = this.BuildDoReductionImpl();
            this.DoReduction = this.BuildDoReduction();
            this.CreateTokenCountImpl();
            this.Create_TokenStreamImpl();
            this.CreateLengthImpl();
            this.CreateGetEnumeratorImpl();
            this.SetScope();
            this.BuildIdentityImpl();
            this.BuildInitialState();
            this.BuildInitialStateImpl();
            this.BuildFollowState();
            this.BuildFollowStateImpl();
            this.BuildCtor();
            this.BuildDelineateCapture();
            this.BuildStartPosition();
            this.BuildStartPositionImpl();
            this.BuildEndPosition();
            this.BuildEndPositionImpl();
        }

        internal void Build3()
        {
            this.BuildToStringOverride();
        }

        private void BuildStartPosition()
        {
            var startPosition = this.ILanguageRuleSymbol.Properties.Add(new TypedName("StartPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
            this.StartPosition = startPosition;
        }

        private void BuildStartPositionImpl()
        {
            var startPosition = this.LanguageRuleSymbol.Properties.Add(new TypedName("StartPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
            startPosition.AccessLevel = AccessLevelModifiers.Public;
            startPosition.GetMethod.Return(this._compiler.TokenSymbolBuilder.StartPosition.GetReference(this._compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this._TokenStreamImpl.GetReference(), this.__startTokenIndexImpl.GetReference())));
            this.StartPositionImpl = startPosition;
        }

        private void BuildEndPosition()
        {
            var startPosition = this.ILanguageRuleSymbol.Properties.Add(new TypedName("EndPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
            this.EndPosition = startPosition;
        }

        private void BuildEndPositionImpl()
        {
            var startPosition = this.LanguageRuleSymbol.Properties.Add(new TypedName("EndPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
            startPosition.AccessLevel = AccessLevelModifiers.Public;
            startPosition.GetMethod.Return(this._compiler.TokenSymbolBuilder.EndPosition.GetReference(this._compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this._TokenStreamImpl.GetReference(), this.__endTokenIndexImpl.GetReference())));
            this.EndPositionImpl = startPosition;
        }

        private void BuildToStringOverride()
        {
            //return this._tokenStream.SCANNERNAME.CharStream[StartPosition, EndPosition];
            var toStringMethod = this.LanguageRuleSymbol.Methods.Add(new TypedName("ToString", RuntimeCoreType.String, this._identityManager));
            toStringMethod.IsOverride = true;
            toStringMethod.AccessLevel = AccessLevelModifiers.Public;
            toStringMethod.Return(this._compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this._compiler.LexerBuilder.CharStream.GetReference(this._compiler.TokenStreamBuilder.TokenizerImpl.GetReference(this._TokenStreamImpl.GetReference())), this.StartPositionImpl.GetReference(), this.EndPositionImpl.GetReference()));
        }

        private void BuildHasError()
        {
            var hasError = this._resultInterface.Properties.Add(new TypedName("HasError", RuntimeCoreType.Boolean, this._identityManager), true, true);
            this.HasError = hasError;
        }

        private void BuildHasErrorImpl()
        {
            var hasErrorImpl = this._resultClass.Properties.Add(new TypedName("HasError", RuntimeCoreType.Boolean, this._identityManager), true, true);
            hasErrorImpl.GetMethod.Return(this.GetExplicitCapture.GetReference(new SpecialReferenceExpression(SpecialReferenceKind.This), this._identityManager.ObtainTypeReference(RuntimeCoreType.Boolean)).Invoke("__HasError".ToPrimitive()));
            hasErrorImpl.SetMethod.Assign(this._captureContextImpl.GetReference().GetIndexer("__HasError".ToPrimitive()), IntermediateGateway.TrueValue);
            hasErrorImpl.AccessLevel = AccessLevelModifiers.Public;
            this.HasErrorImpl = hasErrorImpl;
        }

        private void BuildFollowState()
        {
            this.FollowState = this._resultInterface.Properties.Add(new TypedName("FollowState", RuntimeCoreType.Int32, this._identityManager), true, false);
        }

        private void BuildInitialState()
        {
            this.InitialState = this._resultInterface.Properties.Add(new TypedName("InitialState", RuntimeCoreType.Int32, this._identityManager), true, false);
        }

        private void BuildFollowStateImpl()
        {
            var _followStateImpl = this._resultClass.Fields.Add(new TypedName("_followState", RuntimeCoreType.Int32, this._identityManager));
            var followStateImpl = this._resultClass.Properties.Add(new TypedName("FollowState", RuntimeCoreType.Int32, this._identityManager), true,false);
            followStateImpl.AccessLevel = AccessLevelModifiers.Public;
            _followStateImpl.AccessLevel = AccessLevelModifiers.Private;
            this.FollowStateImpl = followStateImpl;
            this._FollowStateImpl = _followStateImpl;
            this.FollowStateImpl.GetMethod.Return(this._FollowStateImpl.GetReference());
        }

        private void BuildInitialStateImpl()
        {
            var _initialStateImpl   = this._resultClass.Fields.Add(new TypedName("_initialState", RuntimeCoreType.Int32, this._identityManager));
            var initialStateImpl    = this._resultClass.Properties.Add(new TypedName("InitialState", RuntimeCoreType.Int32, this._identityManager), true, false);
            initialStateImpl.AccessLevel = AccessLevelModifiers.Public;
            _initialStateImpl.AccessLevel = AccessLevelModifiers.Private;
            this.InitialStateImpl = initialStateImpl;
            this._InitialStateImpl = _initialStateImpl;
            this.InitialStateImpl.GetMethod.Return(this._InitialStateImpl.GetReference());
        }

        private void BuildDelineateCapture()
        {
            this._delineateCapture = this._resultInterface.Methods.Add(
                new TypedName("DelineateCapture", RuntimeCoreType.VoidType, this._identityManager),
                new TypedNameSeries(
                    new TypedName("name", RuntimeCoreType.String, this._identityManager),
                    new TypedName("capture", RuntimeCoreType.RootType, this._identityManager)));
            this._delineateCaptureImpl = this._resultClass.Methods.Add(
                new TypedName("DelineateCapture", RuntimeCoreType.VoidType, this._identityManager),
                new TypedNameSeries(
                    new TypedName("name", RuntimeCoreType.String, this._identityManager),
                    new TypedName("capture", RuntimeCoreType.RootType, this._identityManager)));

            this._delineateCaptureImpl.AccessLevel = AccessLevelModifiers.Public;
            this._delineateCaptureImpl.Call(this._captureContextImpl.GetReference().GetMethod("Add").Invoke(this._delineateCaptureImpl.Parameters["name"].GetReference(), this._delineateCaptureImpl.Parameters["capture"].GetReference()));
        }

        private void BuildIdentityImpl()
        {
            var _identityImpl = this._resultClass.Fields.Add(new TypedName("_identity", this._compiler.SymbolStoreBuilder.Identities));
            var identityImpl = this._resultClass.Properties.Add(new TypedName("Identity", this._compiler.SymbolStoreBuilder.Identities), true, false);
            identityImpl.GetMethod.Return(_identityImpl.GetReference());
            identityImpl.AccessLevel = AccessLevelModifiers.Public;
            _identityImpl.AccessLevel = AccessLevelModifiers.Private;
            this._IdentityImpl = _identityImpl;
            this.IdentityImpl = identityImpl;
        }

        private void BuildCtor()
        {
            //IJsonReaderTokenStream tokenStream, IJsonReaderRuleContext parent, JsonReaderSymbols identity
            var ctor = this._resultClass.Constructors.Add(
                new TypedName("tokenStream", this._compiler.TokenStreamBuilder.ResultInterface),
                new TypedName("parent", this._resultInterface),
                new TypedName("identity", this._compiler.SymbolStoreBuilder.Identities),
                new TypedName("initialState", RuntimeCoreType.Int32, this._identityManager),
                new TypedName("followState", RuntimeCoreType.Int32, this._identityManager));

            var tokenStream = ctor.Parameters["tokenStream"];
            var parent = ctor.Parameters["parent"];
            var identity = ctor.Parameters["identity"];
            var initialState = ctor.Parameters["initialState"];
            var followState = ctor.Parameters["followState"];

            ctor.Assign(this._TokenStreamImpl.GetReference(), tokenStream.GetReference());
            ctor.Assign(this.__parentImpl.GetReference(), parent.GetReference());
            ctor.Assign(this._IdentityImpl.GetReference(), identity.GetReference());
            ctor.Assign(_captureContextImpl.GetReference(), _captureContextImpl.FieldType.GetNewExpression());
            ctor.Assign(this._InitialStateImpl.GetReference(), initialState.GetReference());
            ctor.Assign(this._FollowStateImpl.GetReference(), followState.GetReference());

            ctor.AccessLevel = AccessLevelModifiers.Public;
        }

        private void SetScope()
        {
            this._resultClass.Assembly.ScopeCoercions.Add("System.Linq");
        }

        private void BuildCount()
        {
            var countImpl = this._resultInterface.Properties.Add(new TypedName("Count", RuntimeCoreType.Int32, this._identityManager), true, false);
            
            this.Count = countImpl;
        }

        private void BuildCountImpl()
        {
            var countImpl = this._resultClass.Properties.Add(new TypedName("Count", RuntimeCoreType.Int32, this._identityManager), true, false);
            countImpl.AccessLevel = AccessLevelModifiers.Public;
            countImpl.GetMethod.Return(this._bodyContextImpl.GetReference().GetProperty("Count"));
            this.CountImpl = countImpl;
        }
        private void Create_TokenStreamImpl()
        {
            var tokenStreamImpl = this._resultClass.Fields.Add(new TypedName("_tokenStream", this._compiler.TokenStreamBuilder.ResultInterface));
            tokenStreamImpl.AccessLevel = AccessLevelModifiers.Private;
            this._TokenStreamImpl = tokenStreamImpl;
        }

        private void CreateLengthImpl()
        {
            var lengthImpl = this._resultClass.Properties.Add(new TypedName("Length", RuntimeCoreType.Int32, this._identityManager), true, false);
            lengthImpl.AccessLevel = AccessLevelModifiers.Public;
            var firstTok = lengthImpl.GetMethod.Locals.Add(new TypedName("firstTok", this._compiler.TokenSymbolBuilder.ILanguageToken), this._compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this._TokenStreamImpl.GetReference(), this.__startTokenIndexImpl.GetReference()));
            var lastTok = lengthImpl.GetMethod.Locals.Add(new TypedName("lastTok", this._compiler.TokenSymbolBuilder.ILanguageToken), this._compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this._TokenStreamImpl.GetReference(), this.__endTokenIndexImpl.GetReference()));
            lengthImpl.GetMethod.Return(this._compiler.TokenSymbolBuilder.EndPosition.GetReference(lastTok.GetReference()).Subtract(this._compiler.TokenSymbolBuilder.StartPosition.GetReference(firstTok.GetReference())));
            this.LengthImpl = lengthImpl;
        }

        private void CreateTokenCountImpl()
        {
            var tokenCountImpl = this._resultClass.Properties.Add(new TypedName("TokenCount", RuntimeCoreType.Int32, this._identityManager), true, false);
            tokenCountImpl.GetMethod.Return(this.__endTokenIndexImpl.Subtract(this.__startTokenIndexImpl.Subtract(1)));
            tokenCountImpl.AccessLevel = AccessLevelModifiers.Public;
            this.TokenCountImpl = tokenCountImpl;
        }
        
        private void CreateGetEnumeratorImpl()
        {
            var genericGetEnumerator = this._resultClass.Methods.Add(new TypedName("GetEnumerator", ((IInterfaceType)this._identityManager.ObtainTypeReference(typeof(IEnumerator<>))).MakeGenericClosure(this._compiler.CommonSymbolBuilder.ILanguageSymbol)));
            var enumerable = genericGetEnumerator.Enumerate("symbol", _bodyContextImpl.GetReference());
            enumerable.YieldReturn(enumerable.Local.GetReference());
            var oldEnumerator = this._resultClass.Methods.Add(new TypedName("GetEnumerator2", ((IInterfaceType)(this._identityManager.ObtainTypeReference(typeof(IEnumerator))))));
            oldEnumerator.Implementations.Add(this._identityManager.ObtainTypeReference(typeof(IEnumerable)));
            oldEnumerator.LanguageSpecificQualifier = "IEnumerable";
            oldEnumerator.Name = "GetEnumerator";
            oldEnumerator.Return(genericGetEnumerator.GetReference().Invoke());
            genericGetEnumerator.AccessLevel = AccessLevelModifiers.Public;
            this.GetEnumeratorImpl = genericGetEnumerator;
        }

        private void BuildIndexer()
        {
            this.Indexer=this._resultInterface.Indexers.Add(this._compiler.CommonSymbolBuilder.ILanguageSymbol, new TypedNameSeries(new TypedName("index", RuntimeCoreType.Int32, this._identityManager)), true, false);
            var indexerImpl = this._resultClass.Indexers.Add(this._compiler.CommonSymbolBuilder.ILanguageSymbol, new TypedNameSeries(new TypedName("index", RuntimeCoreType.Int32, this._identityManager)), true, false);
            indexerImpl.GetMethod.Return(this._bodyContextImpl.GetReference().GetIndexer(indexerImpl.Parameters["index"].GetReference()));
            indexerImpl.AccessLevel = AccessLevelModifiers.Public;
            this.IndexerImpl = indexerImpl;
        }

        private void BuildGetFirstViableContext()
        {
            var firstViableContext = this._resultClass.Methods.Add(new TypedName("GetFirstViableContext", this._resultInterface));
            firstViableContext.AccessLevel = AccessLevelModifiers.Private;
            var repeatLabel = firstViableContext.DefineLabel("Repeat");
            var collapseRules =
                this._compiler._GrammarSymbols.GetRuleSymbols(this._compiler.Source.GetRules().Where(r => r.IsRuleCollapsePoint))
                    .Select(grs => this._compiler.SyntacticalSymbolModel.GetIdentitySymbolField(grs).GetReference());
            var currentContext = firstViableContext.Locals.Add(new TypedName("currentContext", this._resultInterface), new SpecialReferenceExpression(SpecialReferenceKind.This));
            var idSwitch = firstViableContext.Switch(this._compiler.CommonSymbolBuilder.Identity.GetReference(currentContext.GetReference()));
            var idSwitchCase = idSwitch
                .Case(collapseRules.ToArray());
            if (this._compiler.TokensCastAsRules.Count == 0)
            {
                idSwitchCase.Assign(currentContext.GetReference(), 
                    this.Indexer.GetReference(
                        currentContext.GetReference(), IntermediateGateway.NumberZero).Cast(currentContext.LocalType)).FollowedBy()
                    .GoTo(repeatLabel);
            }
            else
            {
                var idSwitchContext = idSwitchCase.Locals.Add(new TypedName("firstChildSymbol", this._compiler.CommonSymbolBuilder.ILanguageSymbol), this.IndexerImpl.GetReference(currentContext.GetReference(), IntermediateGateway.NumberZero));
                var idSwitchCaseSwitch = idSwitchCase.Switch(this._compiler.CommonSymbolBuilder.Identity.GetReference(idSwitchContext.GetReference()));
                var idSwitchCaseSwitchCase = idSwitchCaseSwitch.Case(this._compiler.LexicalSymbolModel.GetIdentitySymbolsFieldReferences(this._compiler._GrammarSymbols.GetSymbolsFromEntries(this._compiler.TokensCastAsRules)).ToArray());
                idSwitchCaseSwitchCase.Comment("Tokens aren't rules, so we have to use the entity that has no base rule of its own.  The CreateRule() will find the proper identity for it.");
                idSwitchCaseSwitchCase.Return(currentContext.GetReference());
                idSwitchCaseSwitch.Case(true).Assign(currentContext.GetReference(), idSwitchContext.GetReference().Cast(currentContext.LocalType)).FollowedBy()
                    .GoTo(repeatLabel);
            }
            idSwitch.Case(true).Return(currentContext.GetReference());
            this.GetFirstViableContext = firstViableContext;
        }

        private bool AssertSymbol(InlinedTokenEntry token, IGrammarTokenSymbol symbol)
        {
            if (token != null)
                Debug.Assert(symbol != null, string.Format("{0} was not inserted into the model!", token.Name));
            else
                Debug.Assert(false, "Token retrieved from TokensCastAsRules that was null!");
            return symbol != null;
        }


        private void BuildGetFirstViableParentContext()
        {
            var firstViableParentContext = this._resultClass.Methods.Add(
                new TypedName("GetFirstViableParentContext", this._resultInterface),
                new TypedNameSeries(new TypedName("origin", this.ILanguageRuleSymbol)));
            var originParam = firstViableParentContext.Parameters["origin"];
            firstViableParentContext.IsStatic = true;
            firstViableParentContext.AccessLevel = AccessLevelModifiers.Internal;
            var repeatLabel = firstViableParentContext.DefineLabel("Repeat");
            var collapseRules =
                this._compiler._GrammarSymbols.GetRuleSymbols(this._compiler.Source.GetRules().Where(r => r.IsRuleCollapsePoint))
                    .Select(grs => this._compiler.SyntacticalSymbolModel.GetIdentitySymbolField(grs).GetReference());
            var currentContext = firstViableParentContext.Locals.Add(new TypedName("currentContext", this._resultInterface), originParam.GetReference());
            var idSwitch = firstViableParentContext.Switch(this._compiler.CommonSymbolBuilder.Identity.GetReference(currentContext.GetReference()));
            idSwitch
                .Case(collapseRules.ToArray())
                .Assign(currentContext.GetReference(), this.ParentImpl.GetReference(currentContext.GetReference())).FollowedBy()
                .GoTo(repeatLabel);
            idSwitch.Case(true).Return(currentContext.GetReference());
            this.GetFirstViableParentContext = firstViableParentContext;
        }

        private void BuildCreateRule()
        {
            var createRule = this._resultInterface.Methods.Add(new TypedName("CreateRule", this._compiler.RootRuleBuilder.ILanguageRule));
            this.CreateRule = createRule;
        }

        private void BuildCreateRuleImpl()
        {
            var createRule = this._resultClass.Methods.Add(new TypedName("CreateRule", this._compiler.RootRuleBuilder.ILanguageRule));
            var activeContextLocal = createRule.Locals.Add(new TypedName("activeContext", this.ILanguageRuleSymbol), this.GetFirstViableContext.GetReference().Invoke());
            createRule.If(activeContextLocal.GetReference().InequalTo(new SpecialReferenceExpression(SpecialReferenceKind.This)))
                .Return(createRule.GetReference(activeContextLocal.GetReference()).Invoke());
            createRule.If(this.RuleCreated.GetReference()).Return(this._RuleImpl.GetReference());
            var orm = this._compiler.RelationalModelMapping;
            var tokens = this._compiler.LexicalSymbolModel.GetIdentitySymbolsFieldReferences(this._compiler._GrammarSymbols.GetSymbolsFromEntries(this._compiler.TokensCastAsRules)).ToArray();
            var rules = orm.Keys.Where(k => k is IOilexerGrammarProductionRuleEntry).Cast<IOilexerGrammarProductionRuleEntry>().ToList();
            var ruleSwitch = HandleOrmScaffolding(createRule, rules.Where(r=>!r.IsRuleCollapsePoint), activeContextLocal.GetReference(), orm.ImplementationDetails, true);
            var collapseRules = rules.Where(r => r.IsRuleCollapsePoint);
            var collpaseIdentities = this._compiler.SyntacticalSymbolModel.GetIdentitySymbolsFieldReferences(this._compiler._GrammarSymbols.GetSymbolsFromEntries(collapseRules)).ToArray();

            ISwitchCaseBlockStatement tokenCase;
            if (orm.ContainsKey(this._compiler.TokenCastAsRule))
            {
                var tokenOrm = orm.ImplementationDetails[this._compiler.TokenCastAsRule];
                var collapseCase = ruleSwitch.Case(collpaseIdentities);
                var collapseCheck = collapseCase.If(this.CountImpl.GetReference(activeContextLocal.GetReference()).GreaterThan(IntermediateGateway.NumberZero));
                var collapseSwitch = collapseCheck.Switch(this._compiler.CommonSymbolBuilder.Identity.GetReference(this._compiler.StackContextBuilder.IndexerImpl.GetReference(activeContextLocal.GetReference(), IntermediateGateway.NumberZero)));
                tokenCase = collapseSwitch.Case(tokens);

                var tokenSwitch = HandleOrmScaffolding(tokenCase, tokenOrm.Keys, activeContextLocal.GetReference(), tokenOrm, true);
            }
            createRule.Return(_RuleImpl.GetReference());
            createRule.AccessLevel = AccessLevelModifiers.Public;

            this.CreateRuleImpl = createRule;
        }

        public ISwitchStatement HandleOrmScaffolding(IBlockStatementParent currentDropZone, IEnumerable<IOilexerGrammarScannableEntry> entries, IMemberReferenceExpression activeContext, RuleTree implementationDetails, bool initialPass)
        {
            var currentSwitch = currentDropZone.Switch(this._compiler.CommonSymbolBuilder.Identity.GetReference((IMemberParentReferenceExpression)activeContext));
            foreach (var entry in entries)
            {
                //bool isCollapsePoint = entry is IOilexerGrammarProductionRuleEntry && ((IOilexerGrammarProductionRuleEntry)entry).IsRuleCollapsePoint;
                var ormData = implementationDetails[entry];
                var identity = GetEntryIdentity(entry);
                var currentCase = currentSwitch.Case(identity.GetReference());
                if (!initialPass)
                    currentCase.Assign(activeContext, this.Parent.GetReference((IMemberParentReferenceExpression)activeContext));
                /* When the sub-identities possible don't yield anything, we yield the default element at the current state. */
                var symbol = this._compiler._GrammarSymbols.GetSymbolFromEntry((IOilexerGrammarProductionRuleEntry)entry);
                Debug.Assert(symbol != null, "Symbol not inserted into model!");
                IIntermediateEnumFieldMember identityField = this._compiler.SyntacticalSymbolModel.GetIdentitySymbolField(symbol);
                if (ormData.Count > 0)
                {
                    IBlockStatement targetOfORM = currentCase;
                    var parentSelector = this.Parent.GetReference((IMemberParentReferenceExpression)activeContext);
                    var nullCheck = targetOfORM.If(parentSelector.InequalTo(IntermediateGateway.NullValue));
                    targetOfORM = nullCheck;
                    if (ormData.Value.Class != null)
                    {
                        nullCheck.CreateNext();
                        nullCheck.Next.Assign(this._RuleImpl.GetReference(), ormData.Value.Class.GetNewExpression(new SpecialReferenceExpression(SpecialReferenceKind.This)));
                    }
                    var mySwitch = HandleOrmScaffolding(targetOfORM, ormData.Keys, activeContext, ormData, false);

                    mySwitch.Selection = this._compiler.CommonSymbolBuilder.Identity.GetReference(parentSelector);
                    if (ormData.Value.Class != null)
                        mySwitch.Case(true).Assign(this._RuleImpl.GetReference(), ormData.Value.Class.GetNewExpression(new SpecialReferenceExpression(SpecialReferenceKind.This)));
                }
                else if (ormData.Value.Class != null)
                    currentCase.Assign(this._RuleImpl.GetReference(), ormData.Value.Class.GetNewExpression(new SpecialReferenceExpression(SpecialReferenceKind.This)));
            }
            return currentSwitch;
        }

        private IIntermediateEnumFieldMember GetEntryIdentity(IOilexerGrammarScannableEntry entry)
        {
            if (entry is IOilexerGrammarTokenEntry)
            {
                var symbol = this._compiler._GrammarSymbols.GetSymbolFromEntry((IOilexerGrammarTokenEntry)entry);
                Debug.Assert(symbol != null, "Symbol not inserted into model!");
                Debug.WriteLine("Symbol: {0} ({1} -- {2})", symbol.Source.Name, symbol, symbol.ElementName);
                return this._compiler.LexicalSymbolModel.GetIdentitySymbolField(symbol);
            }
            else
            {
                var symbol = this._compiler._GrammarSymbols.GetSymbolFromEntry((IOilexerGrammarProductionRuleEntry)entry);
                Debug.Assert(symbol != null, "Symbol not inserted into model!");
                return this._compiler.SyntacticalSymbolModel.GetIdentitySymbolField(symbol);
            }
        }

        public IIntermediateInterfaceType ILanguageRuleSymbol { get { return this._resultInterface; } }
        public IIntermediateClassType LanguageRuleSymbol { get { return this._resultClass; } }

        private IIntermediateInterfacePropertyMember BuildEndTokenIndex()
        {
            var result = this._resultInterface.Properties.Add(new TypedName("EndTokenIndex", RuntimeCoreType.Int32, this._assembly.IdentityManager, this._assembly), true, false);
            result.SummaryText = string.Format(@"Returns an @s:Int32; value denoting the end offset within the @s:I{0}TokenStream; the @s:{1}; is within the token stream.", this._compiler.Source.Options.AssemblyName, this._resultInterface.Name);
            return result;
        }

        private IIntermediateInterfaceMethodMember BuildGetExplicitCapture()
        {
            const string captureTParamName = "TCapture";
            var result = this._resultInterface.Methods.Add(new TypedName("GetExplicitCapture", captureTParamName), new TypedNameSeries(new TypedName("name", RuntimeCoreType.String, this._assembly.IdentityManager)), new GenericParameterData(captureTParamName));
            var nameParam = result.Parameters[TypeSystemIdentifiers.GetMemberIdentifier("name")];
            nameParam.SummaryText = @"The @s:String; value denoting the name of the capture.";
            var captureTParam = result.TypeParameters[TypeSystemIdentifiers.GetGenericParameterIdentifier(captureTParamName, false)];
            captureTParam.SummaryText = @"The type of capture to return.";
            result.SummaryText = string.Format(@"Returns the @t:{0}; of the provided @p:name;.", captureTParam.Name);
            result.ReturnsText = string.Format(@"A @t:{0}; instance denoted by the @p:name; provided.", captureTParam.Name);
            return result;
        }

        private IIntermediateInterfacePropertyMember BuildPrevious()
        {
            var result = this._resultInterface.Properties.Add(new TypedName("Previous", this._resultInterface), true, false);
            result.SummaryText = string.Format(@"Returns the @s:{0}; which was parsed just before the current @s:{0}; relative to the @s:Parent;", this._resultInterface.Name);
            result.RemarksText = @"May be null if the current is the first child of the @s:Parent;";
            return result;
        }

        private IIntermediateInterfacePropertyMember BuildNext()
        {
            var result = this._resultInterface.Properties.Add(new TypedName("Next", this._resultInterface), true, false);
            result.SummaryText = string.Format(@"Returns the @s:{0}; which was parsed just after the current @s:{0}; relative to the @s:Parent;", this._resultInterface.Name);
            result.RemarksText = @"May be null if the current is the last child of the @s:Parent;";
            return result;
        }

        private IIntermediateInterfacePropertyMember BuildParent()
        {
            var result = this._resultInterface.Properties.Add(new TypedName("Parent", this._resultInterface), true, false);
            result.SummaryText = string.Format(@"Returns the @s:{0}; which contains the current @s:{0}; as a sub-context.", this._resultInterface.Name);
            result.RemarksText = @"May be null if the current context was the first entered into the context stack.";
            return result;
        }

        private IIntermediateInterfacePropertyMember BuildFirstChild()
        {
            var result = this._resultInterface.Properties.Add(new TypedName("FirstChild", this._resultInterface), true, false);
            result.SummaryText = string.Format(@"Returns the @s:{0}; is the first child of the @s:{0}; as a sub-context.", this._resultInterface.Name);
            result.RemarksText = @"May be null if the current context is mid-parse or has no child rules relative to the source text, or definition.";
            return result;
        }

        private IIntermediateInterfacePropertyMember BuildChildren()
        {
            var result = this._resultInterface.Properties.Add(new TypedName("Children", ((IGenericType)this._assembly.IdentityManager.ObtainTypeReference(this._assembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.RootType).Assembly.UniqueIdentifier.GetTypeIdentifier("System.Collections.Generic", "IEnumerable", 1))).MakeGenericClosure(this._resultInterface)), true, false);
            result.SummaryText = string.Format(@"Returns an @s:IEnumerable{{T}}; which steps through the child nodes of the current @s:{0};.", this._resultInterface.Name);
            return result;
        }

        public IIntermediateInterfacePropertyMember EndTokenIndex { get { return this._endTokenIndex; } }

        public IIntermediateInterfaceMethodMember GetExplicitCapture { get { return this._getExplicitCapture; } }

        public IIntermediateInterfacePropertyMember Previous { get { return this._previous; } }

        public IIntermediateInterfacePropertyMember Next { get { return this._next; } }

        public IIntermediateInterfacePropertyMember Parent { get { return this._parent; } }

        public IIntermediateInterfacePropertyMember FirstChild { get { return this._firstChild; } }

        public IIntermediateInterfacePropertyMember Children { get { return this._children; } }

        public IIntermediateClassPropertyMember EndTokenIndexImpl { get { return this._endTokenIndexImpl; } }

        public IIntermediateClassMethodMember GetExplicitCaptureImpl { get { return this._getExplicitCaptureImpl; } }

        public IIntermediateClassPropertyMember PreviousImpl { get { return this._previousImpl; } }

        public IIntermediateClassPropertyMember ParentImpl { get { return this._parentImpl; } }

        public IIntermediateClassPropertyMember NextImpl { get { return this._nextImpl; } }

        public IIntermediateClassPropertyMember FirstChildImpl { get { return this._firstChildImpl; } }

        public IIntermediateClassPropertyMember ChildrenImpl { get { return this._childrenImpl; } }

        public IIntermediateInterfacePropertyMember RuleCreated { get; set; }

        public IIntermediateInterfacePropertyMember RuleCreatedImpl { get; set; }

        public IIntermediateClassMethodMember GetFirstViableContext { get; set; }

        public IIntermediateClassIndexerMember IndexerImpl { get; set; }

        public IIntermediateClassMethodMember GetFirstViableParentContext { get; set; }

        public IIntermediateClassMethodMember CreateRuleImpl { get; set; }

        public IIntermediateInterfaceMethodMember CreateRule { get; set; }

        public IIntermediateClassMethodMember DoReductionImpl { get; set; }

        public IIntermediateClassMethodMember GetEnumeratorImpl { get; set; }

        public IIntermediateClassPropertyMember TokenCountImpl { get; set; }

        public IIntermediateClassPropertyMember LengthImpl { get; set; }

        public IIntermediateClassFieldMember _TokenStreamImpl { get; set; }

        public IIntermediateClassPropertyMember CountImpl { get; set; }

        public IIntermediateInterfacePropertyMember Count { get; set; }

        public IIntermediateClassFieldMember _IdentityImpl { get; set; }

        public IIntermediateClassPropertyMember IdentityImpl { get; set; }

        public IIntermediateInterfaceIndexerMember Indexer { get; set; }

        public IIntermediateInterfaceMethodMember DoReduction { get; set; }

        public IIntermediateInterfaceMethodMember DelineateCapture { get { return this._delineateCapture; } }
        public IIntermediateClassMethodMember DelineateCaptureImpl { get { return this._delineateCaptureImpl; } }

        public IIntermediateClassPropertyMember FollowStateImpl { get; set; }

        public IIntermediateClassFieldMember _FollowStateImpl { get; set; }

        public IIntermediateClassPropertyMember InitialStateImpl { get; set; }

        public IIntermediateClassFieldMember _InitialStateImpl { get; set; }

        public IIntermediateInterfacePropertyMember HasError { get; set; }

        public IIntermediateClassPropertyMember HasErrorImpl { get; set; }

        public IIntermediateInterfacePropertyMember FollowState { get; set; }

        public IIntermediateInterfacePropertyMember InitialState { get; set; }

        public IIntermediateInterfacePropertyMember StartPosition { get; set; }

        public IIntermediateClassPropertyMember StartPositionImpl { get; set; }

        public IIntermediateClassPropertyMember EndPositionImpl { get; set; }

        public IIntermediateInterfacePropertyMember EndPosition { get; set; }
    }
}
