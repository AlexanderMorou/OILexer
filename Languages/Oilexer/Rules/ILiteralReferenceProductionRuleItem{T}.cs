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
    /// <summary>
    /// Defines properties and methods for working with a <see cref="ILiteralReferenceProductionRuleItem"/> with
    /// a speific data-type.
    /// </summary>
    /// <typeparam name="T">The type of value stored in the <typeparamref name="TLiteral"/>.</typeparam>
    /// <typeparam name="TLiteral">The type of literal.</typeparam>
    public interface ILiteralReferenceProductionRuleItem<T, TLiteral> :
        ILiteralReferenceProductionRuleItem
        where TLiteral :
            ILiteralTokenItem<T>
    {
        /// <summary>
        /// Returns the literal the <see cref="ILiteralReferenceProductionRuleItem{T, TLiteral}"/> references.
        /// </summary>
        new TLiteral Literal { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralReferenceProductionRuleItem{T, TLiteral}"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralReferenceProductionRuleItem{T, TLiteral}"/> with the data
        /// members of the current <see cref="ILiteralReferenceProductionRuleItem{T, TLiteral}"/>.</returns>
        new ILiteralReferenceProductionRuleItem<T, TLiteral> Clone();
    }
}
