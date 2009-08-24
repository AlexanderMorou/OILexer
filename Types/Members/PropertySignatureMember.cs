using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    [Serializable]
    public partial class PropertySignatureMember :
        PropertySignatureMember<ISignatureMemberParentType>,
        IPropertySignatureMember
    {
        public PropertySignatureMember(TypedName nameAndType, ISignatureMemberParentType parentTarget)
            : base(nameAndType, parentTarget)
        {

        }

        public PropertySignatureMember(string name, ISignatureMemberParentType parentTarget)
            : base(name, parentTarget)
        {

        }
    }
}
