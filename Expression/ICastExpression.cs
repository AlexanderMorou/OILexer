using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;

namespace Oilexer.Expression
{
    public interface ICastExpression :
        IExpression<CodeCastExpression>,
        IMemberParentExpression<CodeCastExpression>
    {
        IExpression Target { get; set; }
        ITypeReference Type { get; set; }
    }
}
