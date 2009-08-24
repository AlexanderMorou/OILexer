using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    /// <summary>
    /// Base type of an <see cref="IEnumeratorType"/>
    /// </summary>
    [Serializable]
    public enum EnumeratorBaseType
    {
        /// <summary>
        /// <para>The default enumeration base-type.</para>
        /// <seealso cref="System.SInt"/>
        /// </summary>
        Default = SInt,

        /// <summary>
        /// <para>The enum's base-type is an unsigned byte; value range from 0->255.</para>
        /// <seealso cref="System.Byte"/>.
        /// </summary>
        UByte=0, 

        /// <summary>
        /// <para>The enum's base-type is a signed byte; value range from -128->127</para>
        /// <seealso cref="System.SByte"/>.
        /// </summary>
        SByte, 

        /// <summary>
        /// <para>The enum's base-type is an unsigned 16-bit integer; value range from 0->65536.</para>
        /// <seealso cref="System.UInt16"/>.
        /// </summary>
        UShort, 

        /// <summary>
        /// <para>The enum's base-type is a signed 16-bit integer; value range from
        /// -32768->32767.</para>
        /// <seealso cref="System.Int16"/>.
        /// </summary>
        Short, 

        /// <summary>
        /// <para>The enum's base-type is an unsigned 32-bit integer; value range from 
        /// 0->4294967296.</para>
        /// <seealso cref="System.UInt32"/>.
        /// </summary>
        UInt, 

        /// <summary>
        /// <para>The enum's base-type is a signed 32-bit integer; value range from 
        /// -2147483648->2147483647.</para>
        /// <seealso cref="System.Int32"/>.
        /// </summary>
        SInt, 

        /// <summary>
        /// <para>The enum's base-type is an unsigned  64-bit integer; value range from 
        /// 0->18446744073709551616</para>
        /// <seealso cref="System.UInt64"/>.
        /// </summary>
        ULong,

        /// <summary>
        /// <para>The enum's base-type is a signed 64-bit integer; value range from 
        /// -9223372036854775808->9223372036854775807</para>
        /// <seealso cref="System.Int64"/>.
        /// </summary>
        SLong
    }
}
