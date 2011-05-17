using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IPreprocessorCExp
    {
        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="IPreprocessorCExp"/> was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the line index the <see cref="IPreprocessorCExp"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the position the <see cref="IPreprocessorCExp"/> was declared at.
        /// </summary>
        long Position { get; }
    }
}
