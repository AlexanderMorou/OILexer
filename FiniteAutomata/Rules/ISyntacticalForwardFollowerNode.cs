using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a 
    /// forward node which represents a node ahead that is
    /// capable of being empty.
    /// </summary>
    public interface ISyntacticalForwardFollowerNode :
        ISyntacticalForwardNode
    {
        /// <summary>
        /// Returns the <see cref="ISyntacticalForwardNode"/> which represents 
        /// the node which was empty.
        /// </summary>
        ISyntacticalForwardNode EmptyNode { get; }
    }
}
