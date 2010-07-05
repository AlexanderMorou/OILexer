using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Defines properties and methods for working with an expression item defined 
    /// in a <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ITokenItem :
        IScannableEntryItem,
        ITokenSource
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ITokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ITokenItem"/> with the data
        /// members of the current <see cref="ITokenItem"/>.</returns>
        new ITokenItem Clone();
        /// <summary>
        /// Returns the default value associated with the <see cref="ITokenItem"/>.
        /// </summary>
        /// <remarks>
        /// If the <see cref="ITokenItem"/> is a literal, this refers to the default string value.
        /// If the <see cref="ITokenItem"/> is a <see cref="ITokenGroupItem"/>, the value
        /// refers to a soft-reference of a member contained within a simple literal series.
        /// </remarks>
        string DefaultSoftRefOrValue { get; }
    }
}
