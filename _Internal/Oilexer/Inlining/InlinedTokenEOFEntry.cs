using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    internal class IOilexerGrammarnlinedTokenEofEntry :
        InlinedTokenEntry,
        IOilexerGrammarTokenEofEntry
    {
        public IOilexerGrammarnlinedTokenEofEntry(IOilexerGrammarTokenEofEntry source)
            : base(source, null)
        {
        }
    }
}
