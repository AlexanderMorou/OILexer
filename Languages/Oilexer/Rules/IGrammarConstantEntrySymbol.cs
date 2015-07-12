using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a constant
    /// grammar token entry which has exactly one unnamed element
    /// only.
    /// </summary>
    /// <remarks>
    /// <code language="OILexer">
    /// Token :=
    ///     @"constant"; //@ means case insensitive.
    /// </code>
    /// </remarks>
    public interface IGrammarConstantEntrySymbol :
        IGrammarConstantSymbol
    {
    }
}
