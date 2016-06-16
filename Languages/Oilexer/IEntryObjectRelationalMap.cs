using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Ast;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    using RuleTree = KeyedTree<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using RuleTreeNode = KeyedTreeNode<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using AllenCopeland.Abstraction.Slf.Ast.Members;
    public interface IEntryObjectRelationalMap
    {
        IControlledCollection<IOilexerGrammarScannableEntry> Implements { get; }
        /// <summary>
        /// Obtains an enumerable state machine which yields the sequences of 
        /// variations of which the <see cref="IEntryObjectRelationalMap"/>'s
        /// <see cref="OilexerGrammarEntry"/> can be.
        /// </summary>
        IEnumerable<IEnumerable<IOilexerGrammarScannableEntry>> Variations { get; }
        /// <summary>
        /// The <see cref="IOilexerGrammarScannableEntry"/> which is represented by the <see cref="IEntryObjectRelationalMap"/>.
        /// </summary>
        IOilexerGrammarScannableEntry Entry { get; }
        RuleTreeNode ImplementationDetails { get; }
        /// <summary>
        /// Represents the points of reference where the child entry is 
        /// valid for the production rules with 'IsRuleCollapsePoint' marked
        /// as true.
        /// </summary>
        IControlledDictionary<IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember> CaseFields { get; }
    }

}
