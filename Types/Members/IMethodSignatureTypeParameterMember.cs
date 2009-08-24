using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    public interface IMethodSignatureTypeParameterMember :
        IMethodSignatureTypeParameterMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>
    {
    }
}
