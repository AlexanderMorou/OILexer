using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Runtime.Serialization;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class PropertyMembers :
        Members<IPropertyMember, IMemberParentType, CodeMemberProperty>,
        IPropertyMembers
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyMembers"/> with the
        /// <paramref name="targetDeclaration"/> provided.
        /// </summary>
        /// <param name="targetDeclaration">The <see cref="IMemberParentType"/> that contains
        /// the <see cref="PropertyMembers"/>.</param>
        public PropertyMembers(IMemberParentType targetDeclaration)
            : base(targetDeclaration)
        {
        }

        public PropertyMembers(IMemberParentType targetDeclaration, PropertyMembers sibling)
            : base(targetDeclaration, sibling)
        {
        }

        protected override IMembers<IPropertyMember, IMemberParentType, CodeMemberProperty> OnGetPartialClone(IMemberParentType parent)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #region IPropertyMembers Members


        public new IPropertyMembers GetPartialClone(IMemberParentType parent)
        {
            return new PropertyMembers(parent, this);
        }

        public IIndexerMember AddNew(ITypeReference indexerType, params TypedName[] parameters)
        {
            return this.AddNew(indexerType, true, true, parameters);
        }

        public IIndexerMember AddNew(ITypeReference indexerType, bool hasGet, bool hasSet, params TypedName[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Indexers must have at least one parameter.");
            IIndexerMember result = new IndexerMember(indexerType, this.TargetDeclaration);
            foreach (TypedName tn in parameters)
                result.Parameters.AddNew(tn);
            result.HasGet = hasGet;
            result.HasSet = hasSet;
            this._Add(result.GetUniqueIdentifier(), result);
            return result;
        }

        public IIndexerMember AddNew(TypedName nameAndType, TypedName[] parameters)
        {
            return this.AddNew(nameAndType, true, true, parameters);
        }

        public IIndexerMember AddNew(TypedName nameAndType, bool hasGet, bool hasSet, TypedName[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Indexers must have at least one parameter.");
            IIndexerMember result = new IndexerMember(nameAndType.Name, nameAndType.TypeReference, this.TargetDeclaration);
            foreach (TypedName tn in parameters)
                result.Parameters.AddNew(tn);
            result.HasGet = hasGet;
            result.HasSet = hasSet;
            this._Add(result.GetUniqueIdentifier(), result);
            return result;
        }


        #endregion

        #region IPropertySignatureMembers<IPropertyMember,IMemberParentType> Members

        IPropertySignatureMembers<IPropertyMember, IMemberParentType> IPropertySignatureMembers<IPropertyMember,IMemberParentType>.GetPartialClone(IMemberParentType parent)
        {
            return this.GetPartialClone(parent);
        }

        public IPropertyMember AddNew(string name, ITypeReference propertyType, bool hasGet, bool hasSet)
        {
            return this.AddNew(new TypedName(name, propertyType), hasGet, hasSet);
        }

        public IPropertyMember AddNew(TypedName nameAndType, bool hasGet, bool hasSet)
        {
            IPropertyMember result = new PropertyMember(nameAndType, this.TargetDeclaration);
            result.HasGet = hasGet;
            result.HasSet = hasSet;
            this._Add(result.GetUniqueIdentifier(), result);
            return result;
        }

        public IPropertyMember AddNew(string name, ITypeReference propertyType)
        {
            return this.AddNew(new TypedName(name, propertyType));
        }

        public IPropertyMember AddNew(TypedName nameAndType)
        {
            return this.AddNew(nameAndType, true, true);
        }


        public new void Add(IPropertyMember ipm)
        {
            this._Add(ipm.GetUniqueIdentifier(), ipm);
        }

        #endregion

    }
}
