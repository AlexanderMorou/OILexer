using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Oilexer.FileModel;
using Oilexer.Utilities.Collections;

namespace Oilexer.Compiler
{
    /// <summary>
    /// Defines properties and methods for working with the results of a compile operation.
    /// </summary>
    public interface IIntermediateCompilerResults :
        IIntermediateCompilerResultsBase
    {
        /// <summary>
        /// Returns the resulted assembly from the compile operation.
        /// </summary>
        Assembly ResultedAssembly { get; }
        /// <summary>
        /// Returns the <see cref="TemporaryDirectory"/> that the files were stored in
        /// during the compile process.
        /// </summary>
        TempFileCollection TemporaryFiles { get; }
        /// <summary>
        /// Returns the command line used to compile.
        /// </summary>
        /// <remarks>If the compiler wasn't command-line based, this is null.</remarks>
        string CommandLine { get; }
    }
}
