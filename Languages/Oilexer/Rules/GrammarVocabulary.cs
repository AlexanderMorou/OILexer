using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Ast;

#if x64
using SlotType = System.UInt64;
#elif x86
#if HalfWord
using SlotType = System.UInt16;
#else
using SlotType = System.UInt32;
#endif
#endif

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a grammar vocabulary bit set.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public class GrammarVocabulary :
        FiniteAutomataBitSet<GrammarVocabulary>
    {
        private static Dictionary<ParserCompiler, GrammarVocabulary> completeSets = new Dictionary<ParserCompiler, GrammarVocabulary>();
        private static long instanceCount = 0;
#if DEBUG
        /* *
         * See comments in ToString() for logic
         * behind caching only in DEBUG.
         * */
        private string stringForm;
#endif
        public static GrammarVocabulary ObtainCompleteSet(ParserCompiler compiler)
        {
            if (!completeSets.ContainsKey(compiler))
            {
                var newItem = new GrammarVocabulary(compiler.GrammarSymbols);
                newItem.IsNegativeSet = true;
                completeSets.Add(compiler, newItem);
            }
            return completeSets[compiler];
        }

        internal override uint OnGetFullLength()
        {
            if (this.symbols != null)
                return (uint)this.symbols.Count;
            else
                return 0;
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
            instanceCount++;
        }

        /// <summary>
        /// Creates a new <see cref="GrammarVocabulary"/> with the
        /// <paramref name="symbols"/> provided.
        /// </summary>
        /// <param name="symbols">The <see cref="IGrammarSymbolSet"/>
        /// associated to the active language.</param>
        public GrammarVocabulary(IGrammarSymbolSet symbols)
            : this()
        {
            this.symbols = symbols;
            if (symbols != null)
            NullCheck(symbols);
        }

        private void NullCheck(IGrammarSymbolSet symbols)
        {
            if (NullInst != null && NullInst.symbols == null)
                NullInst.symbols = symbols;
        }

        /// <summary>
        /// Creates a new <see cref="GrammarVocabulary"/> with the
        /// <paramref name="symbols"/> provided.
        /// </summary>
        /// <param name="symbols">The <see cref="IGrammarSymbolSet"/>
        /// associated to the active language.</param>
        public GrammarVocabulary(IGrammarSymbolSet symbols, IGrammarSymbol element)
            : this()
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
            base.Set(values.ObtainFiniteSeries(symbols.Count), (uint)index, 1U);
        }

        public GrammarVocabulary(IGrammarSymbolSet symbols, IGrammarSymbol[] elements)
            : this()
        {
            if (symbols == null)
                throw new ArgumentNullException("symbols");
            if (elements == null)
                throw new ArgumentNullException("elements");
            NullCheck(symbols);
            int[] indices = new int[elements.Length];
            for (int i = 0; i < elements.Length; i++)
                indices[i] = symbols.IndexOf(elements[i]);
            int maxIndex = indices.Length == 0 ? 0 : indices.Max();
            BitArray values = new BitArray(maxIndex + 1);
            for (int i = 0; i < elements.Length; i++)
                values[indices[i]] = true;
            this.symbols = symbols;
            base.Set(values.ObtainFiniteSeries(symbols.Count), 0, (uint)values.Length);
        }
        
        /// <summary>
        /// Obtains a new <see cref="GrammarVocabulary"/> instance
        /// relative to the current symbol set for the active
        /// language.
        /// </summary>
        /// <returns>A new <see cref="GrammarVocabulary"/>
        /// initialized to its default state with the current
        /// symbol set.</returns>
        protected override sealed GrammarVocabulary GetCheck()
        {
            if (this.symbols == null)
                return new GrammarVocabulary();
            else
                lock (this.symbols)
                    return new GrammarVocabulary(this.symbols);
        }

        public override string ToString()
        {
#if DEBUG
            /* *
             * In debug mode, we'll probably be looking at these sets quite a bit. So it's nice to cache them for performant debugging.
             * *
             * Just set the stringForm to null if you need it to recalculate the structure.  This shouldn't be necessary because Reduce is the only method that 
             * operates on the active instance, all other operations are immutable and yield a new instance.
             * */
            return this.stringForm ?? (this.stringForm = this.FormBody());
#else
            return this.FormBody();
#endif
        }

        private string FormBody()
        {
            if (this.symbols == null || this.IsEmpty)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            IGrammarSymbol[] currentSet = this.GetSymbols();
            Dictionary<IOilexerGrammarTokenEntry, List<IGrammarConstantItemSymbol>> tokenItems;
            List<IGrammarSymbol> looseItems;
            GetGroupsAndLoose(currentSet, out tokenItems, out looseItems);
            BuildLooseAndGroups(sb, tokenItems, looseItems, true);
            return sb.ToString();
        }

        private static void BuildLooseAndGroups(StringBuilder sb, Dictionary<IOilexerGrammarTokenEntry, List<IGrammarConstantItemSymbol>> tokenItems, List<IGrammarSymbol> looseItems, bool first)
        {
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
        }

        private static void GetGroupsAndLoose(IGrammarSymbol[] currentSet, out Dictionary<IOilexerGrammarTokenEntry, List<IGrammarConstantItemSymbol>> tokenItems, out List<IGrammarSymbol> looseItems/*, bool ambiguous = false*/)
        {
            tokenItems = new Dictionary<IOilexerGrammarTokenEntry, List<IGrammarConstantItemSymbol>>();
            looseItems = new List<IGrammarSymbol>();
            foreach (var symbol in currentSet)
            {
                //if (!ambiguous && symbol is IGrammarAmbiguousSymbol)
                //    continue;
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

        /// <summary>
        /// Obtains the <see cref="GrammarBreakdown"/> of the current <see cref="GrammarVocabulary"/>.
        /// </summary>
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

        public GrammarVocabulary GetRuleVariant()
        {
            var ruleSymbols = (from s in this.GetSymbols()
                               where (s is IGrammarRuleSymbol)
                               select s).ToArray();
            if (ruleSymbols.Length == 0)
                return NullInst;
            return new GrammarVocabulary(this.symbols, ruleSymbols);
        }

        public bool Contains(IOilexerGrammarProductionRuleEntry entry)
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

        public bool Contains(IOilexerGrammarTokenEntry entry)
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

        public GrammarVocabulary DisambiguateVocabulary()
        {
            var symbols = this.GetSymbols();
            var ambiguousSymbolSet = symbols.Where(k => k is IGrammarAmbiguousSymbol).ToArray();
            symbols = symbols.Except(ambiguousSymbolSet).ToArray();
            var demystifiedAmbiguousSymbols =
                (from ambig in ambiguousSymbolSet.Cast<IGrammarAmbiguousSymbol>()
                 from symbol in ambig.AmbiguityKey.GetSymbols()
                 select symbol).ToArray();
            symbols = symbols.Concat(demystifiedAmbiguousSymbols).Distinct().ToArray();
            return new GrammarVocabulary(this.symbols, symbols);
        }

        internal GrammarVocabularySymbolicBreakdown SymbolicBreakdown(ParserCompiler compiler)
        {
            var symbols = this.GetSymbols();

            return new GrammarVocabularySymbolicBreakdown()
            {
                Tokens = new GrammarVocabularyTokenBreakdown(
                    symbols.Where(s => s is IGrammarTokenSymbol)
                    .Cast<IGrammarTokenSymbol>()
                    .Select(token => compiler.TokenSymbolDetail[token])
                    .ToDictionary(t => t.Symbol, t => t)),
                Rules = new GrammarVocabularyRuleBreakdown(
                    symbols.Where(s => s is IGrammarRuleSymbol)
                    .Cast<IGrammarRuleSymbol>()
                    .Select(rule => compiler.RuleDetail[rule.Source])
                    .ToDictionary(k => k.Symbol, v => v)),
                Ambiguities =
                    new GrammarVocabularyAmbiguityBreakdown(
                        symbols.Where(s => s is IGrammarAmbiguousSymbol)
                        .Cast<IGrammarAmbiguousSymbol>()
                        .Select(ambiguity => compiler.AmbiguityDetail[ambiguity])
                        .ToDictionary(k => k.Symbol, v => v)),
            };
        }
    }

    internal class GrammarVocabularySymbolicBreakdown
    {
        public GrammarVocabularyTokenBreakdown Tokens { get; set; }
        public GrammarVocabularyRuleBreakdown Rules { get; set; }
        public GrammarVocabularyAmbiguityBreakdown Ambiguities { get; set; }
    }

    internal class GrammarVocabularyAmbiguityBreakdown :
        Dictionary<IGrammarAmbiguousSymbol, GrammarVocabularyAmbiguitySymbolBreakdown>
    {
        public GrammarVocabularyAmbiguityBreakdown(IDictionary<IGrammarAmbiguousSymbol, GrammarVocabularyAmbiguitySymbolBreakdown> orig)
            : base(orig)
        {

        }
    }

    internal class GrammarVocabularyAmbiguitySymbolBreakdown
    {
        public IGrammarAmbiguousSymbol Symbol { get; set; }
        public IIntermediateEnumFieldMember Identity { get; set; }
        public IIntermediateEnumFieldMember ValidIdentity { get; set; }
        public IIntermediateClassPropertyMember AmbiguityKeyReference { get; set; }
        public IIntermediateClassFieldMember AmbiguityReference { get; set; }
    }

    internal class GrammarVocabularyTokenBreakdown :
        Dictionary<IGrammarTokenSymbol, GrammarVocabularyTokenSymbolBreakdown>
    {
        public GrammarVocabularyTokenBreakdown(IDictionary<IGrammarTokenSymbol, GrammarVocabularyTokenSymbolBreakdown> orig)
            : base(orig)
        {

        }
    }
    internal class GrammarVocabularyRuleBreakdown :
        Dictionary<IGrammarRuleSymbol, GrammarVocabularyRuleDetail>
    {
        public GrammarVocabularyRuleBreakdown(IDictionary<IGrammarRuleSymbol, GrammarVocabularyRuleDetail> orig)
            : base(orig)
        {
        }
    }

    internal class GrammarVocabularyTokenSymbolBreakdown
    {
        public IGrammarTokenSymbol Symbol { get; set; }
        public IIntermediateEnumFieldMember Identity { get; set; }
        public IIntermediateEnumFieldMember ValidIdentity { get; set; }
        public ITokenEntryObjectRelationalMap ObjectModelDetails { get; set; }
        public IIntermediateInterfaceType RelativeInterface { get { return ObjectModelDetails.ImplementationDetails.Value.RelativeInterface; } }
    }

    internal class GrammarVocabularyRuleDetail
    {
        public ParserCompiler Compiler { get; set; }
        public IGrammarRuleSymbol Symbol { get; set; }
        public OilexerGrammarProductionRuleEntry Rule { get { return (OilexerGrammarProductionRuleEntry)this.DFAState.Entry; } }
        public IIntermediateEnumFieldMember Identity { get { return this.Compiler.SyntacticalSymbolModel.GetIdentitySymbolField(this.Symbol); } }
        public IIntermediateEnumFieldMember ValidIdentity { get { return this.Compiler.SyntacticalSymbolModel.GetValidSymbolField(this.Symbol); } }
        public IIntermediateClassMethodMember ParseMethod { get { return this.Compiler.ParserBuilder.GetEntryParseMethod(this.Rule); } }
        public IIntermediateClassMethodMember PredictMethod { get { return this.ProjectionAdapter == null ? null : this.Compiler.ParserBuilder.GetProjectionPredictMethod(this.ProjectionAdapter); } }
        public IIntermediateClassMethodMember InternalParseMethod { get { return this.Compiler.ParserBuilder.GetEntryInternalParseMethod(this.Rule); } }
        public SyntacticalDFARootState DFAState { get; set; }
        public ProductionRuleNormalAdapter NormalAdapter { get { return this.Compiler.AllRuleAdapters[this.Rule, this.DFAState]; } }
        public PredictionTreeDFAdapter ProjectionAdapter
        {
            get
            {
                if (NormalAdapter.AssociatedContext.RequiresProjection)
                    return this.Compiler.AdvanceMachines[Leaf];
                return null;
            }
        }
        public PredictionTreeLeaf Leaf { get { return this.Compiler.AllProjectionNodes[this.DFAState]; } }
        public IRuleEntryObjectRelationalMap ObjectModelDetails { get; set; }
        public IIntermediateInterfaceType RelativeInterface { get { return ObjectModelDetails.ImplementationDetails.Value.RelativeInterface; } }
        public IIntermediateClassType Class { get { return ObjectModelDetails.ImplementationDetails.Value.Class; } }
    }
}
