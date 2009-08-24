using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Statements;
using Oilexer.Expression;
using System.ComponentModel;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public partial class StatementBlockLocalMember :
        Member<IStatementBlock, CodeVariableDeclarationStatement>,
        IStatementBlockLocalMember
    {
        private ITypeReference localType;
        private bool autoDeclare;
        private IExpression initializationExpression;

        public StatementBlockLocalMember(TypedName nameAndType, IStatementBlock parentTarget)
            : base(nameAndType.Name, parentTarget)
        {
            this.LocalType = nameAndType.TypeReference;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override CodeVariableDeclarationStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        #region IStatementBlockLocalMember Members


        public bool AutoDeclare
        {
            get
            {
                return this.autoDeclare;
            }
            set
            {
                this.autoDeclare = value;
            }
        }

        public ILocalDeclarationStatement GetDeclarationStatement()
        {
            return new DeclarationStatement(this);
        }

        public ITypeReference LocalType
        {
            get
            {
                return this.localType;
            }
            set
            {
                this.localType = value;
            }
        }

        public IExpression InitializationExpression
        {
            get
            {
                return this.initializationExpression;
            }
            set
            {
                this.initializationExpression = value;
            }
        }
        public new ILocalReferenceExpression GetReference()
        {
            return new ReferenceExpression(this);
        }

        #endregion


        protected override IMemberReferenceExpression OnGetReference()
        {
            return this.GetReference();
        }

        /// <summary>
        /// Performs a look-up on the <see cref="StatementBlockLocalMember"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="StatementBlockLocalMember"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.localType != null)
                result.Add(this.LocalType);
            if (this.initializationExpression != null)
                this.InitializationExpression.GatherTypeReferences(ref result, options);
        }
    }
}
