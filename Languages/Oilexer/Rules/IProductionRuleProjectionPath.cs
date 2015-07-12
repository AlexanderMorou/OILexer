using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface IProductionRuleProjectionPath<TPath, TNode> :
        IControlledCollection<TNode>
        where TPath :
            IProductionRuleProjectionPath<TPath, TNode>
        where TNode :
            IProductionRuleProjectionNode<TPath, TNode>,
            new()
    {
    }
}
