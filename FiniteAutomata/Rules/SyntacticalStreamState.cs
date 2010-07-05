using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.Builder;

namespace Oilexer.FiniteAutomata.Rules
{
    public partial class SyntacticalStreamState
    {
        #region SyntacticalStreamState data members
        //internal static ParserBuilder builder;
        private SourcesInfo sources;
        
        private static Dictionary<IProductionRuleEntry, SyntacticalStreamState> ruleCache = new Dictionary<IProductionRuleEntry, SyntacticalStreamState>();
        
        private static Dictionary<GrammarVocabulary, List<SyntacticalStreamState>> stateCache = new Dictionary<GrammarVocabulary, List<SyntacticalStreamState>>();
        
        private TransitionTable transitions;
        #endregion // SyntacticalStreamState data members
        #region SyntacticalStreamState properties
        internal SourcesInfo Sources
        {
            get
            {
                return this.sources;
            }
        }
        
        public TransitionTable Transitions
        {
            get
            {
                if (this.transitions == null)
                    this.transitions = new TransitionTable(this);
                return this.transitions;
            }
        }
        #endregion // SyntacticalStreamState properties
        #region SyntacticalStreamState methods
        private static void CalculateFollowSet(SourcesBuilder sources, List<StatePath> newlyAdded)
        {
            if (newlyAdded.Count == 0)
                return;
            List<StatePath> followUpSet = new List<StatePath>();
             /* ----------------------------------------------------------------------\
             |  This works by checking the currently active path nodes, when one      |
             |  is encountered as a terminal edge, peek above it and use its follow   |
             |  set.                                                                  |
             |------------------------------------------------------------------------|
             |  There can be more than one follow for a given path, because           |
             |  left-recursion introduces multiplicity on a rule.                     |
             \---------------------------------------------------------------------- */
            foreach (StatePath possibleTerminalPath in newlyAdded)
            {
                if (possibleTerminalPath.Original.IsEdge && (possibleTerminalPath.Rule.FollowSet.Count > 0))
                {
                    foreach (RulePath.FollowInfo currentFollowData in possibleTerminalPath.Rule.FollowSet)
                    {
                        StatePath newNode = new StatePath(currentFollowData.Follow, currentFollowData.Rule);
                        if (sources.AddFollow(newNode))
                            followUpSet.Add(newNode);
                    }
                }
            }
            CalculateFollowSet(sources, followUpSet);
        }
        
        private static void GetSourceSet(SourcesBuilder sources, StatePath currentPath)
        {
            SyntacticalDFAState originalState = currentPath.Original;
            RulePath currentRulePath = null;
            List<StatePath> newlyAdded = new List<StatePath>();
            if (currentPath.IsRule)
                currentRulePath = ((RulePath)(currentPath));
             /* ---------------------------------------------------------------\
             |  Gather the rule references at the current level and construct  |
             |  nodes relative to each item, delving further when necessary.   |
             \--------------------------------------------------------------- */
            foreach (IProductionRuleEntry id in from symbol in originalState.OutTransitions.FullCheck.Breakdown.Rules
                                                select symbol.Source)
            {
                RulePath firstNode;

                var ruleTransition = originalState.OutTransitions.GetNode(id);
                 /* ----------------------------------------------------------------------\
                 |  Left recursive rules are only evaluated this far, to ensure the       |
                 |  entire thing doesn't recurse infinitely.                              |
                 |------------------------------------------------------------------------|
                 |  The trick here is to check whether the current path contains the      |
                 |  rule already (from the last non-rule state only), if it does it       |
                 |  stores the source of origin and the follow state.                     |
                 |------------------------------------------------------------------------|
                 |  It doesn't do any good to include the left recursive path because     |
                 |  its children aren't evaluated and it won't persist through the rest   |
                 |  of the states.                                                        |
                 |------------------------------------------------------------------------|
                 |  Also, they wouldn't add anything to the first set, or the follow      |
                 |  set; the follow portion is taken care of by LineContains, which       |
                 |  adds the path information to its own follow information, effectively  |
                 |  as long as the follow states at the terminal points of that rule      |
                 |  persist, it's still recursing on the left side.                       |
                 \---------------------------------------------------------------------- */
                if ((currentRulePath != null) && currentRulePath.LineContains(id, currentRulePath, ruleTransition.Item3, 
                    currentPath))
                    goto continuePoint;
                 /* -------------------------------------------------------------\
                 |  Everything else is pretty simple.                            |
                 |  Create a node, mark the origin point, and the follow point.  |
                 |  Add if it doesn't exist.                                     |
                 \------------------------------------------------------------- */
                firstNode = new RulePath(ruleTransition.Item2, currentPath, currentPath.Rule, 
                    ruleTransition.Item3);
                if (sources.AddFirst(firstNode))
                {
                    newlyAdded.Add(firstNode);
                    GetSourceSet(sources, firstNode);
                }
            continuePoint:
                ;
            }
             /* ----------------------------------------------------\
             |  Include what comes after the current set of states  |
             |  when there's a terminal edge.                       |
             \---------------------------------------------------- */
            CalculateFollowSet(sources, newlyAdded);
        }
        
