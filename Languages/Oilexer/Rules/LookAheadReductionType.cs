using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public enum LookAheadReductionType
    {
        Uncalculated = -1,
        None = 0,
        /// <summary>
        /// All transitions point to a common symbol that is beyond depth 0.
        /// </summary>
        CommonForwardSymbol,
        /// <summary>
        /// All transitions point to depth 0.
        /// </summary>
        LocalTransition,
        /// <summary>
        /// The current transition point represents a repetition point within the grammar.  This is noted to avoid
        /// indeterminable status.
        /// </summary>
        RepetitionPoint,
        /// <summary>The current transition represents an indirectly left recursive transition.</summary>
        LeftRecursive,
    }
}
