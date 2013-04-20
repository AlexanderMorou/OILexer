using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Ast.Members;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IGDFileObjectRelationalMap :
        IControlledDictionary<IScannableEntry, IEntryObjectRelationalMap>
    {
        /// <summary>
        /// Returns the <see cref="IGDFile"/> from which the object relational 
        /// map is derived.
        /// </summary>
        IGDFile Source { get; }

        IMultikeyedDictionary<IProductionRuleEntry, IProductionRuleEntry, IIntermediateEnumFieldMember> CasesLookup { get; }
    }
}
