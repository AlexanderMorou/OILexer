using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    /// <summary>
    /// The type of syntax element.
    /// </summary>
    internal enum SyntaxActivationItemType
    {
        /// <summary>
        /// The item refers to a rule reference.
        /// </summary>
        RuleReference,
        /// <summary>
        /// The item refers to a token reference.
        /// </summary>
        TokenReference,
        /// <summary>
        /// The item refers to an enum item reference.
        /// </summary>
        EnumItemReference,
    }
}
