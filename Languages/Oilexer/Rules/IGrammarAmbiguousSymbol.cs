using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface IGrammarAmbiguousSymbol :
        IControlledCollection<IGrammarTokenSymbol>,
        IGrammarSymbol
    {
        /// <summary>
        /// Returns the <see cref="GrammarVocabulary"/> necessary to match the <see cref="IGrammarAmbiguousSymbol"/>.
        /// </summary>
        GrammarVocabulary AmbiguityKey { get; }
        /// <summary>
        /// Returns the <see cref="Int32"/> value denoting the ordinal number of the ambiguity relative to the full set of ambiguities.
        /// </summary>
        int AmbiguityNumber { get; }
        /// <summary>
        /// Returns the <see cref="Int32"/> value denoting the total number of ambiguities within the current grammar.
        /// </summary>
        int AmbiguityCount { get; }
        /// <summary>
        /// Returns/sets the <see cref="Int32"/> value denoting the frequency at which a given ambiguity occurs.
        /// </summary>
        int Occurrences { get; set; }
        RegularLanguageDFAState[] EdgeStates { get; }
    }
}
