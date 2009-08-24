using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface IPreprocessorCLogicalOrConditionExp :
        IPreprocessorCExp
    {
        IPreprocessorCLogicalOrConditionExp Left { get; }
        IPreprocessorCLogicalAndConditionExp Right { get; }

    }
}
