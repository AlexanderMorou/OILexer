using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    /// <summary>
    /// Defines properties and methods for working with a character
    /// range structural item.
    /// </summary>
    internal interface ICaptureTokenCharRangeStructuralItem :
        ICaptureTokenStructuralItem
    {
        /// <summary>
        /// Returns whether the <see cref="ICaptureTokenCharRangeStructuralItem"/> contains
        /// siblings within the token expression in which the sources are defined.
        /// </summary>
        /// <remarks>If true, groups containing this element cannot
        /// change it to a string value type.</remarks>
        bool HasSiblings { get; set; }
        new IControlledCollection<ICharRangeTokenItem> Sources { get; }

    }
}
