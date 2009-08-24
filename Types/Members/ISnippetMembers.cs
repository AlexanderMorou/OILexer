using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    public interface ISnippetMembers :
        IMembers<ISnippetMember, IMemberParentType, CodeSnippetTypeMember>
    {
        ISnippetMember AddNew(string text);
    }
}
