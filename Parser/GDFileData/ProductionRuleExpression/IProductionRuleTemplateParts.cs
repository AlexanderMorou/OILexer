using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    /// <summary>
    /// Defines properties and methods for working with a series of template parts 
    /// for template expansion.
    /// </summary>
    public interface IProductionRuleTemplateParts :
        IReadOnlyCollection<IProductionRuleTemplatePart>
    {

    }
}
