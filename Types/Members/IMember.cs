using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods of a declared member.
    /// </summary>
    public interface IMember :
        IDeclaration,
        IAttributeDeclarationTarget
    {
        /// <summary>
        /// Returns the parent of the current <see cref="IMember"/>.
        /// </summary>
        new IDeclarationTarget ParentTarget { get; set; }
        IMemberReferenceExpression GetReference();
    }
}
