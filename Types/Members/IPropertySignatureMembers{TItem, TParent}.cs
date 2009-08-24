using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for a generic that works
    /// with a type's property and indexer members.
    /// </summary>
    /// <typeparam name="TItem">The type of properties inserted into the 
    /// <see cref="IPropertySignatureMembers"/>.</typeparam>
    /// <typeparam name="TParent">The type of parent which contains the <see cref="IPopertySignatureMembers"/>.</typeparam>
    public interface IPropertySignatureMembers<TItem, TParent> :
        IMembers<TItem, TParent, CodeMemberProperty>
        where TItem :
            IPropertySignatureMember<TParent>
        where TParent :
            IDeclarationTarget
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IPropertySignatureMembers{TItem, TParent}"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="IMembers{TItem, TParent, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IPropertySignatureMembers{TItem, TParent}"/> implementation instance.</param>
        /// <returns>A new <see cref="IPropertySignatureMembers{TItem, TParent}"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IPropertySignatureMembers<TItem, TParent> GetPartialClone(TParent parent);
        /// <summary>
        /// Creates, inserts and returns a new <typeparamref name="TItem"/> with the 
        /// <paramref name="name"/> and <paramref name="propertyType"/> provided.
        /// </summary>
        /// <param name="name">The name of the property signature.</param>
        /// <param name="propertyType">The type the property signature yields.</param>
        /// <returns>A new instance of <typeparamref name="TItem"/> containing the 
        /// <paramref name="name"/> and <paramref name="propertyType"/>.</returns>
        /// <exception cref="System.ArgumentNullException">when <paramref name="propertyType"/> is null.</exception>
        TItem AddNew(string name, ITypeReference propertyType);
        /// <summary>
        /// Creates, inserts and returns a new <typeparamref name="TItem"/> with the 
        /// <paramref name="nameAndType"/> provided.
        /// </summary>
        /// <param name="nameAndType">The name and type of the property signature.</param>
        /// <returns>A new instance of <typeparamref name="TItem"/> containing the 
        /// <paramref name="nameAndType"/> given.</returns>
        TItem AddNew(TypedName nameAndType);
        /// <summary>
        /// Creates, inserts and returns a new <typeparamref name="TItem"/> with the 
        /// <paramref name="name"/>, <paramref name="propertyType"/>, 
        /// <paramref name="hasGet"/>, and <paramref name="hasSet"/> provided.
        /// </summary>
        /// <param name="name">The name of the property signature.</param>
        /// <param name="propertyType">The type the property signature yields.</param>
        /// <param name="hasGet">Whether the property has a get method area.</param>
        /// <param name="hasSet">Whether the property has a set method area.</param>
        /// <returns>A new instance of <typeparamref name="TItem"/> containing the 
        /// <paramref name="name"/>, <paramref name="propertyType"/>, 
        /// <paramref name="hasGet"/>, and <paramref name="hasSet"/>.</returns>
        /// <exception cref="System.ArgumentNullException">when <paramref name="propertyType"/> is null.</exception>
        TItem AddNew(string name, ITypeReference propertyType, bool hasGet, bool hasSet);
        /// <summary>
        /// Creates, inserts and returns a new <typeparamref name="TItem"/> with the 
        /// <paramref name="nameAndType"/>, 
        /// <paramref name="hasGet"/>, and <paramref name="hasSet"/> provided.
        /// </summary>
        /// <param name="nameAndType">The name and type of the property signature.</param>
        /// <param name="hasGet">Whether the property has a get method area.</param>
        /// <param name="hasSet">Whether the property has a set method area.</param>
        /// <param name="nameAndType">The name and type of the property signature.</param>
        /// <returns>A new instance of <typeparamref name="TItem"/> containing the 
        /// <paramref name="nameAndType"/>, <paramref name="hasGet"/>, and 
        /// <paramref name="hasSet"/> given.</returns>
        TItem AddNew(TypedName nameAndType, bool hasGet, bool hasSet);
        new void Add(TItem ipm);
    }
}
