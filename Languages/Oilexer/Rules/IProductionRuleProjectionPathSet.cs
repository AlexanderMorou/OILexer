using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with
    /// a series of <see cref="IProductionRuleProjectionPath"/>
    /// elements.
    /// </summary>
    public interface IProductionRuleProjectionPathSet<TPath, TNode> :
        IControlledCollection<TPath>
        where TPath :
            IProductionRuleProjectionPath<TPath, TNode>
        where TNode :
            IProductionRuleProjectionNode<TPath, TNode>,
            new()
    {
    }
}
