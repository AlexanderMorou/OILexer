using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Comments;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public class FieldReferenceExpression :
        ParentMemberReference<CodeFieldReferenceExpression>,
        IFieldReferenceExpression
    {
        public FieldReferenceExpression(string name, IMemberParentExpression reference)
            : base(name, reference)
        {
        }

        public override CodeFieldReferenceExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression();
            fieldRef.TargetObject = Reference.GenerateCodeDom(options);
            fieldRef.FieldName = this.Name;
            return fieldRef;
        }

        #region IFieldReferenceExpression Members

        public new IFieldReferenceComment GetReferenceParticle()
        {
            return ((IFieldReferenceComment)(this.OnGetReferenceParticle()));
        }

        #endregion
    }
}