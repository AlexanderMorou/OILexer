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
    public abstract class GrammarTokenSymbol :
        IGrammarTokenSymbol
    {
        /// <summary>
        /// Creates a new <see cref="GrammarTokenSymbol"/>
        /// with the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="IOilexerGrammarTokenEntry"/>
        /// from which the <see cref="GrammarTokenSymbol"/> is
        /// derived.</param>
        protected GrammarTokenSymbol(IOilexerGrammarTokenEntry source)
        {
            this.Source = source; 
        }

        //#region IGrammarTokenSymbol Members

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> on which the
        /// <see cref="GrammarTokenSymbol"/> is based.
        /// </summary>
        public IOilexerGrammarTokenEntry Source { get; private set; }

        //#endregion

        public override string ToString()
        {
            return this.Source.Name;
        }

        //#region IGrammarSymbol Members

        public virtual string ElementName
        {
            get { return this.Source.Name; }
        }

        //#endregion
    }
}
