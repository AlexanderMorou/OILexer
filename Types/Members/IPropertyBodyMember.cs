using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;

namespace Oilexer.Types.Members
{
    public interface IPropertyBodyMember :
        IBlockParent,
        IStatementBlockInsertBase,
        IAttributeDeclarationTarget
    {
        /// <summary>
        /// Returns the property that the body belongs to.
        /// </summary>
        new IPropertyMember ParentTarget { get;set; }
        PropertyBodyMemberPart Part { get; }
    }
}
