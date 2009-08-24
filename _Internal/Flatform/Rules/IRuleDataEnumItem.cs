using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal interface IRuleDataEnumItem :
        IRuleDataTokenItem
    {
        /// <summary>
        /// Returns the <see cref="TokenBitArray"/> which represents the
        /// elements that are covered by the <see cref="IRuleDataEnumItem"/>.
        /// </summary>
        TokenBitArray CoveredSet { get; }
    }
}
