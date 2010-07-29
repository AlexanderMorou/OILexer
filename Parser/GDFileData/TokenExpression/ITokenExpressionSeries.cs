using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Defines properties and methods for working with an expression series that define
    /// the branches of an <see cref="ITokenEntry"/> or <see cref="IGroupTokenItem"/>
    /// </summary>
    public interface ITokenExpressionSeries :
        IReadOnlyCollection<ITokenExpression>,
        ITokenSource
    {
        /// <summary>
        /// Returns the line the <see cref="ITokenExpressionSeries"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the column in the <see cref="Line"/> the <see cref="ITokenExpressionSeries"/>
        /// was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the index in the original stream the <see cref="ITokenExpressionSeries"/> was declared at.
        /// </summary>
        long Position { get; }
        /// <summary>
        /// Returns the <see cref="String"/> representing the
        /// name of the file in which the <see cref="ITokenExpressionSeries"/>
        /// was defined.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Obtains the string form of the body of the 
        /// <see cref="ITokenExpressionSeries"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> representing
        /// the elements within the description of the
        /// <see cref="ITokenExpressionSeries"/>.</returns>
        string GetBodyString();
    }
}
