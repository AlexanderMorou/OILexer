using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;
using Oilexer.Translation;
using System.Linq;

namespace Oilexer.Expression
{
    [Serializable]
    public class TypeReferenceExpression :
        MemberParentExpression<CodeTypeReferenceExpression>,
        ITypeReferenceExpression
    {
        /// <summary>
        /// Data member for <see cref="TypeReference"/>.
        /// </summary>
        private ITypeReference typeReference;
        /// <summary>
        /// Creates a new instance of <see cref="TypeReferenceExpression"/>
        /// </summary>
        /// <param name="typeReference">The <see cref="ITypeReference"/> the <see cref="TypeReferenceExpression"/> 
        /// represents.</param>
        public TypeReferenceExpression(ITypeReference typeReference)
        {
            this.typeReference = typeReference;
        }

        public override CodeTypeReferenceExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeTypeReferenceExpression(this.typeReference.GenerateCodeDom(options));
        }

        #region ITypeMemberExpression Members

        public ITypeReference TypeReference
        {
            get { return this.typeReference; }
        }

        #endregion
        public static TypeReferenceExpression GetTypeReferenceExpression<TType>()
        {
            return GetTypeReferenceExpression(CodeGeneratorHelper.GetTypeReference<TType>().TypeInstance);
        }
        public static TypeReferenceExpression GetTypeReferenceExpression(IType typeReference)
        {
            return GetTypeReferenceExpression(typeReference.GetTypeReference());
        }
        public static TypeReferenceExpression GetTypeReferenceExpression(ITypeReference typeReference)
        {
            return new TypeReferenceExpression(typeReference);
        }

        /// <summary>
        /// Performs a look-up on the <see cref="TypeReferenceExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="TypeReferenceExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if ((this.typeReference != null))
                result.Add(this.TypeReference);
        }
    }
}
