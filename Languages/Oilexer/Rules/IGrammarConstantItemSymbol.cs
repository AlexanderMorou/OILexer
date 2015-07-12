using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a grammar
    /// constant which is defined as an element of a token category.
    /// </summary>
    public interface IGrammarConstantItemSymbol :
        IGrammarConstantSymbol
    {
        /// <summary>
        /// Returns the <see cref="ILiteralTokenItem"/> on which the
        /// <see cref="IGrammarConstantItemSymbol"/> is based.
        /// </summary>
        ILiteralTokenItem SourceItem { get; }
    }
}
