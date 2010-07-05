using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a grammar
    /// symbol which references a rule.
    /// </summary>
    public interface IGrammarRuleSymbol :
        IGrammarSymbol
    {
        /// <summary>
        /// Returns the <see cref="IProductionRuleEntry"/>
        /// on which the <see cref="IGrammarRuleSymbol"/> is based.
        /// </summary>
        IProductionRuleEntry Source { get; }
    }
}
