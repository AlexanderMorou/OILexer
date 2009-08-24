using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    public interface IReadOnlyDictionary<TKey, TValue> :
        IControlledStateDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {

    }
}
