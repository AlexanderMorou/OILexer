using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace Oilexer.Parser.Builder
{
    /// <summary>
    /// Defines properties and methods for working with a rough linker implementation that handles
    /// code expansion and reference resolution.
    /// </summary>
    public interface IGDLinker
    {
        /// <summary>
        /// Resolves the templates associated with the <paramref name="file"/> provided.
        /// </summary>
        /// <param name="file">The <see cref="IGDFile"/> to resolve the templates for.</param>
        /// <param name="errors">The <see cref="CompilerErrorCollection"/> to send ambiguous template
        /// reference and ambiguous expect reference errors to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="file"/> or 
        /// <paramref name="errors"/> is null.</exception>
        void ResolveTemplates(GDFile file, CompilerErrorCollection errors);
        /// <summary>
        /// Expands the templates from their compact form to their full form.
        /// </summary>
        /// <param name="file">The <see cref="IGDFile"/> to expand the templates of.</param>
        /// <param name="errors">The <see cref="CompilerErrorCollection"/> to send expansion
        /// errors to.</param>
        /// <remarks>Templates can generate whole production rules, therefore the resulted
        /// grammar size can be substantially larger.</remarks>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="file"/> or 
        /// <paramref name="errors"/> is null.</exception>
        void ExpandTemplates(GDFile file, CompilerErrorCollection errors);
        /// <summary>
        /// Links the <see cref="IGDFile"/> to eliminate all ambiguities.
        /// </summary>
        /// <param name="file">The <see cref="IGDFile"/> to link.</param>
        /// <param name="errors">The <see cref="CompilerErrorCollection"/> to send ambiguous
        /// reference errors to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="file"/> or 
        /// <paramref name="errors"/> is null.</exception>
        void FinalLink(GDFile file, CompilerErrorCollection errors);
    }
}
