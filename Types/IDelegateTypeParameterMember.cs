using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.Reflection;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a parameter of a delegate type.
    /// </summary>
    public interface IDelegateTypeParameterMember :
        IParameteredParameterMember<IDelegateTypeParameterMember, CodeTypeDelegate, ITypeParent>//,
        //IFauxableReliant<ParameterInfo, Type>
    {
    }
}
