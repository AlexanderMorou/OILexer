using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace Oilexer.Translation
{
    /// <summary>
    /// Defines properties and methods for working with a code-dom translator
    /// </summary>
    public interface ICodeDomTranslator :
        IIntermediateCodeTranslator
    {
        /// <summary>
        /// Returns the <see cref="CodeDomProvider"/> for the <see cref="ICodeDomTranslator"/>
        /// that manages the translation process.
        /// </summary>
        CodeDomProvider Provider { get; }
        /// <summary>
        /// Returns/sets the <see cref="IIntermediateCodeTranslatorOptions"/> which drive the 
        /// code generation process.
        /// </summary>
        new ICodeDOMTranslationOptions Options { get; set; }
    }
}
