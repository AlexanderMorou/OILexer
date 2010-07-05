using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    public interface ISyntacticalForwardNode
    {
        /// <summary>
        /// Returns the kind of node the forward node is.
        /// </summary>
        ForwardNodeType Kind { get; }
    }
}
