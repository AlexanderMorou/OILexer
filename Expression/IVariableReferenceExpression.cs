using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;
using System.ComponentModel;
using Oilexer.Comments;
using System.CodeDom;

namespace Oilexer.Expression
{
    /// <summary>
    /// Defines properties and methods for working with a reference expression that points back to 
    /// a variable.
    /// </summary>
    public interface IVariableReferenceExpression :
        IExpression<CodeVariableReferenceExpression>,
        IMemberParentExpression<CodeVariableReferenceExpression>,
        IMemberReferenceExpression,
        IAssignStatementTarget
    {
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new IMemberReferenceComment GetReferenceParticle();

    }
}
