using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    public interface ISnippetMember :
        IMember<IMemberParentType, CodeSnippetTypeMember>
    {
        string Text { get; set; }
    }
}
