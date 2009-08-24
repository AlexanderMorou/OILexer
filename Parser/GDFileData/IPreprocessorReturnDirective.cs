using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Defines properties and methods for working with a conditional return directive that
    /// yields the result of the conditional for the current iteration or run-through.
    /// </summary>
    public interface IPreprocessorConditionalReturnDirective :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the array of <see cref="IProductionRule"/> which result.
        /// </summary>
        IProductionRule[] Result { get; }
    }
}
