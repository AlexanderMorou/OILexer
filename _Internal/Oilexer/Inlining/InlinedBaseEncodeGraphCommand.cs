using AllenCopeland.Abstraction.Numerics;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    internal class InlinedBaseEncodeGraphCommand :
        BaseEncodeGraphCommand,
        IInlinedTokenItem
    {
        private RegularLanguageNFAState state;
        private Dictionary<EncodeBase, Func<int, NumericBase>> baseCreator = new Dictionary<EncodeBase, Func<int, NumericBase>>()
        { 
            { 
                EncodeBase.Octal, 
                o => new OctalBase(o) 
            },
            { 
                EncodeBase.Hexadecimal, 
                h => new HexadecimalBase(h) 
            },
            { 
                EncodeBase.Octodecimal, 
                od => new OctodecimalBase(od) 
            },
            { 
                EncodeBase.Septemvigesimal, 
                sv => new SeptemvigesimalBase(sv) 
            },
            { 
                EncodeBase.Sexatrigesimal, 
                stg => new SexatrigesimalBase(stg) 
            },
            { 
                EncodeBase.Sexagesimal, 
                sg => new SexagesimalBase(sg) 
            },
        };

        private enum EncodeBase
        {
            /// <summary>
            /// Base 8
            /// </summary>
            Octal = 8,
            /// <summary>
            /// Base 16
            /// </summary>
            Hexadecimal = 16,
            /// <summary>
            /// Base 18,
            /// </summary>
            Octodecimal = 18,
            /// <summary>
            /// Base 27
            /// </summary>
            Septemvigesimal = 27,
            /// <summary>
            /// Base 36
            /// </summary>
            Sexatrigesimal = 36,
            /// <summary>
            /// Base 60
            /// </summary>
            Sexagesimal = 60,
        };

        public InlinedBaseEncodeGraphCommand(IBaseEncodeGraphCommand source, OilexerGrammarTokens.NumberLiteral numericBase, IOilexerGrammarTokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(OilexerGrammarInliningCore.Inline(source.EncodeTarget, sourceRoot, root, oldNewLookup), numericBase, source.Digits, source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;

        }

        public InlinedBaseEncodeGraphCommand(IBaseEncodeGraphCommand source, OilexerGrammarTokens.StringLiteralToken stringBase, IOilexerGrammarTokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
            : base(OilexerGrammarInliningCore.Inline(source.EncodeTarget, sourceRoot, root, oldNewLookup), stringBase, source.Digits, source.Column, source.Line, source.Position)
        {
            this.Source = source;
            this.SourceRoot = sourceRoot;
            this.Root = root;
            this.RepeatOptions = source.RepeatOptions;
            this.Name = source.Name;

        }

        public IBaseEncodeGraphCommand Source { get; private set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarTokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        public IOilexerGrammarTokenEntry SourceRoot { get; private set; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="InlinedScanCommandTokenItem"/>.
        /// </summary>
        public InlinedTokenEntry Root { get; private set; }

        #region IInlinedTokenItem Members

        ITokenItem IInlinedTokenItem.Source
        {
            get { return this.Source; }
        }

        public RegularLanguageNFAState State
        {
            get
            {
                return this.state;
            }
        }

        private class AnyController :
            NumericBaseController
        {
            public AnyController(string baseEntities, bool caseSensitive) :
                base(baseEntities, caseSensitive)
            {
            }
        }

        private class AnyBase :
            NumericBase
        {
            private AnyController controller;
            public AnyBase(AnyController controller, BigInteger value)
                : base(controller, value)
            {
                this.controller = controller;
            }

            protected override NumericBase GetNew(BigInteger value)
            {
                return new AnyBase(this.controller, value);
            }
        }

        public void BuildState(Dictionary<ITokenSource, Captures.ICaptureTokenStructuralItem> sourceReplacementLookup)
        {
            /* *
             * ToDo: Analyze encodeTarget state and find the number of 
             * characters necessary to encode it in the target
             * base.
             * */
            var thisReplacement = sourceReplacementLookup.ContainsKey(this) ? (ITokenSource)(sourceReplacementLookup[this]) : (ITokenSource)this;
            InlinedTokenExpressionSeries encodeTarget = (InlinedTokenExpressionSeries)this.EncodeTarget;
            encodeTarget.BuildState(sourceReplacementLookup);
            var state = encodeTarget.State;
            List<RegularLanguageNFAState> originalStates = new List<RegularLanguageNFAState>();
            RegularLanguageNFAState.FlatlineState(state, originalStates);
            if (!originalStates.Contains(state))
                originalStates.Add(state);
            var onLookup = new Dictionary<RegularLanguageNFAState, RegularLanguageNFAState>();
            foreach (var originalState in originalStates)
                onLookup.Add(originalState, new RegularLanguageNFAState());
            Func<int, NumericBase> baseFactory = null;
            bool caseSensitive = false;
            if (this.NumericBase == null)
            {
                /* *
                 * User-supplied base.
                 * */
                var ac = new AnyController(this.StringBase.GetCleanValue(), !this.StringBase.CaseInsensitive);
                baseFactory = ab => new AnyBase(ac, ab);
                caseSensitive = !StringBase.CaseInsensitive;
            }
            else
            {
                var cleanValue = this.NumericBase.GetCleanValue();
                baseFactory = baseCreator[(EncodeBase)cleanValue];
                switch (cleanValue)
                {
                    case 8:
                    case 16:
                    case 18:
                    case 27:
                    case 36:
                        caseSensitive = false;
                        break;
                    case 60:
                        caseSensitive = true;
                        break;
                }
            }

            Parallel.ForEach(originalStates, originalState =>
            {
                var targetState = onLookup[originalState];
                targetState.SetSources(originalState.Sources.ToDictionary(k => k.Item1, v => v.Item2));
                CreateBaseEncodedGraph(targetState, originalState, onLookup, baseFactory, caseSensitive, Digits.GetCleanValue());
                targetState.IsEdge = originalState.IsEdge;
            });
            this.state = onLookup[state];
            this.state.HandleRepeatCycle<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource, RegularLanguageNFARootState, IInlinedTokenItem>(this, thisReplacement, OilexerGrammarInliningCore.TokenRootStateClonerCache, OilexerGrammarInliningCore.TokenStateClonerCache);
        }

        #endregion

        private static void CreateBaseEncodedGraph(RegularLanguageNFAState targetState, RegularLanguageNFAState originalState, Dictionary<RegularLanguageNFAState, RegularLanguageNFAState> oldNewLookup, Func<int, NumericBase> baseFactory, bool caseSensitive, int numChars)
        {
            /* *
             * If it's using a user-supplied base, '0' isn't always
             * correct, but it *will* always be one character.
             * */
            var zero = baseFactory(0).ToString()[0];
            foreach (var set in originalState.OutTransitions.Keys)
            {
                var targets = originalState.OutTransitions[set];
                var endTargets = (from s in targets
                                  select oldNewLookup[s]).ToArray();
                List<string> characters = null;
                Dictionary<char, List<string>> currentSubsets = new Dictionary<char, List<string>>();
                for (int i = (int)set.Offset; i < set.Offset + set.Length; i++)
                {
                    if (set[(uint)i])
                    {
                        var s = baseFactory(i);
                        string current = s.ToString();
                        if (numChars > current.Length)
                            /* *
                             * The math of the radix yields the firstSeries element within the
                             * set always carries a value of zero.  The headache of a 
                             * variable-length escape sequence is not worth it.
                             * */
                            current = zero.Repeat(numChars - current.Length) + current;
                        char first = current[0];
                        if (!currentSubsets.ContainsKey(first))
                            currentSubsets.Add(first, characters = new List<string>());
                        string substr = current.Substring(1);
                        if (!characters.Contains(substr))
                            characters.Add(substr);
                    }
                }
                Breakdown(targetState, currentSubsets, endTargets, caseSensitive);
            }
        }

        private static void Breakdown(RegularLanguageNFAState targetState, Dictionary<char, List<string>> currentSubsets, RegularLanguageNFAState[] endTargets, bool caseSensitive)
        {
            foreach (var c in currentSubsets.Keys)
            {
                var currentSet = currentSubsets[c];
                var transition = new RegularLanguageSet(caseSensitive, c);
                List<string> characters = null;
                Dictionary<char, List<string>> nextSubsets = new Dictionary<char, List<string>>();
                foreach (var str in currentSet)
                {
                    if (str == string.Empty)
                    {
                        foreach (var state in endTargets)
                            targetState.MoveTo(transition, state);
                    }
                    else
                    {
                        char first = str[0];
                        if (!nextSubsets.ContainsKey(first))
                            nextSubsets.Add(first, characters = new List<string>());
                        string substr = str.Substring(1);
                        if (!characters.Contains(substr))
                            characters.Add(substr);
                    }
                }
                if (nextSubsets.Count > 0)
                {
                    var nextSubstate = new RegularLanguageNFAState();
                    targetState.MoveTo(transition, nextSubstate);
                    Breakdown(nextSubstate, nextSubsets, endTargets, caseSensitive);
                }
            }
        }
    }
}
