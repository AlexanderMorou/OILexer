using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
using System.Collections;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal class ReadOnlyWrapper<T> :
        IControlledCollection<T>
    {
        private IControlledCollection<T> source;

        internal ReadOnlyWrapper(IControlledCollection<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            this.source = source;
        }

        //#region IControlledCollection<T> Members

        public bool Contains(T item)
        {
            return this.source.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex = 0)
        {
            this.source.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.source.Count; }
        }

        public int IndexOf(T element)
        {
            return this.source.IndexOf(element);
        }

        public T[] ToArray()
        {
            return this.source.ToArray();
        }

        public T this[int index]
        {
            get { return this.source[index]; }
        }

        //#endregion

        //#region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        //#endregion

        //#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //#endregion
    }
}
