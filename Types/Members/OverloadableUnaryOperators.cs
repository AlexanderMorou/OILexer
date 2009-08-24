using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// </summary>
    public enum OverloadableUnaryOperators :
        int
    {
        /// <summary>
        /// Plus unary operator, often '+'.
        /// </summary>
        Plus,
        /// <summary>
        /// Negative unary operator, often '-'.
        /// </summary>
        Negative,
        /// <summary>
        /// Evaluates to false unary operator, often 'false' or 'IsFalse'.
        /// </summary>
        EvaluatesToFalse,
        /// <summary>
        /// Evaluates to false unary operator, often 'true' or 'IsTrue'.
        /// </summary>
        EvaluatesToTrue,
        /// <summary>
        /// Logical inversion unary operator, often '!' or 'Not'.
        /// </summary>
        LogicalInvert
    }
}
