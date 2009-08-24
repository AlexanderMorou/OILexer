using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Oilexer.Translation;
namespace Oilexer.Comments
{
    public abstract class DocumentationComment :
        IDocumentationComment
    {
        #region IComment Members

        public virtual CodeComment GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeComment(this.BuildCommentBody(options), true);
        }

        public abstract string BuildCommentBody(ICodeTranslationOptions options);

        #endregion
    }
}
