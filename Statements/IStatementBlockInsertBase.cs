using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using Oilexer.Types;
using Oilexer.Types.Members;
using System.Collections;

namespace Oilexer.Statements
{
    public interface IStatementBlockInsertBase
    {
        /// <summary>
        /// Inserts and returns a new <see cref="IBlockStatement"/> used to
        /// create a new operating scope.
        /// </summary>
        /// <returns>A new <see cref="IBlockStatement"/> instance.</returns>
        IBlockStatement NewBlock();
        /// <summary>
        /// Inserts and returns an iteration statement that iterates until 
        /// <paramref name="test"/> resolves to false, or a break/return statement is encountered.
        /// </summary>
        /// <param name="init">The initialization statement that is called before
        /// any iterations take place.</param>
        /// <param name="increment">The incremental statement that takes place
        /// after every iteration.</param>
        /// <param name="test">The test expression which determines whether to continue.</param>
        /// <returns>A new instance of an implementation of <see cref="IIterationStatement"/>.</returns>
        IIterationStatement Iterate(IStatement init, IStatement increment, IExpression test);
        /// <summary>
        /// Assigns a <paramref name="value"/> to <paramref name="target"/>. The result of this
        /// is inserted into the statement listing and returns the <see cref="IAssignStatement"/> 
        /// as a result.
        /// </summary>
        /// <param name="target">The <see cref="IAssignStatementTarget"/> which will receive the 
        /// value of '<paramref name="value"/>'.</param>
        /// <param name="value">The <see cref="IExpression"/> which relates to the value of <paramref name="target"/>.</param>
        /// <returns>A new <see cref="IAssignStatement"/> implementation instnace which can
        /// generate the CodeDOM objects necessary to assign <paramref name="target"/> to <paramref name="value"/>.</returns>
        IAssignStatement Assign(IAssignStatementTarget target, IExpression value);
        /// <summary>
        /// Enumerates the values of <paramref name="enumeratorSource"/> with the current member typed
        /// to <paramref name="itemType"/>.
        /// </summary>
        /// <param name="enumeratorSource">The source of the enumeration.  Must be a valid type that contains
        /// GetEnumerator().</param>
        /// <param name="itemType">The <see cref="ITypeReference"/> which the current member
        /// is type-bound to.  Initialization (retrieval of the <see cref="IEnumerator.Current"/>
        /// member) and casting will be performed to ensure this.</param>
        /// <returns>A new <see cref="IEnumeratorStatement"/> which defines the enumeration statement.</returns>
        IEnumeratorStatement Enumerate(IMemberParentExpression enumeratorSource, ITypeReference itemType);

        ILocalDeclarationStatement DefineLocal(IStatementBlockLocalMember local);
        /// <summary>
        /// Inserts a break into the current block.  This terminates the execution of 
        /// <see cref="IBreakTargetStatement"/> statements.
        /// </summary>
        /// <returns>A new <see cref="IBreakStatement"/> which exits the <see cref="IBreakTargetStatement"/>.</returns>
        IBreakStatement Break();
        IBreakStatement Break(IExpression conditionForBreak);
        IReturnStatement Return(IExpression result);
        /// <summary>
        /// Fully terminates the code execution for the entry at the top of the stack and returns
        /// the control to the caller.
        /// </summary>
        /// <returns>A new <see cref="IReturnStatement"/> implementation instance
        /// which relates to the stop execution statement.</returns>
        IReturnStatement Return();

        IConditionStatement IfThen(IExpression condition, params IStatement[] trueStatements);

        IConditionStatement IfThen(IExpression condition, IStatement[] trueStatements, IStatement[] falseStatements);
        ISwitchStatement SelectCase(IExpression caseSwitch);
        ISwitchStatement SelectCase(IExpression caseSwitch, IExpression[][] cases, IStatement[][] caseStatements);
        ICrementStatement Crement(IAssignStatementTarget target, CrementType crementType, CrementOperation operation);
        ICrementStatement Preincrement(IAssignStatementTarget target);
        ICrementStatement Postincrement(IAssignStatementTarget target);
        ICrementStatement Increment(IAssignStatementTarget target, CrementType crementType);
        ICrementStatement Predecrement(IAssignStatementTarget target);
        ICrementStatement Postdecrement(IAssignStatementTarget target);
        ICrementStatement Decrement(IAssignStatementTarget target, CrementType crementType);

        ISimpleStatement CallMethod(IMethodReferenceExpression method, params IExpression[] arguments);
        /// <summary>
        /// Invokes a method given the information about the call in the form of <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The data pertinent to make the call.</param>
        /// <returns>A new <see cref="ISimpleStatement"/> implementation instance which relates
        /// to the call.</returns>
        ISimpleStatement CallMethod(IMethodInvokeExpression method);
        /// <summary>
        /// Returns the locals defined within the <see cref="IStatementBlock"/>.
        /// </summary>
        IStatementBlockLocalMembers Locals { get; }
    }
}
