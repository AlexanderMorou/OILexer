using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class MethodSignatureMember :
        MethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>,
        IMethodSignatureMember
    {
        #region MethodSignatureMember Constructors
        public MethodSignatureMember(string name, ISignatureMemberParentType parentTarget)
            : base(new TypedName(name, typeof(void)), parentTarget)
        {
        }

        public MethodSignatureMember(TypedName nameAndReturn, ISignatureMemberParentType parentTarget)
            : base(nameAndReturn, parentTarget, new TypeConstrainedName[0])
        {
        }

        public MethodSignatureMember(TypedName nameAndReturn, ISignatureMemberParentType parentTarget, params TypedName[] parameters)
            : base(nameAndReturn, parentTarget, parameters, new TypeConstrainedName[0])
        {
        }

        public MethodSignatureMember(TypedName nameAndReturn, ISignatureMemberParentType parentTarget, params TypeConstrainedName[] typeParameters)
            : base(nameAndReturn, parentTarget, new TypedName[0], typeParameters)
        {
        }

        public MethodSignatureMember(TypedName nameAndReturn, ISignatureMemberParentType parentTarget, TypedName[] parameters, TypeConstrainedName[] typeParameters)
            : base(nameAndReturn, parentTarget, parameters, typeParameters)
        {
        }
        #endregion

        #region IMethodSignatureMember Members

        public new IMethodSignatureParameterMembers Parameters
        {
            get
            {
                return (IMethodSignatureParameterMembers)base.Parameters;
            }
        }

        public new IMethodSignatureTypeParameterMembers TypeParameters
        {
            get
            {
                return (IMethodSignatureTypeParameterMembers)base.TypeParameters;
            }
        }

        #endregion

        protected override IParameteredParameterMembers<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType> InitializeParameters()
        {
            return new MethodSignatureParameterMembers(this);
        }

        protected override IMethodSignatureTypeParameterMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> InitializeTypeParameters()
        {
            return new MethodSignatureTypeParameterMembers(this);
        }

        protected override Oilexer.Expression.IMemberReferenceExpression OnGetReference()
        {
            throw new NotSupportedException("Non-instance capable entities cannot be referenced in this manner, they cannot have method bodies as signatures.");
        }
    }
}
