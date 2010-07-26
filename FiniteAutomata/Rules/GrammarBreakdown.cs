using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    public class GrammarBreakdown
    {
        public class TokenElements :
            ControlledStateCollection<IGrammarConstantItemSymbol>
        {
            public TokenElements(IGrammarConstantItemSymbol[] symbols)
                : base(symbols)
            {
                
            }
        }

        public IControlledStateDictionary<ITokenEntry, TokenElements> LiteralSeriesTokens { get; private set; }
        public IControlledStateCollection<IGrammarConstantEntrySymbol> ConstantTokens { get; private set; }
        public IControlledStateCollection<IGrammarTokenSymbol> CaptureTokens { get; private set; }
        public IControlledStateCollection<IGrammarRuleSymbol> Rules { get; private set; }
        public IReadOnlyCollection<ITokenEntry> Tokens { get; private set; }
        public GrammarBreakdown(IGrammarSymbol[] symbols)
        {
            /* *
             * Obtain a sequence of the elements which are constant
             * items within a series of other constants.
             * */
            var symbolSets = (from symbol in symbols
                              let itemSymbol = symbol as IGrammarConstantItemSymbol
                              where itemSymbol != null
                              select itemSymbol).ToArray();
            //Obtain the distinct container tokens.
            var tokenSets = (from symbol in symbolSets
                             select symbol.Source).Distinct().ToArray();
            /* *
             * Obtain the container tokens along with the individual 
             * elements within them relative to the symbols provided.
             * */
            var tokenSymbolSets = from entry in tokenSets
                                  join itemSymbol in symbolSets on entry equals itemSymbol.Source into entrySymbols
                                  select new { Entry = entry, Items = entrySymbols.ToArray() };
            //Obtain the tokens which are just a constant by themselves.
            var constantEntries = (from symbol in symbols
                                   let constantEntry = symbol as IGrammarConstantEntrySymbol
                                   where constantEntry != null
                                   select constantEntry).ToArray();
            //Obtain the rules within the symbols.
            var ruleEntries = (from symbol in symbols
                               let ruleEntry = symbol as IGrammarRuleSymbol
                               where ruleEntry != null
                               select ruleEntry).ToArray();
            //Obtain the capture-style token symbols.
            var tokenEntries = (from symbol in symbols
                                let tokenEntry = symbol as IGrammarTokenSymbol
                                where tokenEntry != null &&
                                   (!(tokenEntry is IGrammarConstantEntrySymbol)) &&
                                   (!(tokenEntry is IGrammarConstantItemSymbol))
                                select tokenEntry).ToArray();
            var literalSeriesTokens = new Dictionary<ITokenEntry, TokenElements>();
            foreach (var tokenSymbolSet in tokenSymbolSets)
                literalSeriesTokens.Add(tokenSymbolSet.Entry, new TokenElements(tokenSymbolSet.Items));
            this.LiteralSeriesTokens = new ControlledStateDictionary<ITokenEntry, TokenElements>(literalSeriesTokens);
            ConstantTokens = new ControlledStateCollection<IGrammarConstantEntrySymbol>(constantEntries);
            CaptureTokens = new ControlledStateCollection<IGrammarTokenSymbol>(tokenEntries);
            Rules = new ControlledStateCollection<IGrammarRuleSymbol>(ruleEntries);
            Tokens = new ReadOnlyCollection<ITokenEntry>((
                      from constant in ConstantTokens
                      select constant.Source).Concat(
                      from captureToken in CaptureTokens
                      select captureToken.Source).Concat(
                      from literalToken in LiteralSeriesTokens.Keys
                      select literalToken).ToArray());

        }
    }
}
