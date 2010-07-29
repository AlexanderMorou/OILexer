using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public interface IProductionRuleSeries :
        IReadOnlyCollection<IProductionRule>,
        IProductionRuleSource
    {

        /// <summary>
        /// Obtains the string form of the body of the 
        /// <see cref="IProductionRuleSeries"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> representing
        /// the elements within the description of the
        /// <see cref="IProductionRuleSeries"/>.</returns>
        string GetBodyString();
    }
}
