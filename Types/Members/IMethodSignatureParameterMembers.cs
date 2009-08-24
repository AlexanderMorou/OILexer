using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    public interface IMethodSignatureParameterMembers :
        IParameteredParameterMembers<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType>
    {
    }
}
