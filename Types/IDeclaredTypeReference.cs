using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    public interface IDeclaredTypeReference :
        ITypeReference
    {
        /// <summary>
        /// Returns the <see cref="IDeclaredType"/> which the <see cref="IDeclaredTypeReference"/> represents.
        /// </summary> 
        new IDeclaredType TypeInstance { get; }
    }
}
