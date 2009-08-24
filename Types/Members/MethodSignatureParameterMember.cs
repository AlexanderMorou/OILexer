using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class MethodSignatureParameterMember :
        ParameteredParameterMember<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType>,
        IMethodSignatureParameterMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="MethodSignatureParameterMember"/>
        /// with the parameter type, name and target provided.
        /// </summary>
        /// <param name="nameAndType">The type and name of the parameter.</param>
        /// <param name="parentTarget">The place the parameter exists on.</param>
        public MethodSignatureParameterMember(TypedName nameAndType, IMethodSignatureMember parentTarget)
            :base(nameAndType, parentTarget)
        {
        }
    }
}
