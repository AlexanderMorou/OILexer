using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    public abstract class GrammarConstantSymbol :
        GrammarTokenSymbol,
        IGrammarConstantSymbol
    {
        /// <summary>
        /// Creates a new <see cref="GrammarConstantSymbol"/>
        /// with the <paramref name="source"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="ITokenEntry"/>
        /// from which the <see cref="GrammarConstantSymbol"/> is
        /// derived.</param>
        protected GrammarConstantSymbol(ITokenEntry source)
            : base(source)
        {
        }

        #region IGrammarConstantSymbol Members

        /// <summary>
        /// Returns the <see cref="GrammarConstantType"/> which
        /// determines the kind of constant the 
        /// <see cref="GrammarConstantSymbol"/> is.
        /// </summary>
        public abstract GrammarConstantType Type { get; }

        #endregion
    }
}
