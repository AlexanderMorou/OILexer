using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.FiniteAutomata.Tokens
{
    public class RegularLanguageNFAState :
        NFAState<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource>
    {

        public RegularLanguageNFAState()
        {
        }


    }
}
