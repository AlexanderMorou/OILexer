using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    public interface IExpression<TDom> :
        IExpression
        where TDom :
            CodeExpression,
            new()
    {

        new TDom GenerateCodeDom(ICodeDOMTranslationOptions options);

    }
}
