using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface IPreprocessorCLogicalAndConditionExp :
        IPreprocessorCExp
    {
        IPreprocessorCLogicalAndConditionExp Left { get; }
        IPreprocessorCEqualityExp Right { get; }
    }
}
