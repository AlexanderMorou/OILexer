using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Oilexer.Utilities.Collections;

namespace Oilexer.Types.Members
{
    partial class ParameteredParameterMember<TParameter, TSignatureDom, TParent>
    {
        protected class FauxCast :
            ParameterInfo
        {
        }
    }
}
