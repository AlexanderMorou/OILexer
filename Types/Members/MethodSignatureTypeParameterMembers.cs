using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Runtime.Serialization;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class MethodSignatureTypeParameterMembers :
        TypeParameterMembers<IMethodSignatureTypeParameterMember, CodeTypeParameter, IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>>,
        IMethodSignatureTypeParameterMembers
    {

        public MethodSignatureTypeParameterMembers(IMethodSignatureMember targetDeclaration)
            : base(targetDeclaration)
        {
        }
        protected MethodSignatureTypeParameterMembers(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name)
        {
            return this.AddNew(name, TypeParameterSpecialCondition.None);
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name, bool requiresConstructor)
        {
            return this.AddNew(name, requiresConstructor, TypeParameterSpecialCondition.None);
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name, ITypeReferenceCollection constraints)
        {
            return this.AddNew(name, constraints, TypeParameterSpecialCondition.None);
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor)
        {
            return this.AddNew(name, constraints, requiresConstructor, TypeParameterSpecialCondition.None);
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name, ITypeReference[] constraints, bool requiresConstructor)
        {
            return this.AddNew(name, constraints, requiresConstructor, TypeParameterSpecialCondition.None);
        }

        public override IMethodSignatureTypeParameterMember AddNew(TypeConstrainedName data)
        {
            return this.AddNew(data.Name, data.TypeReferences, data.RequiresConstructor, data.SpecialCondition);
        }

        #region IMethodSignatureTypeParameterMembers<IMethodSignatureParameterMember,IMethodSignatureTypeParameterMember,ISignatureMemberParentType> Members

        public event EventHandler<DeclarationChangeArgs> SignatureChange;

        #endregion

        protected override IMembers<IMethodSignatureTypeParameterMember, IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>, CodeTypeParameter> OnGetPartialClone(IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType> parent)
        {
            throw new NotSupportedException("Method signature Type-parameters cannot be spanned across multiple instances");
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, false, specialCondition);
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, new TypeReferenceCollection(), requiresConstructor, specialCondition);
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name, ITypeReferenceCollection constraints, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, constraints, false, specialCondition);
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
        {
            MethodSignatureTypeParameterMember result = new MethodSignatureTypeParameterMember(name, (IMethodSignatureMember)this.TargetDeclaration);
            result.Constraints.AddRange(constraints.ToArray());
            result.SpecialCondition = specialCondition;
            result.RequiresConstructor = requiresConstructor;
            this.Add(result.GetUniqueIdentifier(), result);
            this.OnSignatureChanged();
            return result;
        }

        protected virtual void OnSignatureChanged()
        {
            if (this.SignatureChange != null)
                this.SignatureChange(this, new DeclarationChangeArgs(this.TargetDeclaration));
        }

        public override IMethodSignatureTypeParameterMember AddNew(string name, ITypeReference[] constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, new TypeReferenceCollection(constraints), requiresConstructor, specialCondition);
        }
    }
}
