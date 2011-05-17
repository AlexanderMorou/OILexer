using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    internal class RuleEntryObjectRelationalMap :
        IRuleEntryObjectRelationalMap
    {
        private IReadOnlyCollection<IScannableEntry> implementsList;
        private IReadOnlyCollection<IProductionRuleEntry> implements;
        private IList<IProductionRuleEntry> implementsSource;
        private IGDFileObjectRelationalMap fileMap;
        public RuleEntryObjectRelationalMap(IProductionRuleEntry[] implementsSeries, IGDFileObjectRelationalMap fileMap, IProductionRuleEntry entry)
        {
            this.Entry = entry;
            this.implementsSource = implementsSeries.ToList();
            this.fileMap = fileMap;
        }

        #region IRuleEntryObjectRelationalMap Members

        public IReadOnlyCollection<IProductionRuleEntry> Implements
        {
            get
            {
                if (this.implements == null)
                    this.implements = new ReadOnlyCollection<IProductionRuleEntry>(this.implementsSource);
                return this.implements;
            }
        }

        public IEnumerable<IEnumerable<IProductionRuleEntry>> Variations
        {
            get
            {
                foreach (var entry in this.Implements)
                    foreach (var variation in GetVariationThereof(entry))
                        yield return variation;
                yield return GetSingleVariation();
            }
        }

        private IEnumerable<IProductionRuleEntry> GetSingleVariation()
        {
            yield return this.Entry;
        }

        public IProductionRuleEntry Entry { get; private set; }

        #endregion

        private IEnumerable<IEnumerable<IProductionRuleEntry>> GetVariationThereof(IProductionRuleEntry target)
        {
            var targetMap = this.fileMap[target] as IRuleEntryObjectRelationalMap;
            foreach (var variation in targetMap.Variations)
                yield return GetVariationThereof(variation);
        }

        private IEnumerable<IProductionRuleEntry> GetVariationThereof(IEnumerable<IProductionRuleEntry> source)
        {
            foreach (var element in source)
                yield return element;
            yield return this.Entry;
        }

        #region IEntryObjectRelationalMap Members

        IReadOnlyCollection<IScannableEntry> IEntryObjectRelationalMap.Implements
        {
            get
            {
                if (this.implementsList == null)
                    this.implementsList = this.Implements.GetCovariant<IScannableEntry, IProductionRuleEntry>();
                return implementsList;
            }
        }

        IEnumerable<IEnumerable<IScannableEntry>> IEntryObjectRelationalMap.Variations
        {
            get { return this.Variations; }
        }

        IScannableEntry IEntryObjectRelationalMap.Entry
        {
            get { return this.Entry; }
        }

        #endregion
    }
}
