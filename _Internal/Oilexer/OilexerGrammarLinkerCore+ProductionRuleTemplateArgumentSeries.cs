using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    partial class OilexerGrammarLinkerCore
    {
        internal struct ProductionRuleTemplateArgumentSeries :
            IEnumerable<ProductionRuleTemplateArgumentSeries>
        {
            internal struct ArgumentData
            {
                internal readonly IProductionRuleTemplatePart Relative;
                internal readonly IProductionRuleSeries Replacement;
                internal ArgumentData(IProductionRuleTemplatePart relative, IProductionRuleSeries replacement)
                {
                    this.Relative = relative;
                    this.Replacement = replacement;
                }
            }
            internal readonly IDictionary<IProductionRuleTemplatePart, ArgumentData> Lookup;
            private IDictionary<IProductionRuleTemplatePart, ArgumentData>[] dynLookup;
            private IDictionary<IProductionRuleTemplatePart, ArgumentData> fixedLookup;
            private int index;
            private ProductionRuleTemplateArgumentSeries(IDictionary<IProductionRuleTemplatePart, ArgumentData> lookup, int index)
            {
                this.Lookup = lookup;
                this.dynLookup = null;
                this.fixedLookup = null;
                this.index = index;
            }
            [DebuggerStepThrough]
            internal ProductionRuleTemplateArgumentSeries(IOilexerGrammarProductionRuleTemplateEntry entry, ITemplateReferenceProductionRuleItem reference)
            {
                TemplateArgumentInformation tai = entry.GetArgumentInformation();
                if (tai.InvalidArguments > 0)
                {
                    this.dynLookup = null;
                    this.fixedLookup = null;
                    this.Lookup = null;
                    this.index = int.MinValue;
                    return;
                }
                this.fixedLookup = new Dictionary<IProductionRuleTemplatePart, ArgumentData>();
                if (tai.DynamicArguments > 0)
                    this.dynLookup = new IDictionary<IProductionRuleTemplatePart, ArgumentData>[(reference.Count - tai.FixedArguments) / tai.DynamicArguments];
                else
                    this.dynLookup = new IDictionary<IProductionRuleTemplatePart, ArgumentData>[0];
                for (int i = 0; i < tai.FixedArguments; i++)
                {
                    fixedLookup.Add(entry.Parts[i], new ArgumentData(entry.Parts[i], reference[i]));
                }
                //Dynamic series index.
                for (int i = 0, dSerInd = 0; i < reference.Count - tai.FixedArguments; i += tai.DynamicArguments, dSerInd++)
                {
                    dynLookup[dSerInd] = new Dictionary<IProductionRuleTemplatePart, ArgumentData>();
                    for (int j = i; j < i + tai.DynamicArguments; j++)
                    {
                        dynLookup[dSerInd].Add(entry.Parts[j - i], new ArgumentData(entry.Parts[j - i], reference[j]));
                    }
                }
                this.Lookup = null;
                this.index = int.MinValue;
            }

            [DebuggerStepThrough]
            public IEnumerator<ProductionRuleTemplateArgumentSeries> GetEnumerator()
            {
                if (this.dynLookup == null)
                    throw new InvalidOperationException("Object in invalid state.");
                if (dynLookup.Length > 0)
                {
                    int dSerInd = 0;
                    foreach (Dictionary<IProductionRuleTemplatePart, ArgumentData> d in dynLookup)
                    {
                        Dictionary<IProductionRuleTemplatePart, ArgumentData> currentLookup = new Dictionary<IProductionRuleTemplatePart, ArgumentData>();
                        foreach (IProductionRuleTemplatePart iprtp in fixedLookup.Keys)
                            currentLookup.Add(iprtp, fixedLookup[iprtp]);
                        foreach (IProductionRuleTemplatePart iprtp in d.Keys)
                            currentLookup.Add(iprtp, d[iprtp]);
                        yield return new ProductionRuleTemplateArgumentSeries(currentLookup, dSerInd++);
                    }
                }
                else
                    yield return new ProductionRuleTemplateArgumentSeries(fixedLookup, 0);
                yield break;
            }

            #region IEnumerable<ProductionRuleTemplateArgumentSeries> Members

            IEnumerator<ProductionRuleTemplateArgumentSeries> IEnumerable<ProductionRuleTemplateArgumentSeries>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            public int Index
            {
                get
                {
                    return this.index;
                }
            }

            public bool ContainsParameter(string name)
            {
                if (this.Lookup == null)
                    throw new InvalidOperationException("Object not in a valid state to perform this action.");
                foreach (IProductionRuleTemplatePart part in this.Lookup.Keys)
                    if (part.Name == name)
                        return true;
                return false;
            }
            public IProductionRuleTemplatePart GetParameter(string name)
            {
                if (this.Lookup == null)
                    throw new InvalidOperationException("Object not in a valid state to perform this action.");
                foreach (IProductionRuleTemplatePart part in this.Lookup.Keys)
                    if (part.Name == name)
                        return part;
                throw new ArgumentOutOfRangeException("name");
            }

            public IProductionRuleSeries this[string name]
            {
                get
                {
                    if (this.Lookup == null)
                        throw new InvalidOperationException("Object not in a valid state to perform this action.");
                    foreach (IProductionRuleTemplatePart part in this.Lookup.Keys)
                        if (part.Name == name)
                            return this.Lookup[part].Replacement;
                    throw new ArgumentOutOfRangeException("name");
                }
            }

            public IProductionRuleSeries this[IProductionRuleTemplatePart part]
            {
                get
                {
                    if (this.Lookup == null)
                        throw new InvalidOperationException("Object not in a valid state to perform this action.");
                    if (this.Lookup.ContainsKey(part))
                        return this.Lookup[part].Replacement;
                    throw new ArgumentOutOfRangeException("name");
                }
            }
        }
    }
}
