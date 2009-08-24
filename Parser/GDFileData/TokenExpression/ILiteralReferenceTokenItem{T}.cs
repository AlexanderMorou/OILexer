using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="ILiteralReferenceTokenItem"/> with
    /// a speific data-type.
    /// </summary>
    /// <typeparam name="T">The type of value stored in the <typeparamref name="TLiteral"/>.</typeparam>
    /// <typeparam name="TLiteral">The type of literal.</typeparam>
    public interface ILiteralReferenceTokenItem<T, TLiteral> :
        ILiteralReferenceTokenItem
        where TLiteral :
            ILiteralTokenItem<T>
    {
        /// <summary>
        /// Returns the literal the <see cref="ILiteralReferenceTokenItem{T, TLiteral}"/> references.
        /// </summary>
        new TLiteral Literal { get; }
    }
}
