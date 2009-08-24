using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Properties;
using Oilexer._Internal;
namespace Oilexer.Parser
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

            #endregion

            public static MyOptions operator +(MyOptions leftSide, MyOptions rightSide)
            {
                MyOptions result = new MyOptions();
                if (leftSide.rulePrefix == null || leftSide.rulePrefix == string.Empty &&
                    rightSide.rulePrefix != null && rightSide.rulePrefix != string.Empty)
                    result.rulePrefix = rightSide.RulePrefix;
                else
                    result.RulePrefix = leftSide.RulePrefix;

                if (leftSide.ruleSuffix == null || leftSide.ruleSuffix == string.Empty &&
                    rightSide.ruleSuffix != null && rightSide.ruleSuffix != string.Empty)
                    result.ruleSuffix = rightSide.RuleSuffix;
                else
                    result.RuleSuffix = leftSide.RuleSuffix;

                if (leftSide.startEntry == null || leftSide.startEntry == string.Empty &&
                    rightSide.startEntry != null && rightSide.startEntry != string.Empty)
                    result.startEntry = rightSide.StartEntry;
                else
                    result.startEntry = leftSide.StartEntry;

                if (leftSide.grammarName == null || leftSide.grammarName == string.Empty &&
                    rightSide.grammarName != null && rightSide.grammarName != string.Empty)
                    result.GrammarName = rightSide.GrammarName;
                else
                    result.GrammarName = leftSide.GrammarName;

                if (leftSide.assemblyName == null || leftSide.assemblyName == string.Empty &&
                    rightSide.assemblyName != null && rightSide.assemblyName != string.Empty)
                    result.assemblyName = rightSide.assemblyName;
                else
                    result.assemblyName = leftSide.assemblyName;

                if (leftSide.parserName == null || leftSide.parserName == string.Empty &&
                    rightSide.parserName != null && rightSide.parserName != string.Empty)
                    result.ParserName = rightSide.ParserName;
                else
                    result.ParserName = leftSide.ParserName;

                if (leftSide.lexerName == null || leftSide.lexerName == string.Empty &&
                    rightSide.lexerName != null && rightSide.lexerName != string.Empty)
                    result.LexerName = rightSide.LexerName;
                else
                    result.LexerName = leftSide.LexerName;

                if (leftSide.errorPrefix == null &&
                    rightSide.errorPrefix != null && rightSide.errorPrefix != string.Empty)
                    result.ErrorPrefix = rightSide.ErrorPrefix;
                else
                    result.ErrorPrefix = leftSide.ErrorPrefix;

                if (leftSide.errorSuffix == null &&
                    rightSide.errorSuffix != null && rightSide.errorSuffix != string.Empty)
                    result.ErrorSuffix = rightSide.ErrorSuffix;
                else
                    result.ErrorSuffix = leftSide.ErrorSuffix;

                if (leftSide.tokenPrefix == null &&
                    rightSide.tokenPrefix != null && rightSide.tokenPrefix != string.Empty)
                    result.TokenPrefix = rightSide.TokenPrefix;
                else
                    result.TokenPrefix = leftSide.TokenPrefix;

                if (leftSide.tokenSuffix == null &&
                    rightSide.tokenSuffix != null && rightSide.tokenSuffix != string.Empty)
                    result.TokenSuffix = rightSide.TokenSuffix;
                else
                    result.TokenSuffix = leftSide.TokenSuffix;
                return result;
            }
        }
    }
}
