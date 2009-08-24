using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// The requirement set forth on the <see cref="ITypeConversionOverloadMember"/> which
    /// stipulates whether the conversion is implicit or explicit.
    /// </summary>
    public enum TypeConversionRequirement
    {
        /// <summary>
        /// The conversion from/to the containing type of the <see cref="ITypeConversionOverloadMember"/>
        /// must be specified through a cast.
        /// </summary>
        Explicit,
        /// <summary>
        /// The conversion from/to the containing type of the <see cref="ITypeConversionOverloadMember"/>
        /// is inferred by use.
        /// </summary>
        Implicit
    }
    /// <summary>
    /// The direction in which the containing type of the 
    /// <see cref="ITypeConversionOverloadMember"/> is coerced.
    /// </summary>
    public enum TypeConversionDirection
    {
        /// <summary>
        /// The conversion is to the containing type from the type specified.
        /// </summary>
        ToContainingType,
        /// <summary>
        /// The conversion is from the containing type to the type specified.
        /// </summary>
        FromContainingType
    }
}
