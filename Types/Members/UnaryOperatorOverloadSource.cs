using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Comments;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    public sealed partial class UnaryOperatorOverloadSource :
        MemberParentExpression<CodeVariableReferenceExpression>,
        IUnaryOperatorOverloadSource
    {
        private IParameterReferenceComment referenceParticle;

        internal UnaryOperatorOverloadSource()
        {
        }

        #region IParameterReferenceExpression Members

        public IParameterReferenceComment GetReferenceParticle()
        {
            if (referenceParticle == null)
                this.referenceParticle = new ReferenceParticle();
            return this.referenceParticle;
        }

        #endregion

        #region IVariableReferenceExpression Members

        IMemberReferenceComment IVariableReferenceExpression.GetReferenceParticle()
        {
            return this.GetReferenceParticle();
        }

        #endregion

        #region IMemberReferenceExpression Members

        public string Name
        {
            get { return "source"; }
        }

        public IMemberParentExpression Reference
        {
            get { return null; }
        }

        IMemberReferenceComment IMemberReferenceExpression.GetReferenceParticle()
        {
            return this.GetReferenceParticle();
        }

        #endregion

        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            return;
        }
    }
}
