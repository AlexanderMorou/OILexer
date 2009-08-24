using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    public interface IPropertySignatureMembers :
        IPropertySignatureMembers<IPropertySignatureMember, ISignatureMemberParentType>
    {
        IIndexerSignatureMember AddNew(ITypeReference indexerType, params TypedName[] parameters);
        IIndexerSignatureMember AddNew(ITypeReference indexerType, bool hasGet, bool hasSet, params TypedName[] parameters);
        IIndexerSignatureMember AddNew(TypedName nameAndType, TypedName[] parameters);
        IIndexerSignatureMember AddNew(TypedName nameAndType, bool hasGet, bool hasSet, TypedName[] parameters);
        /// <summary>
        /// Creates a new instance of the <see cref="IPropertySignatureMembers"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="IMembers{TItem, TParent, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IPropertySignatureMembers"/> implementation instance.</param>
        /// <returns>A new <see cref="IPropertySignatureMembers"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IPropertySignatureMembers GetPartialClone(ISignatureMemberParentType parent);
    }
}
