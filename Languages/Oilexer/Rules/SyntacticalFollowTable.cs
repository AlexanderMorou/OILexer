using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class SyntacticalFollowTable :
        ControlledStateDictionary<SyntacticalDFAState, SyntacticalFollowInfo>
    {
        public SyntacticalFollowTable()
        {
        }

        public SyntacticalFollowInfo Follow(SyntacticalDFAState origin, SyntacticalDFAState continuesAt)
        {
            SyntacticalFollowInfo result = new SyntacticalFollowInfo(origin, continuesAt);
            this._Add(origin, result);
            return result;
        }

    }
}
