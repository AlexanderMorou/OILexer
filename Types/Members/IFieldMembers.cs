using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a series of field members.
    /// </summary>
    public interface IFieldMembers :
        IFieldMembersBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IFieldMembers"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="IMembers{TItem, TParent, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IFieldMembers"/> implementation instance.</param>
        /// <returns>A new <see cref="IFieldMembers"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IFieldMembers GetPartialClone(IFieldParentType parent);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndType">The name and return-type of the <see cref="IFieldMember"/>.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(TypedName nameAndType);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndType">The name and return-type of the <see cref="IFieldMember"/>.</param>
        /// <param name="initializationExpression">The initializer for the <see cref="IFieldMember"/> created.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(TypedName nameAndType, IExpression initializationExpression);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, bool initValue);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, char initValue);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, string initValue);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, float initValue);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, double initValue);
    }
}
