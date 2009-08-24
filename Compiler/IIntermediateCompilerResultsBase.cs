using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Compiler
{
    public interface IIntermediateCompilerResultsBase
    {
        /// <summary>
        /// Returns a series of <see cref="IIntermediateCompilerErrors"/> instances that occurred.
        /// </summary>
        IReadOnlyCollection<IIntermediateCompilerError> Errors { get; }
        /// <summary>
        /// Returns a series of <see cref="IIntermediateCompilerError"/> instances, as warnings, that occurred.
        /// </summary>
        IReadOnlyCollection<IIntermediateCompilerError> Warnings { get; }
        /// <summary>
        /// Returns whether there was an error during the compilation process.
        /// </summary>
        bool HasErrors { get; }
        /// <summary>
        /// Returns whether there was any warnings during the compilation process.
        /// </summary>
        bool HasWarnings { get; }
        /// <summary>
        /// Returns the native return from the target that handled the request.
        /// </summary>
        int NativeReturnValue { get; }
    }
}
