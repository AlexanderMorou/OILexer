using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Oilexer.FiniteAutomata.Tokens;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public interface ICharRangeTokenItem :
        ITokenItem
    {
        RegularLanguageSet Range { get; }
        bool Inverted { get; }
    }
}
