using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="System.Char"/> literal 
    /// defined in an <see cref="IOilexerGrammarProductionRuleEntry"/>.
    /// </summary>
    public interface ILiteralCharProductionRuleItem :
        ILiteralProductionRuleItem<char>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralCharProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralCharProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralCharProductionRuleItem"/>.</returns>
        new ILiteralCharProductionRuleItem Clone();
        /// <summary>
        /// Returns whether the <see cref="ILiteralCharProductionRuleItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        bool CaseInsensitive { get; }
    }
}
