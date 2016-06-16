using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class GrammarSymbolComparer :
        IComparer<IGrammarSymbol>
    {
        private GrammarSymbolSet _grammarSymbols;

        public GrammarSymbolComparer() { }

        public GrammarSymbolComparer(GrammarSymbolSet grammarSymbols)
        {
            this._grammarSymbols = grammarSymbols;
        }
        //#region IComparer<IGrammarSymbol> Members

        public int Compare(IGrammarSymbol x, IGrammarSymbol y)
        {
            if (x == null)
                if (y == null)
                    return 0;
                else
                    return -1;
            else if (y == null)
                return 1;
            if (x is IGrammarTokenSymbol)
            {
                var xgts = x as IGrammarTokenSymbol;
                if (y is IGrammarTokenSymbol)
                {
                    var ygts = y as IGrammarTokenSymbol;
                    bool xas = SymbolIsAmbiguity(xgts),
                         yas = SymbolIsAmbiguity(ygts);
                    if (xas && !yas)
                        return 1;
                    else if (!xas && yas)
                        return -1;
                    if (xgts is IGrammarConstantSymbol)
                    {
                        if (ygts is IGrammarConstantSymbol)
                        {
                            if (xgts is IGrammarConstantEntrySymbol)
                                if (ygts is IGrammarConstantEntrySymbol)
                                    return string.Compare(xgts.Source.Name, ygts.Source.Name, true);
                                else
                                    return -1;
                            else if (ygts is IGrammarConstantEntrySymbol)
                                return 1;
                            else if (xgts is IGrammarConstantItemSymbol)
                            {
                                if (ygts is IGrammarConstantItemSymbol)
                                {
                                    if (xgts.Source == ygts.Source)
                                    {
                                        var xgcis = xgts as IGrammarConstantItemSymbol;
                                        var ygcis = ygts as IGrammarConstantItemSymbol;
                                        return string.Compare(xgcis.SourceItem.Name, ygcis.SourceItem.Name, true);
                                    }
                                    else
                                        return string.Compare(xgts.Source.Name, ygts.Source.Name, true);
                                }
                                else
                                    return -1;
                            }
                        }
                        else
                            return -1;
                    }
                    else if (ygts is IGrammarConstantSymbol)
                        return 1;
                    else if (xgts is IGrammarVariableSymbol)
                    {
                        if (y is IGrammarVariableSymbol)
                            return string.Compare(xgts.Source.Name, ygts.Source.Name, true);
                        else
                            return -1;
                    }
                    return string.Compare(xgts.Source.Name, ygts.Source.Name, true);
                }
                else
                    return -1;
            }
            else if (y is IGrammarTokenSymbol)
            {
                return 1;
            }
            else if (x is IGrammarRuleSymbol)
            {
                var xgrs = x as IGrammarRuleSymbol;
                if (y is IGrammarRuleSymbol)
                {
                    var ygrs = y as IGrammarRuleSymbol;
                    return string.Compare(xgrs.Source.Name, ygrs.Source.Name, true);
                }
                else
                    return -1;
            }
            return 0;
        }

        protected bool SymbolIsAmbiguity(IGrammarTokenSymbol symbol)
        {
            if (this._grammarSymbols == null)
                return false;
            return this._grammarSymbols.AmbiguousSymbols.Any(k => k.Contains((IGrammarTokenSymbol)symbol));
        }

        //#endregion
    }
}
