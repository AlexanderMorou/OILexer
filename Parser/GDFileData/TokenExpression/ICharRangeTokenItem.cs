using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public interface ICharRangeTokenItem :
        ITokenItem
    {
        BitArray Range { get; }
        bool Inverted { get; }
    }
}
