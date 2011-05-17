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
    /// Defines properties and methods for working with a reference to a
    /// literal character from a <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ILiteralCharReferenceProductionRuleItem :
        ILiteralReferenceProductionRuleItem<char, ILiteralCharTokenItem>
    {
        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralCharReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="ILiteralCharReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="ILiteralCharReferenceProductionRuleItem"/>.</returns>
        new ILiteralCharReferenceProductionRuleItem Clone();
    }
}
