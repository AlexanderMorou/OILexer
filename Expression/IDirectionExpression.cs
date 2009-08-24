using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Expression
{
    public interface IDirectionExpression :
        IExpression<CodeDirectionExpression>
    {
        /// <summary>
        /// Determines the direction to transition the reference.
        /// </summary>
        FieldDirection Direction { get; set; }
        /// <summary>
        /// Determines the reference to direct.
        /// </summary>
        IExpression DirectedExpression { get; set; }
    }
}
