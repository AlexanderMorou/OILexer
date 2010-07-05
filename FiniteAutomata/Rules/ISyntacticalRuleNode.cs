using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    public interface ISyntacticalForwardRuleNode :
        ISyntacticalForwardNode
    {
        /// <summary>
        /// Returns the <see cref="SyntacticalDFARootState"/> associated to
        /// the <see cref="ISyntacticalForwardRuleNode"/>.
        /// </summary>
        SyntacticalDFARootState Target { get; }
        /// <summary>
        /// Returns the <see cref="ISyntacticalForwardRootNode"/> associated to a given state.
        /// </summary>
        ISyntacticalForwardRootNode RuleRoot { get; }
    }
}
