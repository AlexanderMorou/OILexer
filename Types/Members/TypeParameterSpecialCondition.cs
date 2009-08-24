using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    public enum TypeParameterSpecialCondition
    {
        /// <summary>
        /// The type-parameter has no special condition applied.
        /// </summary>
        None,
        /// <summary>
        /// The type-parameter has the 'class' special condition applied.
        /// </summary>
        /// <remarks>For Visual Basic.Net, this is 'Class', and in C# it is 'class'</remarks>
        Class,
        /// <summary>
        /// The type-parameter has the 'value-type' special condition applied.
        /// </summary>
        /// <remarks>For Visual Basic.Net, this is 'Structure', and in C# it is 'struct'</remarks>
        ValueType
    }
}
