using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Utilities.Collections;
using System.Runtime.Serialization;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class MethodParameterMembers :
        ParameteredParameterMembers<IMethodParameterMember, CodeMemberMethod, IMemberParentType>,
        IMethodParameterMembers
    {

        public MethodParameterMembers(IMethodMember targetDeclaration)
            : base(targetDeclaration)
        {

        }
        protected MethodParameterMembers(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override IMembers<IMethodParameterMember, IParameteredDeclaration<IMethodParameterMember, CodeMemberMethod, IMemberParentType>, CodeParameterDeclarationExpression> OnGetPartialClone(IParameteredDeclaration<IMethodParameterMember, CodeMemberMethod, IMemberParentType> parent)
        {
            throw new NotSupportedException("Method Parameter sets cannot be spanned across multiple instances, methods aren't segmentable.");
        }

        protected override IMethodParameterMember GetParameterMember(TypedName data)
        {
            return new MethodParameterMember(data, (IMethodMember)this.TargetDeclaration);
        }
    }
}
