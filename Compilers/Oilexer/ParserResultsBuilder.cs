using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Cli;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IIntermediateClassTypeParameter   = AllenCopeland.Abstraction.Slf.Ast.Members.IIntermediateGenericTypeParameter<AllenCopeland.Abstraction.Slf.Abstract.IGeneralGenericTypeUniqueIdentifier, AllenCopeland.Abstraction.Slf.Abstract.IClassType, AllenCopeland.Abstraction.Slf.Ast.IIntermediateClassType>;
using IIntermediateInterfaceTypeParameter   = AllenCopeland.Abstraction.Slf.Ast.Members.IIntermediateGenericTypeParameter<AllenCopeland.Abstraction.Slf.Abstract.IGeneralGenericTypeUniqueIdentifier, AllenCopeland.Abstraction.Slf.Abstract.IInterfaceType, AllenCopeland.Abstraction.Slf.Ast.IIntermediateInterfaceType>;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    using InterfaceDetail   = ParserResultsInterfaceDetail<IIntermediateInterfacePropertyMember, IIntermediateInterfaceTypeParameter, IIntermediateInterfaceType>;
    using ClassDetail       =  ParserResultsConcreteDetail<IIntermediateClassPropertyMember,     IIntermediateClassTypeParameter,     IIntermediateClassType, IIntermediateClassFieldMember>    ;
    public class ParseResultsBuilder :
        IConstructBuilder<Tuple<ParserCompiler, IIntermediateAssembly>, Tuple<IIntermediateInterfaceType, IIntermediateClassType>>
    {
        private IIntermediateAssembly _assembly;
        private ParserCompiler _compiler;

        public Tuple<IIntermediateInterfaceType, IIntermediateClassType> Build(Tuple<ParserCompiler, IIntermediateAssembly> input)
        {
            this._assembly = input.Item2;
            this._compiler = input.Item1;
            this.BuildInterface();
            this.BuildClass();
            return Tuple.Create(this.InterfaceDetail.Type, this.ClassDetail.Type);

        }

        private void BuildInterface()
        {
            this.InterfaceDetail = new InterfaceDetail();
            InterfaceDetail.Type = this._assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}ParseResults", this._compiler.Source.Options.AssemblyName);
            InterfaceDetail.TResult = this.InterfaceDetail.Type.TypeParameters.Add(new GenericParameterData("TResult", new []{this._compiler.RootRuleBuilder.ILanguageRule}) { SpecialConstraint = GenericTypeParameterSpecialConstraint.Class });
        }

        private void BuildClass()
        {
            this.ClassDetail = new ClassDetail();
            this.ClassDetail.Type = this._assembly.DefaultNamespace.Parts.Add().Classes.Add("{0}ParseResults", this._compiler.Source.Options.AssemblyName);
            this.ClassDetail.TResult = this.ClassDetail.Type.TypeParameters.Add(new GenericParameterData("TResult", new []{this._compiler.RootRuleBuilder.ILanguageRule}) { SpecialConstraint = GenericTypeParameterSpecialConstraint.Class });
            this.ClassDetail.Type.ImplementedInterfaces.ImplementInterfaceQuick(InterfaceDetail.Type.MakeGenericClosure(this.ClassDetail.TResult));
        }

        public void Build2()
        {
            var iListType                                       = typeof(IList<>).GetTypeReference<IInterfaceType>((ICliManager)this._assembly.IdentityManager);
            var iListTypeGenericInstance                        = iListType.MakeGenericClosure(this._compiler.ErrorContextBuilder.ILanguageErrorContext);

            var listType                                        = typeof(List<>).GetTypeReference<IClassType>((ICliManager)this._assembly.IdentityManager);
            var listTypeGenericInstance                         = listType.MakeGenericClosure(this._compiler.ErrorContextBuilder.ILanguageErrorContext);


            this.InterfaceDetail.Result                         = this.InterfaceDetail.Type.Properties.Add(new TypedName("Result", this.InterfaceDetail.TResult), true, false);
            this.InterfaceDetail.SyntaxErrors                   = this.InterfaceDetail.Type.Properties.Add(iListTypeGenericInstance.WithName("SyntaxErrors"), true, false);
            this.InterfaceDetail.Successful                     = this.InterfaceDetail.Type.Properties.Add(typeof(bool).GetTypeReference<IStructType>((ICliManager)this._assembly.IdentityManager).WithName("Successful"), true, false);

            this.ClassDetail.Result                             = this.ClassDetail.Type.Properties.Add(this.InterfaceDetail.Result.PropertyType.WithName(this.InterfaceDetail.Result.Name), true, true);
            this.ClassDetail.SyntaxErrors                       = this.ClassDetail.Type.Properties.Add(this.InterfaceDetail.SyntaxErrors.PropertyType.WithName(this.InterfaceDetail.SyntaxErrors.Name), true, true);
            this.ClassDetail.Successful                         = this.ClassDetail.Type.Properties.Add(this.InterfaceDetail.Successful.PropertyType.WithName(this.InterfaceDetail.Successful.Name), true, true);

            this.ClassDetail._Result                            = this.ClassDetail.Type.Fields.Add(this.ClassDetail.Result.PropertyType.WithName("_result"));
            this.ClassDetail._SyntaxErrors                      = this.ClassDetail.Type.Fields.Add(this.ClassDetail.SyntaxErrors.PropertyType.WithName("_syntaxErrors"), listTypeGenericInstance.GetNewExpression());
            this.ClassDetail._Successful                        = this.ClassDetail.Type.Fields.Add(this.ClassDetail.Successful.PropertyType.WithName("_successful"));

            this.ClassDetail.Result.AccessLevel                 = AccessLevelModifiers.Public;
            this.ClassDetail.Result.SetMethod.AccessLevel       = AccessLevelModifiers.Internal;
            this.ClassDetail.Result.GetMethod.Return(this.ClassDetail._Result);
            this.ClassDetail.Result.SetMethod.Assign(this.ClassDetail._Result, this.ClassDetail.Result.SetMethod.ValueParameter);

            this.ClassDetail.SyntaxErrors.AccessLevel           = AccessLevelModifiers.Public;
            this.ClassDetail.SyntaxErrors.SetMethod.AccessLevel = AccessLevelModifiers.Internal;
            this.ClassDetail.SyntaxErrors.GetMethod.Return(this.ClassDetail._SyntaxErrors);
            this.ClassDetail.SyntaxErrors.SetMethod.Assign(this.ClassDetail._SyntaxErrors, this.ClassDetail.SyntaxErrors.SetMethod.ValueParameter);

            this.ClassDetail.Successful.AccessLevel             = AccessLevelModifiers.Public;
            this.ClassDetail.Successful.SetMethod.AccessLevel   = AccessLevelModifiers.Internal;
            this.ClassDetail.Successful.GetMethod.Return(this.ClassDetail._Successful);
            this.ClassDetail.Successful.SetMethod.Assign(this.ClassDetail._Successful, this.ClassDetail.Successful.SetMethod.ValueParameter);

            this.ClassDetail._Result.SummaryText                = string.Format("Data member for @s:{0};.", this.ClassDetail.Result.Name);
            this.ClassDetail._SyntaxErrors.SummaryText          = string.Format("Data member for @s:{0};.", this.ClassDetail.SyntaxErrors.Name);
            this.ClassDetail._Successful.SummaryText            = string.Format("Data member for @s:{0};.", this.ClassDetail.Successful.Name);
            
            this.ClassDetail.Result.SummaryText                 = 
                this.InterfaceDetail.Result.SummaryText         = string.Format("Returns the @t:{0}; value which represents the results of the parse.", this.InterfaceDetail.TResult.Name);

            this.ClassDetail.Successful.SummaryText             =
                this.InterfaceDetail.Successful.SummaryText     = "Returns whether the parse was successful.";

            this.ClassDetail.SyntaxErrors.SummaryText           =
                this.InterfaceDetail.SyntaxErrors.SummaryText = string.Format("Returns the @s:{0}; which represents the list of syntax errors that lead to the parse failing.", iListType.MakeGenericClosure(iListType.TypeParameters.Values.ToArray()).BuildTypeName(true, typeParameterDisplayMode: TypeParameterDisplayMode.CommentStandard));

            this.ClassDetail.SyntaxErrors.RemarksText           =
                this.InterfaceDetail.SyntaxErrors.RemarksText   = string.Format("null when @s:{0}; is true.", this.InterfaceDetail.Successful.Name);

        }

        public InterfaceDetail InterfaceDetail { get; set; }

        public ClassDetail ClassDetail { get; set; }

    }
    public class ParserResultsConcreteDetail<TProperty, TTypeParameter, TType, TField> :
        ParserResultsInterfaceDetail<TProperty, TTypeParameter, TType>
    {
        public TField _Result { get; set; }
        public TField _SyntaxErrors { get; set; }
        public TField _Successful { get; set; }
    }
    public class ParserResultsInterfaceDetail<TProperty, TTypeParameter, TType>
    {
        public TType Type { get; set; }
        public TTypeParameter TResult { get; set; }
        public TProperty Result { get; set; }
        public TProperty SyntaxErrors { get; set; }
        public TProperty Successful { get; set; }
    }
}
