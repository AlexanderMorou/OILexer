using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    partial class SyntacticalStreamState
    {
        partial class BuildTransition
        {
            internal class ParentChildPairing
            {
                private SyntacticalDFAState parent;
                private SyntacticalDFAState child;

                public ParentChildPairing(SyntacticalDFAState parent, SyntacticalDFAState child)
                {
                    this.parent = parent;
                    this.child = child;
                }

                public SyntacticalDFAState Parent
                {
                    get
                    {
                        return this.parent;
                    }
                }

                public SyntacticalDFAState Child
                {
                    get
                    {
                        return this.child;
                    }
                }
            }
        }
    }
}
