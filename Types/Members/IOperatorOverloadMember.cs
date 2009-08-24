using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    public interface IOperatorOverloadMember
    {
        /// <summary>
        /// The operator overloaded.
        /// </summary>
        int Operator { get; set; }
    }
}
