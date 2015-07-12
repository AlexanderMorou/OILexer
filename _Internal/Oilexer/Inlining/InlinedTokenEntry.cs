using System.Collections.Generic;
using System.Linq;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
using AllenCopeland.Abstraction.Utilities.Collections;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    /// <summary>
    /// Provides a base class to inline an <see cref="IOilexerGrammarTokenEntry"/>.
    /// </summary>
    internal class InlinedTokenEntry :
        OilexerGrammarTokenEntry
    {
        private RegularCaptureType? captureType = null;
        private RegularLanguageNFARootState nfaState;
        private RegularLanguageDFARootState dfaState;
        private ICaptureTokenStructure structure;
        IOilexerGrammarFile file;
        /// <summary>
        /// Returns the <see cref="Dictionary{TKey, TValue}"/> which 
        /// </summary>
        public Dictionary<ITokenItem, ITokenItem> OldNewLookup { get; private set; }
        /// <summary>
        /// Creates a new <see cref="InlinedTokenEntry"/> with the <paramref name="source"/>
        /// provided.
        /// </summary>
        /// <param name="source">The <see cref="IOilexerGrammarTokenEntry"/> from which the 
        /// current <see cref="InlinedTokenEntry"/> derives.</param>
        public InlinedTokenEntry(IOilexerGrammarTokenEntry source, IOilexerGrammarFile file)
            : base(source.Name, null, source.ScanMode, source.FileName, source.Column, source.Line, source.Position, source.Unhinged, source.LowerPrecedenceTokens, source.ForcedRecognizer)
        {
            this.OldNewLookup = new Dictionary<ITokenItem, ITokenItem>();
            this.branches = OilexerGrammarInliningCore.Inline(source.Branches, source, this, this.OldNewLookup);
            this.Source = source;
            this.file = file;
            this.Contextual = source.Contextual;
        }

        internal void ResolveLowerPrecedencesAgain(Dictionary<IOilexerGrammarTokenEntry, InlinedTokenEntry> originalNewLookup)
        {
            List<IOilexerGrammarTokenEntry> lowerPrecedences = new List<IOilexerGrammarTokenEntry>();
            if (this.LowerPrecedenceTokens != null)
                foreach (var token in this.LowerPrecedenceTokens)
                    if (originalNewLookup.ContainsKey(token))
                        lowerPrecedences.Add(originalNewLookup[token]);
            this.LowerPrecedenceTokens = lowerPrecedences.ToArray();
        }

        public void BuildNFA(IOilexerGrammarFile source)
        {
            if (this.Source is IOilexerGrammarTokenEofEntry)
                return;
            this.captureType = this.DetermineKind();
            Dictionary<ITokenSource, ICaptureTokenStructuralItem> replacements = new Dictionary<ITokenSource, ICaptureTokenStructuralItem>();
            if (captureType.Value != RegularCaptureType.Recognizer)
            {
                this.structure = TokenStructuralExtractionCore.BuildStructureFor(this, source);
                replacements = TokenStructuralExtractionCore.ObtainReplacements(this.structure);
            }
            else
                replacements = new Dictionary<ITokenSource, ICaptureTokenStructuralItem>();
            this.nfaState = new RegularLanguageNFARootState(this);
            bool first = true;
            foreach (var expression in this.Branches.Cast<InlinedTokenExpression>())
            {
                expression.BuildState(replacements);
                var expressionNFA = expression.NFAState;
                if (first)
                {
                    bool isEdge = expressionNFA.IsEdge;
                    first = false;
                    nfaState.Union(expression.NFAState);
                    if (nfaState.IsEdge && !isEdge)
                        nfaState.IsEdge = isEdge;
                }
                else
                    nfaState.Union(expression.NFAState);
            }
            if (ForcedRecognizer)
            {
                List<RegularLanguageNFAState> flatline = new List<RegularLanguageNFAState>();
                RegularLanguageNFAState.FlatlineState(this.nfaState, flatline);
                foreach (var state in flatline)
                    state.IgnoreSources = true;
            }
        }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> from which
        /// the current <see cref="InlinedTokenEntry"/> derives.
        /// </summary>
        public IOilexerGrammarTokenEntry Source { get; private set; }

        public void BuildDFA()
        {
            if (this.Source is IOilexerGrammarTokenEofEntry)
                return;
            if (this.nfaState == null)
                return;
            this.dfaState = this.nfaState.DeterminateAutomata();

        }

        public void ReduceDFA()
        {
            if (this.Source is IOilexerGrammarTokenEofEntry)
                return;
            if (this.dfaState == null)
                return;
            this.dfaState.Reduce(this.captureType.Value);
            //this.dfaState.Enumerate();
        }

        public RegularCaptureType DetermineKind()
        {
            if (this.ForcedRecognizer)
                return RegularCaptureType.Recognizer;
            RegularCaptureType result = DetermineKind(this, this.file);
            return result;
        }

        private static RegularCaptureType DetermineKind(IOilexerGrammarTokenEntry target, IOilexerGrammarFile file)
        {
            return DetermineKind(target, target.Branches, file);
        }

        internal static RegularCaptureType DetermineKind(IOilexerGrammarTokenEntry entry, ITokenExpressionSeries series, IOilexerGrammarFile file)
        {
            RegularCaptureType result = RegularCaptureType.Undecided;
            foreach (var expression in series)
            {
                var currentKind = DetermineKind(entry, expression, file);
                switch (currentKind)
                {
                    case RegularCaptureType.Recognizer:
                        if (result == RegularCaptureType.Undecided)
                            result = currentKind;
                        break;
                    case RegularCaptureType.Capturer:
                        result = currentKind;
                        break;
                    case RegularCaptureType.Transducer:
                        if (result == RegularCaptureType.Undecided)
                            result = RegularCaptureType.Transducer;
                        else if (result == RegularCaptureType.Recognizer)
                            result = RegularCaptureType.Capturer;
                        break;
                }
            }
            if (result == RegularCaptureType.Undecided)
                result = RegularCaptureType.Recognizer;

            if (result == RegularCaptureType.Transducer)
            {
                if ((from gdEntry in file
                     where gdEntry is IOilexerGrammarProductionRuleEntry
                     let ruleEntry = (IOilexerGrammarProductionRuleEntry)gdEntry
                     from productionRuleItem in GetProductionRuleSeriesItems(ruleEntry)
                     where productionRuleItem is ILiteralReferenceProductionRuleItem
                     let literalItem = (ILiteralReferenceProductionRuleItem)productionRuleItem
                     where literalItem.Source == entry
                     select literalItem).Count() > 0)
                    return RegularCaptureType.ContextfulTransducer;

            }
            return result;
        }

        private static IEnumerable<IProductionRuleItem> GetProductionRuleSeriesItems(IProductionRuleSeries series)
        {
            foreach (var rule in series)
                foreach (var item in GetProductionRuleItems(rule))
                    yield return item;
        }

        private static IEnumerable<IProductionRuleItem> GetProductionRuleItems(IProductionRule rule)
        {
            foreach (var item in rule)
            {
                yield return item;
                if (item is IProductionRuleGroupItem)
                    foreach (var entry in GetProductionRuleSeriesItems((IProductionRuleGroupItem)item))
                        yield return entry;
            }
        }

        internal static RegularCaptureType DetermineKind(IOilexerGrammarTokenEntry entry, ITokenExpression target, IOilexerGrammarFile file)
        {
            //
            if (target.Count == 1)
            {
                /* *
                 * If the expression consists of a single named literal, then
                 * for the sake of this one expression, the best result is a 
                 * transducer type state machine.
                 * *
                 * When mixed with a set of singular named literals, the resulted
                 * state machine is a transducer which reports back the exact 
                 * phrase matched.  Since the strings are all known, they can be
                 * boiled down to a finite set of numbers.
                 * */
                var first = target[0];
                if (first is ILiteralTokenItem &&
                   !string.IsNullOrEmpty(first.Name) &&
                    first.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                    return RegularCaptureType.Transducer;
                if (first is ITokenReferenceTokenItem)
                {
                    var refItem = first as ITokenReferenceTokenItem;
                    var refKind = DetermineKind(refItem.Reference, file);
                    if (refKind == RegularCaptureType.Recognizer)
                        goto other;
                    else if (refKind == RegularCaptureType.Capturer)
                        return RegularCaptureType.Capturer;
                    return RegularCaptureType.Transducer;
                }
                else if (first is ILiteralReferenceTokenItem)
                {
                    var lFirst = (first as ILiteralReferenceTokenItem).Literal;
                    if (lFirst is ILiteralTokenItem &&
                       !string.IsNullOrEmpty(lFirst.Name) &&
                        lFirst.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                        return RegularCaptureType.Transducer;

                }
                else if (first is ITokenGroupItem)
                {
                    var gFirst = first as ITokenGroupItem;
                    var gRepOpt = DetermineKind(entry, gFirst, file);
                    if (gRepOpt == RegularCaptureType.Transducer &&
                        gFirst.RepeatOptions != ScannableEntryItemRepeatInfo.None)
                        return RegularCaptureType.Recognizer;
                    return gRepOpt;
                }
            }
        other:
            if (AnyContainAName(target))
                return RegularCaptureType.Capturer;

            return RegularCaptureType.Recognizer;
        }

        private static bool AnyContainAName(ITokenExpressionSeries series)
        {
            foreach (var expression in series)
                if (AnyContainAName(expression))
                    return true;
            return false;
        }

        private static bool AnyContainAName(ITokenExpression expression)
        {
            foreach (var item in expression)
                if (item is ITokenGroupItem)
                {
                    if (AnyContainAName((ITokenGroupItem)item))
                        return true;
                }
                else if (!item.Name.IsEmptyOrNull())
                    return true;
            return false;
        }

        public RegularLanguageNFAState NFAState
        {
            get
            {
                return this.nfaState;
            }
        }

        public RegularLanguageDFARootState DFAState
        {
            get
            {
                return this.dfaState;
            }
            internal set { this.dfaState = value; }
        }

        public RegularCaptureType CaptureKind
        {
            get
            {
                if (this.captureType.HasValue)
                    return this.captureType.Value;
                else
                    return RegularCaptureType.Undecided;
            }
        }
        public ICaptureTokenStructure CaptureStructure
        {
            get
            {
                return this.structure;
            }
        }
    }
}
