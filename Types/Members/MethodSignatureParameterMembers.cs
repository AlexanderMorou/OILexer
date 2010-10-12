using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Utilities.Collections;
using System.Runtime.Serialization;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class MethodSignatureParameterMembers :
        ParameteredParameterMembers<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType>,
        IMethodSignatureParameterMembers
    {

        public MethodSignatureParameterMembers(IMethodSignatureMember targetDeclaration)
            : base(targetDeclaration)
        {

        }

        protected override IMembers<IMethodSignatureParameterMember, IParameteredDeclaration<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType>, CodeParameterDeclarationExpression> OnGetPartialClone(IParameteredDeclaration<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType> parent)
        {
            throw new NotSupportedException("Method Parameter sets cannot be spanned across multiple instances, methods aren't segmentable.");
        }

        protected override IMethodSignatureParameterMember GetParameterMember(TypedName data)
        {
            return new MethodSignatureParameterMember(data, (IMethodSignatureMember)this.TargetDeclaration);
        }
    }
}
