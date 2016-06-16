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
    internal interface IOilexerGrammarTokenEofEntry : IOilexerGrammarTokenEntry { }
    public class OilexerGrammarTokenEofEntry :
        OilexerGrammarTokenEntry,
        IOilexerGrammarTokenEofEntry
    {
        internal OilexerGrammarTokenEofEntry(IOilexerGrammarTokenEntry[] tokens)
            : base("EndOfFile", new TokenExpressionSeries(new ITokenExpression[0], 0, 0, 0, "OilexerGrammarTokenEOFEntry.cs"), EntryScanMode.Inherited, "OilexerGrammarTokenEOFEntry.cs", 0, 0, 0, false, tokens, false)
        {
        }
    }
}
