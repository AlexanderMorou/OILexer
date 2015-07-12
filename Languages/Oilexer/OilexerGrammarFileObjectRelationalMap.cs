using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Ast.Members;
/*---------------------------------------------------------------------\
| Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    using RuleTree = KeyedTree<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using RuleTreeNode = KeyedTreeNode<IOilexerGrammarScannableEntry, IOilexerGrammarScannableEntryObjectification>;
    using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
    using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
    using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
    using AllenCopeland.Abstraction.Slf.Ast.Expressions;
    using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
using AllenCopeland.Abstraction.Slf.Ast.Cli;

    internal class OilexerGrammarFileObjectRelationalMap :
        ControlledDictionary<IOilexerGrammarScannableEntry, IEntryObjectRelationalMap>,
        IOilexerGrammarFileObjectRelationalMap
    {
        private MultikeyedDictionary<IOilexerGrammarProductionRuleEntry, IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember> casesLookup;
        private RootRuleBuilder rootRuleBuilder;
        private List<ITokenEntryObjectRelationalMap> tokensToMap = new List<ITokenEntryObjectRelationalMap>();
        private IIntermediateAssembly project;
        private RuleTree _ruleVariants;

        public RuleTree ImplementationDetails { get { return this._ruleVariants; } }

        internal class ScannableEntryObjectification :
            IOilexerGrammarScannableEntryObjectification
        {
            public ScannableEntryObjectification(IIntermediateInterfaceType @interface, IIntermediateClassType @class, IOilexerGrammarScannableEntry entry)
            {
                this.Class = @class;
                this.RelativeInterface = @interface;
                this.Entry = entry;
            }

            public IIntermediateInterfaceType RelativeInterface { get; private set; }
            public IIntermediateClassType Class { get; private set; }
            public IOilexerGrammarScannableEntry Entry { get; private set; }
        }

        public void ConnectConstructs(IControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> ruleStates)
        {
            var rules = (from r in Source
                         let rule = r as IOilexerGrammarProductionRuleEntry
                         where rule != null
                         orderby rule.Name
                         select rule).ToArray();
            var tokens = (from t in Source
                          let token = t as InlinedTokenEntry
                          where token != null
                          orderby token.Name
                          select token).ToArray();
            var tokenAsRule = new InlinedTokenEntry(new OilexerGrammarTokenEntry("TokenDerived", new TokenExpressionSeries(new ITokenExpression[0], 0, 0, 0, string.Empty), EntryScanMode.Inherited, string.Empty, 0, 0, 0, false, new IOilexerGrammarTokenEntry[0], false), this.rootRuleBuilder.Compiler.Source);

            var tokenDependencyGraph = (from tokenPrimary in tokens
                                        from ruleSecondary in rules
                                        where ruleSecondary.IsRuleCollapsePoint
                                        let secondaryState = ruleStates[ruleSecondary]
                                        where secondaryState.OutTransitions.FullCheck.Breakdown.Tokens.Any(k => k == tokenPrimary)
                                        group ruleSecondary by tokenAsRule).ToDictionary(k => k.Key, v => v.Distinct().ToArray());
            var actualTokenGraph = (from tokenPrimary in tokens
                                    from ruleSecondary in rules
                                    where ruleSecondary.IsRuleCollapsePoint
                                    let secondaryState = ruleStates[ruleSecondary]
                                    where secondaryState.OutTransitions.FullCheck.Breakdown.Tokens.Any(k => k == tokenPrimary)
                                    group ruleSecondary by tokenPrimary).Select(t => t.Key).Distinct().ToArray();
            this.rootRuleBuilder.Compiler.TokensCastAsRules = actualTokenGraph.ToList();
            this.rootRuleBuilder.Compiler.TokenCastAsRule = tokenAsRule;
            if (tokenDependencyGraph.ContainsKey(tokenAsRule))
            {
                var tokenORM = new TokenEntryObjectRelationalMap(tokenDependencyGraph[tokenAsRule], this, tokenAsRule);
                this._Add(tokenAsRule, tokenORM);
                RuleTreeNode startingNode = CheckRootVariant(project, _ruleVariants, tokenAsRule);
                var tokenProp = startingNode.Value.RelativeInterface.Properties.Add(new TypedName("Token", this.rootRuleBuilder.Compiler.TokenSymbolBuilder.ILanguageToken), true, false);
                var tokenPropImpl = startingNode.Value.Class.Properties.Add(new TypedName("Token", this.rootRuleBuilder.Compiler.TokenSymbolBuilder.ILanguageToken), true, false);
                tokenPropImpl.AccessLevel = AccessLevelModifiers.Public;
                tokenPropImpl.GetMethod.Return(this.rootRuleBuilder.ContextImpl.GetReference().GetIndexer(IntermediateGateway.NumberZero).Cast(tokenProp.PropertyType));
                this.rootRuleBuilder.Compiler.RootRuleBuilder.TokenDerived_Token = tokenPropImpl;
                var reorm = (this[tokenAsRule] as ITokenEntryObjectRelationalMap);

                ((TokenEntryObjectRelationalMap)reorm).ImplementationDetails = startingNode;
                var variations = (from variation in reorm.Variations
                                  select variation.ToArray()).ToArray();

                foreach (var variation in variations)
                {
                    var currentNode = startingNode;
                    foreach (var variationElement in variation.Skip(1) /* First is always the current. */)
                    {
                        var currentRootVariant = CheckRootVariant(project, _ruleVariants, variationElement);
                        currentNode = BuildVariation(currentNode, currentRootVariant, project, tokenPropImpl);
                    }
                }
            }

        }

        public OilexerGrammarFileObjectRelationalMap(IOilexerGrammarFile source, IControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> ruleStates, IIntermediateAssembly project, RootRuleBuilder ruleBuilder)
        {
            this.Source = source;
            this.rootRuleBuilder = ruleBuilder;
            this.Process(ruleStates, project);
        }

        private IEnumerable<T> FilterScannable<T>(IOilexerGrammarFile target)
            where T :
                class,
                IOilexerGrammarScannableEntry
        {
            return (from t in target
                    let tItem = t as T
                    where tItem != null
                    orderby tItem.Name
                    select tItem);
        }

        private void Process(IControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> ruleStates, IIntermediateAssembly project)
        {
            const string defaultNamespaceSubspace = "Cst";
            this.project = project;
            var dNamespace = project.DefaultNamespace;
            var dNamespaceName = dNamespace.FullName;
            string subspaceName = string.Format("{0}.{1}", dNamespaceName, defaultNamespaceSubspace);
            if (dNamespace.Namespaces.PathExists(subspaceName))
                dNamespace = project.Namespaces[subspaceName];
            else
                dNamespace = project.Namespaces.Add(subspaceName);
            var rules = (from r in Source
                         let rule = r as IOilexerGrammarProductionRuleEntry
                         where rule != null
                         orderby rule.Name
                         select rule).ToArray();
            //#if false
            var ruleDependencyGraph = (from rulePrimary in rules
                                       from ruleSecondary in rules
                                       where ruleSecondary.IsRuleCollapsePoint
                                       let secondaryState = ruleStates[ruleSecondary]
                                       where secondaryState.OutTransitions.FullCheck.Breakdown.Rules.Any(p => p.Source == rulePrimary)
                                       group ruleSecondary by rulePrimary).ToDictionary(k => k.Key, v => v.ToArray());
            RuleTree ruleVariants = new RuleTree();
            this._ruleVariants = ruleVariants;
            this.casesLookup = new MultikeyedDictionary<IOilexerGrammarProductionRuleEntry, IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember>();

            IOilexerGrammarProductionRuleEntry[] emptySet = new IOilexerGrammarProductionRuleEntry[0];
            foreach (var rule in from rule in rules
                                 where !ruleDependencyGraph.Any(dependency => dependency.Key == rule)
                                 select rule)
            {
                IRuleEntryObjectRelationalMap ruleORM;
                if (rule.IsRuleCollapsePoint)
                {
                    var casesEnum = dNamespace.Parts.Add().Enums.Add(string.Format("{0}{1}{2}Cases", this.Source.Options.RulePrefix, rule.Name, this.Source.Options.RuleSuffix));
                    casesEnum.SummaryText = string.Format("Denotes the specific sub-rule that is condensed into the current @s:I{0}{1}{2};.", this.Source.Options.RulePrefix, rule.Name, this.Source.Options.RuleSuffix);
                    casesEnum.RemarksText = string.Format("@para;'.{0}' at: line {1}, column {2}@/para;\r\n@code language=oilexer;{3}@/code;", rule.FileName.Substring(Source.RelativeRoot.Length), rule.Line, rule.Column, rule.GetDocComment());
                    casesEnum.AccessLevel = AccessLevelModifiers.Public;
                    var breakdown = ruleStates[rule].OutTransitions.FullCheck.Breakdown;
                    foreach (var subRule in breakdown.Rules)
                    {
                        var localizedReference = (from src in ruleStates[rule].Sources
                                                  where src.Item1 is IRuleReferenceProductionRuleItem
                                                  let ruleRef = (IRuleReferenceProductionRuleItem)src.Item1
                                                  where ruleRef.Reference == subRule.Source
                                                  select ruleRef).FirstOrDefault();
                        LineColumnPair lcp = localizedReference == null ? LineColumnPair.Zero : new LineColumnPair(localizedReference.Line, localizedReference.Column);
                        var currentField = casesEnum.Fields.Add(subRule.Source.Name);
                        currentField.SummaryText = string.Format("The @s:I{0}{1}{2}; is an @s:I{0}{3}{2}; instance.", this.Source.Options.RulePrefix, rule.Name, this.Source.Options.RuleSuffix, subRule.Source.Name);
                        currentField.RemarksText = string.Format("Line {0}, column {1}", lcp.Line, lcp.Column);
                        casesLookup.Add(rule, subRule.Source, currentField);
                    }
                    foreach (var token in breakdown.Tokens)
                    {
                        var localizedReference = (from src in ruleStates[rule].Sources
                                                  where src.Item1 is ITokenReferenceProductionRuleItem
                                                  let tokenRef = (ITokenReferenceProductionRuleItem)src.Item1
                                                  where tokenRef.Reference == token
                                                  select tokenRef).FirstOrDefault();
                        LineColumnPair lcp = localizedReference == null ? LineColumnPair.Zero : new LineColumnPair(localizedReference.Line, localizedReference.Column);
                        var currentField = casesEnum.Fields.Add(token.Name);
                        currentField.SummaryText = string.Format("The @s:I{0}{1}{2}; is a{4} {3} token.", this.Source.Options.RulePrefix, rule.Name, this.Source.Options.RuleSuffix, token.Name, token != null && token.Name != null && IsEnglishVowel(token.Name[0]) ? "n" : string.Empty);
                        currentField.RemarksText = string.Format("Line {0}, column {1}", lcp.Line, lcp.Column);
                        casesLookup.Add(rule, token, currentField);
                    }
                    ruleORM = new RuleEntryBranchObjectRelationalMap(casesEnum, emptySet, this, rule);
                }
                else
                    ruleORM = new RuleEntryObjectRelationalMap(emptySet, this, rule);
                this._Add(rule, ruleORM);
            }
            foreach (var ruleDependency in ruleDependencyGraph)
            {
                IRuleEntryObjectRelationalMap ruleORM;
                if (ruleDependency.Key.IsRuleCollapsePoint)
                {
                    var casesEnum = dNamespace.Parts.Add().Enums.Add(string.Format("{0}{1}{2}Cases", this.Source.Options.RulePrefix, ruleDependency.Key.Name, this.Source.Options.RuleSuffix));
                    casesEnum.SummaryText = string.Format("Denotes the specific sub-rule that is condensed into the current @s:I{0}{1}{2};.", this.Source.Options.RulePrefix, ruleDependency.Key.Name, this.Source.Options.RuleSuffix);
                    casesEnum.RemarksText = string.Format("@para;'.{0}' at: line {1}, column {2}@/para;\r\n@code language=oilexer;{3}@/code;", ruleDependency.Key.FileName.Substring(Source.RelativeRoot.Length), ruleDependency.Key.Line, ruleDependency.Key.Column, ruleDependency.Key.GetDocComment());
                    casesEnum.AccessLevel = AccessLevelModifiers.Public;
                    var breakdown = ruleStates[ruleDependency.Key].OutTransitions.FullCheck.Breakdown;
                    foreach (var subRule in breakdown.Rules)
                    {
                        var localizedReference = (from src in ruleStates[ruleDependency.Key].Sources
                                                  where src.Item1 is IRuleReferenceProductionRuleItem
                                                  let ruleRef = (IRuleReferenceProductionRuleItem)src.Item1
                                                  where ruleRef.Reference == subRule.Source
                                                  select ruleRef).FirstOrDefault();
                        LineColumnPair lcp = localizedReference == null ? LineColumnPair.Zero : new LineColumnPair(localizedReference.Line, localizedReference.Column);
                        var currentField = casesEnum.Fields.Add(subRule.Source.Name);
                        currentField.SummaryText = string.Format("The @s:I{0}{1}{2}; is an @s:I{0}{3}{2}; instance.", this.Source.Options.RulePrefix, ruleDependency.Key.Name, this.Source.Options.RuleSuffix, subRule.Source.Name);
                        currentField.RemarksText = string.Format("Line {0}, column {1}", lcp.Line, lcp.Column);
                        casesLookup.Add(ruleDependency.Key, subRule.Source, currentField);
                    }
                    foreach (var token in breakdown.Tokens)
                    {
                        var localizedReference = (from src in ruleStates[ruleDependency.Key].Sources
                                                  where src.Item1 is ITokenReferenceProductionRuleItem
                                                  let tokenRef = (ITokenReferenceProductionRuleItem)src.Item1
                                                  where tokenRef.Reference == token
                                                  select tokenRef).FirstOrDefault();
                        LineColumnPair lcp = localizedReference == null ? LineColumnPair.Zero : new LineColumnPair(localizedReference.Line, localizedReference.Column);
                        var currentField = casesEnum.Fields.Add(token.Name);
                        currentField.SummaryText = string.Format("The @s:I{0}{1}{2}; is a{4} {3} token.", this.Source.Options.RulePrefix, ruleDependency.Key.Name, this.Source.Options.RuleSuffix, token.Name, token != null && token.Name != null && IsEnglishVowel(token.Name[0]) ? "n" : string.Empty);
                        currentField.RemarksText = string.Format("Line {0}, column {1}", lcp.Line, lcp.Column);
                        casesLookup.Add(ruleDependency.Key, token, currentField);
                    }
                    ruleORM = new RuleEntryChildBranchObjectRelationalMap(casesEnum, ruleDependency.Value, this, ruleDependency.Key);
                }
                else
                    ruleORM = new RuleEntryChildObjectRelationalMap(ruleDependency.Value, this, ruleDependency.Key);
                this._Add(ruleDependency.Key, ruleORM);
            }
            foreach (var rule in rules)
            {
                RuleTreeNode startingNode = CheckRootVariant(project, ruleVariants, rule);
                var reorm = (this[rule] as IRuleEntryObjectRelationalMap);
                ((RuleEntryObjectRelationalMap)reorm).ImplementationDetails = startingNode;
                if (rule.IsRuleCollapsePoint)
                    continue;
                var variations = (from variation in reorm.Variations
                                  select variation.ToArray()).ToArray();
                foreach (var variation in variations)
                {
                    var currentNode = startingNode;
                    foreach (var variationElement in variation.Skip(1) /* First is always the current. */)
                    {
                        var currentRootVariant = CheckRootVariant(project, ruleVariants, variationElement);
                        currentNode = BuildVariation(currentNode, currentRootVariant, project);
                    }
                }
            }
            //#endif
            //#if false
            //var followInformation = (from primaryRule in rules
            //                         from secondaryRule in ruleStates.Values
            //                         from secondarySubstate in GetRuleStates(secondaryRule)
            //                         from transition in secondarySubstate.OutTransitions.Keys
            //                         where transition.Breakdown.Rules.Any(p => p.Source == primaryRule)
            //                         let follow = secondarySubstate.OutTransitions[transition]
            //                         let useless = new { a = primaryRule.ToString(), b = secondaryRule.ToString(), c = secondarySubstate.ToString(), d = follow.ToString() }
            //                         group new { Primary = primaryRule, Secondary = secondaryRule, OriginatingState = secondarySubstate, FollowState = follow } by primaryRule).ToDictionary(k => k.Key, k => k.ToDictionary(j => j.OriginatingState, j => Tuple.Create(j.FollowState, j.Secondary)));

            //var firstSeries = followInformation.First();
            //#endif
            //var followData = (from f in followInformation.Keys
            //                  from sets in YieldFollowVocabulary(followInformation, f)
            //                  let variations = sets.Item3
            //                  where variations != null
            //                  group variations by f).ToDictionary(k => k.Key, k => k.Distinct().ToArray());
            //return;
            //OldCodeBuild(project, rules, ruleVariants);
        }

        private bool IsEnglishVowel(char c)
        {
            switch (c)
            {
                case 'a':
                case 'A':
                case 'e':
                case 'E':
                case 'i':
                case 'I':
                case 'o':
                case 'O':
                case 'u':
                case 'U':
                    return true;
            }
            return false;
        }


        private IEnumerable<Tuple<SyntacticalDFAState, SyntacticalDFARootState, GrammarVocabulary>> YieldFollowVocabulary(IDictionary<IOilexerGrammarProductionRuleEntry, Dictionary<SyntacticalDFAState, Tuple<SyntacticalDFAState, SyntacticalDFARootState>>> lookupTable, IOilexerGrammarProductionRuleEntry target)
        {
            var currentSet = lookupTable[target];
            foreach (var kvp in currentSet)
            {
                var entry = kvp.Value;
                var vocabSet = YieldPathFollowVocabulary(target, entry.Item2.Entry, entry.Item1, lookupTable, new List<IOilexerGrammarProductionRuleEntry>()).ToArray();
                var vocab = vocabSet.FirstOrDefault();
                if (vocab != null)
                    foreach (var subVocab in vocabSet.Skip(1))
                        vocab |= subVocab;
                yield return new Tuple<SyntacticalDFAState, SyntacticalDFARootState, GrammarVocabulary>(entry.Item1, entry.Item2, vocab);
            }
        }

        private IEnumerable<GrammarVocabulary> YieldPathFollowVocabulary(IOilexerGrammarProductionRuleEntry originatingRule, IOilexerGrammarProductionRuleEntry callingRule, SyntacticalDFAState currentFollowState, IDictionary<IOilexerGrammarProductionRuleEntry, Dictionary<SyntacticalDFAState, Tuple<SyntacticalDFAState, SyntacticalDFARootState>>> lookupTable, List<IOilexerGrammarProductionRuleEntry> currentPaths)
        {
            if (!lookupTable.ContainsKey(callingRule))
                yield break;
            if (currentFollowState.IsEdge)
            {
                if (currentPaths.Contains(callingRule))
                    yield break;
                currentPaths.Add(callingRule);
                var whatCallsCaller = lookupTable[callingRule];
                foreach (var set in whatCallsCaller.Values)
                    foreach (var vocab in YieldPathFollowVocabulary(originatingRule, set.Item2.Entry, set.Item1, lookupTable, currentPaths))
                        if (!vocab.IsEmpty)
                            yield return vocab;
                currentPaths.Remove(callingRule);
            }
            var fullVocab = currentFollowState.OutTransitions.FullCheck;

            if (!fullVocab.IsEmpty)
                yield return fullVocab;
        }

        private static List<SyntacticalDFAState> GetRuleStates(SyntacticalDFARootState state)
        {
            var result = new List<SyntacticalDFAState>();
            SyntacticalDFAState.FlatlineState(state, result);
            return result;
        }

        private RuleTreeNode BuildVariation(RuleTreeNode currentNode, RuleTreeNode secondaryRoot, IIntermediateAssembly project, IIntermediatePropertyMember tokenPropRef = null)
        {
            if (currentNode.ContainsKey(secondaryRoot.Value.Entry))
                return currentNode[secondaryRoot.Value.Entry];

            var currentSubVariant = currentNode.Value.Class.Parts.Add().Classes.Add(string.Format("_{0}", secondaryRoot.Value.Entry.Name));
            currentSubVariant.AccessLevel = AccessLevelModifiers.Internal;
            currentSubVariant.BaseType = currentNode.Value.Class;
            currentSubVariant.ImplementedInterfaces.ImplementInterfaceQuick(secondaryRoot.Value.RelativeInterface);
            var ctor = currentSubVariant.Constructors.Add(new TypedName("context", this.rootRuleBuilder.Compiler.RuleSymbolBuilder.ILanguageRuleSymbol));
            ctor.AccessLevel = AccessLevelModifiers.Internal;
            ctor.CascadeTarget = ConstructorCascadeTarget.Base;
            ctor.CascadeMembers.Add(ctor.Parameters["context"].GetReference());

            var branchORM = (IRuleEntryBranchObjectRelationalMap)this[secondaryRoot.Value.Entry];
            /* *
             * The variation of the rule needs linked to the appropriate 
             * group rule marked with 'IsRuleCollapsePoint'.
             * *
             * And within this, the proper path needs specified.
             * */
            var branchKind = currentSubVariant.Properties.Add(new TypedName(string.Format("{0}Kind", branchORM.Entry.Name), branchORM.CasesEnum), true, false);
            branchKind.AccessLevel = AccessLevelModifiers.Public;
            var childORM = (EntryObjectRelationalMap)this[currentNode.Value.Entry];
            if (childORM is ITokenEntryObjectRelationalMap)
            {
                if (tokenPropRef != null)
                {
                    var childTokens = (from tokenCase in childORM.GetTokenCases((IOilexerGrammarProductionRuleEntry)secondaryRoot.Value.Entry)
                                       let symbol = this.rootRuleBuilder.Compiler._GrammarSymbols.GetSymbolFromEntry((IOilexerGrammarTokenEntry)tokenCase.Item1)
                                       where symbol != null
                                       let identity = this.rootRuleBuilder.Compiler.LexicalSymbolModel.GetIdentitySymbolField(symbol)
                                       select new { Identity = identity, CollapseVariation = tokenCase.Item2 }).ToArray();
                    var unknownResultField = branchORM.CasesEnum.Fields.Add(GrammarVocabularyModelBuilder.GetUniqueEnumFieldName("Unknown", branchORM.CasesEnum));
                    var identitySwitch = branchKind.GetMethod.Switch(this.rootRuleBuilder.Compiler.CommonSymbolBuilder.Identity.GetReference(tokenPropRef.GetReference()));
                    foreach (var collapseToId in childTokens)
                        identitySwitch.Case(collapseToId.Identity.GetReference())
                            .Return(collapseToId.CollapseVariation.GetReference());
                    identitySwitch.Case(true).Return(unknownResultField.GetReference());
                }
            }
            else
                branchKind.GetMethod.Return(childORM.CaseFields[branchORM.Entry].GetReference());
            return currentNode.Add(secondaryRoot.Value.Entry, new ScannableEntryObjectification(secondaryRoot.Value.RelativeInterface, currentSubVariant, secondaryRoot.Value.Entry));
        }

        private RuleTreeNode CheckRootVariant(IIntermediateAssembly project, RuleTree ruleVariants, IOilexerGrammarScannableEntry entry)
        {
            if (ruleVariants.ContainsKey(entry))
                return ruleVariants[entry];
            else
                return ruleVariants.Add(entry, BuildRootObjectification(entry, project));
        }

        private ScannableEntryObjectification BuildRootObjectification(IOilexerGrammarScannableEntry entry, IIntermediateAssembly project)
        {
            const string defaultNamespaceSubspace = "Cst";
            var dNamespace = project.DefaultNamespace;
            var dNamespaceName = dNamespace.FullName;
            string subspaceName = string.Format("{0}.{1}", dNamespaceName, defaultNamespaceSubspace);
            if (dNamespace.Namespaces.PathExists(subspaceName))
                dNamespace = project.Namespaces[subspaceName];
            else
                dNamespace = project.Namespaces.Add(subspaceName);
            const string nameFormat = "{0}{1}{2}";
            const string iNameFormat = "I" + nameFormat;
            var iFace = dNamespace.Parts.Add().Interfaces.Add(string.Format(iNameFormat, this.Source.Options.RulePrefix, entry.Name, this.Source.Options.RuleSuffix));
            bool languageTail = this.Source.Options.GrammarName.ToLower().EndsWith("language");
            iFace.SummaryText = string.Format("Defines properites and methods for defining the {3} {0} of the {1}{2}.", entry is IOilexerGrammarProductionRuleEntry ? "rule" : "token", this.Source.Options.GrammarName, languageTail ? string.Empty : " language", entry.Name);
            iFace.AccessLevel = AccessLevelModifiers.Public;
            var entryORM = (IEntryObjectRelationalMap)this[entry];
            if (entryORM is IRuleEntryBranchObjectRelationalMap)
            {
                var branchORM = (IRuleEntryBranchObjectRelationalMap)entryORM;
                var entryInterfaceCase = iFace.Properties.Add(new TypedName(string.Format("{0}Kind", entry.Name), branchORM.CasesEnum), true, false);
            }

            IIntermediateClassType bClass = null;
            var ruleEntry = entry as IOilexerGrammarProductionRuleEntry;
            var tokenEntry = entry as IOilexerGrammarTokenEntry;
            if (ruleEntry != null)
                this.rootRuleBuilder.Compiler.RuleDetail[ruleEntry].ObjectModelDetails = (IRuleEntryObjectRelationalMap)entryORM;
            else if (tokenEntry != null)
                foreach (var sym in this.Compiler._GrammarSymbols.GetSymbolsFromEntry(tokenEntry))
                    this.rootRuleBuilder.Compiler.TokenSymbolDetail[sym].ObjectModelDetails = (ITokenEntryObjectRelationalMap)entryORM;

            iFace.ImplementedInterfaces.Add(this.rootRuleBuilder.ILanguageRule);

            if (tokenEntry != null || ruleEntry != null && !ruleEntry.IsRuleCollapsePoint)
            {
                bClass = dNamespace.Parts.Add().Classes.Add(string.Format(nameFormat, this.Source.Options.RulePrefix, entry.Name, this.Source.Options.RuleSuffix));
                //ACC - Update June 15, 2015: Added context-based constructor.
                var ctor = bClass.Constructors.Add(new TypedName("context", this.rootRuleBuilder.Compiler.RuleSymbolBuilder.ILanguageRuleSymbol));
                ctor.AccessLevel = AccessLevelModifiers.Internal;
                ctor.CascadeTarget = ConstructorCascadeTarget.Base;
                ctor.CascadeMembers.Add(ctor.Parameters["context"].GetReference());

                bClass.BaseType = rootRuleBuilder.LanguageRuleRoot;
                bClass.SummaryText = string.Format("Provides a base implementation of the @s:{0};.", iFace.Name);
                bClass.ImplementedInterfaces.ImplementInterfaceQuick(iFace);
                bClass.AccessLevel = AccessLevelModifiers.Internal;
                string docComment = ruleEntry == null ? tokenEntry.GetDocComment() : ruleEntry.GetDocComment();
                if (tokenEntry == null)
                    iFace.RemarksText = string.Format("@para;'.{0}' at: line {1}, column {2}@/para;\r\n@code language=oilexer;{3}@/code;", entry.FileName.Substring(Source.RelativeRoot.Length), entry.Line, entry.Column, docComment);
            }

            return new ScannableEntryObjectification(iFace, bClass, entry);
        }

        //#region IOilexerGrammarFileObjectRelationalMap Members

        public IOilexerGrammarFile Source { get; private set; }

        //#endregion

        //#region IOilexerGrammarFileObjectRelationalMap Members


        public IMultikeyedDictionary<IOilexerGrammarProductionRuleEntry, IOilexerGrammarScannableEntry, IIntermediateEnumFieldMember> CasesLookup
        {
            get { return this.casesLookup; }
        }

        //#endregion


        private ParserCompiler Compiler { get { return this.rootRuleBuilder.Compiler; } }

        public void BuildPrimaryMembers(IIntermediateCliManager identityManager)
        {
            foreach (var topLevel in this.Keys)
            {
                if (topLevel is OilexerGrammarProductionRuleEntry)
                {
                    var topLevelRule = (OilexerGrammarProductionRuleEntry)topLevel;
                    var orm = this.ImplementationDetails[topLevelRule];
                    if (!topLevelRule.IsRuleCollapsePoint)
                    {
                        var structure = topLevelRule.CaptureStructure;
                        foreach (var elementName in structure.Keys)
                        {
                            var structureItem = structure[elementName];
                            switch (structureItem.ResultType)
                            {
                                case ResultedDataType.Counter:
                                    {
                                        var propImpl = GetPropImpl(identityManager, orm, structure, structureItem, identityManager.ObtainTypeReference(RuntimeCoreType.Int32));
                                        SetStandardPropGet(structureItem, propImpl);
                                    }
                                    break;
                                case ResultedDataType.FlagEnumerationItem:
                                    {
                                        if (structureItem.Sources.AllSourcesAreSameIdentity())
                                            goto case ResultedDataType.Flag;
                                        else
                                            goto case ResultedDataType.ImportTypeList;
                                    }
                                case ResultedDataType.Flag:
                                    {
                                        var propImpl = GetPropImpl(identityManager, orm, structure, structureItem, identityManager.ObtainTypeReference(RuntimeCoreType.Boolean));
                                        SetStandardPropGet(structureItem, propImpl);
                                        break;
                                    }
                                case ResultedDataType.Enumeration:
                                case ResultedDataType.EnumerationItem:
                                    {
                                        var propImpl = GetPropImpl(identityManager, orm, structure, structureItem, this.Compiler.LexicalSymbolModel.IdentityEnum);
                                        var capture = propImpl.GetMethod.Locals.Add(new TypedName("{0}Capture", this.Compiler.TokenSymbolBuilder.ILanguageToken, structureItem.BucketName.LowerFirstCharacter()), this.Compiler.RuleSymbolBuilder.GetExplicitCapture.GetReference(this.Compiler.RootRuleBuilder.ContextImpl.GetReference(), this.Compiler.TokenSymbolBuilder.ILanguageToken).Invoke(structureItem.BucketName.ToPrimitive()));
                                        var nullCheck = propImpl.GetMethod.If(capture.InequalTo(IntermediateGateway.NullValue));
                                        nullCheck.Return(this.Compiler.CommonSymbolBuilder.Identity.GetReference(capture.GetReference()));
                                        propImpl.GetMethod.Return(this.Compiler.LexicalSymbolModel.NoIdentityField.GetReference());
                                        break;
                                    }
                                case ResultedDataType.ComplexType:

                                    break;
                                case ResultedDataType.Character:
                                    {
                                        var propImpl = GetPropImpl(identityManager, orm, structure, structureItem, identityManager.ObtainTypeReference(RuntimeCoreType.Char));
                                        break;
                                    }
                                case ResultedDataType.String:
                                    {
                                        var propImpl = GetPropImpl(identityManager, orm, structure, structureItem, identityManager.ObtainTypeReference(RuntimeCoreType.String));
                                        break;
                                    }
                                case ResultedDataType.ImportType:
                                    {
                                        var ruleTypeSource = GetRuleTypeSourceForListOrType(structureItem);
                                        GenerateImportTypeFromScannableEntry(identityManager, orm, structure, structureItem, ruleTypeSource);
                                        break;
                                    }
                                case ResultedDataType.ImportTypeList:
                                    {
                                        var ruleTypeSource = GetRuleTypeSourceForListOrType(structureItem);
                                        GenerateImportListTypeFromScannableEntry(identityManager, orm, structure, structureItem, ruleTypeSource);
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        RepeatCollapseCheck:
            bool topLevelCheckChanged = false;
            foreach (var topLevel in this.Keys)
            {
                if (topLevel is OilexerGrammarProductionRuleEntry)
                {
                    var topLevelRule = (OilexerGrammarProductionRuleEntry)topLevel;
                    var state = this.Compiler.RuleDFAStates[topLevelRule];
                    var orm = this.ImplementationDetails[topLevelRule];
                    var symbolicRep = state.OutTransitions.FullCheck.SymbolicBreakdown(this.Compiler);
                    var interfaces = GatherCollapsePointChildInterfaces(orm, topLevelRule, symbolicRep).ToArray();
                    if (topLevelRule.IsRuleCollapsePoint && interfaces.Length > 0)
                    {
                        var firstInterface =
                            interfaces[0];
                        if (firstInterface == null)
                            continue;
                        var properties = firstInterface.Properties.Keys;
                        var filteredProperties =
                            from propertyKey in properties
                            let subQuery = from otherInterface in interfaces.Skip(1)
                                           where otherInterface != null
                                           where otherInterface.Properties.ContainsKey(propertyKey)
                                           let ormProp = otherInterface.Properties[propertyKey]
                                           where ormProp.PropertyType == firstInterface.Properties[propertyKey].PropertyType
                                           select otherInterface
                            where subQuery.Count() == (symbolicRep.Rules.Count + symbolicRep.Tokens.Count) - 1
                            select propertyKey;
                        foreach (var propertyKey in filteredProperties)
                        {
                            if (!orm.Value.RelativeInterface.Properties.ContainsKey(propertyKey))
                            {
                                topLevelCheckChanged = true;
                                var propertyRef = firstInterface.Properties[propertyKey];
                                orm.Value.RelativeInterface.Properties.Add(new TypedName(propertyKey.Name, propertyRef.PropertyType), true, false);
                            }
                        }
                    }
                }
            }
            if (topLevelCheckChanged)
                goto RepeatCollapseCheck;
        }

        private IEnumerable<IIntermediateInterfaceType> GatherCollapsePointChildInterfaces(RuleTreeNode orm, OilexerGrammarProductionRuleEntry topLevelRule, GrammarVocabularySymbolicBreakdown symbolicRep)
        {
            foreach (var rule in symbolicRep.Rules.Keys)
                yield return symbolicRep.Rules[rule].RelativeInterface;
        }

        private static IOilexerGrammarScannableEntry GetRuleTypeSourceForListOrType(IProductionRuleCaptureStructuralItem structureItem)
        {
            var ruleTypeSource =
                structureItem.Sources
                .Where(k =>
                    (k is IRuleReferenceProductionRuleItem) ||
                    (k is ITokenReferenceProductionRuleItem))
                .Select(k =>
                    k is IRuleReferenceProductionRuleItem
                        ? ((IOilexerGrammarScannableEntry)(((IRuleReferenceProductionRuleItem)(k)).Reference))
                        : ((IOilexerGrammarScannableEntry)(((ITokenReferenceProductionRuleItem)(k)).Reference))).FirstOrDefault();
            return ruleTypeSource;
        }

        private void GenerateImportListTypeFromScannableEntry(IIntermediateCliManager identityManager, RuleTreeNode orm, IProductionRuleCaptureStructure structure, IProductionRuleCaptureStructuralItem structureItem, IOilexerGrammarScannableEntry importTypeSource)
        {
            if (importTypeSource != null && (this.ImplementationDetails.ContainsKey(importTypeSource) || importTypeSource is IOilexerGrammarTokenEntry))
            {
                IType targetType = 
                    importTypeSource is IOilexerGrammarTokenEntry 
                        ? this.Compiler.TokenSymbolBuilder.ILanguageToken
                        : this.ImplementationDetails[importTypeSource].Value.RelativeInterface;
                IType captureType =
                    this.Compiler.CommonSymbolBuilder.ILanguageSymbol;
                if (targetType != null)
                {
                    var propImpl = GetPropImpl(identityManager, orm, structure, structureItem, ((IGenericType)(identityManager.ObtainTypeReference(typeof(IList<>)))).MakeGenericClosure(targetType));
                    var fieldImpl = orm.Value.Class.Fields.Add(new TypedName("_{0}", propImpl.PropertyType, propImpl.Name.LowerFirstCharacter()));
                    fieldImpl.AccessLevel = AccessLevelModifiers.Private;
                    fieldImpl.SummaryText = string.Format("Data member for @s:{0};", propImpl.Name);
                    var fieldNullCheck = propImpl.GetMethod.If(fieldImpl.EqualTo(IntermediateGateway.NullValue));
                    var ruleSymbols = fieldNullCheck.Locals.Add(
                        new TypedName("capturedSymbols", ((IGenericType)(identityManager.ObtainTypeReference(typeof(IList<>)))).MakeGenericClosure(captureType)),
                        this.Compiler.RuleSymbolBuilder.GetExplicitCapture.GetReference(this.Compiler.RootRuleBuilder.ContextImpl.GetReference(), ((IGenericType)(identityManager.ObtainTypeReference(typeof(IList<>)))).MakeGenericClosure(captureType)).Invoke(structureItem.BucketName.ToPrimitive()));
                    ruleSymbols.AutoDeclare = false;
                    fieldNullCheck.DefineLocal(ruleSymbols);
                    var symbolNullCheck = fieldNullCheck.If(ruleSymbols.InequalTo(IntermediateGateway.NullValue));
                    symbolNullCheck.Assign(fieldImpl.GetReference(), ((IGenericType)(identityManager.ObtainTypeReference(typeof(List<>)))).MakeGenericClosure(targetType).GetNewExpression());
                    var enumerateBlock = symbolNullCheck.Enumerate("capturedSymbol", ruleSymbols.GetReference());
                    if (importTypeSource is IOilexerGrammarTokenEntry)
                        enumerateBlock.Call(fieldImpl.GetReference().GetMethod("Add").Invoke(enumerateBlock.Local.GetReference().Cast(this.Compiler.TokenSymbolBuilder.ILanguageToken)));
                    else
                        enumerateBlock.If(enumerateBlock.Local.GetReference().Is(this.Compiler.RuleSymbolBuilder.ILanguageRuleSymbol))
                            .Call(fieldImpl.GetReference().GetMethod("Add").Invoke(this.Compiler.RuleSymbolBuilder.CreateRuleImpl.GetReference(enumerateBlock.Local.GetReference().Cast(this.Compiler.RuleSymbolBuilder.ILanguageRuleSymbol)).Invoke().Cast(targetType)));
                    propImpl.GetMethod.Return(fieldImpl.GetReference());
                }
            }
        }

        private void GenerateImportTypeFromScannableEntry(IIntermediateCliManager identityManager, RuleTreeNode orm, IProductionRuleCaptureStructure structure, IProductionRuleCaptureStructuralItem structureItem, IOilexerGrammarScannableEntry importTypeSource)
        {
            if (importTypeSource != null && (this.ImplementationDetails.ContainsKey(importTypeSource) || importTypeSource is IOilexerGrammarTokenEntry))
            {
                IType targetType =
                    importTypeSource is IOilexerGrammarTokenEntry
                        ? this.Compiler.TokenSymbolBuilder.ILanguageToken
                        : this.ImplementationDetails[importTypeSource].Value.RelativeInterface;
                IType captureType =
                    importTypeSource is IOilexerGrammarTokenEntry
                        ? this.Compiler.TokenSymbolBuilder.ILanguageToken
                        : this.Compiler.RuleSymbolBuilder.ILanguageRuleSymbol;
                if (targetType != null)
                {
                    var propImpl = GetPropImpl(identityManager, orm, structure, structureItem, targetType);
                    var fieldImpl = orm.Value.Class.Fields.Add(new TypedName("_{0}", targetType, propImpl.Name.LowerFirstCharacter()));
                    fieldImpl.AccessLevel = AccessLevelModifiers.Private;
                    fieldImpl.SummaryText = string.Format("Data member for @s:{0};", propImpl.Name);
                    var fieldNullCheck = propImpl.GetMethod.If(fieldImpl.EqualTo(IntermediateGateway.NullValue));
                    var ruleSymbol = fieldNullCheck.Locals.Add(
                        new TypedName("capturedSymbol", captureType),
                        this.Compiler.RuleSymbolBuilder.GetExplicitCapture.GetReference(this.Compiler.RootRuleBuilder.ContextImpl.GetReference(), captureType).Invoke(structureItem.BucketName.ToPrimitive()));
                    ruleSymbol.AutoDeclare = false;
                    fieldNullCheck.DefineLocal(ruleSymbol);
                    var symbolNullCheck = fieldNullCheck.If(ruleSymbol.InequalTo(IntermediateGateway.NullValue));
                    if (importTypeSource is IOilexerGrammarTokenEntry)
                        symbolNullCheck.Assign(fieldImpl.GetReference(), ruleSymbol.GetReference());
                    else
                        symbolNullCheck.Assign(fieldImpl.GetReference(), this.Compiler.RuleSymbolBuilder.CreateRuleImpl.GetReference(ruleSymbol.GetReference()).Invoke().Cast(targetType));
                    propImpl.GetMethod.Return(fieldImpl.GetReference());
                }
            }
        }

        private void SetStandardPropGet(IProductionRuleCaptureStructuralItem structureItem, IIntermediateClassPropertyMember propImpl)
        {
            propImpl.GetMethod.Return(this.Compiler.RuleSymbolBuilder.GetExplicitCapture.GetReference(this.Compiler.RootRuleBuilder.ContextImpl.GetReference(), propImpl.PropertyType).Invoke(structureItem.BucketName.ToPrimitive()));
        }

        private static IIntermediateClassPropertyMember GetPropImpl(IIntermediateCliManager identityManager, RuleTreeNode orm, IProductionRuleCaptureStructure structure, IProductionRuleCaptureStructuralItem structureItem, IType propType)
        {
            var prop = orm.Value.RelativeInterface.Properties.Add(
                new TypedName(structureItem.BucketName, propType), true, false);
            var propImpl = orm.Value.Class.Properties.Add(
                new TypedName(structureItem.BucketName, propType), true, false);
            propImpl.AccessLevel = AccessLevelModifiers.Public;
            return propImpl;
        }
    }
}
