using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    using RuleTree = KeyedTree<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using RuleTreeNode = KeyedTreeNode<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using AllenCopeland.Abstraction.Slf.Ast.Cli;
    public interface IOilexerGrammarFileObjectRelationalMap :
        IControlledDictionary<IOilexerGrammarScannableEntry, IEntryObjectRelationalMap>
    {
        /// <summary>
        /// Returns the <see cref="IOilexerGrammarFile"/> from which the object relational 
        /// map is derived.
        /// </summary>
        IOilexerGrammarFile Source { get; }

        IMultikeyedDictionary<IOilexerGrammarProductionRuleEntry, IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember> CasesLookup { get; }

        void ConnectConstructs(IControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> ruleStates);

        void BuildPrimaryMembers(IIntermediateCliManager identityManager);

        RuleTree ImplementationDetails { get; }
    }
}
