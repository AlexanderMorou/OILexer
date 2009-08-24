using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for 
    /// </summary>
    /// <typeparam name="TParent">The type that is valid as a parent of the <see cref="Member{TParent, TDom}"/>.</typeparam>
    /// <typeparam name="TDom">The type of <see cref="CodeObject"/> which the
    /// <see cref="IDeclaration{TParent, TDom}"/> yields.</typeparam>
    [Serializable]
    public abstract partial class Member<TParent, TDom> :
        Declaration<TParent, TDom>,
        IMember<TParent, TDom>
        where TParent :
            IDeclarationTarget
        where TDom :
            CodeObject
    {
        private IAttributeDeclarations attributes;

        /// <summary>
        /// Creates a new instance of <see cref="Member{TParent, TDom}"/>
        /// </summary>
        /// <param name="name">The name of the declaration.</param>
        /// <param name="parentTarget">The <typeparamref name="TParent"/> the 
        /// <see cref="Member{TParent, TDom}"/> exists as a sub-member of.</param>
        protected Member(string name, TParent parentTarget)
            : base(name, parentTarget)
        {

        }


        #region IMember<TParent,TDom> Members

        /// <summary>
        /// Returns/sets the parent of the current <see cref="Member{TParent, TDom}"/>.
        /// </summary>
        public new TParent ParentTarget
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

        #endregion

        #region IMember Members
        IDeclarationTarget IMember.ParentTarget
        {
            get
            {
                return this.ParentTarget;
            }
            set
            {
                if (!(value is TParent))
                    throw new ArgumentException("value must be of type TParent");
                this.ParentTarget = (TParent)value;
            }
        }



        public IMemberReferenceExpression GetReference()
        {
            return this.OnGetReference();
        }

        protected abstract IMemberReferenceExpression OnGetReference();

        public IAttributeDeclarations Attributes
        {
            get {
                if (this.attributes == null)
                    this.InitializeAttributes();
                return this.attributes;
            }
        }

        #endregion

        private void InitializeAttributes()
        {
            this.attributes = new AttributeDeclarations();
        }

        /// <summary>
        /// Performs a look-up on the <see cref="Member{TParent, TDom}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="Member{TParent, TDom}"/> relies on.</param>
        /// <remarks>The default <see cref="Member{TParent, TDom}"/> carries an implied reference
        /// requirement in the form of its <see cref="Attributes"/>.</remarks>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.attributes != null)
                this.Attributes.GatherTypeReferences(ref result, options);
        }
    }
}
