using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer.Statements
{
    public static class StatementExtensions
    {
        public static IYieldStatement Yield(this IBlockedStatement owner, IExpression result)
        {
            var r = new YieldStatement() { Result = result };
            owner.Statements.Add(r);
            return r;
        }

        public static IYieldBreakStatement YieldBreak(IBlockedStatement owner)
        {
            var r = new YieldBreakStatement();
            owner.Statements.Add(r);
            return r;
        }
    }
}
