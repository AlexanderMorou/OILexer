using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a type parameter of 
    /// a method.
    /// </summary>
    public interface IMethodTypeParameterMember :
        IMethodSignatureTypeParameterMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>
    {
    }
}
