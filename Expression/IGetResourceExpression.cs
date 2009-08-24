using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for working with an expression that obtains information
    /// from a type resource or the project resource.
    /// </summary>
    public interface IGetResourceExpression :
        IExpression<CodeExpression>,
        IMemberParentExpression
    {
        /// <summary>
        /// Returns/sets the name of the resource to obtain.
        /// </summary>
        string ResourceName { get; set; }
        /// <summary>
        /// The type that contains the resources from which to obtain the value.-or- The type
        /// which contains the reference to the project that contains the resources from which
        /// to obtain the value.
        /// </summary>
        ITypeMemberParent EnclosingType { get; set; }
    }
}
