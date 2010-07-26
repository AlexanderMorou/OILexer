using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Compiler
{
    public interface IIntermediateCompilerModuleActionResult :
        IIntermediateCompilerResultsBase
    {
        /// <summary>
        /// Returns whether the compiler module action was successful.
        /// </summary>
        bool Successful { get; }
    }
}
