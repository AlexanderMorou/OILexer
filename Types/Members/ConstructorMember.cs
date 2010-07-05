using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Statements;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Oilexer._Internal;
using System.CodeDom;
using System.Data;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    [DebuggerDisplay("{StringForm}")]
    public class ConstructorMember :
        Member<IMemberParentType, CodeConstructor>,
        IConstructorMember
    {
        #region ConstructorMember Data Members
        private ICollection<string> definedLabelNames = new Collection<string>();
        private IStatementBlock statements;
        /// <summary>
        /// Data member for <see cref="IsStatic"/>.
        /// </summary>
        private bool isStatic = false;
        /// <summary>
        /// Data member for <see cref="CascadeExpressionsTarget"/>
        /// </summary>
        private ConstructorCascadeTarget cascadeExpressionsTarget;
        /// <summary>
        /// Data member for <see cref="Parameters"/>
        /// </summary>
        private IConstructorParameterMembers parameters;
        /// <summary>
        /// Data member for <see cref="CascadeMembers"/>
        /// </summary>
        private IExpressionCollection cascadeMembers = new ExpressionCollection();
        #endregion

        #region ConstructorMember Constructors
        /// <summary>
        /// Creates a new <see cref="ConstructorMember"/> instance with the <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="parentTarget">The type that contains the constructor.</param>
        public ConstructorMember(IMemberParentType parentTarget)
            : base(".ctor", parentTarget)
        {
        }

        #endregion

        /// <summary>
        /// Generates the <see cref="CodeConstructor"/> that represents the <see cref="ConstructorMember"/>.
        /// </summary>
        /// <param name="options">The CodeDOM generator options that directs the generation
        /// process.</param>
        /// <returns>A new instance of a <see cref="CodeConstructor"/> if successful.-null- otherwise.</returns>
        public override CodeConstructor GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeConstructor result = new CodeConstructor();
            /* *
             * Most collections in the OIL framework have Generate Code Dom
             * functionality; as such, simply add the custom attributes,
             * parameters, and statements of the constructor member.
             * */
            result.CustomAttributes = this.Attributes.GenerateCodeDom(options);
            result.Parameters.AddRange(this.Parameters.GenerateCodeDom(options));
            result.Statements.AddRange(this.Statements.GenerateCodeDom(options));
            //Next insert the cascade parameters, if the target is defined.
            switch (this.CascadeExpressionsTarget)
            {
                case ConstructorCascadeTarget.Undefined:
                    break;
                case ConstructorCascadeTarget.This:
                    /* *
                     * If they need to call another constructor with no arguments
                     * use a null snippet expression, otherwise just insert the 
                     * expressions collection.
                     * */
                    if (this.CascadeMembers.Count > 0)
                        result.ChainedConstructorArgs.AddRange(this.CascadeMembers.GenerateCodeDom(options));
                    else
                        result.ChainedConstructorArgs.Add(new CodeSnippetExpression(""));
                    break;
                case ConstructorCascadeTarget.Base:
                    //Same as above, but this time insert it into the base 
                    //constructor args.
                    if (this.CascadeMembers.Count > 0)
                        result.BaseConstructorArgs.AddRange(this.CascadeMembers.GenerateCodeDom(options));
                    else
                        result.BaseConstructorArgs.Add(new CodeSnippetExpression(""));
                    break;
            }
            result.Attributes = AccessLevelAttributes(this.AccessLevel);
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }

        #region IConstructorMember Members

        public ConstructorCascadeTarget CascadeExpressionsTarget
        {
            get
            {
                return this.cascadeExpressionsTarget;
            }
            set
            {
                this.cascadeExpressionsTarget = value;
            }
        }

        /// <summary>
        /// Returns the parameters of the <see cref="IConstructorMember"/>.
        /// </summary>
        public IConstructorParameterMembers Parameters
        {
            get {
                if (this.parameters == null)
                {
                    this.parameters = new ConstructorParameterMembers(this);
                    this.parameters.MembersChanged += new EventHandler(parameters_MembersChanged);
                }
                return this.parameters;
            }
        }

        void parameters_MembersChanged(object sender, EventArgs e)
        {
            this.InvokeDeclarationChange(sender, this);
        }

        /// <summary>
        /// Returns the cascade parameter expressions for the <see cref="ConstructorMember"/>.
        /// </summary>
        public IExpressionCollection CascadeMembers
        {
            get 
            {
                if (this.cascadeMembers == null)
                    this.cascadeMembers = new ExpressionCollection();
                return this.cascadeMembers;
            }
        }

        #endregion

        #region IParameteredDeclaration<IConstructorParameterMember,CodeConstructor,IMemberParentType> Members

        IParameteredParameterMembers<IConstructorParameterMember, CodeConstructor, IMemberParentType> IParameteredDeclaration<IConstructorParameterMember, CodeConstructor, IMemberParentType>.Parameters
        {
            get { return this.Parameters; }
        }

        public IStatementBlock Statements
        {
            get
            {
                if (this.statements == null)
                    this.statements = this.InitializeStatements();
                return this.statements;
            }
        }

        private IStatementBlock InitializeStatements()
        {
            return new StatementBlock(this);
        }

        #endregion

        public override string Name
        {
            get
            {
                if (this.isStatic)
                    return ".cctor";
                else
                    return ".ctor";
            }
            set
            {
                throw new ReadOnlyException("Constructors can only have a constant name.");
            }
        }

        /// <summary>
        /// Returns a unique identifier for the <see cref="ConstructorMember"/>.
        /// </summary>
        /// <returns>A unique <see cref="System.String"/> pertinent to the 
        /// <see cref="ConstructorMember"/>.</returns>
        public override string GetUniqueIdentifier()
        {
            string signature = this.Name;
            IConstructorParameterMembers parameters = this.Parameters;
            /* *
             * Add the parameters, use the parameter type information to yield a type-signature.
             * Names aren't as important because they are more likely to repeat in a type-strict
             * system that translates various types into an common understanding underneath.
             * */
            if (parameters.Count > 0)
            {
                string[] names = new string[parameters.Count];
                for (int i = 0; i < parameters.Count; i++)
                    /* *
                     * ToDo: Add 'GetTypeName' to type-references, stand-alone types
                     * may not contain necessary type-arguments.
                     * */
                    names[i] = parameters.Values[i].ParameterType.TypeInstance.
                        GetTypeName(CodeGeneratorHelper.DefaultDomOptions);
                signature += string.Format("({0})", string.Join(", ", names)); ;
            }
            else
                signature += "()";
            return signature;
        }


        protected override IMemberReferenceExpression OnGetReference()
        {
            throw new NotSupportedException("Constructors cannot be referred to in this manner.");
        }


        #region IBlockParent Members


        public ICollection<string> DefinedLabelNames
        {
            get { return this.definedLabelNames; }
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
        /// Performs a look-up on the <see cref="ConstructorMember"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ConstructorMember"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.cascadeMembers != null)
                this.cascadeMembers.GatherTypeReferences(ref result, options);
            if (this.parameters != null)
                this.Parameters.GatherTypeReferences(ref result, options);
            if (this.statements != null)
                this.Statements.GatherTypeReferences(ref result, options);
        }

        #region IInvokableMember Members

        public bool IsStatic
        {
            get
            {
                if (this.parentTarget is IClassType && ((IClassType)(this.parentTarget)).IsStatic)
                    return true;
                return this.isStatic;
            }
            set
            {
                this.isStatic = value;
            }
        }

        #endregion
    }
}
