using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a parser that reads in a 
    /// <see cref="IOilexerGrammarFile"/>.
    /// </summary>
    public interface IOilexerGrammarParser :
        IParser<IOilexerGrammarToken, IOilexerGrammarTokenizer, IOilexerGrammarFile>
    {
    }
}
