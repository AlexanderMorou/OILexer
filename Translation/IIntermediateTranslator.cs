using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Expression;
using Oilexer.Types.Members;
using Oilexer.Statements;
using System.CodeDom;

namespace Oilexer.Translation
{
    public interface IIntermediateTranslator
    {
        /// <summary>
        /// Translates a single <see cref="IAttributeDeclaration"/> onto the declaration of <paramref name="attributeSource"/>.
        /// </summary>
        /// <param name="attributeSource">The <see cref="IAttributeDeclarationTarget"/> which
        /// contains the <see cref="IAttributeDeclaration"/>.</param>
        /// <remarks>The <paramref name="attributeSource"/> is used to reduce the
        /// emission based upon whether the target is segmentable.
        /// To prevent errors on non-multi-use attributes, partial targets have attributes 
        /// on the root instance only.</remarks>
        void TranslateAttribute(IAttributeDeclarationTarget attributeSource, IAttributeDeclaration attribute);

        /// <summary>
        /// Translates a single <see cref="IAttributeDeclaration"/> onto the <paramref name="attributeSource"/>
        /// using the <paramref name="target"/> to identify the special type of <paramref name="attribute"/>.
        /// </summary>
        /// <param name="target">The <see cref="AttributeTargets"/> which designates the target of the attribute.</param>
        /// <param name="attributeSource">The <see cref="IAttributeDeclarationTarget"/> which designates
        /// the source of the attribute.</param>
        /// <param name="attribute">The <see cref="IAttributeDeclaration"/> to translate.</param>
        void TranslateAttribute(AttributeTargets target, IAttributeDeclarationTarget attributeSource, IAttributeDeclaration attribute);

        /// <summary>
        /// Translates a series of <see cref="IAttributeDeclarations"/>
        /// onto the <paramref name="attributeSource"/> of the given <paramref name="target"/> type.
        /// </summary>
        /// <param name="target">The <see cref="AttributeTargets"/> which designates the target of the attribute.</param>
        /// <param name="attributeSource">The <see cref="IAttributeDeclarationTarget"/> which designates
        /// the source of the attribute.</param>
        /// <param name="attributes">The <see cref="IAttributeDeclarations"/> to translate.</param>
        void TranslateAttributes(AttributeTargets target, IAttributeDeclarationTarget attributeSource, IAttributeDeclarations attributes);

        /// <summary>
        /// Translates a series of <see cref="IAttributeDeclaration"/> instances given the
        /// target point.
        /// </summary>
        /// <param name="attributeSource">The <see cref="IAttributeDeclarationTarget"/> which
        /// contains the <see cref="IAttributeDeclarations"/>.</param>
        /// <param name="attributes">The series of <see cref="IAttributeDeclaration"/> instances to be translated.</param>
        /// <remarks>The <paramref name="attributeSource"/> is used to reduce the
        /// emission based upon whether the target is segmentable.
        /// To prevent errors on non-multi-use attributes, partial targets have attributes 
        /// on the root instance only.</remarks>
        void TranslateAttributes(IAttributeDeclarationTarget attributeSource, IAttributeDeclarations attributes);

        /// <summary>
        /// Translates a generic expression.
        /// </summary>
        /// <param name="expression">The expression to translate.</param>
        void TranslateExpression(IExpression expression);

        /// <summary>
        /// Translates an array indexer expression.  That is an expression that accesses the
        /// <see cref="IArrayIndexerExpression.Reference"/> as if it were an array.
        /// </summary>
        /// <param name="arrayIndexerExpression">The <see cref="IArrayIndexerExpression"/> to translate.</param>
        void TranslateExpression(IArrayIndexerExpression arrayIndexerExpression);

        /// <summary>
        /// Translates a binary operation expression, or an expression that has a <see cref="IBinaryOperationExpression.LeftSide"/> and
        /// <see cref="IBinaryOperationExpression.RightSide"/> demarcated by the operator <see cref="IBinaryOperationExpression.Operation"/>
        /// in standard implementations.
        /// </summary>
        /// <param name="binOpExpression">The <see cref="IBinaryOperationExpression"/> to translate.</param>
        void TranslateExpression(IBinaryOperationExpression binOpExpression);

