using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Properties;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    partial class GrammarCore
    {
        internal class CompilerErrors
        {
            /// <summary>
            /// Reference compiler error message for when a rule is used like a template but the
            /// number of fixed arguments provided does not match any variation of the template.
            /// </summary>
            public static ICompilerReferenceError FixedArgumentMismatch
            {
                get
                {
                    if (GrammarCore.CompilerErrors._FixedArgumentMismatch == null)
                        _FixedArgumentMismatch = new CompilerReferenceError(Resources.GrammarParserErrors_FixedArgumentCountError, (int)GDLogicErrors.FixedArgumentCountError);
                    return _FixedArgumentMismatch;
                }
            }

            private static ICompilerReferenceError _FixedArgumentMismatch;

            /// <summary>
            /// Reference compiler error message for when a rule is used like a template,
            /// but no template of the same name can be found.
            /// </summary>
            public static ICompilerReferenceError RuleNotTemplate
            {
                get
                {
                    if (GrammarCore.CompilerErrors._RuleNotTemplate == null)
                        _RuleNotTemplate = new CompilerReferenceError(Resources.GrammarParserErrors_RuleNotTemplate, (int)GDLogicErrors.RuleNotTemplate);
                    return _RuleNotTemplate;
                }
            }

            private static ICompilerReferenceError _RuleNotTemplate;
            /// <summary>
            /// Reference compiler error message for when a rule is used without template
            /// insertions, but is only defined as a template.
            /// </summary>
            public static ICompilerReferenceError RuleIsTemplate
            {
                get
                {
                    if (_RuleIsTemplate == null)
                        _RuleIsTemplate = new CompilerReferenceError(Resources.GrammarParserErrors_RuleIsTemplate, (int)GDLogicErrors.RuleIsTemplate);
                    return _RuleIsTemplate;
                }
            }
            private static ICompilerReferenceError _RuleIsTemplate;
            /// <summary>
            /// Reference compiler error message for when a token expression refers 
            /// to some term which is undefined.
            /// </summary>
            public static ICompilerReferenceError UndefinedTokenReference
            {
                get
                {
                    if (_UndefinedTokenReference == null)
                        _UndefinedTokenReference = new CompilerReferenceError(Resources.GrammarParserErrors_UndefinedTokenReference, (int)GDLogicErrors.UndefinedTokenReference);
                    return _UndefinedTokenReference;
                }
            }
            private static ICompilerReferenceError _UndefinedTokenReference;
            /// <summary>
            /// Reference compiler error message for when a rule expression refers 
            /// to some rule or token which is undefined.
            /// </summary>
            public static ICompilerReferenceError UndefinedRuleReference
            {
                get
                {
                    if (_UndefinedRuleReference == null)
                        _UndefinedRuleReference = new CompilerReferenceError(Resources.GrammarParserErrors_UndefinedRuleReference, (int)GDLogicErrors.UndefinedRuleReference);
                    return _UndefinedRuleReference;
                }
            }
            private static ICompilerReferenceError _UndefinedRuleReference;
            /// <summary>
            /// Reference compiler error message for when a grammar has no start rule.
            /// </summary>
            public static ICompilerReferenceError NoStartDefined
            {
                get
                {
                    if (_NoStartDefined == null)
                        _NoStartDefined = new CompilerReferenceError(Resources.GrammarParserErrors_NoStartDefined, (int)GDLogicErrors.NoStartDefined);
                    return _NoStartDefined;
                }
            }
            private static ICompilerReferenceError _NoStartDefined;
            /// <summary>
            /// Reference compiler error message for when a grammar has an invalid
            /// start defined, such as a terminal instead of a nonterminal.
            /// </summary>
            public static ICompilerReferenceError InvalidStartDefined
            {
                get
                {
                    if (_InvalidStartDefined == null)
                        _InvalidStartDefined = new CompilerReferenceError(Resources.GrammarParserErrors_InvalidStartDefined, (int)GDLogicErrors.InvalidStartDefined);
                    return _InvalidStartDefined;
                }
            }
            private static ICompilerReferenceError _InvalidStartDefined;
            /// <summary>
            /// Reference compiler error message for when a template is used with a 
            /// series of dynamic arguments, but the number of arguments provided, 
            /// past the fixed arguments, does not evenly distribute across the dynamic
            /// arguments.
            /// </summary>
            public static ICompilerReferenceError DynamicArgumentCountError
            {
                get
                {
                    if (_DynamicArgumentCountError == null)
                        _DynamicArgumentCountError = new CompilerReferenceError(Resources.GrammarErrors_DynamicArgumentCountError, (int)GDLogicErrors.DynamicArgumentCountError);
                    return _DynamicArgumentCountError;
                }
            }
            private static ICompilerReferenceError _DynamicArgumentCountError;
            /// <summary>
            /// Reference compiler error message associated to templates which define a series 
            /// of dynamic arguments, and then define a series of fixed arguments, 
            /// which is invalid.
            /// </summary>
            public static ICompilerReferenceError InvalidRepeatOptions
            {
                get
                {
                    if (_InvalidRepeatOptions == null)
                        _InvalidRepeatOptions = new CompilerReferenceError(Resources.GrammarParserErrors_InvalidRepeatOptions, (int)GDLogicErrors.InvalidRepeatOptions);
                    return _InvalidRepeatOptions;
                }
            }
            private static ICompilerReferenceError _InvalidRepeatOptions;
            /// <summary>
            /// Reference compiler error message associated to a duplicate term being defined
            /// by the '&#35;define' directive.
            /// </summary>
            public static ICompilerReferenceError DuplicateTermDefined
            {
                get
                {
                    if (_DuplicateTermDefined == null)
                        _DuplicateTermDefined = new CompilerReferenceError(Resources.GrammarErrors_DuplicateTermDefined, (int)GDLogicErrors.DuplicateTermDefined);
                    return _DuplicateTermDefined;
                }
            }
            private static ICompilerReferenceError _DuplicateTermDefined;
            /// <summary>
            /// Reference compiler error message associated to the &#35;AddRule
            /// directive arises when the rule associated to the directive is
            /// not defined.
            /// </summary>
            public static ICompilerReferenceError UndefinedAddRuleTarget
            {
                get
                {
                    if (_UndefinedAddRuleTarget == null)
                        _UndefinedAddRuleTarget = new CompilerReferenceError(Resources.GrammarErrors_AddRuleTargetUndefined, (int)GDLogicErrors.UndefinedAddRuleTarget);
                    return _UndefinedAddRuleTarget;
                }
            }
            private static ICompilerReferenceError _UndefinedAddRuleTarget;
            /// <summary>
            /// Reference compiler error message associated to the deliteralization
            /// process of the linking stage when an unexpected, or unknown, literal
            /// is encountered.
            /// </summary>
            /// <remarks>Likely to occur if compiler is extended, but the appropriate code isn't
            /// updated to follow suit.</remarks>
            public static ICompilerReferenceError UnexpectedLiteralEntry
            {
                get
                {
                    if (_UnexpectedLiteralEntry == null)
                        _UnexpectedLiteralEntry = new CompilerReferenceError(Resources.GrammarCompilerErrors_UnexpectedLiteralEntry, (int)GDLogicErrors.UnexpectedLiteralEntry);
                    return _UnexpectedLiteralEntry;
                }
            }
            private static ICompilerReferenceError _UnexpectedLiteralEntry;
            public static ICompilerReferenceError InvalidPreprocessorCondition
            {
                get
                {
                    if (_InvalidPreprocessorCondition == null)
                        _InvalidPreprocessorCondition = new CompilerReferenceError(Resources.GrammarCompilerErrors_InvalidPreprocessorCondition, (int)GDLogicErrors.InvalidPreprocessorCondition);
                    return _InvalidPreprocessorCondition;
                }
            }
            private static ICompilerReferenceError _InvalidPreprocessorCondition;
            public static ICompilerReferenceError IsDefinedTemplateParameterMustExpectRule
            {
                get
                {
                    if (_IsDefinedTemplateParameterMustExpectRule == null)
                        _IsDefinedTemplateParameterMustExpectRule = new CompilerReferenceError(Resources.IsDefinedTemplateParameterMustExpectRule, (int)GDLogicErrors.ParameterMustExpectRule);
                    return _IsDefinedTemplateParameterMustExpectRule;
                }
            }
            private static ICompilerReferenceError _IsDefinedTemplateParameterMustExpectRule;
            public static ICompilerReferenceError InvalidDefinedTarget
            {
                get
                {
                    if (_InvalidDefinedTarget == null)
                        _InvalidDefinedTarget = new CompilerReferenceError(Resources.GrammarCompilerErrors_InvalidIsDefinedTarget, (int)GDLogicErrors.InvalidIsDefinedTarget);
                    return _InvalidDefinedTarget;
                }
            }
            private static ICompilerReferenceError _InvalidDefinedTarget;
            public static ICompilerReferenceError LanguageDefinedError
            {
                get
                {
                    if (_LanguageDefinedError == null)
                        _LanguageDefinedError = new CompilerReferenceError(Resources.GrammarCompilerErrors_LanguageDefinedError, (int)GDLogicErrors.LanguageDefinedError);
                    return _LanguageDefinedError;
                }
            }
            private static ICompilerReferenceError _LanguageDefinedError;
            public static ICompilerReferenceError UnexpectedUndefinedEntry
            {
                get
                {
                    if (_UnexpectedUndefinedEntry == null)
                        _UnexpectedUndefinedEntry = new CompilerReferenceError(Resources.GrammarCompilerErrors_UnexpectedUndefinedEntry, (int)GDLogicErrors.UnexpectedUndefinedEntry);
                    return _UnexpectedUndefinedEntry;
                }
            }
            private static ICompilerReferenceError _UnexpectedUndefinedEntry;
        }
    }
}
