using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Linq;

using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer._Internal;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.Builder
{
    public class GDLinker :
        IGDLinker
    {
        #region IGDLinker Members

        /// <summary>
        /// Resolves the templates associated with the <paramref name="file"/> provided.
        /// </summary>
        /// <param name="file">The <see cref="IGDFile"/> to resolve the templates for.</param>
        /// <param name="errors">The <see cref="CompilerErrorCollection"/> to send ambiguous template
        /// reference and ambiguous expect reference errors to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="file"/> or 
        /// <paramref name="errors"/> is null.</exception>
        public void ResolveTemplates(GDFile file, CompilerErrorCollection errors)
        {
            file.ResolveTemplates(errors);
        }

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
        public void ExpandTemplates(GDFile file, CompilerErrorCollection errors)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (errors == null)
                throw new ArgumentNullException("errors");
            while (file.NeedsExpansion())
                file.ExpandTemplates(errors);
        }

        /// <summary>
        /// Links the <see cref="IGDFile"/> to eliminate all ambiguities.
        /// </summary>
        /// <param name="file">The <see cref="IGDFile"/> to link.</param>
        /// <param name="errors">The <see cref="CompilerErrorCollection"/> to send ambiguous
        /// reference errors to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="file"/> or 
        /// <paramref name="errors"/> is null.</exception>
        public void FinalLink(GDFile file, CompilerErrorCollection errors)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (errors == null)
                throw new ArgumentNullException("errors");
            file.FinalLink(errors);
        }

        #endregion
    }
}
