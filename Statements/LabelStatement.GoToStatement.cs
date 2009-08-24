using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using System.Data;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    partial class LabelStatement
    {
        protected class GoToStatement :
            Statement<CodeGotoStatement>,
            IGoToLabelStatement
        {
            /// <summary>
            /// Data member for <see cref="LabelStatement"/>.
            /// </summary>
            private LabelStatement labelStatement = null;

            internal protected GoToStatement(IStatementBlock sourceBlock, LabelStatement labelStatement)
                : base(sourceBlock)
            {
                this.labelStatement = labelStatement;
            }

            #region IGoToLabelStatement Members

            public ILabelStatement LabelStatement
            {
                get
                {
                    return this.labelStatement;
                }
                set
                {
                    throw new ReadOnlyException("Internal goto statement type; label cannot be changed.");
                }
            }

            #endregion



            public override CodeGotoStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
            {
                if (this.LabelStatement == null)
                    throw new NullReferenceException("Code Execution Transfer target is null.");
                if (options.NameHandler.HandlesName(this.labelStatement.Name))
                    return new CodeGotoStatement(options.NameHandler.HandleName(this.LabelStatement.Name));
                else
                    return new CodeGotoStatement(this.LabelStatement.Name);
            }

            /// <summary>
            /// Performs a look-up on the <see cref="LabelStatement.GoToStatement"/> to determine its 
            /// dependencies.
            /// </summary>
            /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
            /// relates to the <see cref="ITypeReference"/> instance implementations
            /// that the <see cref="LabelStatement.GoToStatement"/> relies on.</param>
            /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
            /// guide the gathering process.</param>
            public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
            {
                return;
            }
        }
    }
}
