using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// The type of operator that is overloaded by the <see cref="IBinaryOperatorOverloadMember"/>.
    /// </summary>
    public enum OverloadableBinaryOperators :
        int
    {
        /// <summary>
        /// Addition binary operator, often '+'.
        /// </summary>
        Add,
        /// <summary>
        /// Subtraction binary operator, often '-'.
        /// </summary>
        Subtract,
        /// <summary>
        /// Multiplication binary operator, often '*'.
        /// </summary>
        Multiply,
        /// <summary>
        /// Division binary operator, often '/'.
        /// </summary>
        Divide,
        /// <summary>
        /// Modulus binary operator, often '%' or 'Mod'.
        /// </summary>
        Modulus,
        /// <summary>
        /// Logical And binary operator, often '&' or 'And'.
        /// </summary>
        LogicalAnd,
        /// <summary>
        /// Logical Or binary operator, often '|' or 'Or'.
        /// </summary>
        LogicalOr,
        /// <summary>
        /// Exclusive Or binary operator, often '^' or 'XOr'.
        /// </summary>
        ExclusiveOr,
        /// <summary>
        /// Left-shift binary operator, often '&lt;&lt;'.
        /// </summary>
        LeftShift,
        /// <summary>
        /// Right-shift binary operator, often '&gt;&gt;'.
        /// </summary>
        RightShift,
        /// <summary>
        /// Equal to binary operator, often '==' or '='.
        /// </summary>
        IsEqualTo,
        /// <summary>
        /// Not equal to binary operator, often '!=' or '&lt;&gt;'.
        /// </summary>
        IsNotEqualTo,
        /// <summary>
        /// Less than binary operator, often '&lt;'.
        /// </summary>
        LessThan,
        /// <summary>
        /// Greater than binary operator, often '&gt;'.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Less than or equal to binary operator, often '&lt;='.
        /// </summary>
        LessThanOrEqualTo,
        /// <summary>
        /// Greater than or equal to binary operator, often '&gt;='.
        /// </summary>
        GreaterThanOrEqualTo
    }
}
