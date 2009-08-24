using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;
using Oilexer.Types;

namespace Oilexer.Statements
{
    public class CommentStatement :
        Statement<CodeCommentStatement>,
        ICommentStatement
    {
        private string comment;

        /// <summary>
        /// Creates a new <see cref="CommentStatement"/> with the <paramref name="comment"/>
        /// provided.
        /// </summary>
        /// <param name="comment">The comment represented by the <see cref="CommentStatement"/>.</param>
        public CommentStatement(string comment, IStatementBlock sourceBlock)
            : base(sourceBlock)
        {
            this.comment = comment;
        }

        public CommentStatement(string comment)
        {
            this.comment = comment;
        }

        public override CodeCommentStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeCommentStatement(this.Comment, false);
        }

        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            return;
        }

        #region ICommentStatement Members

        /// <summary>
        /// Returns/sets the comment relative to the <see cref="ICommentStatement"/>.
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
