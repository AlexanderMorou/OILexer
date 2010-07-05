using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.FiniteAutomata.Rules
{
    public class SyntacticalFollowTable :
        ControlledStateDictionary<SyntacticalDFAState, SyntacticalFollowInfo>
    {
        public SyntacticalFollowTable()
        {
        }

        public SyntacticalFollowInfo Follow(SyntacticalDFAState origin, SyntacticalDFAState continuesAt)
        {
            SyntacticalFollowInfo result = new SyntacticalFollowInfo(origin, continuesAt);
            base.Add(origin, result);
            return result;
        }

    }
}
