using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="ILiteralStringReferenceProductionRuleItem"/>
    /// </summary>
    public interface ILiteralStringReferenceProductionRuleItem :
        ILiteralReferenceProductionRuleItem<string, ILiteralStringTokenItem>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralStringReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralStringReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralStringReferenceProductionRuleItem"/>.</returns>
        new ILiteralStringReferenceProductionRuleItem Clone();
    }
}
