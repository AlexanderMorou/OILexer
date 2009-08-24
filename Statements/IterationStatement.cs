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
    public class IterationStatement :
        BreakableBlockedStatement<CodeIterationStatement>,
        IIterationStatement
    {
        /// <summary>
        /// Data member for <see cref="InitializationStatement"/>.
        /// </summary>
        private IStatement initializationStatement;
        /// <summary>
        /// Data member for <see cref="IncrementStatement"/>
        /// </summary>
        private IStatement incrementStatement;
        /// <summary>
        /// Data member for <see cref="TestExression"/>.
        /// </summary>
        private IExpression testStatement;

        public IterationStatement(IStatementBlock sourceBlock)
            :base(sourceBlock)
        {

        }

        public override CodeIterationStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeIterationStatement result = new CodeIterationStatement();
            if (this.TestExpression == null)
                throw new InvalidOperationException("The case for the iteration logic cannot be null.");
            if (this.InitializationStatement != null)
                result.InitStatement = this.InitializationStatement.GenerateCodeDom(options);
            else
                result.InitStatement = new CodeSnippetStatement(string.Empty);
            if (this.IncrementStatement != null)
                result.IncrementStatement = this.IncrementStatement.GenerateCodeDom(options);
            else
                result.IncrementStatement = new CodeSnippetStatement(string.Empty);
            result.TestExpression = this.TestExpression.GenerateCodeDom(options);
            result.Statements.AddRange(this.Statements.GenerateCodeDom(options));
            return result;
        }

        #region IIterationStatement Members

        public IStatement InitializationStatement
        {
            get
            {
                return this.initializationStatement;
            }
            set
            {
                this.initializationStatement = value;
            }
        }

        public IStatement IncrementStatement
        {
            get
            {
                return this.incrementStatement;
            }
            set
            {
                this.incrementStatement = value;
            }
        }

        public IExpression TestExpression
        {
            get
            {
                return this.testStatement;
            }
            set
            {
                this.testStatement = value;
            }
        }

        #endregion


        /// <summary>
        /// Performs a look-up on the <see cref="IterationStatement"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="IterationStatement"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.incrementStatement != null)
                this.IncrementStatement.GatherTypeReferences(ref result, options);
            if (this.initializationStatement != null)
                this.initializationStatement.GatherTypeReferences(ref result, options);
            if (this.testStatement != null)
                this.TestExpression.GatherTypeReferences(ref result, options);
        }

    }
}
