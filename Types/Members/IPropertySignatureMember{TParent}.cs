using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for the generic signature of a property.
    /// </summary>
    /// <typeparam name="TParent">The type of parent intance that will contain the property.</typeparam>
    public interface IPropertySignatureMember<TParent> :
        IMember<TParent, CodeMemberProperty>,
        IAutoCommentMember,
        ISignatureTableMember
        where TParent :
            IDeclarationTarget
    {
        /// <summary>
        /// Returns/sets whether the property has a get area.
        /// </summary>
        bool HasGet { get; set; }
        /// <summary>
        /// Returns/sets whether the property has a set area.
        /// </summary>
        bool HasSet { get; set; }
        /// <summary>
        /// Returns/sets the type reference the property uses.
        /// </summary>
        ITypeReference PropertyType { get; set; }
    }
}
