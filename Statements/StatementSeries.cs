using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    [Serializable]
    public abstract class StatementSeries :
        IStatementSeries
    {
        private IStatementBlock sourceBlock;
        public StatementSeries(IStatementBlock sourceBlock)
        {
            this.sourceBlock = sourceBlock;
        }

        #region IStatementSeries Members

        public virtual CodeStatement[] GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IStatement Members

        CodeStatement IStatement.GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool Skip
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotSupportedException("By default statement series aren't skippable.");
            }
        }

        #endregion

        #region IBlockParent Members


        public ICollection<string> DefinedLabelNames
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IDeclarationTarget Members

        public IDeclarationTarget ParentTarget
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Name
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Oilexer.Types.INameSpaceDeclaration GetRootNameSpace()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.sourceBlock = null;
        }

        #endregion

        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="StatementSeries"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="StatementSeries"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public abstract void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options);

        #endregion

        #region IStatement Members


        public IStatementBlock SourceBlock
        {
            get
            {
                return this.sourceBlock;
            }
            set
            {
                this.sourceBlock = value;
            }
        }

        #endregion
    }
}
