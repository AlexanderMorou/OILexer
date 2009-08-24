using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Expression;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for working with the conditional parts of a 
    /// <see cref="IConditionSeriesStatement"/>.
    /// </summary>
    public interface IConditionSeriesParts :
        IControlledStateCollection<IConditionBlock>
    {
        IConditionBlock AddCondition(IExpression condition);
        IConditionBlock AddCondition(IExpression condition, IStatement[] statements);
    }
}
