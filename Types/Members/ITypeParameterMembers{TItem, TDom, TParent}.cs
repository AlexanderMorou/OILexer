using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a type-strict dictionary
    /// of named <typeparamref name="TItem"/> type-parameters.
    /// </summary>
    /// <typeparam name="TItem">The type of <see cref="ITypeParameterMember{TDom, TParent}"/> contained
    /// within the <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>.</typeparam>
    /// <typeparam name="TDom">The type of <see cref="CodeTypeParameter"/> which 
    /// <typeparamref name="TItem"/> instances yield.</typeparam>
    /// <typeparam name="TParent">The type of parent the <typeparamref name="TItem"/> instances
    /// will belong to.</typeparam>
    public interface ITypeParameterMembers<TItem, TDom, TParent> :
        IMembers<TItem, TParent, TDom>,
        IAutoCommentFragmentMembers
        where TItem :
            ITypeParameterMember<TDom, TParent>
        where TDom :
            CodeTypeParameter,
            new()
        where TParent :
            IDeclaration
    {
        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/>.</returns>
        TItem AddNew(string name);

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/> 
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>.</returns>
        TItem AddNew(string name, bool requiresConstructor);

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/> and 
        /// <paramref name="constraints"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// constraints as expressed by <paramref name="constraints"/>..</returns>
        TItem AddNew(string name, ITypeReferenceCollection constraints);

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/>, <paramref name="constraints"/>,
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>
        /// and additional constraints as expressed by <paramref name="constraints"/>.</returns>
        TItem AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor);
        /// <summary>
        /// Inserts a new <typeparamref name="TItem"/> into the <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>
        /// with the <paramref name="name"/>, <paramref name="constraints"/>, and <paramref name="requiresConstructor"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the new  <typeparamref name="TItem"/></param>
        /// <param name="constraints">The constraints of the new <typeparamref name="TItem"/></param>
        /// <param name="requiresConstructor">Whether the new <typeparamref name="TItem"/>
        /// has a null-parameter constructor as a condition.</param>
        /// <returns>A new instance of the type-parameter created and inserted.</returns>
        TItem AddNew(string name, ITypeReference[] constraints, bool requiresConstructor);


        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/>.</returns>
        TItem AddNew(string name, TypeParameterSpecialCondition specialCondition);

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/> 
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>.</returns>
        TItem AddNew(string name, bool requiresConstructor, TypeParameterSpecialCondition specialCondition);

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/> and 
        /// <paramref name="constraints"/> provided.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// constraints as expressed by <paramref name="constraints"/>..</returns>
        TItem AddNew(string name, ITypeReferenceCollection constraints, TypeParameterSpecialCondition specialCondition);

        /// <summary>
        /// Adds a new <typeparamref name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the <paramref name="name"/>, <paramref name="constraints"/>,
        /// and <paramref name="requiresConstructor"/> provided. 
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="constraints">The type-reference constraints for the resulted <paramref name="TItem"/>.</param>
        /// <param name="requiresConstructor">Whether or not the resulted <typeparamref name="TItem"/>
        /// has a null-constructor constraint.</param>
        /// <returns>A new <typeparamref name="TItem"/> named <paramref name="name"/> and
        /// has the null constructor constraint based upon <paramref name="requiresConstructor"/>
        /// and additional constraints as expressed by <paramref name="constraints"/>.</returns>
        TItem AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition);
        /// <summary>
        /// Inserts a new <typeparamref name="TItem"/> into the <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>
        /// with the <paramref name="name"/>, <paramref name="constraints"/>, and <paramref name="requiresConstructor"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the new  <typeparamref name="TItem"/></param>
        /// <param name="constraints">The constraints of the new <typeparamref name="TItem"/></param>
        /// <param name="requiresConstructor">Whether the new <typeparamref name="TItem"/>
        /// has a null-parameter constructor as a condition.</param>
        /// <returns>A new instance of the type-parameter created and inserted.</returns>
        TItem AddNew(string name, ITypeReference[] constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition);


        /// <summary>
        /// Inserts a new <typeparamref name="TItem"/>, into the
        /// <see cref="ITypeParameterMembers{TItem, TDom, TParent}"/>, with the data provided.
        /// </summary>
        /// <param name="data">The information about the type-parameter.</param>
        /// <returns>A new instance of the type-parameter created and inserted.</returns>
        TItem AddNew(TypeConstrainedName data);

    }
}
