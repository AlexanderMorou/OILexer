using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="System.String"/>
    /// literal defined in an <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ILiteralStringTokenItem :
        ILiteralTokenItem<string>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralStringTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralStringTokenItem"/> with the data
        /// members of the current <see cref="ILiteralStringTokenItem"/>.</returns>
        new ILiteralStringTokenItem Clone();
        /// <summary>
        /// Returns whether the <see cref="ILiteralStringTokenItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        bool CaseInsensitive { get; }
    }
}
