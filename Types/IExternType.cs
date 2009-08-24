using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    public interface IExternType :
        IType
    {
        /// <summary>
        /// Returns the <see cref="System.Type"/> which the extern refers to.
        /// </summary>
        System.Type Type { get; }
    }
}
