using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Diagnostics;
using System.Runtime.Serialization;
using Oilexer.Utilities.Collections;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// A series of method members for a <see cref="IMemberParentType"/>.
    /// </summary>
    [Serializable]
    public partial class MethodMembers :
        Members<IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>, IMemberParentType, CodeMemberMethod>,
        IMethodMembers,
        IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>
    {
        /// <summary>
        /// Data member for <see cref="Values"/>.
        /// </summary>
        private ValuesCollection valuesCollection = null;
        /// <summary>
        /// Creates a new instance of a <see cref="MethodMembers"/> collection with the 
        /// <see cref="Members{TItem, TParent, TDom}.TargetDeclaration"/> defined.
        /// </summary>
        /// <param name="targetDeclaration">The target declaration that contains the 
        /// <see cref="MethodMembers"/>.</param>
        public MethodMembers(IMemberParentType targetDeclaration)
            : base(targetDeclaration)
        {
            InitializeValuesCollection();
        }

        public MethodMembers(IMemberParentType targetDeclaration, IDictionary<string, IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>> partialBaseMembers)
            : base(targetDeclaration, partialBaseMembers)
        {
            InitializeValuesCollection();
        }
        protected MethodMembers(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            InitializeValuesCollection();
        }

        private void InitializeValuesCollection()
        {
            this.valuesCollection = new ValuesCollection(this.dictionaryCopy.Values);
        }

        #region IMethodMembers Members

        [DebuggerHidden]
        public new IMethodMembers GetPartialClone(IMemberParentType parent)
        {
            return new MethodMembers(parent, this.dictionaryCopy);
        }

        [DebuggerHidden]
        public IMethodMember AddNew(string name, ITypeReference returnType)
        {
            return AddNew(new TypedName(name, returnType));
        }

        [DebuggerHidden]
        public IMethodMember AddNew(TypedName nameAndReturn)
        {
            return AddNew(nameAndReturn, new TypedName[0]);
        }

        [DebuggerHidden]
        public IMethodMember AddNew(TypedName nameAndReturn, params TypedName[] parameters)
        {
            return AddNew(nameAndReturn, parameters, new TypeConstrainedName[0]);
        }

        [DebuggerHidden]
        public IMethodMember AddNew(TypedName nameAndReturn, params TypeConstrainedName[] typeParameters)
        {
            return AddNew(nameAndReturn, new TypedName[0], typeParameters);
        }
        public IMethodMember AddNew(TypedName nameAndReturn, TypedName[] parameters, TypeConstrainedName[] typeParameters)
        {
            return this.AddNew(null, nameAndReturn, parameters, typeParameters);
        }


        public IMethodMember AddNew(ITypeReference privateImplTarget, TypedName nameAndReturn, params TypedName[] parameters)
        {
            return this.AddNew(privateImplTarget, nameAndReturn, parameters, new TypeConstrainedName[0]);
        }

        public IMethodMember AddNew(ITypeReference privateImplTarget, TypedName nameAndReturn, params TypeConstrainedName[] typeParameters)
        {
            return this.AddNew(privateImplTarget, nameAndReturn, new TypedName[0], typeParameters);
        }


        public IMethodMember AddNew(ITypeReference privateImplTarget, TypedName nameAndReturn, TypedName[] parameters, TypeConstrainedName[] typeParameters)
        {
            IMethodMember methodMember = new MethodMember(nameAndReturn, this.TargetDeclaration, parameters, typeParameters);
            methodMember.PrivateImplementationTarget = privateImplTarget;
            base.CheckIndexingStatus();
            if (this.ContainsKey(methodMember.GetUniqueIdentifier()))
                throw new InvalidOperationException("method signature exists");
            this.Add(methodMember.GetUniqueIdentifier(), methodMember);
            return methodMember;
        }

        protected override void Add(string key, IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> value)
        {
            value.IsOverload = IsOverload(value.Name);
            base.Add(key, value);
        }

        private bool IsOverload(string name)
        {
            foreach (IMethodMember memberMethod in this.Values)
                if (memberMethod.Name == name)
                    return true;
            return false;
        }

        public IMethodMember FindBySig(string name, int typeParamCount, params Type[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, parameterTypes.GetTypeReferences());
        }

        public IMethodMember FindBySig(string name, int typeParamCount, params IType[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, CodeGeneratorHelper.GetTypeReferences(parameterTypes));
        }

        public IMethodMember FindBySig(string name, int typeParamCount, params ITypeReference[] parameterTypes)
        {
            foreach (IMethodMember sig in this.Values)
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

        public IMethodMember FindBySig(string name, params Type[] parameterTypes)
        {
            return FindBySig(name, parameterTypes.GetTypeReferences());
        }
        public IMethodMember FindBySig(string name, params IType[] parameterTypes)
        {
            return FindBySig(name, CodeGeneratorHelper.GetTypeReferences(parameterTypes));
        }
        public IMethodMember FindBySig(string name, params ITypeReference[] parameterTypes)
        {
            return this.FindBySig(name, 0, parameterTypes);
        }

        public string GetUnusedName(string baseName)
        {
            string current = "";
            bool checking = true;
            int adder=0;
            current = baseName;
            while (checking)
            {
                bool collide = false;
                foreach (IMethodMember member in this.Values)
                {
                    if (member.Name == current)
                        collide = true;
                }
                if (!collide)
                    return current;
                current = string.Format("{0}_{1}", baseName, (adder++));
            }
            return baseName;
        }


        protected override IMembers<IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>, IMemberParentType, CodeMemberMethod> OnGetPartialClone(IMemberParentType parent)
        {
            return this.GetPartialClone(parent);
        }

        public new IEnumerator<KeyValuePair<string, IMethodMember>> GetEnumerator()
        {
            foreach (KeyValuePair<string, IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>> item in base.dictionaryCopy)
                yield return new KeyValuePair<string, IMethodMember>(item.Key, (IMethodMember)item.Value);
            yield break;
        }

        public new IReadOnlyCollection<IMethodMember> Values
        {
            get
            {
                return this.valuesCollection;
            }
        }

        public new IMethodMember this[string name]
        {
            get { return (IMethodMember)base[name]; }
        }

        IMethodMember IMethodMembers.this[int index]
        {
            get { return (IMethodMember)base[index]; }
        }

        #endregion

        #region IMethodSignatureMembers<IMethodParameterMember,IMethodTypeParameterMember,CodeMemberMethod,IMemberParentType> Members

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.AddNew(string name, ITypeReference returnType)
        {
            return this.AddNew(name, returnType);
        }

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.AddNew(TypedName nameAndReturn)
        {
            return this.AddNew(nameAndReturn);
        }

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.AddNew(TypedName nameAndReturn, params TypedName[] parameters)
        {
            return this.AddNew(nameAndReturn, parameters);
        }

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.AddNew(TypedName nameAndReturn, params TypeConstrainedName[] typeParameters)
        {
            return this.AddNew(nameAndReturn, typeParameters);
        }

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.AddNew(TypedName nameAndReturn, TypedName[] parameters, TypeConstrainedName[] typeParameters)
        {
            return this.AddNew(nameAndReturn, parameters, typeParameters);
        }

        IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.GetPartialClone(IMemberParentType parent)
        {
            return this.GetPartialClone(parent);
        }

        #endregion


        #region IMethodSignatureMembers<IMethodParameterMember,IMethodTypeParameterMember,CodeMemberMethod,IMemberParentType> Members

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.FindBySig(string name, int typeParamCount, params Type[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, parameterTypes);
        }

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.FindBySig(string name, int typeParamCount, params IType[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, parameterTypes);
        }

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.FindBySig(string name, int typeParamCount, params ITypeReference[] parameterTypes)
        {
            return this.FindBySig(name, typeParamCount, parameterTypes);
        }

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.FindBySig(string name, params Type[] parameterTypes)
        {
            return this.FindBySig(name, parameterTypes);
        }

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.FindBySig(string name, params IType[] parameterTypes)
        {
            return this.FindBySig(name, parameterTypes);
        }

        IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> IMethodSignatureMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>.FindBySig(string name, params ITypeReference[] parameterTypes)
        {
            return this.FindBySig(name, parameterTypes);
        }

        #endregion



    }
}
