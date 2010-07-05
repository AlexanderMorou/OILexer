using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    public class GrammarSymbolComparer :
        IComparer<IGrammarSymbol>
    {
        #region IComparer<IGrammarSymbol> Members

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
                    if (xgts is IGrammarConstantSymbol)
                    {
                        if (ygts is IGrammarConstantSymbol)
                        {
                            if (xgts is IGrammarConstantEntrySymbol)
                                if (ygts is IGrammarConstantEntrySymbol)
                                    return xgts.Source.Name.CompareTo(ygts.Source.Name);
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
                                        return xgcis.SourceItem.Name.CompareTo(ygcis.SourceItem.Name);
                                    }
                                    else 
                                        return xgts.Source.Name.CompareTo(ygts.Source.Name);
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
                            return xgts.Source.Name.CompareTo(ygts.Source.Name);
                        else
                            return -1;
                    }
                    return xgts.Source.Name.CompareTo(ygts.Source.Name);
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
                    return xgrs.Source.Name.CompareTo(ygrs.Source.Name);
                }
                else
                    return -1;
            }
            return 0;
        }

        #endregion
    }
}
