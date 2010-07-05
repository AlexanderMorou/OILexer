using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a language's
    /// symbols.
    /// </summary>
    public interface IGrammarSymbolSet :
        IControlledStateCollection<IGrammarSymbol>
    {
        IGrammarSymbol this[ITokenEntry entry] { get; }
        IGrammarSymbol this[ILiteralTokenItem entry] { get; }
        IGrammarSymbol this[IProductionRuleEntry entry] { get; }
    }
}
