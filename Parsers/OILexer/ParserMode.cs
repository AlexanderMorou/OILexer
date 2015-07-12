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
    public enum ParserTokenizerMode
    {
        /// <summary>
        /// The tokenizer requires the parser's state to work.
        /// The resulted token can change based upon the state the parser is in.
        /// </summary>
        StateBased,
        /// <summary>
        /// The tokenizer becomes a traditional tokenizer, in that, tokens 
        /// </summary>
        Traditional,
    }
}
