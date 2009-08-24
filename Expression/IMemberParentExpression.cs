using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for a member which can contain sub-elements of its own.
    /// </summary>
    public interface IMemberParentExpression :
        IExpression
    {
        /// <summary>
        /// Obtains a <see cref="IMethodReferenceExpression"/>.
        /// </summary>
        /// <param name="name">The name of the method expression.</param>
        /// <param name="typeArguments">The <see cref="System.Type"/> type-arguments of the method expression.</param>
        /// <returns>A new <see cref="IMethodReferenceExpression"/> if successful.</returns>
        IMethodReferenceExpression GetMethod(string name, Type[] typeArguments);
        /// <summary>
        /// Obtains a <see cref="IMethodReferenceExpression"/>.
        /// </summary>
        /// <param name="name">The name of the method expression.</param>
        /// <param name="typeArguments">The <see cref="IType"/> type-arguments of the method expression.</param>
        /// <returns>A new <see cref="IMethodReferenceExpression"/> if successful.</returns>
        IMethodReferenceExpression GetMethod(string name, params ITypeReference[] typeArguments);
        /// <summary>
        /// Obtains a <see cref="IFieldReferenceExpression"/> which can contain members of its own.
        /// </summary>
        /// <param name="name">The name of the member to retrieve.</param>
        /// <returns>An instance of an implementation of an <see cref="IFieldReferenceExpression"/>.</returns>
        IFieldReferenceExpression GetField(string name);
        /// <summary>
        /// Obtains a <see cref="IPropertyReferenceExpression"/> which can contain members of its own.
        /// </summary>
        /// <param name="name">The name of the member to retrieve.</param>
        /// <returns>An instance of an implementation of an <see cref="IPropertyReferenceExpression"/>.</returns>
        IPropertyReferenceExpression GetProperty(string name);
        IIndexerReferenceExpression GetIndex(params object[] parameters);
    }
}
