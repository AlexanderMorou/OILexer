using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    partial class SyntacticalStreamState
    {
        private partial class BuildTransition :
            List<BuildTransition.ParentChildPairing>
        {
            public BuildTransition(SyntacticalDFAState parent, SyntacticalDFAState child)
            {
                base.Add(new ParentChildPairing(parent, child));
            }

            public BuildTransition(BuildTransition original)
                : base(original)
            {
            }
        }
    }
}
