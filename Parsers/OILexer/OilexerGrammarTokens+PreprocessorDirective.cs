using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    partial class OilexerGrammarTokens
    {
        public enum PreprocessorType
        {
            /// <summary>
            /// Includes a file, #include "file";
            /// </summary>
            IncludeDirective,
            /// <summary>
            /// If conditional directive, #if condition.
            /// </summary>
            IfDirective,
            /// <summary>
            /// If conditional directive valid within rules and templates.
            /// #ifin condition.
            /// </summary>
            IfInDirective,
            /// <summary>
            /// If defined conditional directive, #ifdef.
            /// </summary>
            IfDefinedDirective,
            /// <summary>
            /// If not defined conditional directive, #ifndef.
            /// </summary>
            IfNotDefinedDirective,
            /// <summary>
            /// Else if conditional directive, #elif
            /// </summary>
            ElseIfDirective,
            /// <summary>
            /// Else if in conditional directive.
            /// #elifin targets.
            /// </summary>
            ElseIfInDirective,
            /// <summary>
            /// Else if defined conditional directive, #elifdef
            /// </summary>
            ElseIfDefinedDirective,
            /// <summary>
            /// Else directive, #else
            /// </summary>
            ElseDirective,
            /// <summary>
            /// End if series, #endif
            /// </summary>
            EndIfDirective,
            /// <summary>
            /// Define directive, defines a rule, #define name, expression(s).
            /// </summary>
            DefineDirective,
            /// <summary>
            /// Adds a leaf to a rule, #addrule expression series.
            /// </summary>
            AddRuleDirective,
            /// <summary>
            /// Throws a preprocessing error, #throw.
            /// </summary>
            ThrowDirective,
            /// <summary>
            /// Returns an expression series.
            /// </summary>
            ReturnDirective,
            LexerNameDirective,
            ParserNameDirective,
            GrammarNameDirective,
            AssemblyNameDirective,
            RootDirective,
            RulePrefixDirective,
            RuleSuffixDirective,
            TokenPrefixDirective,
            TokenSuffixDirective,
            NamespaceDirective,
        }
        public class PreprocessorDirective :
            OilexerGrammarToken
        {
            PreprocessorType type;

            public PreprocessorDirective(PreprocessorType type, int line, int column, long position)
                : base(column, line, position)
            {
                this.type = type;
            }
            public override OilexerGrammarTokenType TokenType
            {
                get { return OilexerGrammarTokenType.PreprocessorDirective; }
            }

            public override int Length
            {
                get
                {
                    switch (type)
                    {
                        case PreprocessorType.IfDirective:                //#if
                            return 3;
                        case PreprocessorType.RootDirective:              //#Root
                        case PreprocessorType.IfInDirective:              //#ifin
                        case PreprocessorType.ElseIfDirective:            //#elif
                        case PreprocessorType.ElseDirective:              //#else
                            return 5;
                        case PreprocessorType.IfDefinedDirective:         //#ifdef
                        case PreprocessorType.ThrowDirective:             //#throw
                        case PreprocessorType.EndIfDirective:             //#endif
                            return 6;
                        case PreprocessorType.ElseIfInDirective:          //#elifin
                        case PreprocessorType.IfNotDefinedDirective:      //#ifndef
                        case PreprocessorType.DefineDirective:            //#define
                        case PreprocessorType.ReturnDirective:            //#return
                            return 7;
                        case PreprocessorType.AddRuleDirective:           //#addrule
                        case PreprocessorType.IncludeDirective:           //#include
                        case PreprocessorType.ElseIfDefinedDirective:     //#elifdef
                            return 8;
                        case PreprocessorType.NamespaceDirective:         //#Namespace
                        case PreprocessorType.LexerNameDirective:         //#LexerName
                            return 10;
                        case PreprocessorType.RulePrefixDirective:        //#RulePrefix
                        case PreprocessorType.RuleSuffixDirective:        //#RuleSuffix
                        case PreprocessorType.ParserNameDirective:        //#ParserName
                            return 11;
                        case PreprocessorType.GrammarNameDirective:       //#GrammarName
                        case PreprocessorType.TokenPrefixDirective:       //#TokenPrefix
                        case PreprocessorType.TokenSuffixDirective:       //#TokenSuffix
                            return 12;
                        case PreprocessorType.AssemblyNameDirective:      //#AssemblyName
                            return 13;
                        default:
                            return 0;
                    }
                }
            }

            public override bool ConsumedFeed
            {
                get { return false; }
            }

            public PreprocessorType Type
            {
                get
                {
                    return this.type;
                }
            }
            public override string ToString()
            {
                switch (this.Type)
                {
                    case PreprocessorType.IncludeDirective:
                        return "#include";
                    case PreprocessorType.IfDirective:
                        return "#if";
                    case PreprocessorType.IfInDirective:
                        return "#ifin";
                    case PreprocessorType.IfDefinedDirective:
                        return "#ifdef";
                    case PreprocessorType.IfNotDefinedDirective:
                        return "#ifndef";
                    case PreprocessorType.RootDirective:
                        return "#RootDirective";
                    case PreprocessorType.ElseIfDirective:
                        return "#elif";
                    case PreprocessorType.ElseIfDefinedDirective:
                        return "#elifdef";
                    case PreprocessorType.ElseDirective:
                        return "#else";
                    case PreprocessorType.ElseIfInDirective:
                        return "#elifin";
                    case PreprocessorType.EndIfDirective:
                        return "#endif";
                    case PreprocessorType.DefineDirective:
                        return "#define";
                    case PreprocessorType.ReturnDirective:
                        return "#return";
                    case PreprocessorType.AddRuleDirective:
                        return "#addrule";
                    case PreprocessorType.ThrowDirective:
                        return "#throw";
                    case PreprocessorType.LexerNameDirective:
                        return "#LexerNameDirective";
                    case PreprocessorType.ParserNameDirective:
                        return "#ParserNameDirective";
                    case PreprocessorType.GrammarNameDirective:
                        return "#GrammarNameDirective";
                    case PreprocessorType.AssemblyNameDirective:
                        return "#AssemblyNameDirective";
                    case PreprocessorType.NamespaceDirective:
                        return "#Namespace";
                    default:
                        return "/#invalid#/";
                }
            }
        }
    }
}
