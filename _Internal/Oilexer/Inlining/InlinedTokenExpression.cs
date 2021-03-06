﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    /// <summary>
    /// Provides a base class for inlining an <see cref="ITokenExpression"/>.
    /// </summary>
    internal class InlinedTokenExpression :
        TokenExpression
    {
        private RegularLanguageNFAState nfaState;
        /// <summary>
        /// Creates a new <see cref="InlinedTokenExpression"/> from
        /// the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="TokenExpression"/> from which the
        /// <see cref="InlinedTokenExpression"/> is derived.</param>
        public InlinedTokenExpression(ITokenExpression source, IOilexerGrammarTokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(OilexerGrammarInliningCore.Inline((IControlledCollection<ITokenItem>)source, sourceRoot, root, oldNewLookup), source.FileName, source.Column, source.Line, source.Position)
        {
            this.Source = source;
        }
        /// <summary>
        /// The <see cref="ITokenExpression"/> from which the current
        /// <see cref="InlinedTokenExpression"/> is derived.
        /// </summary>
        public ITokenExpression Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public IOilexerGrammarTokenEntry SourceRoot { get; private set; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="InlinedTokenExpression"/>.
        /// </summary>
        public InlinedTokenEntry Root { get; private set; }

        public void BuildState(Dictionary<ITokenSource, ICaptureTokenStructuralItem> sourceReplacementLookup) 
        {
            RegularLanguageNFAState state = new RegularLanguageNFAState();
            foreach (var item in this.Cast<IInlinedTokenItem>())
            {
                item.BuildState(sourceReplacementLookup);
                state.Concat(item.State);
            }

            this.nfaState = state;
        }


        public RegularLanguageNFAState NFAState
        {
            get
            {
                return this.nfaState;
            }
        }

    }
}
