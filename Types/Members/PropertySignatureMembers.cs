using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Collections;
namespace Oilexer.Types.Members
{
    [Serializable]
    public class PropertySignatureMembers :
        Members<IPropertySignatureMember, ISignatureMemberParentType, CodeMemberProperty>,
        IPropertySignatureMembers,
        IPropertySignatureMembers<IPropertySignatureMember, ISignatureMemberParentType>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyMembers"/> with the
        /// <paramref name="targetDeclaration"/> provided.
        /// </summary>
        /// <param name="targetDeclaration">The <see cref="IMemberParentType"/> that contains
        /// the <see cref="PropertyMembers"/>.</param>
        public PropertySignatureMembers(ISignatureMemberParentType targetDeclaration)
            : base(targetDeclaration)
        {
        }

        public PropertySignatureMembers(ISignatureMemberParentType targetDeclaration, IDictionary<string, IPropertySignatureMember> partialBaseMembers)
            : base(targetDeclaration, partialBaseMembers)
        {
        }


        protected override IMembers<IPropertySignatureMember, ISignatureMemberParentType, CodeMemberProperty> OnGetPartialClone(ISignatureMemberParentType parent)
        {
            return this.GetPartialClone(parent);
        }

        #region IPropertySignatureMembers Members

        public IIndexerSignatureMember AddNew(ITypeReference indexerType, params TypedName[] parameters)
        {
            return this.AddNew(indexerType, true, true, parameters);
        }

        public IIndexerSignatureMember AddNew(ITypeReference indexerType, bool hasGet, bool hasSet, params TypedName[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Indexers must have at least one parameter.");
            IIndexerSignatureMember result = new IndexerSignatureMember(indexerType, this.TargetDeclaration);
            foreach (TypedName tn in parameters)
                result.Parameters.AddNew(tn);
            result.HasGet = hasGet;
            result.HasSet = hasSet;
            this.Add(result.GetUniqueIdentifier(), result);
            return result;
        }

        public IIndexerSignatureMember AddNew(TypedName nameAndType, params TypedName[] parameters)
        {
            return this.AddNew(nameAndType, true, true, parameters);
        }

        public IIndexerSignatureMember AddNew(TypedName nameAndType, bool hasGet, bool hasSet, params TypedName[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Indexers must have at least one parameter.");
            IIndexerSignatureMember result = new IndexerSignatureMember(nameAndType.Name, nameAndType.TypeReference, this.TargetDeclaration);
            foreach (TypedName tn in parameters)
                result.Parameters.AddNew(tn);
            result.HasGet = hasGet;
            result.HasSet = hasSet;
            this.Add(result.GetUniqueIdentifier(), result);
            return result;
        }

        public new IPropertySignatureMembers GetPartialClone(ISignatureMemberParentType parent)
        {
            return new PropertySignatureMembers(parent, this.dictionaryCopy);
        }

        #endregion

        #region IPropertySignatureMembers<IPropertySignatureMember,ISignatureMemberParentType> Members

        IPropertySignatureMembers<IPropertySignatureMember, ISignatureMemberParentType> IPropertySignatureMembers<IPropertySignatureMember, ISignatureMemberParentType>.GetPartialClone(ISignatureMemberParentType parent)
        {
            return this.GetPartialClone(parent);
        }

        public IPropertySignatureMember AddNew(string name, ITypeReference propertyType)
        {
            return this.AddNew(new TypedName(name, propertyType));
        }

        public IPropertySignatureMember AddNew(TypedName nameAndType)
        {
            return this.AddNew(nameAndType, true, true);
        }

        public IPropertySignatureMember AddNew(string name, ITypeReference propertyType, bool hasGet, bool hasSet)
        {
            return this.AddNew(new TypedName(name, propertyType), hasGet, hasSet);
        }

        public IPropertySignatureMember AddNew(TypedName nameAndType, bool hasGet, bool hasSet)
        {
            IPropertySignatureMember result = new PropertySignatureMember(nameAndType, this.TargetDeclaration);
            result.HasGet = hasGet;
            result.HasSet = hasSet;
            this.Add(result.GetUniqueIdentifier(), result);
            return result;
        }


        #endregion



        #region IPropertySignatureMembers<IPropertySignatureMember,ISignatureMemberParentType> Members

        public new void Add(IPropertySignatureMember ipm)
        {
            base.Add(ipm.GetUniqueIdentifier(), ipm);
        }

        #endregion
    }
}
