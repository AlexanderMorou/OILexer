using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Comments;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public abstract class ParentMemberReference<TDom> :
        MemberParentExpression<TDom>,
        IMemberReferenceExpression
        where TDom :
            CodeExpression,
            new()
    {
        private IMemberParentExpression reference;
        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        protected string name;

        public ParentMemberReference(string name, IMemberParentExpression reference)
        {
            this.name = name;
            this.reference = reference;
        }

        #region IMemberReferenceExpression Members

        public virtual string Name
        {
            get { return this.name; }
        }

        public IMemberParentExpression Reference
        {
            get { return this.reference; }
        }

        #endregion


        #region IMemberReferenceExpression Members

        public virtual IMemberReferenceComment GetReferenceParticle()
        {
            return OnGetReferenceParticle();
        }

        #endregion

        protected virtual IMemberReferenceComment OnGetReferenceParticle()
        {
            throw new NotSupportedException("Derivable ParentMemberReference`1 does not know how to encapsulate itself.");
        }


        /// <summary>
        /// Performs a look-up on the <see cref="ParentMemberReference{TDom}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ParentMemberReference{TDom}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.reference != null)
                this.reference.GatherTypeReferences(ref result, options);
        }
    }
}
