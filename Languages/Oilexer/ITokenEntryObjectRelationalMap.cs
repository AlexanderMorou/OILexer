using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface ITokenEntryObjectRelationalMap :
        IEntryObjectRelationalMap
    {
        /// <summary>
        /// The <see cref="IOilexerGrammarProductionRuleEntry"/> which is represented by the <see cref="IRuleEntryObjectRelationalMap"/>.
        /// </summary>
        new IOilexerGrammarProductionRuleEntry Entry { get; }
    }
}
