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
    public class VariableReferenceExpression :
        MemberParentExpression<CodeVariableReferenceExpression>,
        IVariableReferenceExpression
    {
        private IMemberParentExpression reference;
        /// <summary
        /// Data member for <see cref="Name"/>
        /// </summary>
        private string name;
        public VariableReferenceExpression(string name, IMemberParentExpression reference)
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

        #region IVariableReferenceExpression Members

        public IMemberReferenceComment GetReferenceParticle()
        {
            throw new InvalidOperationException("Variables, beyond special case parameters, cannot be referenced.");
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="VariableReferenceExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="VariableReferenceExpression"/> relies on.</param>
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
