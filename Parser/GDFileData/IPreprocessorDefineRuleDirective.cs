using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Defines properties and methods for a define preprocessor directive.
    /// </summary>
    public interface IPreprocessorDefineRuleDirective :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the target of the <see cref="IPreprocessorDefineRuleDirective"/>.
        /// </summary>
        string DeclareTarget { get; }
        /// <summary>
        /// Returns the array of <see cref="IProductionRule"/> which result.
        /// </summary>
        IProductionRule[] DefinedRules { get; }
    }
}
