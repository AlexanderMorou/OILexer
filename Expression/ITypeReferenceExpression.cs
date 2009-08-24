using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="IMemberParentExpression"/>
    /// that needs no instance to refer to members.
    /// </summary>
    public interface ITypeReferenceExpression :
        IMemberParentExpression
    {
        /// <summary>
        /// Returns the <see cref="ITypeReference"/> the <see cref="ITypeMemberExpression"/> relates
        /// to.
        /// </summary>
        ITypeReference TypeReference { get; }
    }
}
