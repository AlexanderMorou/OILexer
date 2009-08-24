using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Defines a constant used in a <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ILiteralTokenItem :
        ITokenItem
    {
        /// <summary>
        /// Returns the value defined by the <see cref="ILiteralTokenItem"/>.
        /// </summary>
        object Value { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralTokenItem"/> with the data
        /// members of the current <see cref="ILiteralTokenItem"/>.</returns>
        new ILiteralTokenItem Clone();
        /// <summary>
        /// Returns whether the captured literal is a flag.  That is, if captured successfully,
        /// is stored as a boolean value only.  Only applies to optional members.
        /// </summary>
        bool? IsFlag { get; }
    }
}
