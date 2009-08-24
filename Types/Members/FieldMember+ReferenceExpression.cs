using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Comments;
using System.Data;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    partial class FieldMember
    {
        public class ReferenceExpression :
            FieldReferenceExpression
        {
            internal FieldMember referencePoint;
            public ReferenceExpression(FieldMember referencePoint)
                : base(referencePoint.Name, GetRootReference(referencePoint))
            {
                this.referencePoint = referencePoint;
            }
            public override string Name
            {
                get
                {
                    return this.referencePoint.Name;
                }
            }
            private static IMemberParentExpression GetRootReference(FieldMember referencePoint)
            {
                if (referencePoint.IsStatic || referencePoint.IsConstant)
                    return referencePoint.ParentTarget.GetTypeReference().GetTypeExpression();
                else if (referencePoint.ParentTarget is IMemberParentType)
                    return ((IMemberParentType)(referencePoint.ParentTarget)).GetThisExpression();
                else
                    return null;
            }

            public override CodeFieldReferenceExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
            {
                CodeFieldReferenceExpression result = base.GenerateCodeDom(options);
                if (options.NameHandler.HandlesName(this.referencePoint))
                    result.FieldName = options.NameHandler.HandleName(this.referencePoint);
                return result;
            }

            protected override IMemberReferenceComment OnGetReferenceParticle()
            {
                return new Comment(this);
            }

            protected class Comment :
                MemberReferenceComment<IFieldReferenceExpression>,
                IFieldReferenceComment
            {

                public Comment(ReferenceExpression reference)
                    : base(reference)
                {
                }

                public override IFieldReferenceExpression Reference
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

                public override string BuildCommentBody(ICodeTranslationOptions options)
                {
                    return _OIL._Core.BuildMemberReferenceComment(options, ((ReferenceExpression)this.Reference).referencePoint);
                }
                
            }
        }
    }
}