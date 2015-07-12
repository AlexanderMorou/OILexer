using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class ErrorContextBuilder 
    {
        private ParserCompiler _compiler;
        private IIntermediateAssembly _assembly;
        private IGeneralDeclarationUniqueIdentifier _targetSpaceName;
        private IIntermediateCliManager _identityManager;

        public ErrorContextBuilder(ParserCompiler compiler, IIntermediateCliManager identityManager, IIntermediateAssembly assembly)
        {
            this._identityManager = identityManager;
            this._assembly = assembly;
            this._compiler = compiler;
        }

        public void Build()
        {
            this.BuildSymbol();
            this.BuildCauseEnum();
            this.BuildInterface();
            this.BuildClass();
        }

        public void Build2()
        {
            //Second half includes: State, and Expected in which build-order is important due to ambiguity contexts.
            this.FinishInterface();
            this.FinishClass();
        }

        private void FinishInterface()
        {
            this.BuildState();
            this.BuildExpected();
        }

        private void BuildState()
        {
            this.State = this.ILanguageErrorContext.Properties.Add(new TypedName("State", RuntimeCoreType.Int32, this._identityManager), true, false);
            this.State.SummaryText = string.Format("Returns the @s:Int32; value denoting the state within the @s:{0}; or @s:{1}; depending on the @s:{2};", this._compiler.LexerBuilder.LexerInterface.Name, this._compiler.ParserBuilder.ParserInterface.Name, this.Cause.Name);
        }

        private void BuildExpected()
        {
            this.Expected = this.ILanguageErrorContext.Properties.Add(new TypedName("Expected", this._compiler.LexicalSymbolModel.ValidSymbols.MakeNullable()), true, false);
        }

        private void FinishClass()
        {
            this.BuildStateImpl();
            this.BuildExpectedImpl();
            this.BuildCtor();
        }

        private void BuildCtor()
        {
            this.LanguageErrorContextCtor = 
                this.LanguageErrorContext.Constructors.Add(
                    new TypedName("cause", this.CauseEnum),
                    new TypedName("state", RuntimeCoreType.Int32, this._identityManager),
                    new TypedName("expected", this._compiler.LexicalSymbolModel.ValidSymbols));
            var cause = this.LanguageErrorContextCtor.Parameters["cause"];
            var state = this.LanguageErrorContextCtor.Parameters["state"];
            var expected = this.LanguageErrorContextCtor.Parameters["expected"];
            this.LanguageErrorContextCtor.Assign(this._CauseImpl.GetReference(), cause.GetReference());
            this.LanguageErrorContextCtor.Assign(this._StateImpl.GetReference(), state.GetReference());
            this.LanguageErrorContextCtor.Assign(this._ExpectedImpl.GetReference(), expected.GetReference());
            this.LanguageErrorContextCtor.AccessLevel = AccessLevelModifiers.Public;
        }

        private void BuildStateImpl()
        {
            this._StateImpl = this.LanguageErrorContext.Fields.Add(new TypedName("_state", RuntimeCoreType.Int32, this._identityManager));
            this._StateImpl.AccessLevel = AccessLevelModifiers.Private;

            this.StateImpl = this.LanguageErrorContext.Properties.Add(new TypedName("State", RuntimeCoreType.Int32, this._identityManager), true, false);
            this.StateImpl.GetMethod.Return(_StateImpl.GetReference());
            this.StateImpl.AccessLevel = AccessLevelModifiers.Public;

            this._StateImpl.SummaryText = string.Format("Data member for @s:{0};", this.StateImpl.Name);
            this.StateImpl.SummaryText = string.Format("Returns the @s:Int32; value denoting the state within the @s:{0}; or @s:{1}; depending on the @s:{2};", this._compiler.LexerBuilder.LexerInterface.Name, this._compiler.ParserBuilder.ParserInterface.Name, this.CauseImpl.Name);
        }

        private void BuildExpectedImpl()
        {
            this._ExpectedImpl = this.LanguageErrorContext.Fields.Add(new TypedName("_expected", this._compiler.LexicalSymbolModel.ValidSymbols.MakeNullable()));
            this._ExpectedImpl.AccessLevel = AccessLevelModifiers.Private;
            this.ExpectedImpl = this.LanguageErrorContext.Properties.Add(new TypedName("Expected", this._compiler.LexicalSymbolModel.ValidSymbols.MakeNullable()), true, false);
            this.ExpectedImpl.GetMethod.Return(this._ExpectedImpl.GetReference());
            this.ExpectedImpl.AccessLevel = AccessLevelModifiers.Public;
            this._ExpectedImpl.SummaryText = string.Format("Data member for @s:{0};", this.ExpectedImpl.Name);

            this.ExpectedImpl.SummaryText = string.Format("Returns the @s:{0}; which denote the valid set of token symbols at the time of the request.", this._compiler.LexicalSymbolModel.ValidSymbols.Name);
        }

        private void BuildInterface()
        {
            this.ILanguageErrorContext = this._assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}ErrorContext", this._compiler.Source.Options.AssemblyName);

            this.BuildCause();
        }

        private void BuildCause()
        {
            this.Cause = this.ILanguageErrorContext.Properties.Add(new TypedName("Cause", this.CauseEnum), true, false);
        }

        private void BuildCauseEnum()
        {
            this.CauseEnum = this._assembly.DefaultNamespace.Parts.Add().Enums.Add("{0}ErrorCause", this._compiler.Source.Options.AssemblyName);
            this.CauseEnum.SummaryText = "The possible causes that would yield an error during parsing.";
            this.CauseExpected = this.CauseEnum.Fields.Add("Expected");
            this.CauseExpected.SummaryText = "Notes that an error occurred due to an expected token not being found.";
            this.CauseExpected.RemarksText = 
                "@para;Also provides context as to what token was found.@/para;" +
                "@para;This is a parser-based error@/para;";
            this.CauseUnrecognized = this.CauseEnum.Fields.Add("Unrecognized");
            this.CauseUnrecognized.SummaryText = "Notes that an error occurred due to an unrecognized character sequence within the char stream.";
            this.CauseUnrecognized.RemarksText = "This is a lexer-based error";
            this.CauseInternalError = this.CauseEnum.Fields.Add("InternalError");
            this.CauseInternalError.SummaryText = "Notes that an error occurred due to an internal error in the parsing mechanism or one of its dependencies.";
            this.CauseInternalError.RemarksText = "This is a general purpose error";
        }

        private void BuildSymbol()
        {
            var errorContextIdentity = this._compiler.SymbolStoreBuilder.Identities.Fields.Add(GrammarVocabularyModelBuilder.GetUniqueEnumFieldName("Error", this._compiler.SymbolStoreBuilder.Identities));
            this.ErrorIdentity = errorContextIdentity;
        }

        private void BuildClass()
        {
            this.LanguageErrorContext = this._assembly.DefaultNamespace.Parts.Add().Classes.Add("{0}ErrorContext", this._compiler.Source.Options.AssemblyName);
            this.LanguageErrorContext.ImplementedInterfaces.ImplementInterfaceQuick(this.ILanguageErrorContext);
            this.LanguageErrorContext.AccessLevel = AccessLevelModifiers.Internal;
            this.BuildLengthImpl();
            this.BuildCauseImpl();
        }
        
        private void BuildLengthImpl()
        {
            this.LengthImpl = this.LanguageErrorContext.Properties.Add(new TypedName("Length", RuntimeCoreType.Int32, this._identityManager), true, false);
            this.LengthImpl.AccessLevel = AccessLevelModifiers.Public;
            this.LengthImpl.GetMethod.Return(IntermediateGateway.NumberZero);
        }

        private void BuildCauseImpl()
        {
            this._CauseImpl = this.LanguageErrorContext.Fields.Add(new TypedName("_cause", this.CauseEnum));
            this._CauseImpl.AccessLevel = AccessLevelModifiers.Public;
            this.CauseImpl = this.LanguageErrorContext.Properties.Add(new TypedName("Cause", this.CauseEnum), true, false);
            this.CauseImpl.AccessLevel = AccessLevelModifiers.Public;
            this.CauseImpl.SummaryText = string.Format("Returns the @s:{0}; which denotes the source of the error.", this.CauseEnum.Name);
            this.CauseImpl.GetMethod.Return(this._CauseImpl.GetReference());
        }

        public IIntermediateClassFieldMember _CauseImpl { get; set; }

        public IIntermediateClassFieldMember _ExpectedImpl { get; set; }

        public IIntermediateClassFieldMember _StateImpl { get; set; }

        public IIntermediateClassPropertyMember CauseImpl { get; set; }

        public IIntermediateClassPropertyMember ExpectedImpl { get; set; }

        public IIntermediateClassPropertyMember LengthImpl { get; set; }

        public IIntermediateClassPropertyMember StartTokenIndexImpl { get; set; }

        public IIntermediateClassPropertyMember StateImpl { get; set; }

        public IIntermediateClassType LanguageErrorContext { get; set; }


        public IIntermediateEnumFieldMember CauseExpected { get; set; }

        public IIntermediateEnumFieldMember CauseInternalError { get; set; }

        public IIntermediateEnumFieldMember CauseUnrecognized { get; set; }

        public IIntermediateEnumFieldMember ErrorIdentity { get; set; }

        public IIntermediateEnumType CauseEnum { get; set; }

        public IIntermediateInterfacePropertyMember Cause { get; set; }

        public IIntermediateInterfacePropertyMember Expected { get; set; }

        public IIntermediateInterfacePropertyMember State { get; set; }

        public IIntermediateInterfaceType ILanguageErrorContext { get; set; }


        public IIntermediateClassCtorMember LanguageErrorContextCtor { get; set; }
    }
}
