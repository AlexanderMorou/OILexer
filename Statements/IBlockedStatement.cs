using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Statements
{
    public interface IBlockedStatement :
        IStatement,
        IBlockParent,
        IStatementBlockInsertBase
    {
        new IStatementBlock Statements { get; }
    }
}
