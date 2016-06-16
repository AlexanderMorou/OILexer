using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IPreprocessorThrowDirective :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the reference to the error referenced by the throw directive.
        /// </summary>
        IOilexerGrammarErrorEntry Reference { get; }

        /// <summary>
        /// Returns the arguments associated with the throw directive, all tokens.
        /// </summary>
        IOilexerGrammarToken[] Arguments { get; }
    }
}
