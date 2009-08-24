using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.Builder
{
    public interface IProductionSubRuleDefinition :
        IProductionRuleDefinition
    {
        /// <summary>
        /// Returns the root rule which defined the <see cref="IProductionRuleDefintion"/>.
        /// </summary>
        IProductionRuleRootDefinition RootRule { get; }

    }
}
