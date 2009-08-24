using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    partial class MethodMember
    {
        [Serializable]
        public class ReferenceExpression :
            MethodReferenceExpression
        {
            internal MethodMember referencePoint;
            public ReferenceExpression(MethodMember referencePoint)
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
            private static IMemberParentExpression GetRootReference(MethodMember referencePoint)
            {
                if (referencePoint.IsStatic)
                    return referencePoint.ParentTarget.GetTypeReference().GetTypeExpression();
                else
                    return referencePoint.ParentTarget.GetThisExpression();

            }
            
            public override CodeMethodReferenceExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
            {
                CodeMethodReferenceExpression result = base.GenerateCodeDom(options);
                if (options.NameHandler.HandlesName(this.referencePoint))
                    result.MethodName = options.NameHandler.HandleName(this.referencePoint);
                return result;
            }

        }
    }
}
