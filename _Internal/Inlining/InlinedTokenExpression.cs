using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.Utilities.Collections;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Inlining
{
    /// <summary>
    /// Provides a base class for inlining an <see cref="ITokenExpression"/>.
    /// </summary>
    internal class InlinedTokenExpression :
        TokenExpression
    {
        /// <summary>
        /// Creates a new <see cref="InlinedTokenExpression"/> from
        /// the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="TokenExpression"/> from which the
        /// <see cref="InlinedTokenExpression"/> is derived.</param>
        public InlinedTokenExpression(ITokenExpression source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(InliningCore.Inline((IControlledStateCollection<ITokenItem>)source, sourceRoot, root, oldNewLookup), source.FileName, source.Column, source.Line, source.Position)
        {
            this.Source = source;
        }
        /// <summary>
        /// The <see cref="ITokenExpression"/> from which the current
        /// <see cref="InlinedTokenExpression"/> is derived.
        /// </summary>
        public ITokenExpression Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public ITokenEntry SourceRoot { get; private set; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="InlinedTokenExpression"/>.
        /// </summary>
        public InlinedTokenEntry Root { get; private set; }


    }
}
