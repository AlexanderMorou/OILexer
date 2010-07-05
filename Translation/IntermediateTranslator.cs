using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Expression;
using Oilexer.Statements;

namespace Oilexer.Translation
{
    /// <summary>
    /// Provides a shell intermediate translator for objectified code,
    /// implementing basic routines for refining general cases into
    /// specific ones.
    /// </summary>
    public abstract class IntermediateTranslator :
        IIntermediateTranslator
    {

        #region IIntermediateTranslator Members

        public abstract void TranslateAttribute(AttributeTargets target, IAttributeDeclarationTarget attributeSource, IAttributeDeclaration attribute);

        public abstract void TranslateAttribute(IAttributeDeclarationTarget attributeSource, IAttributeDeclaration attribute);

        public virtual void TranslateAttributes(AttributeTargets target, IAttributeDeclarationTarget attributeSource, IAttributeDeclarations attributes)
        {
            if ((attributeSource is ISegmentableDeclarationTarget) && (!((ISegmentableDeclarationTarget)attributeSource).IsRoot))
                return;
            foreach (IAttributeDeclaration iad in attributes)
                TranslateAttribute(target, attributeSource, iad);
        }

        public virtual void TranslateAttributes(IAttributeDeclarationTarget attributeSource, IAttributeDeclarations attributes)
        {
            if ((attributeSource is ISegmentableDeclarationTarget) && (!((ISegmentableDeclarationTarget)attributeSource).IsRoot))
                return;
            foreach (IAttributeDeclaration iad in attributes)
                TranslateAttribute(attributeSource, iad);
        }

        public void TranslateConstraints(ITypeParameterMember typeParamMember)
        {
            if (typeParamMember is IMethodSignatureTypeParameterMember)
            {
                this.TranslateConstraints((IMethodSignatureTypeParameterMember)typeParamMember);
            }
            else if (typeParamMember is IMethodTypeParameterMember)
            {
                this.TranslateConstraints((IMethodTypeParameterMember)typeParamMember);
            }
            else if (typeParamMember is ITypeParameterMember<CodeTypeDelegate>)
            {
                this.TranslateConstraints<CodeTypeParameter, IDeclaredType<CodeTypeDelegate>>((ITypeParameterMember<CodeTypeDelegate>)typeParamMember);
            }
            else if (typeParamMember is ITypeParameterMember<CodeTypeDeclaration>)
            {
                this.TranslateConstraints((ITypeParameterMember<CodeTypeDeclaration>)typeParamMember);
            }
            else
                throw new NotSupportedException("Unknown type-parameter.");
        }

        public void TranslateConstraints<TParameter, TTypeParameter, TSignatureDom, TParent>(IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethSigTypeParamMember)
            where TParameter :
                IParameteredParameterMember<TParameter, TSignatureDom, TParent>
            where TTypeParameter :
                IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
            where TSignatureDom :
                CodeMemberMethod,
                new()
            where TParent :
                IDeclarationTarget
        {
            this.TranslateConstraints<CodeTypeParameter, IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent>>(ambigMethSigTypeParamMember);
        }

        public virtual void TranslateConstraints(ITypeParameterMember<CodeTypeDeclaration> ambigTypeParameter)
        {
            TranslateConstraints<CodeTypeParameter, IDeclaredType<CodeTypeDeclaration>>(ambigTypeParameter);
        }

        public abstract void TranslateConstraints<TDom, TParent>(ITypeParameterMember<TDom, TParent> ambigTypeParamMember)
            where TDom :
                CodeTypeParameter,
                new()
            where TParent :
                IDeclaration;

        public virtual void TranslateMembers(IMemberParentType parent, IConstructorMembers ctorMembers)
        {
            this.TranslateMembers<IConstructorMember, IMemberParentType, CodeConstructor>(parent, ctorMembers);
        }

        public virtual void TranslateMembers(IMemberParentType parent, IPropertyMembers propertyMembers)
        {
            this.TranslateMembers<IPropertyMember, IMemberParentType>(parent, propertyMembers);
        }

        public virtual void TranslateMembers(ISignatureMemberParentType parent, IPropertySignatureMembers propertySigMembers)
        {
            this.TranslateMembers<IPropertySignatureMember, ISignatureMemberParentType>(parent, propertySigMembers);
        }

        public virtual void TranslateMembers(IMemberParentType parent, IMethodMembers methodMembers)
        {
            this.TranslateMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>(parent, methodMembers);
        }
        public abstract void TranslateMembers<TParameter, TTypeParameter, TSignatureDom, TParent>(TParent parent, IMethodSignatureMembers<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethodSigMembers)
            where TParameter :
                IParameteredParameterMember<TParameter, TSignatureDom, TParent>
            where TTypeParameter :
                IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
            where TSignatureDom :
                CodeMemberMethod,
                new()
            where TParent :
                IDeclarationTarget;

        public virtual void TranslateMembers(ISignatureMemberParentType parent, IMethodSignatureMembers methodSigMembers)
        {
            this.TranslateMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>(parent, methodSigMembers);
        }

        public virtual void TranslateMemberParentTypeMembers(IMemberParentType parent)
        {
            TranslateMembers(parent, parent.Fields);
            TranslateMembers(parent, parent.Properties);
            TranslateMembers(parent, parent.Methods);
            TranslateMembers(parent, parent.Constructors);
            TranslateMembers(parent, parent.Coercions);
        }

        public virtual void TranslateSignatureMemberParentTypeMembers(ISignatureMemberParentType parent)
        {
            TranslateMembers(parent, parent.Properties);
            TranslateMembers(parent, parent.Methods);
        }

        public abstract void TranslateMembers(IMemberParentType parent, IExpressionCoercionMembers coercionMembers);

        void IIntermediateTranslator.TranslateMember<TOperator>(IOperatorOverloadMember<TOperator> operatorOverloadMember)
        {
            this.TranslateMember<TOperator>(operatorOverloadMember);
        }

        public abstract void TranslateMember<TOperator>(IOperatorOverloadMember<TOperator> operatorOverloadMember)
            where TOperator :
                struct;

        public abstract void TranslateMember(IUnaryOperatorOverloadMember unaryMember);

        public abstract void TranslateMember(IBinaryOperatorOverloadMember binaryMember);

        public abstract void TranslateMember(ITypeConversionOverloadMember typeConversionMember);

        public abstract void TranslateConceptComment(string commentBase, bool docComment);


        /// <summary>
        /// Translates a general-case declared type.
        /// </summary>
        /// <param name="declaredType">The <see cref="IDeclaredType"/> to translate.</param>
        public virtual void TranslateType(IDeclaredType declaredType)
        {
            if (declaredType is IClassType)
                this.TranslateType((IClassType)declaredType);
            else if (declaredType is IEnumeratorType)
                this.TranslateType((IEnumeratorType)declaredType);
            else if (declaredType is IDelegateType)
                this.TranslateType((IDelegateType)declaredType);
            else if (declaredType is IInterfaceType)
                this.TranslateType((IInterfaceType)declaredType);
            else if (declaredType is IStructType)
                this.TranslateType((IStructType)declaredType);
            else
            {
                throw new NotSupportedException(string.Format("Type {0} not supported", declaredType.GetType().Name));
            }
        }
        public abstract void TranslateTypeParentTypes(ITypeParent parent);
        /// <summary>
        /// Translates a class-based declared type.
        /// </summary>
        /// <param name="classType">The <see cref="IClassType"/> to translate.</param>
        public abstract void TranslateType(IClassType classType);

        /// <summary>
        /// Translates an enumerator-based declared type.
        /// </summary>
        /// <param name="enumeratorType">The <see cref="IEnumeratorType"/> to translate.</param>
        public abstract void TranslateType(IEnumeratorType enumeratorType);

        /// <summary>
        /// Translates a delegate-based declared type.
        /// </summary>
        /// <param name="delegateType">The <see cref="IDelegateType"/> to translate.</param>
        public abstract void TranslateType(IDelegateType delegateType);

        /// <summary>
        /// Translates an interface-based declared type.
        /// </summary>
        /// <param name="interfaceType">The <see cref="IInterfaceType"/> to translate.</param>
        public abstract void TranslateType(IInterfaceType interfaceType);

        /// <summary>
        /// Translates a structure-based declared type.
        /// </summary>
        /// <param name="structureType">The <see cref="IStructType"/> to translate.</param>
        public abstract void TranslateType(IStructType structureType);

        /// <summary>
        /// Translates an <see cref="IIntermediateProject"/>.
        /// </summary>
        /// <param name="project">The <see cref="IIntermediateProject"/> to translate.</param>
        /// <remarks>Depending on the <see cref="Options"/>, this will translate all declared namespaces 
        /// or just those on the current partial.</remarks>
        public abstract void TranslateProject(IIntermediateProject project);

        /// <summary>
        /// Translates a namespace declaration.
        /// </summary>
        /// <param name="nameSpace">The <see cref="INameSpaceDeclaration"/> to translate.</param>
        public abstract void TranslateNameSpace(INameSpaceDeclaration nameSpace);

        public virtual void TranslateMember(IMember member)
        {
            if (member is IMethodMember)
                this.TranslateMember((IMethodMember)member);
            else if (member is IMethodSignatureMember)
                this.TranslateMember((IMethodSignatureMember)member);
            else if (member is IPropertyMember)
                this.TranslateMember((IPropertyMember)member);
            else if (member is IPropertySignatureMember)
                this.TranslateMember((IPropertySignatureMember)member);
            else if (member is IFieldMember)
                this.TranslateMember((IFieldMember)member);
            else if (member is IConstructorMember)
                this.TranslateMember((IConstructorMember)member);
            else
            {
                throw new NotSupportedException("Member not supported.");
            }
        }
        public abstract void TranslateMembers<TItem, TParent>(TParent parent, IPropertySignatureMembers<TItem, TParent> ambigPropertySigMembers)
            where TItem : 
                IPropertySignatureMember<TParent>
            where TParent : 
                IDeclarationTarget;

        public virtual void TranslateMembers(IEnumeratorType parent, IEnumTypeFieldMembers fieldMembers)
        {
            TranslateMembers((IFieldParentType)parent, (IFieldMembersBase)fieldMembers);
        }

        public abstract void TranslateMembers(IFieldParentType parent, IFieldMembers fieldMembers);

        public virtual void TranslateMembers(IFieldParentType parent, IFieldMembersBase fieldMembers)
        {
            TranslateMembers<IFieldMember, IFieldParentType, CodeMemberField>(parent, fieldMembers);
        }

        public abstract void TranslateMembers<TItem, TParent, TDom>(TParent parent, IMembers<TItem, TParent, TDom> members)
            where TItem :
                IMember<TParent, TDom>
            where TParent :
                IDeclarationTarget
            where TDom :
                CodeObject;

        public abstract void TranslateTypeParameters(ITypeParameterMembers typeParameterMembers);

        public virtual void TranslateParameters(IMethodParameterMembers methodParameters)
        {
            this.TranslateParameters<IMethodParameterMember, CodeMemberMethod, IMemberParentType>(methodParameters);
        }

        public virtual void TranslateParameters(IMethodSignatureParameterMembers methodParameters)
        {
            this.TranslateParameters<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType>(methodParameters);
        }

        public virtual void TranslateParameters(IConstructorParameterMembers ctorParameters)
        {
            TranslateParameters<IConstructorParameterMember, CodeConstructor, IMemberParentType>(ctorParameters);
        }

        public virtual void TranslateParameters(IIndexerParameterMembers indexerParameters)
        {
            TranslateParameters<IIndexerParameterMember, CodeMemberProperty, IIndexerMember>(indexerParameters);
        }

        public virtual void TranslateParameters(IIndexerSignatureParameterMembers indexerParameters)
        {
            this.TranslateParameters<IIndexerSignatureParameterMember, CodeMemberProperty, IIndexerSignatureMember>(indexerParameters);
        }

        public abstract void TranslateParameters<TParameter, TParameteredDom, TParent>(IParameteredParameterMembers<TParameter, TParameteredDom, TParent> parameterMembers)
            where TParameter :
                IParameteredParameterMember<TParameter, TParameteredDom, TParent>
            where TParameteredDom :
                CodeObject
            where TParent :
                IDeclarationTarget;

        public abstract void TranslateNameSpaces(INameSpaceParent parent, INameSpaceDeclarations nameSpaces);

        public virtual void TranslateTypes(ITypeParent parent, IClassTypes classTypes)
        {
            TranslateTypes<IClassType, CodeTypeDeclaration>(parent, classTypes);
        }

        public virtual void TranslateTypes(ITypeParent parent, IDelegateTypes delegateTypes)
        {
            TranslateTypes<IDelegateType, CodeTypeDelegate>(parent, delegateTypes);
        }

        public virtual void TranslateTypes(ITypeParent parent, IEnumeratorTypes enumTypes)
        {
            TranslateTypes<IEnumeratorType, CodeTypeDeclaration>(parent, enumTypes);
        }

        public virtual void TranslateTypes(ITypeParent parent, IInterfaceTypes interfaceTypes)
        {
            TranslateTypes<IInterfaceType, CodeTypeDeclaration>(parent, interfaceTypes);
        }

        public virtual void TranslateTypes(ITypeParent parent, IStructTypes structTypes)
        {
            TranslateTypes<IStructType, CodeTypeDeclaration>(parent, structTypes);
        }

        public abstract void TranslateTypes<TItem, TDom>(ITypeParent parent, IDeclaredTypes<TItem, TDom> ambigTypes)
            where TItem :
                IDeclaredType<TDom>
            where TDom :
                CodeTypeDeclaration;

        public virtual void TranslateMember(IMethodMember methodMember)
        {
            this.TranslateMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>(methodMember);
        }

        public virtual void TranslateMember(IMethodSignatureMember methodSigMember)
        {
            this.TranslateMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>(methodSigMember);
        }

        public abstract void TranslateMember<TParameter, TTypeParameter, TSignatureDom, TParent>(IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethodSigMember)
            where TParameter :
                IParameteredParameterMember<TParameter, TSignatureDom, TParent>
            where TTypeParameter :
                IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
            where TSignatureDom :
                CodeMemberMethod,
                new()
            where TParent :
                IDeclarationTarget;

        public virtual void TranslateMember(IPropertySignatureMember propertySigMember)
        {
            if (propertySigMember is IIndexerSignatureMember)
            {
                this.TranslateMember((IIndexerSignatureMember)propertySigMember);
                return;
            }
            this.TranslateMember<ISignatureMemberParentType>(propertySigMember);
        }

        public abstract void TranslateMember(IIndexerSignatureMember indexerSigMember);

        public virtual void TranslateMember(IPropertyMember propertyMember)
        {
            if (propertyMember is IIndexerMember)
            {
                this.TranslateMember((IIndexerMember)propertyMember);
                return;
            }
            this.TranslateMember<IMemberParentType>(propertyMember);
        }

        public abstract void TranslateMember(IIndexerMember indexerMember);

        public abstract void TranslateMember<TParent>(IPropertySignatureMember<TParent> ambigPropertySigMember)
            where TParent :
                IDeclarationTarget;

        public abstract void TranslateMember(IFieldMember fieldMember);

        public abstract void TranslateMember(IConstructorMember constructorMember);

        public virtual void TranslateMember(IIndexerParameterMember indexerParamMember)
        {
            this.TranslateMember<IIndexerParameterMember, CodeMemberProperty, IIndexerMember>(indexerParamMember);
        }

        public void TranslateMember(IIndexerSignatureParameterMember indexerSigParamMember)
        {
            this.TranslateMember<IIndexerSignatureParameterMember, CodeMemberProperty, IIndexerSignatureMember>(indexerSigParamMember);
        }

        public virtual void TranslateMember<TItem, TSignatureDom, TParent>(IIndexerSignatureParameterMember<TItem, TSignatureDom, TParent> ambigIndexerSigParamMember)
            where TItem :
                IIndexerSignatureParameterMember<TItem, TSignatureDom, TParent>
            where TSignatureDom :
                CodeMemberProperty
            where TParent :
                IDeclarationTarget
        {
            this.TranslateMember<TItem, TSignatureDom, TParent>((IParameteredParameterMember<TItem, TSignatureDom, TParent>)ambigIndexerSigParamMember);
        }

        public virtual void TranslateMember(IMethodParameterMember methodParamMember)
        {
            this.TranslateMember<IMethodParameterMember, CodeMemberMethod, IMemberParentType>(methodParamMember);
        }

        public virtual void TranslateMember(IMethodSignatureParameterMember methodSigParamMember)
        {
            this.TranslateMember<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType>(methodSigParamMember);
        }

        public virtual void TranslateMember(IConstructorParameterMember constructorParamMember)
        {
            this.TranslateMember<IConstructorParameterMember, CodeConstructor, IMemberParentType>(constructorParamMember);
        }

        public virtual void TranslateMember(IDelegateTypeParameterMember delegateParamMember)
        {
            this.TranslateMember<IDelegateTypeParameterMember, CodeTypeDelegate, ITypeParent>(delegateParamMember);
        }


        public abstract void TranslateMember<TParameter, TParameteredDom, TParent>(IParameteredParameterMember<TParameter, TParameteredDom, TParent> ambigParamMember)
            where TParameter :
                IParameteredParameterMember<TParameter, TParameteredDom, TParent>
            where TParameteredDom :
                CodeObject
            where TParent :
                IDeclarationTarget;

        public virtual void TranslateMember(ITypeParameterMember typeParamMember)
        {
            if (typeParamMember is IMethodSignatureTypeParameterMember)
            {
                this.TranslateMember((IMethodSignatureTypeParameterMember)typeParamMember);
            }
            else if (typeParamMember is IMethodTypeParameterMember)
            {
                this.TranslateMember((IMethodTypeParameterMember)typeParamMember);
            }
            else if (typeParamMember is ITypeParameterMember<CodeTypeDelegate>)
            {
                this.TranslateMember((ITypeParameterMember<CodeTypeDelegate>)typeParamMember);
            }
            else if (typeParamMember is ITypeParameterMember<CodeTypeDeclaration>)
            {
                this.TranslateMember((ITypeParameterMember<CodeTypeDeclaration>)typeParamMember);
            }
        }

        public virtual void TranslateMember<TParameter, TTypeParameter, TSignatureDom, TParent>(IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethSigTypeParamMember)
            where TParameter :
                IParameteredParameterMember<TParameter, TSignatureDom, TParent>
            where TTypeParameter :
                IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
            where TSignatureDom :
                CodeMemberMethod,
                new()
            where TParent :
                IDeclarationTarget
        {
            this.TranslateMember<CodeTypeParameter, IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent>>(ambigMethSigTypeParamMember);
        }

        public abstract void TranslateMember<TDom, TParent>(ITypeParameterMember<TDom, TParent> typeParamMember)
            where TDom :
                CodeTypeParameter,
                new()
            where TParent :
                IDeclaration;

        public virtual void TranslateMember(ITypeParameterMember<CodeTypeDeclaration> ambigTypeParameter)
        {
            TranslateMember<CodeTypeParameter, IDeclaredType<CodeTypeDeclaration>>(ambigTypeParameter);
        }

        public virtual void TranslateStatement(IStatement statement)
        {
            if (statement is IAssignStatement)
                this.TranslateStatement((IAssignStatement)statement);
            else if (statement is IBreakTargetExitPoint)
                this.TranslateStatement((IBreakTargetExitPoint)statement);
            else if (statement is IBreakStatement)
                this.TranslateStatement((IBreakStatement)statement);
            else if (statement is ICommentStatement)
                this.TranslateStatement((ICommentStatement)statement);
            else if (statement is IConditionStatement)
                this.TranslateStatement((IConditionStatement)statement);
            else if (statement is IConditionStatement)
                this.TranslateStatement((IConditionStatement)statement);
            else if (statement is IForRangeStatement)
                this.TranslateStatement((IForRangeStatement)statement);
            else if (statement is IGoToLabelStatement)
                this.TranslateStatement((IGoToLabelStatement)statement);
            else if (statement is ILabelStatement)
                this.TranslateStatement((ILabelStatement)statement);
            else if (statement is IEnumeratorStatement)
                this.TranslateStatement((IEnumeratorStatement)statement);
            else if (statement is ISwitchStatement)
                this.TranslateStatement((ISwitchStatement)statement);
            else if (statement is IReturnStatement)
                this.TranslateStatement((IReturnStatement)statement);
            else if (statement is ISimpleStatement)
                this.TranslateStatement((ISimpleStatement)statement);
            else if (statement is ICrementStatement)
                this.TranslateStatement((ICrementStatement)statement);
            else if (statement is IIterationStatement)
                this.TranslateStatement((IIterationStatement)statement);
            else if (statement is ILocalDeclarationStatement)
                this.TranslateStatement((ILocalDeclarationStatement)statement);
            else if (statement is IBlockStatement)
                this.TranslateStatement((IBlockStatement)statement);
            else if (statement is IYieldStatement)
                this.TranslateStatement((IYieldStatement)statement);
            else if (statement is IYieldBreakStatement)
                this.TranslateStatement((IYieldBreakStatement)statement);
            else
                throw new NotSupportedException(string.Format("Statement {0} unknown.", statement.GetType().FullName));
        }

        public abstract void TranslateStatement(IAssignStatement assignStatement);

        public abstract void TranslateStatement(IBreakTargetExitPoint breakTarget);

        public abstract void TranslateStatement(IBreakStatement breakStatement);

        public abstract void TranslateStatement(ICommentStatement commentStatement);

        public abstract void TranslateStatement(IConditionStatement ifThenStatement);

        public abstract void TranslateStatement(ISwitchStatement switchStatement);

        public abstract void TranslateStatement(IEnumeratorStatement enumStatement);

        public abstract void TranslateStatement(IForRangeStatement forRangeStatement);

        public abstract void TranslateStatement(IGoToLabelStatement gotoLabelStatement);

        public abstract void TranslateStatement(IIterationStatement iterationStatement);

        public abstract void TranslateStatement(ILabelStatement labelStatement);

        public abstract void TranslateStatement(ILocalDeclarationStatement localDeclare);

        public abstract void TranslateStatement(IReturnStatement returnStatement);

        public abstract void TranslateStatement(ISimpleStatement callMethodStatement);

        public virtual void TranslateStatementBlock(IStatementBlock statementBlock)
        {
            foreach (IStatementBlockLocalMember isblm in statementBlock.Locals.Values)
                if (isblm.AutoDeclare)
                    TranslateStatement(isblm.GetDeclarationStatement());
            foreach (IStatement ist in statementBlock)
                this.TranslateStatement(ist);
        }

        public virtual void TranslateExpression(IExpression expression)
        {
            if (expression is IArrayIndexerExpression)
                TranslateExpression((IArrayIndexerExpression)expression);
            else if (expression is IBinaryOperationExpression)
                TranslateExpression((IBinaryOperationExpression)expression);
            else if (expression is ICastExpression)
                TranslateExpression((ICastExpression)expression);
            else if (expression is ICreateArrayExpression)
                TranslateExpression((ICreateArrayExpression)expression);
            else if (expression is ICreateNewObjectExpression)
                TranslateExpression((ICreateNewObjectExpression)expression);
            else if (expression is IDirectionExpression)
                TranslateExpression((IDirectionExpression)expression);
            else if (expression is IEventReferenceExpression)
                TranslateExpression((IEventReferenceExpression)expression);
            else if (expression is IFieldReferenceExpression)
                TranslateExpression((IFieldReferenceExpression)expression);
            else if (expression is IGetResourceExpression)
                TranslateExpression((IGetResourceExpression)expression);
            else if (expression is IIndexerReferenceExpression)
                TranslateExpression((IIndexerReferenceExpression)expression);
            else if (expression is ILocalReferenceExpression)
                TranslateExpression((ILocalReferenceExpression)expression);
            else if (expression is IMethodInvokeExpression)
                TranslateExpression((IMethodInvokeExpression)expression);
            else if (expression is IMethodReferenceExpression)
                TranslateExpression((IMethodReferenceExpression)expression);
            else if (expression is IParameterReferenceExpression)
                TranslateExpression((IParameterReferenceExpression)expression);
            else if (expression is IPrimitiveExpression)
                TranslateExpression((IPrimitiveExpression)expression);
            else if (expression is IPropertyReferenceExpression)
                TranslateExpression((IPropertyReferenceExpression)expression);
            else if (expression is IPropertySetValueReferenceExpression)
                TranslateExpression((IPropertySetValueReferenceExpression)expression);
            else if (expression is IThisReferenceExpression)
                TranslateExpression((IThisReferenceExpression)expression);
            else if (expression is IBaseReferenceExpression)
                TranslateExpression((IBaseReferenceExpression)expression);//*/
            else if (expression is ITypeOfExpression)
                TranslateExpression((ITypeOfExpression)expression);
            else if (expression is ITypeReferenceExpression)
                TranslateExpression((ITypeReferenceExpression)expression);
            else if (expression is IUnaryOperationExpression)
                TranslateExpression((IUnaryOperationExpression)expression);
            else if (expression is IWrappingExpression)
                TranslateExpression(((IWrappingExpression)(expression)).Reference);
            else
                throw new NotSupportedException("Expression type not supported.");
        }

        public abstract void TranslateExpression(IArrayIndexerExpression arrayIndexerExpression);

        public abstract void TranslateExpression(IBaseReferenceExpression baseRefExpression);

        public abstract void TranslateExpression(IBinaryOperationExpression binOpExpression);

        public abstract void TranslateExpression(ICastExpression castExpression);

        public abstract void TranslateExpression(ICreateArrayExpression expression);

        public abstract void TranslateExpression(ICreateNewObjectExpression expression);

        public abstract void TranslateExpression(IDirectionExpression directionExpression);

        public abstract void TranslateExpression(IEventReferenceExpression eventReferenceExpression);

        public abstract void TranslateExpression(IFieldReferenceExpression fieldRefExpression);

        public abstract void TranslateExpression(IGetResourceExpression getResourceExpression);

        public abstract void TranslateExpression(IIndexerReferenceExpression indexerRefExpression);

        public abstract void TranslateExpression(ILocalReferenceExpression localRefExpression);

        public abstract void TranslateExpression(IMethodInvokeExpression methodInvExpression);

        public abstract void TranslateExpression(IMethodReferenceExpression methodRefExpression);

        public abstract void TranslateExpression(IParameterReferenceExpression paramRefExpression);

        public abstract void TranslateExpression(IPrimitiveExpression primitiveExpression);

        public abstract void TranslateExpression(IPropertyReferenceExpression propRefExpression);

        public abstract void TranslateExpression(IPropertySetValueReferenceExpression propSetValRefExpression);

        public abstract void TranslateExpression(IThisReferenceExpression thisRefExpression);

        public abstract void TranslateExpression(ITypeOfExpression typeOfExpression);

        public abstract void TranslateExpression(ITypeReferenceExpression typeRefExpression);

        public abstract void TranslateExpressionGroup(IExpressionCollection expressions);

        public abstract void TranslateTypeReferenceCollection(ITypeReferenceCollection typeRefCol);

        public abstract void TranslateExpression(IUnaryOperationExpression unOpExpression);

        public abstract void TranslateStatement(ICrementStatement crementStatement);

        public abstract void TranslateStatement(IBlockStatement blockStatement);

        public abstract void TranslateStatement(IYieldStatement yieldStatement);

        public abstract void TranslateStatement(IYieldBreakStatement breakStatement);

        #endregion
    }
}
