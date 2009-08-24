using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Comments;
using Oilexer.Expression;
using System.Data;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    partial class BinaryOperatorParameter
    {
        class ReferenceParticle :
            MemberReferenceComment<IBinaryOperatorParameter>,
            IParameterReferenceComment
        {
            /// <summary>
            /// Creates a new <see cref="ReferenceParticle"/> which is a documentation comment reference
            /// to the <see cref="BinaryOperatorParameter"/> which was used to create this object.
            /// </summary>
            /// <param name="reference">The <see cref="BinaryOperatorParameter"/> that the
            /// <see cref="ReferenceParticle"/> references.</param>
            public ReferenceParticle(BinaryOperatorParameter reference)
                : base(reference)
            {

            }
            public override string BuildCommentBody(ICodeTranslationOptions options)
            {
                return string.Format("<paramref name=\"{0}\"/>", this.Reference.Name);
            }

            #region IMemberReferenceComment<IParameterReferenceExpression> Members

            public new IParameterReferenceExpression Reference
            {
                get
                {
                    return base.Reference;
                }
                set
                {
                    throw new ReadOnlyException();
                }
            }

            #endregion

        }
    }
}
