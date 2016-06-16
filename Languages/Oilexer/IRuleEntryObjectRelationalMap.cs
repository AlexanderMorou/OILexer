using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IRuleEntryObjectRelationalMap :
        IEntryObjectRelationalMap
    {
        new IControlledCollection<IOilexerGrammarScannableEntry> Implements { get; }
        /// <summary>
        /// The <see cref="IOilexerGrammarProductionRuleEntry"/> which is represented by the <see cref="IRuleEntryObjectRelationalMap"/>.
        /// </summary>
        new IOilexerGrammarProductionRuleEntry Entry { get; }

        IIntermediateClassType DebuggerProxy { get; }
    }

    public interface IRuleEntryChildObjectRelationalMap
    {
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
