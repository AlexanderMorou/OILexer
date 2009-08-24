using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Statements;
using Oilexer.Expression;
using Oilexer.Comments;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace Oilexer.Translation
{
    public partial class CodeDomTranslator<T> :
        IntermediateCodeTranslator,
        ICodeDomTranslator<T>
        where T :
            CodeDomProvider,
            ICodeCompiler,
            ICodeGenerator
    {
        /// <summary>
        /// Data member for <see cref="Provider"/>.
        /// </summary>
        private T provider;
        public CodeDomTranslator(T provider)
        {
            this.provider = provider;
        }

        public override void TranslateType(IClassType classType)
        {
            this.Provider.GenerateCodeFromType(classType.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateType(IEnumeratorType enumeratorType)
        {
            this.Provider.GenerateCodeFromType(enumeratorType.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateType(IDelegateType delegateType)
        {
            this.Provider.GenerateCodeFromType(delegateType.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateType(IInterfaceType interfaceType)
        {
            this.Provider.GenerateCodeFromType(interfaceType.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateType(IStructType structureType)
        {
            this.Provider.GenerateCodeFromType(structureType.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateProject(IIntermediateProject project)
        {
            this.Provider.GenerateCodeFromCompileUnit(project.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateNameSpace(INameSpaceDeclaration nameSpace)
        {
            foreach (CodeNamespace cns in nameSpace.GenerateGroupCodeDom(this.Options))
                this.Provider.GenerateCodeFromNamespace(cns, base.Target, this.Options.Options);
        }

        public override void TranslateMember<TParameter, TTypeParameter, TSignatureDom, TParent>(IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethodSigMember)
        {
            this.Provider.GenerateCodeFromMember((CodeMemberMethod)ambigMethodSigMember.GenerateCodeDom(Options), base.Target, this.Options.Options); 
        }

        public override void TranslateMember(IIndexerSignatureMember indexerSigMember)
        {
            this.Provider.GenerateCodeFromMember(indexerSigMember.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateMember(IIndexerMember indexerMember)
        {
            this.Provider.GenerateCodeFromMember(indexerMember.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateMember<TParent>(IPropertySignatureMember<TParent> ambigPropertySigMember)
        {
            this.Provider.GenerateCodeFromMember(ambigPropertySigMember.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateMember(IFieldMember fieldMember)
        {
            this.Provider.GenerateCodeFromMember(fieldMember.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateMember(IConstructorMember constructorMember)
        {
            this.Provider.GenerateCodeFromMember(constructorMember.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateMember<TParameter, TParameteredDom, TParent>(IParameteredParameterMember<TParameter, TParameteredDom, TParent> ambigParamMember)
        {
            this.Provider.GenerateCodeFromExpression(ambigParamMember.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateMember<TDom, TParent>(ITypeParameterMember<TDom, TParent> typeParamMember)
        {
            Target.Write(typeParamMember.Name);
        }

        public override void TranslateStatement(IAssignStatement assignStatement)
        {
            this.Provider.GenerateCodeFromStatement(assignStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(IBreakTargetExitPoint breakTarget)
        {
            this.Provider.GenerateCodeFromStatement(breakTarget.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(IBreakStatement breakStatement)
        {
            foreach (CodeStatement cst in breakStatement.GenerateCodeDom(this.Options))
                this.Provider.GenerateCodeFromStatement(cst, base.Target, this.Options.Options);
        }

        public override void TranslateStatement(ICommentStatement commentStatement)
        {
            this.Provider.GenerateCodeFromStatement(commentStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(IConditionStatement ifThenStatement)
        {
            this.Provider.GenerateCodeFromStatement(ifThenStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(ISwitchStatement switchStatement)
        {
            this.Provider.GenerateCodeFromStatement(switchStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(IEnumeratorStatement enumStatement)
        {
            this.Provider.GenerateCodeFromStatement(enumStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(IForRangeStatement forRangeStatement)
        {
            this.Provider.GenerateCodeFromStatement(forRangeStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(IGoToLabelStatement gotoLabelStatement)
        {
            this.Provider.GenerateCodeFromStatement(gotoLabelStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(IIterationStatement iterationStatement)
        {
            this.Provider.GenerateCodeFromStatement(iterationStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(ILabelStatement labelStatement)
        {
            this.Provider.GenerateCodeFromStatement(labelStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(ILocalDeclarationStatement localDeclare)
        {
            this.Provider.GenerateCodeFromStatement(localDeclare.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(IReturnStatement returnStatement)
        {
            this.Provider.GenerateCodeFromStatement(returnStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateStatement(ISimpleStatement callMethodStatement)
        {
            this.Provider.GenerateCodeFromStatement(callMethodStatement.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateConceptPartial(ISegmentableDeclarationTarget seggedDecl)
        {            
        }

        public override void TranslateConceptAccessModifiers(IDeclaration decl)
        {
        }

        public override void TranslateConceptNotInstantiableClass(IClassType classType)
        {
        }

        public override void TranslateConceptNotInheritableClass(IClassType classType)
        {
        }

        public override void TranslateConceptNotInheritableOrInstantiableClass(IClassType classType)
        {
        }

        public override string EscapeString(string value, int indentLevel)
        {
            throw new NotSupportedException("Original CodeDom infrastructure does not support string escaping.");
        }

        public override bool IsKeyword(string identifier)
        {
            throw new NotSupportedException("Original CodeDom infrastructure does not support querying identifiers for keyword status.");
        }

        public override string EscapeIdentifier(string identifier)
        {
            throw new NotSupportedException("Original CodeDom infrastructure does not support identifier escaping.");
        }

        public override void TranslateTypeParameters(ITypeParameterMembers typeParameterMembers)
        {
            throw new NotImplementedException();
        }

        public override void TranslateParameters<TParameter, TParameteredDom, TParent>(IParameteredParameterMembers<TParameter, TParameteredDom, TParent> parameterMembers)
        {
            throw new NotImplementedException();
        }

        public override void TranslateExpression(IArrayIndexerExpression arrayIndexerExpression)
        {
            this.Provider.GenerateCodeFromExpression(arrayIndexerExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IBaseReferenceExpression baseRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(baseRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IBinaryOperationExpression binOpExpression)
        {
            this.Provider.GenerateCodeFromExpression(binOpExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(ICastExpression castExpression)
        {
            this.Provider.GenerateCodeFromExpression(castExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(ICreateArrayExpression expression)
        {
            this.Provider.GenerateCodeFromExpression(expression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(ICreateNewObjectExpression expression)
        {
            this.Provider.GenerateCodeFromExpression(expression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IDirectionExpression directionExpression)
        {
            this.Provider.GenerateCodeFromExpression(directionExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IEventReferenceExpression eventReferenceExpression)
        {
            this.Provider.GenerateCodeFromExpression(eventReferenceExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IFieldReferenceExpression fieldRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(fieldRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IGetResourceExpression getResourceExpression)
        {
            this.Provider.GenerateCodeFromExpression(getResourceExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IIndexerReferenceExpression indexerRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(indexerRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(ILocalReferenceExpression localRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(localRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IMethodInvokeExpression methodInvExpression)
        {
            this.Provider.GenerateCodeFromExpression(methodInvExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IMethodReferenceExpression methodRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(methodRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IParameterReferenceExpression paramRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(paramRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IPrimitiveExpression primitiveExpression)
        {
            this.Provider.GenerateCodeFromExpression(primitiveExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IPropertyReferenceExpression propRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(propRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IPropertySetValueReferenceExpression propSetValRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(propSetValRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(IThisReferenceExpression thisRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(thisRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(ITypeOfExpression typeOfExpression)
        {
            this.Provider.GenerateCodeFromExpression(typeOfExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpression(ITypeReferenceExpression typeRefExpression)
        {
            this.Provider.GenerateCodeFromExpression(typeRefExpression.GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateExpressionGroup(IExpressionCollection expressions)
        {
            foreach (CodeExpression cexp in expressions.GenerateCodeDom(this.Options))
                this.Provider.GenerateCodeFromExpression(cexp, base.Target, this.Options.Options);
        }

        public override void TranslateTypeReferenceCollection(ITypeReferenceCollection typeRefCol)
        {
            throw new NotSupportedException("Cannot translate a series of type references, unsure of implementation.");
        }

        public override void TranslateConceptTypeName(IExternTypeReference type, ITypeReferenceCollection typeParameters)
        {
            this.Provider.GenerateCodeFromExpression(type.GetTypeExpression().GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateConceptTypeName(IDeclaredTypeReference type, ITypeReferenceCollection typeParameters)
        {
            this.Provider.GenerateCodeFromExpression(type.GetTypeExpression().GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateConceptTypeName(ITypeParameterMember type, ITypeReferenceCollection typeParameters)
        {
            this.Provider.GenerateCodeFromExpression(type.GetTypeReference(typeParameters).GetTypeExpression().GenerateCodeDom(this.Options), base.Target, this.Options.Options);
        }

        public override void TranslateConstraints<TDom, TParent>(ITypeParameterMember<TDom, TParent> ambigTypeParamMember)
        {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        public override void TranslateConceptComment(string commentBase, bool docComment)
        {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        public override string SubToolVersion
        {
            get
            {
                return "1.0.0.0";
            }
        }

        public override string SubToolName
        {
            get
            {
                return "Oilexer.CodeDomTranslator";
            }
        }

        public override string Language
        {
            get
            {
                return string.Format("{1} (Runtime version: {0})", this.Provider.GetType().Assembly.ImageRuntimeVersion, this.Provider.GetType().Name);
            }
        }

        public override void TranslateAttribute(IAttributeDeclarationTarget attributeSource, IAttributeDeclaration attribute)
        {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        public override void TranslateConceptRegionStart(string regionText)
        {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        public override void TranslateConceptRegionEnd(string regionText)
        {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        public override void TranslateConceptKeyword(int keyWord)
        {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        public override void TranslateComment(IDocumentationComment docComment)
        {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        #region ICodeDomTranslator<T> Members

        /// <summary>
        /// Returns the <typeparamref name="T"/> for the <see cref="CodeDomTranslator{T}"/>
        /// that manages the translation process.
        /// </summary>
        public T Provider
        {
            get
            {
                return this.provider;
            }
        }

        #endregion

        #region ICodeDomTranslator<T> Members


        public new ICodeDOMTranslationOptions Options
        {
            get
            {
                return (ICodeDOMTranslationOptions)base.Options;
            }
            set
            {
                if ((base.Options == value) || ((base.Options is MixedOptions) && ((base.Options as MixedOptions).options == value)))
                    return;
                if (base.Options is MixedOptions)
                    (base.Options as MixedOptions).Dispose();
                SetOptions(value);
            }
        }

        #endregion

        internal override void SetOptions(IIntermediateCodeTranslatorOptions options)
        {
            if (options is ICodeDOMTranslationOptions)
            {
                base.SetOptions(options);
            }
            else
            {
                this.SetOptions((IIntermediateCodeTranslatorOptions)new MixedOptions(options));
            }
        }

        private void SetOptions(ICodeDOMTranslationOptions options)
        {
            this.SetOptions((IIntermediateCodeTranslatorOptions)new MixedOptions(options));
        }

        #region ICodeDomTranslator Members

        CodeDomProvider ICodeDomTranslator.Provider
        {
            get { return this.Provider; }
        }

        #endregion

        public override void TranslateMember<TOperator>(IOperatorOverloadMember<TOperator> operatorOverloadMember)
        {
            throw new NotImplementedException();
        }

        public override void TranslateMember(IUnaryOperatorOverloadMember unaryMember)
        {
            throw new NotImplementedException();
        }

        public override void TranslateMember(IBinaryOperatorOverloadMember binaryMember)
        {
            throw new NotImplementedException();
        }

        public override void TranslateMember(ITypeConversionOverloadMember typeConversionMember)
        {
            throw new NotImplementedException();
        }

        public override void TranslateExpression(IUnaryOperationExpression unOpExpression)
        {
            throw new NotImplementedException();
        }

        public override void TranslateStatement(ICrementStatement crementStatement)
        {
            throw new NotImplementedException();
        }

        public override void TranslateAttribute(AttributeTargets target, IAttributeDeclarationTarget attributeSource, IAttributeDeclaration attribute)
        {
            throw new NotImplementedException();
        }

        public override void TranslateStatement(IBlockStatement blockStatement)
        {
            throw new NotImplementedException();
        }

        public override void TranslateStatement(IYieldStatement yieldStatement)
        {
            throw new NotImplementedException();
        }

        public override void TranslateStatement(IYieldBreakStatement breakStatement)
        {
            throw new NotImplementedException();
        }
    }
}
