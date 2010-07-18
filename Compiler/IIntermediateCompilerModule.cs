using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Compiler
{
    /// <summary>
    /// Defines properties and methods for a module that manages the compilation process.
    /// </summary>
    public interface IIntermediateCompilerModule
    {
        /// <summary>
        /// Returns the parts of compilation that the <see cref="IIntermediateCompilerModule"/> 
        /// supports.
        /// </summary>
        CompilerModuleSupportFlags Support { get; }
        /// <summary>
        /// Returns the type of intermediateCompiler module the <see cref="IIntermediateCompilerModule"/> is.
        /// </summary>
        CompilerModuleType Type { get; }
        /// <summary>
        /// Returns whether the <see cref="IIntermediateCompilerModule"/> supports the given <paramref name="supportRequest"/>.
        /// </summary>
        /// <param name="supportRequest">The support to check for.</param>
        /// <returns>true if the <see cref="IIntermediateCompilerModule"/> supports the <paramref name="supportRequest"/></returns>
        bool Supports(CompilerModuleSupportFlags supportRequest);
    }
}
