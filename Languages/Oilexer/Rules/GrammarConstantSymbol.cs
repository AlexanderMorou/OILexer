using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public abstract class GrammarConstantSymbol :
        GrammarTokenSymbol,
        IGrammarConstantSymbol
    {
        /// <summary>
        /// Creates a new <see cref="GrammarConstantSymbol"/>
        /// with the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="IOilexerGrammarTokenEntry"/>
        /// from which the <see cref="GrammarConstantSymbol"/> is
        /// derived.</param>
        protected GrammarConstantSymbol(IOilexerGrammarTokenEntry source)
            : base(source)
        {
        }

        //#region IGrammarConstantSymbol Members

        /// <summary>
        /// Returns the <see cref="GrammarConstantType"/> which
        /// determines the kind of constant the 
        /// <see cref="GrammarConstantSymbol"/> is.
        /// </summary>
        public abstract GrammarConstantType Type { get; }

        //#endregion
    }
}
