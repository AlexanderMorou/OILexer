using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Data;

namespace Oilexer.Utilities.Collections
{
    [Serializable]
    public class ReadOnlyDictionary<TKey, TValue> :
        ControlledStateDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IReadOnlyDictionary
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        public ReadOnlyDictionary()
            : base()
        {
        }
        /// <summary>
        /// Creates a new instance of <see cref="ReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionaryCopy)
            : base(dictionaryCopy)
        {
        }
        protected ReadOnlyDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void Remove(TKey key)
        {
            throw new ReadOnlyException(string.Format("ReadOnlyCollection<{0}, {1}> is read-only", typeof(TKey).Name, typeof(TValue).Name));
        }
        
        protected override void Remove(int index)
        {
            throw new ReadOnlyException(string.Format("ReadOnlyCollection<{0}, {1}> is read-only", typeof(TKey).Name, typeof(TValue).Name));
        }
        protected override void Add(TKey key, TValue value)
        {
            throw new ReadOnlyException(string.Format("ReadOnlyCollection<{0}, {1}> is read-only", typeof(TKey).Name, typeof(TValue).Name));
        }
    }
}
