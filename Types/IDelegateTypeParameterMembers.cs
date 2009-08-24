using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a delegate type's parameters.
    /// </summary>
    public interface IDelegateTypeParameterMembers : 
        IParameteredParameterMembers<IDelegateTypeParameterMember, CodeTypeDelegate, ITypeParent>
    {
    }
}
