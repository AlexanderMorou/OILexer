using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    partial class StatementBlockLocalMember
    {
        [Serializable]
        public class DeclarationStatement :
            Statement<CodeVariableDeclarationStatement>,
            ILocalDeclarationStatement
        {
            /// <summary>
            /// Data member for <see cref="ReferencedMember"/>.
            /// </summary>
            private StatementBlockLocalMember referencedMember;
            public DeclarationStatement(StatementBlockLocalMember referencedMember)
                : base(referencedMember.ParentTarget)
            {
                this.referencedMember = referencedMember;
            }
                
            public override CodeVariableDeclarationStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
            {
                CodeVariableDeclarationStatement result = new CodeVariableDeclarationStatement();
                if (options.NameHandler.HandlesName(this.ReferencedMember))
                    result.Name = options.NameHandler.HandleName(this.ReferencedMember);
                else
                    result.Name = this.ReferencedMember.Name;
                if (this.ReferencedMember.LocalType != null)
                    result.Type = this.ReferencedMember.LocalType.GenerateCodeDom(options);
                if (this.referencedMember.InitializationExpression != null)
                    result.InitExpression = this.ReferencedMember.InitializationExpression.GenerateCodeDom(options);
                return result;
            }

            #region ILocalDeclarationStatement Members

            public IStatementBlockLocalMember ReferencedMember
            {
                get { return this.referencedMember; }
            }

            #endregion


            /// <summary>
            /// Performs a look-up on the <see cref="StatementBlockLocalMember.DeclarationStatement"/> to determine its 
            /// dependencies.
            /// </summary>
            /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
            /// relates to the <see cref="ITypeReference"/> instance implementations
            /// that the <see cref="StatementBlockLocalMember.DeclarationStatement"/> relies on.</param>
            /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
            /// guide the gathering process.</param>
            public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
            {
            }
        }
    }
}
