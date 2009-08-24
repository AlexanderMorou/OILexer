using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a type's property and indexer members.
    /// </summary>
    public interface IPropertyMembers :
        IPropertySignatureMembers<IPropertyMember, IMemberParentType>
    {
        /// <summary>
        /// Creates, inserts and returns a new <see cref="IIndexerMember"/> with the 
        /// <paramref name="indexerType"/> and <paramref name="indexerType"/> provided.
        /// </summary>
        /// <param name="indexerType">The type the indexer member yields.</param>
        /// <param name="parameters">The types and names of the parameters of the indexer member.</param>
        /// <returns>A new instance of <typeparamref name="TItem"/> containing the 
        /// <paramref name="indexerType"/> and <paramref name="parameters"/> given.</returns>
        IIndexerMember AddNew(ITypeReference indexerType, params TypedName[] parameters);
        /// <summary>
        /// Creates, inserts and returns a new <see cref="IIndexerMember"/> with the 
        /// <paramref name="indexerType"/>, 
        /// <paramref name="hasGet"/>, and <paramref name="hasSet"/> provided.
        /// </summary>
        /// <param name="indexerType">The type the indexer member yields.</param>
        /// <param name="hasGet">Whether the indexer member has a get method area.</param>
        /// <param name="hasSet">Whether the indexer member has a set method area.</param>
        /// <param name="parameters">The types and names of the parameters of the indexer member.</param>
        /// <returns>A new instance of <typeparamref name="TItem"/> containing the 
        /// <paramref name="indexerType"/>, <paramref name="hasGet"/>, and 
        /// <paramref name="hasSet"/> given.</returns>
        IIndexerMember AddNew(ITypeReference indexerType, bool hasGet, bool hasSet, params TypedName[] parameters);

        /// <summary>
        /// Creates, inserts and returns a new <see cref="IIndexerMember"/> with the 
        /// <paramref name="indexerType"/> and <paramref name="indexerType"/> provided.
        /// </summary>
        /// <param name="nameAndType">The type the indexer member yields and alternate indexer name.</param>
        /// <param name="parameters">The types and names of the parameters of the indexer member.</param>
        /// <returns>A new instance of <typeparamref name="TItem"/> containing the 
        /// <paramref name="nameAndType"/> and <paramref name="parameters"/> given.</returns>
        IIndexerMember AddNew(TypedName nameAndType, TypedName[] parameters);
        /// <summary>
        /// Creates, inserts and returns a new <see cref="IIndexerMember"/> with the 
        /// <paramref name="indexerType"/>, 
        /// <paramref name="hasGet"/>, and <paramref name="hasSet"/> provided.
        /// </summary>
        /// <param name="nameAndType">The type the indexer member yields and alternate indexer name.</param>
        /// <param name="hasGet">Whether the indexer member has a get method area.</param>
        /// <param name="hasSet">Whether the indexer member has a set method area.</param>
        /// <param name="parameters">The types and names of the parameters of the indexer member.</param>
        /// <returns>A new instance of <typeparamref name="TItem"/> containing the 
        /// <paramref name="nameAndType"/>, <paramref name="hasGet"/>, and 
        /// <paramref name="hasSet"/> given.</returns>
        IIndexerMember AddNew(TypedName nameAndType, bool hasGet, bool hasSet, TypedName[] parameters);

        /// <summary>
        /// Creates a new instance of the <see cref="IPropertyMembers"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="IMembers{TItem, TParent, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IPropertyMembers"/> implementation instance.</param>
        /// <returns>A new <see cref="IPropertyMembers"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IPropertyMembers GetPartialClone(IMemberParentType parent);

    }
}
