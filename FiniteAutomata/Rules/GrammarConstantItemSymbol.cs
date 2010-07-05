using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Provides an implementation of a grammar constant symbol as a
    /// sub-element of token.
    /// </summary>
    public class GrammarConstantItemSymbol :
        GrammarConstantSymbol,
        IGrammarConstantItemSymbol
    {
        /// <summary>
        /// Creates a new <see cref="GrammarConstantItemSymbol"/>
        /// with the <paramref name="source"/> and
        /// <paramref name="sourceItem"/> provided.
        /// </summary>
        /// <param name="source">The <see cref="ITokenEntry"/> which
        /// contains the <paramref name="sourceItem"/> from which the
        /// current <see cref="GrammarConstantItemSymbol"/> is
        /// derived.</param>
        /// <param name="sourceItem">The 
        /// <see cref="ILiteralTokenItem"/> from within the
        /// <paramref name="source"/> entry from which the
        /// <see cref="GrammarConstantItemSymbol"/> is derived.
        /// </param>
        public GrammarConstantItemSymbol(ITokenEntry source, ILiteralTokenItem sourceItem)
            : base(source)
        {
            this.SourceItem = sourceItem;
        }

        /// <summary>
        /// Returns the <see cref="GrammarConstantType"/> which
        /// determines the kind of constant the 
        /// <see cref="GrammarConstantItemSymbol"/> is.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="GrammarConstantType.Item"/>.</remarks>
        public override GrammarConstantType Type
        {
            get { return GrammarConstantType.Item; }
        }

        #region IGrammarConstantItemSymbol Members

        public ILiteralTokenItem SourceItem { get; private set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} ({1})", base.ToString(), this.SourceItem.Name);
        }
    }
}
