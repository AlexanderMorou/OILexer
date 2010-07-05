using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    public abstract class GrammarTokenSymbol :
        IGrammarTokenSymbol
    {
        /// <summary>
        /// Creates a new <see cref="GrammarTokenSymbol"/>
        /// with the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="ITokenEntry"/>
        /// from which the <see cref="GrammarTokenSymbol"/> is
        /// derived.</param>
        protected GrammarTokenSymbol(ITokenEntry source)
        {
            this.Source = source; 
        }

        #region IGrammarTokenSymbol Members

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> on which the
        /// <see cref="GrammarTokenSymbol"/> is based.
        /// </summary>
        public ITokenEntry Source { get; private set; }

        #endregion

        public override string ToString()
        {
            return this.Source.Name;
        }
    }
}
