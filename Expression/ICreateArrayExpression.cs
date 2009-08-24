using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;

namespace Oilexer.Expression
{
    public interface ICreateArrayExpression :
        IExpression<CodeArrayCreateExpression>
    {
        IExpression SizeExpression { get; set; }

        int Size { get; set; }

        IExpressionCollection Initializers { get; }
        ITypeReference ArrayType { get; set; }
    }
}
