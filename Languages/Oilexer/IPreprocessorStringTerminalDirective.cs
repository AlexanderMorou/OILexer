using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IPreprocessorStringTerminalDirective :
        IPreprocessorDirective
    {
        string Literal { get; }
        StringTerminalKind Kind { get; }
        OilexerGrammarTokens.StringLiteralToken LiteralToken { get; }
        OilexerGrammarTokens.PreprocessorDirective KindToken { get; }
        /// <summary>
        /// Returns/sets the target the actual directive refers to when
        /// <see cref="Kind"/> is <see cref="StringTerminalKind.Include"/>.
        /// </summary>
        string Target { get; set; }

    }
}
