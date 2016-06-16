using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
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
        EncodeUnicode
    }
}
