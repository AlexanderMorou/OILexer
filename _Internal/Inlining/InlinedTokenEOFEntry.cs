using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Inlining
{
    internal class InlinedTokenEofEntry :
        InlinedTokenEntry,
        ITokenEofEntry
    {
        public InlinedTokenEofEntry(ITokenEofEntry source)
            : base(source)
        {
        }
    }
}
