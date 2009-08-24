using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class IndexerSignatureParameterMember :
        ParameteredParameterMember<IIndexerSignatureParameterMember, CodeMemberProperty, IIndexerSignatureMember>,
        IIndexerSignatureParameterMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="IndexerSignatureParameterMember"/>
        /// with the parameter type, name and target provided.
        /// </summary>
        /// <param name="nameAndType">The type and name of the parameter.</param>
        /// <param name="parentTarget">The place the parameter exists on.</param>
        public IndexerSignatureParameterMember(TypedName nameAndType, IIndexerSignatureMember parentTarget)
            : base(nameAndType, parentTarget)
        {
        }

    }
}
