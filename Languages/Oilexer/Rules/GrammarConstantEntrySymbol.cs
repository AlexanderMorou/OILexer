using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides an implementation of a grammar constant symbol.
    /// </summary>
    public class GrammarConstantEntrySymbol :
        GrammarConstantSymbol,
        IGrammarConstantEntrySymbol
    {
        /// <summary>
        /// Creates a new <see cref="GrammarConstantEntrySymbol"/>
        /// with the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="ITokenEntry"/>
        /// from which the <see cref="GrammarConstantEntrySymbol"/>
        /// is derived.</param>
        public GrammarConstantEntrySymbol(ITokenEntry source)
            : base(source)
        {
        }

        /// <summary>
        /// Returns the <see cref="GrammarConstantType"/> which
        /// determines the kind of constant the 
        /// <see cref="GrammarConstantEntrySymbol"/> is.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="GrammarConstantType.Entry"/>.
        /// </remarks>
        public override GrammarConstantType Type
        {
            get { return GrammarConstantType.Entry; }
        }
    }
}
