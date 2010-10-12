using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2010 Allen Copeland Jr.                                  |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace Oilexer.Utilities.Collections
{
    public class ReadOnlyCollection<T> :
        ControlledStateCollection<T>,
        IReadOnlyCollection<T>
    {
        public ReadOnlyCollection()
            : base(new List<T>())
        {

        }
        public ReadOnlyCollection(IList<T> baseList)
            : base(baseList)
        {

        }
    }
}
