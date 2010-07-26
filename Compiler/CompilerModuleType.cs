using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Compiler
{
    /// <summary>
    /// The type of compiler module, used to determine interaction with said module.
    /// </summary>
    public enum CompilerModuleType
    {
        /// <summary>
        /// The compiler works by using a call-back to compile the code and yield a result.
        /// </summary>
        Callback,
        /// <summary>
        /// The compiler works by executing a secondary command-line driven intermediateCompiler.
        /// </summary>
        CommandLine
    }
}
