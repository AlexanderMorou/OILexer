using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using Oilexer.Statements;
using System.Collections.ObjectModel;
using Oilexer._Internal;
using System.Diagnostics;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    [DebuggerDisplay("{StringForm}")]
    public partial class MethodMember :
        MethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>,
        IMethodMember,
        IStatementBlockInsertBase
    {
        private bool isAbstract;
        /// <summary>
        /// Data member for <see cref="IsStatic"/>.
        /// </summary>
        private bool isStatic;
        private bool isFinal = true;
        private bool isVirtual;
        private bool overrides;
        private ICollection<string> labelNames = new Collection<string>();
        /// <summary>
        /// Data member for <see cref="Statements"/>.
        /// </summary>
        private IStatementBlock statements;
        /// <summary>
        /// Data member for <see cref="ImplementationTypes"/>.
        /// </summary>
        private ITypeReferenceCollection implementationTypes;
        private ITypeReference privateImplementationTarget;
        #region MethodMember Constructors
        public MethodMember(string name, IMemberParentType parentTarget)
            : base(new TypedName(name, typeof(void)), parentTarget)
        {
        }
        
        public MethodMember(TypedName nameAndReturn, IMemberParentType parentTarget) 
            : base(nameAndReturn, parentTarget)
        {
        }

        public MethodMember(TypedName nameAndReturn, IMemberParentType parentTarget, params TypedName[] parameters)
            : base(nameAndReturn, parentTarget, parameters)
        {
        }

        public MethodMember(TypedName nameAndReturn, IMemberParentType parentTarget, params TypeConstrainedName[] typeParameters)
            : base(nameAndReturn, parentTarget, new TypedName[0], typeParameters)
        {
        }

        public MethodMember(TypedName nameAndReturn, IMemberParentType parentTarget, TypedName[] parameters, TypeConstrainedName[] typeParameters)
            : base(nameAndReturn, parentTarget, parameters, typeParameters)
        {
        }
        #endregion

        public override CodeMemberMethod GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeMemberMethod result = base.GenerateCodeDom(options);
            if (this.implementationTypes != null && this.implementationTypes.Count > 0)
                foreach (ITypeReference implementationType in this.ImplementationTypes)
                    result.ImplementationTypes.Add(implementationType.GenerateCodeDom(options));
            else if (this.PrivateImplementationTarget != null)
            {
                CodeTypeReference ctr = new CodeTypeReference();
                bool typeCollision = _OIL._Core.ImplementsNameCollideCheck(this, this.privateImplementationTarget.TypeInstance.GetTypeName(options), options);
                bool autoResolve;
                if (typeCollision)
                {
                    autoResolve = options.AutoResolveReferences;
                    if (autoResolve)
                        options.AutoResolveReferences = false;
                }
                else
                    autoResolve = false;
                ctr.BaseType = _OIL._Core.GenerateExpressionSnippet(options, this.PrivateImplementationTarget.GetTypeExpression().GenerateCodeDom(options));
                if (typeCollision && autoResolve)
                    options.AutoResolveReferences = autoResolve;
                result.PrivateImplementationType = ctr;
            }
            if (this.IsStatic)
            {
                if ((result.Attributes & MemberAttributes.Final) == MemberAttributes.Final)
                    result.Attributes ^= MemberAttributes.Final;
                result.Attributes |= MemberAttributes.Static;
            }
            else if (!(this.IsFinal || this.IsVirtual))
                result.Attributes ^= MemberAttributes.Final;
            if (this.IsAbstract)
                result.Attributes |= MemberAttributes.Abstract;
            if (this.Overrides)
                result.Attributes |= MemberAttributes.Override;
            if (this.HidesPrevious)
                result.Attributes |= MemberAttributes.New;
            result.Statements.AddRange(this.Statements.GenerateCodeDom(options));
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }

        public override string GetUniqueIdentifier()
        {
            if (this.privateImplementationTarget == null)
                return base.GetUniqueIdentifier();
            else
                return string.Format("{0} on {1}", base.GetUniqueIdentifier(), this.privateImplementationTarget.ToString());
        }
        #region IMethodMember Members

        /// <summary>
        /// Returns/sets whether the <see cref="MethodMember"/> is statically defined.
        /// </summary>
        /// <remarks>Static members are instance-free members and cannot be used with 
        /// this reference expressions; conversely, non-static members cannot be used within
        /// static members.</remarks>
        public bool IsStatic
        {
            get
            {
                if (this.ParentTarget is IClassType && ((IClassType)this.ParentTarget).IsStatic)
                    return true;
                return this.isStatic;
            }
            set
            {
                this.isStatic = value;
                if (value)
                {
                    this.isFinal = false;
                    this.isVirtual = false;
                }
            }
        }

        public override bool HidesPrevious
        {
            get
            {
                return base.HidesPrevious;
            }
            set
            {
                base.HidesPrevious = value;
                if (this.Overrides && value)
                    this.Overrides = false;
            }
        }

        public bool Overrides
        {
            get
            {
                return this.overrides;
            }
            set
            {
                this.overrides = value;
                if (value)
                {
                    this.hidesPrevious = false;
                    this.isStatic = false;
                }
            }
        }
        public bool IsAbstract
        {
            get
            {
                return this.isAbstract;
            }
            set
            {
                this.isAbstract = value;
                if (value && this.isStatic)
                    this.isStatic = false;
                else if (value && this.Overrides)
                    this.overrides = false;
                else if (value && this.IsVirtual)
                    this.isVirtual = false;
            }
        }
        public new IMethodParameterMembers Parameters
        {
            get 
            {
                return (IMethodParameterMembers)base.Parameters;
            }
        }

        public new IMethodTypeParameterMembers TypeParameters
        {
            get {
                return (IMethodTypeParameterMembers)base.TypeParameters;
            }
        }

        #endregion

        protected override IParameteredParameterMembers<IMethodParameterMember, CodeMemberMethod, IMemberParentType> InitializeParameters()
        {
            MethodParameterMembers result = new MethodParameterMembers(this);
            result.MembersChanged += new EventHandler(SignatureChanged);
            return result;
        }

        private void SignatureChanged(object sender, EventArgs e)
        {
            base.InvokeDeclarationChange(sender, this);
        }

        public bool IsFinal
        {
            get
            {
                return this.isFinal;
            }
            set
            {
                this.isFinal = value;
                if (value)
                {
                    this.isVirtual = false;
                }
            }
        }

        public bool IsVirtual
        {
            get
            {
                return this.isVirtual;
            }
            set
            {
                this.isVirtual = value;
                if (value)
                {
                    this.overrides = false;
                    this.isFinal = false;
                }
            }
        }

        protected override IMethodSignatureTypeParameterMembers<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType> InitializeTypeParameters()
        {
            MethodTypeParameterMembers result = new MethodTypeParameterMembers(this);
            result.MembersChanged += new EventHandler(SignatureChanged);
            return result;
        }

        public new IMethodReferenceExpression GetReference()
        {
            return new ReferenceExpression(this);
        }

        protected override IMemberReferenceExpression OnGetReference()
        {
            return this.GetReference();
        }

        #region IBlockParent Members

        public IStatementBlock Statements
        {
            get {
                if (this.statements == null)
                    this.statements = this.InitializeStatements();
                return this.statements;
            }
        }

        #endregion
        private IStatementBlock InitializeStatements()
        {
            return new StatementBlock(this);
        }

        #region IBlockParent Members

        public ICollection<string> DefinedLabelNames
        {
            get { return labelNames; }
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

        #region IImplementedMember Members

        public ITypeReferenceCollection ImplementationTypes
        {
            get {
                if (this.implementationTypes == null)
                    this.implementationTypes = new TypeReferenceCollection();
                return this.implementationTypes;
            }
        }

        public ITypeReference PrivateImplementationTarget
        {
            get
            {
                return this.privateImplementationTarget;
            }
            set
            {
                this.privateImplementationTarget = value;
            }
        }

        #endregion

        public string StringForm
        {
            get
            {
                return _OIL._Core.GenerateMemberSnippet(CodeGeneratorHelper.DefaultDomOptions, this.GenerateCodeDom(CodeGeneratorHelper.DefaultDomOptions));

            }
        }

        /// <summary>
        /// Performs a look-up on the <see cref="MethodMember"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="MethodMember"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            this.Statements.GatherTypeReferences(ref result, options);
        }

    }
}
