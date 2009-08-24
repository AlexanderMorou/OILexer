using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a type's constructors.
    /// </summary>
    public interface IConstructorMembers :
        IMembers<IConstructorMember, IMemberParentType, CodeConstructor>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IConstructorMembers"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="TargetDeclaration"/> which
        /// needs a <see cref="IConstructorMembers"/> implementation instance.</param>
        /// <returns>A new <see cref="IConstructorMembers"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IConstructorMembers GetPartialClone(IMemberParentType parent);
        /// <summary>
        /// Inserts a new <see cref="IConstructorMember"/> implementation with the 
        /// <paramref name="cascadeMembers"/>, <paramref name="cascadeExpressionsTarget"/>,
        /// and <paramref name="parameters"/> provided.
        /// </summary>
        /// <param name="cascadeMembers">The series of <see cref="IExpression"/>s which
        /// are used to invoke the next constructor.</param>
        /// <param name="cascadeExpressionsTarget">The target of the constructor cascade.</param>
        /// <param name="parameters">A series of parameters that the constructor uses.</param>
        /// <returns>A new instance of an implementation of <see cref="IConstructorMember"/>.</returns>
        IConstructorMember AddNew(IExpressionCollection cascadeMembers, ConstructorCascadeTarget cascadeExpressionsTarget, params TypedName[] parameters);
        /// <summary>
        /// Inserts a new <see cref="IConstructorMember"/> implementation with the 
        /// <paramref name="parameters"/> provided.
        /// </summary>
        /// <param name="parameters">A series of parameters that the constructor uses.</param>
        /// <returns>A new instance of an implementation of <see cref="IConstructorMember"/>.</returns>
        IConstructorMember AddNew(params TypedName[] parameters);
        new void Add(IConstructorMember constructor);
    }
}
