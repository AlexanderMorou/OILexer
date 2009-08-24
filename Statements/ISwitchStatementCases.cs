using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Expression;

namespace Oilexer.Statements
{
    public interface ISwitchStatementCases :
        IControlledStateCollection<ISwitchStatementCase>
    {

        ISwitchStatementCase AddNew(IExpression caseTarget);
        /// <summary>
        /// Adds a new <see cref="ISwitchStatementCase"/> as the default case.
        /// </summary>
        /// <returns>A new <see cref="ISwitchStatementCase"/> instance as the default case
        /// for the switch statement.</returns>
        ISwitchStatementCase AddNew();
        /// <summary>
        /// Adds a new <see cref="ISwtichStatementCase"/> with the <paramref name="lastIsDefaultCase"/> 
        /// causing the last case being the default case.
        /// </summary>
        /// <param name="lastIsDefaultCase">A boolean value determining whether the last case in the
        /// <see cref="ISwitchStatementCase"/> is a switch stateent.</param>
        /// <param name="caseTargets">The <see cref="IExpressionCollection"/> that relates to the
        /// cases of the <see cref="ISwitchStatementCase"/>.</param>
        /// <returns></returns>
        ISwitchStatementCase AddNew(bool lastIsDefaultCase, IExpressionCollection caseTargets);
    }
}
