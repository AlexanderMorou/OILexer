using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    /// <summary>
    /// The error that was thrown.
    /// </summary>
    public enum GDParserErrors
    {
        /// <summary>
        /// Expected: {0}.
        /// </summary>
        Expected = 100,
        /// <summary>
        /// Expected: end of line.
        /// </summary>
        ExpectedEndOfLine = 101,
        /// <summary>
        /// Expected end of file.
        /// </summary>
        ExpectedEndOfFile = 102,
        /// <summary>
        /// Unexpected: {0}.
        /// </summary>
        Unexpected = 200,
        /// <summary>
        /// Unexpected end of line.
        /// </summary>
        UnexpectedEndOfLine = 201,
        /// <summary>
        /// Unexpected end of file.
        /// </summary>
        UnexpectedEndOfFile = 202,
        /// <summary>
        /// Unknown symbol '{0}'.
        /// </summary>
        UnknownSymbol = 300,
        /// <summary>
        /// Invalid escape.
        /// </summary>
        InvalidEscape = 400,
        /// <summary>
        /// StringTerminal '{0}' not found.
        /// </summary>
        IncludeFileNotFound = 500,
        /// <summary>
        /// For known commands, this is also possible at a 
        /// syntax level.
        /// </summary>
        FixedArgumentCountError = 600,
    }
    public enum GDLogicErrors
    {
        /// <summary>
        /// The repeat options defined on a template are invalid.
        /// </summary>
        InvalidRepeatOptions = 501,
        FixedArgumentCountError = 600,
        DynamicArgumentCountError,
        RuleNotTemplate,
        RuleIsTemplate,
        UndefinedTokenReference,
        UndefinedRuleReference,
        NoStartDefined,
        InvalidStartDefined,
        InvalidPreprocessorCondition,
        DuplicateTermDefined = 902,
        UndefinedAddRuleTarget,
        UnexpectedLiteralEntry,
        UnexpectedUndefinedEntry,
        ParameterMustExpectRule,
        InvalidIsDefinedTarget,
        LanguageDefinedError = 0x1000,
        ReferenceError = 0x2000,
    }
}
