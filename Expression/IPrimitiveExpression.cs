using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for working with a primitive expression.
    /// </summary>
    public interface IPrimitiveExpression :
        IExpression<CodePrimitiveExpression>
    {
        /// <summary>
        /// Returns/sets value that the <see cref="IPrimitiveExpression"/> represents.
        /// </summary>
        object Value { get; set; }
        TypeCode TypeCode { get; }
    }
}
