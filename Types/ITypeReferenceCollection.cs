using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a list of <see cref="ITypeReference"/>
    /// instances.
    /// </summary>
    public interface ITypeReferenceCollection :
        IList<ITypeReference>
    {
        ITypeReference[] AddRange(System.Type[] types);
        ITypeReference Add(System.Type type);
        ITypeReference[] AddRange(IType[] types);
        ITypeReference Add(IType type);
        void AddRange(ITypeReference[] types);
        ITypeReference[] ToArray();
    }
}
