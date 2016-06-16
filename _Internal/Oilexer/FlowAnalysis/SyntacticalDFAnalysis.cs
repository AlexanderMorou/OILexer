using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.FlowAnalysis
{
    public static class SyntacticalDFAnalysis
    {
        public static List<List<Tuple<GrammarVocabulary,SyntacticalDFAState>>> FindNonRecursiveAvenues(SyntacticalDFAState startState)
        {
            return EvaluateSyntacticalDFAState(GrammarVocabulary.NullInst, startState, new List<Tuple<GrammarVocabulary, SyntacticalDFAState>>()).ToList();
        }

        private static IEnumerable<List<Tuple<GrammarVocabulary, SyntacticalDFAState>>> EvaluateSyntacticalDFAState(GrammarVocabulary currentTransition, SyntacticalDFAState currentState, List<Tuple<GrammarVocabulary, SyntacticalDFAState>> currentPath)
        {
            if (currentPath.Any(k => k.Item2 == currentState))
                yield break;
            var myNode = Tuple.Create(currentTransition, currentState);
            currentPath.Add(myNode);
            if (currentState.IsEdge)
                yield return new List<Tuple<GrammarVocabulary, SyntacticalDFAState>>(currentPath);
            foreach (var transition in currentState.OutTransitions.Keys)
            {
                var currentTarget = currentState.OutTransitions[transition];
                foreach (var path in EvaluateSyntacticalDFAState(transition, currentTarget, currentPath))
                    yield return path;
            }
            currentPath.Remove(myNode);
        }
        public static List<List<Tuple<GrammarVocabulary, SyntacticalDFAState>>> FindRecursiveAvenues(SyntacticalDFAState startState)
        {
            return FindRecursiveAvenues(FindNonRecursiveAvenues(startState));
        }

        public static List<List<Tuple<GrammarVocabulary, SyntacticalDFAState>>> FindRecursiveAvenues(List<List<Tuple<GrammarVocabulary, SyntacticalDFAState>>> nonRecursiveAvenues)
        {
            return FindRecursiveAvenuesInternal(nonRecursiveAvenues).Distinct(ListComparer.Singleton).ToList();
        }

        private static IEnumerable<List<Tuple<GrammarVocabulary, SyntacticalDFAState>>> FindRecursiveAvenuesInternal(List<List<Tuple<GrammarVocabulary, SyntacticalDFAState>>> nonRecursiveAvenues)
        {
            foreach (var path in nonRecursiveAvenues)
            {
                int offset = 0;
                foreach (var node in path)
                {
                    offset++;
                    foreach (var transition in node.Item2.OutTransitions.Keys)
                    {
                        var transitionTarget = node.Item2.OutTransitions[transition];
                        if (path.Take(offset).Any(k => k.Item2 == transitionTarget))
                        {
                            var endNode = Tuple.Create(transition, transitionTarget);
                            var newPath = new List<Tuple<GrammarVocabulary, SyntacticalDFAState>>(path.Take(offset));
                            newPath.Add(endNode);
                            yield return newPath;
                        }
                    }
                }
            }
            yield break;
        }

        internal class ListComparer :
            IEqualityComparer<List<Tuple<GrammarVocabulary, SyntacticalDFAState>>>
        {
            private ListComparer() { }

            public static readonly ListComparer Singleton = new ListComparer();

            public bool Equals(List<Tuple<GrammarVocabulary, SyntacticalDFAState>> x, List<Tuple<GrammarVocabulary, SyntacticalDFAState>> y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(List<Tuple<GrammarVocabulary, SyntacticalDFAState>> obj)
            {
                return obj.Select(k => k.GetHashCode()).Aggregate((a, b) => a ^ b);
            }
        }
    }
}
