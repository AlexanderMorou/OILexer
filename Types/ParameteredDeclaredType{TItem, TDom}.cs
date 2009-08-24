using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types.Members;
using Oilexer.Translation;

namespace Oilexer.Types
{
    [Serializable]
    public abstract partial class ParameteredDeclaredType<TItem, TDom> :
        DeclaredType<TDom>,
        IParameteredDeclaredType<TDom>
        where TItem :
            IParameteredDeclaredType<TDom>
        where TDom :
            CodeTypeDeclaration,
            new()
    {
        #region ParameteredDeclaredType<TItem, TDom> Data members

        /// <summary>
        /// Data member for <see cref="TypeParameters"/>.
        /// </summary>
        private ITypeParameterMembers<ITypeParameterMember<TDom>, TDom> typeParameters;

        #endregion

        #region ParameteredDeclaredType<TItem, TDom> Members
        /// <summary>
        /// Creates a new instance of <see cref="ParameteredDeclaredType{TItem, TDom}"/> with the 
        /// <paramref name="name"/> and <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="ParameteredDeclaredType{TItem, TDom}"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> that contains
        /// the <see cref="ParameteredDeclaredType{TItem, TDom}"/>.</param>
        public ParameteredDeclaredType(string name, ITypeParent parentTarget)
            : base(name, parentTarget)
        {
        }

        public ParameteredDeclaredType(ITypeParent parentTarget)
            : base(parentTarget)
        {
        }
        #endregion
        /// <summary>
        /// Returns the <see cref="ParameteredDeclaredType{TItem, TDom}"/>'s type-parameters.
        /// </summary>
        public ITypeParameterMembers<ITypeParameterMember<TDom>, TDom> TypeParameters
        {
            get
            {
                if (this.typeParameters == null)
                    this.typeParameters = InitializeTypeParameters();
                return this.typeParameters;
            }
        }

        /// <summary>
        /// Generates the <typeparamref name="TDom"/> that represents the <see cref="ParameteredDeclaredType{TItem, TDom}"/>.
        /// </summary>
        /// <returns>A new instance of a <typeparamref name="TDom"/> if successful.-null- otherwise.</returns>
        public override TDom GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            TDom result = base.GenerateCodeDom(options);
            result.TypeParameters.AddRange(this.TypeParameters.GenerateCodeDom(options));
            return result;
        }

        #region Initialization Members

        protected virtual ITypeParameterMembers<ITypeParameterMember<TDom>, TDom> InitializeTypeParameters()
        {
            return new TypeParameterMembers(this);
        }

        #endregion 

        #region IParameteredDeclaredType<CodeTypeDeclaration> Members

        ITypeParameterMembers<ITypeParameterMember<TDom>, TDom> IParameteredDeclaredType<TDom>.TypeParameters
        {
            get
            {
                return this.TypeParameters;
            }
        }

        #endregion

        #region IParameteredDeclaredType Members

        /// <summary>
        /// Returns/sets whether the <see cref="ClassType"/> is a generic type.
        /// </summary>
        /// <returns>True, if the <see cref="ClassType"/> is a generic, false otherwise.</returns>
        /// <remarks>If it is a generic and the value is changed to false, the generic
        /// type-parameters are cleared.</remarks>
        public override bool IsGeneric
        {
            get
            {
                if (this.ParentTarget is IDeclaredType)
                    return (this.TypeParameters != null && this.TypeParameters.Count != 0) || ((IDeclaredType)(this.ParentTarget)).IsGeneric;
                return this.TypeParameters != null && this.TypeParameters.Count != 0;
            }
        }

        ITypeParameterMembers IParameteredDeclaredType.TypeParameters
        {
            get
            {
                return (ITypeParameterMembers)this.TypeParameters;
            }
        }

        #endregion


        public override bool IsGenericTypeDefinition
        {
            get { return this.TypeParameters.Count > 0; }
        }

        public override string GetUniqueIdentifier()
        {
            string result = this.Name;
            if (this.TypeParameters != null && this.TypeParameters.Count > 0)
                result += String.Format("`{0}", this.TypeParameters.Count);
            return result;
        }

        /// <summary>
        /// Performs a look-up on the <see cref="ParameteredDeclaredType{TItem, TDom}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ParameteredDeclaredType{TItem, TDom}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.TypeParameters != null)
                this.TypeParameters.GatherTypeReferences(ref result, options);
        }
    }
}
