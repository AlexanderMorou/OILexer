using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a series of type-parameters.
    /// </summary>
    public interface ITypeParameterMembers :
        IMembers,
        IAutoCommentFragmentMembers
    {
        IDictionary<string, ITypeReference> GetTypeReferenceListing();
    }
}
