using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal class UnicodeCollectiveTargetGraph :
        ControlledCollection<IUnicodeTargetGraph>,
        IUnicodeCollectiveTargetGraph
    {

        #region IUnicodeCollectiveTargetGraph Members

        public void Add(IUnicodeTargetGraph graph)
        {
            base.baseList.Add(graph);
        }

        public IUnicodeTargetGraph Find(IUnicodeTargetGraph duplicate, bool relaxOriginatingState = false)
        {
            return this.FirstOrDefault(p => p.Equals(duplicate, relaxOriginatingState));
        }

        #endregion
    }
}
