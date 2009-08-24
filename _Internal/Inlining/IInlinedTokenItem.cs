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
    /// Defines properties and methods for working with an inlined token item.
    /// </summary>
    internal interface IInlinedTokenItem :
        ITokenItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenItem"/> from which the current
        /// <see cref="IInlinedTokenItem"/> is derived.
        /// </summary>
        ITokenItem Source { get; }
        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        ITokenEntry SourceRoot { get; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="IInlinedTokenItem"/>.
        /// </summary>
        InlinedTokenEntry Root { get; }
    }
}
