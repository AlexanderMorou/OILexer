using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="IEntry"/> production rule.
    /// Used to express a part of syntax for a <see cref="IGDFile"/>.
    /// </summary>
    public interface IProductionRuleEntry :
        IProductionRuleSeries,
        IScannableEntry
    {
        /// <summary>
        /// Returns/sets whether the elements of 
        /// the <see cref="IProductionRuleEntry"/>
        /// inherit the name of the 
        /// <see cref="IProductionRuleEntry"/>.
        /// </summary>
        bool ElementsAreChildren { get; set; }
    }
}
