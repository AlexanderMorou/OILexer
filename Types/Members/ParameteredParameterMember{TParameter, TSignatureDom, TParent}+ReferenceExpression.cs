using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Comments;
using System.CodeDom;
using System.Data;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    partial class ParameteredParameterMember<TParameter, TSignatureDom, TParent>
    {
        [Serializable]
        public class ReferenceExpression :
            VariableReferenceExpression,
            IParameterReferenceExpression
        {
            internal ParameteredParameterMember<TParameter, TSignatureDom, TParent> referencePoint;
            public ReferenceExpression(ParameteredParameterMember<TParameter, TSignatureDom, TParent> referencePoint)
                : base(referencePoint.Name, null)
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

            public override CodeVariableReferenceExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
            {
                if (options.NameHandler.HandlesName(this.referencePoint))
                    return new CodeVariableReferenceExpression(options.NameHandler.HandleName(this.referencePoint));
                return new CodeVariableReferenceExpression(this.Name);
            }

            #region IParameterReferenceExpression Members

            public new IParameterReferenceComment GetReferenceParticle()
            {
                return new Comment(this);
            }

            #endregion

            #region IMemberReferenceExpression Members

            IMemberReferenceComment IMemberReferenceExpression.GetReferenceParticle()
            {
                return this.GetReferenceParticle();
            }

            #endregion
            protected class Comment :
                MemberReferenceComment<IParameterReferenceExpression>,
                IParameterReferenceComment
            {
                
                internal protected Comment(ReferenceExpression reference)
                    : base(reference)
                {
                    
                }

                public override IParameterReferenceExpression Reference
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
                    if (options.BuildTrail != null)
                    {
                        foreach (IDeclarationTarget idt in options.BuildTrail) 
                        {
                            if (idt == ((ReferenceExpression)Reference).referencePoint.ParentTarget)
                                return string.Format("<paramref name=\"{0}\"/>", ((ReferenceExpression)Reference).referencePoint.Name);
                        }
                        return string.Format("parameter {0} (out of scope)", ((ReferenceExpression)Reference).referencePoint.Name);
                    }
                    else
                    {
                        return string.Format("<paramref name=\"{0}\"/>", ((ReferenceExpression)Reference).referencePoint.Name);
                    }
                }
            }
        }
    }
}