using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Reflection;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a method's parameter.
    /// </summary>
    public interface IMethodParameterMember :
        IParameteredParameterMember<IMethodParameterMember, CodeMemberMethod, IMemberParentType>//,
        //IFauxableReliant<ParameterInfo, MethodInfo>
    {

    }
}
