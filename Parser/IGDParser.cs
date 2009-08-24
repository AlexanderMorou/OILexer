using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    /// <summary>
    /// Defines properties and methods for working with a parser that reads in a 
    /// <see cref="IGDFile"/>.
    /// </summary>
    public interface IGDParser :
        IParser<IGDToken, IGDTokenizer, IGDFile>
    {
    }
}
