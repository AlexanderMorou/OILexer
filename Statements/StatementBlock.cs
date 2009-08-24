using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Oilexer.Types;
using Oilexer.Expression;
using Oilexer.Types.Members;
using Oilexer.Utilities.Collections;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    [Serializable]
    public class StatementBlock :
        Collection<IStatement>,
        IStatementBlock
    {
        
        /// <summary>
        /// Data member for <see cref="Parent"/>.
        /// </summary>
        private IBlockParent parent;
        /// <summary>
        /// Data member for <see cref="Locals"/>.
        /// </summary>
        private IStatementBlockLocalMembers locals;

        public StatementBlock(IBlockParent parent)
        {
            this.parent = parent;
        }

        #region IDeclarationTarget Members

        public IDeclarationTarget ParentTarget
        {
            get { return this.parent; }
            set {
                if (!(value is IBlockParent))
                    throw new ArgumentException("Value must be of type: IBlockParent.");
                this.parent = (IBlockParent)value;
            }
        }

        public string Name
        {
            get
            {
                if (this.Parent == null)
                    return null;
                return this.Parent.Name;
            }
            set
            {
                if (this.Parent != null)
                    this.Parent.Name = value;
            }
        }

        public INameSpaceDeclaration GetRootNameSpace()
        {
            if (this.Parent != null)
                return this.Parent.GetRootNameSpace();
            return null;
        }
        
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.locals != null)
                this.locals.Dispose();
            this.Clear();
            this.parent = null;
            this.locals = null;
        }

        #endregion
        #region IStatementBlock Members
        public IBlockStatement NewBlock()
        {
            IBlockStatement ibs = new BlockStatement(this);
            Add(ibs);
            return ibs;
        }

        public IEnumeratorStatement Enumerate(IMemberParentExpression enumeratorSource, ITypeReference itemType)
        {
            IEnumeratorStatement enumStatement = new EnumeratorStatement(this);
            enumStatement.EnumeratorSource = enumeratorSource;
            enumStatement.ItemType = itemType;
            Add(enumStatement);
            return enumStatement;
        }

        public IIterationStatement Iterate(IStatement init, IStatement increment, IExpression test)
        {
            IIterationStatement result = new IterationStatement(this);
            result.InitializationStatement = init;
            result.IncrementStatement = increment;
            result.TestExpression = test;
            Add(result);
            return result;
        }

        public CodeStatementCollection GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            List<CodeStatement> statements = new List<CodeStatement>();
            foreach (IStatementBlockLocalMember local in this.Locals.Values)
                if (local.AutoDeclare)
                    statements.Add(local.GetDeclarationStatement().GenerateCodeDom(options));
            foreach (IStatement statement in this)
            {
                if (!statement.Skip)
                {
                    if (statement is IStatementSeries)
                        statements.AddRange(((IStatementSeries)statement).GenerateCodeDom(options));
                    else
                        statements.Add(statement.GenerateCodeDom(options));
                }
            }
            return new CodeStatementCollection(statements.ToArray());
        }

        public bool ScopeContains(string name)
        {
            if (this.Locals.ContainsKey(name))
                return true;
            if ((this.Parent.ParentTarget != null) && this.Parent.ParentTarget is IStatementBlock)
                return ((IStatementBlock)this.Parent.ParentTarget).Locals.ContainsKey(name);
            return false;
        }

        public IBlockParent Parent
        {
            get
            {
                return this.parent;
            }
        }

        public IStatementBlockLocalMembers Locals
        {
            get 
            {
                if (this.locals == null)
                    this.locals = InitializeLocals();
                return this.locals;
            }
        }

        private IStatementBlockLocalMembers InitializeLocals()
        {
            return new StatementBlockLocalMembers(this);
        }

        IBlockParent IStatementBlock.Parent
        {
            get {
                return this.Parent;
            }
        }


        public ILocalDeclarationStatement DefineLocal(IStatementBlockLocalMember local)
        {
            if (local.ParentTarget == this)
            {
                ILocalDeclarationStatement localDecl = local.GetDeclarationStatement();
                localDecl.ReferencedMember.AutoDeclare = false;
                Add(localDecl);
                return localDecl;
            }
            throw new InvalidOperationException("Cannot declare a local outside its scope.");
        }

        public ISwitchStatement SelectCase(IExpression caseSwitch)
        {
            ISwitchStatement iss = new SwitchStatement(this);
            iss.CaseSwitch = caseSwitch;
            this.Add(iss);
            return iss;
        }
        public ISwitchStatement SelectCase(IExpression caseSwitch, IExpression[][] cases, IStatement[][] caseStatements)
        {
            if (caseStatements.Length != cases.Length)
                throw new ArgumentException("cases and caseStatements must have an equal number of members.", "caseStatements");
            ISwitchStatement iss = new SwitchStatement(this);
            iss.CaseSwitch = caseSwitch;
            for (int i = 0; i < cases.Length; i++)
            {
                IExpression[] caseSeries = cases[i];
                ISwitchStatementCase @case = iss.Cases.AddNew(false, new ExpressionCollection(caseSeries));
                foreach (IStatement @is in caseStatements[i])
                    @case.Statements.Add(@is);
            }
            this.Add(iss);
            return iss;
        }

        public ICrementStatement Crement(IAssignStatementTarget target, CrementType crementType, CrementOperation operation)
        {
            ICrementStatement result = new CrementStatement(crementType, operation, target);
            this.Add(result);
            return result;
        }

        public ICrementStatement Preincrement(IAssignStatementTarget target)
        {
            return this.Increment(target, CrementType.Prefix);
        }

        public ICrementStatement Postincrement(IAssignStatementTarget target)
        {
            return this.Increment(target, CrementType.Postfix);
        }

        public ICrementStatement Increment(IAssignStatementTarget target, CrementType crementType)
        {
            return this.Crement(target, crementType, CrementOperation.Increment);
        }

        public ICrementStatement Predecrement(IAssignStatementTarget target)
        {
            return this.Decrement(target, CrementType.Prefix);
        }

        public ICrementStatement Postdecrement(IAssignStatementTarget target)
        {
            return this.Decrement(target, CrementType.Postfix);
        }

        public ICrementStatement Decrement(IAssignStatementTarget target, CrementType crementType)
        {
            return this.Crement(target, crementType, CrementOperation.Decrement);
        }

        public IAssignStatement Assign(IAssignStatementTarget target, IExpression value)
        {
            IAssignStatement result = new AssignStatement(this, target, value);
            Add(result);
            return result;
        }

        public ISimpleStatement CallMethod(IMethodReferenceExpression method, params IExpression[] arguments)
        {
            ISimpleStatement simpleStatement = new SimpleStatement(method.Invoke(arguments), this);
            this.Add(simpleStatement);
            return simpleStatement;
        }
        public ISimpleStatement CallMethod(IMethodInvokeExpression method)
        {
            ISimpleStatement iss = new SimpleStatement(method, this);
            this.Add(iss);
            return iss;
        }

        public IBreakStatement Break()
        {
            BreakStatement result = new BreakStatement(this);
            this.Add(result);
            return result;
        }

        public IBreakStatement Break(IExpression conditionForBreak)
        {
            BreakStatement result = new BreakStatement(this);
            result.Condition = conditionForBreak;
            this.Add(result);
            return result;
        }

        #endregion
        
        private new void Add(IStatement statement)
        {
            base.Add(statement);
            statement.SourceBlock = this;
            if (statement is IBreakTargetStatement)
                base.Add(((IBreakTargetStatement)statement).ExitLabel);
        }


        public IConditionStatement IfThen(IExpression condition, params IStatement[] trueStatements)
        {
            return this.IfThen(condition, trueStatements, new IStatement[0]);
        }

        public IConditionStatement IfThen(IExpression condition, IStatement[] trueStatements, IStatement[] falseStatements)
        {
            IConditionStatement result = new ConditionStatement(this);
            result.Condition = condition;
            foreach (IStatement trueStatement in trueStatements)
                result.Statements.Add(trueStatement);
            foreach (IStatement falseStatement in falseStatements)
                result.FalseBlock.Add(falseStatement);
            this.Add(result);
            return result;
        }

        public IReturnStatement Return(IExpression result)
        {
            IReturnStatement res = new ReturnStatement(this);
            res.Result = result;
            this.Add(res);
            return res;
        }

        public IReturnStatement Return()
        {
            return Return(null);
        }

        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="StatementBlock"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="StatementBlock"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.locals != null)
                this.locals.GatherTypeReferences(ref result, options);
            foreach (IStatement ist in this)
                ist.GatherTypeReferences(ref result, options);
        }

        #endregion

    }
}