        private static void GetSourceSet(SourcesBuilder sources)
        {
            foreach (StatePath currentPath in sources.FullSet.ToArray())
                GetSourceSet(sources, currentPath);
        }
        
        public static SyntacticalStreamState ObtainRule(IProductionRuleEntry sourceId, ParserBuilder builder)
        {
            if (!(ruleCache.ContainsKey(sourceId)))
            {
                SyntacticalDFARootState source = null;
                source = builder.RuleDFAStates[sourceId];
                if (source == null)
                    return null;
                RulePath node = new RulePath(source);
                SourcesBuilder sources = new SourcesBuilder(new StatePath[]
                        {
                            node
                        });
                GetSourceSet(sources);
                SyntacticalStreamState result = new SyntacticalStreamState(sources.GetSources());
                ruleCache.Add(sourceId, result);
            }
            return ruleCache[sourceId];
        }
        
        private static SyntacticalStreamState ObtainState(StatePath[] pathSet, GrammarVocabulary transition)
        {
            // The target series of stream states to check against.
            List<SyntacticalStreamState> tryTarget;
            lock (stateCache)
            {
                if (!(stateCache.TryGetValue(transition, out tryTarget)))
                {
                    tryTarget = new List<SyntacticalStreamState>();
                    stateCache.Add(transition, tryTarget);
                }
            }
            SourcesBuilder sourceBuilder = new SourcesBuilder(pathSet);
            CalculateFollowSet(sourceBuilder, new List<StatePath>(sourceBuilder.FullSet));
            GetSourceSet(sourceBuilder);
            foreach (SyntacticalStreamState tryState in tryTarget)
            {
                 /* -------------------------------------------------------------------\
                 |  The length->count comparison should expedite the overall check     |
                 |  versus the full source count: this should be fractionally faster.  |
                 \------------------------------------------------------------------- */
                if ((tryState.sources.InitialSet.Length == sourceBuilder.InitialSet.Count) && ((tryState.sources.FirstSet.Length == sourceBuilder.FirstSet.Count) && (tryState.sources.FollowSet.Length == sourceBuilder.FollowSet.Count)))
                {
                    bool fullMatchFound = true;
                    for(int trySourceIndex = 0; (trySourceIndex < tryState.sources.Count); trySourceIndex++)
                    {
                        StatePath tryPath = tryState.sources[trySourceIndex];
                        bool matchFound = false;
                        for(int buildSourceIndex = 0; (buildSourceIndex < sourceBuilder.Count); buildSourceIndex++)
                        {
                            if (tryPath.Equals(sourceBuilder[buildSourceIndex]))
                            {
                                matchFound = true;
                                break;
                            }
                        }
                        if (!(matchFound))
                        {
                            fullMatchFound = false;
                            break;
                        }
                    }
                    if (fullMatchFound)
                        return tryState;
                }
            }
            SyntacticalStreamState result = new SyntacticalStreamState(sourceBuilder.GetSources());
            lock (tryTarget)
            {
                tryTarget.Add(result);
            }
            return result;
        }
        
