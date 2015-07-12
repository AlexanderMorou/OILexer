using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Defines properties and methods for working with a scan command
    /// which seeks to a point in the source based upon the 
    /// <paramref name="SearchTarget"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <paramref name="IScanCommandTokenItem.SeekPast"/> is true,
    /// then the terminal edges for the scan command are the terminal
    /// edges of the <see cref="IScanCommandTokenItem.SearchTarget"/>;
    /// otherwise the terminal edges are the non-terminal edges of 
    /// the <see cref="IScanCommandTokenItem.SearchTarget"/>.
    /// </para>
    /// <para>
    /// Scan(Expression, false) is equivalent to:
    /// Subtract([\0-\uFFFF]*, Expression)
    /// </para>
    /// </remarks>
    public interface IScanCommandTokenItem :
        ICommandTokenItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenExpressionSeries"/> which makes up the 
        /// point to scan to.
        /// </summary>
        ITokenExpressionSeries SearchTarget { get; }
        /// <summary>
        /// Returns whether the <see cref="ICommandTokenItem"/> will seek past
        /// the <see cref="SearchTarget"/> and include it within the results.
        /// </summary>
        bool SeekPast { get; }
    }
}
