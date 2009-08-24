using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using Oilexer.Statements;
using System.Reflection;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a property member of a declared type
    /// </summary>
    public interface IPropertyMember :
        IPropertySignatureMember<IMemberParentType>,
        IImplementedMember,
        ICodeBodyTableMember
    {
        new IPropertyReferenceExpression GetReference();
        /// <summary>
        /// Returns the get body of the property.
        /// </summary>
        IPropertyBodyMember GetPart { get; }
        /// <summary>
        /// Returns the set body of the property.
        /// </summary>
        IPropertySetBodyMember SetPart { get; }
    }
}
