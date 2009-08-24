using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace Oilexer.Translation
{
    public interface ICodeDOMTranslationOptions :
        ICodeTranslationOptions
    {
        /// <summary>
        /// Returns/sets the generation language.
        /// </summary>
        /// <remarks>Used to create better transitory comments and fixes common bugs
        /// in the specific <see cref="CodeDomProvider"/>.</remarks>
        CodeDomProvider LanguageProvider { get; set; }
        /// <summary>
        /// Returns/sets the original codedom's generator options.
        /// </summary>
        CodeGeneratorOptions Options { get; set; }
    }
}
