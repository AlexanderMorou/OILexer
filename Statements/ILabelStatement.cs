using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Statements
{
    public interface ILabelStatement :
        IStatement<CodeLabeledStatement>
    {
        /// <summary>
        /// Returns/sets the name of the label.
        /// </summary>
        string Name { get; set; }
        CodeGotoStatement GetCodeDomGoTo();
        IGoToLabelStatement GetGoTo(IStatementBlock sourceBlock);
    }
}
