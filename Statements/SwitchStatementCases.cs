using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Expression;
using System.Collections;

namespace Oilexer.Statements
{
    public class SwitchStatementCases :
        ControlledStateCollection<ISwitchStatementCase>,
        ISwitchStatementCases
    {
        /// <summary>
        /// Data member which is used to pass on the source block to the entities created
        /// by the <see cref="SwitchStatementCases"/>.
        /// </summary>
        private IStatementBlock sourceBlock;

        internal SwitchStatementCases(IStatementBlock sourceBlock)
        {
            this.sourceBlock = sourceBlock;
        }

        #region ISwitchStatementCases Members

        public ISwitchStatementCase AddNew(IExpression caseTarget)
        {
            ISwitchStatementCase @case = new SwitchStatementCase(this.sourceBlock, caseTarget);
            this.baseCollection.Add(@case);
            return @case;
        }

        public ISwitchStatementCase AddNew()
        {
            ISwitchStatementCase @case = new SwitchStatementCase(this.sourceBlock);
            this.baseCollection.Add(@case);
            return @case;
        }

        public ISwitchStatementCase AddNew(bool lastIsDefaultCase, IExpressionCollection caseTargets)
        {
            ISwitchStatementCase @case = new SwitchStatementCase(this.sourceBlock, caseTargets, lastIsDefaultCase);
            this.baseCollection.Add(@case);
            return @case;
        }

        #endregion

    }
}
