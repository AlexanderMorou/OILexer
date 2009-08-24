using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace Oilexer.Expression
{
    /// <summary>
    /// Provides a base implementation of <see cref="IUnaryOperationExpression"/> which enables
    /// support for unary operators.
    /// </summary>
    public class UnaryOperationExpression :
        Expression<CodeSnippetExpression>,
        IUnaryOperationExpression
    {
        /// <summary>
        /// Data member for <see cref="Operation"/>.
        /// </summary>
        private UnaryOperations operation;
        /// <summary>
        /// Data member for <see cref="TargetExpression"/>.
        /// </summary>
        private IExpression targetExpression;

        /// <summary>
        /// Creates a new <see cref="UnaryOperationExpression"/> with the <paramref name="operation"/> and
        /// <paramref name="targetExpression"/> provided.
        /// </summary>
        /// <param name="operation">The operation referred to by the <see cref="UnaryOperationExpression"/>.</param>
        /// <param name="targetExpression">The <see cref="IExpression"/> which has the <paramref name="operation"/> applied to it.</param>
        public UnaryOperationExpression(UnaryOperations operation, IExpression targetExpression)
        {
            this.operation = operation;
            this.targetExpression = targetExpression;
        }

        public override CodeSnippetExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.LanguageProvider != null)
            {
                if (options.LanguageProvider is CSharpCodeProvider)
                    return new CodeSnippetExpression(this.ConvertToString(TranslationLanguage.CSharp));
                //else if (options.LanguageProvider is VBCodeProvider)
                //    return new CodeSnippetExpression(this.ConvertToString(TranslationLanguage.VisualBasic));
            }
            return new CodeSnippetExpression(this.ConvertToString(TranslationLanguage.CSharp) + " /* Unknown language. */");
        }

        /// <summary>
        /// Performs a look-up on the <see cref="UnaryOperationExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="UnaryOperationExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (this.TargetExpression != null)
                this.TargetExpression.GatherTypeReferences(ref result, options);
        }

        #region IUnaryOperationExpression Members

        /// <summary>
        /// Returns/sets the operation referred to by the <see cref="UnaryOperationExpression"/>.
        /// </summary>
        public UnaryOperations Operation
        {
            get
            {
                return this.operation;
            }
            set
            {
                this.operation = value;
            }
        }

        /// <summary>
        /// Returns/sets the <see cref="IExpression"/> which has the <see cref="Operation"/> applied to it.
        /// </summary>
        public IExpression TargetExpression
        {
            get
            {
                return this.targetExpression;
            }
            set
            {
                this.targetExpression = value;
            }
        }

        #endregion
    }
}
