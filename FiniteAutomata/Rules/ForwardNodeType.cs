using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    public enum ForwardNodeType
    {
        /// <summary>
        /// The forward node is the root node of a state.
        /// </summary>
        Root,
        /// <summary>
        /// The forward node is a primary entry of a rule.
        /// </summary>
        Rule,
        /// <summary>
        /// The forward node is the result of a forward rule's 
        /// initial state being an edge state.
        /// </summary>
        EmptyFollower,
    }
}
