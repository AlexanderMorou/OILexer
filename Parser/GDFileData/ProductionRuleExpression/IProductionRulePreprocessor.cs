using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public interface IProductionRulePreprocessorDirective :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the <see cref="IPreprocessorDirective"/> which was parsed 
        /// </summary>
        IPreprocessorDirective Directive { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="IProductionRulePreprocessorDirective"/>.
        /// </summary>
        /// <returns>A new <see cref="IProductionRulePreprocessorDirective"/> with the data
        /// members of the current <see cref="IProductionRulePreprocessorDirective"/>.</returns>
        new IProductionRulePreprocessorDirective Clone();
    }
}
