using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    internal class MethodSignatureTypeParameterMember :
        TypeParameterMember<CodeTypeParameter, IMethodSignatureMember<IMethodSignatureParameterMember, IMethodSignatureTypeParameterMember, CodeMemberMethod, ISignatureMemberParentType>>,
        IMethodSignatureTypeParameterMember
    {

        public MethodSignatureTypeParameterMember(string name, IMethodSignatureMember parentTarget)
            : base(name, parentTarget)
        {
        }

        public override CodeTypeParameter GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return base.GenerateCodeDom(options);
        }
        
    }
}
