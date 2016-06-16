using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    public interface IToken
    {
        /// <summary>
        /// Returns the character in the current <see cref="Line"/> the <see cref="IToken"/> starts at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the line the <see cref="IToken"/> is at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the stream position for the <see cref="IToken"/>.
        /// </summary>
        long Position { get; }
        /// <summary>
        /// Returns the length of the <see cref="IToken"/>.
        /// </summary>
        /// <remarks>Used to flush the buffer using the proper length.</remarks>
        int Length { get; }
        /// <summary>
        /// Returns whether or not the <see cref="IToken"/> has already consumed the buffer
        /// relative to its <see cref="Length"/>.
        /// </summary>
        bool ConsumedFeed { get; }
    }
}
