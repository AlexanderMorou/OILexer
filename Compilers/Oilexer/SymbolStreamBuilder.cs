using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Utilities.Arrays;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class SymbolStreamBuilder :
        IConstructBuilder<Tuple<ParserCompiler, CommonSymbolBuilder, IIntermediateAssembly, GenericSymbolStreamBuilder>, Tuple<IIntermediateInterfaceType, IIntermediateClassType>>
    {
        private const string offsetParam = "offset";
        private const string symbolCountParam = "symbolCount";
        private const string reductionParam = "reduction";
        private IIntermediateAssembly assembly;
        private ParserCompiler compiler;
        private CommonSymbolBuilder tokenSymbolBuilder;

        private IIntermediateCliManager _identityManager;
        
        public Tuple<IIntermediateInterfaceType, IIntermediateClassType> Build(Tuple<ParserCompiler, CommonSymbolBuilder, IIntermediateAssembly, GenericSymbolStreamBuilder> input)
        {
            this.assembly = input.Item3;
            this.compiler = input.Item1;
            this.tokenSymbolBuilder = input.Item2;
            this._identityManager = (IIntermediateCliManager)this.assembly.IdentityManager;


            var resultInterface = assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}SymbolStream", this.compiler.Source.Options.AssemblyName);
            var resultClass = assembly.DefaultNamespace.Parts.Add().Classes.Add("{0}SymbolStream", this.compiler.Source.Options.AssemblyName);
            resultClass.BaseType = input.Item4.ResultClass.MakeGenericClosure(tokenSymbolBuilder.ILanguageSymbol);
            resultClass.ImplementedInterfaces.ImplementInterfaceQuick(resultInterface);
            resultInterface.ImplementedInterfaces.Add(input.Item4.ResultInterface.MakeGenericClosure(tokenSymbolBuilder.ILanguageSymbol));
            this.ResultInterface = resultInterface;
            this.ResultClass = resultClass;
            this.ResultInterface.AccessLevel = AccessLevelModifiers.Public;
            this.ResultClass.AccessLevel = AccessLevelModifiers.Internal;

            //Interface
            this.CreateReduce();

            //Class
            return Tuple.Create(this.ResultInterface, this.ResultClass);
        }

        public void Build2()
        {
            this.CreateLookAhead();
            this.CreateTokenStream();
            this.CreateTokenOffsetImpl();
            this.CreateTokenStreamImpl();
            this.CreateClassCtor();
            this.CreateReduceImpl();
            this.CreateIsSkipTokenImpl();
            this.CreateLookAheadImpl();
            this.BuildResetMethod();
            this.BuildPopNone();
            this.BuildSwapImpl();
        }

        private void BuildSwapImpl()
        {
            var swapImpl = this.ResultClass.Methods.Add(new TypedName("Swap", RuntimeCoreType.VoidType, this._identityManager),
                new TypedNameSeries(
                    new TypedName("current", this.compiler.CommonSymbolBuilder.ILanguageSymbol),
                    new TypedName("replacement", this.compiler.CommonSymbolBuilder.ILanguageSymbol)));
            var current = swapImpl.Parameters["current"];
            var replacement = swapImpl.Parameters["replacement"];
            var indexOf = swapImpl.Locals.Add(
                new TypedName("currentIndex", RuntimeCoreType.Int32, this._identityManager), 
                this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference()
                    .GetMethod("IndexOf")
                    .Invoke(current.GetReference()));
            swapImpl.If(indexOf.GreaterThan((-1).ToPrimitive()))
                .Assign(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference().GetIndexer(indexOf.GetReference()), replacement.GetReference());
            /*
            int clearAfter = replacement.StartTokenIndex;
            if (replacement is IComplexCalcRuleContext)
                clearAfter = ((IComplexCalcRuleContext)replacement).EndTokenIndex;
            if (this.Count - 1 > currentIndex)
                this.InternalStream.RemoveRange(currentIndex + 1, this.Count - (currentIndex + 1));
            this._tokenOffset = clearAfter + 1;
             */
            var ruleCheck = swapImpl.If(replacement.GetReference().Is(this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol));
            ruleCheck.Assign(_TokenOffsetImpl.GetReference(), this.compiler.RuleSymbolBuilder.EndTokenIndex.GetReference(replacement.GetReference().Cast(this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol)).Add(1));
            ruleCheck.CreateNext();
            ruleCheck.Next.Assign(_TokenOffsetImpl.GetReference(), this.compiler.CommonSymbolBuilder.StartTokenIndex.GetReference(replacement.GetReference()).Add(1));
            swapImpl.If(this.compiler.GenericSymbolStreamBuilder.CountImpl.Subtract(1).GreaterThan(indexOf))
                .Call(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference()
                    .GetMethod("RemoveRange").Invoke(
                        indexOf.Add(1), this.compiler.GenericSymbolStreamBuilder.CountImpl.Subtract(indexOf.Add(1))));
            swapImpl.AccessLevel = AccessLevelModifiers.Internal;
            this.SwapImpl = swapImpl;
        }

        private void BuildPopNone()
        {
            var popNoneMethod = this.ResultClass.Methods.Add(new TypedName("PopNone", RuntimeCoreType.VoidType, this._identityManager));
            popNoneMethod.If(this.compiler.GenericSymbolStreamBuilder.CountImpl.LessThanOrEqualTo(IntermediateGateway.NumberZero))
                .Return();
            popNoneMethod.If(this.LookAheadMethodImpl.GetReference().Invoke(this.compiler.GenericSymbolStreamBuilder.CountImpl.GetReference().Subtract(1)).EqualTo(this.compiler.LexicalSymbolModel.NoIdentityField))
                .Call(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference().GetMethod("RemoveAt").Invoke(this.compiler.GenericSymbolStreamBuilder.CountImpl.GetReference().Subtract(1)));

            popNoneMethod.AccessLevel = AccessLevelModifiers.Public;
            this.PopNoneImpl = popNoneMethod;
        }

        private void BuildResetMethod()
        {
            var resetMethod = this.ResultClass.Methods.Add(new TypedName("Reset", RuntimeCoreType.VoidType, this._identityManager));
            resetMethod.Call(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference().GetMethod("Clear").Invoke());
            resetMethod.Assign(this._TokenOffsetImpl.GetReference(),IntermediateGateway.NumberZero);
            resetMethod.AccessLevel = AccessLevelModifiers.Public;
            this.ResetMethodImpl = resetMethod;
        }

        private void CreateIsSkipTokenImpl()
        {
            if (!this.compiler.Source.GetTokens().Any(k => k.Unhinged))
                return;
            var shouldSkipImpl = this.ResultClass.Methods.Add(
                new TypedName("ShouldSkip", RuntimeCoreType.Boolean, this._identityManager),
                new TypedNameSeries(
                    new TypedName("identity", this.compiler.SymbolStoreBuilder.Identities)));
            var identityParam = shouldSkipImpl.Parameters["identity"];
            var unhingedTokens = (from t in this.compiler.Source.GetTokens()
                                  where t.Unhinged
                                  join s in this.compiler._GrammarSymbols.TokenSymbols on t equals s.Source
                                  let idField = this.compiler.LexicalSymbolModel.GetIdentitySymbolField(s)
                                  select new { Symbol = s, IdentityField = idField }).Distinct().ToArray();
            if (unhingedTokens.Length == 1)
                shouldSkipImpl.Return(identityParam.EqualTo(unhingedTokens[0].IdentityField));
            else
            {
                shouldSkipImpl.Switch(identityParam.GetReference()).Case(
                    unhingedTokens.Select(k => (IExpression)k.IdentityField.GetReference()).ToArray())
                    .Comment("The following tokens were noted in the language to be skippable.").FollowedBy()
                    .Return(IntermediateGateway.TrueValue);
                shouldSkipImpl.Return(IntermediateGateway.FalseValue);
            }
            shouldSkipImpl.IsStatic = true;
            shouldSkipImpl.AccessLevel = AccessLevelModifiers.Private;
            this.ShouldSkipImpl = shouldSkipImpl;
        }

        private void CreateTokenStreamImpl()
        {
            var _tokenStreamImpl = this.ResultClass.Fields.Add(new TypedName("_tokenStream", this.compiler.TokenStreamBuilder.ResultInterface));
            var tokenStreamImpl = this.ResultClass.Properties.Add(new TypedName("TokenStream", this.compiler.TokenStreamBuilder.ResultInterface), true, false);
            tokenStreamImpl.GetMethod.Return(_tokenStreamImpl.GetReference());
            tokenStreamImpl.AccessLevel = AccessLevelModifiers.Public;
            _tokenStreamImpl.AccessLevel = AccessLevelModifiers.Private;
            this._TokenStreamImpl = _tokenStreamImpl;
            this.TokenStreamImpl = tokenStreamImpl;
        }

        private void CreateTokenOffsetImpl()
        {
            var tokenOffset = this.ResultClass.Fields.Add(new TypedName("_tokenOffset", RuntimeCoreType.Int32, this._identityManager));
            tokenOffset.AccessLevel = AccessLevelModifiers.Private;
            this._TokenOffsetImpl = tokenOffset;
        }

        private void CreateTokenStream()
        {
            var tokenStream = this.ResultInterface.Properties.Add(new TypedName("TokenStream", this.compiler.TokenStreamBuilder.ResultInterface), true, false);
            this.TokenStream = tokenStream;
        }

        private void CreateReduce()
        {
            var readOnlyList = typeof(IReadOnlyList<>).ObtainCILibraryType<IInterfaceType>(assembly.IdentityManager).MakeGenericClosure(tokenSymbolBuilder.ILanguageSymbol);
            var reduceMethod = ResultInterface.Methods.Add(
                new TypedName("Reduce", readOnlyList), 
                new TypedNameSeries(
                    new TypedName(offsetParam, RuntimeCoreType.Int32, this.assembly.IdentityManager),
                    new TypedName(symbolCountParam, RuntimeCoreType.Int32, this.assembly.IdentityManager),
                    new TypedName(reductionParam, tokenSymbolBuilder.ILanguageSymbol)));
            var offset = reduceMethod.Parameters[offsetParam];
            var symbolCount = reduceMethod.Parameters[symbolCountParam];
            offset.SummaryText = "The @s:Int32; value denoting the starting point of the reduced symbol.";
            symbolCount.SummaryText = "The @s:Int32; value denoting the number of symbols consumed by the reduced symbol.";
            reduceMethod.SummaryText = string.Format("Reduces a given series of @s:{0}; elements into a single symbol.", tokenSymbolBuilder.ILanguageSymbol.Name);
            reduceMethod.RemarksText = "The reduced symbol is injected internally.";
            this.ReduceMethod = reduceMethod;
        }

        private void CreateLookAhead()
        {
            var lookAheadMethod = this.ResultInterface.Methods.Add(
                new TypedName("LookAhead", this.compiler.SymbolStoreBuilder.Identities),
                new TypedNameSeries(
                    new TypedName("index", RuntimeCoreType.Int32, this._identityManager)));
            this.LookAheadMethod = lookAheadMethod;
        }

        private void CreateClassCtor()
        {
            var classCtor = this.ResultClass.Constructors.Add(
                new TypedName("tokenStream", this.compiler.TokenStreamBuilder.ResultInterface));
            classCtor.Assign(_TokenStreamImpl.GetReference(), classCtor.Parameters["tokenStream"].GetReference());
            this.ResultClassCtor = classCtor;
            classCtor.AccessLevel = AccessLevelModifiers.Public;
        }

        private void CreateReduceImpl()
        {
            var readOnlyList = typeof(IReadOnlyList<>).ObtainCILibraryType<IInterfaceType>(assembly.IdentityManager).MakeGenericClosure(tokenSymbolBuilder.ILanguageSymbol);
            
            var reduceMethodImpl = ResultClass.Methods.Add(
                new TypedName("Reduce", readOnlyList), 
                new TypedNameSeries(
                    new TypedName(offsetParam, RuntimeCoreType.Int32, this.assembly.IdentityManager),
                    new TypedName(symbolCountParam, RuntimeCoreType.Int32, this.assembly.IdentityManager),
                    new TypedName(reductionParam, tokenSymbolBuilder.ILanguageSymbol)));
            var offsetParameter = reduceMethodImpl.Parameters[offsetParam];
            var symbolCountParameter = reduceMethodImpl.Parameters[symbolCountParam];
            var reductionParameter = reduceMethodImpl.Parameters[reductionParam];

            

            var reductionListResult = reduceMethodImpl.Locals.Add(new TypedName("reductionList", ((IClassType)this._identityManager.ObtainTypeReference(typeof(List<>))).MakeGenericClosure(tokenSymbolBuilder.ILanguageSymbol)));
            reduceMethodImpl.If(symbolCountParameter.LessThanOrEqualTo(IntermediateGateway.NumberZero))
                .Return(reductionListResult.GetReference());

            reductionListResult.InitializationExpression = reductionListResult.LocalType.GetNewExpression();
            var symbolIndexLocal = reduceMethodImpl.Locals.Add(new TypedName("symbolIndex", RuntimeCoreType.Int32, this._identityManager), offsetParameter.GetReference());
            symbolIndexLocal.AutoDeclare = false;
            reduceMethodImpl.Iterate(symbolIndexLocal.GetDeclarationStatement(), symbolIndexLocal.LessThan(offsetParameter.Add(symbolCountParameter)), symbolIndexLocal.Increment().AsEnumerable())
                .Call(
                    reductionListResult.GetReference().GetMethod("Add").Invoke(this.compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(new SpecialReferenceExpression(SpecialReferenceKind.This), symbolIndexLocal.GetReference())));
            reduceMethodImpl
                .Call(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference().GetMethod("RemoveRange").Invoke(offsetParameter.GetReference(), symbolCountParameter.GetReference())).FollowedBy()
                .Call(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference().GetMethod("Insert").Invoke(offsetParameter.GetReference(), reductionParameter.GetReference())).FollowedBy()
                .Return(reductionListResult.GetReference());

            reduceMethodImpl.AccessLevel = AccessLevelModifiers.Public;
            this.ReduceMethodImpl = reduceMethodImpl;
        }

        private void CreateLookAheadImpl()
        {
            var lookAheadMethodImpl = this.ResultClass.Methods.Add(
                new TypedName("LookAhead", this.compiler.SymbolStoreBuilder.Identities),
                new TypedNameSeries(
                    new TypedName("index", RuntimeCoreType.Int32, this._identityManager)));
            var indexParam = lookAheadMethodImpl.Parameters["index"];
            lookAheadMethodImpl.If(indexParam.LessThan(this.compiler.GenericSymbolStreamBuilder.CountImpl))
                .Return(this.compiler.CommonSymbolBuilder.Identity.GetReference(this.compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(new SpecialReferenceExpression(SpecialReferenceKind.This), indexParam.GetReference())));
            lookAheadMethodImpl.If(indexParam.GreaterThan(this.compiler.GenericSymbolStreamBuilder.CountImpl))
                .Return(this.compiler.LexicalSymbolModel.StreamAccessOutOfSequence.GetReference());
            if (ShouldSkipImpl != null)
            {
                var identityResultLocal = lookAheadMethodImpl.Locals.Add(new TypedName("identityResult", lookAheadMethodImpl.ReturnType));
                identityResultLocal.AutoDeclare = false;
                lookAheadMethodImpl.DefineLocal(identityResultLocal);
                var sharedExpression = this.compiler.TokenStreamBuilder.LookAheadImpl.GetReference(TokenStreamImpl.GetReference()).Invoke(this._TokenOffsetImpl.GetReference());
                lookAheadMethodImpl.Iterate((identityResultLocal.GetReference().Assign(sharedExpression)).AsEnumerable(), ShouldSkipImpl.GetReference().Invoke(identityResultLocal.GetReference()), new IStatementExpression[2] { this._TokenOffsetImpl.Increment(), identityResultLocal.GetReference().Assign((INaryOperandExpression)sharedExpression) });
                lookAheadMethodImpl.Call(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference().GetMethod("Add").Invoke(this.compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this.TokenStreamImpl.GetReference(), this._TokenOffsetImpl.Increment())));
                lookAheadMethodImpl.Return(identityResultLocal.GetReference());
            }
            else
            {
                var identityResultLocal = lookAheadMethodImpl.Locals.Add(new TypedName("identityResult", lookAheadMethodImpl.ReturnType), this.compiler.TokenStreamBuilder.LookAheadImpl.GetReference(TokenStreamImpl.GetReference()).Invoke(this._TokenOffsetImpl.GetReference()));
                identityResultLocal.AutoDeclare = false;
                lookAheadMethodImpl.DefineLocal(identityResultLocal);
                lookAheadMethodImpl.Call(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference().GetMethod("Add").Invoke(this.compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(this.TokenStreamImpl.GetReference(), this._TokenOffsetImpl.Increment())));
                lookAheadMethodImpl.Return(identityResultLocal.GetReference());
            }
            lookAheadMethodImpl.AccessLevel = AccessLevelModifiers.Public;

            this.LookAheadMethodImpl = lookAheadMethodImpl;
        }

        public IIntermediateInterfaceType ResultInterface { get; private set; }
        public IIntermediateClassType ResultClass { get; private set; }


        public IIntermediateClassMethodMember LookAheadMethodImpl { get; set; }

        public IIntermediateInterfaceMethodMember LookAheadMethod { get; set; }

        public IIntermediateClassMethodMember ReduceMethodImpl { get; set; }

        public IIntermediateInterfaceMethodMember ReduceMethod { get; set; }

        public IIntermediateClassPropertyMember TokenStreamImpl { get; set; }

        public IIntermediateClassFieldMember _TokenOffsetImpl { get; set; }

        public IIntermediateInterfacePropertyMember TokenStream { get; set; }

        public IIntermediateClassFieldMember _TokenStreamImpl { get; set; }

        public IIntermediateClassCtorMember ResultClassCtor { get; set; }

        public IIntermediateClassMethodMember ShouldSkipImpl { get; set; }

        public IIntermediateClassMethodMember ResetMethodImpl { get; set; }

        public IIntermediateClassMethodMember PopNoneImpl { get; set; }

        public IIntermediateClassMethodMember SwapImpl { get; set; }
    }
}
