using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.Builder;
using System.Diagnostics;

namespace Oilexer.FiniteAutomata.Rules
{
    [DebuggerDisplay("{StringForm}", Name = "{EntryName}")]
    public class SyntacticalDFARootState :
        SyntacticalDFAState
    {
        private enum LinkElementKind
        {
            First,
            Follow,
        }
        private SyntacticalFollowTable followTable;
        private IProductionRuleEntry entry;
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
            var owner = new Dictionary<SyntacticalDFAState, SyntacticalDFARootState>();
            var following = new Stack<SyntacticalDFARootState>();
            var followed = new List<SyntacticalDFARootState>();
            var fullFlat = new List<SyntacticalDFAState>();
            int uniqueNodes = 0;
            following.Push(this);
            do
            {
                var current = following.Pop();
                followed.Add(current);
                var flatForm = new List<SyntacticalDFAState>();
                SyntacticalDFAState.FlatlineState(current, flatForm);
                
                if (!flatForm.Contains(current))
                    flatForm.Insert(0, current);
                fullFlat.AddRange(flatForm);
                foreach (var ruleState in flatForm)
                {
                    if (!owner.ContainsKey(ruleState) && ruleState != current)
                    {
                        owner.Add(ruleState, current);
                        uniqueNodes++;
                    }
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
                    }
                }
            } while (following.Count > 0);
            foreach (var rsi in (from state in stateRuleInfo.Keys
                                where stateRuleInfo[state].Count == 0
                                select state).ToArray())
                stateRuleInfo.Remove(rsi);
            var ruleStateQuery = (from rule in followed
                                  orderby rule.EntryName
                                  select rule).ToArray();
            followed = (from rule in followed
                        orderby rule.EntryName
                        select rule).ToList();

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
        internal override string StringForm
        {
            get
            {
                return base.ToString();
            }
        }
    }
}
