using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

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
        public ReadOnlyCollection(ICollection<T> baseCollection)
            : base(baseCollection)
        {

        }
    }
}
