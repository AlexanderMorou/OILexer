using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    public interface IImplementedMember :
        IMember
    {
        /// <summary>
        /// Returns the series of <see cref="ITypeReference"/> instances which point to the 
        /// implementation targets.
        /// </summary>
        ITypeReferenceCollection ImplementationTypes { get; }
        /// <summary>
        /// Returns/sets the private implementation target for the <see cref="IImplementedMember"/>.
        /// </summary>
        ITypeReference PrivateImplementationTarget { get; set; }
    }
}
