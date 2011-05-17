using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a conditional return directive that
    /// yields the result of the conditional for the current iteration or run-through.
    /// </summary>
    public interface IPreprocessorConditionalReturnDirective :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the array of <see cref="IProductionRule"/> which result.
        /// </summary>
        IProductionRule[] Result { get; }
    }
}
