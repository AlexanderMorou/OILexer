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
        /// <param name="sourceRoot">The <see cref="ITokenEntry"/> from which
        /// the <paramref name="source"/> is derived.</param>
        /// <param name="root">The <see cref="InlinedTokenEntry"/> in which
        /// the current K<see cref="InlinedLiteralCharTokenItem"/> is contained within.</param>
        public InlinedLiteralCharTokenItem(ILiteralCharTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root)
            : base(source.Value, source.CaseInsensitive, source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;
        }

        /// <summary>
        /// Returns the <see cref="ILiteralCharTokenItem"/> from which the 
        /// <see cref="InlinedLiteralCharTokenItem"/> is derived.
        /// </summary>
        public ILiteralCharTokenItem Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public ITokenEntry SourceRoot { get; private set; }

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
            RegularLanguageNFAState rootState = new RegularLanguageNFAState();
            RegularLanguageNFAState endState = new RegularLanguageNFAState();
            rootState.MoveTo(new RegularLanguageSet(!this.CaseInsensitive, this.Value), endState);
            rootState.SetInitial(this);
            endState.SetFinal(this);
            return rootState;
        }

        #endregion
    }
}
