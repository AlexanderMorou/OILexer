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
    public interface IProductionRulePreprocessorDirective :
        IProductionRuleItem
    {
        /// <summary>
        /// Returns the <see cref="IPreprocessorDirective"/> which was parsed 
        /// </summary>
        IPreprocessorDirective Directive { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="IProductionRulePreprocessorDirective"/>.
        /// </summary>
        /// <returns>A new <see cref="IProductionRulePreprocessorDirective"/> with the data
        /// members of the current <see cref="IProductionRulePreprocessorDirective"/>.</returns>
        new IProductionRulePreprocessorDirective Clone();
    }
}
