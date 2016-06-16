using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Provides a set of results for the build phase of the Grammar Description
    /// builder.
    /// </summary>
    public class ParserCompilerResults
    {
        /// <summary>
        /// Returns the <see cref="IIntermediateAssembly"/> which results
        /// from the build.
        /// </summary>
        public IIntermediateAssembly Project { get; internal set; }

        /// <summary>
        /// Returns the readonly dictionary of time spans for each
        /// phase in the build.
        /// </summary>
        public ControlledDictionary<ParserCompilerPhase, TimeSpan> PhaseTimes { get; internal set; }
        public ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> RuleStateMachines { get; internal set; }
        public ICompilerErrorCollection CompilationErrors { get; internal set; }
        public GrammarVocabulary FullGrammar { get; internal set; }
        public IGrammarSymbolSet Symbols { get; internal set; }

        public ParserCompiler Builder { get; internal set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleEntry"/> which denotes the start
        /// of the language.
        /// </summary>
        public IOilexerGrammarProductionRuleEntry StartEntry { get; internal set; }
    }
}
