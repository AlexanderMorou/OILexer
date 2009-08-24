using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal interface IRuleDataRuleReferenceItem :
        IRuleDataItem
    {
        /// <summary>
        /// Returns the <see cref="IProductionRuleEntry"/> the 
        /// <see cref="IRuleDataRuleReferenceItem"/> represents.
        /// </summary>
        IProductionRuleEntry Reference { get; }
    }
}
