using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Flatform.Tokens;
using Oilexer.Utilities.Collections;
using Oilexer._Internal.Flatform.Rules.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal class FlattenedRuleExpression :
        ReadOnlyDictionary<IProductionRuleItem, IFlattenedRuleItem>,
        IRuleDataSetSource
    {
        //public RuleDataGroupNode DataNode { get; private set; }

        public RuleDataSet DataSet { get; private set; }

        public IProductionRule Source { get; private set; }

        public IProductionRuleEntry SourceRoot { get; private set; }

        public IProductionRuleSeries SourceParent { get; private set; }

        public FlattenedRuleExpressionSeries Parent { get; private set; }

        public FlattenedRuleEntry Root { get; private set; }

        public SimpleLanguageState State { get; private set; }

        public FlattenedRuleExpression(
            IProductionRule source, IProductionRuleEntry sourceRoot, IProductionRuleSeries sourceParent,
            FlattenedRuleExpressionSeries parent, FlattenedRuleEntry root)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.SourceParent = sourceParent;
            this.Parent = parent;
            this.Root = root;
        }

        public void Initialize(TokenFinalDataSet tokenLookup, IProductionRuleEntry[] productionRules)
        {
            this.DataSet = new RuleDataSet(this);
            foreach (var item in this.Source)
            {
                /* *
                 * All other elements at this point have been reduced into nothingness.
                 * *
                 * That is of the following:
                 * 1. Template Rules      
                 * 2. Template References 
                 * 3. Literals            
                 * They were removed via Template Expanding and diposal
                 * of un-used rules, cross-checking all tokens for a 
                 * matching literal, if none exists, make a token containing the 
                 * term.
                 * *
                 * Next version: Use a visitor pattern!
                 * */
                if (item is ILiteralStringReferenceProductionRuleItem)
                    this.dictionaryCopy.Add(item, new FlattenedRuleStringLiteralReferenceItem((ILiteralStringReferenceProductionRuleItem)item, SourceRoot, Root, this));
                else if (item is ILiteralCharReferenceProductionRuleItem)
                    this.dictionaryCopy.Add(item, new FlattenedRuleCharLiteralReferenceItem((ILiteralCharReferenceProductionRuleItem)item, SourceRoot, Root, this));
                else if (item is ITokenReferenceProductionRuleItem)
                    this.dictionaryCopy.Add(item, new FlattenedRuleTokenReferenceItem((ITokenReferenceProductionRuleItem)item, this.SourceRoot, this.Root, this));
                else if (item is IRuleReferenceProductionRuleItem)
                    this.dictionaryCopy.Add(item, new FlattenedRuleReferenceItem((IRuleReferenceProductionRuleItem)item, this.SourceRoot, this.Root, this));
                else if (item is IProductionRuleGroupItem)
                    this.dictionaryCopy.Add(item, new FlattenedRuleGroupItem((IProductionRuleGroupItem)item, SourceRoot, this, Root));
                else
                    Console.WriteLine("Error! {0} - Unknown type.",item.GetType().FullName);
            }
            foreach (var item in this.Values)
            {
                item.Initialize(tokenLookup, productionRules);
                //this.DataNode += item.DataNode;
                if (item is FlattenedRuleGroupItem)
                {
                    var gItem = item as FlattenedRuleGroupItem;
                    var gItemDataSet = gItem.DataSet;
                    if (!string.IsNullOrEmpty(gItem.Source.Name) && gItemDataSet.Count == 1 &&
                        gItemDataSet[0] is IRuleDataEnumItem)
                        this.DataSet.AddItem(((RuleDataEnumItem)gItemDataSet[0]).GetNamedForm(gItem.Source.Name));
                    else
                        this.DataSet.AddSet(gItem.DataSet);
                }
                else
                    this.DataSet.AddItem(item.DataItem);
            }
        }

        private void InitializeState()
        {
            //Overall states for each element in the expression.
            SimpleLanguageState[] states = new SimpleLanguageState[this.Count];
            Stack<SimpleLanguageState>[] edges = new Stack<SimpleLanguageState>[this.Count];
            SimpleLanguageState result = null;
            SimpleLanguageState lastState = null;
            IFlattenedRuleItem lastItem = null;
            ScannableEntryItemRepeatOptions[] repeatOptions = new ScannableEntryItemRepeatOptions[this.Count];
            bool[] optional = new bool[this.Count];
            if (this.Count == 0)
                return;
            for (int i = 0; i < this.Count; i++)
            {
                var currentItem = this.Values[i];
                if (currentItem is FlattenedRuleGroupItem)
                    ((FlattenedRuleGroupItem)(currentItem)).BuildState();
                var currentState =
                    states[i] = currentItem.State;
                var edgeSet =
                    edges[i] = new Stack<SimpleLanguageState>(currentState.ObtainEdges());
                /* *
                 * Mark all edges on the current state set to ensure that when
                 * obtaining all the edges, even after fusing certain sets together
                 * the end points are still known.
                 * *
                 * This ensures that later overlapping regions are still included for
                 * transition sets.  The marked edges are cleared if an element later
                 * in the set is non-optional and thus it is no longer a valid end point.
                 * */
                foreach (var edge in edgeSet)
                    edge.MakeEdge();
                if (edgeSet.Contains(currentState))
                    edges[i] = new Stack<SimpleLanguageState>(edgeSet.Where(p => p != currentState));
                repeatOptions[i] = currentItem.RepeatOptions;
                //Repeat options might not tell this properly.
                optional[i] = currentItem.Optional;
            }
            int previousLastRequired = -1;
            int lastRequiredIndex = -1;
            for (int i = 0; i < this.Count; i++)
            {
                SimpleLanguageState currentState = states[i];
                IFlattenedRuleItem currentItem = this.Values[i];
                try
                {
                    if (result == null)
                    {
                        result = currentState;
                        continue;
                    }
                    int backPoint = lastRequiredIndex == -1 ? 0 : lastRequiredIndex;
                    for (int k = backPoint; k < i; k++)
                    {
                        /* *
                         * Cause the current state to be repeated
                         * back through all the previous sets.
                         * */
                        foreach (var transition in currentState.OutTransitions)
                            foreach (var transitionState in transition.Targets)
                                switch (repeatOptions[k])
                                {
                                    case ScannableEntryItemRepeatOptions.None:
                                        foreach (var edge in edges[k])
                                            edge.MoveTo(transition.Check, transition.Targets, transition.Sources);
                                        if (optional[k])
                                            states[k].MoveTo(transition.Check, transition.Targets);
                                        break;
                                    case ScannableEntryItemRepeatOptions.ZeroOrOne:
                                        states[k].MoveToOptional(transition.Check, transitionState, edges[k].ToArray());
                                        states[k].OutTransitions.GetNode(transition.Check).Sources.AddRange(transition.Sources);
                                        break;
                                    case ScannableEntryItemRepeatOptions.ZeroOrMore:
                                        states[k].MoveToStar(transition.Check, transitionState, edges[k].ToArray());
                                        states[k].OutTransitions.GetNode(transition.Check).Sources.AddRange(transition.Sources);
                                        foreach (var edge in edges[k])
                                            edge.OutTransitions.GetNode(transition.Check).Sources.AddRange(transition.Sources);
                                        break;
                                    case ScannableEntryItemRepeatOptions.OneOrMore:
                                        states[k].MoveToAndStar(transition.Check, transitionState, edges[k].ToArray());
                                        foreach (var edge in edges[k])
                                            edge.OutTransitions.GetNode(transition.Check).Sources.AddRange(transition.Sources);
                                        if (optional[k])
                                            states[k].MoveTo(transition.Check, transition.Targets);
                                        break;
                                }
                    }
                    if (previousLastRequired != -1 || !optional[i])
                    {
                        backPoint = previousLastRequired == -1 ? 0 : previousLastRequired;
                        for (int k = backPoint; k < i; k++)
                        {
                            foreach (var edge in edges[k])
                                if (edge.IsMarked)
                                    edge.ClearEdge();
                        }
                        previousLastRequired = -1;
                    }
                }
                finally
                {
                    if (!optional[i])
                    {
                        previousLastRequired = lastRequiredIndex;
                        lastRequiredIndex = i;
                    }
                    lastState = currentState;
                    lastItem = currentItem;
                }
            }
            int lastIndex = repeatOptions.Length - 1;
            if (lastIndex != 0)
            {
                for (int q = lastIndex - 1; q > 0; q--)
                    if (!optional[q])
                    {
                        previousLastRequired = q;
                        break;
                    }
                int backIndex = (previousLastRequired == -1) ? lastIndex - 1 : previousLastRequired;
                for (int i = backIndex; i <= lastIndex; i++)
                {
                    switch (repeatOptions[lastIndex])
                    {
                        case ScannableEntryItemRepeatOptions.None:
                            //Ensure that it's marked as an edge.

                            //foreach (var edge in edges[lastIndex])
                            //    edge.MakeEdge();
                            if (optional[lastIndex])
                            {
                                foreach (var edge in edges[i])
                                    edge.MakeEdge();
                                if (i == backIndex)
                                    lastState.MakeEdge();
                            }
                            break;
                        case ScannableEntryItemRepeatOptions.ZeroOrOne:
                            foreach (var edge in edges[i])
                                edge.MakeEdge();
                            break;
                        case ScannableEntryItemRepeatOptions.ZeroOrMore:
                            foreach (var edge in edges[i])
                                edge.MakeEdge();
                            if (i == lastIndex)
                                lastState.Star();
                            break;
                        case ScannableEntryItemRepeatOptions.OneOrMore:
                            if (i == lastIndex)
                                lastState.RequiredStar();
                            if (optional[lastIndex])
                                lastState.MakeEdge();
                            break;
                    }
                    continue;
                }
            }
            else
            {
                if (optional[lastIndex])
                    lastState.MakeEdge();
                switch (repeatOptions[lastIndex])
                {
                    case ScannableEntryItemRepeatOptions.ZeroOrMore:
                        foreach (var edge in edges[lastIndex])
                            edge.MakeEdge();
                        lastState.Star();
                        break;
                    case ScannableEntryItemRepeatOptions.ZeroOrOne:
                    case ScannableEntryItemRepeatOptions.None:
                        foreach (var edge in edges[lastIndex])
                            edge.MakeEdge();
                        break;
                    case ScannableEntryItemRepeatOptions.OneOrMore:
                        lastState.RequiredStar();
                        break;
                }
            }
            bool noRequiredElements = true;
            for (int i = 0; i < optional.Length; i++)
                if (!optional[i])
                    noRequiredElements = false;
            if (!noRequiredElements)
            {
                noRequiredElements = true;
                for (int i = 0; i < states.Length; i++)
                    if (!states[i].IsEdge())
                        noRequiredElements = false;
            }
            if (noRequiredElements)
                result.MakeEdge();
            else if (result.IsEdge())
                result.ClearEdge();
            this.State = result;
        }

        internal void BuildState()
        {
            this.InitializeState();
        }
    }
}
