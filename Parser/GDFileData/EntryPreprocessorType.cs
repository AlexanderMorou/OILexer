using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        DefineRule,
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
        /// <summary>
        /// The entry is a delayed string terminal-based directive.
        /// </summary>
        StringTerminal,
        /// <summary>
        /// Defines an entry with delayed inclusion.
        /// </summary>
        EntryContainer,
        /// <summary>
        /// Defines a symbol within the current parse context.
        /// </summary>
        DefineSymbol,
        /// <summary>
        /// Defines a series of production rule expressions for
        /// delayed inclusion.
        /// </summary>
        ProductionRuleSeries,
    }
}
