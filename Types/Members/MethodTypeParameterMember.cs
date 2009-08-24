using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    internal class MethodTypeParameterMember :
        TypeParameterMember<CodeTypeParameter, IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>>,
        IMethodTypeParameterMember
    {

        public MethodTypeParameterMember(string name, IMethodMember parentTarget)
            : base(name, parentTarget)
        {
        }

        public override CodeTypeParameter GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return base.GenerateCodeDom(options);
        }

    }
}
