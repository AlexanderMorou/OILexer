﻿using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a soft reference to an
    /// <see cref="IOilexerGrammarProductionRuleTemplateEntry"/>.
    /// </summary>
    public interface ISoftTemplateReferenceProductionRuleItem :
        ISoftReferenceProductionRuleItem
    {
        /// <summary>
        /// Returns the parts to suppliment the template's parts.
        /// </summary>
        IControlledCollection<IProductionRuleSeries> Parts { get; } 
    }
}
