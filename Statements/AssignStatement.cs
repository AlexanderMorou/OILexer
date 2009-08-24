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
    public class AssignStatement :
        Statement<CodeAssignStatement>,
        IAssignStatement
    {
        /// <summary>
        /// Data member for <see cref="Value"/>
        /// </summary>
        private IExpression value;
        /// <summary>
        /// Data member for <see cref="Reference"/>.
        /// </summary>
        private IAssignStatementTarget reference;

        public AssignStatement(IStatementBlock sourceBlock, IAssignStatementTarget reference, IExpression value)
            : base(sourceBlock)
        {
            this.reference = reference;
            this.value = value;
        }

        public AssignStatement(IAssignStatementTarget reference, IExpression value)
        {
            this.reference = reference;
            this.value = value;
        }

        public override CodeAssignStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeAssignStatement(this.reference.GenerateCodeDom(options), this.value.GenerateCodeDom(options));
        }

        #region IAssignStatement Members

        public IAssignStatementTarget Reference
        {
            get { return this.reference; }
        }

        public IExpression Value
        {
            get {
                return this.value;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="AssignStatement"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="AssignStatement"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.reference != null)
                this.reference.GatherTypeReferences(ref result, options);
            if (this.value != null)
                this.value.GatherTypeReferences(ref result, options);
        }
    }
}
