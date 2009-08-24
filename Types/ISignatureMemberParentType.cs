using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Expression;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines a label to determine a target for member signatures 
    /// (members which have no code body).
    /// </summary>
    public interface ISignatureMemberParentType :
        IDeclaredType,
        IDeclarationTarget
    {
        /// <summary>
        /// Returns the methods defined on the <see cref="IMemberParentType"/>.
        /// </summary>
        IMethodSignatureMembers Methods { get; }
        /// <summary>
        /// Returns the properties defined on the <see cref="IMemberParentType"/>.
        /// </summary>
        IPropertySignatureMembers Properties { get; }
        /// <summary>
        /// Obtains the type-expression associated with the type.
        /// </summary>
        /// <returns>A new <see cref="ITypeReferenceExpression"/> implementation which
        /// denotes the current instance as the type.</returns>
        ITypeReferenceExpression GetTypeExpression(ITypeReferenceCollection typeArguments);
        /// <summary>
        /// Returns the number of members contained within the <see cref="ISignatureMemberParentType"/>.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> relating to the number of members within
        /// the <see cref="ISignatureMemberParentType"/></returns>
        int GetMemberCount();
    }
}
