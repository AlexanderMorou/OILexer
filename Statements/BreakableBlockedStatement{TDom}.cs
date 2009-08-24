using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Types;
using Oilexer.Expression;
using System.CodeDom;

namespace Oilexer.Statements
{
    [Serializable]
    public abstract partial class BreakableBlockedStatement<TDom> :
        BlockedStatement<TDom>,
        IBreakTargetStatement
        where TDom :
            CodeStatement
    {
        /// <summary>
        /// Data member for <see cref="UtilizeBreakMeasures"/>.
        /// </summary>
        private bool utilizeBreakMeasures;

        private IStatementBlockLocalMember breakLocal;
        /// <summary>
        /// Data member for <see cref="ExitPoint"/>.
        /// </summary>
        IBreakTargetExitPoint exitLabel;
        public BreakableBlockedStatement(IStatementBlock sourceBlock)
            : base(sourceBlock)
        {
        }
        #region IBreakTargetStatement Members

        public bool UtilizeBreakMeasures
        {
            get
            {
                return this.utilizeBreakMeasures;
            }
            set
            {
                this.utilizeBreakMeasures = value;
                this.BreakLocal.AutoDeclare = true;
                this.ExitLabel.Skip = false;
            }
        }
        public IStatementBlockLocalMember BreakLocal
        {
            get
            {
                if (this.breakLocal == null)
                    this.breakLocal = this.Statements.Locals.AddNew(new TypedName(this.Statements.Locals.GetUnusedName("__breakPoint"), typeof(bool)), new PrimitiveExpression(false));
                if (breakLocal.AutoDeclare != this.UtilizeBreakMeasures)
                    breakLocal.AutoDeclare = this.UtilizeBreakMeasures;
                return this.breakLocal;
            }

        }
        public IBreakTargetExitPoint ExitLabel
        {
            get {
                if (this.exitLabel == null)
                    this.exitLabel = new ExitPoint(this.SourceBlock);
                return this.exitLabel; 
            }
        }

        #endregion

    }
}
