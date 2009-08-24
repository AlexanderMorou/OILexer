using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a field parent type's fields.
    /// </summary>
    public interface IFieldMembersBase :
        IMembers<IFieldMember, IFieldParentType, CodeMemberField>
    {
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, byte initValue);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, sbyte initValue);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, uint initValue);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, int initValue);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="name">The name of the new entry, the data-type will be derived from the <paramref name="initValue"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, ulong initValue);
        /// <summary>
        /// Inserts a new <see cref="IFieldMember"/> implementation into the 
        /// <see cref="IFieldMembers"/> dictionary using the <see cref="IMember.GetUniqueIdentifier()"/> as a key.
        /// </summary>
        /// <param name="nameAndReturn">The name and return-type of the <see cref="IFieldMember"/>.</param>
        /// <param name="initValue">The primitive initial value.</param>
        /// <returns>A new <see cref="IFieldMember"/> implementation with the <paramref name="nameAndReturn"/> provided.</returns>
        IFieldMember AddNew(string name, long initValue);
    }
}
