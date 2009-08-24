using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    /// <summary>
    /// Defines properties and methods for working with a grammar description file 
    /// tokenizer.
    /// </summary>
    public interface IGDTokenizer :
        ITokenizer<IGDToken>
    {
    }
}
