using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal class SyntaxActivationTokenItem :
        SyntaxActivationItem
    {
        public SyntaxActivationTokenItem(TokenFinalData source)
        {
            this.Source = source;
        }

        public TokenFinalData Source { get; private set; }
        
        public override SyntaxActivationItemType Type
        {
            get { return SyntaxActivationItemType.TokenReference; }
        }

        public override string ToString()
        {
            return string.Format("Token {0}", this.Source.Entry);
        }
    }
}
