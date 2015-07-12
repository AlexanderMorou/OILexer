using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Properties;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    partial class OilexerGrammarCore
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
                    if (OilexerGrammarCore.CompilerErrors._FixedArgumentMismatch == null)
                        _FixedArgumentMismatch = new CompilerReferenceError(ParserResources.GrammarParserErrors_FixedArgumentCountError, (int)OilexerGrammarLogicErrors.FixedArgumentCountError);
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
                    if (OilexerGrammarCore.CompilerErrors._RuleNotTemplate == null)
                        _RuleNotTemplate = new CompilerReferenceError(ParserResources.GrammarParserErrors_RuleNotTemplate, (int)OilexerGrammarLogicErrors.RuleNotTemplate);
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
                        _RuleIsTemplate = new CompilerReferenceError(ParserResources.GrammarParserErrors_RuleIsTemplate, (int)OilexerGrammarLogicErrors.RuleIsTemplate);
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
                        _UndefinedTokenReference = new CompilerReferenceError(ParserResources.GrammarParserErrors_UndefinedTokenReference, (int)OilexerGrammarLogicErrors.UndefinedTokenReference);
                    return _UndefinedTokenReference;
                }
            }
            private static ICompilerReferenceError _UndefinedTokenReference;
            /// <summary>
            /// Reference compiler error message for when a token item is referenced
            /// that is not a literal.
            /// </summary>
            public static ICompilerReferenceError InvalidTokenReference
            {
                get
                {
                    if (_InvalidTokenReference == null)
                        _InvalidTokenReference = new CompilerReferenceError(ParserResources.GrammarParserErrors_InvalidTokenReference, (int)OilexerGrammarLogicErrors.UndefinedTokenReference);
                    return _InvalidTokenReference;
                }
            }
            private static ICompilerReferenceError _InvalidTokenReference;
            /// <summary>
            /// Reference compiler error message for when a rule expression refers 
            /// to some rule or token which is undefined.
            /// </summary>
            public static ICompilerReferenceError UndefinedRuleReference
            {
                get
                {
                    if (_UndefinedRuleReference == null)
                        _UndefinedRuleReference = new CompilerReferenceError(ParserResources.GrammarParserErrors_UndefinedRuleReference, (int)OilexerGrammarLogicErrors.UndefinedRuleReference);
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
                        _NoStartDefined = new CompilerReferenceError(ParserResources.GrammarParserErrors_NoStartDefined, (int)OilexerGrammarLogicErrors.NoStartDefined);
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
                        _InvalidStartDefined = new CompilerReferenceError(ParserResources.GrammarParserErrors_InvalidStartDefined, (int)OilexerGrammarLogicErrors.InvalidStartDefined);
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
                        _DynamicArgumentCountError = new CompilerReferenceError(ParserResources.GrammarErrors_DynamicArgumentCountError, (int)OilexerGrammarLogicErrors.DynamicArgumentCountError);
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
                        _InvalidRepeatOptions = new CompilerReferenceError(ParserResources.GrammarParserErrors_InvalidRepeatOptions, (int)OilexerGrammarLogicErrors.InvalidRepeatOptions);
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
                        _DuplicateTermDefined = new CompilerReferenceError(ParserResources.GrammarErrors_DuplicateTermDefined, (int)OilexerGrammarLogicErrors.DuplicateTermDefined);
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
                        _UndefinedAddRuleTarget = new CompilerReferenceError(ParserResources.GrammarErrors_AddRuleTargetUndefined, (int)OilexerGrammarLogicErrors.UndefinedAddRuleTarget);
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
                        _UnexpectedLiteralEntry = new CompilerReferenceError(ParserResources.GrammarCompilerErrors_UnexpectedLiteralEntry, (int)OilexerGrammarLogicErrors.UnexpectedLiteralEntry);
                    return _UnexpectedLiteralEntry;
                }
            }
            private static ICompilerReferenceError _UnexpectedLiteralEntry;
            public static ICompilerReferenceError InvalidPreprocessorCondition
            {
                get
                {
                    if (_InvalidPreprocessorCondition == null)
                        _InvalidPreprocessorCondition = new CompilerReferenceError(ParserResources.GrammarCompilerErrors_InvalidPreprocessorCondition, (int)OilexerGrammarLogicErrors.InvalidPreprocessorCondition);
                    return _InvalidPreprocessorCondition;
                }
            }
            private static ICompilerReferenceError _InvalidPreprocessorCondition;
            public static ICompilerReferenceError IsDefinedTemplateParameterMustExpectRule
            {
                get
                {
                    if (_IsDefinedTemplateParameterMustExpectRule == null)
                        _IsDefinedTemplateParameterMustExpectRule = new CompilerReferenceError(ParserResources.IsDefinedTemplateParameterMustExpectRule, (int)OilexerGrammarLogicErrors.ParameterMustExpectRule);
                    return _IsDefinedTemplateParameterMustExpectRule;
                }
            }
            private static ICompilerReferenceError _IsDefinedTemplateParameterMustExpectRule;
            public static ICompilerReferenceError InvalidDefinedTarget
            {
                get
                {
                    if (_InvalidDefinedTarget == null)
                        _InvalidDefinedTarget = new CompilerReferenceError(ParserResources.GrammarCompilerErrors_InvalidIsDefinedTarget, (int)OilexerGrammarLogicErrors.InvalidIsDefinedTarget);
                    return _InvalidDefinedTarget;
                }
            }
            private static ICompilerReferenceError _InvalidDefinedTarget;
            public static ICompilerReferenceError LanguageDefinedError
            {
                get
                {
                    if (_LanguageDefinedError == null)
                        _LanguageDefinedError = new CompilerReferenceError(ParserResources.GrammarCompilerErrors_LanguageDefinedError, (int)OilexerGrammarLogicErrors.LanguageDefinedError);
                    return _LanguageDefinedError;
                }
            }
            private static ICompilerReferenceError _LanguageDefinedError;
            public static ICompilerReferenceError UnexpectedUndefinedEntry
            {
                get
                {
                    if (_UnexpectedUndefinedEntry == null)
                        _UnexpectedUndefinedEntry = new CompilerReferenceError(ParserResources.GrammarCompilerErrors_UnexpectedUndefinedEntry, (int)OilexerGrammarLogicErrors.UnexpectedUndefinedEntry);
                    return _UnexpectedUndefinedEntry;
                }
            }
            private static ICompilerReferenceError _UnexpectedUndefinedEntry;
            public static ICompilerReferenceError DuplicateEntryError
            {
                get
                {
                    if (_DuplicateEntryError == null)
                        _DuplicateEntryError = new CompilerReferenceError(ParserResources.GrammarCompilerErrors_DuplicateEntryError, (int)OilexerGrammarLogicErrors.DuplicateEntity);
                    return _DuplicateEntryError;
                }
            }
            private static ICompilerReferenceError _DuplicateEntryError;
            public static ICompilerReferenceError DuplicateEntryReference
            {
                get
                {
                    if (_DuplicateEntryReference == null)
                        _DuplicateEntryReference = new CompilerReferenceError(ParserResources.GrammarCompilerErrors_DuplicateEntryReference, (int)OilexerGrammarLogicErrors.DuplicateReference);
                    return _DuplicateEntryReference;
                }
            }
            private static ICompilerReferenceError _DuplicateEntryReference;

            public static ICompilerReferenceError AmbiguousPathDetected
            {
                get
                {
                    if (_AmbiguousPathDetected == null)
                        _AmbiguousPathDetected = new CompilerReferenceError(ParserResources.GrammarCompilerErrors_AmbiguousPathDetected, (int)OilexerGrammarLogicErrors.AmbiguousParsePath);
                    return _AmbiguousPathDetected;
                }
            }
            private static ICompilerReferenceError _AmbiguousPathDetected;
            public static ICompilerReferenceError InvalidRuleCollapsePoint
            {
                get
                {
                    if (_InvalidRuleCollapsePoint == null)
                        _InvalidRuleCollapsePoint = new CompilerReferenceError(ParserResources.GrammarCompilerErrors_InvalidRuleCollapsePoint, (int)OilexerGrammarLogicErrors.InvalidRuleCollapsePoint);
                    return _InvalidRuleCollapsePoint;
                }
            }
            private static ICompilerReferenceError _InvalidRuleCollapsePoint;
        }

    }
}
