using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser;
using Microsoft.VisualStudio.Text;
using System.Collections;

namespace  AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    internal class GDDelayedLookahed :
        IList<IToken>
    {
        private GDFileHandler handler;
        private List<IToken> pushAheadHelper = new List<IToken>();
        public GDDelayedLookahed(GDFileHandler handler)
        {
            this.handler = handler;

        }

        #region IList<IToken> Members

        public int IndexOf(IToken item)
        {
            int index = 0;
            foreach (var element in this)
                if (item == element)
                    return index;
                else
                    index++;
            return -1;
        }

        public void Insert(int index, IToken item)
        {
            pushAheadHelper.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 && index > pushAheadHelper.Count)
                throw new ArgumentOutOfRangeException("index");
            pushAheadHelper.RemoveAt(index);
        }

        public IToken this[int index]
        {
            get
            {
                int i = 0;
                foreach (var element in this)
                    if (i++ == index)
                        return element;
                throw new IndexOutOfRangeException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<IToken> Members

        public void Add(IToken item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            //Do Nothing.
        }

        public bool Contains(IToken item)
        {
            foreach (var element in this)
                if (element == item)
                    return true;
            return false;
        }

        public void CopyTo(IToken[] array, int arrayIndex)
        {
            this.ToArray().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IToken item)
        {
            if (this.pushAheadHelper.Contains(item))
                return pushAheadHelper.Remove(item);
            return false;
        }

        #endregion

        #region IEnumerable<IToken> Members

        public IEnumerator<IToken> GetEnumerator()
        {
            foreach (var element in this.pushAheadHelper)
                yield return element;
            foreach (var tokenSpanPair in this.handler.TokensFrom(range: new Span(((int)(this.handler.Parser.StreamPosition)), ((int)(this.handler.FullSpan.End - this.handler.Parser.StreamPosition)))))
                yield return tokenSpanPair.Key;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
