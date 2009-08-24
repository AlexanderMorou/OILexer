using System;
using System.Collections.Generic;
using System.Text;
using Oilexer._Internal;

namespace Oilexer.Compiler
{
    /// <summary>
    /// Provides a standard <see cref="IIntermediateCompiler"/> which uses a 
    /// <see cref="CSharpCodeTranslator"/> to handle code translation.
    /// </summary>
    public class CSharpIntermediateCompiler :
        IntermediateCompilerBase
    {
        /// <summary>
        /// Creates a new <see cref="CSharpIntermediateCompiler"/> with the <paramref name="project"/> provided.
        /// </summary>
        /// <param name="project">The <see cref="IIntermediateProject"/> that needs compiled.</param>
        /// <param name="options">The <see cref="IIntermediateCompilerOptions"/> which guides the <see cref="Compile()"/> process.</param>
        public CSharpIntermediateCompiler(IIntermediateProject project, IIntermediateCompilerOptions options)
            : base(project, _OIL._Core.DefaultCSharpCodeTranslator, options, _OIL._Core.DefaultCSharpCompilerModule)
        {
        }
        /// <summary>
        /// Creates a new <see cref="CSharpIntermediateCompiler"/> instance with the <paramref name="project"/> and 
        /// <paramref name="fileRegulationDelegate"/> provided.
        /// </summary>
        /// <param name="project">The <see cref="IIntermediateProject"/> that needs compiled.</param>
        /// <param name="fileRegulationDelegate">The <see cref="Predicate{T}"/> which determines
        /// whether to translate any given <see cref="IIntermediateProject"/> partial
        /// into code.</param>
        /// <param name="options">The <see cref="IIntermediateCompilerOptions"/> which guides the <see cref="Compile()"/> process.</param>
        public CSharpIntermediateCompiler(IIntermediateProject project, IIntermediateCompilerOptions options, Predicate<IIntermediateProject> fileRegulationDelegate)
            : base(project, _OIL._Core.DefaultCSharpCodeTranslator, options, _OIL._Core.DefaultCSharpCompilerModule, fileRegulationDelegate)
        {
        }
    }
}