        private static Dictionary<GrammarVocabulary, StatePath[]> GetLookAhead(SourcesInfo sources)
        {
            Dictionary<SyntacticalDFAState, List<StatePath>> groups = new Dictionary<SyntacticalDFAState, List<StatePath>>();
             /* -----------------------------------------------------------\
             |  Create a reverse lookup on the original sources.           |
             |-------------------------------------------------------------|
             |  Sometimes there might be as many as four to ten paths to   |
             |  a given state due to multiple rules targeting the same     |
             |  sub-rule.                                                  |
             |-------------------------------------------------------------|
             |  The actual look-ahead won't change, so there's no need to  |
             |  calculate the ahead for that state more than once, follow  |
             |  set discovery was completed before this stage.             |
             \----------------------------------------------------------- */
            foreach (KeyValuePair<SourcedFrom, StatePath> fromAndSource in sources)
            {
                SyntacticalDFAState originalSource = fromAndSource.Value.Original;
                if (!(groups.ContainsKey(originalSource)))
                    groups.Add(originalSource, new List<StatePath>());
                groups[originalSource].Add(fromAndSource.Value);
            }
            bool keysChanged = false;
            GrammarVocabulary[] transitionKeys = new GrammarVocabulary[0];
            Dictionary<GrammarVocabulary, BuildTransition> groupings = new Dictionary<GrammarVocabulary, BuildTransition>();
            foreach (SyntacticalDFAState currentSource in groups.Keys)
            {
                foreach (var currentTransitionInfo in currentSource.OutTransitions)
                {
                    SyntacticalDFAState transitionTarget = currentTransitionInfo.Value;
                    GrammarVocabulary sourceTransition = currentTransitionInfo.Key.GetTokenVariant();
                    if (sourceTransition.IsEmpty)
                        continue;
                    if (keysChanged)
                    {
                         /* ----------------------------------------\
                         |  This doesn't occur on cases where the   |
                         |  previous sourceTransition overlapped    |
                         |  perfectly on an existing entry.         |
                         \---------------------------------------- */
                        transitionKeys = Enumerable.ToArray<GrammarVocabulary>(groupings.Keys);
                        keysChanged = false;
                    }
                     /* ----------------------------------------------\
                     |  Breakdown the current transition against the  |
                     |  preexisting set.                              |
                     \---------------------------------------------- */
                    foreach (GrammarVocabulary transitionKey in transitionKeys)
                    {
                        GrammarVocabulary intersection = (transitionKey & sourceTransition);
                        if (!(intersection.IsEmpty))
                        {
                            GrammarVocabulary complement = transitionKey ^ sourceTransition;
                            sourceTransition = (sourceTransition & complement);
                            if (sourceTransition.IsEmpty && (transitionKey == intersection))
                            {
                                groupings[transitionKey].Add(new BuildTransition.ParentChildPairing(currentSource, transitionTarget));
                                goto nextTransitionInfo;
                            }
                            BuildTransition backup = groupings[transitionKey];
                            groupings.Remove(transitionKey);
                            BuildTransition newTransition = new BuildTransition(backup);
                            newTransition.Add(new BuildTransition.ParentChildPairing(currentSource, transitionTarget));
                            groupings.Add(intersection, newTransition);
                            GrammarVocabulary transitionRemainder = (transitionKey & complement);
                            if (!(transitionRemainder.IsEmpty))
                                groupings.Add(transitionRemainder, new BuildTransition(backup));
                            keysChanged = true;
                            if (sourceTransition.IsEmpty)
                                goto nextTransitionInfo;
                        }
                    }
                    if (!(sourceTransition.IsEmpty))
                    {
                        if (!(keysChanged))
                            keysChanged = true;
                        BuildTransition newTransition = new BuildTransition(currentSource, transitionTarget);
                        groupings.Add(sourceTransition, newTransition);
                    }
                nextTransitionInfo:
                    ;
                }
            }
             /* --------------------------------------------------------\
             |  After finding the look-ahead of the individual          |
             |  states involved, use the reverse lookup to construct    |
             |  the paths and build the final source look ahead table.  |
             |----------------------------------------------------------|
             |  Once the source table is built, the table works lazily  |
             |  and only constructs the individual union paths once a   |
             |  transition is requested.                                |
             \-------------------------------------------------------- */
            Dictionary<GrammarVocabulary, StatePath[]> result = new Dictionary<GrammarVocabulary, StatePath[]>();
            foreach (GrammarVocabulary groupTransition in Enumerable.OrderBy<GrammarVocabulary, string>(groupings.Keys, GrammarVocabulary.OrderingSelectorPointer))
            {
                List<StatePath> currentPaths = new List<StatePath>();
                BuildTransition currentTransition = groupings[groupTransition];
                foreach (BuildTransition.ParentChildPairing currentParentChildPair in currentTransition)
                {
                    // Here's where the reverse lookup comes in handy.
                    foreach (StatePath currentPath in groups[currentParentChildPair.Parent])
                    {
                        StatePath newPath = new StatePath(currentParentChildPair.Child, currentPath.Rule);
                        if (!(currentPaths.Contains(newPath)))
                            currentPaths.Add(newPath);
                    }
                }
                result.Add(groupTransition, currentPaths.ToArray());
            }
            return result;
        }
        #endregion // SyntacticalStreamState methods
        #region SyntacticalStreamState .ctors
        private SyntacticalStreamState(SourcesInfo sources)
        {
            this.sources = sources;
        }
        #endregion // SyntacticalStreamState .ctors
    }
}
