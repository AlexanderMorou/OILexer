using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    internal interface ITokenEofEntry : ITokenEntry { }
    public class TokenEofEntry :
        TokenEntry,
        ITokenEofEntry
    {
        internal TokenEofEntry(ITokenEntry[] tokens)
            : base("EndOFile", new TokenExpressionSeries(new ITokenExpression[0], 0, 0, 0, "ProjectConstructor.cs"), EntryScanMode.Inherited, "ProjectConstructor.cs", 0, 0, 0, false, tokens, false)
        {
            
        }
    }
}
