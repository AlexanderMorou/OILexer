using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public interface ICommandTokenItem :
        ITokenItem 
    {
        /// <summary>
        /// Returns the <see cref="CommandType"/> associated to the command.
        /// </summary>
        CommandType Type { get; }
        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> set of 
        /// <see cref="ITokenExpressionSeries"/> which represents the
        /// parameters of the command.
        /// </summary>
        IEnumerable<ITokenExpressionSeries> Arguments { get; }
    }

    public enum CommandType
    {
        ScanCommand,
        SubtractCommand,
    }
}
