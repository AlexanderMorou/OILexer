using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public partial class PropertyMember :
        PropertySignatureMember<IMemberParentType>,
        IPropertyMember,
        IImplementedMember
    {
        /// <summary>
        /// Data member for <see cref="Overrides"/>.
        /// </summary>
        private bool overrides;
        /// <summary>
        /// Data member for <see cref="IsFinal"/>.
        /// </summary>
        private bool isFinal = true;
        /// <summary>
        /// Data member for <see cref="IsStatic"/>.
        /// </summary>
        private bool isStatic;
        /// <summary>
        /// Data member for <see cref="IsVirtual"/>.
        /// </summary>
        private bool isVirtual;
        /// <summary>
        /// Data member for <see cref="GetPart"/>.
        /// </summary>
        PropertyBodyMember getPart;
        /// <summary>
        /// Data member for <see cref="SetPart"/>.
        /// </summary>
        PropertySetBodyMember setPart;
        /// <summary>
        /// Data member for <see cref="ImplementationTypes"/>.
        /// </summary>
        private ITypeReferenceCollection implementationTypes;
        /// <summary>
        /// Data member for <see cref="PrivateImplementationTarget"/>.
        /// </summary>
        private ITypeReference privateImplementationTarget;
        /// <summary>
        /// Data member for <see cref="IsAbstract"/>.
        /// </summary>
        private bool isAbstract;


        public PropertyMember(TypedName nameAndType, IMemberParentType parentTarget)
            : base(nameAndType, parentTarget)
        {

        }
        public override string GetUniqueIdentifier()
        {
            if (this.privateImplementationTarget == null)
                return base.GetUniqueIdentifier();
            else
                return String.Format("{0} on {1}", base.GetUniqueIdentifier(), this.privateImplementationTarget.ToString());
        }

        public override CodeMemberProperty GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeMemberProperty result = base.GenerateCodeDom(options);
            result.CustomAttributes = this.Attributes.GenerateCodeDom(options);
            if (this.implementationTypes != null && this.implementationTypes.Count > 0)
                foreach (ITypeReference implementationType in this.ImplementationTypes)
                    result.ImplementationTypes.Add(implementationType.GenerateCodeDom(options));
            else if (this.PrivateImplementationTarget != null)
            {
                CodeTypeReference ctr = new CodeTypeReference();
                //Just in case, but its necessary to see if the
                //private implementation point collides with another type.  If it does,
                //then it's necessary to temporary disable the auto-resolution feature.
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
            if (((!(this.IsFinal)) || this.IsVirtual))
                result.Attributes ^= MemberAttributes.Final;
            if (this.HasGet)
                result.GetStatements.AddRange(this.GetPart.Statements.GenerateCodeDom(options));
            if (this.HasSet)
                result.SetStatements.AddRange(this.SetPart.Statements.GenerateCodeDom(options));
            if (Overrides)
                result.Attributes |= MemberAttributes.Override;
            if (IsStatic)
                result.Attributes |= MemberAttributes.Static;
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }

        public PropertyMember(string name, IMemberParentType parentTarget)
            : base(name, parentTarget)
        {

        }

        protected override IMemberReferenceExpression OnGetReference()
        {
            return this.GetReference();
        }

        #region IPropertySignatureMember<TParent> Members

        public new IPropertyReferenceExpression GetReference()
        {
            return new ReferenceExpression(this);
        }

        #endregion


        #region IPropertyMember Members

        public IPropertyBodyMember GetPart
        {
            get {
                if (this.getPart == null && this.HasGet)
                    this.getPart = new PropertyBodyMember(PropertyBodyMemberPart.GetPart, this);
                return this.getPart; }
        }

        public IPropertySetBodyMember SetPart
        {
            get {
                if (this.setPart == null && this.HasSet)
                    this.setPart = new PropertySetBodyMember(this);
                return this.setPart; }
        }

        #endregion

        #region IPropertyMember Members


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
                    this.IsVirtual = false;
            }
        }
        /// <summary>
        /// Returns/sets whether the <see cref="PropertySignatureMember{TParent}"/> is static.
        /// </summary>
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
            }
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
                    this.isVirtual = false;
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

        #endregion

        #region IImplementedMember Members

        public ITypeReferenceCollection ImplementationTypes
        {
            get
            {
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
                ITypeReference itr = this.privateImplementationTarget;
                this.privateImplementationTarget = value;
                DeclarationChangeArgs callArgs = new DeclarationChangeArgs(this);
                base.OnDeclarationChanged(callArgs);
                if (callArgs.Cancel)
                    this.privateImplementationTarget = itr;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="PropertyMember"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="PropertyMember"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.HasGet && this.getPart != null)
            {
                this.GetPart.Statements.GatherTypeReferences(ref result, options);
                if (this.getPart.attributes != null && this.getPart.Attributes.Count > 0)
                    this.getPart.Attributes.GatherTypeReferences(ref result, options);
            }
            if (this.HasSet && this.setPart != null)
            {
                this.setPart.Statements.GatherTypeReferences(ref result, options);
                if (this.setPart.attributes != null && this.setPart.Attributes.Count > 0)
                    this.setPart.Attributes.GatherTypeReferences(ref result, options);
            }
        }

        #region ICodeBodyTableMember Members

        public bool IsAbstract
        {
            get
            {
                return this.isAbstract;
            }
            set
            {
                this.isAbstract = value;
            }
        }

        #endregion

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
    }
}
