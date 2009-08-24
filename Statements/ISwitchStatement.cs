using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;

namespace Oilexer.Statements
{
    public interface ISwitchStatement :
        IStatement<CodeSnippetStatement>
    {
        IExpression CaseSwitch { get; set; }
        ISwitchStatementCases Cases { get; }
    }
}
