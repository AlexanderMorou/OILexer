using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public struct TemplateArgumentInformation
    {
        public readonly int FixedArguments;
        public readonly int DynamicArguments;
        public readonly int InvalidArguments;
        internal TemplateArgumentInformation(int fixedArguments, int dynamicArguments, int invalidArguments)
        {
            this.FixedArguments = fixedArguments;
            this.DynamicArguments = dynamicArguments;
            this.InvalidArguments = invalidArguments;
        }
    }
    /// <summary>
    /// Defines properties and methods for working with a template for production rules.
    /// Used to express a part of syntax for a <see cref="IOilexerGrammarFile"/>.
    /// </summary>
    public interface IOilexerGrammarProductionRuleTemplateEntry :
        IOilexerGrammarProductionRuleEntry
    {
        /// <summary>
        /// The names of the parts associated with the <see cref="IOilexerGrammarProductionRuleTemplateEntry"/>.
        /// </summary>
        IProductionRuleTemplateParts Parts { get; }
        TemplateArgumentInformation GetArgumentInformation();
    }
}