        /// <summary>
        /// Translates a unary operation expression, or an expression that has a <see cref="IUnaryOperationExpression.TargetExpression"/> which has 
        /// the operator <see cref="IUnaryOperationExpression.Operation"/> applied to it.
        /// in standard implementations.
        /// </summary>
        /// <param name="unOpExpression">The <see cref="IUnaryOperationExpression"/> to translate.</param>
        void TranslateExpression(IUnaryOperationExpression unOpExpression);

        /// <summary>
        /// Translates an expression used to cast another expression to the desired type.
        /// </summary>
        /// <param name="castExpression">The <see cref="ICastExpression"/> to translate.</param>
        void TranslateExpression(ICastExpression castExpression);

        /// <summary>
        /// Translates an expression used to instanciate an array instance.
        /// </summary>
        /// <param name="expression">The <see cref="ICreateArrayExpression"/> to translate.</param>
        void TranslateExpression(ICreateArrayExpression expression);

        /// <summary>
        /// Translates an expression used to create an object instance.
        /// </summary>
        /// <param name="expression">The <see cref="ICreateNewObjectExpression"/> to be
        /// translated.</param>
        void TranslateExpression(ICreateNewObjectExpression expression);

        /// <summary>
        /// Translates an expression used to direct a parameter in a method invoke parameter
        /// list.
        /// </summary>
        /// <param name="directionExpression">The <see cref="IDirectionExpression"/> to be
        /// translated.</param>
        void TranslateExpression(IDirectionExpression directionExpression);

        /// <summary>
        /// Translates an expression used to reference a property or field event.
        /// </summary>
        /// <param name="eventReferenceExpression">The <see cref="IEventReferenceExpression"/>
        /// to be translated.</param>
        void TranslateExpression(IEventReferenceExpression eventReferenceExpression);

        /// <summary>
        /// Translates an expression used to reference a field.
        /// </summary>
        /// <param name="fieldRefExpression">The <see cref="IFieldReferenceExpression"/> to be
        /// translated.</param>
        void TranslateExpression(IFieldReferenceExpression fieldRefExpression);

        /// <summary>
        /// Translates an expression used to retrieve a resource from the project or
        /// [current] type resources.
        /// </summary>
        /// <param name="getResourceExpression"></param>
        void TranslateExpression(IGetResourceExpression getResourceExpression);

        /// <summary>
        /// Translates an expression used to reference and access an instance or type indexer.
        /// </summary>
        /// <param name="indexerRefExpression">The <see cref="IIndexerReferenceExpression"/> to 
        /// be translated.</param>
        void TranslateExpression(IIndexerReferenceExpression indexerRefExpression);

        /// <summary>
        /// Translates an expression used to reference a local (method) member.
        /// </summary>
        /// <param name="localRefExpression">The <see cref="ILocalReferenceExpression"/> to be
        /// translated.</param>
        void TranslateExpression(ILocalReferenceExpression localRefExpression);

        /// <summary>
        /// Translates an expression used to invoke a method.
        /// </summary>
        /// <param name="methodInvExpression">The <see cref="IMethodInvokeExpression"/> to be
        /// translated.</param>
        void TranslateExpression(IMethodInvokeExpression methodInvExpression);

        /// <summary>
        /// Translates an expression used to reference a method.
        /// </summary>
        /// <param name="methodRefExpression">The <see cref="IMethodReferenceExpression"/> to be
        /// translated.</param>
        void TranslateExpression(IMethodReferenceExpression methodRefExpression);

        /// <summary>
        /// Translates an expression used to reference a parameter.
        /// </summary>
        /// <param name="paramRefExpression">The <see cref="IParameterReferenceExpression"/> to be
        /// translated.</param>
        void TranslateExpression(IParameterReferenceExpression paramRefExpression);

        /// <summary>
        /// Translates a primitive [literal] expression.
        /// </summary>
        /// <param name="primitiveExpression">The <see cref="IPrimitiveExpression"/> to be 
        /// translated.</param>
        void TranslateExpression(IPrimitiveExpression primitiveExpression);

        /// <summary>
        /// Translates an expression used to reference a property.
        /// </summary>
        /// <param name="propRefExpression">The <see cref="IPropertyReferenceExpression"/> to be
        /// translated.</param>
        void TranslateExpression(IPropertyReferenceExpression propRefExpression);

