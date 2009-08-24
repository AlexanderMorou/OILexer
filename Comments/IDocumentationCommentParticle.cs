using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Translation;

namespace Oilexer.Comments
{
    public interface IDocumentationCommentParticle
    {
        /// <summary>
        /// Builds the <see cref="System.String"/> that represents the <see cref="IDocumentationCommentParticle"/>.
        /// </summary>
        /// <param name="options">The CodeDOM generator options that directs the generation
        /// process for type/member resolution.</param>
        /// <returns>A new <see cref="System.String"/> instance if successful.-null- otherwise.</returns>
        string BuildCommentBody(ICodeTranslationOptions options);
    }
}
