using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with an enumerator that will be 
    /// translated into a <see cref="CodeTypeDeclaration"/>
    /// </summary>
    public interface IEnumeratorType :
        IDeclaredType<CodeTypeDeclaration>,
        IFieldParentType
    {
        /// <summary>
        /// Returns/sets the base type of the <see cref="IEnumeratorType"/>.
        /// </summary>
        EnumeratorBaseType BaseType { get; set; }
        /// <summary>
        /// Returns the fields that make up the enumerator.
        /// </summary>
        new IEnumTypeFieldMembers Fields { get; }

        IFieldMember GetMemberByName(string memberName);
    }
}
