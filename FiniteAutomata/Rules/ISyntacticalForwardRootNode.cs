using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Defines properties and methods for working with the root
    /// node of a look-ahead forward node.
    /// </summary>
    interface ISyntacticalForwardRootNode :
        ISyntacticalForwardNode
    {
        /// <summary>
        /// Returns the <see cref="SyntacticalDFAState"/> associated to
        /// the <see cref="ISyntacticalForwardRootNode"/>.
        /// </summary>
        SyntacticalDFAState Target { get; }
    }
}
