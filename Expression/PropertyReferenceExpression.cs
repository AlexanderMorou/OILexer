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
    public class PropertyReferenceExpression :
        ParentMemberReference<CodePropertyReferenceExpression>,
        IPropertyReferenceExpression
    {
        public PropertyReferenceExpression(string name, IMemberParentExpression reference)
            : base(name, reference)
        {
        }

        public override CodePropertyReferenceExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodePropertyReferenceExpression propRef = new CodePropertyReferenceExpression();
            propRef.TargetObject = Reference.GenerateCodeDom(options);
            propRef.PropertyName = this.Name;
            return propRef;
        }

        #region IPropertyReferenceExpression Members

        public virtual new IPropertyReferenceComment GetReferenceParticle()
        {
            return ((IPropertyReferenceComment)this.OnGetReferenceParticle());
        }

        #endregion


    }
}
