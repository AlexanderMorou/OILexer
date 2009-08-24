using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Reflection;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a constructor parameter.
    /// </summary>
    public interface IConstructorParameterMember :
        IParameteredParameterMember<IConstructorParameterMember, CodeConstructor, IMemberParentType>//,
        //IFauxableReliant<ParameterInfo, Type>
    {
    }
}
