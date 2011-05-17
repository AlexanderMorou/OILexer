using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Properties;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    partial class GDFile
    {
        /// <summary>
        /// Private implementation of <see cref="IGDFileOptions"/>.
        /// </summary>
        private class MyOptions :
            IGDFileOptions
        {
            private string assemblyName;
            private string parserName;
            private string lexerName;
            private string errorPrefix;
            private string errorSuffix;
            private string rulePrefix;
            private string ruleSuffix;
            private string tokenPrefix;
            private string tokenSuffix;
            private string startEntry;
            private string nameSpace;
            /// <summary>
            /// Data member for <see cref="GrammarNameDirective"/>
            /// </summary>
            private string grammarName;

            public MyOptions()
            {
            }

            #region IGDFileOptions Members

            /// <summary>
            /// Returns/sets the name of the parser that results.
            /// </summary>
            public string ParserName
            {
                get
                {
                    if (this.parserName == null)
                        this.parserName = Resources.DefaultParserName;
                    return this.parserName;
                }
                set
                {
                    this.parserName = value;
                }
            }

            /// <summary>
            /// Returns/sets the name of the tokenizer/lexical analyzer that results.
            /// </summary>
            public string LexerName
            {
                get
                {
                    if (this.lexerName == null)
                        this.lexerName = Resources.DefaultLexerName;
                    return this.lexerName;
                }
                set
                {
                    this.lexerName = value;
                }
            }

            /// <summary>
            /// Returns/sets the error prefix.
            /// </summary>
            public string ErrorPrefix
            {
                get
                {
                    return this.errorPrefix;
                }
                set
                {
                    this.errorPrefix = value;
                }
            }

            /// <summary>
            /// Returns/sets the error suffix.
            /// </summary>
            public string ErrorSuffix
            {
                get
                {
                    return this.errorSuffix;
                }
                set
                {
                    this.errorSuffix = value;
                }
            }

            /// <summary>
            /// Returns/sets the rule prefix.
            /// </summary>
            public string RulePrefix
            {
                get
                {
                    return this.rulePrefix;
                }
                set
                {
                    this.rulePrefix = value;
                }
            }

            /// <summary>
            /// Returns/sets the rule suffix.
            /// </summary>
            public string RuleSuffix
            {
                get
                {
                    return this.ruleSuffix;
                }
                set
                {
                    this.ruleSuffix = value;
                }
            }

            /// <summary>
            /// Returns/sets the token prefix.
            /// </summary>
            public string TokenPrefix
            {
                get
                {
                    return this.tokenPrefix;
                }
                set
                {
                    this.tokenPrefix = value;
                }
            }

            /// <summary>
            /// Returns/sets the token suffix.
            /// </summary>
            public string TokenSuffix
            {
                get
                {
                    return this.tokenSuffix;
                }
                set
                {
                    this.tokenSuffix = value;
                }
            }

            /// <summary>
            /// Returns/sets the name of the grammar.
            /// </summary>
            public string GrammarName
            {
                get
                {
                    if (this.grammarName == null)
                        this.grammarName = Resources.DefaultGrammarName;
                    return this.grammarName;
                }
                set
                {
                    this.grammarName = value;
                }
            }

            public string AssemblyName
            {
                get
                {
                    if (this.assemblyName == null)
                        this.assemblyName = Resources.DefaultAssemblyName;
                    return this.assemblyName;
                }
                set
                {
                    this.assemblyName = value;
                }
            }

            public string StartEntry
            {
                get
                {
                    return this.startEntry;
                }
                set
                {
                    this.startEntry = value;
                }
            }

            public string Namespace
            {
                get { return this.nameSpace; }
                set { this.nameSpace = value; }
            }

            #endregion

            public static MyOptions operator +(MyOptions leftSide, MyOptions rightSide)
            {
                MyOptions result = new MyOptions();
                if (string.IsNullOrEmpty(leftSide.rulePrefix) &&
                    !string.IsNullOrEmpty(rightSide.rulePrefix))
                    result.rulePrefix = rightSide.RulePrefix;
                else
                    result.RulePrefix = leftSide.RulePrefix;

                if (string.IsNullOrEmpty(leftSide.ruleSuffix) &&
                    !string.IsNullOrEmpty(rightSide.ruleSuffix))
                    result.ruleSuffix = rightSide.RuleSuffix;
                else
                    result.RuleSuffix = leftSide.RuleSuffix;

                if (string.IsNullOrEmpty(leftSide.startEntry) &&
                    !string.IsNullOrEmpty(rightSide.startEntry))
                    result.startEntry = rightSide.StartEntry;
                else
                    result.startEntry = leftSide.StartEntry;

                if (string.IsNullOrEmpty(leftSide.grammarName) &&
                    !string.IsNullOrEmpty(rightSide.grammarName))
                    result.GrammarName = rightSide.GrammarName;
                else
                    result.GrammarName = leftSide.GrammarName;

                if (string.IsNullOrEmpty(leftSide.assemblyName) &&
                    !string.IsNullOrEmpty(rightSide.assemblyName))
                    result.assemblyName = rightSide.assemblyName;
                else
                    result.assemblyName = leftSide.assemblyName;

                if (string.IsNullOrEmpty(leftSide.parserName) &&
                    !string.IsNullOrEmpty(rightSide.parserName))
                    result.ParserName = rightSide.ParserName;
                else
                    result.ParserName = leftSide.ParserName;

                if (string.IsNullOrEmpty(leftSide.lexerName) &&
                    !string.IsNullOrEmpty(rightSide.lexerName))
                    result.LexerName = rightSide.LexerName;
                else
                    result.LexerName = leftSide.LexerName;

                if (string.IsNullOrEmpty(leftSide.errorPrefix) &&
                    !string.IsNullOrEmpty(rightSide.errorPrefix))
                    result.ErrorPrefix = rightSide.ErrorPrefix;
                else
                    result.ErrorPrefix = leftSide.ErrorPrefix;

                if (string.IsNullOrEmpty(leftSide.errorSuffix) &&
                    !string.IsNullOrEmpty(rightSide.errorSuffix))
                    result.ErrorSuffix = rightSide.ErrorSuffix;
                else
                    result.ErrorSuffix = leftSide.ErrorSuffix;

                if (string.IsNullOrEmpty(leftSide.tokenPrefix) &&
                    !string.IsNullOrEmpty(rightSide.tokenPrefix))
                    result.TokenPrefix = rightSide.TokenPrefix;
                else
                    result.TokenPrefix = leftSide.TokenPrefix;

                if (string.IsNullOrEmpty(leftSide.tokenSuffix) &&
                    !string.IsNullOrEmpty(rightSide.tokenSuffix))
                    result.TokenSuffix = rightSide.TokenSuffix;
                else
                    result.TokenSuffix = leftSide.TokenSuffix;
                if (string.IsNullOrEmpty(leftSide.nameSpace) &&
                    !string.IsNullOrEmpty(rightSide.nameSpace))
                    result.Namespace = rightSide.Namespace;
                else
                    result.Namespace = leftSide.Namespace;
                return result;
            }
        }
    }
}
