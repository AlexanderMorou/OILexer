using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Statements;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for a member which coerces either expression execution or
    /// interpretation.
    /// </summary>
    public interface IExpressionCoercionMember :
        IMember<IMemberParentType, CodeSnippetTypeMember>,
        IBlockParent,
        IStatementBlockInsertBase
    {
    }
}
