using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Denotes the type of projection expected from a given
    /// Production Rule Projection Determination Path Set.
    /// </summary>
    public enum ProductionRuleProjectionType :
        byte
    {
        Unspecified = 0,
        LookAhead = 1,
        FollowAmbiguity = 2,
        LookAheadFollowAmbiguity = 4,
    }
}
