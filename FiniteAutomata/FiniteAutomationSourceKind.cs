using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata
{
    [Flags]
    public enum FiniteAutomationSourceKind
    {
        None = 0,
        Initial = 1,
        Intermediate = 2,
        RepeatPoint = 4,
        Final = 8,
    }
}
