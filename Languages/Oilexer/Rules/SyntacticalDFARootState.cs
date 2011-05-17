using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class SyntacticalDFARootState :
        SyntacticalDFAState
    {

        private SyntacticalFollowTable followTable;
        private IProductionRuleEntry entry;
        private enum LinkElementKind
        {
            First,
            Follow,
        }
        public SyntacticalDFARootState(IProductionRuleEntry entry, ParserBuilder builder)
            : base(builder)
        {
            this.entry = entry;
        }
        internal IProductionRuleEntry Entry
        {
            get
            {
                return this.entry;
            }
        }
        internal void ReduceDFA()
        {
            Reduce(this, false);
            this.Enumerate();
        }

        public SyntacticalFollowTable FollowTable
        {
            get
            {
                if (this.followTable == null)
                    this.followTable = new SyntacticalFollowTable();
                return this.followTable;
            }
        }

        internal void Connect()
        {
            //var followTargets = new Dictionary<SyntacticalDFARootState, Dictionary<SyntacticalDFAState, SyntacticalDFAState>>();
            var stateRuleInfo = new Dictionary<SyntacticalDFAState, Dictionary<IProductionRuleEntry, SyntacticalDFARootState>>();
            List<SyntacticalDFARootState> leftRecursiveRules =null;
            var owner = new Dictionary<SyntacticalDFAState, SyntacticalDFARootState>();
            var following = new Stack<SyntacticalDFARootState>();
            var followed = new List<SyntacticalDFARootState>();
            var fullFlat = new List<SyntacticalDFAState>();
            var ruleEdges = new Dictionary<SyntacticalDFARootState, List<SyntacticalDFAState>>();
            int followInstances = 0;
            int potentiallyEmptyRules = 0;
            following.Push(this);
            do
            {
                var current = following.Pop();
                followed.Add(current);
                if (current.IsEdge)
                    potentiallyEmptyRules++;
                ruleEdges.Add(current, new List<SyntacticalDFAState>());
                var flatForm = new List<SyntacticalDFAState>();
                SyntacticalDFAState.FlatlineState(current, flatForm);
                
                if (!flatForm.Contains(current))
                    flatForm.Insert(0, current);
                fullFlat.AddRange(flatForm);
                foreach (var ruleState in flatForm)
                {
                    if (!owner.ContainsKey(ruleState) && ruleState != current)
                        owner.Add(ruleState, current);
                    if (ruleState.IsEdge)
                        ruleEdges[current].Add(ruleState);
                    var ruleInfo = (from ruleTransition in ruleState.OutTransitions.Keys
                                    from ruleSymbol in ruleTransition.Breakdown.Rules
                                    select builder.RuleDFAStates[ruleSymbol.Source]);//.ToArray();
                    if (!stateRuleInfo.ContainsKey(ruleState))
                        stateRuleInfo.Add(ruleState, new Dictionary<IProductionRuleEntry, SyntacticalDFARootState>());
                    foreach (var rule in ruleInfo)
                    {
                        if (!followed.Contains(rule) && !following.Contains(rule))
                            following.Push(rule);
                        if (!stateRuleInfo[ruleState].ContainsKey(rule.Entry))
                            stateRuleInfo[ruleState].Add(rule.Entry, rule);
                        rule.FollowTable.Follow(ruleState, ruleState.OutTransitions[rule.Entry]);
                        followInstances++;
                    }
                }
            } while (following.Count > 0);
            foreach (var deadState in (from state in stateRuleInfo.Keys
                                       where stateRuleInfo[state].Count == 0
                                       select state).ToArray())
                stateRuleInfo.Remove(deadState);
            ruleEdges = (from rule in ruleEdges.Keys
                         orderby rule.EntryName
                         select rule).ToDictionary(key => key, value => (from edge in ruleEdges[value]
                                                                         orderby edge.StateValue
                                                                         select edge).ToList());
            leftRecursiveRules = (from ruleState in ruleEdges.Keys
                                  where ruleState.DependsOn(ruleState.Entry)
                                  select ruleState).ToList();

            var nonLeftRecursiveRules = ruleEdges.Keys.Except(leftRecursiveRules).ToList();
            leftRecursiveRules.Sort(SyntacticalRootComparer.Singleton);
            nonLeftRecursiveRules.Sort(SyntacticalRootComparer.Singleton);
        }

        public string EntryName
        {
            get
            {
                return this.Entry.Name;
            }
        }

        public override string ToString()
        {
            return string.Format("EntryState {0}", this.EntryName);
        }


        public void GetLookAhead()
        {

        }

        private static void GetLookAhead(List<SyntacticalDFAState> searched)
        {

        }

    }
}
