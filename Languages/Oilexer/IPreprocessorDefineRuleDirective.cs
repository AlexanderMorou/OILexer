using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Defines properties and methods for a define preprocessor directive.
    /// </summary>
    public interface IPreprocessorDefineRuleDirective :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the target of the <see cref="IPreprocessorDefineRuleDirective"/>.
        /// </summary>
        string DeclareTarget { get; }
        /// <summary>
        /// Returns the array of <see cref="IProductionRule"/> which result.
        /// </summary>
        IProductionRule[] DefinedRules { get; }
    }
}
