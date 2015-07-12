using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="System.String"/>
    /// literal defined in an <see cref="IOilexerGrammarProductionRuleEntry"/>.
    /// </summary>
    public interface ILiteralStringProductionRuleItem :
        ILiteralProductionRuleItem<string>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralStringProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralStringProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralStringProductionRuleItem"/>.</returns>
        new ILiteralStringProductionRuleItem Clone();
        /// <summary>
        /// Returns whether the <see cref="ILiteralStringProductionRuleItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        bool CaseInsensitive { get; }
    }
}
