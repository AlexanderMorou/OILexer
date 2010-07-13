﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.Builder;
using System.Diagnostics;
using Oilexer._Internal;

namespace Oilexer.FiniteAutomata.Rules
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
            followed.Sort(SyntacticalRootComparer.Singleton);
            ruleEdges = (from rule in ruleEdges.Keys
                         orderby rule.EntryName
                         select rule).ToDictionary(key => key, value => (from edge in ruleEdges[value]
                                                                         orderby edge.StateValue
                                                                         select edge).ToList());
            leftRecursiveRules = (from v in ruleEdges.Keys
                                  where v.DependsOn(v.Entry)
                                  select v).ToList();

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