using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// The preprocessor type.
    /// </summary>
    public enum EntryPreprocessorType
    {
        /// <summary>
        /// The preprocessor is an if statement.
        /// </summary>
        /// <remarks>#if</remarks>
        If,
        /// <summary>
        /// The preprocessor is an if statement checking if something is not defined.
        /// </summary>
        /// <remarks>#ifndef</remarks>
        IfNotDefined,
        /// <summary>
        /// The preprocessor is an if statement checking if something is defined.
        /// </summary>
        /// <remarks>#ifdef</remarks>
        IfDefined,
        /// <summary>
        /// The preprocessor is an else if statement.
        /// </summary>
        /// <remarks>#elif</remarks>
        ElseIf,
        /// <summary>
        /// The preprocessor is an else if statement checking if something else is defined.
        /// </summary>
        /// <remarks>#elifdef</remarks>
        ElseIfDefined,
        /// <summary>
        /// The preprocessor is an else statement
        /// </summary>
        /// <remarks>#else</remarks>
        Else,
        /// <summary>
        /// The preprocessor ends an <see cref="If"/>, <see cref="IfNotDefined"/> or <see cref="IfDefined"/>
        /// block.
        /// </summary>
        /// <remarks>#endif</remarks>
        EndIf,
        /// <summary>
        /// The preprocessor defines the given identifier given the expression series.
        /// </summary>
        /// <remarks>#define identifier = expression<sub>1</sub> | expression<sub>2</sub> ... | ... | expression<sub>n</sub>;</remarks>
        Define,
        /// <summary>
        /// The preprocessor adds the given expression to the designated target.
        /// </summary>
        /// <remarks>#addrule Target, expression<sub>1</sub> | expression<sub>2</sub> ... | ... | expression<sub>n</sub>;</remarks>
        AddRule,
        /// <summary>
        /// The preprocessor is a throw command.
        /// </summary>
        /// <remarks>#throw ErrorName, Arg1, Arg2, ..., ArgN</remarks>
        Throw,
        /// <summary>
        /// The preprocessor is a return command
        /// </summary>
        /// <remarks>#return expression<sub>1</sub> | expression<sub>2</sub> ... | ... | expression<sub>n</sub>;</remarks>
        Return,
    }
    public interface IPreprocessorDirective
    {
        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="IPreprocessorDirective"/> was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the line index the <see cref="IPreprocessorDirective"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the position the <see cref="IPreprocessorDirective"/> was declared at.
        /// </summary>
        long Position { get; }

        /// <summary>
        /// Returns the type of the preprocessor.
        /// </summary>
        EntryPreprocessorType Type { get; }
        
    }
}
