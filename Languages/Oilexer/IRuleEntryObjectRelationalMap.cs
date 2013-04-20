using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IRuleEntryObjectRelationalMap :
        IEntryObjectRelationalMap
    {
        new IControlledCollection<IProductionRuleEntry> Implements { get; }
        /// <summary>
        /// Obtains an enumerable state machine which yields the sequences of 
        /// variations of which the <see cref="IRuleEntryObjectRelationalMap"/>'s
        /// <see cref="Entry"/> can be.
        /// </summary>
        new IEnumerable<IEnumerable<IProductionRuleEntry>> Variations { get; }
        /// <summary>
        /// The <see cref="IProductionRuleEntry"/> which is represented by the <see cref="IRuleEntryObjectRelationalMap"/>.
        /// </summary>
        new IProductionRuleEntry Entry { get; }
    }

    public interface IRuleEntryChildObjectRelationalMap
    {
        /// <summary>
        /// Represents the points of reference where the child entry is 
        /// valid for the production rules with 'ElementsAreChildren' marked
        /// as true.
        /// </summary>
        IControlledDictionary<IProductionRuleEntry, IIntermediateEnumFieldMember> CaseFields { get; }
    }

    public interface IRuleEntryBranchObjectRelationalMap : 
        IRuleEntryObjectRelationalMap
    {
        /// <summary>
        /// Returns the <see cref="IIntermediateEnumType"/> which denotes the 
        /// various rules which are valid for this entry.
        /// </summary>
        IIntermediateEnumType CasesEnum { get; }
    }
}
