using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.Builder
{
    internal interface IUnicodeCollectiveTargetGraph :
        IControlledStateCollection<IUnicodeTargetGraph>
    {
        void Add(IUnicodeTargetGraph graph);

        IUnicodeTargetGraph Find(IUnicodeTargetGraph duplicate);
    }
}
