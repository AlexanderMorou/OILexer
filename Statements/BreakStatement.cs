using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Types.Members;
using Oilexer.Types;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Statements
{
    [Serializable]
    public class BreakStatement :
        StatementSeries,
        IBreakStatement
    {
        /// <summary>
        /// Data member for <see cref="Condition"/>.
        /// </summary>
        private IExpression condition;
        /// <summary>
        /// Data member for <see cref="TerminalVariable"/>.
        /// </summary>
        private IStatementBlockLocalMember terminalVariable;
        /// <summary>
        /// Data member for <see cref="ExitSymbol"/>.
        /// </summary>
        private IBreakTargetExitPoint exitSymbol;

        public BreakStatement(IStatementBlock sourceBlock)
            : base(sourceBlock)
        {
            IStatementBlockLocalMember local = null;
            IBreakTargetExitPoint exitPoint = null;
            IDeclarationTarget parent = sourceBlock;
            //Search up for the block that needs the break.
            while (parent != null && local == null)
            {
                if (parent is IBreakTargetStatement)
                {
                    local = ((IBreakTargetStatement)parent).BreakLocal;
                    exitPoint = ((IBreakTargetStatement)parent).ExitLabel;
                    ((IBreakTargetStatement)parent).UtilizeBreakMeasures = true;
                    break;
                }
                parent = parent.ParentTarget;
            }
            if (local == null)
            {
                throw new InvalidOperationException("Cannot insert a break into a non-terminable block.");
            }
            this.terminalVariable = local;
            this.exitSymbol = exitPoint;
        }

        public override CodeStatement[] GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            List<CodeStatement> result = new List<CodeStatement>();

            CodeVariableReferenceExpression varRef = TerminalVariable.GetReference().GenerateCodeDom(options);
            result.Add(new CodeSnippetStatement());
            if (this.condition != null)
            {
                result.Add(new CodeAssignStatement(varRef, this.condition.GenerateCodeDom(options)));
                result.Add(new CodeConditionStatement(varRef, new CodeCommentStatement("break;"), ExitSymbol.GetCodeDomGoTo()));
            }
            else
            {
                result.Add(new CodeCommentStatement("break;"));
                result.Add(ExitSymbol.GetCodeDomGoTo());
            }
            return result.ToArray();
        }

        #region IBreakStatement Members

        public IExpression Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                this.condition = value;
            }
        }

        #endregion

        #region IBreakStatement Members


        public IStatementBlockLocalMember TerminalVariable
        {
            get { return this.terminalVariable; }
        }

        public IBreakTargetExitPoint ExitSymbol
        {
            get { return this.exitSymbol; }
        }

        #endregion


        /// <summary>
        /// Performs a look-up on the <see cref="BreakStatement"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="BreakStatement"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.condition != null)
                this.condition.GatherTypeReferences(ref result, options);
            if (this.terminalVariable != null)
                this.terminalVariable.GatherTypeReferences(ref result, options);
        }
    }
}