        /// <summary>
        /// Translates an expression used to refer to the value parameter used in a property set method.
        /// </summary>
        /// <param name="propSetValRefExpression">The <see cref="IPropertySetValueReferenceExpression"/> to be
        /// translated.</param>
        void TranslateExpression(IPropertySetValueReferenceExpression propSetValRefExpression);

        /// <summary>
        /// Translates an expression used to refer to the current instanciable instance.
        /// </summary>
        /// <param name="thisRefExpression">The <see cref="IThisReferenceExpression"/> to be
        /// translated.</param>
        void TranslateExpression(IThisReferenceExpression thisRefExpression);

        void TranslateExpression(IBaseReferenceExpression baseRefExpression);

        /// <summary>
        /// Translates an expression used to return the <see cref="System.Type"/> of an identifier
        /// that resolves to a type.
        /// </summary>
        /// <param name="typeOfExpression">The <see cref="ITypeOfExpression"/> to be
        /// translated.</param>
        void TranslateExpression(ITypeOfExpression typeOfExpression);

        /// <summary>
        /// Translates an expression that refers to a <see cref="System.Type"/>.
        /// </summary>
        /// <param name="typeRefExpression">The <see cref="ITypeReferenceExpression"/> to be translated.</param>
        void TranslateExpression(ITypeReferenceExpression typeRefExpression);

        /// <summary>
        /// Translates an expression collection.
        /// </summary>
        /// <param name="expressions">The series of <see cref="IExpression"/> instance implementations
        /// to be translated.</param>
        void TranslateExpressionGroup(IExpressionCollection expressions);


        /// <summary>
        /// Translates a series of <see cref="IConstructorMember"/> instances pertinent to the 
        /// <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The parent of the constructors.  Used in prior step to determine
        /// proper target and simplifies hierarchy access.</param>
        /// <param name="ctorMembers">The series of <see cref="IConstructorMember"/> instances
        /// which needs translated.</param>
        void TranslateMembers(IMemberParentType parent, IConstructorMembers ctorMembers);

        /// <summary>
        /// Translates a series of <see cref="IMethodMember"/> instances pertinent to the 
        /// <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The parent of the methods.  Used in prior step to determine
        /// proper target and simplifies hierarchy access.</param>
        /// <param name="methodMembers">The series of <see cref="IMethodMember"/> instances
        /// which needs translated.</param>
        void TranslateMembers(IMemberParentType parent, IMethodMembers methodMembers);

        /// <summary>
        /// Translates a series of <see cref="IMethodSignatureMember"/> instances pertinent to the 
        /// <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The parent of the methods.  Used in prior step to determine
        /// proper target and simplifies hierarchy access.</param>
        /// <param name="methodSigMembers">The series of <see cref="IMethodSignatureMember"/> instances
        /// which needs translated.</param>
        void TranslateMembers(ISignatureMemberParentType parent, IMethodSignatureMembers methodSigMembers);

        /// <summary>
        /// Translates a series of <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> instances pertinent to the 
        /// <paramref name="parent"/>.
        /// </summary>
        /// <typeparam name="TParameter">The type of parameters used.</typeparam>
        /// <typeparam name="TTypeParameter">The type of type-parameters used.</typeparam>
        /// <typeparam name="TSignatureDom">The <see cref="CodeMemberMethod"/> derived
        /// object the <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> yields.</typeparam>
        /// <typeparam name="TParent">The type of parent that contains the signatures.</typeparam>
        /// <param name="parent">The parent of the methods.  Used in prior step to determine
        /// proper target and simplifies hierarchy access.</param>
        /// <param name="ambigMethodSigMembers">The series of <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> instances
        /// which needs translated.</param>
        void TranslateMembers<TParameter, TTypeParameter, TSignatureDom, TParent>(TParent parent, IMethodSignatureMembers<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethodSigMembers)
            where TParameter :
                IParameteredParameterMember<TParameter, TSignatureDom, TParent>
            where TTypeParameter :
                IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
            where TParent :
                IDeclarationTarget
            where TSignatureDom :
                CodeMemberMethod,
                new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="propertyMembers"></param>
        void TranslateMembers(IMemberParentType parent, IPropertyMembers propertyMembers);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="propertySigMembers"></param>
        void TranslateMembers(ISignatureMemberParentType parent, IPropertySignatureMembers propertySigMembers);

