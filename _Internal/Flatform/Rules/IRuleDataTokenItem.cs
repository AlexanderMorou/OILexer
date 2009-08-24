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
    /// <summary>
    /// Defines properties and methods for working with a data item
    /// on a rule which referenes a token.
    /// </summary>
    internal interface IRuleDataTokenItem :
        IRuleDataItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> from which the 
        /// <see cref="IRuleDataTokenItem"/> refers to.
        /// </summary>
        ITokenEntry Source { get; }
    }
}
