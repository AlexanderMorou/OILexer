using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using AllenCopeland.Abstraction.Utilities.Collections;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    using RuleTree = AllenCopeland.Abstraction.Utilities.Collections.KeyedTree<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using RuleTreeNode = AllenCopeland.Abstraction.Utilities.Collections.KeyedTreeNode<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using AllenCopeland.Abstraction.Slf.Ast.Members;
    internal class EntryObjectRelationalMap :
        IEntryObjectRelationalMap
    {
        private IControlledCollection<IOilexerGrammarScannableEntry> implementsList;
        private IControlledCollection<IOilexerGrammarScannableEntry> implements;
        private IList<IOilexerGrammarScannableEntry> implementsSource;
        private IControlledDictionary<IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember> caseFields;


        internal IOilexerGrammarFileObjectRelationalMap fileMap;
        private RuleTreeNode implementationDetails;
        internal EntryObjectRelationalMap(IOilexerGrammarScannableEntry[] implementsSeries, OilexerGrammarFileObjectRelationalMap fileMap, IOilexerGrammarScannableEntry entry)
        {
            this.Entry = entry;
            this.implementsSource = implementsSeries.ToList();
            this.fileMap = fileMap;
        }

        //#region IRuleEntryObjectRelationalMap Members

        public IControlledDictionary<IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember> CaseFields
        {
            get
            {
                if (this.caseFields == null)
                    this.caseFields = new ControlledDictionary<IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember>(
                        from caseField in fileMap.CasesLookup
                        where caseField.Keys.Key2 == this.Entry
                        select new KeyValuePair<IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember>(caseField.Keys.Key1, caseField.Value));
                return this.caseFields;
            }
        }

        public IEnumerable<Tuple<IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember>> GetTokenCases(IOilexerGrammarProductionRuleEntry from)
        {
            foreach (var caseField in fileMap.CasesLookup)
                if (caseField.Keys.Key1 == from && caseField.Keys.Key2 is IOilexerGrammarTokenEntry)
                    yield return Tuple.Create(caseField.Keys.Key2, caseField.Value);
        }

        public IControlledCollection<IOilexerGrammarScannableEntry> Implements
        {
            get
            {
                if (this.implements == null)
                    this.implements = new ControlledCollection<IOilexerGrammarScannableEntry>(this.implementsSource);
                return this.implements;
            }
        }

        public IEnumerable<IEnumerable<IOilexerGrammarScannableEntry>> Variations
        {
            get
            {
                foreach (var entry in this.Implements)
                    foreach (var variation in GetVariationThereof(entry))
                        yield return variation;
                yield return GetSingleVariation();
            }
        }



        private IEnumerable<IOilexerGrammarScannableEntry> GetSingleVariation()
        {
            yield return this.Entry;
        }

        //#endregion

        private IEnumerable<IEnumerable<IOilexerGrammarScannableEntry>> GetVariationThereof(IOilexerGrammarScannableEntry target)
        {
            var targetMap = this.fileMap[target] as IRuleEntryObjectRelationalMap;
            foreach (var variation in targetMap.Variations)
                yield return GetVariationThereof(variation);
        }

        private IEnumerable<IOilexerGrammarScannableEntry> GetVariationThereof(IEnumerable<IOilexerGrammarScannableEntry> source)
        {
            yield return this.Entry;
            foreach (var element in source)
                yield return element;
        }

        //#region IEntryObjectRelationalMap Members

        IControlledCollection<IOilexerGrammarScannableEntry> IEntryObjectRelationalMap.Implements
        {
            get
            {
                if (this.implementsList == null)
                    this.implementsList = this.Implements;
                return implementsList;
            }
        }


        public IOilexerGrammarScannableEntry Entry { get; private set; }

        //#endregion

        public RuleTreeNode ImplementationDetails
        {
            get { return this.implementationDetails; }
            internal set { this.implementationDetails = value; }
        }
    }
}
