using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with an indexer property's parameter members.
    /// </summary>
    public interface IIndexerSignatureParameterMembers :
        IIndexerSignatureParameterMembers<IIndexerSignatureParameterMember, CodeMemberProperty, IIndexerSignatureMember>
    {
    }
}
