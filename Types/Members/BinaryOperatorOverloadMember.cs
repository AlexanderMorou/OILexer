using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using Oilexer._Internal;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using Oilexer.Statements;
using Oilexer.Translation;
namespace Oilexer.Types.Members
{
    public class BinaryOperatorOverloadMember :
        Member<IMemberParentType, CodeSnippetTypeMember>,
        IBinaryOperatorOverloadMember
    {
        #region BinaryOperatorOverloadMember Data members
        /// <summary>
        /// Data member for <see cref="Operator"/>.
        /// </summary>
        private OverloadableBinaryOperators _operator;
        /// <summary>
        /// Data member for <see cref="OtherSide"/>.
        /// </summary>
        private ITypeReference otherSide;
        /// <summary>
        /// Data member for <see cref="ContainingSide"/>.
        /// </summary>
        private BinaryOperatorOverloadContainingSide containingSide;

        /// <summary>
        /// Data member for <see cref="LeftParameter"/>.
        /// </summary>
        private IBinaryOperatorParameter leftParameter;
        /// <summary>
        /// Data member for <see cref="RightParameter"/>.
        /// </summary>
        private IBinaryOperatorParameter rightParameter;
        /// <summary>
        /// Data member for <see cref="DefinedLabelNames"/>.
        /// </summary>
        private Collection<string> definedLabelNames;
        private IStatementBlock statements;
        #endregion

        /// <summary>
        /// Creates a new <see cref="BinaryOperatorOverloadMember"/> instance
        /// with the <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="parentTarget">The containing type of the <see cref="BinaryOperatorOverloadMember"/></param>
        public BinaryOperatorOverloadMember(IMemberParentType parentTarget)
            : base(null, parentTarget)
        {
        }

        public override string GetUniqueIdentifier()
        {
            StringBuilder result = new StringBuilder();
            switch (containingSide)
            {
                case BinaryOperatorOverloadContainingSide.LeftSide:
                    result.Append(this.ParentTarget.GetTypeName(CodeGeneratorHelper.DefaultTranslatorOptions));
                    result.Append(" ");
                    result.Append(this.Operator.ToString());
                    result.Append(" ");
                    result.Append(this.OtherSide.TypeInstance.GetTypeName(CodeGeneratorHelper.DefaultTranslatorOptions, this.OtherSide.TypeParameters.ToArray()));
                    break;
                case BinaryOperatorOverloadContainingSide.RightSide:
                    result.Append(this.OtherSide.TypeInstance.GetTypeName(CodeGeneratorHelper.DefaultTranslatorOptions, this.OtherSide.TypeParameters.ToArray()));
                    result.Append(" ");
                    result.Append(this.Operator.ToString());
                    result.Append(" ");
                    result.Append(this.ParentTarget.GetTypeName(CodeGeneratorHelper.DefaultTranslatorOptions));
                    break;
                case BinaryOperatorOverloadContainingSide.Both:
                    result.Append(this.ParentTarget.GetTypeName(CodeGeneratorHelper.DefaultTranslatorOptions));
                    result.Append(" ");
                    result.Append(this.Operator.ToString());
                    result.Append(" ");
                    result.Append(this.ParentTarget.GetTypeName(CodeGeneratorHelper.DefaultTranslatorOptions));
                    break;
                default:
                    break;
            }
            return result.ToString();
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

        #region IBinaryOperatorOverloadMember Members

        /// <summary>
        /// Returns/sets the side the containing type the <see cref="BinaryOperatorOverloadMember"/>
        /// is contained within matches.
        /// </summary>
        /// <value><see cref="containingSide"/></value>
        public BinaryOperatorOverloadContainingSide ContainingSide
        {
            get
            {
                return this.containingSide;
            }
            set
            {
                this.containingSide = value;
            }
        }

        /// <summary>
        /// The other type used in the overload.
        /// </summary>
        /// <remarks>If <see cref="ContainingSide"/> is <see cref="BinaryOperatorOverloadContainingSide.Both"/>
        /// then this property is read-only.</remarks>
        /// <value><see cref="otherSide"/></value>
        public ITypeReference OtherSide
        {
            get
            {
                return this.otherSide;
            }
            set
            {
                this.otherSide = value;
            }
        }

        #endregion
        #region IOperatorOverloadMember<OverloadableBinaryOperators> Members

        /// <summary>
        /// The binary operator overloaded.
        /// </summary>
        /// <value><see cref="_operator"/></value>
        public OverloadableBinaryOperators Operator
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

        #region IBinaryOperatorOverloadMember Members


        public IBinaryOperatorParameter LeftParameter
        {
            get 
            {
                if (this.leftParameter == null)
                    this.leftParameter = this.InitializeLeftParameter();
                return this.leftParameter;
            }
        }

        public IBinaryOperatorParameter RightParameter
        {
            get
            {
                if (this.rightParameter == null)
                    this.rightParameter = this.InitializeRightParameter();
                return this.rightParameter;
            }
        }

        private IBinaryOperatorParameter InitializeLeftParameter()
        {
            return new BinaryOperatorParameter(this, BinaryOperatorParameter.OperatorSide.Left);
        }

        private IBinaryOperatorParameter InitializeRightParameter()
        {
            return new BinaryOperatorParameter(this, BinaryOperatorParameter.OperatorSide.Right);
        }

        private IStatementBlock InitializeStatements()
        {
            return new StatementBlock(this);
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
            return this.statements.Assign(target, value);
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
            return this.statements.Enumerate(enumeratorSource, itemType);
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

        /// <summary>
        /// Invokes a method given the information about the call in the form of <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The data pertinent to make the call.</param>
        /// <returns>A new <see cref="ISimpleStatement"/> implementation instance which relates
        /// to the call.</returns>
        public ISimpleStatement CallMethod(IMethodReferenceExpression method, params IExpression[] arguments)
        {
            return this.Statements.CallMethod(method, arguments);
        }

        public ISimpleStatement CallMethod(IMethodInvokeExpression method)
        {
            return this.Statements.CallMethod(method);
        }

        /// <summary>
        /// Returns the locals defined within the <see cref="BinaryOperatorOverloadMember"/>.
        /// </summary>
        public IStatementBlockLocalMembers Locals
        {
            get { return this.Statements.Locals; }
        }

        #endregion

    }
}
