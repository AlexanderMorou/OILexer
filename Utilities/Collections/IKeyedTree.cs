using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Utilities.Collections
{
    public interface IKeyedTree<TKey, TValue, TNode> :
        IControlledStateKeyedTree<TKey, TValue, TNode>
        where TNode :
            IKeyedTreeNode<TKey, TValue, TNode>,
            new()
    {
        /// <summary>
        /// Adds a new <typeparamref name="TNode"/> to the
        /// <see cref="IKeyedTree{TKey, TValue, TNode}"/>
        /// with the <paramref name="key"/>, and <paramref name="value"/>
        /// provided.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> to associate to the
        /// <typeparamref name="TNode"/> to create.</param>
        /// <param name="value">The <typeparamref name="TValue"/>
        /// to associate to the new node.</param>
        /// <returns>A new <typeparamref name="TNode"/> instance
        /// which is represented by the <paramref name="key"/>
        /// and <paramref name="value"/> provided.</returns>
        TNode Add(TKey key, TValue value);
        /// <summary>
        /// Adds a series of <typeparamref name="TNode"/> instances
        /// based off of the <see cref="IEnumerable{T}"/> of
        /// <see cref="KeyValuePair{TKey, TValue}"/> elements.
        /// </summary>
        /// <param name="entries">The series of <see cref="KeyValuePair{TKey, TValue}"/>
        /// series.</param>
        /// <returns>A series of <typeparamref name="TNode"/>
        /// instances relative to the <paramref name="entries"/>
        /// provided.</returns>
        TNode[] AddRange(IEnumerable<KeyValuePair<TKey, TValue>> entries);
        /// <summary>
        /// Clears the <see cref="IKeyedTree"/>.
        /// </summary>
        void Clear();
        /// <summary>
        /// Removes a <typeparamref name="TNode"/> element
        /// based off of the <paramref name="target"/> <typeparamref name="TKey"/> provided.
        /// </summary>
        /// <param name="target">The <typeparamref name="TKey"/>
        /// which represents the identifying key associated to the 
        /// <typeparamref name="TNode"/> to remove.</param>
        /// <returns>true if an element was found and removed; false otherwise.</returns>
        bool Remove(TKey target);
    }
}
