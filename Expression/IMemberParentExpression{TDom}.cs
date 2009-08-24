using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for the reference to a member that contains
    /// members of its own.
    /// </summary>
    /// <typeparam name="TDom">The type of CodeDom object which the <see cref="IMemberParentExpression{TDom}"/> translates to.</typeparam>
    public interface IMemberParentExpression<TDom> :
        IExpression<TDom>,
        IMemberParentExpression
        where TDom :
            CodeExpression,
            new()
    {
    }
}
