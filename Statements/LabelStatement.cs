using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    [Serializable]
    public partial class LabelStatement :
        Statement<CodeLabeledStatement>,
        ILabelStatement
    {
        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;

        public LabelStatement(IStatementBlock sourceBlock, string name)
            :base(sourceBlock)
        {
            this.name = name;
        }

        public LabelStatement(string name)
        {
            this.name = name;
        }

        public override CodeLabeledStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.NameHandler != null && options.NameHandler.HandlesName(this.Name))
                return new CodeLabeledStatement(options.NameHandler.HandleName(this.Name));
            else
                return new CodeLabeledStatement(this.name);
        }

        #region ILabelStatement Members

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public CodeGotoStatement GetCodeDomGoTo()
        {
            return new CodeGotoStatement(this.Name);
        }

        public IGoToLabelStatement GetGoTo(IStatementBlock sourceBlock)
        {
            return new GoToStatement(sourceBlock, this);
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="LabelStatement"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="LabelStatement"/> relies on.</param>
        /// <remarks>Labels carry no reference dependency.</remarks>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            //Labels carry no reference dependency.
            return;
        }

    }
}
