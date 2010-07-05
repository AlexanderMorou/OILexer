using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Provides an implementation of a symbol which is derived
    /// from a rule reference.
    /// </summary>
    public class GrammarRuleSymbol :
        IGrammarRuleSymbol
    {
        /// <summary>
        /// Creates a new <see cref="GrammarRuleSymbol"/> with the
        /// <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The 
        /// <see cref="IProductionRuleEntry"/> from which the
        /// <see cref="GrammarRuleSymbol"/> is derived.</param>
        public GrammarRuleSymbol(IProductionRuleEntry source)
        {
            this.Source = source;
        }

        #region IGrammarRuleSymbol Members

        /// <summary>
        /// Returns the <see cref="IProductionRuleEntry"/>
        /// on which the <see cref="GrammarRuleSymbol"/> is based.
        /// </summary>
        public IProductionRuleEntry Source { get; private set; }

        #endregion

        public override string ToString()
        {
            return this.Source.Name;
        }
    }
}
