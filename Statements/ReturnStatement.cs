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
    public class ReturnStatement :
        Statement<CodeMethodReturnStatement>,
        IReturnStatement
    {
        IExpression result;
        public ReturnStatement(IStatementBlock sourceBlock)
            : base(sourceBlock)
        { 
        }

        public ReturnStatement()
        {
        }

        public ReturnStatement(IExpression result)
        {
            this.result = result;
        }

        public ReturnStatement(IExpression result, IStatementBlock sourceBlock)
            : base(sourceBlock)
        {
            this.result = result;
        }
        
        public override CodeMethodReturnStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (result == null)
                return new CodeMethodReturnStatement();
            else
                return new CodeMethodReturnStatement(result.GenerateCodeDom(options));

        }

        #region IReturnStatement Members

        public IExpression Result
        {
            get
            {
                return result;
            }
            set
            {
                this.result = value;
            }
        }

        #endregion


        /// <summary>
        /// Performs a look-up on the <see cref="ReturnStatement"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ReturnStatement"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.result != null)
                this.Result.GatherTypeReferences(ref result, options);
        }
    }
}
