using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal enum ResultedDataType
    {
        None,
        /// <summary>
        /// The element is an enumeration item.
        /// No Data-type is yielded, but the parent
        /// container will utilize it in its resulted
        /// enum type.
        /// </summary>
        EnumerationItem,
        /// <summary>
        /// The element is an enumeration item, which requires
        /// a flag to be set with it.
        /// </summary>
        FlagEnumerationItem,
        Flag,
        Enumeration,
        ComplexType,
        Counter,
        /// <summary>
        /// The element is a character range.
        /// </summary>
        Character,
        /// <summary>
        /// The element is a character range which
        /// repeats
        /// </summary>
        String,
        ImportType,
        ImportTypeList,
        PassThrough,
    }
}
