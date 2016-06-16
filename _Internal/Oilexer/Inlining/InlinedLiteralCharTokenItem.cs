using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    /// <summary>
    /// Provides a base class from which a <see cref="ILiteralCharTokenItem"/>
    /// can be inlined.
    /// </summary>
    internal class InlinedLiteralCharTokenItem :
        LiteralCharTokenItem,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;
        /// <summary>
        /// Creates a new <see cref="InlinedLiteralCharTokenItem"/> instance
        /// with the <paramref name="source"/>, <paramref name="sourceRoot"/>
        /// and <paramref name="root"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="ILiteralCharTokenItem"/> from which the current
        /// <see cref="InlinedLiteralCharTokenItem"/> is derived.</param>
        /// <param name="sourceRoot">The <see cref="IOilexerGrammarTokenEntry"/> from which
        /// the <paramref name="source"/> is derived.</param>
        /// <param name="root">The <see cref="InlinedTokenEntry"/> in which
        /// the current K<see cref="InlinedLiteralCharTokenItem"/> is contained within.</param>
        public InlinedLiteralCharTokenItem(ILiteralCharTokenItem source, IOilexerGrammarTokenEntry sourceRoot, InlinedTokenEntry root)
            : base(source.Value, source.CaseInsensitive, source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.SiblingAmbiguity = source.SiblingAmbiguity;
            this.Name = source.Name;
            if (source.IsFlag.HasValue)
            {
                this.IsFlag = source.IsFlag;
                this.IsFlagToken = source.IsFlagToken;
            }
        }

        /// <summary>
        /// Returns the <see cref="ILiteralCharTokenItem"/> from which the 
        /// <see cref="InlinedLiteralCharTokenItem"/> is derived.
        /// </summary>
        public ILiteralCharTokenItem Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public IOilexerGrammarTokenEntry SourceRoot { get; private set; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="InlinedLiteralCharTokenItem"/>.
        /// </summary>
        public InlinedTokenEntry Root { get; private set; }

        #region IInlinedTokenItem Members

        ITokenItem IInlinedTokenItem.Source
        {
            get { return this.Source; }
        }

        public RegularLanguageNFAState State
        {
            get
            {
                return this.state;
            }
        }

        #endregion

        #region IInlinedTokenItem Members


        public void BuildState(Dictionary<ITokenSource, Captures.ICaptureTokenStructuralItem> sourceReplacementLookup)
        {
            var thisReplacement = sourceReplacementLookup.ContainsKey(this) ? (ITokenSource)(sourceReplacementLookup[this]) : (ITokenSource)this;
            RegularLanguageNFAState rootState = new RegularLanguageNFAState();
            RegularLanguageNFAState endState = new RegularLanguageNFAState();
            rootState.MoveTo(new RegularLanguageSet(!this.CaseInsensitive, this.Value), endState);
            rootState.SetInitial(thisReplacement);
            endState.SetFinal(thisReplacement);
            rootState.HandleRepeatCycle<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource, RegularLanguageNFARootState, IInlinedTokenItem>(this, thisReplacement, OilexerGrammarInliningCore.TokenRootStateClonerCache, OilexerGrammarInliningCore.TokenStateClonerCache);
            this.state = rootState;
        }

        #endregion
    }
}
