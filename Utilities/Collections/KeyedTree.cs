using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    public class KeyedTree<TKey, TValue> :
        KeyedTree<TKey, TValue, KeyedTreeNode<TKey, TValue>>
    {
        public KeyedTree()
        {
        }

        public KeyedTree(IEnumerable<KeyValuePair<TKey, KeyedTreeNode<TKey, TValue>>> entries)
            : base(entries)
        {
        }
    }

    public class KeyedTree<TKey, TValue, TNode> :
        ControlledStateKeyedTree<TKey, TValue, TNode>,
        IKeyedTree<TKey, TValue, TNode>
        where TNode :
            IKeyedTreeNode<TKey, TValue, TNode>,
            new()
    {
        public KeyedTree()
        {
        }
        public KeyedTree(IEnumerable<KeyValuePair<TKey, TNode>> entries)
            : base(entries)
        {
        }
        #region IKeyedTree<TKey,TValue,TNode> Members

        public TNode Add(TKey key, TValue value)
        {
            var result = new TNode();
            result.Value = value;
            this._Add(key, result);
            return result; 
        }

        public TNode[] AddRange(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            return (from e in entries
                    select this.Add(e.Key, e.Value)).ToArray();
        }

        public void Clear()
        {
            this._Clear();
        }

        public bool Remove(TKey target)
        {
            return this._Remove(target);
        }

        #endregion

    }
}
