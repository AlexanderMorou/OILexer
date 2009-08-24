using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Defines properties and methods for working with a generic <see cref="ILiteralTokenItem"/>
    /// of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value represented by the constant.</typeparam>
    public interface ILiteralTokenItem<T>  :
        ILiteralTokenItem
    {
        /// <summary>
        /// Returns the value defined by the <see cref="ILiteralTokenItem{T}"/>.
        /// </summary>
        new T Value { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralTokenItem{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralTokenItem{T}"/> with the data
        /// members of the current <see cref="ILiteralTokenItem{T}"/>.</returns>
        new ILiteralTokenItem<T> Clone();
    }
}
