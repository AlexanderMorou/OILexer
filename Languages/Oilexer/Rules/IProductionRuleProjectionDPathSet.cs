using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a series of
    /// <see cref="IProductionRuleProjectionPath"/> elements
    /// which is structured towards dependency analysis.
    /// </summary>
    /// <remarks>
    /// Used to disambiguate the target path of a given
    /// production in look-ahead analysis.</remarks>
    public interface IProductionRuleProjectionDPathSet<TPath, TNode> :
        IProductionRuleProjectionPathSet<TPath, TNode>,
        IEquatable<IProductionRuleProjectionDPathSet<TPath, TNode>>
        where TPath :
            IProductionRuleProjectionPath<TPath, TNode>
        where TNode :
            IProductionRuleProjectionNode<TPath, TNode>,
            new()
    {
        /// <summary>
        /// Returns the <typeparamref name="TNode"/> which
        /// contains the current <see cref="IProductionRuleProjectionDPathSet{TPath, TNode}"/>.
        /// </summary>
        TNode Root { get; }
        /// <summary>
        /// Denotes the <see cref="GrammarVocabulary"/> for which
        /// all of the elements are collected for.
        /// </summary>
        GrammarVocabulary Discriminator { get; }
        /// <summary>
        /// Returns the <see cref="IProductionRuleProjectionDPathSet{TPath, TNode}"/> which
        /// came firstSeries.
        /// </summary>
        IProductionRuleProjectionDPathSet<TPath, TNode> Previous { get; }
        /// <summary>
        /// Returns the <see cref="IControlledDictionary{TKey, TValue}"/>
        /// which denotes the look-ahead for the current
        /// machine, if necessary.
        /// </summary>
        IControlledDictionary<GrammarVocabulary, IProductionRuleProjectionDPathSet<TPath, TNode>> LookAhead { get; }
        /// <summary>
        /// Returns the current sequence of <see cref="GrammarVocabulary"/> yielded by the current setup.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> which denotes the
        /// sequence of <see cref="GrammarVocabulary"/> yielded by the
        /// current setup.
        /// </returns>
        IEnumerable<GrammarVocabulary> GetCurrentLookAheadStream();
        /// <summary>
        /// Returns the <see cref="LookAheadReductionType"/> which denotes
        /// the type of reduction employed at the current state.
        /// </summary>
        LookAheadReductionType ReductionType { get; }
        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> of <typeparamref name="TPath"/>
        /// instances of the original path in its unaltered form.
        /// </summary>
        /// <remarks>Due to reductions in look-ahead paths this
        /// is necessary to ensure proper path determination.</remarks>
        IEnumerable<TPath> UnalteredOriginals { get; }
        ProductionRuleProjectionNode ProjectedRootTarget { get; }
        GrammarVocabulary ProjectedRootTransition { get; }

        short ReductionCount { get; }
    }
}
