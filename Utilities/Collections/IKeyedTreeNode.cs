using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    public interface IKeyedTreeNode<TKey, TValue, TNode> :
        IKeyedTree<TKey, TValue, TNode>,
        IControlledStateKeyedTreeNode<TKey, TValue, TNode>
        where TNode :
            IKeyedTreeNode<TKey, TValue, TNode>,
            new()
    {
        /// <summary>
        /// Returns/sets the <typeparamref name="TValue"/> which represents
        /// the current <see cref="IControlledStateKeyedTreeNode"/>.
        /// </summary>
        TValue Value { get; set; }
    }
}
