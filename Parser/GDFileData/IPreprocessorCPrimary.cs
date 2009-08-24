using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface IPreprocessorCPrimary :
        IPreprocessorCExp
    {
        /// <summary>
        /// Returns the rule that was matched.
        /// </summary>
        /// <remarks>Can either be:
        /// 1 - string<br/>
        /// 2 - char<br/>
        /// 3 - '(' ProcessorConditionExp ')' <br/>
        /// 4 - Identifier <br/>
        /// 5 - Number</remarks>
        int Rule { get; }
        GDTokens.IdentifierToken Identifier { get; }
        string String { get; }
        char? Char { get; }
        IPreprocessorCLogicalOrConditionExp PreCLogicalOrExp { get; }
        int? Number { get; }
    }
}
