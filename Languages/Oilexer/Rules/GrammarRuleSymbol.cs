using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
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

        #region IGrammarSymbol Members

        public string ElementName
        {
            get { return this.Source.Name; }
        }

        #endregion
    }
}
