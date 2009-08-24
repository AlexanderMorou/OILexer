using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Reflection;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for a generic simple method signature with a
    /// type-insert for the parent.
    /// </summary>
    /// <typeparam name="TParameter">The type of parameter used by the <typeparamref name="TParent"/></typeparam>
    /// <typeparam name="TDom">The type of <see cref="CodeObject"/> the <see cref="IParameteredDeclaration{TParameter, TParameteredDom, TParent}"/> yields.</typeparam>
    /// <typeparam name="TParent">The type that represents the container of the 
    /// <see cref="IParameteredDeclaration{TParameter, TParameteredDom, TParent}"/>.</typeparam>
    public interface IParameteredDeclaration<TParameter, TDom, TParent> :
        IDeclarationTarget
        where TParameter :
            IParameteredParameterMember<TParameter, TDom, TParent>
        where TParent :
            IDeclarationTarget
        where TDom :
            CodeObject
    {
        /// <summary>
        /// Returns the <see cref="IParameteredParameterMembers{TParameter, TParameteredDom, TParent}"/>
        /// </summary>
        IParameteredParameterMembers<TParameter, TDom, TParent> Parameters { get; }
    }
}
