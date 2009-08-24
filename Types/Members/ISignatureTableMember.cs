using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    public interface ISignatureTableMember :
        IMember
    {
        /// <summary>
        /// Returns/sets whether the <see cref="ISignatureTableMember"/> hides the previous
        /// member(s) of the same name.
        /// </summary>
        bool HidesPrevious { get; set; }
    }
}
