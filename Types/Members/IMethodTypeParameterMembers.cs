using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a method's 
    /// type parameters.
    /// </summary>
    public interface IMethodTypeParameterMembers :
        IMethodSignatureTypeParameterMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>
    {
    }
}
