using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface IPreprocessorGroupProductionRuleItem :
        IControlledDictionary<IPreprocessorCLogicalOrConditionExp, IProductionRuleSeries>,
        IProductionRuleItem
    {
        
    }
}
