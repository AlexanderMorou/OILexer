using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    public interface ITypeMemberParent :
        ITypeParent,
        IMemberParentType,
        IResourceable
    {
    }
}
