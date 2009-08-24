using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    public interface IInvokableMember
    {
        /// <summary>
        /// Returns/sets whether the <see cref="IInvokableMember"/> is statically defined.
        /// </summary>
        /// <remarks>Static members are instance-free members and cannot be used with 
        /// <see cref="IThisReferenceExpression"/>; conversely, non-static members 
        /// cannot be used within static members without a direct reference to 
        /// that member.</remarks>
        bool IsStatic { get; set; }
    }
}
