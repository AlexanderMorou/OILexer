using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Translation;

namespace Oilexer.Compiler
{
    /// <summary>
    /// Defines properties and methods for working with a compiler that first must translate
    /// the code from an intermediate format into a proper language code and then invokes the
    /// appropriate compiler.
    /// </summary>
    public interface IIntermediateCompiler
    {
        /// <summary>
        /// Returns the <see cref="TranslationProcessingSource"/> which notes the means to which 
        /// the intermediate construct is translated into code.
        /// </summary>
        TranslationProcessingSource GeneratorSource { get; }
        /// <summary>
        /// Compiles the <see cref="IIntermediateProject"/> associated with the current
        /// <see cref="IIntermediateCompiler"/>.
        /// </summary>
        /// <returns>A new <see cref="IIntermediateCompilerResults"/> instance which
        /// contains information about the compile process.</returns>
        IIntermediateCompilerResults Compile();
        /// <summary>
        /// Returns the <see cref="IIntermediateProject"/> which is to be translated and 
        /// compiled.
        /// </summary>
        IIntermediateProject Project { get; }
        /// <summary>
        /// Returns the <see cref="IIntermediateCodeTranslator"/> which is responsible for translating
        /// the <see cref="Project"/> into valid code for the <see cref="IIntermediateCompiler"/>'s
        /// backend.
        /// </summary>
        IIntermediateCodeTranslator Translator { get; }
        IIntermediateCompilerModule Module { get; }
        /// <summary>
        /// Returns/sets the <see cref="IIntermediateCompilerOptions"/> that are used to guide the
        /// <see cref="Compile()"/> process.
        /// </summary>
        IIntermediateCompilerOptions Options { get; set; }
    }
}
