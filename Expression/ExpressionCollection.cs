using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public class ExpressionCollection :
        List<IExpression>,
        IExpressionCollection
    {
        public ExpressionCollection()
        {
        }
        public ExpressionCollection(params IExpression[] expressions)
        {
            this.AddRange(expressions);
        }
        #region IExpressionCollection<TItem,TDom> Members

        public CodeExpression[] GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            List<CodeExpression> result = new List<CodeExpression>(this.Count);
            foreach (IExpression expression in this)
                result.Add(expression.GenerateCodeDom(options));
            return result.ToArray();
        }

        #endregion

        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="ExpressionCollection"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ExpressionCollection"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            foreach (IExpression iexp in this)
                iexp.GatherTypeReferences(ref result, options);
        }

        #endregion
    }
}
