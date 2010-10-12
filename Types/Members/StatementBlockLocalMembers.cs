using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;
using System.CodeDom;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class StatementBlockLocalMembers :
        Members<IStatementBlockLocalMember, IStatementBlock, CodeVariableDeclarationStatement>,
        IStatementBlockLocalMembers
    {
        public StatementBlockLocalMembers(IStatementBlock targetDeclaration)
            : base(targetDeclaration)
        {

        }
        protected override IMembers<IStatementBlockLocalMember, IStatementBlock, CodeVariableDeclarationStatement> OnGetPartialClone(IStatementBlock parent)
        {
            throw new NotSupportedException("Locals cannot be spanned across multiple instances..");
        }

        #region IStatementBlockLocalMembers Members

        public IStatementBlockLocalMember AddNew(string name, ITypeReference localType)
        {
            return this.AddNew(new TypedName(name, localType));
        }

        public IStatementBlockLocalMember AddNew(string name, ITypeReference localType, IExpression initializationExpression)
        {
            return this.AddNew(new TypedName(name, localType), initializationExpression);
        }

        public IStatementBlockLocalMember AddNew(TypedName nameAndType)
        {
            return this.AddNew(nameAndType, null);
        }

        public IStatementBlockLocalMember AddNew(TypedName nameAndType, IExpression initializationExpression)
        {
            IStatementBlockLocalMember local = new StatementBlockLocalMember(nameAndType, this.TargetDeclaration);
            local.InitializationExpression = initializationExpression;
            local.AutoDeclare = true;
            this._Add(local.GetUniqueIdentifier(), local);
            return local;
        }

        public string GetUnusedName(string baseName)
        {
            string itemName = baseName;
            int index = 0;
            while (this.TargetDeclaration.ScopeContains(itemName))
            {
                index++;
                itemName = string.Format("{0}_{1}", baseName, index.ToString());
            }
            return itemName;
        }

        #endregion

        }
}
