using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Oilexer._Internal;
using Oilexer.Statements;
using System.Collections.ObjectModel;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    public class UnaryOperatorOverloadMember :
        Member<IMemberParentType, CodeSnippetTypeMember>,
        IUnaryOperatorOverloadMember
    {
        #region UnaryOperatorOverloadMember
        /// <summary>
        /// Data member for <see cref="Operator"/>.
        /// </summary>
        private OverloadableUnaryOperators _operator;
        /// <summary>
        /// Data member for <see cref="DefinedLabelNames"/>.
        /// </summary>
        private Collection<string> definedLabelNames;
        /// <summary>
        /// Data member for <see cref="Statements"/>.
        /// </summary>
        private IStatementBlock statements;
        /// <summary>
        /// Data member for <see cref="Source"/>.
        /// </summary>
        private IUnaryOperatorOverloadSource source;
        #endregion

        /// <summary>
        /// Creates a new <see cref="UnaryOperatorOverloadMember"/> instance
        /// with the <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="parentTarget">The containing type of the <see cref="UnaryOperatorOverloadMember"/></param>
        public UnaryOperatorOverloadMember(IMemberParentType parentTarget)
            : base(null, parentTarget)
        {
        }

        public override string GetUniqueIdentifier()
        {
            return this.Operator.ToString() + this.ParentTarget.GetTypeName(CodeGeneratorHelper.DefaultTranslatorOptions);
        }

        protected override IMemberReferenceExpression OnGetReference()
        {
            throw new NotSupportedException("Coercion members cannot be referenced as they alter expression interpretation/execution.");
        }

        public override CodeSnippetTypeMember GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            IIntermediateCodeTranslator iict = null;
            if (options.LanguageProvider != null)
            {
                if (options.LanguageProvider is CSharpCodeProvider)
                    iict = _OIL._Core.DefaultCSharpCodeTranslator;
                //else if (options.LanguageProvider is VBCodeProvider)
                //{
                //    iict = new VBCodeTranslator();
                //    iict.Options = CodeGeneratorHelper.DefaultTranslatorOptions;
                //}
            }
            else
                iict = _OIL._Core.DefaultCSharpCodeTranslator;
            iict.Target = new StringFormWriter();
            iict.TranslateMember(this);
            return new CodeSnippetTypeMember(iict.Target.ToString());
        }

        #region IOperatorOverloadMember<OverloadableUnaryOperators> Members

        /// <summary>
        /// The unary operator overloaded.
        /// </summary>
        public OverloadableUnaryOperators Operator
        {
            get
            {
                return this._operator;
            }
            set
            {
                this._operator = value;
            }
        }

        #endregion


        #region IBlockParent Members

        public IStatementBlock Statements
        {
            get
            {
                if (this.statements == null)
                    this.statements = this.InitializeStatements();
                return this.statements;
            }
        }

        public ICollection<string> DefinedLabelNames
        {
            get
            {
                if (this.definedLabelNames == null)
                    this.definedLabelNames = new Collection<string>();
                return this.definedLabelNames;
            }
        }

        #endregion

        private IStatementBlock InitializeStatements()
        {
            return new StatementBlock(this);
        }

        public IUnaryOperatorOverloadSource Source
        {
            get
            {
                if (this.source == null)
                    this.source = new UnaryOperatorOverloadSource();
                return this.source;
            }
        }

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
    }
}
