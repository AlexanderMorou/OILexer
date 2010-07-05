using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;
using System.Collections.ObjectModel;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    [Serializable]
    public partial class PropertyBodyMember :
        IPropertyBodyMember
    {
        /// <summary>
        /// Data member for <see cref="Part"/>.
        /// </summary>
        private PropertyBodyMemberPart part;

        /// <summary>
        /// Data member for <see cref="DefinedLabelNames"/>.
        /// </summary>
        private ICollection<string> definedLabelNames = new Collection<string>();
        /// <summary>
        /// Data member for <see cref="Statements"/>.
        /// </summary>
        private IStatementBlock statements;

        /// <summary>
        /// Data member for <see cref="Attributes"/>.
        /// </summary>
        internal IAttributeDeclarations attributes;

        /// <summary>
        /// Data member for <see cref="ParentTarget"/>
        /// </summary>
        private IPropertyMember parentTarget;


        public PropertyBodyMember(PropertyBodyMemberPart part, IPropertyMember parentTarget)
        {
            this.part = part;
            this.parentTarget = parentTarget;
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
            get { return this.definedLabelNames; }
        }

        #endregion

        #region IDeclarationTarget Members

        IDeclarationTarget IDeclarationTarget.ParentTarget
        {
            get { return this.ParentTarget; }
            set {
                if (!(value is IPropertyMember))
                    throw new ArgumentException("value must be of type: IPropertyMember.");
                this.ParentTarget = (IPropertyMember)value;
            }
        }

        public string Name
        {
            get
            {
                return this.ParentTarget.Name;
            }
            set
            {
                this.ParentTarget.Name = value;
            }
        }

        public INameSpaceDeclaration GetRootNameSpace()
        {
            return this.ParentTarget.GetRootNameSpace();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.parentTarget == null || this.statements == null)
                return;
            this.parentTarget = null;
            this.statements.Dispose();
            this.statements = null;
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

        #region IPropertyBodyMember Members

        public PropertyBodyMemberPart Part
        {
            get { return this.part; }
        }

        public IPropertyMember ParentTarget
        {
            get
            {
                return this.parentTarget;
            }
            set
            {
                this.parentTarget = value;
            }
        }

        public IAttributeDeclarations Attributes
        {
            get
            {
                if (this.attributes == null)
                    this.attributes = InitializeAttributes();
                return this.attributes;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the attributes declarations.
        /// </summary>
        /// <returns>A new <see cref="IAttributeDeclarations"/> collection instance implementation.</returns>
        protected virtual IAttributeDeclarations InitializeAttributes()
        {
            return new AttributeDeclarations();
        }

    }
}
