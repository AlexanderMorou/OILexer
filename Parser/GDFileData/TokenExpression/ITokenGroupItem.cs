using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Defines properties and methods for working with a grouped expression series inside
    /// a <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ITokenGroupItem :
        ITokenExpressionSeries,
        ITokenItem
    {
        /// <summary>
        /// Returns the line the <see cref="IGroupTokenItem"/> was declared at.
        /// </summary>
        new int Line { get; }
        /// <summary>
        /// Returns the column in the <see cref="Line"/> the <see cref="IGroupTokenItem"/>
        /// was declared at.
        /// </summary>
        new int Column { get; }
        /// <summary>
        /// Returns the index in the original stream the <see cref="IGroupTokenItem"/> was declared at.
        /// </summary>
        new long Position { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ITokenGroupItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITokenGroupItem"/> with the data
        /// members of the current <see cref="ITokenGroupItem"/>.</returns>
        new ITokenGroupItem Clone();
    }
}
