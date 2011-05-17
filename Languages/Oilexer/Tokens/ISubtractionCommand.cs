using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public interface ISubtractionCommandTokenItem :
        ICommandTokenItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenExpressionSeries"/> which
        /// denotes the positive set of the expression.
        /// </summary>
        ITokenExpressionSeries Left { get; }
        /// <summary>
        /// Returns the <see cref="ITokenExpressionSeries"/> which
        /// denotes the negative set of the expression.
        /// </summary>
        /// <remarks>
        /// <para>Basic concept is simple: All edge states from
        /// the <see cref="Right"/> set will yield a non-edge on the
        /// <see cref="Left"/> set, even if the edge was marked on the
        /// left set.</para>
        /// <para>Additionally, any time the start state of the 
        /// negative set is entered, a field relative to the negative
        /// assertion is set to the current length of the parse.  
        /// This is to ensure that the valid length returned
        /// is the last point before the subtracted set matched.
        /// </para>
        /// <para>Upon determining exit length, a switch shall be
        /// used across the exit state to determine whether the
        /// full length should be used or the negative assertion
        /// start point should be used.</para>
        /// </remarks>
        ITokenExpressionSeries Right { get; }
    }
}
