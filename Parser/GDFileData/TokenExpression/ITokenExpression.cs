using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Provides a series of <see cref="ITokenItem"/> instances in a chain which define a 
    /// <see cref="ITokenEntry"/> or a sub-expression in a <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ITokenExpression :
        IReadOnlyCollection<ITokenItem>,
        IAmbiguousGDEntity
    {
        /// <summary>
        /// Returns the line the <see cref="IItem"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the column in the <see cref="Line"/> the <see cref="IItem"/>
        /// was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the index in the original stream the <see cref="IItem"/> was declared at.
        /// </summary>
        long Position { get; }
        /// <summary>
        /// Returns the file at which the <see cref="IProductionRule"/> was defined.
        /// </summary>
        string FileName { get; }
    }
}
