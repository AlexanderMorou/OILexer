using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    public class KeyedTreeNode<TKey, TValue> :
        KeyedTree<TKey, TValue>,
        IKeyedTreeNode<TKey, TValue, KeyedTreeNode<TKey, TValue>>
    {
        public KeyedTreeNode()
        {
        }
        public KeyedTreeNode(IEnumerable<KeyValuePair<TKey, KeyedTreeNode<TKey, TValue>>> entries)
            : base(entries)
        {
        }

        #region IKeyedTreeNode<TKey,TValue,KeyedTreeNode<TKey,TValue>> Members

        public TValue Value { get; set; }

        #endregion

    }
}
