using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Compiler
{
    public sealed class IntermediateCompiler :
        IntermediateCompilerBase
    {
        /// <summary>
        /// Creates a new <see cref="IntermediateCompiler"/> with the <paramref name="project"/> and
        /// <paramref name="translator"/>
        /// </summary>
        /// <param name="project">The <see cref="IIntermediateProject"/> that needs compiled.</param>
        /// <param name="translator">The <see cref="IIntermediateCodeTranslator"/> root to
        /// translate the intermediate code into a proper langauge for compilation.</param>
        /// <param name="options">The <see cref="IIntermediateCompilerOptions"/> which guides the <see cref="Compile()"/> process.</param>
        public IntermediateCompiler(IIntermediateProject project, IIntermediateCodeTranslator translator, IIntermediateCompilerOptions options, IIntermediateCompilerModule module)
            : base(project, translator, options, module)
        {
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCompiler"/> instance with the <paramref name="project"/>, 
        /// <paramref name="translator"/> and <paramref name="fileRegulationDelegate"/> provided.
        /// </summary>
        /// <param name="project">The <see cref="IIntermediateProject"/> that needs compiled.</param>
        /// <param name="translator">The <see cref="IIntermediateProject"/> root to translate
        /// the intermediate code into a proper langauge for compilation.</param>
        /// <param name="fileRegulationDelegate">The <see cref="Predicate{T}"/> which determines
        /// whether to translate any given <see cref="IIntermediateProject"/> partial
        /// into code.</param>
        /// <param name="options">The <see cref="IIntermediateCompilerOptions"/> which guides the <see cref="Compile()"/> process.</param>
        public IntermediateCompiler(IIntermediateProject project, IIntermediateCodeTranslator translator, IIntermediateCompilerOptions options, IIntermediateCompilerModule module, Predicate<IIntermediateProject> fileRegulationDelegate)
            : base(project, translator, options, module, fileRegulationDelegate)
        {
        }

    }
}
