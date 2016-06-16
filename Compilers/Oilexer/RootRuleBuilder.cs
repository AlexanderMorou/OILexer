using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class RootRuleBuilder :
        IConstructBuilder<Tuple<ParserCompiler, RuleSymbolBuilder, IIntermediateAssembly>, Tuple<IIntermediateClassType, IIntermediateInterfaceType>>
    {
        private ParserCompiler compiler;
        private RuleSymbolBuilder commonSymbol;
        private IIntermediateAssembly assembly;
        private IIntermediateInterfaceType resultInterface;
        private IIntermediateClassType resultClass;
        private IIntermediateInterfacePropertyMember interfaceContext;
        private IIntermediateInterfacePropertyMember interfaceParent;
        private IIntermediateClassPropertyMember classContext;
        private IIntermediateClassPropertyMember classParent;
        private IIntermediateClassFieldMember classContextField;
        private IIntermediateClassFieldMember classParentField;

        public ParserCompiler Compiler { get { return this.compiler; } }

        public Tuple<IIntermediateClassType, IIntermediateInterfaceType> Build(Tuple<ParserCompiler, RuleSymbolBuilder, IIntermediateAssembly> input)
        {
            this.compiler = input.Item1;
            this.commonSymbol = input.Item2;
            this.assembly = input.Item3;
            INamespaceDeclaration targetSpace;
            var targetSpaceName = TypeSystemIdentifiers.GetDeclarationIdentifier(string.Format("{0}.Cst", this.assembly.DefaultNamespace.FullName));
            if (!assembly.Namespaces.PathExists(targetSpaceName.Name))
                targetSpace = this.assembly.DefaultNamespace.Namespaces.Add("Cst");
            else
                targetSpace = this.assembly.DefaultNamespace.Namespaces[targetSpaceName];
            var mutableTargetSpace = (IIntermediateNamespaceDeclaration)targetSpace;
            this.resultInterface = mutableTargetSpace.Parts.Add().Interfaces.Add("I{0}Rule", compiler.Source.Options.AssemblyName);
            this.resultClass = mutableTargetSpace.Parts.Add().Classes.Add("{0}RuleBase", compiler.Source.Options.AssemblyName);
            this.resultClass.AccessLevel = AccessLevelModifiers.Internal;
            this.resultClass.ImplementedInterfaces.ImplementInterfaceQuick(resultInterface);
            this.resultInterface.AccessLevel = AccessLevelModifiers.Public;
            this.interfaceContext = this.BuildContext();
            this.interfaceParent = this.BuildParent();
            this.classContextField = this.BuildClassContextField();
            this.classParentField = this.BuildClassParentField();
            this.classContext = BuildClassContext();
            this.classParent = BuildClassParent();
            this.resultClass.SpecialModifier = SpecialClassModifier.Abstract;
            return Tuple.Create(resultClass, resultInterface);
        }

        public IIntermediateClassPropertyMember ContextImpl { get { return this.classContext; } }

        public void Build2()
        {
            this.BuildClassCtor();
            this.FinishClassParent();
        }

        private void FinishClassParent()
        {
            var classParent = this.classParent;
            classParent.AccessLevel = AccessLevelModifiers.Public;
            classParent.SummaryText = string.Format(@"Returns/sets the @s:{0}; which contains the current @s:{1};.", this.resultInterface.Name, this.resultClass.Name);
            classParent.RemarksText = @"May be null if there was no ancestor in the context stack at the time of parse.";
            var parentNullCheck = classParent.GetMethod.If(classParentField.EqualTo(IntermediateGateway.NullValue));
            var parentContextLocal = 
                parentNullCheck.Locals.Add(
                    new TypedName(
                        "parentContext",
                        this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol), 
                    this.compiler.RuleSymbolBuilder.GetFirstViableParentContext
                    .GetReference()
                    .Invoke(
                        this.compiler.RuleSymbolBuilder.Parent.GetReference(
                            this.ContextImpl.GetReference())));
            parentContextLocal.AutoDeclare = false;
            parentNullCheck.DefineLocal(parentContextLocal);
            IConditionBlockStatement parentContextNullCheck = null;
            //if (this.compiler.Source.GetRules().Any(k=>k.IsRuleCollapsePoint))
            //    parentContextNullCheck = parentNullCheck.If(parentContextLocal.InequalTo(IntermediateGateway.NullValue).LogicalAnd(parentContextLocal.InequalTo(this.classContextField.GetReference().RightComment("If we started parsing at a collapse point..."))));
            //else
            parentContextNullCheck = parentNullCheck.If(parentContextLocal.InequalTo(IntermediateGateway.NullValue));
            parentContextNullCheck
                .Assign(this.classParentField.GetReference(), this.compiler.RuleSymbolBuilder.CreateRuleImpl.GetReference(parentContextLocal.GetReference()).Invoke());
            classParent.GetMethod.Return(this.classParentField.GetReference());
        }

        private void BuildClassCtor()
        {
            var ctor = this.resultClass.Constructors.Add(
                new TypedName("ruleContext", this.compiler.RuleSymbolBuilder.ILanguageRuleSymbol));
            ctor.Assign(this.classContext.GetReference(), ctor.Parameters["ruleContext"].GetReference());
            ctor.AccessLevel = AccessLevelModifiers.Public;
            this.ClassConstructor = ctor;
        }

        private IIntermediateClassFieldMember BuildClassContextField()
        {
            return resultClass.Fields.Add(new TypedName("context", this.interfaceContext.PropertyType));
        }

        private IIntermediateClassFieldMember BuildClassParentField()
        {
            return resultClass.Fields.Add(new TypedName("parent", this.interfaceParent.PropertyType));
        }

        private IIntermediateClassPropertyMember BuildClassParent()
        {
            var classParent = this.resultClass.Properties.Add(new TypedName(this.interfaceParent.Name, this.interfaceParent.PropertyType), true, false);
            return classParent;
        }

        private IIntermediateClassPropertyMember BuildClassContext()
        {
            var classContext = this.resultClass.Properties.Add(new TypedName(this.interfaceContext.Name, this.interfaceContext.PropertyType), true, true);
            classContext.AccessLevel = AccessLevelModifiers.Public;
            classContext.SummaryText = string.Format(@"Returns/sets the @s:{0}; which denotes the parse stack context of the @s:{1}; as the parser progressed.", this.commonSymbol.ILanguageRuleSymbol.Name, this.resultInterface.Name);
            classContext.RemarksText = string.Format(@"@para;The ancestor of a given @s:{0}; may differ from the direct @s:Parent;.@/para;@para;This is due to the context folding of rules which merely represent a series of alternations between one or more rules.@/para;", this.resultInterface.Name);
            classContext.GetMethod.Return(this.classContextField.GetReference());
            classContext.SetMethod.Assign(this.classContextField, classContext.SetMethod.ValueParameter);
            classContext.SetMethod.AccessLevel = AccessLevelModifiers.Private;
            return classContext;
        }

        private IIntermediateInterfacePropertyMember BuildContext()
        {
            var result = this.resultInterface.Properties.Add(new TypedName("Context", this.commonSymbol.ILanguageRuleSymbol), true, false);
            result.SummaryText = string.Format(@"Returns the @s:{0}; which denotes the parse stack context of the @s:{1}; as the parser progressed.", this.commonSymbol.ILanguageRuleSymbol.Name, this.resultInterface.Name);
            result.RemarksText = string.Format(@"@para;The ancestor of a given @s:{0}; may differ from the direct @s:Parent;.@/para;@para;This is due to the context folding of rules which merely represent a series of alternations between one or more rules.@/para;", this.resultInterface.Name);
            return result;
        }

        private IIntermediateInterfacePropertyMember BuildParent()
        {
            var result = this.resultInterface.Properties.Add(new TypedName("Parent", this.resultInterface), true, false);
            result.SummaryText = string.Format(@"Returns the @s:{0}; which contains the current @s:{0};.", this.resultInterface.Name);
            result.RemarksText = @"May be null if there was no ancestor in the context stack at the time of parse.";
            return result;
        }

        public IIntermediateInterfaceType ILanguageRule { get { return this.resultInterface; } }
        public IIntermediateClassType LanguageRuleRoot { get { return this.resultClass; } }

        public IIntermediateClassCtorMember ClassConstructor { get; set; }

        public IIntermediateClassPropertyMember TokenDerived_Token { get; set; }
    }
}
