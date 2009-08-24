using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using System.Reflection;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a generic-form of a method signature.
    /// </summary>
    /// <typeparam name="TParameter">The type of parameters used.</typeparam>
    /// <typeparam name="TTypeParameter">The type of type-parameters used.</typeparam>
    /// <typeparam name="TSignatureDom">The <see cref="CodeMemberMethod"/> derived
    /// object the <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> yields.</typeparam>
    /// <typeparam name="TParent">The type of parent that contains the signatures.</typeparam>
    public interface IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> :
        IParameteredDeclaration<TParameter, TSignatureDom, TParent>,
        IMember<TParent, TSignatureDom>,
        IAutoCommentMember,
        ISignatureTableMember
        //,
        //IFauxableReliant<MethodInfo, Type>
        where TParameter :
            IParameteredParameterMember<TParameter, TSignatureDom, TParent>
        where TTypeParameter :
            IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
        where TParent :
            IDeclarationTarget
        where TSignatureDom :
            CodeMemberMethod,
            new()
    {
        /// <summary>
        /// Returns/sets the <see cref="ITypeReference"/> that the method returns.
        /// </summary>
        ITypeReference ReturnType { get; set; }
        /// <summary>
        /// Returns the <see cref="IMethodSignatureTypeParameterMembers{TParameter, TTypeParameter, TSignatureDom, TParent}"/>
        /// </summary>
        IMethodSignatureTypeParameterMembers<TParameter, TTypeParameter, TSignatureDom, TParent> TypeParameters { get; }
        /// <summary>
        /// Returns/sets whether the <see cref="IMethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> is
        /// an overloaded member.
        /// </summary>
        /// <remarks>
        /// It is required by some langauges to explicitly declare overloads as such.  
        /// An example: Visual Basic.Net
        /// </remarks>
        bool IsOverload { get; set; }
    }
}
