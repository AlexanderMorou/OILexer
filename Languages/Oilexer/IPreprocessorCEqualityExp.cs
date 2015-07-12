using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IPreprocessorCEqualityExp :
        IPreprocessorCExp
    {
        /// <summary>
        /// Returns the rule that was matched.
        /// </summary>
        /// <remarks>Can either be:
        /// 1 - PreprocessorCEqualityExp "==" PreprocessorCPrimary, 
        /// 2 - PreprocessorCEqualityExp "!=" PreprocessorCPrimary,
        /// 3 - PreprocessorCPrimary</remarks>
        int Rule { get; }
        IPreprocessorCEqualityExp PreCEqualityExp { get; }
        IPreprocessorCPrimary PreCPrimary { get; }
    }
}
