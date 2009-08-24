using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;

namespace Oilexer.Statements
{
    public interface IReturnStatement :
        IStatement<CodeMethodReturnStatement>
    {
        /// <summary>
        /// Returns/sets the result of the call.
        /// </summary>
        IExpression Result { get; set; }
    }
}
