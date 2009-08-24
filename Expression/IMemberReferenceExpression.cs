using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Comments;

namespace Oilexer.Expression
{
    /// <summary>
    /// Refers to the member of an IType.
    /// </summary>
    public interface IMemberReferenceExpression :
        IExpression
    {
        /// <summary>
        /// Returns the name of the method reference.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Returns where the <see cref="IMemberParentExpression"/> was
        /// obtained.
        /// </summary>
        IMemberParentExpression Reference { get; }
        /// <summary>
        /// Creates a reference particle for the <see cref="IMemberReferenceExpression"/>.
        /// </summary>
        /// <returns>A new <see cref="IMemberReferenceComment"/> implementation instance which
        /// refers to the active <see cref="IMemberReferenceExpression"/>'s <see cref="Reference"/>.</returns>
        IMemberReferenceComment GetReferenceParticle();
    }
}
