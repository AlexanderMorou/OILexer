using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Expression;
using System.Collections;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    [Serializable]
    public abstract class BlockedStatement<TDom> :
        Statement<TDom>,
        IBlockedStatement<TDom>
        where TDom :
            CodeStatement
    {
        /// <summary>
        /// Whether the <see cref="BlockedStatement{TDom}"/> has been disposed.
        /// </summary>
        private bool isDisposed;
        /// <summary>
        /// Data member for <see cref="Statements"/>.
        /// </summary>
        private IStatementBlock statements;
        /// <summary>
        /// Data member for <see cref="ParentTarget"/>.
        /// </summary>
        private IDeclarationTarget parentTarget;

        #region BlockedStatement<TDom> Constructors
        /// <summary>
        /// Creates a new <see cref="BlockedStatement{TDom}"/> with the <paramref name="sourceBlock"/>
        /// provided.
        /// </summary>
        /// <param name="sourceBlock">The target point which the <see cref="BlockedStatement{TDom}"/> is 
        /// created in.</param>
        protected BlockedStatement(IStatementBlock sourceBlock)
            : base(sourceBlock)
        {
            this.parentTarget = sourceBlock;
        }

        protected BlockedStatement()
            : base()
        {
        }

        #endregion
        public override IStatementBlock SourceBlock
        {
            get
            {
                return base.SourceBlock;
            }
            set
            {
                base.SourceBlock = value;
                this.parentTarget = value;
            }
        }
        #region IBlockedStatement Members

        public IStatementBlock Statements
        {
            get {
                if (statements == null)
                    statements = InitializeStatementBlock();
                return statements; }
        }

        /// <summary>
        /// Initializes the <see cref="Statements"/>' data member.
        /// </summary>
        /// <returns>A new <see cref="IStatementBlock"/> implementation instance.</returns>
        protected virtual IStatementBlock InitializeStatementBlock()
        {
            return new StatementBlock(this);
        }

        #endregion

        /// <summary>
        /// Finalizes the <see cref="BlockedStatement{TDom}"/> member
        /// </summary>
        ~BlockedStatement()
        {
            if (!this.isDisposed)
                this.Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.isDisposed)
                return;
            if (this.statements != null)
                this.statements.Dispose();
            this.statements = null;
            isDisposed = true;
        }

        #endregion

        #region IDeclarationTarget Members

        public IDeclarationTarget ParentTarget
        {
            get { return this.parentTarget; }
            set { this.parentTarget=value; }
        }

        public string Name
        {
            get
            {
                if (this.ParentTarget != null)
                    return this.ParentTarget.Name;
                return null;
            }
            set
            {
                if (this.ParentTarget != null)
                    this.ParentTarget.Name = value;
            }
        }

        public INameSpaceDeclaration GetRootNameSpace()
        {
            if (this.ParentTarget != null)
                return this.ParentTarget.GetRootNameSpace();
            else
                return null;
        }

        #endregion

        #region IBlockParent Members


        public ICollection<string> DefinedLabelNames
        {
            get {
                return this.SourceBlock.Parent.DefinedLabelNames;
            }
        }

        #endregion

        #region IStatementBlockInsertBase Members
        public void Add(IStatement statement)
        {
            this.Statements.Add(statement);
        }

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

        public IConditionStatement IfThen(IExpression condition, params IStatement[] trueStatements)
        {
            return this.Statements.IfThen(condition, trueStatements);
        }

        public IConditionStatement IfThen(IExpression condition, IStatement[] trueStatements, IStatement[] falseStatements)
        {
            return this.Statements.IfThen(condition, trueStatements, falseStatements);
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


        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="BlockedStatement{TDom}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="BlockedStatement{TDom}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (this.isDisposed)
                return;
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.statements != null)
                this.Statements.GatherTypeReferences(ref result, options);
        }
    }
}
