using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public enum StringTerminalKind
    {
        Unknown,
        Include,
        Root,
        AssemblyName,
        LexerName,
        GrammarName,
        ParserName,
        TokenPrefix,
        TokenSuffix,
        RulePrefix,
        RuleSuffix,
        Namespace,
    }
}
