using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    public class ConditionStatement :
        BlockedStatement<CodeConditionStatement>,
        IConditionStatement
    {
        IStatementBlock falseStatements;
        IExpression condition;
        public ConditionStatement(IStatementBlock sourceBlock)
            : base(sourceBlock)
        {

        }

        public override CodeConditionStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeConditionStatement result = new CodeConditionStatement();
            result.TrueStatements.AddRange(base.Statements.GenerateCodeDom(options));
            if (this.falseStatements != null && this.falseStatements.Count > 0)
                result.FalseStatements.AddRange(this.FalseBlock.GenerateCodeDom(options));
            result.Condition = this.Condition.GenerateCodeDom(options);
            return result;
        }

        #region IConditionStatement Members

        public IStatementBlock FalseBlock
        {
            get {
                if (this.falseStatements == null)
                    this.falseStatements = new StatementBlock(this);
                return this.falseStatements;
            }
        }

        #endregion

        #region IConditionBlock Members

        public IExpression Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.condition = value;
            }
        }

        #endregion


        /// <summary>
        /// Performs a look-up on the <see cref="ConditionStatement"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ConditionStatement"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.condition != null)
                this.condition.GatherTypeReferences(ref result, options);
            if (this.falseStatements != null)
                this.FalseBlock.GatherTypeReferences(ref result, options);
        }
    }
}
