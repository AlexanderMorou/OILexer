using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Ast.Walkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.FlowAnalysis
{
    internal class DiscriminatorEquivalenceWalker :
        StackedMethodEquivalenceWalker
    {
        public override bool Visit(IPrimitiveExpression<int> expression, object context)
        {
            /* Simple logic: The follow method predictions are all simple machines that follow a common routine.
             * The assignments are state, so the states will be different, but should be equivalent should the logical flow of all else be equivalent,
             * The ints that show up in the switches should be equivalent, unless we're actually returning what we should be doing, which needs to be the same.
             * If everything else, from a control-flow standpoint, is the same, the machines are equivalent in function.
             * A caveat: we do lose the parse tree context, from the comments, but in all honesty that is of little consequence. */
            var returnContext = this.ActiveContext.OfType<IReturnStatement>().FirstOrDefault();
            var switchContext = this.ActiveContext.OfType<ISwitchStatement>().FirstOrDefault();
            var assignContext = this.ActiveContext.OfType<IAssignmentExpression>().FirstOrDefault();
            if (returnContext == null && switchContext != null || assignContext != null)
                return true;
            return base.Visit(expression, context);
        }

        public bool MethodsAreEquivalent(IIntermediateMethodMember methodA, IIntermediateMethodMember methodB)
        {
            return this.Visit((IEnumerable<IStatement>)methodA, methodB);
        }

        public override bool Visit(ILabelStatement statement, IStatement context)
        {
            return true;
        }
    }
}
