using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer.Parser.GDFileData
{
    internal interface ITokenEofEntry : ITokenEntry { }
    public class TokenEofEntry :
        TokenEntry,
        ITokenEofEntry
    {
        public TokenEofEntry()
            : base("EndOFile", new TokenExpressionSeries(new ITokenExpression[0], 0, 0, 0, "ProjectConstructor.cs"), EntryScanMode.Inherited, "ProjectConstructor.cs", 0, 0, 0, false, false, new ITokenEntry[0])
        {

        }
    }
}
