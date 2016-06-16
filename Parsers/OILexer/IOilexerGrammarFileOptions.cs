using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    /// <summary>
    /// Defines properties and methods for the options of a <see cref="IOilexerGrammarFile"/>.
    /// </summary>
    public interface IOilexerGrammarFileOptions
    {
        /// <summary>
        /// Returns/sets the name of the parser that results.
        /// </summary>
        string ParserName { get; set; }
        /// <summary>
        /// Returns/sets the name of the tokenizer/lexical analyzer that results.
        /// </summary>
        string LexerName { get; set; }
        /// <summary>
        /// Returns/sets the error prefix.
        /// </summary>
        string ErrorPrefix { get; set; }
        /// <summary>
        /// Returns/sets the error suffix.
        /// </summary>
        string ErrorSuffix { get; set; }
        /// <summary>
        /// Returns/sets the rule prefix.
        /// </summary>
        string RulePrefix { get; set; }
        /// <summary>
        /// Returns/sets the rule suffix.
        /// </summary>
        string RuleSuffix { get; set; }
        /// <summary>
        /// Returns/sets the token prefix.
        /// </summary>
        string TokenPrefix { get; set; }
        /// <summary>
        /// Returns/sets the token suffix.
        /// </summary>
        string TokenSuffix { get; set; }
        /// <summary>
        /// Returns/sets the name of the grammar.
        /// </summary>
        string GrammarName { get; set; }
        /// <summary>
        /// Returns/sets the name of the assembly that results.
        /// </summary>
        string AssemblyName { get; set; }
        /// <summary>
        /// Returns/sets the name of the entry that starts the parse.
        /// </summary>
        string StartEntry { get; set; }
        /// <summary>
        /// Returns/sets the default namespace associated to the project.
        /// </summary>
        string Namespace { get; set; }
    }
}
