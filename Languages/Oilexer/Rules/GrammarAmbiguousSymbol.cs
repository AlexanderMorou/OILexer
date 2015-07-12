using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a base implementation of an ambiguous symbol.
    /// </summary>
    public class GrammarAmbiguousSymbol :
        ControlledCollection<IGrammarTokenSymbol>,
        IGrammarAmbiguousSymbol
    {
        /// <summary>
        /// Data member for <see cref="AmbiguityKey"/>.
        /// </summary>
        private GrammarVocabulary _ambiguityKey;
        /// <summary>
        /// <para>Denotes the <see cref="IGrammarSymbolSet"/> which represents the grammar's full set of symbols.</para>
        /// <para>Data member aids in creating the <see cref="AmbiguityKey"/>.</para>
        /// </summary>
        private IGrammarSymbolSet _symbols;

        private RegularLanguageDFAState[] _edgeStates;

        /// <summary>
        /// Creates a new <see cref="GrammarAmbiguousSymbol"/> with the <paramref name="symbols"/>, <paramref name="set"/>, <paramref name="ambiguityNumber"/> and <paramref name="ambiguityCount"/> provided.
        /// </summary>
        /// <param name="symbols">The <see cref="IGrammarSymbolSet"/> which denotes the grammar's full set of symbols.</param>
        /// <param name="set">The <see cref="IEnumerable{T}"/> which denotes the <see cref="IGrammarTokenSymbol"/> set represented by the <see cref="GrammarAmbiguousSymbol"/>.</param>
        /// <param name="ambiguityNumber">The <see cref="Int32"/> value denoting the ordinal number of the ambiguity relative to the full set of ambiguities.</param>
        /// <param name="ambiguityCount">The <see cref="Int32"/> value denoting the total number of ambiguities within the current grammar.</param>
        public GrammarAmbiguousSymbol(IGrammarSymbolSet symbols, IEnumerable<IGrammarTokenSymbol> set, int ambiguityNumber, int ambiguityCount, RegularLanguageDFAState[] edgeStates)
            : base(set.ToList())
        {
            this._symbols = symbols;
            this.AmbiguityCount = ambiguityCount;
            this.AmbiguityNumber = ambiguityNumber;
            this._edgeStates = edgeStates;
        }

        /// <summary>Returns the <see cref="GrammarVocabulary"/> necessary for matching the <see cref="GrammarAmbiguousSymbol"/></summary>
        /// <value><see cref="_ambiguityKey"/> and <see cref="_symbols"/>.</value>
        public GrammarVocabulary AmbiguityKey { get { return this._ambiguityKey ?? (this._ambiguityKey = new GrammarVocabulary(this._symbols, this.ToArray())); } }

        /// <summary>Returns the <see cref="String"/> value which represents the <see cref="GrammarAmbiguousSymbol"/>'s name.</summary>
        public string ElementName { get { return string.Format(string.Format("Ambiguity{{0:{0}}}", '0'.Repeat(this.AmbiguityCount.ToString().Length)), this.AmbiguityNumber); } }

        /// <summary>Returns the <see cref="Int32"/> value denoting the ordinal number of the ambiguity relative to the full set of ambiguities.</summary>
        public int AmbiguityNumber { get; internal set; }

        /// <summary>Returns the <see cref="Int32"/> value denoting the total number of ambiguities within the current grammar.</summary>
        public int AmbiguityCount { get; internal set; }

        /// <summary>Returns the <see cref="String"/> value which represents the current <see cref="GrammarAmbiguousSymbol"/></summary>
        /// <returns>A <see cref="String"/> value which represents the current <see cref="GrammarAmbiguousSymbol"/></returns>
        public override string ToString() { return string.Format("{0} {{{1}}}", this.ElementName, this.AmbiguityKey); }

        /// <summary>
        /// Returns/sets the <see cref="Int32"/> value denoting the number of times a given <see cref="GrammarAmbiguousSymbol"/> has been observed
        /// within the result grammar.
        /// </summary>
        /// <remarks>Ambiguities which are not observed are pruned from the langauge at a later time.</remarks>
        public int Occurrences { get; set; }

        public RegularLanguageDFAState[] EdgeStates { get { return this._edgeStates; } }
    }
}
