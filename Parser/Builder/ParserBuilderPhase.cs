using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.Builder
{
    /// <summary>
    /// The current phase the parser builder is in.
    /// </summary>
    public enum ParserBuilderPhase
    {
        /// <summary>
        /// The parser builder hasn't started the process yet.
        /// </summary>
        None,
        /// <summary>
        /// The parser builder is linking the internal
        /// structure of the grammar description.
        /// </summary>
        Linking = 0x010,
        /// <summary>
        /// The parser builder is expanding template references.
        /// </summary>
        ExpandingTemplates = 0x0303,
        /// <summary>
        /// The parser builder is referencing the literals from the
        /// rule definitions into enumerator or literal token elements
        /// that match the literals.
        /// </summary>
        LiteralLookup = 0x1011,
        /// <summary>
        /// The parser builder is inlining the tokens to reduce 
        /// state machine dependency.
        /// </summary>
        InliningTokens = 0x2021,
        /// <summary>
        /// The parser builder is building a nondeterministic view
        /// of the each token's state machine.
        /// </summary>
        TokenNFAConstruction = 0x0121,
        /// <summary>
        /// The parser builder is creating a left-sided union on the
        /// nondeterministic view of each token's state machine to
        /// produce a deterministic model.
        /// </summary>
        TokenDFAConstruction = 0x91,
        /// <summary>
        /// The parser builder is reducing the deterministic
        /// automations of the grammar description.
        /// </summary>
        TokenDFAReduction = 0x555,
        /// <summary>
        /// The parser builder is building a nondeterministic
        /// view of each rule's internal structure.
        /// </summary>
        RuleNFAConstruction = 0x1,
        /// <summary>
        /// The parser builder is creating a left-sided union on the
        /// nondeterministic view of each rule's state machine to
        /// produce a deterministic model.
        /// </summary>
        RuleDFAConstruction = 0x2122,
        /// <summary>
        /// The Parser builder is analyzing the call tree of the
        /// language and constructing a larger deterministic state 
        /// machine of the entire language.
        /// </summary>
        CallTreeAnalysis = 0x91111,
        /// <summary>
        /// The parser builder is constructing the initial object
        /// set for the next phase.
        /// </summary>
        ObjectModelRootTypesConstruction = 0x67,
        /// <summary>
        /// The parser builder is constructing the objectified view
        /// of each individual token's data structure for the capture
        /// styled tokens.
        /// </summary>
        ObjectModelTokenCaptureConstruction = 0x36,
        /// <summary>
        /// The parser builder is constructing the objectified view
        /// of each individual categorized token's enumerator.
        /// </summary>
        ObjectModelTokenEnumConstruction = 0x48,
        /// <summary>
        /// The parser builder is constructing an objectified view of
        /// each rule's structure.
        /// </summary>
        ObjectModelRuleStructureConstruction = 0x815,
        /// <summary>
        /// The parser builder is finalizing the objectified 
        /// code model build process.
        /// </summary>
        ObjectModelFinalTypesConstruction = 0x3839,
        /// <summary>
        /// The parser builder is writing the source files associated
        /// to the objectified view.
        /// </summary>
        WritingFiles = 0x04567,
        /// <summary>
        /// The parser builder is compiling the project.
        /// </summary>
        Compiling = 0x101001,
        Parsing =   0x102002,
    }
}
