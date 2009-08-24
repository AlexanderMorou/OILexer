using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Expression
{
    partial class CreateArrayExpression
    {
        /// <summary>
        /// The means to which the <see cref="CreateArrayExpression"/> is serialized in the
        /// stream.
        /// </summary>
        [Serializable]
        [Flags]
        internal enum SerializationType :
            byte
        {
            /// <summary>
            /// There is no serialization type defined.
            /// </summary>
            None = 0,
            /// <summary>
            /// The serialization contains a primitive form of the size.
            /// </summary>
            SizePrimitive = 1,
            /// <summary>
            /// The serialization contains an expression form of the size.
            /// </summary>
            SizeExpression = 3,
            /// <summary>
            /// The serialization contains a series of initializers.
            /// </summary>
            Initializers = 4
        }
    }
}
