using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Comments
{
    public abstract class MemberReferenceComment<T> :
        IMemberReferenceComment<T>
        where T :
            IMemberReferenceExpression
    {
        private T reference;

        public MemberReferenceComment(T reference)
        {
            this.reference = reference;
        }

        #region IMemberReferenceComment<T> Members

        public virtual T Reference
        {
            get { return this.reference; }
            set { this.reference = value; }
        }

        #endregion

        #region IMemberReferenceComment Members

        IMemberReferenceExpression IMemberReferenceComment.Reference
        {
            get
            {
                return this.Reference;
            }
            set
            {
                this.Reference = (T)value;
            }
        }

        #endregion

        #region IDocumentationCommentParticle Members

        /// <summary>
        /// Builds the <see cref="System.String"/> that represents the <see cref="MemberReferenceComment{T}"/>.
        /// </summary>
        /// <param name="options">The CodeDOM translation options that directs the generation
        /// process for type/member resolution.</param>
        /// <returns>A new <see cref="System.String"/> instance if successful.-null- otherwise.</returns>
        public abstract string BuildCommentBody(ICodeTranslationOptions options);

        #endregion
    }
}
