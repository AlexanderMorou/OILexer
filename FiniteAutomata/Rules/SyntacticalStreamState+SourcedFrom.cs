using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    partial class SyntacticalStreamState
    {
        public enum SourcedFrom
        {
            /// <summary>
            /// The path was sourced from the initial set for the state.
            /// </summary>
            Initial,

            /// <summary>
            /// The path was sourced from the sub-rule states introduced into the stream.
            /// </summary>
            First,

            /// <summary>
            /// The path was sourced from the terminal rule edges of a given state
            /// causing the state following the caller of the rule to be introduced
            /// into the stream.
            /// </summary>
            Follow
        }
    }
}
