using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Types.Members;

namespace Oilexer.Parser.Builder
{
    public class ProductionRuleRootDefinition :
        ProductionRuleDefinition,
        IProductionRuleRootDefinition
    {
        private List<IProductionSubRuleDefinition> subRules;
        private IProductionRuleEntry relativeRule;
        private IMethodMember parseMethod;

        /// <summary>
        /// Creates a new <see cref="ProductionRuleRootDefinition"/> instance with the 
        /// <paramref name="relativeRule"/> provided.
        /// </summary>
        /// <param name="relativeRule">The <see cref="IProductionRuleEntry"/> which 
        /// defined the <see cref="ProductionRuleRootDefinition"/>.</param>
        public ProductionRuleRootDefinition(IProductionRuleEntry relativeRule)
        {
            this.relativeRule = relativeRule;
        }

        #region IProductionRuleRootDefinition Members

        /// <summary>
        /// Returns the <see cref="IList{T}"/> which contains the sub-rules of the 
        /// <see cref="ProductionRuleRootDefinition"/>.
        /// </summary>
        public IList<IProductionSubRuleDefinition> SubRules
        {
            get
            {
                if (this.subRules == null)
                    this.subRules = new List<IProductionSubRuleDefinition>();
                return this.subRules;
            }
        }

        /// <summary>
        /// Returns the rule that declared the <see cref="ProductionRuleRootDefinition"/>.
        /// </summary>
        public IProductionRuleEntry RelativeRule
        {
            get { return this.relativeRule; }
        }

        /// <summary>
        /// Returns the parse method for the <see cref="IProductionRuleRootDefinition"/>.
        /// </summary>
        /// <returns>A <see cref="IMethodMember"/> relative to the <see cref="IProductionRuleRootDefinition"/>
        /// which is responsible for resolving the rule.</returns>
        public IMethodMember ParseMethod
        {
            get
            {
                return this.parseMethod;
            }
            internal set
            {
                this.parseMethod = value;
            }
        }

        #endregion
    }
}
