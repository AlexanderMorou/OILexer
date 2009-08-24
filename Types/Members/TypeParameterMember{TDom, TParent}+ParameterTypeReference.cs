using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{

    partial class TypeParameterMember<TDom, TParent>
    {

        [Serializable]
        private class ParameterTypeReference :
            TypeReferenceBase,
            ITypeReference
        {
            public ParameterTypeReference(TypeParameterMember<TDom, TParent> reference)
                : base(reference)
            {
            }
            public override CodeTypeReference GenerateCodeDom(ICodeDOMTranslationOptions options)
            {
                CodeTypeReference result = base.GenerateCodeDom(options);
                result.Options = CodeTypeReferenceOptions.GenericTypeParameter;
                return result;
            }
        }
    }
}
