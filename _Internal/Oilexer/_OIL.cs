#if x64
using SlotType = System.UInt64;
#elif x86
#if HalfWord
using SlotType = System.UInt16;
#else
using SlotType = System.UInt32;
#endif
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
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
//using Oilexer.Types;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Parsers;
using AllenCopeland.Abstraction.Slf.Cst;
using AllenCopeland.Abstraction.Slf.Abstract;
using System.Globalization;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */


namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    internal static partial class _OIL
    {
        public static string FixedJoinSeries(this string[] series, string jointElement, int maxWidth = 80)
        {
            int jointElementLength = jointElement.Length;
            int seriesMax = 0;
            if (series.Length > 0)
                seriesMax = series.Max(element => element.Length);
            int maxLengthAllowed = Math.Max(seriesMax, maxWidth) + jointElementLength;
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

        private static bool DependsOn(this SyntacticalDFAState target, IOilexerGrammarProductionRuleEntry entry, List<SyntacticalDFAState> followed)
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

        public static bool DependsOn(this SyntacticalDFARootState target, IOilexerGrammarProductionRuleEntry entry)
        {
            return target.DependsOn(entry, new List<SyntacticalDFAState>());
        }

        private class DependsOnPredicatedHelper
        {
            internal IOilexerGrammarProductionRuleEntry rule;
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


        public static T ObtainCILibraryType<T>(this Type t, IIdentityManager identityManager)
            where T :
                IType
        {
            if (t.Assembly != typeof(int).Assembly)
                throw new ArgumentException();
            return (T)identityManager.ObtainTypeReference(identityManager.ObtainTypeReference(RuntimeCoreType.RootType).Assembly.UniqueIdentifier.GetTypeIdentifier(t.Namespace, t.Name));
        }

        public static string GetDocComment(this IOilexerGrammarProductionRuleEntry entry)
        {
            StringBuilder s = new StringBuilder();
            if (entry == null ||
                entry.PreexpansionText == null)
                return string.Empty;

            s.Append(entry.PreexpansionText.Replace('\x20', '\xA0'));
            return s.ToString();
        }

        public static string GetDocComment(this IOilexerGrammarTokenEntry entry)
        {
            StringBuilder s = new StringBuilder();
            if (entry == null || entry is IOilexerGrammarTokenEofEntry)
                return string.Empty;

            s.Append(entry.ToString().Replace('\x20', '\xA0'));
            return s.ToString();
        }

#if WINDOWS

        public static string GetFilenameProperCasing(this string filename)
        {
            try
            {
                var dirInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
                var files = dirInfo.GetFiles(Path.GetFileName(filename));
                if (files.Length > 0)
                    return Path.Combine(files[0].DirectoryName.GetDirectoryProperCasing(), files[0].Name);
                return filename;
            }
            catch
            {
                return filename.ToLower();
            }
        }
        public static string GetDirectoryProperCasing(this string path)
        {
            try
            {
                var attrs = File.GetAttributes(path);
                if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var parent = path + @"\..\";
                    var dirName = Path.GetFileName(path);
                    var parentAdjust = Path.GetFullPath(parent);
                    if (parentAdjust.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        parentAdjust = parentAdjust.Substring(0, parentAdjust.Length - 1);
                    /* *
                     * Once we've reached the end of the line.
                     * */
                    if (parentAdjust == path)
                        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(path) + Path.DirectorySeparatorChar;
                    else
                    {
                        var previous = GetDirectoryProperCasing(parentAdjust);
                        DirectoryInfo di = new DirectoryInfo(previous);
                        return Path.Combine(previous, di.GetDirectories(dirName)[0].Name);
                    }

                }
                else
                    throw new ArgumentException("path");
            }
            catch
            {
                /* *
                 * In the case of a general failure, yield the lower-case version
                 * of the filename.  Likely caused by permissions issues.
                 * */
                return path.ToLower();
            }
        }

#endif

    }
}
