using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace Oilexer.Translation
{
    /// <summary>
    /// Defines properties and methods for working with an <see cref="ICodeDomTranslator"/> that
    /// wraps around the original Code Document Object Model infrastructure.
    /// </summary>
    public interface ICodeDomTranslator<T> :
        ICodeDomTranslator
        where T :
            CodeDomProvider,
            ICodeCompiler,
            ICodeGenerator
    {
        /// <summary>
        /// Returns the <typeparamref name="T"/> for the <see cref="ICodeDomTranslator{T}"/>
        /// that manages the translation process.
        /// </summary>
        new T Provider { get; }
    }
}
