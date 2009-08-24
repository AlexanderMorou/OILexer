using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
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
