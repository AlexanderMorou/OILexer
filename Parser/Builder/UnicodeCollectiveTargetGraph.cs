using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.Builder
{
    internal class UnicodeCollectiveTargetGraph :
        ControlledStateCollection<IUnicodeTargetGraph>,
        IUnicodeCollectiveTargetGraph
    {

        #region IUnicodeCollectiveTargetGraph Members

        public void Add(IUnicodeTargetGraph graph)
        {
            base.baseCollection.Add(graph);
        }

        public IUnicodeTargetGraph Find(IUnicodeTargetGraph duplicate)
        {
            return this.FirstOrDefault(p => p.Equals(duplicate));
        }

        #endregion

    }
}
