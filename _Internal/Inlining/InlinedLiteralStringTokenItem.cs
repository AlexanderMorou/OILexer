using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.FiniteAutomata.Tokens;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Inlining
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
        /// <param name="sourceRoot">The <see cref="ITokenEntry"/> which contains
        /// the <paramref name="source"/>.</param>
        /// <param name="root">The <see cref="InlinedTokenEntry"/> which roots the entire
        /// token structure.</param>
        public InlinedLiteralStringTokenItem(ILiteralStringTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root)
            : base(source.Value, source.CaseInsensitive, source.Column, source.Line, source.Position, source.SiblingAmbiguity)
        {
            this.SourceRoot = sourceRoot;
            this.Source = source;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;
        }

        /// <summary>
        /// Returns the <see cref="ILiteralStringTokenItem"/> from which the
        /// current <see cref="InlinedLiteralStringTokenItem"/> is derived.
        /// </summary>
        public ILiteralStringTokenItem Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public ITokenEntry SourceRoot { get; private set; }

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
            get {
                if (this.state == null)
                {
                    this.state = this.BuildNFAState();
                    this.state.HandleRepeatCycle<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource, RegularLanguageNFARootState, IInlinedTokenItem>(this, InliningCore.TokenRootStateClonerCache, InliningCore.TokenStateClonerCache);
                }
                return this.state;
            }
        }

        private RegularLanguageNFAState BuildNFAState()
        {
            return this.BuildStringState(this.CaseInsensitive, this.Value);
        }

        #endregion
    }
}
