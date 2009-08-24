using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Oilexer.Translation;

namespace Oilexer.Comments
{
    /// <summary>
    /// A documentation comment denoted by a simple string.
    /// </summary>
    public class DocumentationStringComment :
        DocumentationComment,
        IDocumentationStringComment
    {
        /// <summary>
        /// Data member for <see cref="Comment"/>.
        /// </summary>
        private string comment;

        public DocumentationStringComment(string comment)
        {
            this.comment = comment;
        }

        public override string BuildCommentBody(ICodeTranslationOptions options)
        {
            return this.Comment;
        }

        #region IDocumentationStringComment Members

        /// <summary>
        /// Returns/sets the string that represents the <see cref="DocumentationStringComment"/>.
        /// </summary>
        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                this.comment = value;
            }
        }

        #endregion
    }
}
