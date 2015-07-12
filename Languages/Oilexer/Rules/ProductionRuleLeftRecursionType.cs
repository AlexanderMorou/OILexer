using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    [Flags]
    public enum ProductionRuleLeftRecursionType :
        byte
    {
        None,
        Direct = 1,
        Indirect = 2,
        Hidden = 4,
        DirectAndIndirect = Direct | Indirect,
        DirectAndHidden = Direct | Hidden,
        IndirectAndHidden = Hidden | Indirect,
        All = Direct | Indirect | Hidden,
    }
}
