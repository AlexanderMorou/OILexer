using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    /// <summary>
    /// Defines properties and methods for a rule's named data item.
    /// </summary>
    internal interface IRuleDataNamedItem :
        IRuleDataItem
    {
        /// <summary>
        /// Returns the <see cref="String"/> representing the 
        /// element's name.
        /// </summary>
        string Name { get; }
    }
}
