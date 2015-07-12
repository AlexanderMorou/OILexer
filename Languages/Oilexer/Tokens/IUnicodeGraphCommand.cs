using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public interface IBaseEncodeGraphCommand :
        ICommandTokenItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenExpressionSeries"/> which makes up the 
        /// expression series to encode.
        /// </summary>
        ITokenExpressionSeries EncodeTarget { get; }
        OilexerGrammarTokens.NumberLiteral Digits { get; }
        OilexerGrammarTokens.NumberLiteral NumericBase { get; }
        OilexerGrammarTokens.StringLiteralToken StringBase { get; }
    }
}
