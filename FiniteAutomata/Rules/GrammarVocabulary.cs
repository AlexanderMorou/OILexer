using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using Oilexer._Internal;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.Builder;

#if x64
using SlotType = System.UInt64;
#elif x86
using SlotType = System.UInt32;
#endif
namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Provides a grammar vocabulary bit set.
    /// </summary>
    public class GrammarVocabulary :
        FiniteAutomataBitSet<GrammarVocabulary>
    {
        private static Dictionary<ParserBuilder, GrammarVocabulary> completeSets = new Dictionary<ParserBuilder, GrammarVocabulary>();

        public static GrammarVocabulary ObtainCompleteSet(ParserBuilder builder)
        {
            if (!completeSets.ContainsKey(builder))
            {
                var newItem = new GrammarVocabulary(builder.GrammarSymbols);
                newItem.IsNegativeSet = true;
                completeSets.Add(builder, newItem);
            }
            return completeSets[builder];
        }
        private GrammarBreakdown breakdown;
        /// <summary>
        /// The symbols which determine the elements in the active
        /// language.
        /// </summary>
        internal IGrammarSymbolSet symbols;

        /// <summary>
        /// Creates a new <see cref="GrammarVocabulary"/> initialized
        /// to a default state.
        /// </summary>
        public GrammarVocabulary()
        {
        }

        /// <summary>
        /// Creates a new <see cref="GrammarVocabulary"/> with the
        /// <paramref name="symbols"/> provided.
        /// </summary>
        /// <param name="symbols">The <see cref="IGrammarSymbolSet"/>
        /// associated to the active language.</param>
        public GrammarVocabulary(IGrammarSymbolSet symbols)
        {
            this.symbols = symbols;
            if (symbols != null)
                this.FullLength = (uint)this.symbols.Count;
            NullCheck(symbols);
        }

        private void NullCheck(IGrammarSymbolSet symbols)
        {
            if (NullInst != null && NullInst.symbols == null)
            {
                NullInst.symbols = symbols;
                NullInst.FullLength = this.FullLength;
            }
        }

        /// <summary>
        /// Creates a new <see cref="GrammarVocabulary"/> with the
        /// <paramref name="symbols"/> provided.
        /// </summary>
        /// <param name="symbols">The <see cref="IGrammarSymbolSet"/>
        /// associated to the active language.</param>
        public GrammarVocabulary(IGrammarSymbolSet symbols, IGrammarSymbol element)
        {
            if (symbols == null)
                throw new ArgumentNullException("symbols");
            if (element == null)
                throw new ArgumentNullException("element");
            NullCheck(symbols);
            int index = symbols.IndexOf(element);
            BitArray values = new BitArray(1);
            values[0] = true;
            this.symbols = symbols;
            base.Set(values.ObtainFiniteSeries(symbols.Count), (uint)index, 1U, (uint)symbols.Count);
        }

        public GrammarVocabulary(IGrammarSymbolSet symbols, IGrammarSymbol[] elements)
        {
            if (symbols == null)
                throw new ArgumentNullException("symbols");
            if (elements == null)
                throw new ArgumentNullException("elements");
            NullCheck(symbols);
            int[] indices = new int[elements.Length];
            for (int i = 0; i < elements.Length; i++)
                indices[i] = symbols.IndexOf(elements[i]);
            int maxIndex = indices.Max();
            BitArray values = new BitArray(maxIndex + 1);
            for (int i = 0; i < elements.Length; i++)
                values[indices[i]] = true;
            this.symbols = symbols;
            base.Set(values.ObtainFiniteSeries(symbols.Count), 0, (uint)values.Length, (uint)symbols.Count);
        }

        /// <summary>
        /// Obtains a new <see cref="GrammarVocabulary"/> instance
        /// relative to the current symbol set for the active
        /// language.
        /// </summary>
        /// <returns>A new <see cref="GrammarVocabulary"/>
        /// initialized to its default state with the current
        /// symbol set.</returns>
        protected override GrammarVocabulary GetCheck()
        {
            if (this.symbols == null)
                return new GrammarVocabulary();
            else
                lock (this.symbols) 
                    return new GrammarVocabulary(this.symbols);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.symbols == null || this.IsEmpty)
                goto nullSet;
            var tokenItems = new Dictionary<ITokenEntry, List<IGrammarConstantItemSymbol>>();
            List<IGrammarSymbol> looseItems = new List<IGrammarSymbol>();
            IGrammarSymbol[] currentSet = GetSymbols();
            foreach (var symbol in currentSet)
            {
                if (symbol is IGrammarConstantItemSymbol)
                {
                    var constantItem = (IGrammarConstantItemSymbol)symbol;
                    if (!tokenItems.ContainsKey(constantItem.Source))
                        tokenItems.Add(constantItem.Source, new List<IGrammarConstantItemSymbol>());
                    tokenItems[constantItem.Source].Add(constantItem);
                }
                else
                    looseItems.Add(symbol);

            }
            bool first = true;
            foreach (var tokenEntry in tokenItems.Keys)
            {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(tokenEntry.Name);
                sb.Append("[");
                bool firstConstant = true;
                foreach (var constantItem in tokenItems[tokenEntry])
                {
                    if (firstConstant)
                        firstConstant = false;
                    else
                        sb.Append(", ");
                    sb.Append(constantItem.SourceItem.Value);
                }
                sb.Append("]");
            }
            foreach (var symbol in looseItems)
            {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(symbol.ToString());
            }
            nullSet:
            return sb.ToString();
        }

        internal IGrammarSymbol[] GetSymbols()
        {
            IGrammarSymbol[] currentSet = new IGrammarSymbol[this.TrueCount];
            uint length = this.IsNegativeSet ? this.FullLength : this.Offset + this.Length;
            uint offset = this.IsNegativeSet ? 0 : this.Offset;
            for (uint i = offset, symbolIndex = 0; i < length; i++)
                if (this[i])
                    currentSet[symbolIndex++] = this.symbols[(int)i];
            return currentSet;
        }

        public GrammarBreakdown Breakdown
        {
            get
            {
                if (this.breakdown == null)
                    this.breakdown = new GrammarBreakdown(this.GetSymbols());
                return this.breakdown;
            }
        }

        public GrammarVocabulary GetTokenVariant()
        {
            var tokenSymbols = (from s in this.GetSymbols()
                                where (!(s is IGrammarRuleSymbol))
                                select s).ToArray();
            if (tokenSymbols.Length == 0)
                return NullInst;
            return new GrammarVocabulary(this.symbols, tokenSymbols);
        }

        public bool Contains(IProductionRuleEntry entry)
        {
            IGrammarRuleSymbol rSymbol = (IGrammarRuleSymbol)this.symbols[entry];
            if (rSymbol == null)
                return false;
            int index = symbols.IndexOf(rSymbol);
            if (this.Offset <= index && Offset + this.Length > index)
                return this[(uint)index];
            else
                return false;
        }

        public bool Contains(ITokenEntry entry)
        {
            var tSymbols = from symbol in this.symbols
                           let tokenSymbol = symbol as IGrammarTokenSymbol
                           where tokenSymbol != null && tokenSymbol.Source == entry
                           select (uint)symbols.IndexOf(tokenSymbol);
            foreach (var tokenSymbolIndex in tSymbols)
                if (this.Offset <= tokenSymbolIndex && Offset + this.Length > tokenSymbolIndex && this[tokenSymbolIndex])
                    return true;
            return false;
        }

    }
}
