using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Utilities.Collections;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Flatform.Tokens.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens
{
    internal class FlattenedTokenExpression :
        ControlledStateDictionary<ITokenItem, IFlattenedTokenItem>
    {
        private ITokenExpression source;
        public FlattenedTokenExpression(ITokenExpression source)
        {
            this.source = source;
        }

        internal void Initialize() 
        {
            //Console.WriteLine(this.source);
            for (int i = 0; i < this.source.Count; i++)
            {
                var current = this.source[i];
                IFlattenedTokenItem currentResult = null;
                if (current is ILiteralTokenItem ||
                    current is ILiteralReferenceTokenItem)
                    currentResult = new FlattenedTokenLiteralItem(current);
                else if (current is ITokenGroupItem)
                    currentResult = new FlattenedTokenGroupItem((ITokenGroupItem)current);
                else if (current is ICharRangeTokenItem)
                    currentResult = new FlattenedTokenCharRangeItem((ICharRangeTokenItem)current);
                else if (current is ITokenReferenceTokenItem)
                {
                    var currentItem = new TokenGroupItem(((ITokenReferenceTokenItem)current).Reference.FileName, ((ITokenReferenceTokenItem)current).Reference.Branches.ToArray(), current.Column, current.Line, current.Position);
                    currentItem.RepeatOptions = current.RepeatOptions;
                    currentResult = new FlattenedTokenGroupItem(currentItem);
                }
                else if (current is IScanCommandTokenItem)
                    currentResult = new FlattenedScanTokenCommandItem((IScanCommandTokenItem)(current));
                currentResult.Initialize();
                this.Add(current, currentResult);
            }
        }

        internal RegularLanguageState GetState()
        {
                   RegularLanguageState[] states        = new RegularLanguageState[this.Count];
            Stack<RegularLanguageState>[] edges         = new Stack<RegularLanguageState>[this.Count];
                   RegularLanguageState   result        = null;
                   RegularLanguageState   lastState     = null;
                    IFlattenedTokenItem   lastItem      = null;
        ScannableEntryItemRepeatOptions[] repeatOptions = new ScannableEntryItemRepeatOptions[this.Count];
                                   bool[] optional      = new bool[this.Count];

            if (this.Count == 0)
                return null;
            for (int i = 0; i < this.Count; i++)
            {
                var currentItem = this.Values[i];
                var currentState =
                    states[i] = currentItem.GetState();
                var edgeSet =
                    edges[i] = new Stack<RegularLanguageState>(currentState.ObtainEdges());
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
                    edges[i] = new Stack<RegularLanguageState>(edgeSet.Where(p => p != currentState));
                repeatOptions[i] = currentItem.RepeatOptions;
                optional[i] = currentItem.Optional;
            }
            int previousLastRequired = -1;
            int lastRequiredIndex = -1;
            for (int i = 0; i < this.Count; i++)
            {
                RegularLanguageState currentState = states[i];
                IFlattenedTokenItem currentItem = this.Values[i];
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
                                            edge.MoveTo(transition.Check, transition.Targets);
                                        if (optional[k])
                                            states[k].MoveTo(transition);
                                        break;
                                    case ScannableEntryItemRepeatOptions.ZeroOrOne:
                                        states[k].MoveToOptional(transition.Check, transitionState, edges[k].ToArray());
                                        break;
                                    case ScannableEntryItemRepeatOptions.ZeroOrMore:
                                        states[k].MoveToStar(transition.Check, transitionState, edges[k].ToArray());
                                        break;
                                    case ScannableEntryItemRepeatOptions.OneOrMore:
                                        states[k].MoveToAndStar(transition.Check, transitionState, edges[k]);
                                        if (optional[k])
                                            states[k].MoveTo(transition);
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
#if OLD_REGULAR_LANGUAGE_STATE
                        lastState.OptionalStarTransition();
#else
                        lastState.Star();
#endif
                        break;
                    case ScannableEntryItemRepeatOptions.ZeroOrOne:
                    case ScannableEntryItemRepeatOptions.None:
                        foreach (var edge in edges[lastIndex])
                            edge.MakeEdge();
                        break;
                    case ScannableEntryItemRepeatOptions.OneOrMore:
#if OLD_REGULAR_LANGUAGE_STATE
                        lastState.RequiredStarTransition();
#else
                        lastState.RequiredStar();
#endif
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
            return result;
        }

    }
}
