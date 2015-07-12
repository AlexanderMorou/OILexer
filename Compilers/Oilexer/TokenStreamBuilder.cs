using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using AllenCopeland.Abstraction.Slf.Ast.Cli;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class TokenStreamBuilder :
        IConstructBuilder<Tuple<ParserCompiler, TokenSymbolBuilder, IIntermediateAssembly, GenericSymbolStreamBuilder>, Tuple<IIntermediateInterfaceType, IIntermediateClassType>>
    {
        private IIntermediateAssembly assembly;
        private ParserCompiler compiler;
        private TokenSymbolBuilder tokenSymbolBuilder;
        private IIntermediateCliManager _identityManager;

        public Tuple<IIntermediateInterfaceType, IIntermediateClassType> Build(Tuple<ParserCompiler, TokenSymbolBuilder, IIntermediateAssembly, GenericSymbolStreamBuilder> input)
        {
            this.assembly = input.Item3;
            this.compiler = input.Item1;
            this.tokenSymbolBuilder = input.Item2;
            this._identityManager = (IIntermediateCliManager)this.assembly.IdentityManager;
            var resultInterface = assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}TokenStream", this.compiler.Source.Options.AssemblyName);
            var resultClass = assembly.DefaultNamespace.Parts.Add().Classes.Add("{0}TokenStream", this.compiler.Source.Options.AssemblyName);
            resultClass.BaseType = input.Item4.ResultClass.MakeGenericClosure(input.Item2.ILanguageToken);
            resultClass.ImplementedInterfaces.ImplementInterfaceQuick(resultInterface);
            resultInterface.ImplementedInterfaces.Add(input.Item4.ResultInterface.MakeGenericClosure(input.Item2.ILanguageToken));
            this.ResultInterface = resultInterface;
            this.ResultClass = resultClass;
            this.ResultInterface.AccessLevel = AccessLevelModifiers.Public;
            this.ResultClass.AccessLevel = AccessLevelModifiers.Internal;
            return Tuple.Create(this.ResultInterface, this.ResultClass);
        }

        public void Build2()
        {
            this.CreateTokenizer();
            this.CreateLookAhead();
            this.CreateTokenizerImpl();
            this.CreateLookAheadImpl();
            this.CreateCtor();
        }

        private void CreateTokenizer()
        {
            this.Tokenizer = this.ResultInterface.Properties.Add(new TypedName(this.compiler.Source.Options.LexerName, this.compiler.LexerBuilder.LexerInterface), true, false);
        }

        private void CreateLookAhead()
        {
            this.LookAhead = this.ResultInterface.Methods.Add(
                new TypedName("LookAhead", this.compiler.SymbolStoreBuilder.Identities),
                new TypedNameSeries(
                    new TypedName("tokenOffset", RuntimeCoreType.Int32, this._identityManager)));

        }

        private void CreateCtor()
        {
            string tokenizerName = this.compiler.Source.Options.LexerName.LowerFirstCharacter();
            var classCtor = this.ResultClass.Constructors.Add(
                new TypedName(tokenizerName, this.compiler.LexerBuilder.LexerInterface));
            var tokenizerParam = classCtor.Parameters[tokenizerName];
            classCtor.Assign(this._TokenizerImpl.GetReference(), tokenizerParam.GetReference());
            classCtor.AccessLevel = AccessLevelModifiers.Public;
        }

        private void CreateTokenizerImpl()
        {
            
            this._TokenizerImpl =
                this.ResultClass.Fields.Add(
                new TypedName(
                    string.Format("_{0}", this.compiler.Source.Options.LexerName.LowerFirstCharacter()),
                    this.compiler.LexerBuilder.LexerInterface));
            this._TokenizerImpl.AccessLevel = AccessLevelModifiers.Private;
            this.TokenizerImpl = 
                this.ResultClass.Properties.Add(
                    new TypedName(this.compiler.Source.Options.LexerName, this.compiler.LexerBuilder.LexerInterface), true, false);
            this.TokenizerImpl.AccessLevel = AccessLevelModifiers.Public;
            this.TokenizerImpl.GetMethod.Return(this._TokenizerImpl.GetReference());
            this.TokenizerImpl.SummaryText = string.Format("Returns the @s:{0}; from which the @s:{1}; obtains the initial symbols for when further context is necessary.", this._TokenizerImpl.FieldType.Name, this.ResultClass.Name);
            this.TokenizerImpl.RemarksText = string.Format("If the zero-based index of the requested symbol is equal to the number of elements within the @s:{0};, the next token will be retrieved; otherwise, @s:{1}.{2}; will result.", this.ResultClass.Name, this.compiler.LexicalSymbolModel.StreamAccessOutOfSequence.Parent.Name, this.compiler.LexicalSymbolModel.StreamAccessOutOfSequence.Name);
        }

        private void CreateLookAheadImpl()
        {
            var lookAheadImpl = this.ResultClass.Methods.Add(
                new TypedName("LookAhead", this.compiler.SymbolStoreBuilder.Identities),
                new TypedNameSeries(
                    new TypedName("tokenOffset", RuntimeCoreType.Int32, this._identityManager)));
            var tokenOffsetParam = lookAheadImpl.Parameters["tokenOffset"];
            lookAheadImpl.If(tokenOffsetParam.LessThan(this.compiler.GenericSymbolStreamBuilder.CountImpl))
                /* This indirect mechanism seems excessive; however, it serves a purpose:
                 * In t-html mode, it'll ensure that the properties/methods represented 
                 * are click-able and navigate to the proper member/file via an anchor tag. */
                .Return(
                    this.compiler.CommonSymbolBuilder.Identity.GetReference(
                        this.compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(
                            new SpecialReferenceExpression(SpecialReferenceKind.This), tokenOffsetParam.GetReference())));
            lookAheadImpl.If(this.compiler.GenericSymbolStreamBuilder.EndOFilePresentImpl.GetReference())
                .Return(this.compiler.LexicalSymbolModel.GetEofIdentityField().GetReference());

            lookAheadImpl.If(tokenOffsetParam.GreaterThan(this.compiler.GenericSymbolStreamBuilder.CountImpl.GetReference()))
                .Return(this.compiler.LexicalSymbolModel.StreamAccessOutOfSequence.GetReference());
            var expectedPosition = lookAheadImpl.Locals.Add(new TypedName("expectedPosition", RuntimeCoreType.Int32, this._identityManager), IntermediateGateway.NumberZero);
            expectedPosition.AutoDeclare = false;
            lookAheadImpl.DefineLocal(expectedPosition);
            lookAheadImpl.If(this.compiler.GenericSymbolStreamBuilder.CountImpl.GreaterThan(IntermediateGateway.NumberZero))
                .Assign(expectedPosition.GetReference(), this.compiler.TokenSymbolBuilder.EndPosition.GetReference(this.compiler.GenericSymbolStreamBuilder.IndexerImpl.GetReference(new SpecialReferenceExpression(SpecialReferenceKind.This), this.compiler.GenericSymbolStreamBuilder.CountImpl.Subtract(1))));
            //lookAheadImpl.If(this.compiler.LexerBuilder.NextTokenImpl.GetReference().Invoke());
            lookAheadImpl.If(this.compiler.LexerBuilder.PositionImpl.GetReference(this.TokenizerImpl.GetReference()).InequalTo(expectedPosition))
                .Assign(this.compiler.LexerBuilder.PositionImpl.GetReference(this.TokenizerImpl.GetReference()), expectedPosition.GetReference().Cast(this._identityManager.ObtainTypeReference(RuntimeCoreType.Int64)));
            lookAheadImpl.AccessLevel = AccessLevelModifiers.Public;
            this.LookAheadImpl = lookAheadImpl;
        }

        public void Build3()
        {
            var nextToken = this.LookAheadImpl.Locals.Add(new TypedName("nextToken", this.compiler.TokenSymbolBuilder.ILanguageToken), this.compiler.LexerBuilder.NextTokenImpl.GetReference(this.TokenizerImpl.GetReference()).Invoke());
            nextToken.AutoDeclare = false;
            this.LookAheadImpl.DefineLocal(nextToken);
            this.LookAheadImpl.Assign(this.compiler.TokenSymbolBuilder.StartTokenIndex.GetReference(nextToken.GetReference()), this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference().GetProperty("Count"));
            this.LookAheadImpl.Call(this.compiler.GenericSymbolStreamBuilder.InternalStreamImpl.GetReference().GetMethod("Add").Invoke(nextToken.GetReference()));
            this.LookAheadImpl.Return(this.compiler.CommonSymbolBuilder.Identity.GetReference(nextToken.GetReference()));
        }

        public IIntermediateInterfaceType ResultInterface { get; private set; }
        public IIntermediateClassType ResultClass { get; private set; }


        public IIntermediateInterfaceMethodMember LookAhead { get; set; }

        public IIntermediateClassMethodMember LookAheadImpl { get; set; }

        public IIntermediateInterfacePropertyMember Tokenizer { get; set; }

        public IIntermediateClassPropertyMember TokenizerImpl { get; set; }

        public IIntermediateClassFieldMember _TokenizerImpl { get; set; }

    }
}
