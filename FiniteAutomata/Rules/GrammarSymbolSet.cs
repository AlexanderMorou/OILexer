using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer._Internal;
using Oilexer.Parser;
using Oilexer.Parser.Builder;
using Oilexer.Utilities.Collections;
using Oilexer._Internal.Inlining;
using Oilexer.Parser.GDFileData;
using Oilexer.FiniteAutomata.Tokens;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.FiniteAutomata.Rules
{
    public class GrammarSymbolSet :
        ControlledStateCollection<IGrammarSymbol>,
        IGrammarSymbolSet
    {
        /// <summary>
        /// Creates a new <see cref="GrammarSymbolSet"/> with
        /// the <paramref name="source"/> file provided.
        /// </summary>
        /// <param name="source">The grammar description file
        /// which denotes the symbols of the language.</param>
        public GrammarSymbolSet(IGDFile source)
        {
            /* *
             * The symbol set will be used to construct the 
             * language's primary enumerator.  Since it will
             * consist of elements from a very large pool 
             * (beyond 64 elements), it will consist of a structure
             * with 'slots', the elements from this listing will
             * be a sorted representation of that enum's values.
             * * 
             * The bits will be aligned relative to their position
             * in this listing.
             * */
            IEnumerable<InlinedTokenEntry> tokens = source.GetTokens();
            IEnumerable<IProductionRuleEntry> rules = source.GetRules();
            List<IGrammarSymbol> sorted = new List<IGrammarSymbol>();
            foreach (var token in tokens)
                if (token is InlinedTokenEofEntry)
                    sorted.Add(new GrammarConstantEntrySymbol(token));
                else
                    switch (token.DetermineKind())
                    {
                        case RegularCaptureType.Recognizer:
                            //Simple literal values are quick recognizers.
                            if (token.Branches.Count == 1 &&
                                token.Branches[0].Count == 1 &&
                                token.branches[0][0] is ILiteralTokenItem)
                                sorted.Add(new GrammarConstantEntrySymbol(token));
                            else
                                goto case RegularCaptureType.Capturer;
                            break;
                        case RegularCaptureType.Capturer:
                            //Element is a variable capturer/recognizer.
                            sorted.Add(new GrammarVariableSymbol(token));
                            break;
                        case RegularCaptureType.Transducer:
                            /* *
                             * A transducer which matches a literal
                             * from a set.
                             * */
                            InjectTokenLiteralSet(token, sorted);
                            break;
                    }
            /* *
             * Add rules last, quickest sort is a list already
             * in order.
             * *
             * Rules won't be a part of the in-order list of 
             * elements, but it is important to be able to refer to
             * all constructs in a uniform manner for the sake of
             * the parser compiler.
             * */
            foreach (var rule in rules)
                sorted.Add(new GrammarRuleSymbol(rule));
            sorted.Sort(new GrammarSymbolComparer());

            foreach (var item in sorted)
                base.baseList.Add(item);
        }

        private void InjectTokenLiteralSet(InlinedTokenEntry token, ICollection<IGrammarSymbol> sorted)
        {
            InjectTokenLiteralSet(token, token.Branches, sorted);
        }
        private void InjectTokenLiteralSet(InlinedTokenEntry token, ITokenExpressionSeries series, ICollection<IGrammarSymbol> sorted)
        {
            foreach (var expression in series)
                InjectTokenLiteralSet(token, expression, sorted);
        }
        private void InjectTokenLiteralSet(InlinedTokenEntry token, ITokenExpression expression, ICollection<IGrammarSymbol> sorted)
        {
            if (expression.Count != 1)
                return;
            if (expression[0] is ILiteralTokenItem)
                InjectTokenLiteralSet(token, (ILiteralTokenItem)(expression[0]), sorted);
            else if (expression[0] is ITokenGroupItem)
                InjectTokenLiteralSet(token, (ITokenGroupItem)(expression[0]), sorted);
        }

        private void InjectTokenLiteralSet(InlinedTokenEntry token, ILiteralTokenItem item, ICollection<IGrammarSymbol> sorted)
        {
            sorted.Add(new GrammarConstantItemSymbol(token, item));
        }

        #region IGrammarSymbolSet Members

        public IGrammarSymbol this[ITokenEntry entry]
        {
            get {
                return this.FirstOrDefault(currentEntry =>
                    {
                        if (currentEntry is IGrammarTokenSymbol)
                        {
                            var currentCES = (IGrammarTokenSymbol)currentEntry;
                            return currentCES.Source == entry;
                        }
                        else if (currentEntry is IGrammarConstantEntrySymbol)
                        {
                            var currentCES = (IGrammarConstantEntrySymbol)currentEntry;
                            return currentCES.Source == entry;
                        }
                        return false;
                    });
            }
        }

        public IGrammarSymbol this[ILiteralTokenItem entry]
        {
            get {
                return this.FirstOrDefault(currentEntry =>
                {
                    if (currentEntry is IGrammarConstantItemSymbol)
                    {
                        var currentCES = (IGrammarConstantItemSymbol)currentEntry;
                        return currentCES.SourceItem == entry;
                    }
                    return false;
                });
            }
        }

        public IGrammarSymbol this[IProductionRuleEntry entry]
        {
            get
            {
                return this.FirstOrDefault(currentEntry =>
                {
                    if (currentEntry is IGrammarRuleSymbol)
                    {
                        var currentCES = (IGrammarRuleSymbol)currentEntry;
                        return currentCES.Source == entry;
                    }
                    return false;
                });

            }
        }

        #endregion
    }
}
