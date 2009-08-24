using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;
using System.CodeDom;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    public interface IStatementBlockLocalMembers :
        IMembers<IStatementBlockLocalMember, IStatementBlock, CodeVariableDeclarationStatement>
    {
        IStatementBlockLocalMember AddNew(string name, ITypeReference localType);
        IStatementBlockLocalMember AddNew(string name, ITypeReference localType, IExpression initializationExpression);
        IStatementBlockLocalMember AddNew(TypedName nameAndType);
        IStatementBlockLocalMember AddNew(TypedName nameAndType, IExpression initializationExpression);
        string GetUnusedName(string baseName);
    }
}
