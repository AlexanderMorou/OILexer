using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
namespace Oilexer.FiniteAutomata.Tokens
{
    /// <summary>
    /// Provides a regular language deterministic state.
    /// </summary>
    public class RegularLanguageDFAState :
        DFAState<RegularLanguageSet, RegularLanguageDFAState, ITokenSource>
    {
        public RegularLanguageDFAState()
        {
        }

        protected override bool SourceSetPredicate(ITokenSource source)
        {
            return source is ITokenItem;
        }

        protected override IFiniteAutomataTransitionTable<RegularLanguageSet, RegularLanguageDFAState, RegularLanguageDFAState> InitializeOutTransitionTable()
        {
            return new RegularLanguageDFATransitionTable();
        }
    }
}
