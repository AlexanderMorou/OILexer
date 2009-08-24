using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Expression
{
    public interface IEventReferenceExpression :
        IExpression<CodeEventReferenceExpression>,
        IMemberReferenceExpression
    {
        /// <summary>
        /// Returns the name of the event reference.
        /// </summary>
        new string Name { get; }
    }
}
