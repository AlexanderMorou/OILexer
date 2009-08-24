using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a parameterized type that may 
    /// contain type-arguments, members, and sub-types.
    /// </summary>
    public interface IParameteredParentType<TDom> :
        IParameteredDeclaredType<TDom>,
        ITypeMemberParent
        where TDom :
            CodeTypeDeclaration,
            new()
    {
        
    }
}
