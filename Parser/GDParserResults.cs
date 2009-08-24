using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    internal class GDParserResults :
        ParserResults<IGDFile>
    {
        internal void SetResult(IGDFile result)
        {
            base.Result = result;
        }
    }
}