        /// <summary>
        /// Translates a series of <see cref="IExpressionCoercionMember"/> instances that belong to the 
        /// <paramref name="parent"/> provided.
        /// </summary>
        /// <param name="parent">The <see cref="IMemberTypeParent"/> that contains the <paramref name="coercionMembers"/>.</param>
        /// <param name="coercionMembers">The <see cref="IExpressionCoercionMembers"/> to translate.</param>
        void TranslateMembers(IMemberParentType parent, IExpressionCoercionMembers coercionMembers);

        void TranslateMember<TOperator>(IOperatorOverloadMember<TOperator> operatorOverloadMember)
            where TOperator :
                struct;

        void TranslateMember(IUnaryOperatorOverloadMember unaryMember);
        void TranslateMember(IBinaryOperatorOverloadMember binaryMember);
        void TranslateMember(ITypeConversionOverloadMember typeConversionMember);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="parent"></param>
        /// <param name="ambigPropertySigMembers"></param>
        void TranslateMembers<TItem, TParent>(TParent parent, IPropertySignatureMembers<TItem, TParent> ambigPropertySigMembers)
            where TItem :
                IPropertySignatureMember<TParent>
            where TParent :
                IDeclarationTarget;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fieldMembers"></param>
        void TranslateMembers(IEnumeratorType parent, IEnumTypeFieldMembers fieldMembers);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fieldMembers"></param>
        void TranslateMembers(IFieldParentType parent, IFieldMembers fieldMembers);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fieldMembers"></param>
        void TranslateMembers(IFieldParentType parent, IFieldMembersBase fieldMembers);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TDom"></typeparam>
        /// <param name="parent"></param>
        /// <param name="members"></param>
        void TranslateMembers<TItem, TParent, TDom>(TParent parent, IMembers<TItem, TParent, TDom> members)
            where TItem :
                IMember<TParent, TDom>
            where TParent :
                IDeclarationTarget
            where TDom :
                CodeObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        void TranslateMemberParentTypeMembers(IMemberParentType parent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        void TranslateSignatureMemberParentTypeMembers(ISignatureMemberParentType parent);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeParameterMembers"></param>
        void TranslateTypeParameters(ITypeParameterMembers typeParameterMembers);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TParameteredDom"></typeparam>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="parameterMembers"></param>
        void TranslateParameters<TParameter, TParameteredDom, TParent>(IParameteredParameterMembers<TParameter, TParameteredDom, TParent> parameterMembers)
            where TParameter :
                IParameteredParameterMember<TParameter, TParameteredDom, TParent>
            where TParent :
                IDeclarationTarget
            where TParameteredDom :
                CodeObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodParameters"></param>
        void TranslateParameters(IMethodParameterMembers methodParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodParameters"></param>
        void TranslateParameters(IMethodSignatureParameterMembers methodParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctorParameters"></param>
        void TranslateParameters(IConstructorParameterMembers ctorParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexerParameters"></param>
        void TranslateParameters(IIndexerParameterMembers indexerParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexerParameters"></param>
        void TranslateParameters(IIndexerSignatureParameterMembers indexerParameters);

        void TranslateNameSpaces(INameSpaceParent parent, INameSpaceDeclarations nameSpaces);
        void TranslateTypes(ITypeParent parent, IClassTypes classTypes);
        void TranslateTypes(ITypeParent parent, IDelegateTypes delegateTypes);
        void TranslateTypes(ITypeParent parent, IEnumeratorTypes enumTypes);
        void TranslateTypes(ITypeParent parent, IInterfaceTypes interfaceTypes);
        void TranslateTypes(ITypeParent parent, IStructTypes structTypes);
        void TranslateTypes<TItem, TDom>(ITypeParent parent, IDeclaredTypes<TItem, TDom> ambigTypes)
            where TItem :
                IDeclaredType<TDom>
            where TDom :
                CodeTypeDeclaration;

        void TranslateTypeParentTypes(ITypeParent parent);

        /// <summary>
        /// Translates a series of <see cref="ITypeReference"/> instances to their string form.
        /// </summary>
        /// <param name="typeRefCol">The series of <see cref="ITypeReference"/> instances to 
        /// translate.</param>
        void TranslateTypeReferenceCollection(ITypeReferenceCollection typeRefCol);

        /* *
         * The member items
         * */
        //General
        void TranslateMember(IMember member);

        //Method
        void TranslateMember(IMethodMember methodMember);
        void TranslateMember(IMethodSignatureMember methodSigMember);
        void TranslateMember<TParameter, TTypeParameter, TSignatureDom, TParent>(IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethodSigMember)
            where TParameter :
                IParameteredParameterMember<TParameter, TSignatureDom, TParent>
            where TTypeParameter :
                IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
            where TParent :
                IDeclarationTarget
            where TSignatureDom :
                CodeMemberMethod,
                new();

        //Property
        void TranslateMember(IPropertySignatureMember propertySigMember);
        void TranslateMember(IIndexerSignatureMember indexerSigMember);
        void TranslateMember(IPropertyMember propertyMember);
        void TranslateMember(IIndexerMember indexerMember);
        void TranslateMember<TParent>(IPropertySignatureMember<TParent> ambigPropertySigMember)
            where TParent :
                IDeclarationTarget;
        //Field
        void TranslateMember(IFieldMember fieldMember);

        //Event (not here yet!)

        //Constructors
        void TranslateMember(IConstructorMember constructorMember);

        /* *
         * Sub-members
         * */
        void TranslateMember(IIndexerParameterMember indexerParamMember);
        void TranslateMember(IIndexerSignatureParameterMember indexerSigParamMember);
        void TranslateMember<TItem, TSignatureDom, TParent>(IIndexerSignatureParameterMember<TItem, TSignatureDom, TParent> ambigIndexerSigParamMember)
            where TItem :
                IIndexerSignatureParameterMember<TItem, TSignatureDom, TParent>
            where TSignatureDom :
                CodeMemberProperty
            where TParent :
                IDeclarationTarget;

        void TranslateMember(IMethodParameterMember methodParamMember);
        void TranslateMember(IMethodSignatureParameterMember methodSigParamMember);
        void TranslateMember(IConstructorParameterMember constructorParamMember);
        void TranslateMember(IDelegateTypeParameterMember delegateParamMember);


        void TranslateMember<TParameter, TParameteredDom, TParent>(IParameteredParameterMember<TParameter, TParameteredDom, TParent> ambigParamMember)
            where TParameter :
                IParameteredParameterMember<TParameter, TParameteredDom, TParent>
            where TParent :
                IDeclarationTarget
            where TParameteredDom :
                CodeObject;

        void TranslateMember(ITypeParameterMember typeParamMember);

        void TranslateMember<TParameter, TTypeParameter, TSignatureDom, TParent>(IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethSigTypeParamMember)
            where TParameter :
                IParameteredParameterMember<TParameter, TSignatureDom, TParent>
            where TTypeParameter :
                IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
            where TParent :
                IDeclarationTarget
            where TSignatureDom :
                CodeMemberMethod,
                new();

        void TranslateMember(ITypeParameterMember<CodeTypeDeclaration> ambigTypeParameter);

        void TranslateMember<TDom, TParent>(ITypeParameterMember<TDom, TParent> ambigTypeParamMember)
            where TParent :
                IDeclaration
            where TDom :
                CodeTypeParameter,
                new();

        void TranslateConstraints(ITypeParameterMember typeParamMember);

        void TranslateConstraints<TParameter, TTypeParameter, TSignatureDom, TParent>(IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent> ambigMethSigTypeParamMember)
            where TParameter :
                IParameteredParameterMember<TParameter, TSignatureDom, TParent>
            where TTypeParameter :
                IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
            where TParent :
                IDeclarationTarget
            where TSignatureDom :
                CodeMemberMethod,
                new();

        void TranslateConstraints(ITypeParameterMember<CodeTypeDeclaration> ambigTypeParameter);

        void TranslateConstraints<TDom, TParent>(ITypeParameterMember<TDom, TParent> ambigTypeParamMember)
            where TParent :
                IDeclaration
            where TDom :
                CodeTypeParameter,
                new();


        /* *
         * The large container items...
         * */
        /// <summary>
        /// Translates a general-case declared type.
        /// </summary>
        /// <param name="declaredType">The <see cref="IDeclaredType"/> to translate.</param>
        void TranslateType(IDeclaredType declaredType);
        /// <summary>
        /// Translates a class-based declared type.
        /// </summary>
        /// <param name="classType">The <see cref="IClassType"/> to translate.</param>
        void TranslateType(IClassType classType);
        /// <summary>
        /// Translates an enumerator-based declared type.
        /// </summary>
        /// <param name="enumeratorType">The <see cref="IEnumeratorType"/> to translate.</param>
        void TranslateType(IEnumeratorType enumeratorType);
        /// <summary>
        /// Translates a delegate-based declared type.
        /// </summary>
        /// <param name="delegateType">The <see cref="IDelegateType"/> to translate.</param>
        void TranslateType(IDelegateType delegateType);
        /// <summary>
        /// Translates an interface-based declared type.
        /// </summary>
        /// <param name="interfaceType">The <see cref="IInterfaceType"/> to translate.</param>
        void TranslateType(IInterfaceType interfaceType);
        /// <summary>
        /// Translates a structure-based declared type.
        /// </summary>
        /// <param name="structureType">The <see cref="IStructType"/> to translate.</param>
        void TranslateType(IStructType structureType);
        /// <summary>
        /// Translates an <see cref="IIntermediateProject"/> as a whole or 
        /// partial by partial.
        /// </summary>
        /// <param name="project">The <see cref="IIntermediateProject"/> to translate.</param>
        /// <remarks>Depending on the <see cref="Options"/>, this will translate all declared namespaces 
        /// or just those on the current partial.</remarks>
        void TranslateProject(IIntermediateProject project);
        /// <summary>
        /// Translates a namespace declaration.
        /// </summary>
        /// <param name="nameSpace">The <see cref="INameSpaceDeclaration"/> to translate.</param>
        /// <remarks>Depending on the <see cref="Options"/>, this will translate all of the declared 
        /// child namespaces and the current <see cref="INameSpaceDeclaration"/>'s types or just 
        /// those on the current partial.</remarks>
        void TranslateNameSpace(INameSpaceDeclaration nameSpace);

        /* *
         * Statements
         * */
        /// <summary>
        /// Translates a general-case statement.
        /// </summary>
        /// <param name="statement">The <see cref="IStatement"/> implementation to 
        /// toss to the appropriate <see cref="TranslateStatement(IStatement)"/> method.</param>
        void TranslateStatement(IStatement statement);
        /// <summary>
        /// Translates an assignment statement.
        /// </summary>
        /// <param name="assignStatement">The <see cref="IAssignStatement"/> which contains the 
        /// <see cref="IAssignStatement.Reference"/> to set to the <see cref="IAssignStatement.Value"/>.</param>
        void TranslateStatement(IAssignStatement assignStatement);

        /// <summary>
        /// Translates a block statement.
        /// </summary>
        /// <param name="blockStatement">The <see cref="IBlockStatement"/> to translate.</param>
        void TranslateStatement(IBlockStatement blockStatement);

        /// <summary>
        /// Translates a break point's target.
        /// </summary>
        /// <param name="breakTarget">The <see cref="IBreakTargetExitPoint"/>.</param>
        /// <remarks>Used for compatability with the old CodeDom.</remarks>
        void TranslateStatement(IBreakTargetExitPoint breakTarget);
        /// <summary>
        /// Translates a break statement that exists loops, switches, and so on.
        /// </summary>
        /// <param name="breakStatement">The <see cref="IBreakStatement"/> that exits the current 
        /// code-block that has a <see cref="IBreakTargetExitPoint"/>.</param>
        void TranslateStatement(IBreakStatement breakStatement);
        /// <summary>
        /// Translates a comment statement.
        /// </summary>
        /// <param name="commentStatement">The <see cref="ICommentStatement"/> that represents
        /// a mention to what the code does.</param>
        void TranslateStatement(ICommentStatement commentStatement);
        /// <summary>
        /// Translates a standard case logical if/then statement.
        /// </summary>
        /// <param name="ifThenStatement">The <see cref="IConditionStatement"/> which 
        /// manages the flow of the execution.</param>
        void TranslateStatement(IConditionStatement ifThenStatement);
        /// <summary>
        /// Translates a standard switch/[select case] statement.
        /// </summary>
        /// <param name="switchStatement">The <see cref="ISwitchStatement"/> which
        /// handles large constant based cases.</param>
        void TranslateStatement(ISwitchStatement switchStatement);
        /// <summary>
        /// Translates an enumerator statement that iterates an <see cref="IEnumerable"/>
        /// entity.
        /// </summary>
        /// <param name="enumStatement">The <see cref="IEnumeratorStatement"/> that
        /// is to be translated.</param>
        void TranslateStatement(IEnumeratorStatement enumStatement);
        /// <summary>
        /// Translates a crement operation statement that increments/decrements an <see cref="IAssignStatementTarget"/>
        /// </summary>
        /// <param name="crementStatement">The <see cref="ICrementStatement"/> that
        /// is to be translated.</param>
        void TranslateStatement(ICrementStatement crementStatement);
        /// <summary>
        /// Translates a for range statement which is an iteration statement that has 
        /// a clearly defined start and end and optional step as well.
        /// </summary>
        /// <param name="forRangeStatement">The <see cref="IForRangeStatement"/> to be translated.</param>
        void TranslateStatement(IForRangeStatement forRangeStatement);
        /// <summary>
        /// Translates a go-to statement which transfers the execution point to the label defined
        /// in the <see cref="IGoToLabelStatement"/>.
        /// </summary>
        /// <param name="gotoLabelStatement">The <see cref="IGoToLabelStatement"/></param>
        void TranslateStatement(IGoToLabelStatement gotoLabelStatement);
        /// <summary>
        /// Translates an iteration statement which allows for a more complex variant of <see cref="TranslateStatement(IForRangeStatement)"/>.
        /// </summary>
        /// <param name="iterationStatement"></param>
        void TranslateStatement(IIterationStatement iterationStatement);
        /// <summary>
        /// Translates a local declaration statement.
        /// </summary>
        /// <param name="localDeclare">The <see cref="ILocalDeclarationStatement"/> which is to be defined 
        /// at the current point.</param>
        void TranslateStatement(ILocalDeclarationStatement localDeclare);
        /// <summary>
        /// Translates a label statement which marks a point at which execution can be forwarded to.
        /// </summary>
        /// <param name="labelStatement">The <see cref="ILabelStatement"/> to translate.</param>
        void TranslateStatement(ILabelStatement labelStatement);
        /// <summary>
        /// Translates a method return statement which can exit the procedure on non-returning methods
        /// or yield a return value (<see cref="IReturnStatement.Result"/>).
        /// </summary>
        /// <param name="returnStatement">The <see cref="IReturnStatement"/> to translate.</param>
        void TranslateStatement(IReturnStatement returnStatement);
        /// <summary>
        /// Translates a simple method call statement.
        /// </summary>
        /// <param name="callMethodStatement">The <see cref="ISimpleStatement"/> to translate.</param>
        void TranslateStatement(ISimpleStatement callMethodStatement);
        /// <summary>
        /// Translates an iterator yield statement.
        /// </summary>
        /// <param name="yieldStatement">The <see cref="IYieldStatement"/> which
        /// handles large constant based cases.</param>
        void TranslateStatement(IYieldStatement yieldStatement);
        /// <summary>
        /// Translates an iterator yield break statement.
        /// </summary>
        /// <param name="breakStatement">The <see cref="breakStatement"/> which
        /// handles large constant based cases.</param>
        void TranslateStatement(IYieldBreakStatement breakStatement);

        /* *
         * Statement container
         * */
        /// <summary>
        /// Translates a series of <see cref="IStatement"/> instances.
        /// </summary>
        /// <param name="statementBlock">The <see cref="IStatementBlock"/> to translate.</param>
        void TranslateStatementBlock(IStatementBlock statementBlock);

        /* *
         * Concepts/Common Keywords
         * */

    }
}
