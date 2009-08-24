using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a enumerator type's members.
    /// </summary>
    public interface IEnumTypeFieldMembers :
        IFieldMembersBase
    {
        /// <summary>
        /// Returns the fixed type the field members represent.
        /// </summary>
        ITypeReference EnumType { get; }
        /// <summary>
        /// Inserts a new field by name.
        /// </summary>
        /// <param name="name">The name of the new member.</param>
        /// <returns>A new field member with the <see cref="EnumType"/> and <paramref name="name"/> provided.</returns>
        IFieldMember AddNew(string name);
        /// <summary>
        /// Inserts a new field by name with an initialization expression.
        /// </summary>
        /// <param name="name">The name of the new member.</param>
        /// <param name="initExpression">The expression that initializes the field member.</param>
        /// <returns>A new field member with the <see cref="EnumType"/>, <paramref name="name"/>, and <paramref name="initExpression"/> provided.</returns>
        IFieldMember AddNew(string name, IExpression initExpression);
        /// <summary>
        /// Returns the <see cref="IEnumeratorType"/> the <see cref="IEnumTypeFieldMembers"/> belongs
        /// to.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumeratorType"/> that contains the <see cref="IEnumTypeFieldMembers"/>.
        /// </returns>
        new IEnumeratorType TargetDeclaration { get; }
    }
}
