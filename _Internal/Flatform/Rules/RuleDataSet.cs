using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Oilexer.Utilities.Collections;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    /// <summary>
    /// Provides a structure for maintaining a series of 
    /// rule data items which aims to define the data structure
    /// of a rule.
    /// </summary>
    internal partial class RuleDataSet :
        ControlledStateCollection<IRuleDataItem>
    {
        private IRuleDataSetSource source;
        private RuleDataSet leftMerge,
                            rightMerge;
        public RuleDataSet(IRuleDataSetSource source)
        {
            this.source = source;
        }

        private RuleDataSet(RuleDataSet left, RuleDataSet right)
        {
            this.leftMerge = left;
            this.rightMerge = right;
        }

        public void AddItem(IRuleDataItem item)
        {
            AddImpl(item, true, true);
        }

        private void AddImpl(IRuleDataItem item, bool rankIncrease, bool intersectMustExist)
        {
            List<IRuleDataItem> equalNatured = new List<IRuleDataItem>();
            foreach (var current in this)
                if (current.AreEqualNatured(item))
                {
                    if (intersectMustExist)
                    {
                        if (current is IRuleDataEnumItem)
                        {
                            var left = ((IRuleDataEnumItem)current);
                            var right = ((IRuleDataEnumItem)item);
                            var intersect = left.CoveredSet & right.CoveredSet;
                            if (intersect.CountTrue() == 0)
                                continue;
                        }
                    }
                    equalNatured.Add(current);
                }
            if (equalNatured.Count > 0)
            {
                var bestNatured = item.ObtainBestNatured(equalNatured.ToArray());
                var mergedItem = bestNatured.MergeWith(item);
                this.baseCollection.Remove(bestNatured);
                if (rankIncrease && mergedItem.Rank == 0)
                    mergedItem.Rank = 1;
                this.baseCollection.Add(mergedItem);
            }
            else
                this.baseCollection.Add(item);
        }

        public void AddSet(RuleDataSet set)
        {
            foreach (var item in set)
                this.AddItem(item);
        }

        public void AddRank()
        {
            foreach (var item in this)
                item.Rank++;
        }

        public RuleDataSet MergeWith(RuleDataSet set)
        {
            RuleDataSet result = new RuleDataSet(this, set);
            foreach (var item in this)
                result.baseCollection.Add(item);
            foreach (var item in set)
                result.AddImpl(item, false, false);
            return result;
        }

        public IEnumerable<NamedSet> NamedSets
        {
            get
            {
                List<string> namedSets = new List<string>();
                foreach (var item in this)
                    if (item is IRuleDataNamedItem)
                    {
                        var nItem = ((IRuleDataNamedItem)item);
                        if (!namedSets.Contains(nItem.Name))
                            namedSets.Add(nItem.Name);
                    }
                foreach (var name in namedSets)
                    yield return new NamedSet(this, name);
                yield break;
            }
        }
    }
}
