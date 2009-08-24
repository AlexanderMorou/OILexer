using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    [Serializable]
    internal class MethodParameterMember :
        ParameteredParameterMember<IMethodParameterMember, CodeMemberMethod, IMemberParentType>,
        IMethodParameterMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="MethodParameterMember"/>
        /// with the parameter type, name and target provided.
        /// </summary>
        /// <param name="nameAndType">The type and name of the parameter.</param>
        /// <param name="parentTarget">The place the parameter exists on.</param>
        public MethodParameterMember(TypedName nameAndType, IMethodMember parentTarget)
            :base(nameAndType, parentTarget)
        {
        }
    }
}
