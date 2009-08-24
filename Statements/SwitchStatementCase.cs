using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Types;
using System.Collections.ObjectModel;
using Oilexer.Expression;
using Oilexer.Types.Members;

namespace Oilexer.Statements
{
    public class SwitchStatementCase :
        ISwitchStatementCase,
        IBlockParent
    {
        private IStatementBlock statements;
        private ICollection<string> definedLabelNames;
        private IStatementBlock parentBlock;
        private IExpressionCollection cases;
        private bool lastIsDefault = false;
        public SwitchStatementCase(IStatementBlock parentBlock, IExpression firstCase)
        {
            if (firstCase == null)
                throw new ArgumentNullException("firstCase");
            this.parentBlock = parentBlock;
            this.Cases.Add(firstCase);
        }
        public SwitchStatementCase(IStatementBlock parentBlock)
        {
            this.lastIsDefault = true;
        }

        public SwitchStatementCase(IStatementBlock parentBlock, IExpressionCollection cases, bool lastIsDefault)
        {
            if (parentBlock == null)
                throw new ArgumentNullException("parentBlock");
            if (cases == null)
                throw new ArgumentNullException("cases");
            this.parentBlock = parentBlock;
            this.lastIsDefault = lastIsDefault;
            foreach (IExpression expression in cases)
                this.Cases.Add(expression);
        }

        #region IBlockParent Members

        public IStatementBlock Statements
        {
            get
            {
                if (this.statements == null)
                    this.statements = new StatementBlock(this);
                return this.statements;
            }
        }

        public ICollection<string> DefinedLabelNames
        {
            get {
                if (this.definedLabelNames == null)
                    this.definedLabelNames = new Collection<string>();
                return this.definedLabelNames;
            }
        }

        #endregion

        #region IDeclarationTarget Members

        public IDeclarationTarget ParentTarget
        {
            get
            {
                return this.parentBlock;
            }
            set
            {
                if (!(value is IStatementBlock))
                    throw new ArgumentException("Must be an IStatementBlock", "value");
                this.parentBlock = (IStatementBlock)value;
            }
        }

        public string Name
        {
            get
            {
                return null;
            }
            set
            {
                ;
            }
        }

        public INameSpaceDeclaration GetRootNameSpace()
        {
            return this.parentBlock.GetRootNameSpace();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.statements != null)
            {
                this.Statements.Dispose();
                this.statements = null;
            }
            this.parentBlock = null;
            this.lastIsDefault = false;
            if (this.definedLabelNames != null)
            {
                this.definedLabelNames.Clear();
                this.definedLabelNames = null;
            }
        }

        #endregion

        #region ISwitchStatementCase Members

        public IExpressionCollection Cases
        {
            get {
                if (this.cases == null)
                    this.cases = new ExpressionCollection();
                return this.cases; }
        }

        public bool LastIsDefaultCase
        {
            get
            {
                return this.lastIsDefault;
            }
            set
            {
                this.lastIsDefault = value;
            }
        }

        #endregion

        #region IStatementBlockInsertBase Members
        public IBlockStatement NewBlock()
        {
            return this.Statements.NewBlock();
        }
        
        public ISwitchStatement SelectCase(IExpression caseSwitch)
        {
            return this.Statements.SelectCase(caseSwitch);
        }

        public ISwitchStatement SelectCase(IExpression caseSwitch, IExpression[][] cases, IStatement[][] caseStatements)
        {
            return this.Statements.SelectCase(caseSwitch, cases, caseStatements);
        }

        public ICrementStatement Crement(IAssignStatementTarget target, CrementType crementType, CrementOperation operation)
        {
            return this.Statements.Crement(target, crementType, operation);
        }

        public ICrementStatement Preincrement(IAssignStatementTarget target)
        {
            return this.Statements.Preincrement(target);
        }

        public ICrementStatement Postincrement(IAssignStatementTarget target)
        {
            return this.Statements.Postincrement(target);
        }

        public ICrementStatement Increment(IAssignStatementTarget target, CrementType crementType)
        {
            return this.Statements.Increment(target, crementType);
        }

        public ICrementStatement Predecrement(IAssignStatementTarget target)
        {
            return this.Statements.Predecrement(target);
        }

        public ICrementStatement Postdecrement(IAssignStatementTarget target)
        {
            return this.Statements.Postdecrement(target);
        }

        public ICrementStatement Decrement(IAssignStatementTarget target, CrementType crementType)
        {
            return this.Statements.Decrement(target, crementType);
        }

        public IIterationStatement Iterate(IStatement init, IStatement increment, IExpression test)
        {
            return this.Statements.Iterate(init, increment, test);
        }

        public IAssignStatement Assign(IAssignStatementTarget target, IExpression value)
        {
            return this.Statements.Assign(target, value);
        }

        public IEnumeratorStatement Enumerate(IMemberParentExpression enumeratorSource, ITypeReference itemType)
        {
            return this.Statements.Enumerate(enumeratorSource, itemType);
        }

        public ILocalDeclarationStatement DefineLocal(IStatementBlockLocalMember local)
        {
            return this.Statements.DefineLocal(local);
        }

        public IBreakStatement Break()
        {
            return this.Statements.Break();
        }

        public IBreakStatement Break(IExpression conditionForBreak)
        {
            return this.Statements.Break(conditionForBreak);
        }

        public ISimpleStatement CallMethod(IMethodReferenceExpression method, params IExpression[] arguments)
        {
            return this.Statements.CallMethod(method, arguments);
        }
        public ISimpleStatement CallMethod(IMethodInvokeExpression method)
        {
            return this.Statements.CallMethod(method);
        }

        public IStatementBlockLocalMembers Locals
        {
            get
            {
                return this.Statements.Locals;
            }
        }
        public IReturnStatement Return(IExpression result)
        {
            return this.Statements.Return(result);
        }

        public IReturnStatement Return()
        {
            return this.Statements.Return();
        }

        public IConditionStatement IfThen(IExpression condition, params IStatement[] trueStatements)
        {
            return this.Statements.IfThen(condition, trueStatements);
        }

        public IConditionStatement IfThen(IExpression condition, IStatement[] trueStatements, IStatement[] falseStatements)
        {
            return this.Statements.IfThen(condition, trueStatements, falseStatements);
        }

        #endregion
    }
}
