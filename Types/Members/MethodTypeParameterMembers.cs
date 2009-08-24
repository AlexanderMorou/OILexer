using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Runtime.Serialization;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class MethodTypeParameterMembers :
        TypeParameterMembers<IMethodTypeParameterMember, CodeTypeParameter, IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>>,
        IMethodTypeParameterMembers
    {

        public MethodTypeParameterMembers(IMethodMember targetDeclaration)
            :base (targetDeclaration)
        {
        }
        protected MethodTypeParameterMembers(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override IMethodTypeParameterMember AddNew(string name)
        {
            return this.AddNew(name, TypeParameterSpecialCondition.None);
        }

        public override IMethodTypeParameterMember AddNew(string name, bool requiresConstructor)
        {
            return this.AddNew(name, requiresConstructor, TypeParameterSpecialCondition.None);
        }

        public override IMethodTypeParameterMember AddNew(string name, ITypeReferenceCollection constraints)
        {
            return this.AddNew(name, constraints, TypeParameterSpecialCondition.None);
        }

        public override IMethodTypeParameterMember AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor)
        {
            return AddNew(name, constraints, requiresConstructor, TypeParameterSpecialCondition.None);
        }

        public override IMethodTypeParameterMember AddNew(string name, ITypeReference[] constraints, bool requiresConstructor)
        {
            return this.AddNew(name, constraints, requiresConstructor, TypeParameterSpecialCondition.None);
        }

        public override IMethodTypeParameterMember AddNew(TypeConstrainedName data)
        {
            return this.AddNew(data.Name, data.TypeReferences, data.RequiresConstructor, data.SpecialCondition);
        }

        #region IMethodSignatureTypeParameterMembers<IMethodParameterMember,IMethodTypeParameterMember,IMemberParentType> Members

        public event EventHandler<DeclarationChangeArgs> SignatureChange;

        #endregion

        protected override IMembers<IMethodTypeParameterMember, IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>, CodeTypeParameter> OnGetPartialClone(IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> parent)
        {
            throw new NotSupportedException("Method Type-parameters cannot be spanned across multiple instances");
        }

        public override IMethodTypeParameterMember AddNew(string name, TypeParameterSpecialCondition specialCondition)
        {
            return AddNew(name, false, specialCondition);
        }

        public override IMethodTypeParameterMember AddNew(string name, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, new ITypeReference[0], requiresConstructor, specialCondition);
        }

        public override IMethodTypeParameterMember AddNew(string name, ITypeReferenceCollection constraints, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, constraints, false, specialCondition);
        }

        public override IMethodTypeParameterMember AddNew(string name, ITypeReferenceCollection constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
        {
            MethodTypeParameterMember result = new MethodTypeParameterMember(name, (IMethodMember)this.TargetDeclaration);
            result.Constraints.AddRange(constraints.ToArray());
            result.SpecialCondition = specialCondition;
            result.RequiresConstructor = requiresConstructor;
            this.Add(result.GetUniqueIdentifier(), result);
            this.OnSignatureChanged();
            return result;
        }

        public override IMethodTypeParameterMember AddNew(string name, ITypeReference[] constraints, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
        {
            return this.AddNew(name, new TypeReferenceCollection(constraints), requiresConstructor, specialCondition);
        }


        protected virtual void OnSignatureChanged()
        {
            if (this.SignatureChange != null)
                this.SignatureChange(this, new DeclarationChangeArgs(this.TargetDeclaration));
        }

    }
}
