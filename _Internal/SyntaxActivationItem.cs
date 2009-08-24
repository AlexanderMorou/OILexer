using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal abstract class SyntaxActivationItem :
        ISyntaxActivationItem
    {
        /// <summary>
        /// Returns the kind of activation item the <see cref="SyntaxActivationItem"/> is.
        /// </summary>
        public abstract SyntaxActivationItemType Type { get; }
    }
}
