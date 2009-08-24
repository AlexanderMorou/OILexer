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
        IReadOnlyCollection<ITokenExpression>
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
        string FileName { get; }
    }
}
