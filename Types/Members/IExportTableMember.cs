using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for an <see cref="IMember"/> which can alter
    /// the export table of the base definition.
    /// </summary>
    public interface IExportTableMember :
        ISignatureTableMember,
        IInvokableMember
    {
    }
}
