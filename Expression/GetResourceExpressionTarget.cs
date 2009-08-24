using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Expression
{
    /// <summary>
    /// Determines where the resource is to be obtained from.
    /// </summary>
    public enum GetResourceExpressionTarget
    {
        /// <summary>
        /// The resource value should be obtained from the current type's resource.
        /// </summary>
        TypeResource,
        /// <summary>
        /// The resource value should be obtained from the project's resource.
        /// </summary>
        ProjectResource,
    }
}
