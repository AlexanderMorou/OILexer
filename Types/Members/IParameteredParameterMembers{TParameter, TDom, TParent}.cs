using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a series of <typeparamref name="TParameter"/>s.
    /// </summary>
    /// <typeparam name="TParameter">The type of parameters that are inserted into the 
    /// <see cref="IParameteredParameterMembers{TParameter, TParameteredDom, TParent}"/>.</typeparam>
    /// <typeparam name="TParent">The type of parent which holds the method signatures.</typeparam>
    public interface IParameteredParameterMembers<TParameter, TParameteredDom, TParent> :
        IMembers<TParameter, IParameteredDeclaration<TParameter, TParameteredDom, TParent>, CodeParameterDeclarationExpression>,
        IAutoCommentFragmentMembers
        where TParameter :
            IParameteredParameterMember<TParameter, TParameteredDom, TParent>
        where TParent :
            IDeclarationTarget
        where TParameteredDom :
            CodeObject
    {
        /// <summary>
        /// Inserts a new <typeparamref name="TParameter"/> into the 
        /// <see cref="IParameteredParameterMembers{TParameter, TParameteredDom, TParent}"/> with the <paramref name="name"/>
        /// and <paramref name="paramType"/> provided.
        /// </summary>
        /// <param name="name">The name of the new <typeparamref name="TParameter"/>.</param>
        /// <param name="paramType">The type of the new <typeparamref name="TParameter"/>.</param>
        /// <returns>A new instance of <typeparamref name="TParameter"/>.</returns>
        TParameter AddNew(string name, ITypeReference paramType);
        /// <summary>
        /// Inserts a new <typeparamref name="TParameter"/> into the 
        /// <see cref="IParameteredParameterMembers{TParameter, TParameteredDom, TParent}"/> with the <paramref name="data"/>
        /// provided.
        /// </summary>
        /// <param name="data">The information about the parameter.</param>
        /// <returns>A new instance of <typeparamref name="TParameter"/>.</returns>
        TParameter AddNew(TypedName data);
    }
}
