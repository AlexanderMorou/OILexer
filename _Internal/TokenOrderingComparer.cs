using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal class TokenOrderingComparer :
        IComparer<ITokenEntry>
    {
        private Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> precedences;
        internal TokenOrderingComparer(Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> precedences)
        {
            this.precedences = precedences;
        }
        #region IComparer<ITokenEntry> Members

        public int Compare(ITokenEntry x, ITokenEntry y)
        {
            if (precedences[x][y] == TokenPrecedence.Equal)
                return x.Name.CompareTo(y.Name);
            return -(int)precedences[x][y];
        }

        #endregion
    }
}
