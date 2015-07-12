using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
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
        OilexerGrammarTokens.IdentifierToken Identifier { get; }
        OilexerGrammarTokens.StringLiteralToken String { get; }
        OilexerGrammarTokens.CharLiteralToken Char { get; }
        IPreprocessorCLogicalOrConditionExp PreCLogicalOrExp { get; }
        OilexerGrammarTokens.NumberLiteral Number { get; }
        IOilexerGrammarToken Token { get; }
    }
}
