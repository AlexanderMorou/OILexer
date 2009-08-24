using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;

namespace Oilexer.Statements
{
    public interface ISimpleStatement :
        IStatement<CodeExpressionStatement>
    {
        /// <summary>
        /// Returns/sets the simple statement expression that defines the <see cref="ISimpleStatement"/>.
        /// </summary>
        ISimpleStatementExpression Expression { get; }
    }
}
