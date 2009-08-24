using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Types;
using Oilexer.Types.Members;
using System.Collections;
using Oilexer._Internal;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    [Serializable]
    public class EnumeratorStatement :
        BreakableBlockedStatement<CodeIterationStatement>,
        IEnumeratorStatement
    {
        private IMemberParentExpression enumeratorSource;
        private ITypeReference itemType;
        private IStatementBlockLocalMember currentMember;
        private IStatementBlockLocalMember enumMember;
        private ICastExpression currentMemberInit;
        public EnumeratorStatement(IStatementBlock sourceBlock)
            : base(sourceBlock)
        {
            enumMember = this.Statements.Locals.AddNew(this.Statements.Locals.GetUnusedName("__enumerator"), typeof(IEnumerator).GetTypeReference());
            enumMember.AutoDeclare = false;
            currentMember = this.Statements.Locals.AddNew(this.Statements.Locals.GetUnusedName("__current"), typeof(void).GetTypeReference());
            currentMember.AutoDeclare = true;
            currentMemberInit = new CastExpression(enumMember.GetReference().GetProperty("Current"), itemType);
            currentMember.InitializationExpression = currentMemberInit;
        }

        #region IEnumeratorStatement Members
        public IMemberParentExpression EnumeratorSource
        {
            get
            {
                return this.enumeratorSource;
            }
            set
            {
                this.enumeratorSource = value;
                if (value != null)
                {
                    enumMember.InitializationExpression = enumeratorSource.GetMethod("GetEnumerator").Invoke();
                    currentMemberInit.Type = this.ItemType;
                }
            }
        }

        public ITypeReference ItemType
        {
            get
            {
                return this.itemType;
            }
            set
            {
                if (currentMember != null)
                    currentMember.LocalType = value;
                this.itemType = value;
                currentMemberInit.Type = value;
            }
        }

        public IStatementBlockLocalMember CurrentMember
        {
            get { 
                return this.currentMember;
            }
        }

        #endregion

        #region IStatement<CodeIterationStatement> Members

        public override CodeIterationStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            #if LIGHT_CORE
            IEnumeratorExp enumMemberExp;
            enumMemberExp = new IEnumeratorExp(enumMember.GetReference());
            #endif
            CodeIterationStatement result = new CodeIterationStatement();
            result.Statements.Add(new CodeCommentStatement(String.Format("foreach ({0} {1} in {2})", this.currentMember.LocalType.TypeInstance.GetTypeName(options), this.enumMember.Name, _OIL._Core.GenerateExpressionSnippet(options, this.enumeratorSource.GenerateCodeDom(options)))));
            #if LIGHT_CORE 
            //LIGHT_CORE
            result.InitStatement = this.enumMember.GetDeclarationStatement().GenerateCodeDom(options);
            result.TestExpression = enumMemberExp.MoveNext().GenerateCodeDom(options);
            #else
            result.TestExpression = enumMember.GetReference().GetMethod("MoveNext").Invoke().GenerateCodeDom(options);
            #endif
            result.IncrementStatement = new CodeSnippetStatement("");
            result.Statements.AddRange(this.Statements.GenerateCodeDom(options));
            return result;
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="EnumeratorStatement"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="EnumeratorStatement"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            base.GatherTypeReferences(ref result, options);
            if (this.currentMember != null)
                this.currentMember.GatherTypeReferences(ref result, options);
            if (this.currentMemberInit != null)
                this.currentMemberInit.GatherTypeReferences(ref result, options);
            if (this.enumeratorSource != null)
                this.enumeratorSource.GatherTypeReferences(ref result, options);
            if (this.enumMember != null)
                this.enumMember.GatherTypeReferences(ref result, options);
            if (this.itemType != null)
                result.Add(this.ItemType);
        }
    }
}
