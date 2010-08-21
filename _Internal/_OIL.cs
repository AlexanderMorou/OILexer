#if x64
using SlotType = System.UInt64;
#elif x86
using SlotType = System.UInt32;
#endif
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CSharp;
using Oilexer;
using Oilexer.Types;
using Oilexer.Utilities.Collections;
using Oilexer.FiniteAutomata.Rules;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Inlining;
using Oilexer.FiniteAutomata.Tokens;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;


namespace Oilexer._Internal
{
    internal static partial class _OIL
    {
        public static string FixedJoinSeries(this string[] series, string jointElement, int maxWidth = 80)
        {
            int jointElementLength = jointElement.Length;
            int maxLengthAllowed = Math.Max(series.Max(element => element.Length), maxWidth) + jointElementLength;
            StringBuilder resultBuilder = new StringBuilder();
            bool firstElement = true;
            int currentLength = 0;
            int maxActual = maxLengthAllowed - jointElement.Length;
            foreach (var element in series)
            {
                if (firstElement)
                    firstElement = false;
                else
                {
                    resultBuilder.Append(jointElement);
                    currentLength += jointElementLength;
                }
                int newLength = currentLength + element.Length;
                if (newLength >= maxActual)
                {
                    resultBuilder.AppendLine();
                    currentLength = element.Length;
                }
                else
                    currentLength = newLength;
                resultBuilder.Append(element);
            }
            return resultBuilder.ToString();
        }
        public static string HTMLEncode(this string toEncode, bool encodeSpaces = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in toEncode)
            {
                switch (c)
                {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    case ' ':
                        if (encodeSpaces)
                            sb.Append("&nbsp;");
                        else
                            sb.Append(" ");
                        break;
                    default:
                        if (c <= 0xFF)
                            sb.Append(c);
                        else
                            sb.AppendFormat("&#{0:000#};", (int)c);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string Repeat(this char c, int length)
        {
            char[] result = new char[length];
            for (int i = 0; i < result.Length; i++)
                result[i] = c;
            return new string(result);
        }
        public static string Repeat(this string s, int length)
        {
            char[] result = new char[s.Length * length];
            for (int j = 0, k = 0; j < length; j++)
                for (int i = 0; i < s.Length; i++)
                    result[k++] = s[i];
            return new string(result);
        }

        internal unsafe static SlotType[] ObtainFiniteSeries(this BitArray characters, int FullSetLength)
        {
            if (characters == null)
                throw new ArgumentNullException("characters");
            else if (characters.Length > FullSetLength + 1)
                throw new ArgumentOutOfRangeException("characters");

            int[] values = new int[(int)Math.Ceiling(((double)(characters.Length)) / 32D)];
            characters.CopyTo(values, 0);
#if x86
            uint[] values2 = new uint[values.Length];
            for (int i = 0; i < values2.Length; i++)
                values2[i] = unchecked((uint)values[i]);
#elif x64
            ulong[] values2 = new ulong[(int)Math.Ceiling(((double)(values.Length)) / 2)];
            fixed (SlotType* values2ptr = values2)
            {
                uint* v2p = (uint*)values2ptr;
                fixed (int* valuesPtr = values)
                {
                    uint* vp = (uint*)valuesPtr;
                    for (int i = 0; i < values.Length; i++)
                    {
                        *v2p = *vp;
                        v2p++;
                        vp++;
                    }
                }
            }
#endif
            return values2;
        }

        private static bool DependsOn(this SyntacticalDFAState target, IProductionRuleEntry entry, List<SyntacticalDFAState> followed)
        {
            //Ensure that cyclic models don't recurse infinitely
            if (followed.Contains(target))
                return target.ContainsRule(entry);
            //Add the current element to the followed elements
            followed.Add(target);
            var stateTransitionUnion = target.OutTransitions.FullCheck;
            var breakdown = stateTransitionUnion.Breakdown;
            //Step through the rules within the state.
            var helper = new DependsOnPredicatedHelper();
            foreach (var rule in from rule in breakdown.Rules
                                 select rule.Source)
            {
                helper.rule = rule;
                SyntacticalDFARootState state = null;
                //Exit with true when found.
                if (rule == entry)
                    return true;
                //otherwise, if the root-state of the rule depends upon it...
                else if ((state = target[rule]).DependsOn(entry, followed))
                    return true;
                /* *
                 * ... in the event that the initial state of the rule is 
                 * an edge state, continue checking the state after that rule
                 * is called for, to ensure that the dependencies after that
                 * rule's reference point are considered.  i.e. a hidden
                 * dependency.
                 * */
                else if (state.CanBeEmpty && 
                    target.OutTransitions[target.OutTransitions.Keys.First(helper.rulePredicate)].DependsOn(entry, followed))
                    return true;
            }
            /* *
             * Same thing as the above, but only on the tokens
             * instead of the rules.
             * */
            foreach (var token in breakdown.Tokens)
            {
                helper.token = token as InlinedTokenEntry;
                if ((helper.token == null) ||
                    (helper.token.DFAState == null))
                    continue;
                if (helper.token.DFAState.IsEdge &&
                    target.OutTransitions[target.OutTransitions.Keys.First(helper.tokenPredicate)].DependsOn(entry, followed))
                    return true;
            }
            helper.token = null;
            helper.rule = null;
            return false;
        }

        public static bool DependsOn(this SyntacticalDFARootState target, IProductionRuleEntry entry)
        {
            return target.DependsOn(entry, new List<SyntacticalDFAState>());
        }

        private class DependsOnPredicatedHelper
        {
            internal IProductionRuleEntry rule;
            internal InlinedTokenEntry token;

            internal Func<GrammarVocabulary, bool> rulePredicate;
            internal Func<GrammarVocabulary, bool> tokenPredicate;

            public DependsOnPredicatedHelper()
            {
                this.rulePredicate = ContainsRule;
                this.tokenPredicate = ContainsToken;
            }

            public bool ContainsRule(GrammarVocabulary set)
            {
                return set.Contains(rule);
            }

            public bool ContainsToken(GrammarVocabulary set)
            {
                return set.Contains(token);
            }
        }

        private static IEnumerable<Match> AsEnumerable(this Match target)
        {
            while (target != null && target.Success)
            {
                yield return target;
                target = target.NextMatch();
            }
        }

        public static IEnumerable<Match> MatchSet(this Regex target, string text)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (text == null)
                throw new ArgumentNullException("text");
            return target.Match(text).AsEnumerable();
        }

    }
}
