using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{

    public interface ILiteralReferenceProductionRuleItem :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the source of the literal that the <see cref="ILiteralReferenceProductionRuleItem"/>
        /// relates to.
        /// </summary>
        ITokenEntry Source { get; }
        /// <summary>
        /// Returns the literal the <see cref="ILiteralReferenceProductionRuleItem"/> references.
        /// </summary>
        ILiteralTokenItem Literal { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralReferenceProductionRuleItem"/>.</returns>
        new ILiteralReferenceProductionRuleItem Clone();
        /// <summary>
        /// Returns whether the literal the <see cref="ILiteralReferenceProductionRuleItem"/>
        /// points to was originally a flag.
        /// </summary>
        bool IsFlag { get; }
        /// <summary>
        /// Returns whether the original <see cref="ILiteralProductionRuleItem"/> 
        /// was a counter.
        /// </summary>
        bool Counter { get; }
    }
}
