using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// An indexer property's parameter members.
    /// </summary>
    [Serializable]
    public class IndexerSignatureParameterMembers :
    IndexerSignatureParameterMembers<IIndexerSignatureParameterMember, CodeMemberProperty, IIndexerSignatureMember>,
    IIndexerSignatureParameterMembers
    {
        /// <summary>
        /// Creates a new instance of <see cref="IndexerSignatureParameterMembers"/> with
        /// the <paramref name="targetDeclaration"/> provided.
        /// </summary>
        /// <param name="targetDeclaration">The <see cref="IParameteredDeclaration{TParameter, TSignatureDom, TParent}"/> which the 
        /// <see cref="IndexerSignatureParameterMembers"/> belongs to.</param>
        public IndexerSignatureParameterMembers(IIndexerSignatureMember targetDeclaration)
            : base(targetDeclaration)
        {
        }

        /// <summary>
        /// Obtains an instance of an <see cref="IIndexerSignatureParameterMember"/> implementation with 
        /// the type and name provided.
        /// </summary>
        /// <param name="data">The typed name which defines the name and type of the parameter.</param>
        /// <returns>A new instance of the <see cref="IIndexerSignatureParameterMember"/>.</returns>
        protected override IIndexerSignatureParameterMember GetParameterMember(TypedName data)
        {
            return new IndexerSignatureParameterMember(data, (IIndexerSignatureMember)this.TargetDeclaration);
        }
    }

}
