using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class GrammarBreakdown
    {
        public class TokenElements :
            ControlledCollection<IGrammarConstantItemSymbol>
        {
            public TokenElements(IGrammarConstantItemSymbol[] symbols)
                : base(symbols)
            {
                
            }
        }

        public IControlledDictionary<IOilexerGrammarTokenEntry, TokenElements> LiteralSeriesTokens { get; private set; }
        public IControlledCollection<IGrammarConstantEntrySymbol> ConstantTokens { get; private set; }
        public IControlledCollection<IGrammarTokenSymbol> CaptureTokens { get; private set; }
        public IControlledCollection<IGrammarRuleSymbol> Rules { get; private set; }
        public IControlledCollection<IOilexerGrammarTokenEntry> Tokens { get; private set; }
        public GrammarBreakdown(IGrammarSymbol[] symbols)
        {
            /* *
             * Obtain a sequence of the elements which are constant items within a series of other constants (i.e.: enum field members.)
             * */
            var symbolSets = (from symbol in symbols
                              let itemSymbol = symbol as IGrammarConstantItemSymbol
                              where itemSymbol != null
                              select itemSymbol).ToArray();
            //Obtain the distinct container tokens.
            var tokenSets = (from symbol in symbolSets
                             select symbol.Source).Distinct().ToArray();
            /* *
             * Obtain the container tokens along with the individual elements within them relative to the symbols provided.
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
            var literalSeriesTokens = new Dictionary<IOilexerGrammarTokenEntry, TokenElements>();
            foreach (var tokenSymbolSet in tokenSymbolSets)
                literalSeriesTokens.Add(tokenSymbolSet.Entry, new TokenElements(tokenSymbolSet.Items));
            this.LiteralSeriesTokens = new ControlledDictionary<IOilexerGrammarTokenEntry, TokenElements>(literalSeriesTokens);
            ConstantTokens = new ControlledCollection<IGrammarConstantEntrySymbol>(constantEntries);
            CaptureTokens = new ControlledCollection<IGrammarTokenSymbol>(tokenEntries);
            Rules = new ControlledCollection<IGrammarRuleSymbol>(ruleEntries);
            Tokens = new ControlledCollection<IOilexerGrammarTokenEntry>((
                      from constant in ConstantTokens
                      select constant.Source).Concat(
                      from captureToken in CaptureTokens
                      select captureToken.Source).Concat(
                      from literalToken in LiteralSeriesTokens.Keys
                      select literalToken).Distinct().ToArray());

        }
    }
}
