using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    public interface IControlledStateKeyedTreeNode<TKey, TValue, TNode> :
        IControlledStateKeyedTree<TKey, TValue, TNode>
        where TNode :
            IControlledStateKeyedTreeNode<TKey, TValue, TNode>
    {
        /// <summary>
        /// Returns the <typeparamref name="TValue"/> which represents
        /// the current <see cref="IControlledStateKeyedTreeNode"/>.
        /// </summary>
        TValue Value { get; }
    }
}
