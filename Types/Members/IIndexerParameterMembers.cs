using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    public interface IIndexerParameterMembers :
        //IParameteredParameterMembers<IIndexerParameterMember, CodeMemberProperty, IIndexerMember>
        IIndexerSignatureParameterMembers<IIndexerParameterMember, CodeMemberProperty, IIndexerMember>
    {
    }
}
