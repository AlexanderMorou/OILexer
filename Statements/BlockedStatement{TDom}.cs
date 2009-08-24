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

        /// <summary>
        /// Inserts and returns an iteration statement that iterates until 
        /// <paramref name="test"/> resolves to false, or a break/return statement is encountered.
        /// </summary>
        /// <param name="init">The initialization statement that is called before
        /// any iterations take place.</param>
        /// <param name="increment">The incremental statement that takes place
        /// after every iteration.</param>
        /// <param name="test">The test expression which determines whether to continue.</param>
        /// <returns>A new instance of an implementation of <see cref="IIterationStatement"/>.</returns>
        public IIterationStatement Iterate(IStatement init, IStatement increment, IExpression test)
        {
            return this.Statements.Iterate(init, increment, test);
        }

        /// <summary>
        /// Assigns a <paramref name="value"/> to <paramref name="target"/>. The result of this
        /// is inserted into the statement listing and returns the <see cref="IAssignStatement"/> 
        /// as a result.
        /// </summary>
        /// <param name="target">The <see cref="IAssignStatementTarget"/> which will receive the 
        /// value of '<paramref name="value"/>'.</param>
        /// <param name="value">The <see cref="IExpression"/> which relates to the value of <paramref name="target"/>.</param>
        /// <returns>A new <see cref="IAssignStatement"/> implementation instnace which can
        /// generate the CodeDOM objects necessary to assign <paramref name="target"/> to <paramref name="value"/>.</returns>
        public IAssignStatement Assign(IAssignStatementTarget target, IExpression value)
        {
            return this.Statements.Assign(target, value);
        }

        /// <summary>
        /// Enumerates the values of <paramref name="enumeratorSource"/> with the current member typed
        /// to <paramref name="itemType"/>.
        /// </summary>
        /// <param name="enumeratorSource">The source of the enumeration.  Must be a valid type that contains
        /// GetEnumerator().</param>
        /// <param name="itemType">The <see cref="ITypeReference"/> which the current member
        /// is type-bound to.  Initialization (retrieval of the <see cref="IEnumerator.Current"/>
        /// member) and casting will be performed to ensure this.</param>
        /// <returns>A new <see cref="IEnumeratorStatement"/> which defines the enumeration statement.</returns>
        public IEnumeratorStatement Enumerate(IMemberParentExpression enumeratorSource, ITypeReference itemType)
        {
            return this.Statements.Enumerate(enumeratorSource, itemType);
        }

        public ILocalDeclarationStatement DefineLocal(IStatementBlockLocalMember local)
        {
            return this.Statements.DefineLocal(local);
        }

        /// <summary>
        /// Inserts a break into the current block.  This terminates the execution of 
        /// <see cref="IBreakTargetStatement"/> statements.
        /// </summary>
        /// <returns>A new <see cref="IBreakStatement"/> which exits the <see cref="IBreakTargetStatement"/>.</returns>
        public IBreakStatement Break()
        {
            return this.Statements.Break();
        }

        public IBreakStatement Break(IExpression conditionForBreak)
        {
            return this.Statements.Break(conditionForBreak);
        }


        public IStatementBlockLocalMembers Locals
        {
            get
            {
                return this.Statements.Locals;
            }
        }

        public ISimpleStatement CallMethod(IMethodReferenceExpression method, IExpression[] arguments)
        {
            return this.Statements.CallMethod(method, arguments);
        }
        public ISimpleStatement CallMethod(IMethodInvokeExpression method)
        {
            return this.Statements.CallMethod(method);
        }
        public IReturnStatement Return(IExpression result)
        {
            return this.Statements.Return(result);
        }

        /// <summary>
        /// Fully terminates the code execution for the entry at the top of the stack and returns
        /// the control to the caller.
        /// </summary>
        /// <returns>A new <see cref="IReturnStatement"/> implementation instance
        /// which relates to the stop execution statement.</returns>
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

        public IBlockStatement NewBlock()
        {
            return this.Statements.NewBlock();
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
