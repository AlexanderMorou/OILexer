using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using System.Reflection;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public abstract partial class MethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> :
        Member<TParent, TSignatureDom>,
        IMethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent>
        where TParameter :
            IParameteredParameterMember<TParameter, TSignatureDom, TParent>
        where TTypeParameter :
            IMethodSignatureTypeParameterMember<TParameter, TTypeParameter, TSignatureDom, TParent>
        where TParent :
            IDeclarationTarget
        where TSignatureDom :
            CodeMemberMethod,
            new()
    {
        #region MethodSignatureMember<TParameter, TTypeParameter, TParent> Data Members

        /// <summary>
        /// Data member for <see cref="Parameters"/>.
        /// </summary>
        private IParameteredParameterMembers<TParameter, TSignatureDom, TParent> parameters;

        /// <summary>
        /// Data member for <see cref="TypeParameters"/>.
        /// </summary>
        private IMethodSignatureTypeParameterMembers<TParameter, TTypeParameter, TSignatureDom, TParent> typeParameters;

        /// <summary>
        /// Data member for <see cref="ReturnType"/>.
        /// </summary>
        private ITypeReference returnType;

        /// <summary>
        /// Data member for <see cref="IsOverload"/>.
        /// </summary>
        private bool isOverload = false;

        protected bool hidesPrevious;

        /// <summary>
        /// Data member for <see cref="Summary"/>.
        /// </summary>
        private string summary;
        /// <summary>
        /// Data member for <see cref="Remarks"/>.
        /// </summary>
        private string remarks;

        #endregion

        #region MethodSignatureMember<TParameter, TTypeParameter, TSignatureDom, TParent> Constructors
        public MethodSignatureMember(string name, TParent parentTarget)
            : this(new TypedName(name, typeof(void)), parentTarget)
        {
        }

        public MethodSignatureMember(TypedName nameAndReturn, TParent parentTarget)
            : this(nameAndReturn, parentTarget, new TypeConstrainedName[0])
        {
        }

        public MethodSignatureMember(TypedName nameAndReturn, TParent parentTarget, params TypedName[] parameters)
            : this(nameAndReturn, parentTarget, parameters, new TypeConstrainedName[0])
        {
        }

        public MethodSignatureMember(TypedName nameAndReturn, TParent parentTarget, params TypeConstrainedName[] typeParameters)
            : this(nameAndReturn, parentTarget, new TypedName[0], typeParameters)
        {
        }

        public MethodSignatureMember(TypedName nameAndReturn, TParent parentTarget, TypedName[] parameters, TypeConstrainedName[] typeParameters)
            : base(nameAndReturn.Name, parentTarget)
        {
            this.ReturnType = nameAndReturn.TypeReference;
            foreach (TypeConstrainedName tcn in typeParameters)
                this.TypeParameters.AddNew(tcn);
            foreach (TypedName tn in parameters)
                this.Parameters.AddNew(tn);
        }

        #endregion

        #region IParameteredDeclaration<IMethodParameterMember,IMemberParentType> Members

        public IParameteredParameterMembers<TParameter, TSignatureDom, TParent> Parameters
        {
            get {
                if (this.parameters == null)
                    this.parameters = InitializeParameters();
                return this.parameters;
            }
        }

        protected abstract IParameteredParameterMembers<TParameter, TSignatureDom, TParent> InitializeParameters();

        #endregion

        #region IMethodSignatureMember<TParameter,TTypeParameter,TParent> Members

        /// <summary>
        /// Returns/sets whether the <see cref="MethodSignatureMember{TParameter, TTypeParameter, TSignatureDom, TParent}"/> is
        /// an overloaded member.
        /// </summary>
        /// <remarks>
        /// It is required by some langauges to strictly define overloads as such.  
        /// An example: Visual Basic.Net
        /// </remarks>
        public bool IsOverload
        {
            get
            {
                return this.isOverload;
            }
            set
            {
                this.isOverload = value;
            }
        }
        public virtual bool HidesPrevious
        {
            get
            {
                return this.hidesPrevious;
            }
            set
            {
                this.hidesPrevious = value;
            }
        }

        public ITypeReference ReturnType
        {
            get
            {
                return this.returnType;
            }
            set
            {
                this.returnType = value;
            }
        }

        public IMethodSignatureTypeParameterMembers<TParameter, TTypeParameter, TSignatureDom, TParent> TypeParameters
        {
            get
            {
                if (this.typeParameters == null)
                    this.typeParameters = InitializeTypeParameters();
                return this.typeParameters;
            }
        }

        protected abstract IMethodSignatureTypeParameterMembers<TParameter, TTypeParameter, TSignatureDom, TParent> InitializeTypeParameters();

        #endregion
        public override TSignatureDom GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            TSignatureDom result = new TSignatureDom();
            result.CustomAttributes = this.Attributes.GenerateCodeDom(options);            
            //Setup the name, handle it with the handler if need be.
            if (options.NameHandler.HandlesName(this))
                result.Name = options.NameHandler.HandleName(this);
            else
                result.Name = this.Name;
            result.Attributes = MemberAttributes.Final;
            if (this.IsOverload)
                result.Attributes |= MemberAttributes.Overloaded;
            result.Parameters.AddRange(this.Parameters.GenerateCodeDom(options));
            result.TypeParameters.AddRange(this.TypeParameters.GenerateCodeDom(options));
            result.ReturnType = this.ReturnType.GenerateCodeDom(options);
            if (this.summary != null && this.summary != string.Empty)
                result.Comments.Add(new CodeCommentStatement(new CodeComment(this.summary, true)));
            if (this.remarks != null && this.remarks != string.Empty)
                result.Comments.Add(new CodeCommentStatement(new CodeComment(this.remarks, true)));

            result.Comments.AddRange(this.TypeParameters.GenerateCommentCodeDom(options));
            result.Comments.AddRange(this.Parameters.GenerateCommentCodeDom(options));
            result.Attributes |= (MemberAttributes)(int)this.AccessLevel;
            return result;
        }

        public override string GetUniqueIdentifier()
        {
            string signature = this.Name;
            IMethodSignatureTypeParameterMembers<TParameter, TTypeParameter, TSignatureDom, TParent> typeParameters = this.TypeParameters;
            IParameteredParameterMembers<TParameter, TSignatureDom, TParent> parameters = this.Parameters;
            if (typeParameters.Count > 0)
            {
                signature += String.Format("`{0}", typeParameters.Count);
            }
            if (parameters.Count > 0)
            {
                string[] names = new string[parameters.Count];
                for (int i = 0; i < parameters.Count; i++)
                    if (parameters[i].ParameterType == null)
                        names[i] = "INVALID";
                    else
                        names[i] = parameters.Values[i].ParameterType.ToString();
                signature += string.Format("({0})", string.Join(", ", names)); ;
            }
            else
                signature += "()";
            return signature;
        }

        public override string ToString()
        {
            if (this.ParentTarget != null)
                return string.Format("{0}.{1}", this.ParentTarget.ToString(), this.GetUniqueIdentifier());
            else
                return base.ToString();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.Parameters.GetHashCode() ^ this.TypeParameters.GetHashCode();
        }

        /// <summary>
        /// Performs a look-up on the <see cref="MethodSignatureMember{TParemeter, TTypeParameter, TSignatureDom, TParent}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="MethodSignatureMember{TParemeter, TTypeParameter, TSignatureDom, TParent}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.parameters != null)
                this.Parameters.GatherTypeReferences(ref result, options);
            if (this.typeParameters != null)
                this.TypeParameters.GatherTypeReferences(ref result, options);
            if (this.returnType != null)
                result.Add(this.ReturnType);
        }

        #region IAutoCommentMember Members

        /// <summary>
        /// Returns/sets a string related to the auto-documentation summary comment generated by the system.
        /// </summary>
        public string Summary
        {
            get
            {
                return this.summary;
            }
            set
            {
                this.summary = value;
            }
        }

        /// <summary>
        /// Returns/sets a string related to the auto-documentation remarks comment generated by the system.
        /// </summary>
        public string Remarks
        {
            get
            {
                return this.remarks;
            }
            set
            {
                this.remarks = value;
            }
        }

        #endregion
    }
}
