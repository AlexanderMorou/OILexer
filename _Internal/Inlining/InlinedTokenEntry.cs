using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
using System.Linq;
using Oilexer.FiniteAutomata.Tokens;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Inlining
{
    /// <summary>
    /// Provides a base class to inline an <see cref="ITokenEntry"/>.
    /// </summary>
    internal class InlinedTokenEntry :
        TokenEntry
    {
        private RegularCaptureType? captureType = null;
        private RegularLanguageNFARootState nfaState;
        private RegularLanguageDFARootState dfaState;
        /// <summary>
        /// Returns the <see cref="Dictionary{TKey, TValue}"/> which 
        /// </summary>
        public Dictionary<ITokenItem, ITokenItem> OldNewLookup { get; private set; }
        /// <summary>
        /// Creates a new <see cref="InlinedTokenEntry"/> with the <paramref name="source"/>
        /// provided.
        /// </summary>
        /// <param name="source">The <see cref="ITokenEntry"/> from which the 
        /// current <see cref="InlinedTokenEntry"/> derives.</param>
        public InlinedTokenEntry(ITokenEntry source)
            : base(source.Name, null, source.ScanMode, source.FileName, source.Column, source.Line, source.Position, source.Unhinged, source.LowerPrecedenceTokens, source.ForcedRecognizer)
        {
            this.OldNewLookup = new Dictionary<ITokenItem, ITokenItem>();
            this.branches = InliningCore.Inline(source.Branches, source, this, this.OldNewLookup);
            this.Source = source;
        }

        internal void ResolveLowerPrecedencesAgain(Dictionary<ITokenEntry, InlinedTokenEntry> originalNewLookup)
        {
            List<ITokenEntry> lowerPrecedences = new List<ITokenEntry>();
            if (this.LowerPrecedenceTokens != null)
                foreach (var token in this.LowerPrecedenceTokens)
                    if (originalNewLookup.ContainsKey(token))
                        lowerPrecedences.Add(originalNewLookup[token]);
            this.LowerPrecedenceTokens = lowerPrecedences.ToArray();
        }

        public void BuildNFA()
        {
            if (this.Source is ITokenEofEntry)
                return;
            this.nfaState = new RegularLanguageNFARootState(this);
            bool first = true;
            foreach (var expression in this.Branches.Cast<InlinedTokenExpression>())
            {
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
        }

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> from which
        /// the current <see cref="InlinedTokenEntry"/> derives.
        /// </summary>
        public ITokenEntry Source { get; private set; }

        public void BuildDFA()
        {
            if (this.Source is ITokenEofEntry)
                return;
            if (this.nfaState == null)
                return;
            this.dfaState = this.nfaState.DeterminateAutomata();
        }

        public void ReduceDFA()
        {
            if (this.Source is ITokenEofEntry)
                return;
            if (this.dfaState == null)
                return;
            this.captureType = this.DetermineKind();
            this.dfaState.Reduce(this.captureType.Value);
            this.dfaState.Enumerate();
        }

        public RegularCaptureType DetermineKind()
        {
            if (this.ForcedRecognizer)
                return RegularCaptureType.Recognizer;
            RegularCaptureType result = DetermineKind(this);
            return result;
        }

        private static RegularCaptureType DetermineKind(ITokenEntry target)
        {
            return DetermineKind(target.Branches);
        }

        internal static RegularCaptureType DetermineKind(ITokenExpressionSeries series)
        {
            RegularCaptureType result = RegularCaptureType.Undecided;
            foreach (var expression in series)
            {
                var currentKind = DetermineKind(expression);
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
                        break;
                }
            }
            if (result == RegularCaptureType.Undecided)
                result = RegularCaptureType.Recognizer;
            return result;
        }

        internal static RegularCaptureType DetermineKind(ITokenExpression target)
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
                    var refKind = DetermineKind(refItem.Reference);
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
                    var gRepOpt = DetermineKind(gFirst);
                    if (gRepOpt == RegularCaptureType.Transducer &&
                        gFirst.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                        return RegularCaptureType.Recognizer;
                    return gRepOpt;
                }
            }
        other: 
            foreach (var element in target)
                if (!string.IsNullOrEmpty(element.Name))
                    return RegularCaptureType.Capturer;
            return RegularCaptureType.Recognizer;
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
        }
    }
}
