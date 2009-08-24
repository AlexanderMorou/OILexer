using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Inlining
{
    /// <summary>
    /// Provides a base class for inlining a <see cref="TokenExpressionSeries"/>.
    /// </summary>
    internal class InlinedTokenExpressionSeries :
        TokenExpressionSeries
    {
        /// <summary>
        /// Creates a new <see cref="InlinedTokenExpressionSeries"/> with the 
        /// <paramref name="source"/>, <paramref name="sourceRoot"/> and
        /// <paramref name="root"/>
        /// </summary>
        /// <param name="source">The <see cref="TokenExpressionSeries"/> from which the current
        /// <see cref="InlinedTokenExpressionSeries"/> derives.</param>
        /// <param name="sourceRoot">The <see cref="ITokenEntry"/> which contains the 
        /// <paramref name="source"/>.</param>
        /// <param name="root">The <see cref="InlinedTokenEntry"/> which contains the <see cref="InlinedTokenExpressionSeries"/>.</param>
        public InlinedTokenExpressionSeries(ITokenExpressionSeries source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(InliningCore.Inline(source.ToArray(), sourceRoot, root, oldNewLookup), source.Line, source.Column, source.Position, source.FileName)
        {
            this.Source = source;
            this.SourceRoot = SourceRoot;
            this.Root = root;
        }

        public ITokenExpressionSeries Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public ITokenEntry SourceRoot { get; private set; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="InlinedTokenExpressionSeries"/>.
        /// </summary>
        public InlinedTokenEntry Root { get; private set; }

    }
}
