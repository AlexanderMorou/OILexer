using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Runtime.Serialization;

namespace Oilexer.Types.Members
{

    [Serializable]
    public class MethodSignatureMembers :
        Members<IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>, ISignatureMemberParentType, CodeMemberMethod>,
        IMethodSignatureMembers,
        IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>
    {
        public MethodSignatureMembers(ISignatureMemberParentType targetDeclaration)
            : base(targetDeclaration)
        {
        }

        public MethodSignatureMembers(ISignatureMemberParentType targetDeclaration, MethodSignatureMembers sibling)
            : base(targetDeclaration, sibling)
        {
        }
        #region IMethodSignatureMembers Members

        public IMethodSignatureMember FindBySig(string name, int typeParamCount, params Type[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, parameterTypes.GetTypeReferences());
        }

        public IMethodSignatureMember FindBySig(string name, int typeParamCount, params IType[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, CodeGeneratorHelper.GetTypeReferences(parameterTypes));
        }

        public IMethodSignatureMember FindBySig(string name, int typeParamCount, params ITypeReference[] parameterTypes)
        {
            foreach (IMethodSignatureMember sig in this.Values)
            {
                if (sig.Name != name || sig.Parameters.Count != parameterTypes.Length || sig.TypeParameters.Count != typeParamCount)
                    continue;
                for (int i = 0; i < sig.Parameters.Count; i++)
                    if (!sig.Parameters[i].ParameterType.Equals(parameterTypes[i]))
                        goto __notSig;
                return sig;
            __notSig: ;
            }
            return null;
        }

        public new IMethodSignatureMembers GetPartialClone(ISignatureMemberParentType parent)
        {
            return new MethodSignatureMembers(parent, this);
        }

        public IMethodSignatureMember AddNew(string name, ITypeReference returnType)
        {
            return AddNew(new TypedName(name, returnType));
        }

        public IMethodSignatureMember AddNew(TypedName nameAndReturn)
        {
            return AddNew(nameAndReturn, new TypedName[0]);
        }

        public IMethodSignatureMember AddNew(TypedName nameAndReturn, params TypedName[] parameters)
        {
            return AddNew(nameAndReturn, parameters, new TypeConstrainedName[0]);
        }

        public IMethodSignatureMember AddNew(TypedName nameAndReturn, params TypeConstrainedName[] typeParameters)
        {
            return AddNew(nameAndReturn, new TypedName[0], typeParameters);
        }

        public IMethodSignatureMember AddNew(TypedName nameAndReturn, TypedName[] parameters, TypeConstrainedName[] typeParameters)
        {
            IMethodSignatureMember methodMember = new MethodSignatureMember(nameAndReturn, this.TargetDeclaration, parameters, typeParameters);
            if (this.ContainsKey(methodMember.GetUniqueIdentifier()))
                throw new InvalidOperationException("method signature exists");
            this._Add(methodMember.GetUniqueIdentifier(), methodMember);
            return methodMember;
        }


        #endregion

        #region IMethodSignatureMembers<IMethodSignatureParameterMember,IMethodSignatureTypeParameterMember,ISignatureMemberParentType> Members

        IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.GetPartialClone(ISignatureMemberParentType parent)
        {
            return this.GetPartialClone(parent);
        }

        #endregion

        protected override IMembers<IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>, ISignatureMemberParentType, CodeMemberMethod> OnGetPartialClone(ISignatureMemberParentType parent)
        {
            return this.GetPartialClone(parent);
        }
        public IMethodSignatureMember FindBySig(string name, params Type[] parameterTypes)
        {
            return FindBySig(name, parameterTypes.GetTypeReferences());
        }
        public IMethodSignatureMember FindBySig(string name, params IType[] parameterTypes)
        {
            return FindBySig(name, CodeGeneratorHelper.GetTypeReferences(parameterTypes));
        }
        public IMethodSignatureMember FindBySig(string name, params ITypeReference[] parameterTypes)
        {
            return this.FindBySig(name, 0, parameterTypes);
        }

        #region IMethodSignatureMembers<IMethodSignatureParameterMember,IMethodSignatureTypeParameterMember,CodeMemberMethod,ISignatureMemberParentType> Members

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.AddNew(string name, ITypeReference returnType)
        {
            return this.AddNew(name, returnType);
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.AddNew(TypedName nameAndReturn)
        {
            return this.AddNew(nameAndReturn);
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.AddNew(TypedName nameAndReturn, params TypedName[] parameters)
        {
            return this.AddNew(nameAndReturn, parameters);
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.AddNew(TypedName nameAndReturn, params TypeConstrainedName[] typeParameters)
        {
            return this.AddNew(nameAndReturn, typeParameters);
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.AddNew(TypedName nameAndReturn, TypedName[] parameters, TypeConstrainedName[] typeParameters)
        {
            return this.AddNew(nameAndReturn, parameters, typeParameters);
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.FindBySig(string name, params Type[] parameterTypes)
        {
            return this.FindBySig(name, parameterTypes);
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.FindBySig(string name, params IType[] parameterTypes)
        {
            return this.FindBySig(name, parameterTypes);
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.FindBySig(string name, params ITypeReference[] parameterTypes)
        {
            return this.FindBySig(name, parameterTypes);
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.FindBySig(string name, int typeParamCount, params Type[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, parameterTypes.GetTypeReferences());
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>.FindBySig(string name, int typeParamCount, params IType[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, CodeGeneratorHelper.GetTypeReferences(parameterTypes));
        }

        IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> IMethodSignatureMembers<IMethodSignatureParameterMember,IMethodSignatureTypeParameterMember,CodeMemberMethod,ISignatureMemberParentType>.FindBySig(string name, int typeParamCount, params ITypeReference[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, parameterTypes);
        }

        #endregion

    }
}
