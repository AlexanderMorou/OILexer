using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
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
        /// The repeat options defined on a template are invalid.
        /// </summary>
        InvalidRepeatOptions,
        /// <summary>
        /// StringTerminal '{0}' not found.
        /// </summary>
        IncludeFileNotFound = 500,
        FixedArgumentCountError = 600,
        DynamicArgumentCountError,
        RuleNotTemplate,
        RuleIsTemplate,
        UndefinedTokenReference,
        UndefinedRuleReference,
        NoStartDefined,
        InvalidStartDefined,
        RuleNeverUsed,
        LanguageDefinedError = 0x1000,
    }
}
