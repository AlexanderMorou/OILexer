using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for working with a binary operation.
    /// </summary>
    public interface IBinaryOperationExpression :
        IExpression<CodeBinaryOperatorExpression>
    {
        IExpression LeftSide { get; set; }
        CodeBinaryOperatorType Operation { get; set; }
        IExpression RightSide { get; set; }
    }
}
