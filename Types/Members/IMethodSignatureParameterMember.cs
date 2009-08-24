using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Reflection;

namespace Oilexer.Types.Members
{
    public interface IMethodSignatureParameterMember :
        IParameteredParameterMember<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType>//,
        //IFauxableReliant<ParameterInfo, MethodInfo>
    {
    }
}
