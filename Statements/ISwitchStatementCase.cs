using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Expression;
using Oilexer.Types;

namespace Oilexer.Statements
{
    public interface ISwitchStatementCase :
        IBlockParent,
        IStatementBlockInsertBase,
        ITypeReferenceable
    {
        /// <summary>
        /// Returns the <see cref="IExpressionCollection"/> which denotes the expressions associated
        /// to the <see cref="ISwitchStatementCase"/>.
        /// </summary>
        /// <remarks>Every <see cref="ISwitchStatementCase"/> auto-terminates with a 'break' statement
        /// thus ensure that you properly group your cases using the <see cref="Cases"/> property
        /// instead of adding more individual cases.</remarks>
        IExpressionCollection Cases { get; }
        /// <summary>
        /// Returns/sets whether the last case in the <see cref="ISwitchStatementCase"/> is the
        /// default case.
        /// </summary>
        bool LastIsDefaultCase { get; set; }
    }
}
