using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Indicates which side the containing type of the <see cref="IBinaryOperatorOverloadMember"/>
    /// resides on in the overload.
    /// </summary>
    public enum BinaryOperatorOverloadContainingSide
    {
        /// <summary>
        /// The containing type matches the left side.
        /// </summary>
        LeftSide,
        /// <summary>
        /// The containing type matches the right side.
        /// </summary>
        RightSide,
        /// <summary>
        /// The containing type matches both sides.
        /// </summary>
        Both
    }
}
