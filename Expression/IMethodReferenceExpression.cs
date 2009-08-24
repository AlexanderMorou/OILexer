using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Comments;
using System.CodeDom;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for working with a reference to a method of an 
    /// <see cref="IMemberParentType"/>.
    /// </summary>
    public interface IMethodReferenceExpression :
        IExpression<CodeMethodReferenceExpression>,
        IMemberReferenceExpression
    {
        /// <summary>
        /// Creates a <see cref="IMethodInvokeExpression"/> for the current <see cref="IMethodReferenceExpression"/>.
        /// </summary>
        /// <returns>A new <see cref="IMethodInvokeExpression"/> for the current <see cref="IMethodReferenceExpression"/>.</returns>
        IMethodInvokeExpression Invoke();
        /// <summary>
        /// Creates a <see cref="IMethodInvokeExpression"/> for the current <see cref="IMethodReferenceExpression"/>.
        /// </summary>
        /// <param name="arguments">The arguments needed to invoke the method.</param>
        /// <returns>A new <see cref="IMethodInvokeExpression"/> for the current <see cref="IMethodReferenceExpression"/>.</returns>
        /// <remarks>Primitives, expressions, field/property/method/et cetera references or instances are valid.</remarks>
        IMethodInvokeExpression Invoke(params object[] arguments);
        /// <summary>
        /// Creates a <see cref="IMethodInvokeExpression"/> for the current <see cref="IMethodReferenceExpression"/>.
        /// </summary>
        /// <param name="arguments">The arguments needed to invoke the method.</param>
        /// <returns>A new <see cref="IMethodInvokeExpression"/> for the current <see cref="IMethodReferenceExpression"/>.</returns>
        IMethodInvokeExpression Invoke(params IExpression[] arguments);
        /// <summary>
        /// Returns the type arguments used by the <see cref="IMethodReferenceExpression"/>.
        /// </summary>
        ITypeReferenceCollection TypeArguments { get; }
        new IMethodReferenceComment GetReferenceParticle();
    }
}
