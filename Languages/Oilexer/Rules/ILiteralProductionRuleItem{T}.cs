using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public interface ILiteralProductionRuleItem<T>  :
        ILiteralProductionRuleItem
    {
        /// <summary>
        /// Returns the value defined by the <see cref="ILiteralProductionRuleItem{T}"/>.
        /// </summary>
        new T Value { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralProductionRuleItem{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralProductionRuleItem{T}"/> with the data
        /// members of the current <see cref="ILiteralProductionRuleItem{T}"/>.</returns>
        new ILiteralProductionRuleItem<T> Clone();
    }
}
