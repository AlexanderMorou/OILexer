using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines accessability levels for declarations.
    /// </summary>
    [Serializable]
    public enum DeclarationAccessLevel : int
    {
        /// <summary>
        /// A declaration with public visibility/accessability.
        /// </summary>
        Public = MemberAttributes.Public,
        /// <summary>
        /// A declaration with localized visibility/accessability.
        /// </summary>
        Private = MemberAttributes.Private,
        /// <summary>
        /// A declaration with internal visibility/accessability within the Assembly.
        /// </summary>
        Internal = MemberAttributes.Assembly,
        /// <summary>
        /// A declaration with protected visibility/accessability within the inheritance
        /// family.
        /// </summary>
        Protected = MemberAttributes.Family,
        /// <summary>
        /// A declaration with internal or protected visibility/accessability within the 
        /// inheritance family and the Assembly.
        /// </summary>
        ProtectedInternal = MemberAttributes.FamilyOrAssembly

    }
}
