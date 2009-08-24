using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class IndexerParameterMembers :
        //ParameteredParameterMembers<IIndexerParameterMember, CodeMemberProperty, IIndexerMember>,
        IndexerSignatureParameterMembers<IIndexerParameterMember, CodeMemberProperty, IIndexerMember>,
        IIndexerParameterMembers
    {
        /// <summary>
        /// Creates a new instance of <see cref="IndexerParameterMembers"/> with
        /// the <paramref name="targetDeclaration"/> provided.
        /// </summary>
        /// <param name="targetDeclaration">The <see cref="IParameteredDeclaration{TParameter, TSignatureDom, TParent}"/> which the 
        /// <see cref="IndexerParameterMembers"/> belongs to.</param>
        public IndexerParameterMembers(IIndexerMember targetDeclaration)
            : base(targetDeclaration)
        {
        }


        /// <summary>
        /// Obtains an instance of an <see cref="IIndexerParameterMember"/> implementation with 
        /// the type and name provided.
        /// </summary>
        /// <param name="data">The typed name which defines the name and type of the parameter.</param>
        /// <returns>A new instance of the <see cref="IIndexerParameterMember"/>.</returns>
        protected override IIndexerParameterMember GetParameterMember(TypedName data)
        {
            return new IndexerParameterMember(data, (IIndexerMember)this.TargetDeclaration);
        }
    }
}
