using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
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
