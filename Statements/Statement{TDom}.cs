using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    /// <summary>
    /// A statement that exports a <see cref="CodeStatement"/> of <typeparamref name="TDom"/>.
    /// </summary>
    /// <typeparam name="TDom">The type of statement that is exported into CodeDom</typeparam>
    [Serializable]
    public abstract class Statement<TDom> :
        IStatement<TDom>
            where TDom :
            CodeStatement
    {
        /// <summary>
        /// Data member for <see cref="SourceBlock"/>.
        /// </summary>
        private IStatementBlock sourceBlock;

        protected Statement(IStatementBlock sourceBlock)
        {
            this.sourceBlock = sourceBlock;
        }

        protected Statement() { }

        #region IStatement<TDom> Members

        public abstract TDom GenerateCodeDom(ICodeDOMTranslationOptions options);

        #endregion

        #region IStatement Members
        public virtual bool Skip
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotImplementedException("Cannot skip normal statements.");
            }
        }

        CodeStatement IStatement.GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return this.GenerateCodeDom(options);
        }

        public virtual IStatementBlock SourceBlock
        {
            get { return this.sourceBlock; }
            set { this.sourceBlock = value; }
        }

        #endregion

        #region IStatement Members

        /// <summary>
        /// Performs a look-up on the <see cref="Statement{TDom}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="Statement{TDom}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public abstract void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options);

        #endregion
        public override string ToString()
        {
            IIntermediateCodeTranslator iict = _OIL._Core.DefaultCSharpCodeTranslator;
            iict.Target = new StringFormWriter();
            iict.TranslateStatement(this);
            return iict.Target.ToString();
        }
    }
}
