using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Properties;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    partial class OilexerGrammarCore
    {
        internal class CompilerWarnings
        {
            /// <summary>
            /// Reference compiler warning message for when a rule's series of ternimals and
            /// non-terminals matches identically.
            /// </summary>
            public static ICompilerReferenceWarning DuplicateDefinition
            {
                get
                {
                    if (_DuplicateDefinition == null)
                        _DuplicateDefinition = new CompilerReferenceWarning(ParserResources.GrammarCompilerWarnings_DuplicateDefinition, 2, (int)OilexerGrammarLogicWarnings.DuplicateEntryByDefinition);
                    return _DuplicateDefinition;
                }
            }

            /// <summary>
            /// Reference compiler warning message for when two or more terminals within the grammar overlap
            /// and they are observed within the analysis of the grammar.
            /// </summary>
            public static ICompilerReferenceWarning LexicalAmbiguity
            {
                get
                {
                    if (_LexicalAmbiguity == null)
                        _LexicalAmbiguity = new CompilerReferenceWarning(ParserResources.GrammarCompilerWarnings_LexicalAmbiguity, 1, (int)OilexerGrammarLogicWarnings.LexicalAmbiguity);
                    return _LexicalAmbiguity;
                }
            }

            /// <summary>
            /// Reference compiler warning message for when two or more terminals within the grammar overlap
            /// and they are not observed within the analysis of the grammar.
            /// </summary>
            public static ICompilerReferenceWarning UnobservedLexicalAmbiguity
            {
                get
                {
                    if (_UnobservedLexicalAmbiguity == null)
                        _UnobservedLexicalAmbiguity = new CompilerReferenceWarning(ParserResources.GrammarCompilerWarnings_UnobservedLexicalAmbiguity, 2, (int)OilexerGrammarLogicWarnings.UnobservedLexicalAmbiguity);
                    return _UnobservedLexicalAmbiguity;
                }
            }

            private static ICompilerReferenceWarning _DuplicateDefinition;
            private static ICompilerReferenceWarning _LexicalAmbiguity;
            private static ICompilerReferenceWarning _UnobservedLexicalAmbiguity;
        }
    }
}
