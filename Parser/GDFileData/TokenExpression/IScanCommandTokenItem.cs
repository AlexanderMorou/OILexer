using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public interface IScanCommandTokenItem :
        ITokenItem 
    {
        /// <summary>
        /// Returns the target to search for.
        /// </summary>
        string SearchTarget { get; }

        /// <summary>
        /// Returns whether the <see cref="IScanCommandTokenItem"/> will seek past
        /// the <see cref="SearchTarget"/> and include it within the results.
        /// </summary>
        bool SeekPast { get; }
    }
}
