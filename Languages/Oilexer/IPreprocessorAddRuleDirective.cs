using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a preprocessor directive
    /// which adds a rule to a known 
    /// </summary>
    public interface IPreprocessorAddRuleDirective :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the target of the <see cref="IPreprocessorAddRuleDirective"/>.
        /// </summary>
        string InsertTarget { get; }
        /// <summary>
        /// Returns the rules added by the <see cref="IPreprocessorAddRuleDirective"/>.
        /// </summary>
        IProductionRule[] Rules { get; }
    }
}
