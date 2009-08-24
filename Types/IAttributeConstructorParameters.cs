using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    public interface IAttributeConstructorParameters :
        IControlledStateCollection<IAttributeConstructorParameter>,
        ITypeReferenceable
    {

        CodeAttributeArgumentCollection GenerateCodeDom(ICodeDOMTranslationOptions options);

        IAttributePropertyParameter AddNew(string name, IExpression value);
        IAttributeConstructorParameter AddNew(IExpression value);

        void AddRange(params IAttributeConstructorParameter[] parameters);
        void AddRange(params IAttributePropertyParameter[] parameters);
        void Clear();
        void Remove(IAttributeConstructorParameter parameter);
        void Remove(IAttributePropertyParameter parameter);
    }
}
