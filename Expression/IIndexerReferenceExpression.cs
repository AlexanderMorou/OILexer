using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;
using Oilexer.Comments;
using System.ComponentModel;
using System.CodeDom;

namespace Oilexer.Expression
{
    public interface IIndexerReferenceExpression :
        IMemberParentExpression<CodeIndexerExpression>,
        IMemberReferenceExpression,
        IAssignStatementTarget
    {
        IExpressionCollection Indices { get; }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new IMemberReferenceComment GetReferenceParticle();
    }
}
