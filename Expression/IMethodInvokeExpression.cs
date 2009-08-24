using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Expression
{
    public interface IMethodInvokeExpression :
        IMemberParentExpression<CodeMethodInvokeExpression>,
        ISimpleStatementExpression
    {
        /// <summary>
        /// Returns the <see cref="IMethodReferenceExpression"/> that generated the <see cref="IMethodInvokeExpression"/>.
        /// </summary>
        IMethodReferenceExpression Reference { get; }
        /// <summary>
        /// Returns the <see cref="IExpressionCollection"/> which denotes the
        /// values to use for the invoke.
        /// </summary>
        IExpressionCollection ArgumentExpressions { get; }
    }
}
