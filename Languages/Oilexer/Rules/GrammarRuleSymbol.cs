using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
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
        /// <see cref="IOilexerGrammarProductionRuleEntry"/> from which the
        /// <see cref="GrammarRuleSymbol"/> is derived.</param>
        public GrammarRuleSymbol(IOilexerGrammarProductionRuleEntry source)
        {
            this.Source = source;
        }


        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleEntry"/>
        /// on which the <see cref="GrammarRuleSymbol"/> is based.
        /// </summary>
        public IOilexerGrammarProductionRuleEntry Source { get; private set; }


        public override string ToString()
        {
            return this.Source.Name;
        }


        public string ElementName
        {
            get { return this.Source.Name; }
        }

    }
}
