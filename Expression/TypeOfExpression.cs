using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public class TypeOfExpression :
        MemberParentExpression<CodeTypeOfExpression>,
        ITypeOfExpression
    {
        private ITypeReference typeReference;
        public TypeOfExpression(IType type)
        {
            this.typeReference = type.GetTypeReference();
        }
        public TypeOfExpression(ITypeReference typeReference)
        {
            this.typeReference = typeReference;
        }
        public TypeOfExpression(Type type)
        {
            this.typeReference = type.GetTypeReference();
        }
        public override CodeTypeOfExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeTypeOfExpression(this.TypeReference.GenerateCodeDom(options));
        }

        #region ITypeOfExpression Members

        public ITypeReference TypeReference
        {
            get
            {
                return this.typeReference;
            }
            set
            {
                this.typeReference = value;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="TypeOfExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="TypeOfExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.typeReference != null)
                result.Add(this.typeReference);
        }
    }
}
