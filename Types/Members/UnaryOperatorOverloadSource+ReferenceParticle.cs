using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Comments;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    partial class UnaryOperatorOverloadSource
    {
        internal class ReferenceParticle :
            MemberReferenceComment<IParameterReferenceExpression>,
            IParameterReferenceComment
        {
            public ReferenceParticle()
                : base(null)
            {
            }

            /// <summary>
            /// Builds the <see cref="System.String"/> that represents the <see cref="ReferenceParticle"/>.
            /// </summary>
            /// <param name="options">The CodeDOM generator options that directs the generation
            /// process for type/member resolution.</param>
            /// <returns>A new <see cref="System.String"/> instance if successful.-null- otherwise.</returns>
            public override string BuildCommentBody(ICodeTranslationOptions options)
            {
                return "<paramref name=\"source\"/>";
            }
        }
    }
}
