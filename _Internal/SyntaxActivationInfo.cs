using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Flatform.Tokens;
using Oilexer._Internal.Flatform.Rules;
using Oilexer.Utilities.Collections;
using System.Collections.ObjectModel;
using System.Collections;
using Oilexer._Internal.Flatform.Rules.StateSystem;
using Oilexer.Types;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Types.Members;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal struct SyntaxActivationInfoResult
    {
        public ISyntaxActivationItem[] DataSet { get; private set; }
        public LargeFootprintEnum<ISyntaxActivationItem>.BuildResults EnumResults { get; private set; }
        //public LargeFootprintEnum<FlattenedRuleEntry>.BuildResults RuleResults { get; private set; }
        public IDictionary<ISyntaxActivationItem, LargeFootprintEnumGroupItem<ISyntaxActivationItem>> FootprintLookup { get; private set; }
        //public IDictionary<FlattenedRuleEntry, LargeFootprintEnumGroupItem<FlattenedRuleEntry>> RulePrintLookup { get; private set; }
        //public IPropertyMember RuleMaskProperty { get; private set; }
        public SyntaxActivationInfoResult(
            ISyntaxActivationItem[] dataSet, 
            LargeFootprintEnum<ISyntaxActivationItem>.BuildResults enumResults, 
            IDictionary<ISyntaxActivationItem, LargeFootprintEnumGroupItem<ISyntaxActivationItem>> footprintLookup)
            : this()
        {
            this.DataSet = dataSet;
            this.EnumResults = enumResults;
            //this.RuleMaskProperty = ruleMaskProperty;
            //this.RuleResults = ruleResults;
            this.FootprintLookup = footprintLookup;
            //this.RulePrintLookup = rulePrintLookup;
        }
    }
    internal class SyntaxActivationInfo :
        ControlledStateCollection<ISyntaxActivationItem>
    {
        internal SyntaxActivationInfoResult data;
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Simple (?) concept here:                                                              *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * The rules defined within the scope of the language exist as simple instances of rule  *
         * classes.  The individual transitions from state->state in the rules are handled by a  *
         * special structure which has operator overloads for |, ^ and & for simple set logic.   *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * For lookup reasons, this also implements a HashCode overload, as well as implements   *
         * IComparable<T> for cases where the hash is not unique.                                *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Prior to entering a state, it is checked for 'final' status, if it's not final, it's  *
         * evaluated and all rule based references are eliminated in light of their token        *
         * alternatives.  The true 'magic', in this case is the dictionary that holds the DFA    *
         * nodes generated from each individual rule state unions.  Part of this key is the      *
         * transition requirement needed to reach the state-set.                                 *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * On top of holding information relative to the transitions, each state contains path   *
         * information, which is an aid for reconstitution of the data graph once the parse is   *
         * finished.                                                                             *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private Dictionary<ITokenItem, ProjectConstructor.EnumStateMachineData> enumItemData;
        private Dictionary<ITokenEntry, TokenFinalData> tokenData;
        private Dictionary<IProductionRuleEntry, FlattenedRuleEntry> ruleData;

        public SyntaxActivationInfo(TokenFinalDataSet tokenData, ICollection<FlattenedRuleEntry> ruleData, INameSpaceDeclaration dataStorePoint)
            : base()
        {
            this.data = ObtainSetData(tokenData, ruleData, dataStorePoint, out this.enumItemData, out this.tokenData, out this.ruleData);
            base.baseCollection = this.data.DataSet;
        }

        private static SyntaxActivationInfoResult ObtainSetData(TokenFinalDataSet tokenData, ICollection<FlattenedRuleEntry> ruleData, INameSpaceDeclaration dataStorePoint, out Dictionary<ITokenItem, ProjectConstructor.EnumStateMachineData> enumItemData, out Dictionary<ITokenEntry, TokenFinalData> tokenInfo, out Dictionary<IProductionRuleEntry, FlattenedRuleEntry> ruleInfo)
        {
            int count = 0; //ruleData.Count;
            IDictionary<ISyntaxActivationItem, LargeFootprintEnumGroupItem<ISyntaxActivationItem>> itemMapping =new Dictionary<ISyntaxActivationItem,LargeFootprintEnumGroupItem<ISyntaxActivationItem>>();
            LargeFootprintEnum<ISyntaxActivationItem> resultEnumData = new LargeFootprintEnum<ISyntaxActivationItem>("TokenTransition", false);
            LargeFootprintEnum<FlattenedRuleEntry> ruleEnumData = new LargeFootprintEnum<FlattenedRuleEntry>("RuleIdentifier", false);
            
            //IDictionary<FlattenedRuleEntry, LargeFootprintEnumGroupItem<FlattenedRuleEntry>> ruleItemMapping = new Dictionary<FlattenedRuleEntry, LargeFootprintEnumGroupItem<FlattenedRuleEntry>>();
            enumItemData = new Dictionary<ITokenItem, ProjectConstructor.EnumStateMachineData>();
            tokenInfo = new Dictionary<ITokenEntry, TokenFinalData>();
            ruleInfo = new Dictionary<IProductionRuleEntry, FlattenedRuleEntry>();
            List<LargeFootprintEnum<ISyntaxActivationItem>> ruleSectors = new List<LargeFootprintEnum<ISyntaxActivationItem>>();
            int ruleSectorCount = (int)Math.Ceiling(((double)(ruleData.Count)) / SetCommon.MinimalSetData.setSize);
            int ruleCount = ruleData.Count;
            List<TokenEnumFinalData> enumTokens = new List<TokenEnumFinalData>();
            List<TokenFinalData> normalTokens = new List<TokenFinalData>();
            //ISyntaxActivationItem[][] ruleSectorData = new ISyntaxActivationItem[ruleSectorCount][];

            foreach (var token in tokenData.Keys)
            {
                var currentData = tokenData[token];
                TokenEnumFinalData tokenEnumData = currentData as TokenEnumFinalData;
                if (tokenEnumData == null)
                {
                    count++;
                    normalTokens.Add(currentData);
                }
                else
                {
                    count += tokenEnumData.Elements.Count();
                    enumTokens.Add(tokenEnumData);
                }
            }
            Dictionary<IEnumeratorType, IList<IFieldMember>> captureParents = new Dictionary<IEnumeratorType, IList<IFieldMember>>();
            foreach (var normData in normalTokens)
            {
                var currentParent = (IEnumeratorType)(normData.ValidCaseField.ParentTarget);
                if (!captureParents.ContainsKey(currentParent))
                    captureParents.Add(currentParent, new List<IFieldMember>());
                captureParents[currentParent].Add(normData.ValidCaseField);
            }
            ISyntaxActivationItem[][] captureSelectorData = new ISyntaxActivationItem[captureParents.Count][];

            for (int i = 0; i < captureParents.Count; i++)
            {
                var currentParent = captureParents.Keys.ElementAt(i);
                captureSelectorData[i] = new ISyntaxActivationItem[captureParents[currentParent].Count];
            }
            ISyntaxActivationItem[] items = new ISyntaxActivationItem[count];
            int index = 0;
            foreach (var enumData in enumTokens)
            {
                var elements = enumData.Elements.ToArray();
                int itemCount = elements.Length;
                ISyntaxActivationItem[][] currentSet = new ISyntaxActivationItem[(int)Math.Ceiling((double)itemCount / SetCommon.MinimalSetData.setSize)][];
                LargeFootprintEnumGroup<ISyntaxActivationItem> enumGroup = null;
                for (int i = 0; i < itemCount; i++)
                {
                    if (currentSet[i / SetCommon.MinimalSetData.setSize] == null)
                    {
                        if ((i / SetCommon.MinimalSetData.setSize) == (currentSet.Length - 1) &&
                            ((itemCount % SetCommon.MinimalSetData.setSize) != 0))
                            currentSet[i / SetCommon.MinimalSetData.setSize] = new ISyntaxActivationItem[itemCount % SetCommon.MinimalSetData.setSize];
                        else
                            currentSet[i / SetCommon.MinimalSetData.setSize] = new ISyntaxActivationItem[SetCommon.MinimalSetData.setSize];
                    }
                    var itemData = enumData.FinalItemLookup[elements[i]];
                    items[index++] = currentSet[i / SetCommon.MinimalSetData.setSize][i % SetCommon.MinimalSetData.setSize] = new SyntaxActivationEnumItem(enumData, itemData);
                    enumItemData.Add(elements[i], itemData);
                }
                int setCount = currentSet.Length;
                for (int i = 0; i < setCount; i++)
                {
                    var set = currentSet[i];
                    if (setCount == 1)
                    {
                        var entryDataSet = enumData as TokenEnumFinalData;
                        enumGroup = resultEnumData.Add(enumData.Entry.Name, string.Format("Enumeration set for {0}.", enumData.Entry.Name), string.Empty, entryDataSet.CaseEnumeration);
                    }
                    else
                    {
                        var entryDataSet = enumData as TokenEnumSetFinalData;
                        enumGroup = resultEnumData.Add(string.Format("{0}{1}", enumData.Entry.Name, i + 1), string.Format("Enumeration set {0} of {1} for {2}.", i + 1, setCount, enumData.Entry.Name), string.Empty, entryDataSet.SubsetEnumerations.Values.ElementAt(i));
                    }
                    for (int j = 0; j < set.Length; j++)
                    {
                        var currentJ = set[j] as SyntaxActivationEnumItem;
                        var currentGroupItem = enumGroup.Add(set[j], currentJ.Source.Source.Name, string.Format("Enum-item rule constraint {0} of {1}.", currentJ.Source.Source.Name, currentJ.SourceRoot.Entry.Name), string.Format("<para>Original declaration:</para>\r\n<para>{0}</para>", currentJ.Source.Source.ToString()), currentJ.Source.ResultEnumerationMember);
                        itemMapping.Add(currentJ, currentGroupItem);
                    }
                }
            }

            //Captures are a bit different.
            Dictionary<IEnumeratorType, int> relativeCaptureIndices = new Dictionary<IEnumeratorType, int>();
            foreach (var normalData in normalTokens)
            {
                var currentParent = normalData.ValidCaseField.ParentTarget as IEnumeratorType;
                if (!relativeCaptureIndices.ContainsKey(currentParent))
                    relativeCaptureIndices.Add(currentParent, 0);
                items[index++] = captureSelectorData[GetIndexOf(relativeCaptureIndices.Keys, currentParent)][relativeCaptureIndices[currentParent]++] = new SyntaxActivationTokenItem(normalData);
                tokenInfo.Add(normalData.Entry, normalData);
            }

            LargeFootprintEnumGroup<ISyntaxActivationItem> captureGroup = null;
            for (int i = 0; i < captureSelectorData.Length; i++)
            {
                var set = captureSelectorData[i];
                var currentParent = relativeCaptureIndices.Keys.ElementAt(i);
                if (captureSelectorData.Length == 1)
                    captureGroup = resultEnumData.Add("Captures", "Capture rule restrictions.", string.Empty, currentParent);
                else
                    captureGroup = resultEnumData.Add(string.Format("Captures{0}", i + 1), string.Format("Capture rule restrictions {0} of {1}.", i + 1, relativeCaptureIndices.Count), string.Empty, currentParent);
                for (int j = 0; j < set.Length; j++)
                {
                    var currentJ = set[j] as SyntaxActivationTokenItem;
                    var currentGroupItem = captureGroup.Add(set[j], currentJ.Source.Entry.Name, string.Format("Capture rule constraint {0}.", currentJ.Source.Entry.Name), string.Format("<para>Original declaration:</para>\r\n<para>{0}</para>", currentJ.Source.Entry.ToString()), currentJ.Source.ValidCaseField);
                    itemMapping.Add(currentJ, currentGroupItem);
                }
            }
            

            var buildResults = resultEnumData.Build(dataStorePoint);
            var resSet = buildResults.ResultantSetsType;
            /*
            IExpression ruleMaskExpression = null;
            foreach (var set in ruleSectorData)
                foreach (var item in set)
                {
                    if (ruleMaskExpression == null)
                        ruleMaskExpression = buildResults.TargetMapping[itemMapping[item]].Member.GetReference();
                    else
                        ruleMaskExpression = new BinaryOperationExpression(ruleMaskExpression, CodeBinaryOperatorType.BitwiseOr, buildResults.TargetMapping[itemMapping[item]].Member.GetReference());
                }*/
            return new SyntaxActivationInfoResult(items, buildResults, itemMapping);
        }

        private static int GetIndexOf<T>(IEnumerable<T> target, T element)
            where T :
                class
        {
            int index = 0;
            foreach (var item in target)
                if (item == element)
                    return index;
                else
                    index++;
            return -1;
        }

        public BitArray GetBitArray(SimpleLanguageBitArray source)
        {
            BitArray result = new BitArray(this.Count);
            List<ITokenEntry> enumTokens = new List<ITokenEntry>();
            List<ITokenEntry> normTokens = new List<ITokenEntry>();

            var rules = source.GetRuleRange();
            var tokens = source.GetTokenRange();

            foreach (var tok in tokens)
                if (this.tokenData.ContainsKey(tok))
                    normTokens.Add(tok);
                else
                    enumTokens.Add(tok);
            foreach (var tok in enumTokens)
            {
                var tokRange = source.GetSubsetInformation(tok);
                foreach (var item in tokRange.GetSeries())
                {
                    int index = GetIndexOf(item);
                    if (index > -1)
                        result[index] = true;
                }
            }
            foreach (var tok in normTokens)
            {
                int index = GetIndexOf(tok);
                if (index > -1)
                    result[index] = true;               
            }
#if false
            foreach (var rule in rules)
            {
                int index = GetIndexOf(rule);
                if (index > -1)
                    result[index] = true;
            }
#endif
            return result;
        }
#if false
        private int GetIndexOf(IProductionRuleEntry rule)
        {
            int index = 0;
            foreach (var item in this)
            {
                if (item.Type == SyntaxActivationItemType.RuleReference)
                {
                    var rItem = item as SyntaxActivationRuleItem;
                    if (rItem.Source.Source == rule)
                        return index;
                }
                index++;
            }
            return -1;
        }
#endif
        private int GetIndexOf(ITokenItem enumItem)
        {
            int index = 0;
            foreach (var item in this)
            {
                if (item.Type == SyntaxActivationItemType.EnumItemReference)
                {
                    var eItem = item as SyntaxActivationEnumItem;
                    if (eItem.Source.Source == enumItem)
                        return index;
                }
                index++;
            }
            return -1;
        }

        private int GetIndexOf(ITokenEntry normEntry)
        {
            int index = 0;
            foreach (var item in this)
            {
                if (item.Type == SyntaxActivationItemType.TokenReference)
                {
                    var tokItem = item as SyntaxActivationTokenItem;
                    if (tokItem.Source.Entry == normEntry)
                        return index;
                }
                index++;
            }
            return -1;
        }
    }
}
