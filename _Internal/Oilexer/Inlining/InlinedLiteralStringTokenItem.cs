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
    /// Provides a base class from which 
    /// a literal string token item can derive.
    /// </summary>
    internal class InlinedLiteralStringTokenItem :
        LiteralStringTokenItem,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;
        /// <summary>
        /// Creates a new <see cref="InlinedLiteralStringTokenItem"/> with the
        /// <paramref name="source"/>, and <paramref name="parent"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="ILiteralStringTokenItem"/> from which
        /// the <see cref="InlinedLiteralStringTokenItem"/> is derived.</param>
        /// <param name="sourceRoot">The <see cref="IOilexerGrammarTokenEntry"/> which contains
        /// the <paramref name="source"/>.</param>
        /// <param name="root">The <see cref="InlinedTokenEntry"/> which roots the entire
        /// token structure.</param>
        public InlinedLiteralStringTokenItem(ILiteralStringTokenItem source, IOilexerGrammarTokenEntry sourceRoot, InlinedTokenEntry root)
            : base(source.Value, source.CaseInsensitive, source.Column, source.Line, source.Position, source.SiblingAmbiguity)
        {
            this.SourceRoot = sourceRoot;
            this.Source = source;
            this.Root = root;
            this.SiblingAmbiguity = source.SiblingAmbiguity;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;
        }

        /// <summary>
        /// Returns the <see cref="ILiteralStringTokenItem"/> from which the
        /// current <see cref="InlinedLiteralStringTokenItem"/> is derived.
        /// </summary>
        public ILiteralStringTokenItem Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public IOilexerGrammarTokenEntry SourceRoot { get; private set; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="InlinedLiteralStringTokenItem"/>.
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
            var result = this.BuildStringState(thisReplacement, this.CaseInsensitive, this.Value);
            result.HandleRepeatCycle<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource, RegularLanguageNFARootState, IInlinedTokenItem>(this, thisReplacement, OilexerGrammarInliningCore.TokenRootStateClonerCache, OilexerGrammarInliningCore.TokenStateClonerCache);
            this.state = result;
        }

        #endregion
    }
}
