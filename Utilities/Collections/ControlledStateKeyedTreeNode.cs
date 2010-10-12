using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    public class ControlledStateKeyedTreeNode<TKey, TValue, TNode> :
        ControlledStateKeyedTree<TKey, TValue, TNode>,
        IControlledStateKeyedTreeNode<TKey, TValue, TNode>
        where TNode :
            ControlledStateKeyedTreeNode<TKey, TValue, TNode>
    {
        public ControlledStateKeyedTreeNode()
        {
        }
        public ControlledStateKeyedTreeNode(IEnumerable<KeyValuePair<TKey, TNode>> entries)
            : base(entries)
        {
        }

        #region IControlledStateKeyedTreeNode<TKey,TValue,TNode> Members

        public TValue Value { get; protected set; }

        #endregion

    }
}
