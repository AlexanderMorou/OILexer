using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    internal static class InliningCore
    {

        internal static Func<RegularLanguageNFARootState, RegularLanguageNFARootState> TokenRootStateClonerCache = new Func<RegularLanguageNFARootState, RegularLanguageNFARootState>(CloneState);
        internal static Func<RegularLanguageNFAState> TokenStateClonerCache = new Func<RegularLanguageNFAState>(CloneState);
        internal static Func<SyntacticalNFARootState, SyntacticalNFARootState> ProductionRuleRootStateClonerCache = new Func<SyntacticalNFARootState, SyntacticalNFARootState>(CloneState);

        public static InlinedTokenEntry Inline(ITokenEntry entry)
        {
            if (entry is ITokenEofEntry)
                return new InlinedTokenEofEntry((ITokenEofEntry)entry);
            else
                return new InlinedTokenEntry(entry);
        }

        public static ITokenExpressionSeries Inline(ITokenExpressionSeries source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            return new InlinedTokenExpressionSeries(source, sourceRoot, root, oldNewLookup);
        }

        public static ITokenExpression[] Inline(ITokenExpression[] source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            int c = 0;
            for (int i = 0; i < source.Length; i++)
                if (source[i].Count > 0)
                    c++;
            ITokenExpression[] result = new ITokenExpression[c];
            for (int i = 0, index = 0; i < source.Length; i++)
                if (source[i].Count > 0)
                    result[index++] = Inline(source[i], sourceRoot, root, oldNewLookup);
            return result;
        }

        public static ITokenExpression Inline(ITokenExpression source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            return new InlinedTokenExpression(source, sourceRoot, root, oldNewLookup);
        }

        public static Collection<ITokenItem> Inline(IControlledCollection<ITokenItem> source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            Collection<ITokenItem> result = new Collection<ITokenItem>();
            foreach (var item in source)
            {
                var currentSet = new List<ITokenItem>();
                currentSet.Add(item);
                for (int i = 0; i < currentSet.Count; i++)
                {
                    var subItem = currentSet[i];
                    if (subItem is ITokenGroupItem && subItem.RepeatOptions == ScannableEntryItemRepeatInfo.None && string.IsNullOrEmpty(subItem.Name) &&
                        ((ITokenGroupItem)(subItem)).Count == 1)
                    {
                        int j = i;
                        foreach (var element in ((ITokenGroupItem)(subItem))[0])
                            currentSet.Insert(++j, element);
                        currentSet.Remove(currentSet[i]);
                        i--;
                        continue;
                    }
                }
                foreach (var subItem in currentSet)
                {
                    result.Add(Inline(subItem, sourceRoot, root, oldNewLookup));
                }
            }
            return result;
        }

        public static ITokenItem Inline(ITokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            if (source is ITokenGroupItem)
            {
                var kSource = ((ITokenGroupItem)(source));
                if (kSource.Count == 1 && kSource[0].Count == 1)
                {
                    var lSource = kSource[0][0];
                    if (lSource.RepeatOptions == ScannableEntryItemRepeatInfo.None ||
                        kSource.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                    {
                        var result = Inline(lSource, sourceRoot, root, oldNewLookup);
                        if (lSource.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                            result.RepeatOptions = kSource.RepeatOptions;
                        else if (kSource.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                            result.RepeatOptions = lSource.RepeatOptions;
                        if (string.IsNullOrEmpty(kSource.Name))
                            result.Name = lSource.Name;
                        else if (string.IsNullOrEmpty(lSource.Name))
                            result.Name = kSource.Name;
                        return result;
                    }
                }
                return Inline((ITokenGroupItem)source, sourceRoot, root, oldNewLookup);
            }
            else if (source is ILiteralCharTokenItem)
                return Inline((ILiteralCharTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is ILiteralStringTokenItem)
                return Inline((ILiteralStringTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is ICommandTokenItem)
                return Inline((ICommandTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is ITokenReferenceTokenItem)
            {
                var kSource = (ITokenReferenceTokenItem)source;
                var rSource = kSource.Reference;
                if (rSource.Branches.Count == 1 && rSource.Branches[0].Count == 1)
                {
                    var lSource = rSource.Branches[0][0];
                    if (lSource.RepeatOptions == ScannableEntryItemRepeatInfo.None ||
                        kSource.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                    {
                        var result = Inline(lSource, sourceRoot, root, oldNewLookup);
                        if (lSource.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                            result.RepeatOptions = kSource.RepeatOptions;
                        else if (kSource.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                            result.RepeatOptions = lSource.RepeatOptions;
                        if (string.IsNullOrEmpty(kSource.Name))
                            result.Name = lSource.Name;
                        else if (string.IsNullOrEmpty(lSource.Name))
                            result.Name = kSource.Name;
                        return result;
                    }
                }
                return Inline((ITokenReferenceTokenItem)source, sourceRoot, root, oldNewLookup);
            }
            else if (source is ICharRangeTokenItem)
                return Inline((ICharRangeTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is ILiteralStringReferenceTokenItem)
                return Inline((ILiteralStringReferenceTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is ILiteralCharReferenceTokenItem)
                return Inline((ILiteralCharReferenceTokenItem)source, sourceRoot, root, oldNewLookup);
            throw new NotImplementedException("Not supported");
        }

        public static IInlinedTokenItem Inline(ITokenReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedTokenReferenceTokenItem(source, sourceRoot, root, oldNewLookup);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ICharRangeTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedCharRangeTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ITokenGroupItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedTokenGroupItem(source, sourceRoot, root, oldNewLookup);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ILiteralStringTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedLiteralStringTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ILiteralCharTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedLiteralCharTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ILiteralCharReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedLiteralCharReferenceTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ILiteralStringReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedLiteralStringReferenceTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ICommandTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            if (source is IScanCommandTokenItem)
                return Inline((IScanCommandTokenItem)(source), sourceRoot, root, oldNewLookup);
            else if (source is ISubtractionCommandTokenItem)
                return Inline((ISubtractionCommandTokenItem)(source), sourceRoot, root, oldNewLookup);
            return null;
        }

        private static IInlinedTokenItem Inline(ISubtractionCommandTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedSubtractionCommandTokenItem(source, sourceRoot, root, oldNewLookup);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(IScanCommandTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedScanCommandTokenItem(source, sourceRoot, root, oldNewLookup);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static TState CloneState<TCheck, TState, TDFA, TSourceElement, TRootState>(TState state, Func<TRootState, TRootState> rootCtor, Func<TState> cCtor)
            where TCheck :
                IFiniteAutomataSet<TCheck>,
                new()
            where TState :
                NFAState<TCheck, TState, TDFA, TSourceElement>
            where TDFA :
                FiniteAutomataState<TCheck, TDFA, TDFA, TSourceElement>,
                IDFAState<TCheck, TDFA, TSourceElement>,
                new()
            where TSourceElement :
                IFiniteAutomataSource
            where TRootState :
                TState
        {
            Dictionary<TState, TState> cloneSet = new Dictionary<TState, TState>();
            return CloneState<TCheck, TState, TDFA, TSourceElement, TRootState>(cloneSet, state, rootCtor, cCtor);
        }

        private static TState CloneState<TCheck, TState, TDFA, TSourceElement, TRootState>(Dictionary<TState, TState> cloneSet, TState state, Func<TRootState, TRootState> rootCtor, Func<TState> cCtor)
            where TCheck :
                IFiniteAutomataSet<TCheck>,
                new()
            where TState :
                NFAState<TCheck, TState, TDFA, TSourceElement>
            where TDFA :
                FiniteAutomataState<TCheck, TDFA, TDFA, TSourceElement>,
                IDFAState<TCheck, TDFA, TSourceElement>,
                new()
            where TSourceElement :
                IFiniteAutomataSource
            where TRootState :
                TState
        {
            if (!cloneSet.ContainsKey(state))
            {
                TState newState = state is TRootState ? rootCtor((TRootState)state) : cCtor();
                if (state.IsMarked)
                    state.IsEdge = true;
                cloneSet.Add(state, newState);
                foreach (var transition in state.OutTransitions.Keys)
                    foreach (var transitionTarget in state.OutTransitions[transition])
                        newState.MoveTo(transition, CloneState<TCheck, TState, TDFA, TSourceElement, TRootState>(cloneSet, transitionTarget, rootCtor, cCtor));
            }
            return cloneSet[state];
        }

        public static RegularLanguageNFARootState CloneState(RegularLanguageNFARootState root)
        {
            return new RegularLanguageNFARootState(root.Source);
        }

        public static RegularLanguageNFAState CloneState()
        {
            return new RegularLanguageNFAState();
        }

        public static SyntacticalNFARootState CloneState(SyntacticalNFARootState root)
        {
            return new SyntacticalNFARootState(root.Source, root.builder);
        }

        public static void HandleRepeatCycle<TCheck, TState, TDFA, TSourceElement, TRootState, TSubSourceElement>(this TState state, TSubSourceElement item, Func<TRootState, TRootState> rootCtor, Func<TState> cCtor)
            where TCheck :
                IFiniteAutomataSet<TCheck>,
                new()
            where TState :
                NFAState<TCheck, TState, TDFA, TSourceElement>
            where TDFA :
                FiniteAutomataState<TCheck, TDFA, TDFA, TSourceElement>,
                IDFAState<TCheck, TDFA, TSourceElement>,
                new()
            where TSourceElement :
                IFiniteAutomataSource
            where TRootState :
                TState
            where TSubSourceElement :
                TSourceElement,
                IScannableEntryItem
        {
            switch (item.RepeatOptions.Options)
            {
                case ScannableEntryItemRepeatOptions.None:
                    return;
                case ScannableEntryItemRepeatOptions.ZeroOrOne:
                    state.IsEdge = true;
                    break;
                case ScannableEntryItemRepeatOptions.ZeroOrMore:
                    {
                        var edges = state.ObtainEdges().ToArray();
                        state.IsEdge = true;
                        foreach (var edge in edges)
                        {
                            edge.SetRepeat(item);
                            foreach (var transition in state.OutTransitions.Keys)
                                foreach (var transitionTarget in state.OutTransitions[transition])
                                {
                                    edge.IsEdge = true;
                                    edge.MoveTo(transition, transitionTarget);
                                }
                        }
                    }
                    break;
                case ScannableEntryItemRepeatOptions.OneOrMore:
                    {
                        var edges = state.ObtainEdges().ToArray();
                        foreach (var edge in edges)
                        {
                            edge.SetRepeat(item);
                            foreach (var transition in state.OutTransitions.Keys)
                            {
                                var currentTargets = state.OutTransitions[transition];
                                foreach (var transitionTarget in currentTargets)
                                {
                                    edge.IsEdge = true;
                                    edge.MoveTo(transition, transitionTarget);
                                }
                            }
                        }
                    }
                    break;
                case ScannableEntryItemRepeatOptions.Specific:
                    TState[] set = null;
                    int count = 0;
                    int edgeItem;
                    state.SetInitial(item);
                    if (item.RepeatOptions.Min == null)
                    {
                        state.IsEdge = true;
                        count = item.RepeatOptions.Max.Value;
                        edgeItem = 0;
                    }
                    else
                    {
                        if (item.RepeatOptions.Max == null)
                            count = item.RepeatOptions.Min.Value + 1;
                        else
                            count = item.RepeatOptions.Max.Value;
                        edgeItem = item.RepeatOptions.Min.Value;
                    }
                    set = new TState[count];
                    for (int i = 0; i < count; i++)
                    {
                        if (i == 0)
                            set[0] = state;
                        else
                            set[i] = CloneState<TCheck, TState, TDFA, TSourceElement, TRootState>(state, rootCtor, cCtor);
                    }
                    for (int i = edgeItem; i < count; i++)
                    {
                        var setEdges = set[i].ObtainEdges().ToArray();
                        foreach (var edge in setEdges)
                        {
                            edge.IsEdge = true;
                            edge.SetFinal(item);
                        }
                        set[i].IsEdge = true;
                    }
                    for (int i = count - 1; i > 0; i--)
                        set[i - 1].Concat(set[i]);

                    break;
                default:
                    break;
            }
        }

        public static RegularLanguageNFAState BuildStringState(this IInlinedTokenItem target, bool caseInsensitive, string value)
        {
            RegularLanguageNFAState root = new RegularLanguageNFAState();
            RegularLanguageNFAState current = root;

            foreach (var c in value)
            {
                RegularLanguageNFAState nextState = new RegularLanguageNFAState();
                current.MoveTo(new RegularLanguageSet(!caseInsensitive, c), nextState);
                current = nextState;
            }
            List<RegularLanguageNFAState> flatline = new List<RegularLanguageNFAState>();
            RegularLanguageNFAState.FlatlineState(root, flatline);
            foreach (var fState in flatline)
                fState.SetIntermediate(target);
            root.SetInitial(target);
            current.SetFinal(target);
            return root;
        }
    }
}
