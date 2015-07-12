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
    /// Defines a constant used in a <see cref="IOilexerGrammarProductionRuleEntry"/>.
    /// </summary>
    public interface ILiteralProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the value defined by the <see cref="ILiteralProductionRuleItem"/>.
        /// </summary>
        object Value { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralProductionRuleItem"/>.</returns>
        new ILiteralProductionRuleItem Clone();
        /// <summary>
        /// Returns whether the <see cref="ILiteralProductionRuleItem"/> is
        /// a flag.
        /// </summary>
        /// <remarks>Flags only persist as a <see cref="System.Boolean"/>.</remarks>
        bool Flag { get; }
        /// <summary>
        /// Returns whether the <see cref="ILiteralProductionRuleItem"/> is a
        /// counter.
        /// </summary>
        bool Counter { get; }
    }
}
