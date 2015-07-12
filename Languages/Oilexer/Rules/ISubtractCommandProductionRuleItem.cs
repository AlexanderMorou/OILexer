using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface ISubtractCommandProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the <see cref="IProductionRuleSeries"/> which denotes the
        /// positive set.
        /// </summary>
        IProductionRuleSeries Minuend { get; }
        /// <summary>
        /// Returns the <see cref="IProductionRuleSeries"/> which denotes the
        /// negative aspect to remove from <see cref="Minuend"/>.
        /// </summary>
        IProductionRuleSeries Subtrahend { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="IProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ISubtractCommandProductionRuleItem"/> with the data
        /// members of the current <see cref="ISubtractCommandProductionRuleItem"/>.</returns>
        new ISubtractCommandProductionRuleItem Clone();
    }
}
