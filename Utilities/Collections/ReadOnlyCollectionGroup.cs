using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Oilexer.Utilities.Collections
{
    public class ReadOnlyCollectionGroup<TCollection, TItem> : 
        IReadOnlyCollection<TItem>
        where TCollection :
            IControlledStateCollection<TItem>
    {
        private IList<IControlledStateCollection<TItem>> collections;
        public ReadOnlyCollectionGroup(IControlledStateCollection<TItem>[] collections)
        {
            this.collections = new List<IControlledStateCollection<TItem>>(collections);
        }
        #region IControlledStateCollection<TItem> Members

        public int Count
        {
            get {
                int result = 0;
                foreach (IControlledStateCollection col in this.collections)
                    result += col.Count;
                return result;
            }
        }

        public bool Contains(TItem item)
        {
            foreach (IControlledStateCollection col in this.collections)
                if (col.Contains(item))
                    return true;
            return false;
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            List<TItem> result = new List<TItem>();
            foreach (IControlledStateCollection col in this.collections)
                foreach (TItem item in col)
                    result.Add(item);
            result.CopyTo(array, arrayIndex);
            result.Clear();
            result = null;
        }

        public TItem this[int index]
        {
            get 
            {
                for (
                        int i = 0,
                        rangeStart = 0,
                        rangeEnd = (this.collections.Count > 0) ?
                            this.collections[0].Count : 0;
                        i < this.collections.Count;
                        rangeStart = rangeEnd,
                        i++,
                        rangeEnd += (i < this.collections.Count)
                            ? this.collections[i].Count : 0)
                {
                    if (index >= rangeStart && index < rangeEnd)
                        return (TItem)collections[i][index - rangeStart];
                }
                return default(TItem);
            }
        }

        #endregion

        #region IEnumerable<TItem> Members

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (IControlledStateCollection col in this.collections)
                foreach (TItem item in col)
                    yield return item;
            yield break;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public TItem[] ToArray()
        {
            throw new NotSupportedException();
        }

        #region IControlledStateCollection<TItem> Members


        public int IndexOf(TItem element)
        {
            for (int i = 0, j = 0; i < this.Count; j += this.collections[i].Count, i++)
                if (this.collections[i].Contains(element))
                    return j + this.collections[i].IndexOf(element);
            return -1;
        }

        #endregion
    }
}
