using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;
using Oilexer.Types;
using Oilexer.Comments;
using System.CodeDom;

namespace Oilexer.Expression
{
    /// <summary>
    /// Refers to a property of an <see cref="IMemberParentType"/>.
    /// </summary>
    public interface IPropertyReferenceExpression :
        IAssignStatementTarget,
        IExpression<CodePropertyReferenceExpression>,
        IMemberReferenceExpression,
        IMemberParentExpression<CodePropertyReferenceExpression>
    {
        new IPropertyReferenceComment GetReferenceParticle();

    }
}
