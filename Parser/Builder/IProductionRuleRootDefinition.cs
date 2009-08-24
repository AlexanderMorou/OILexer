using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Types;
using Oilexer.Types.Members;

namespace Oilexer.Parser.Builder
{
    public interface IProductionRuleRootDefinition :
        IProductionRuleDefinition
    {
        /// <summary>
        /// Returns the <see cref="IList{T}"/> which contains the sub-rules of the 
        /// <see cref="IProductionRuleRootDefinition"/>.
        /// </summary>
        IList<IProductionSubRuleDefinition> SubRules { get; }
        /// <summary>
        /// Returns the rule that declared the <see cref="IProductionRuleRootDefinition"/>.
        /// </summary>
        IProductionRuleEntry RelativeRule { get; }
        /// <summary>
        /// Returns the parse method for the <see cref="IProductionRuleRootDefinition"/>.
        /// </summary>
        /// <returns>A <see cref="IMethodMember"/> relative to the <see cref="IProductionRuleRootDefinition"/>
        /// which is responsible for resolving the rule.</returns>
        IMethodMember ParseMethod { get; }
    }
}
