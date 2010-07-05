using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Rules
{
    partial class SyntacticalStreamState
    {
        #region CSStreamState nested types
        private class SourcesBuilder :
            IDisposable
        {
            #region SourcesBuilder data members
            private List<StatePath> firstSet = new List<StatePath>();

            private List<StatePath> followSet = new List<StatePath>();

            private List<StatePath> initialSet = new List<StatePath>();

            private List<StatePath> fullSet = new List<StatePath>();

            private Dictionary<IProductionRuleEntry, List<RulePath>> hideWatches = new Dictionary<IProductionRuleEntry, List<RulePath>>();
            #endregion // SourcesBuilder data members
            #region SourcesBuilder properties
            public List<StatePath> InitialSet
            {
                get
                {
                    return this.initialSet;
                }
            }

            public List<StatePath> FirstSet
            {
                get
                {
                    return this.firstSet;
                }
            }

            public List<StatePath> FollowSet
            {
                get
                {
                    return this.followSet;
                }
            }

            public List<StatePath> FullSet
            {
                get
                {
                    return this.fullSet;
                }
            }

            public StatePath this[int index]
            {
                get
                {
                    int r1 = 0;
                    int r2 = this.initialSet.Count;
                    int r3 = (r2 + this.firstSet.Count);
                    int r4 = (r3 + this.followSet.Count);
                    if ((index >= r1) && (index < r2))
                        return this.initialSet[index];
                    else if ((index >= r2) && (index < r3))
                        return this.firstSet[(index - r2)];
                    else if ((index >= r3) && (index < r4))
                        return this.followSet[(index - r3)];
                    return null;
                }
            }

            public int Count
            {
                get
                {
                    return (this.initialSet.Count + (this.firstSet.Count + this.followSet.Count));
                }
            }
            #endregion // SourcesBuilder properties
            #region SourcesBuilder methods
            private void AddHideWatch(RulePath path)
            {
                IProductionRuleEntry id;
                id = path.Original.Entry;
                if (!(this.hideWatches.ContainsKey(id)))
                    this.hideWatches.Add(id, new List<RulePath>());
                if (!(this.hideWatches[id].Contains(path)))
                    this.hideWatches[id].Add(path);
            }

            public bool AddFirst(StatePath first)
            {
                if (!(this.fullSet.Contains(first)))
                {
                    if (first.IsRule)
                        this.AddHideWatch(((RulePath)(first)));
                    this.fullSet.Add(first);
                    this.firstSet.Add(first);
                    return true;
                }
                return false;
            }

            public bool AddFollow(StatePath follow)
            {
                if (!(this.fullSet.Contains(follow)))
                {
                    RulePath rule = follow.Rule;
                    IProductionRuleEntry id = rule.Original.Entry;
                    if (this.hideWatches.ContainsKey(id) && (this.hideWatches[id].Contains(rule) && follow.Original.ContainsRule(id)))
                        rule.LineContains(id, rule, follow.Original[id],
                            follow);
                    this.fullSet.Add(follow);
                    this.followSet.Add(follow);
                    return true;
                }
                return false;
            }

            internal SourcesInfo GetSources()
            {
                return new SourcesInfo(this.initialSet.ToArray(), this.firstSet.ToArray(), this.followSet.ToArray());
            }

            public void Dispose()
            {
                this.initialSet.Clear();
                this.initialSet = null;
                this.firstSet.Clear();
                this.firstSet = null;
                this.followSet.Clear();
                this.followSet = null;
                this.fullSet.Clear();
                this.fullSet = null;
            }
            #endregion // SourcesBuilder methods
            #region SourcesBuilder .ctors
            internal SourcesBuilder(IEnumerable<StatePath> initialSet)
            {
                this.initialSet.AddRange(initialSet);
                this.fullSet.AddRange(initialSet);
            }
            #endregion // SourcesBuilder .ctors
        }
        #endregion // CSStreamState nested types
    }
}
