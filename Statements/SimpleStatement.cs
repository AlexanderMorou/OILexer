using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    [Serializable]
    public class SimpleStatement :
        Statement<CodeExpressionStatement>,
        ISimpleStatement
    {
        /// <summary>
        /// Data member for <see cref="Expression"/>.
        /// </summary>
        ISimpleStatementExpression expression;

        public SimpleStatement(ISimpleStatementExpression expression, IStatementBlock sourceBlock)
            : base(sourceBlock)
        {
            this.expression = expression;
        }

        public SimpleStatement(ISimpleStatementExpression expression)
        {
            this.expression = expression;
        }

        public override CodeExpressionStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeExpressionStatement(this.Expression.GenerateCodeDom(options));
        }

        #region ISimpleStatement Members

        public ISimpleStatementExpression Expression
        {
            get {
                return this.expression;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="SimpleStatement"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="SimpleStatement"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.expression != null)
                this.expression.GatherTypeReferences(ref result, options);
        }
    }
}
