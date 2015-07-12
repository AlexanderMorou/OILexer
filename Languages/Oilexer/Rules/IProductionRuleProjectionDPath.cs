using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a
    /// linked series of <typeparamref name="TNode"/>
    /// instances which represents a call hierarchy
    /// for look-ahead determination.
    /// </summary>
    /// <typeparam name="TPath">The type of <see cref="IProductionRuleProjectionPath{TPath, TNode}"/>
    /// from which sequences of <typeparamref name="TNode"/>
    /// are constructed.</typeparam>
    /// <typeparam name="TNode">The creatable type of
    /// <see cref="IProductionRuleProjectionNode{TPath, TNode}"/>
    /// on which the <see cref="IProductionRuleProjectionNodeInfo{TPath, TNode}"/>
    /// are contained.</typeparam>
    public interface IProductionRuleProjectionDPath<TPath, TNode> :
        IProductionRuleProjectionPath<TPath, TNode>,
        IEquatable<TPath>
        where TPath :
            IProductionRuleProjectionPath<TPath, TNode>
        where TNode :
            IProductionRuleProjectionNode<TPath, TNode>,
            new()
    {
        /// <summary>
        /// Returns the <see cref="Int32"/> value which denotes
        /// how deep within the series of <typeparamref name="TNode"/>
        /// elements the analysis set is.
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// Returns the <see cref="Int32"/> value which denotes
        /// the minimum depth within the series of <typeparamref name="TNode"/>.
        /// </summary>
        /// <remarks>
        /// <para>If the Minimum depth within the series is non-zero
        /// this indicates the current path is the result of a 
        /// transition.</para>
        /// <para><seealso cref="IncomingPaths"/>.</para>
        /// </remarks>
        int MinDepth { get; }

        /// <summary>
        /// Returns the <typeparamref name="TNode"/>
        /// at the current <see cref="Depth"/>.
        /// </summary>
        TNode CurrentNode { get; }
        /// <summary>
        /// Returns whether the <see cref="CurrentNode"/>'s States Transition Table
        /// collides with the <paramref name="collider"/> provided.
        /// </summary>
        /// <param name="collider">The <see cref="GrammarVocabulary"/>
        /// to check for collisions with.</param>
        /// <returns>true, if the <paramref name="collider"/> collides with the 
        /// transition table.</returns>
        bool CollidesWith(GrammarVocabulary collider);

        /// <summary>
        /// Provides the series of paths which are yielded upon following an
        /// epsilon transition.
        /// </summary>
        /// <param name="rule">The <see cref="IOilexerGrammarProductionRuleEntry"/>
        /// relevant for the transition.</param>
        /// <returns>An <typeparamref name="IEnumerable{T}"/>
        /// instance which yeilds the series of <typeparamref name="TPath"/>
        /// that result from the transition..</returns>
        /// <remarks>The <see cref="CurrentNode"/> within the path
        /// is replaced with the path that is relevant.</remarks>
        IEnumerable<TPath> FollowEpsilonTransition(IOilexerGrammarProductionRuleEntry rule, IDictionary<SyntacticalDFAState, TNode> fullSeries);

        /// <summary>
        /// Returns the <typeparamref name="TPath"/> which is
        /// one level deeper.
        /// </summary>
        /// <returns>A <typeparamref name="TPath"/> instance
        /// which represents a single level deeper within
        /// the path chain.</returns>
        /// <remarks><seealso cref="Depth"/>.</remarks>
        /// <exception cref="System.InvalidOperationException">thrown when
        /// <see cref="Depth"/> is equal to <see cref="IControlledCollection{T}.Count"/> - 1.</exception>
        TPath IncreaseDepth(bool passAlong);

        /// <summary>
        /// Returns the <typeparamref name="TPath"/> which is
        /// one level above.
        /// </summary>
        /// <returns>A <typeparamref name="TPath"/> instance
        /// which represents a single level above 
        /// the path chain.</returns>
        /// <remarks><seealso cref="Depth"/>.</remarks>
        /// <exception cref="System.InvalidOperationException">thrown when
        /// <see cref="Depth"/> is equal to 0.</exception>
        TPath DecreaseDepth(bool passAlong);

        /// <summary>
        /// Yields the top-most <typeparamref name="TPath"/>
        /// instance in which <see cref="Depth"/> = 
        /// <see cref="MinDepth"/>.
        /// </summary>
        /// <returns>A <typeparamref name="TPath"/>
        /// instance in which <see cref="Depth"/> = 
        /// <see cref="MinDepth"/>.</returns>
        /// <remarks>Will not return null, and 
        /// may return itself.</remarks>
        TPath GetTopLevel(bool passAlong);

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> instance
        /// which obtains all possible depth variants of the
        /// current <typeparamref name="TPath"/>.
        /// </summary>
        IEnumerable<TPath> GetAllDepths();

        IEnumerable<TPath>  ObtainEpsilonTransitions(IDictionary<SyntacticalDFAState, TNode> fullSeries);

        /// <summary>
        /// Returns whether a given path is derived from an
        /// epsilon transition, that is the result of an 
        /// implicit transition caused by an empty
        /// production rule path.
        /// </summary>
        bool IsEpsilonDerived(int index);
    }
}
