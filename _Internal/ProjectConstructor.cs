using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using Oilexer.Expression;
using Oilexer._Internal.Flatform.Rules;
using Oilexer._Internal.Flatform.Rules.StateSystem;
using Oilexer._Internal.Flatform.Tokens;
using Oilexer._Internal.Flatform.Tokens.StateSystem;
//using Oilexer._Internal.UI.Visualization;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Statements;
using Oilexer.Translation;
using Oilexer.Types;
using Oilexer.Types.Members;
using EnumFieldLookup = System.Collections.Generic.Dictionary<Oilexer.Types.IEnumeratorType, Oilexer.Types.Members.IFieldMember>;
using EnumFieldRefLookup = System.Collections.Generic.Dictionary<Oilexer.Types.IEnumeratorType, Oilexer.Expression.IFieldReferenceExpression>;
using EnumToConstructorLookup = System.Collections.Generic.Dictionary<Oilexer.Types.IEnumeratorType, Oilexer.Types.Members.IConstructorMember>;
using Operators = System.CodeDom.CodeBinaryOperatorType;
using PrimitiveStatePair = System.Collections.Generic.KeyValuePair<Oilexer.Expression.PrimitiveExpression, Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageState>;
using RegularLanguageBitArraySeries = System.Collections.Generic.List<Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageBitArray>;
using RegularLanguageStateSeries = System.Collections.Generic.List<Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageState>;
using RLBitArrayToTokenEntrySetLookup = System.Collections.Generic.Dictionary<Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageBitArray, System.Collections.Generic.List<Oilexer.Parser.GDFileData.ITokenEntry>>;
using SourceTransitionGraph = Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageState.ForkTransitions;
using TokenEntryToConstructorLookup = System.Collections.Generic.Dictionary<Oilexer.Parser.GDFileData.ITokenEntry, Oilexer.Types.Members.IConstructorMember>;
using TokenEntryToEnumLookup = System.Collections.Generic.Dictionary<Oilexer.Parser.GDFileData.ITokenEntry, Oilexer.Types.IEnumeratorType>;
using TokenEntryToFieldLookup = System.Collections.Generic.Dictionary<Oilexer.Parser.GDFileData.ITokenEntry, Oilexer.Types.Members.IFieldMember>;
using TokenEntryToStateTypeLookup = System.Collections.Generic.Dictionary<Oilexer.Parser.GDFileData.ITokenEntry, Oilexer._Internal.ProjectConstructor.StateMachineType>;
using TransitionGraph = System.Collections.Generic.Dictionary<Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageState, System.Collections.Generic.List<System.Collections.Generic.Dictionary<Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageState, Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageBitArray>>>;
using TransitionGraphElement = System.Collections.Generic.List<System.Collections.Generic.Dictionary<Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageState, Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageBitArray>>;
using TransitionGraphElementEntry = System.Collections.Generic.Dictionary<Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageState, Oilexer._Internal.Flatform.Tokens.StateSystem.RegularLanguageBitArray>;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Oilexer.Parser;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal static partial class ProjectConstructor
    {
        private static CodeTranslationOptions ctoCommon = new CodeTranslationOptions(true);
        internal class StateLabelLookup : Dictionary<RegularLanguageState, LabelStatement> { }

        internal class PrimitiveStateLookup : Dictionary<PrimitiveExpression, RegularLanguageState> { }

        internal class StatePrimitiveLookup : Dictionary<RegularLanguageState, PrimitiveExpression> 
        {
            public PrimitiveStateLookup ObtainReverseLookup()
            {
                PrimitiveStateLookup result = new PrimitiveStateLookup();
                foreach (var item in this)
                    result.Add(item.Value, item.Key);
                return result;
            }

            public StatePrimitiveLookup Sort()
            {
                StatePrimitiveLookup result = new StatePrimitiveLookup();
                foreach (var item in from t in this
                                     orderby t.Key.StateValue
                                     select t)
                    result.Add(item.Key, item.Value);
                return result;
            }
        }

        private class EnumerationTokenDataSet : Dictionary<ITokenEntry, EnumerationTokenData> { }

        private class EnumerationTokenData 
        {
            private IEnumeratorType[] enumerators;
            public EnumerationTokenData(int setCount, IEnumeratorType[] enumerators, List<ITokenItem> items, RegularLanguageState state)
            {
                this.Items = new List<ITokenItem>(items);
                if (enumerators == null)
                    throw new ArgumentNullException("enumerators");
                this.SetCount = setCount;
                this.enumerators = enumerators;
                this.State = state;
            }

            public RegularLanguageState State { get; private set; }

            public int SetCount { get; private set; }

            public List<ITokenItem> Items { get; private set; }

            public IEnumerable<IEnumeratorType> Enumerators
            {
                get
                {
                    return this.enumerators;
                }
            }

            public IEnumeratorType CaseEnumerator
            {
                get
                {
                    if (SetCount == 1)
                        return null;
                    else
                        return enumerators[SetCount];
                }
            }
        }

        /* *
         * For convenience so "Dictionary<ITokenItem, EnumStateMachineData>"
         * is not necessary.
         * */
        internal class EnumStateMachineDataSet :
            Dictionary<ITokenItem, EnumStateMachineData> 
        {
        }

        /* *
         * This is easier to understand than multiple sets of 
         * token-item keyed dictionaries for the same data.
         * */
        internal class EnumStateMachineData
        {
            public EnumStateMachineData(ITokenItem source, RegularLanguageState.ForkTransition transitionData, IEnumeratorType stateMachineCases, ref int baseValue, IFieldMember resultantEnumMember)
            {
                this.ExitStateConstants = new Dictionary<RegularLanguageState, IFieldMember>();
                this.Source = source;
                foreach (var state in transitionData.TargetStates)
                    this.ExitStateConstants.Add(state, CreateEntryFor(source, transitionData, stateMachineCases, ref baseValue, resultantEnumMember.Name, state));
                this.ResultEnumerationMember = resultantEnumMember;
                this.TransitionData = transitionData;
            }

            private static IFieldMember CreateEntryFor(ITokenItem source, RegularLanguageState.ForkTransition transitionData, IEnumeratorType stateMachineCases, ref int baseValue, string rName, RegularLanguageState state)
            {
                int stateSourceCount = state.Sources.Count();
                const string format = "{0}";
                string itemName = null;
                /* *
                 * In cases where two enumeration elements overlap (due to one element
                 * being case insensitive, and one being case sensitive), it's necessary
                 * to create the appropriate 
                 * */
                var targetCount = transitionData.TargetStates.Count;
                if (stateSourceCount > 1)
                {
                    string eName = null;
                    foreach (var stateSource in state.Sources)
                    {
                        if (eName == null)
                            eName = stateSource.Name;
                        else
                            eName += "_" + stateSource.Name;
                    }
                    itemName = string.Format(format, eName);
                }
                else if (source.Name != null && source.Name != string.Empty)
                    itemName = string.Format(format, source.Name);
                else
                    itemName = string.Format(format, rName);
                if (!stateMachineCases.Fields.ContainsKey(itemName))
                {
                    var exitStateConstant = stateMachineCases.Fields.AddNew(itemName);
                    exitStateConstant.InitializationExpression = new PrimitiveExpression(++baseValue);
                    exitStateConstant.IsConstant = true;
                    exitStateConstant.AccessLevel = DeclarationAccessLevel.Private;
                    if (stateSourceCount == 1)
                    {
                        exitStateConstant.Summary = string.Format("Used to express the exit-state for the {0} case.", source.Name);
                        exitStateConstant.Remarks = string.Format("Original definition: {0}", source.ToString());
                    }
                    else
                    {
                        string eName = null;
                        string eDefinition = null;
                        foreach (var stateSource in state.Sources)
                            if (eName == null)
                            {
                                eName = stateSource.Name;
                                eDefinition = stateSource.ToString();
                            }
                            else
                            {
                                eName += ", " + stateSource.Name;
                                eDefinition += ", " + stateSource.ToString();
                            }
                        exitStateConstant.Summary = string.Format("Used to express the exit-state for the {0} cases.", eName);
                        exitStateConstant.Remarks = string.Format("Original definitions: {0}", eDefinition);
                    }
                    return exitStateConstant;
                }
                else
                    return stateMachineCases.Fields[itemName];
            }

            public EnumStateMachineData(ITokenItem source, RegularLanguageState.ForkTransition transitionData, IEnumeratorType stateMachineCases, ref int baseValue, IFieldMember resultantEnumMember, Dictionary<RegularLanguageState, IFieldMember> resultStateMembers)
            {
                this.ExitStateConstants = new Dictionary<RegularLanguageState, IFieldMember>();
                foreach (var state in transitionData.TargetStates)
                {
                    if (resultStateMembers.ContainsKey(state))
                        this.ExitStateConstants.Add(state, resultStateMembers[state]);
                    else
                        this.ExitStateConstants.Add(state, CreateEntryFor(source, transitionData, stateMachineCases, ref baseValue, resultantEnumMember.Name, state));
                }
                this.TransitionData = transitionData;
                this.Source = source;
                this.ResultEnumerationMember = resultantEnumMember;
            }
            /// <summary>
            /// Returns/sets the <see cref="RegularLanguageState.ForkTransition"/> for which the
            /// <see cref="Source"/> exits with.
            /// </summary>
            public RegularLanguageState.ForkTransition TransitionData { get; private set; }
            /// <summary>
            /// The series of states which are used to determine the points of exit for a given
            /// token item.
            /// </summary>
            public Dictionary<RegularLanguageState, IFieldMember> ExitStateConstants { get; private set; }
            /// <summary>
            /// The <see cref="IFieldMember"/> which represents the <see cref="ITokenItem"/>
            /// in the primary enumeration for the token.
            /// </summary>
            public IFieldMember ResultEnumerationMember { get; private set; }
            /// <summary>
            /// The <see cref="ITokenItem"/> for which the context data
            /// is based upon.
            /// </summary>
            public ITokenItem Source { get; private set; }

            public override string ToString()
            {
                return string.Format("{0} - {1} exit states", this.Source.ToString(), ExitStateConstants.Count);
            }
        }

        public enum StateMachineType
        {
            /// <summary>
            /// Token represents a simplistic capturing type token; wherein the data is simply
            /// a string.
            /// </summary>
            Capture,
            /// <summary>
            /// Token represents a sub-set of tokens under a named classification/grouping.
            /// </summary>
            /// <remarks>This only occurs when the token consists of a series of literals
            /// in a large alternation, one literal per expression.</remarks>
            Enumeration,
        }

        internal static string Encode(this string target)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in target)
            {
                switch (c)
                {
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\\':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\v':
                        sb.Append("\\v");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return string.Format("\"{0}\"", sb.ToString());
        }

        private const string BuildNamespaceBase = "Oilexer.Parsers";
        private static List<List<ITokenEntry>> GetTokenPrecedences(IEnumerable<ITokenEntry> currentSet, Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> precedenceAssociation)
        {
            //Only one key per precedence is needed to sort the list.
            Dictionary<ITokenEntry, List<ITokenEntry>> listLookup = new Dictionary<ITokenEntry, List<ITokenEntry>>();
            foreach (var token in currentSet)
                EnsureEqualListingExists(token, listLookup, currentSet, precedenceAssociation);
            List<ITokenEntry> orderedKeys = new List<ITokenEntry>(listLookup.Keys);
            var orderer = new TokenOrderingComparer(precedenceAssociation);
            orderedKeys.Sort(orderer);
            List<List<ITokenEntry>> result = new List<List<ITokenEntry>>();
            foreach (var entry in orderedKeys)
            {
                var currentList = listLookup[entry];
                currentList.Sort(orderer);
                result.Add(currentList);
            }
            return result;
        }

        private static void EnsureEqualListingExists(ITokenEntry relative, Dictionary<ITokenEntry, List<ITokenEntry>> listLookup, IEnumerable<ITokenEntry> currentSet, Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> precedenceAssociation)
        {
            if (listLookup.ContainsKey(relative))
                return;
            foreach (var list in listLookup.Values)
                if (list.Contains(relative))
                    return;
            var result = GetEqualPrecedenceTokens(relative, currentSet, precedenceAssociation);
            listLookup.Add(relative, result);
        }

        private static List<ITokenEntry> GetEqualPrecedenceTokens(ITokenEntry relative, IEnumerable<ITokenEntry> currentSet, Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> precedenceAssociation)
        {
            List<ITokenEntry> result = new List<ITokenEntry>();
            foreach (ITokenEntry current in currentSet)
                if (precedenceAssociation[relative][current] == TokenPrecedence.Equal)
                    result.Add(current);
            return result;
        }

        public static IIntermediateProject Build(this IGDFile target, CompilerErrorCollection errorTarget)
        {
            IProductionRuleEntry startRule = null;
            int initialRuleCount = LinkerCore.ruleEntries.Count();
            if (target.Options.StartEntry == null || target.Options.StartEntry == string.Empty)
            {
                errorTarget.Add(GrammarCore.GetParserError(target.Files[0], 0, 0, GDParserErrors.NoStartDefined, target.Options.GrammarName));
                return null;
            }
            if ((startRule = (LinkerCore.ruleEntries).FindScannableEntry(target.Options.StartEntry)) == null)
            {
                errorTarget.Add(GrammarCore.GetParserError(target.Files[0], 0, 0, GDParserErrors.InvalidStartDefined, target.Options.StartEntry));
                return null;
            }
            string baseTitle2 = string.Format("{0}{1}", Program.baseTitle, " - {0}");
            Console.Title = string.Format(baseTitle2, "Token Analysis");
            const string stateMachineFormat = "{0}{1}{2}StateMachine";
            //file.Options.AssemblyName, string.Format(BuildNamespaceBase, file.Options.GrammarName)
            IDictionary<ITokenEntry, FlattenedTokenEntry> tokenData = new Dictionary<ITokenEntry, FlattenedTokenEntry>();
            Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> precedenceAssociation = BuildPrecedenceTable(LinkerCore.tokenEntries);
            List<ITokenEntry> tokenOrdering = new List<ITokenEntry>(LinkerCore.tokenEntries);
            tokenOrdering.Sort(new TokenOrderingComparer(precedenceAssociation));
            LinkerCore.tokenEntries = tokenOrdering;
            ITokenEntry[][] tokenEntries;
            foreach (ITokenEntry tok in LinkerCore.tokenEntries)
                tokenData.Add(tok, new FlattenedTokenEntry(tok));
            foreach (var item in tokenData.Values)
                item.Initialize();
            IIntermediateProject result = new IntermediateProject(target.Options.AssemblyName, string.Format(BuildNamespaceBase));
            var assemblyChildSpace = result.DefaultNameSpace.ChildSpaces.AddNew(target.Options.AssemblyName).Partials.AddNew();
            var bitStreamClass = BitStreamCreator.CreateBitStream(assemblyChildSpace);
            IClassType parser = result.DefaultNameSpace.Partials.AddNew().Classes.AddNew(target.Options.ParserName);
            IIntermediateModule rootModule = result.RootModule;
            Console.Title = string.Format(baseTitle2, "Token Construction");
            IClassType scannerTokenSetData = null;
            var tokenBuildData = BuildTokens(target, assemblyChildSpace, stateMachineFormat, tokenData, result, parser, ref scannerTokenSetData, bitStreamClass, precedenceAssociation, out tokenEntries);

            //RegularGraphDialog rgd = new RegularGraphDialog(tokenBuildData);
            //rgd.ShowDialog();

            int ruleCount = 0;
        RebuildRules:
            Console.Title = string.Format(baseTitle2, "Rule Analysis");
            Dictionary<IProductionRuleEntry, FlattenedRuleEntry> ruleData = new Dictionary<IProductionRuleEntry, FlattenedRuleEntry>();
            var ruleEntriesArray = LinkerCore.ruleEntries.ToArray();

            ruleCount = ruleEntriesArray.Length;
            //Streamline structure.
            foreach (IProductionRuleEntry rule in ruleEntriesArray)
                ruleData.Add(rule, new FlattenedRuleEntry(rule, ruleData));
            //Initialize internal representation.
            foreach (var item in ruleData.Values)
                item.Initialize(tokenBuildData, ruleEntriesArray);
            //Split the rules where the data dependencies lie.
            Console.Title = string.Format(baseTitle2, "Data Dependency Analysis");
            //foreach (var item in ruleData.Values)
            //    item.ExtractDataDependencies(target);
            if (LinkerCore.ruleEntries.Count() != ruleCount)
                goto RebuildRules;
            BitArray alreadyHit = new BitArray(ruleCount);
            /* *
             * 
             * */
            for (int i = 0; i < ruleCount; i++)
                if (alreadyHit[i] || !((ProductionRuleEntry)(ruleEntriesArray[i])).IsExtract)
                    continue;
                else
                    for (int j = i + 1; j < ruleCount; j++)
                    {
                        if (ruleEntriesArray[j] == startRule)
                            continue;
                        if (ruleEntriesArray[i].IsEqualTo(ruleEntriesArray[j]))
                        {
                            target.ReplaceReferences(ruleEntriesArray[i], ruleEntriesArray[j]);
                            alreadyHit[j] = true;
                        }
                    }

            for (int i = 0; i < ruleCount; i++)
                if (alreadyHit[i])
                    goto RebuildRules;
            //Build the state.
            foreach (var item in ruleData.Values)
                item.BuildState();
            //Condense the state set.
            foreach (var item in ruleData.Values)
                item.CondenseState();
            SyntaxActivationInfo sai = new SyntaxActivationInfo(tokenBuildData, ruleData.Values, assemblyChildSpace);
            BuildLogicConstruct(startRule, result, target, assemblyChildSpace, ruleData, parser, scannerTokenSetData, tokenBuildData, precedenceAssociation, sai, tokenEntries);
            //Console.Title = string.Format(baseTitle2, "Branching from start rule.");
            //Build the transition graph
            Console.Title = string.Format(baseTitle2, "Rule Construction");
            BuildRuleBuilders(ruleData, tokenData, parser, target, sai, assemblyChildSpace);
            Console.WriteLine("Initial vs. final rule count: {{{0}, {1}}}", initialRuleCount, ruleCount);
            //BuildRules(startRule, target, tokenData, tokenBuildData, result, ruleData, parser, scannerTokenSetData);
            return result;
        }

        private static void BuildRuleBuilders(Dictionary<IProductionRuleEntry, FlattenedRuleEntry> ruleData, IDictionary<ITokenEntry, FlattenedTokenEntry> tokenData, IClassType parser, IGDFile target, SyntaxActivationInfo sai, INameSpaceDeclaration assemblyChildSpace)
        {

            #region RuleIdentifier Enumeration
            IEnumeratorType ruleIdentifierEnum = assemblyChildSpace.Partials.AddNew().Enumerators.AddNew("RuleIdentifier");
            Dictionary<IProductionRuleEntry, IFieldMember> ruleIdentifierFieldLookup = new Dictionary<IProductionRuleEntry, IFieldMember>();
            {
                int value = 0;
                foreach (IProductionRuleEntry rule in ruleData.Keys)
                    ruleIdentifierFieldLookup.Add(rule, ruleIdentifierEnum.Fields.AddNew(rule.Name, ++value));
            }
            #endregion
            #region Declaration Site State Definition

            //internal class *FlatDFAState //*= Rule Prefix
            var stateCore = assemblyChildSpace.Partials.AddNew().Classes.AddNew(string.Format("{0}FlatDFAState", target.Options.RulePrefix));
            stateCore.AccessLevel = DeclarationAccessLevel.Internal;
            //{
            //    private bool isTerminalEdge;
            var isTerminalEdgeField = stateCore.Fields.AddNew(new TypedName("isTerminalEdge", typeof(bool)));
            //    public bool IsTerminalEdge
            var isTerminalEdgeProperty = stateCore.Properties.AddNew(new TypedName("IsTerminalEdge", typeof(bool)), true, false);
            isTerminalEdgeProperty.AccessLevel = DeclarationAccessLevel.Public;
            //    {
            //        get
            //        {
            //            return this.isTerminalEdge;
            isTerminalEdgeProperty.GetPart.Return(isTerminalEdgeField.GetReference());
            //        }
            //    }

            var stateCoreCtor = stateCore.Constructors.AddNew();
            var isTerminalEdgeParam = stateCoreCtor.Parameters.AddNew(isTerminalEdgeField.Name, isTerminalEdgeField.FieldType);
            stateCoreCtor.Statements.Assign(isTerminalEdgeField.GetReference(), isTerminalEdgeParam.GetReference());
            stateCoreCtor.AccessLevel = DeclarationAccessLevel.Public;
            #region TransitionData Struct
            //    public class TransitionData
            var stateCoreTransition = stateCore.Partials.AddNew().Classes.AddNew("TransitionData");
            stateCoreTransition.AccessLevel = DeclarationAccessLevel.Public;
            //    {
            //        private TokenTransition check;
            var checkField = stateCoreTransition.Fields.AddNew(new TypedName("check", sai.data.EnumResults.ResultantType));
            //        private DeclarationRuleState target;
            var targetField = stateCoreTransition.Fields.AddNew(new TypedName("target", stateCore));

            //        public TokenTransition Check
            var checkProperty = stateCoreTransition.Properties.AddNew("Check", checkField.FieldType, true, false);
            checkProperty.AccessLevel = DeclarationAccessLevel.Public;
            //        {
            //           get
            //           {
            //               return this.check;
            checkProperty.GetPart.Return(checkField.GetReference());
            //           }
            //        }

            //        public DeclarationRuleState Target
            var targetProperty = stateCoreTransition.Properties.AddNew("Target", targetField.FieldType, true, false);
            targetProperty.AccessLevel = DeclarationAccessLevel.Public;
            //        {
            //           get
            //           {
            //               return this.target;
            targetProperty.GetPart.Return(targetField.GetReference());
            //           }
            //        }

            var transitionCtor = stateCoreTransition.Constructors.AddNew();
            var transitionParam = transitionCtor.Parameters.AddNew(new TypedName(checkField.Name, checkField.FieldType));
            var targetParam = transitionCtor.Parameters.AddNew(new TypedName(targetField.Name, targetField.FieldType));
            transitionCtor.Statements.Assign(checkField.GetReference(), transitionParam.GetReference());
            transitionCtor.Statements.Assign(targetField.GetReference(), targetParam.GetReference());
            transitionCtor.AccessLevel = DeclarationAccessLevel.Public;
            #endregion

            #region Transitions Class
            //    public class TransitionSeries :
            //      List<TransitionData>
            var stateCoreTransitions = stateCore.Partials.AddNew().Classes.AddNew("TransitionSeries");
            stateCoreTransitions.AccessLevel = DeclarationAccessLevel.Public;
            stateCoreTransitions.BaseType = typeof(List<>).GetTypeReference(new TypeReferenceCollection(stateCoreTransition));
            //    {
            #region Add Method
            //        public TransitionData Add(RuleTransitions transition, DeclarationRuleState target)
            var addMethod = stateCoreTransitions.Methods.AddNew(new TypedName("Add", stateCoreTransition));
            var amTransitionParam = addMethod.Parameters.AddNew(new TypedName(checkField.Name, checkField.FieldType));
            var amTargetParam = addMethod.Parameters.AddNew(new TypedName(targetField.Name, targetField.FieldType));
            addMethod.AccessLevel = DeclarationAccessLevel.Public;
            //        {
            //            TransitionData result = new TransitionData(transition, target);
            var addMethodResult = addMethod.Locals.AddNew(new TypedName("result", stateCoreTransition),
                    new CreateNewObjectExpression(stateCoreTransition.GetTypeReference(), amTransitionParam.GetReference(), amTargetParam.GetReference()));
            //            this.Add(result);
            addMethod.CallMethod(stateCoreTransitions.GetThisExpression().GetMethod("Add").Invoke(addMethodResult.GetReference()));

            //            return result;
            addMethod.Return(addMethodResult.GetReference());
            //        }
            #endregion
            //    }
            #endregion

            //    private TransitionSeries transitions;
            var transitionsField = stateCore.Fields.AddNew(new TypedName("transitions", stateCoreTransitions));
            //    public TransitionSeries Transitions
            //    {
            var transitionsProperty = stateCore.Properties.AddNew("Transitions", transitionsField.FieldType, true, false);
            transitionsProperty.AccessLevel = DeclarationAccessLevel.Public;
            //    get
            //    {

            //        if (this.transitions == null)
            //            this.transitions = new TransitionSeries();
            var transitionsNullCheck = transitionsProperty.GetPart.IfThen(new BinaryOperationExpression(transitionsField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            transitionsNullCheck.Assign(transitionsField.GetReference(), new CreateNewObjectExpression(transitionsField.FieldType));
            //        return this.transitions;
            transitionsProperty.GetPart.Return(transitionsField.GetReference());
            //    }
            //}

            #endregion

            #region Declaration Site Rule Definition

            //internal sealed class *FlatDFARoot :
            //    *FlatDFAState //* = rule prefix.
            var declarationRuleCore = assemblyChildSpace.Partials.AddNew().Classes.AddNew(string.Format("{0}FlatDFARule", target.Options.RulePrefix));
            declarationRuleCore.IsSealed = true;
            declarationRuleCore.AccessLevel = DeclarationAccessLevel.Internal;
            declarationRuleCore.BaseType = stateCore.GetTypeReference();
            //{


            #region Declaration Site State Ctor Extension

            //    private DeclarationSiteRule rule;
            var ruleField = stateCore.Fields.AddNew(new TypedName("rule", declarationRuleCore));

            //    public DeclarationSiteRule Rule
            var ruleProperty = stateCore.Properties.AddNew(new TypedName("Rule", declarationRuleCore), true, false);
            ruleProperty.AccessLevel = DeclarationAccessLevel.Public;
            //    {
            //        get
            //        {
            //            return this.rule;
            ruleProperty.GetPart.Return(ruleField.GetReference());
            //        }
            //    }

            // ..., DeclarationSiteRule rule)
            var ruleParam = stateCoreCtor.Parameters.AddNew(new TypedName("rule", declarationRuleCore));
            //...
            //    if (rule == null && this.GetType() == typeof(DeclarationSiteRule))
            //        this.rule = (DeclarationSiteRule)this;
            //    else
            //        this.rule = rule;
            //No 'expression is Type' functionality -_-.
            var ruleCheck = stateCoreCtor.Statements.IfThen(new BinaryOperationExpression(new BinaryOperationExpression(ruleParam.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue), CodeBinaryOperatorType.BooleanAnd, new BinaryOperationExpression(new ThisReferenceExpression().GetMethod("GetType").Invoke(), CodeBinaryOperatorType.IdentityEquality, new TypeOfExpression(declarationRuleCore))));
            ruleCheck.Assign(ruleField.GetReference(), new CastExpression(new ThisReferenceExpression(), declarationRuleCore.GetTypeReference()));
            ruleCheck.FalseBlock.Assign(ruleField.GetReference(), ruleParam.GetReference());
            //...

            #endregion


            //    private RuleIdentifier ruleID;
            var declRuleIdentifierField = declarationRuleCore.Fields.AddNew(new TypedName("ruleId", ruleIdentifierEnum));

            //    public RuleIdentifier RuleId
            var declRuleIdentifierProperty = declarationRuleCore.Properties.AddNew(new TypedName("RuleId", declRuleIdentifierField.FieldType), true, false);
            //    {
            //        get
            //        {

            //            return this.ruleID;
            declRuleIdentifierProperty.AccessLevel = DeclarationAccessLevel.Public;

            //        }
            //    }

            declRuleIdentifierProperty.GetPart.Return(declRuleIdentifierField.GetReference());


            //    public DeclarationSiteRule(bool isTerminalEdge, RuleIdentifier ruleID) 
            var declRuleCtor = declarationRuleCore.Constructors.AddNew();
            declRuleCtor.AccessLevel = DeclarationAccessLevel.Public;
            var ruleIsTerminalEdgeParam = declRuleCtor.Parameters.AddNew(new TypedName("isTerminalEdge", typeof(bool)));
            var ruleIdentifierParam = declRuleCtor.Parameters.AddNew(new TypedName("ruleId", ruleIdentifierEnum));
            //        : base(isTerminalEdge, null)
            declRuleCtor.CascadeExpressionsTarget = ConstructorCascadeTarget.Base;
            declRuleCtor.CascadeMembers.Add(ruleIsTerminalEdgeParam.GetReference());
            declRuleCtor.CascadeMembers.Add(PrimitiveExpression.NullValue);
            //    {

            //        this.ruleID = ruleID;
            declRuleCtor.Statements.Assign(declRuleIdentifierField.GetReference(), ruleIdentifierParam.GetReference());

            //    }

            //    private ReadOnlyCollection<DeclarationSiteState> edges;
            var declEdgesField = declarationRuleCore.Fields.AddNew(new TypedName("edges", typeof(ReadOnlyCollection<>).GetTypeReference(new TypeReferenceCollection(stateCore))));
            //    private List<DeclarationSiteState> _edges;
            var edgesField2 = declarationRuleCore.Fields.AddNew(new TypedName("_edges", typeof(List<>).GetTypeReference(new TypeReferenceCollection(stateCore))));
            edgesField2.InitializationExpression = new CreateNewObjectExpression(edgesField2.FieldType);

            //    public ReadOnlyCollection<DeclarationSiteState> Edges
            var declEdgesProperty = declarationRuleCore.Properties.AddNew(new TypedName("Edges", typeof(ReadOnlyCollection<>).GetTypeReference(new TypeReferenceCollection(stateCore))), true, false);
            //    {

            //        get
            //        {
            //            if (this.edges == null)
            //                this.edges = new ReadOnlyCollection<DeclarationSiteState>(this._edges);
            var edgesCheck = declEdgesProperty.GetPart.IfThen(new BinaryOperationExpression(declEdgesField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            edgesCheck.Assign(declEdgesField.GetReference(), new CreateNewObjectExpression(declEdgesField.FieldType, edgesField2.GetReference()));
            //                return this.edges;
            declEdgesProperty.GetPart.Return(declEdgesField.GetReference());
            //        }

            //    internal void SetEdges(DeclarationSiteState[] edges)
            //    {
            var declSetEdgesMethod = declarationRuleCore.Methods.AddNew(new TypedName("SetEdges", typeof(void)));
            declSetEdgesMethod.AccessLevel = DeclarationAccessLevel.Internal;
            var edgesParam = declSetEdgesMethod.Parameters.AddNew(new TypedName("edges", stateCore.GetTypeReference().MakeArray(1)));

            //        foreach (DeclarationSiteState randomNameHere in edgesParam)
            var edgesIteration = declSetEdgesMethod.Enumerate(edgesParam.GetReference(), stateCore.GetTypeReference());
            //        {
            //            _edges.Add(randomNameHere);
            edgesIteration.CallMethod(edgesField2.GetReference().GetMethod("Add").Invoke(edgesIteration.CurrentMember.GetReference()));
            //        }

            //    }
            //}
            #endregion

            #region FlatDFA Rule Stack Data
            var stateRulePushCore = stateCore.Classes.AddNew("RuleTransition");

            stateRulePushCore.AccessLevel = DeclarationAccessLevel.Internal;
            var statePushRuleIdField = stateRulePushCore.Fields.AddNew(new TypedName("id", ruleIdentifierEnum));
            statePushRuleIdField.AccessLevel = DeclarationAccessLevel.Private;
            var statePushRuleIdProperty = stateRulePushCore.Properties.AddNew(new TypedName("ID", ruleIdentifierEnum), true, false);
            statePushRuleIdProperty.GetPart.Return(statePushRuleIdField.GetReference());
            statePushRuleIdProperty.AccessLevel = DeclarationAccessLevel.Public;

            var statePushRuleFollowStateField = stateRulePushCore.Fields.AddNew(new TypedName("followState", stateCore));
            var statePushRuleFollowStateProperty = stateRulePushCore.Properties.AddNew(new TypedName("FollowState", stateCore), true, false);

            //var varPushRuleSourceStateField = stateRulePushCore.Fields.AddNew(new TypedName("", stateCore));
            statePushRuleFollowStateProperty.AccessLevel = DeclarationAccessLevel.Public;
            statePushRuleFollowStateProperty.GetPart.Return(statePushRuleFollowStateField.GetReference());

            var statePushRulePtrField = stateRulePushCore.Fields.AddNew(new TypedName("pointer", typeof(Func<>).GetTypeReference(new TypeReferenceCollection(declarationRuleCore.GetTypeReference()))));
            var statePushRuleField = stateRulePushCore.Fields.AddNew(new TypedName("ruleState", declarationRuleCore));
            var statePushRuleProperty = stateRulePushCore.Properties.AddNew(new TypedName("RuleState", declarationRuleCore), true, false);
            statePushRuleProperty.AccessLevel = DeclarationAccessLevel.Public;

            var statePushRuleCheck = statePushRuleProperty.GetPart.IfThen(new BinaryOperationExpression(statePushRuleField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            statePushRuleCheck.Assign(statePushRuleField.GetReference(), stateRulePushCore.GetThisExpression().GetMethod(statePushRulePtrField.Name).Invoke());
            statePushRuleCheck.Assign(statePushRulePtrField.GetReference(), PrimitiveExpression.NullValue);

            statePushRuleProperty.GetPart.Return(statePushRuleField.GetReference());

            stateCore.BaseType = typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(ruleIdentifierEnum.GetTypeReference(), stateRulePushCore.GetTypeReference()));

            var stateCoreAddMethod = stateCore.Methods.AddNew(new TypedName("Add", stateRulePushCore));
            var stateCoreAddRuleIDParam = stateCoreAddMethod.Parameters.AddNew(new TypedName("id", ruleIdentifierEnum));
            var stateCoreAddRulePointerParam = stateCoreAddMethod.Parameters.AddNew(new TypedName("pointer", statePushRulePtrField.FieldType));
            var stateCoreAddRuleInternalStateParam = stateCoreAddMethod.Parameters.AddNew(new TypedName("internalState", stateCore));

            var stateCoreAddMethodResultLocal = stateCoreAddMethod.Locals.AddNew(new TypedName("result", stateRulePushCore));
            stateCoreAddMethod.Assign(stateCoreAddMethodResultLocal.GetReference(), new CreateNewObjectExpression(stateRulePushCore.GetTypeReference(), stateCoreAddRuleIDParam.GetReference(), stateCoreAddRulePointerParam.GetReference(), stateCoreAddRuleInternalStateParam.GetReference()));
            stateCoreAddMethod.CallMethod(stateCore.GetThisExpression().GetMethod("Add").Invoke(stateCoreAddRuleIDParam.GetReference(), stateCoreAddMethodResultLocal.GetReference()));
            stateCoreAddMethod.Return(stateCoreAddMethodResultLocal.GetReference());
            stateCoreAddMethod.AccessLevel = DeclarationAccessLevel.Internal;

            var statePushRuleCtor = stateRulePushCore.Constructors.AddNew(
                new TypedName("id", ruleIdentifierEnum),
                new TypedName("pointer", statePushRulePtrField.FieldType),
                new TypedName("internalState", stateCore));
            var statePushRuleCtorIdParam = statePushRuleCtor.Parameters["id"];
            var statePushRuleCtorPointerParam = statePushRuleCtor.Parameters["pointer"];
            var statePushRuleCtorInternalStateParam = statePushRuleCtor.Parameters["internalState"];

            statePushRuleCtor.Statements.Assign(statePushRuleFollowStateField.GetReference(), statePushRuleCtorInternalStateParam.GetReference());
            statePushRuleCtor.Statements.Assign(statePushRuleIdField.GetReference(), statePushRuleCtorIdParam.GetReference());
            statePushRuleCtor.Statements.Assign(statePushRulePtrField.GetReference(), statePushRuleCtorPointerParam.GetReference());
            statePushRuleCtor.AccessLevel = DeclarationAccessLevel.Public;


            #endregion

            #region FlatDFA container
            var flatDFARules = assemblyChildSpace.Partials.AddNew().Classes.AddNew(string.Format("{0}FlatDFARules", target.Options.RulePrefix));
            flatDFARules.AccessLevel = DeclarationAccessLevel.Internal;
            #endregion

            #region Declaration Site Rule Builders
            Dictionary<FlattenedRuleEntry, IMethodMember> ruleDeclarationMethodMapping = new Dictionary<FlattenedRuleEntry, IMethodMember>();
            Dictionary<FlattenedRuleEntry, IPropertyMember> ruleDelegatePropertyMapping = new Dictionary<FlattenedRuleEntry, IPropertyMember>();
            Dictionary<SimpleLanguageBitArray, Tuple<int, IExpression>> checkIndexLookup = new Dictionary<SimpleLanguageBitArray, Tuple<int, IExpression>>();
            IFieldMember dstdField = flatDFARules.Fields.AddNew(new TypedName("dstd", typeof(List<>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType))));
            var staticCtor = flatDFARules.Constructors.AddNew();
            staticCtor.IsStatic = true;
            staticCtor.Statements.Assign(flatDFARules.GetTypeReference().GetTypeExpression().GetField(dstdField.Name), new CreateNewObjectExpression(dstdField.FieldType));

            dstdField.IsStatic = true;
            dstdField.AccessLevel = DeclarationAccessLevel.Private;
            dstdField.Summary = "Stores information about the declaration site transition data used by the rules.";
            int tCount = 0;
            var flatDFARulesPointerPart = flatDFARules.Partials.AddNew(stateCore.ParentTarget);
            foreach (var rule in ruleData)
            {
                var ruleDeclarationMethod = flatDFARules.Methods.AddNew(new TypedName(string.Format("Get{0}", rule.Key.Name), declarationRuleCore));
                var ruleDeclarationField = flatDFARules.Fields.AddNew(new TypedName(string.Format("_{0}", rule.Key.Name), ruleDeclarationMethod.ReturnType));
                var ruleDelegateField = flatDFARulesPointerPart.Fields.AddNew(new TypedName(string.Format("_{0}Pointer", rule.Key.Name), typeof(Func<>).GetTypeReference(new TypeReferenceCollection(declarationRuleCore.GetTypeReference()))));
                ruleDelegateField.AccessLevel = DeclarationAccessLevel.Private;
                ruleDelegateField.IsStatic = true;
                var ruleDelegateProperty = flatDFARulesPointerPart.Properties.AddNew(new TypedName(string.Format("{0}Pointer", rule.Key.Name), ruleDelegateField.FieldType), true, false);
                ruleDelegateProperty.IsStatic = true;
                ruleDeclarationField.AccessLevel = DeclarationAccessLevel.Private;
                ruleDeclarationField.IsStatic = true;
                ruleDeclarationMethod.AccessLevel = DeclarationAccessLevel.Internal;
                ruleDeclarationMethod.IsStatic = true;

                var ruleDelegateCheck = ruleDelegateProperty.GetPart.IfThen(new BinaryOperationExpression(ruleDelegateField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
                ruleDelegateProperty.AccessLevel = DeclarationAccessLevel.Internal;
                ruleDelegateCheck.Assign(ruleDelegateField.GetReference(), ruleDeclarationMethod.GetReference());
                ruleDelegateProperty.GetPart.Return(flatDFARules.GetTypeReference().GetTypeExpression().GetField(ruleDelegateField.Name));
                ruleDelegateProperty.AccessLevel = DeclarationAccessLevel.Internal;

                var ruleFieldCheck = ruleDeclarationMethod.IfThen(new BinaryOperationExpression(ruleDeclarationField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
                ruleDeclarationMethod.Return(ruleDeclarationField.GetReference());
                ruleDeclarationMethodMapping.Add(rule.Value, ruleDeclarationMethod);
                ruleDelegatePropertyMapping.Add(rule.Value, ruleDelegateProperty);
                IDictionary<SimpleLanguageState, IStatementBlockLocalMember> activationLocalLookup = new Dictionary<SimpleLanguageState, IStatementBlockLocalMember>();
                var flatForm = rule.Value.State.GetFlatform();
                if (!flatForm.Contains(rule.Value.State))
                    flatForm.Insert(0, rule.Value.State);
                else if (flatForm[0] != rule.Value.State)
                {
                    flatForm.Remove(rule.Value.State);
                    flatForm.Insert(0, rule.Value.State);
                }
#if false
                SyntaxActivationRuleItem ruleActivator = null;
                foreach (var item in sai.data.DataSet)
                {
                    var ruleItem = (item as SyntaxActivationRuleItem);
                    if (ruleItem == null)
                        continue;
                    if (ruleItem.Source == rule.Value)
                        ruleActivator = ruleItem;
                }
#endif
                foreach (var state in flatForm)
                {
                    if (state == rule.Value.State)
                        activationLocalLookup.Add(state, ruleFieldCheck.Locals.AddNew(new TypedName("rule", declarationRuleCore), new CreateNewObjectExpression(declarationRuleCore.GetTypeReference(), new PrimitiveExpression(state.IsEdge()), ruleIdentifierFieldLookup[rule.Value.Source].GetReference())));
                    else
                        activationLocalLookup.Add(state, ruleFieldCheck.Locals.AddNew(new TypedName(string.Format("State{0:X}", state.StateValue), stateCore), new CreateNewObjectExpression(stateCore.GetTypeReference(), new PrimitiveExpression(state.IsEdge()), activationLocalLookup[rule.Value.State].GetReference())));
                }
                foreach (var state in flatForm)
                {
                    var local = activationLocalLookup[state];
                    if (state.OutTransitions.Count > 0)
                    {
                        ruleFieldCheck.Statements.Add(new CommentStatement(string.Format("Transitions for state {0:X}", state.StateValue)));
                    }
                    foreach (var transition in state.OutTransitions)
                    {
                        bool tokens = true;
                        int transitionIndex = -1;
                        SimpleLanguageBitArray currentTransitionInfo = new SimpleLanguageBitArray(transition.Check);
                        SimpleLanguageBitArray[] ruleTransitions = null;
                    checkPart:
                        if (tokens)
                            currentTransitionInfo.DisposeRuleData();
                        else if (transitionIndex == -1)
                        {
                            currentTransitionInfo = new SimpleLanguageBitArray(transition.Check);
                            currentTransitionInfo.DisposeTokenData();
                            ruleTransitions = currentTransitionInfo.Shatter();
                            transitionIndex++;
                        }
                        if (!tokens)
                        {
                            if (ruleTransitions.Length > 0)
                            {
                                currentTransitionInfo = ruleTransitions[transitionIndex];
                                var currentRule = currentTransitionInfo.GetRuleRange()[0];
                                //Ugly, yet effective lookup.
                                //state.Add(RuleIdHere, RuleStatePointerHere, InternalStateHere);
                                ruleFieldCheck.CallMethod(local.GetReference().GetMethod("Add").Invoke(
                                    /*
                                    sai.data.RuleResults.TargetMapping[
                                        sai.data.RulePrintLookup[
                                            ruleData[
                                                currentRule]]].Member*/
                                    ruleIdentifierFieldLookup[currentRule].GetReference(),
                                    flatDFARules.GetTypeReference().GetTypeExpression().GetProperty(string.Format("{0}Pointer", currentRule.Name)),//ruleDelegatePropertyMapping[ruleData[currentRule]].GetReference()
                                    activationLocalLookup[transition.Targets[0]].GetReference()));
                            }
                        }
                        else if (!currentTransitionInfo.IsNullSet)
                        {
                            if (!checkIndexLookup.ContainsKey(currentTransitionInfo))
                            {
                                IExpression transitionExpression = null;
                                var sas = new SyntaxActivationScope(sai, currentTransitionInfo);
                                var activeItems = sas.GetActiveItems();
                                foreach (var item in activeItems)
                                {
                                    IExpression currentTransitionNode = sai.data.EnumResults.TargetMapping[sai.data.FootprintLookup[item]].Member.GetReference();
                                    if (transitionExpression == null)
                                        transitionExpression = currentTransitionNode;
                                    else
                                        transitionExpression = new BinaryOperationExpression(transitionExpression, CodeBinaryOperatorType.BitwiseOr, currentTransitionNode);
                                }
                                checkIndexLookup.Add(currentTransitionInfo, new Tuple<int, IExpression>(tCount++, transitionExpression));

                            }
                            ruleFieldCheck.Statements.Add(new CommentStatement(string.Format("Transition: {0}", transition.Check)));
                            ruleFieldCheck.CallMethod(local.GetReference().GetProperty("Transitions").GetMethod("Add").Invoke(flatDFARules.GetTypeReference().GetTypeExpression().GetField(dstdField.Name).GetIndex(new PrimitiveExpression(checkIndexLookup[currentTransitionInfo].Item)), activationLocalLookup[transition.Targets[0]].GetReference()));
                        }
                        if (tokens)
                        {
                            tokens = false;
                            goto checkPart;
                        }
                        else if (++transitionIndex < ruleTransitions.Length)
                            goto checkPart;
                    }
                }
                ruleFieldCheck.Statements.Add(new CommentStatement("Rule edge setup."));
                ICreateArrayExpression setEdgesArrayExp = new CreateArrayExpression(stateCore.GetTypeReference(), new IExpression[0]);
                foreach (var state in flatForm)
                {
                    if (state.IsEdge())
                        setEdgesArrayExp.Initializers.Add(activationLocalLookup[state].GetReference());
                }
                ruleFieldCheck.CallMethod(activationLocalLookup[rule.Value.State].GetReference().GetMethod("SetEdges").Invoke(setEdgesArrayExp));
                ruleFieldCheck.Assign(ruleDeclarationField.GetReference(), activationLocalLookup[rule.Value.State].GetReference());
            }
            foreach (var lookupData in checkIndexLookup)
            {
                staticCtor.Statements.Add(new CommentStatement(lookupData.Key.ToString()));
                staticCtor.Statements.CallMethod(dstdField.GetReference().GetMethod("Add").Invoke(lookupData.Value.Item2));
            }
            #endregion

            #region RuleIdentifier Breakdown (From RuleTransition)
            var sType = sai.data.EnumResults.ResultantType;
            var transitionHash = sai.data.EnumResults.ResultantType.Methods.AddNew(new TypedName("GetHashCode", typeof(int)));
            transitionHash.Overrides = true;
            transitionHash.AccessLevel = DeclarationAccessLevel.Public;
            transitionHash.IsFinal = false;
            var transitionHashIntersectionComplement = transitionHash.Locals.AddNew(new TypedName("intersection", typeof(int)), PrimitiveExpression.NumberZero);
            var transitionHashResult = transitionHash.Locals.AddNew(new TypedName("result", typeof(int)), PrimitiveExpression.NumberZero);


            sai.data.EnumResults.ResultantType.ImplementsList.Add(typeof(IEquatable<>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference())));

            var transitionStaticEquals = sai.data.EnumResults.ResultantType.Methods.AddNew(new TypedName("Equals", typeof(bool)));
            var transitionSLeftEq = transitionStaticEquals.Parameters.AddNew(new TypedName("left", sai.data.EnumResults.ResultantType));
            var transitionSRightEq = transitionStaticEquals.Parameters.AddNew(new TypedName("right", sai.data.EnumResults.ResultantType));
            transitionStaticEquals.IsStatic = true;
            transitionStaticEquals.AccessLevel = DeclarationAccessLevel.Public;

            var transitionEqualsOvl = sai.data.EnumResults.ResultantType.Coercions.AddNew(OverloadableBinaryOperators.IsEqualTo);
            transitionEqualsOvl.AccessLevel = DeclarationAccessLevel.Public;
            var transitionNotEqualOvl = sai.data.EnumResults.ResultantType.Coercions.AddNew(OverloadableBinaryOperators.IsNotEqualTo);
            transitionNotEqualOvl.AccessLevel = DeclarationAccessLevel.Public;

            var transitionEquals = sai.data.EnumResults.ResultantType.Methods.AddNew(new TypedName("Equals", typeof(bool)));
            var transitionOtherEq = transitionEquals.Parameters.AddNew(new TypedName("other", sai.data.EnumResults.ResultantType));
            transitionEquals.AccessLevel = DeclarationAccessLevel.Public;

            bool transitionFirst = true;
            foreach (var group in sai.data.EnumResults.FieldMapping.Keys)
            {
                var field = sai.data.EnumResults.FieldMapping[group].Member;
                if (transitionFirst)
                {
                    transitionFirst = false;
                    transitionHash.Assign(transitionHashResult.GetReference(), new CastExpression(sai.data.EnumResults.FieldMapping[group].Member.GetReference(), typeof(int).GetTypeReference()));
                }
                else
                {
                    transitionHash.Assign(transitionHashIntersectionComplement.GetReference(), new UnaryOperationExpression(UnaryOperations.Compliment, new BinaryOperationExpression(new CastExpression(sai.data.EnumResults.FieldMapping[group].Member.GetReference(), typeof(int).GetTypeReference()), CodeBinaryOperatorType.BitwiseAnd, transitionHashResult.GetReference())));
                    transitionHash.Assign(transitionHashResult.GetReference(), new BinaryOperationExpression(transitionHashIntersectionComplement.GetReference(), CodeBinaryOperatorType.BitwiseAnd, new BinaryOperationExpression(new CastExpression(sai.data.EnumResults.FieldMapping[group].Member.GetReference(), typeof(int).GetTypeReference()), CodeBinaryOperatorType.BitwiseOr, transitionHashResult.GetReference())));
                }
                var tseIfThen = transitionStaticEquals.IfThen(new BinaryOperationExpression(transitionSLeftEq.GetReference().GetField(field.Name), CodeBinaryOperatorType.IdentityInequality, transitionSRightEq.GetReference().GetField(field.Name)));
                tseIfThen.Return(PrimitiveExpression.FalseValue);
                var teIfThen = transitionEquals.IfThen(new BinaryOperationExpression(field.GetReference(), CodeBinaryOperatorType.IdentityInequality, transitionOtherEq.GetReference().GetField(field.Name)));
                teIfThen.Return(PrimitiveExpression.FalseValue);
                var teoIfThen = transitionEqualsOvl.IfThen(new BinaryOperationExpression(transitionEqualsOvl.LeftParameter.GetField(field.Name), CodeBinaryOperatorType.IdentityInequality, transitionEqualsOvl.RightParameter.GetField(field.Name)));
                teoIfThen.Return(PrimitiveExpression.FalseValue);
                var tneoIfThen = transitionNotEqualOvl.IfThen(new BinaryOperationExpression(transitionNotEqualOvl.LeftParameter.GetField(field.Name), CodeBinaryOperatorType.IdentityInequality, transitionNotEqualOvl.RightParameter.GetField(field.Name)));
                tneoIfThen.Return(PrimitiveExpression.TrueValue);
            }
            transitionStaticEquals.Return(PrimitiveExpression.TrueValue);
            transitionEquals.Return(PrimitiveExpression.TrueValue);
            transitionEqualsOvl.Return(PrimitiveExpression.TrueValue);
            transitionNotEqualOvl.Return(PrimitiveExpression.FalseValue);
            transitionHash.Return(transitionHashResult.GetReference());
            #endregion

            #region Collection/Dictionary Construction
            #region Collection
            IInterfaceType collectionType = assemblyChildSpace.Partials.AddNew().Interfaces.AddNew("IControlledStateCollection");
            var collectionTParam = collectionType.TypeParameters.AddNew("T").GetTypeReference();
            collectionType.AccessLevel = DeclarationAccessLevel.Internal;
            var collectionCountProperty = collectionType.Properties.AddNew(new TypedName("Count", typeof(int)), true, false);
            var collectionContainsMethod = collectionType.Methods.AddNew(new TypedName("Contains", typeof(bool)));
            var collectionContainsElementParam = collectionContainsMethod.Parameters.AddNew(new TypedName("value", collectionTParam));

            var collectionCopyToMethod = collectionType.Methods.AddNew(new TypedName("CopyTo", typeof(void)));
            var collectionCopyToArrayParam = collectionCopyToMethod.Parameters.AddNew(new TypedName("array", collectionTParam.MakeArray(1)));
            var collectionCopyToArrayIndexParam = collectionCopyToMethod.Parameters.AddNew(new TypedName("arrayIndex", typeof(int)));

            var collectionIndexer = collectionType.Properties.AddNew(collectionTParam, true, false, new TypedName("index", typeof(int)));
            var collectionIndexerIndex = collectionIndexer.Parameters["index"];

            var colectionToArrayMethod = collectionType.Methods.AddNew(new TypedName("ToArray", collectionCopyToArrayParam.ParameterType));

            collectionType.ImplementsList.Add(typeof(IEnumerable<>).GetTypeReference(new TypeReferenceCollection(collectionTParam)));
            #endregion
            #region Dictionary
            IInterfaceType dictionaryType = assemblyChildSpace.Partials.AddNew().Interfaces.AddNew("IControlledStateDictionary");
            var dictionaryKeyParam = dictionaryType.TypeParameters.AddNew("TKey").GetTypeReference();
            var dictionaryValueParam = dictionaryType.TypeParameters.AddNew("TValue").GetTypeReference();
            dictionaryType.ImplementsList.Add(collectionType.GetTypeReference(typeof(KeyValuePair<,>).GetTypeReference(new TypeReferenceCollection(dictionaryKeyParam, dictionaryValueParam))));
            var dictionaryKeysProperty = dictionaryType.Properties.AddNew(new TypedName("Keys", collectionType.GetTypeReference(dictionaryKeyParam)), true, false);
            var dictionaryValuesProperty = dictionaryType.Properties.AddNew(new TypedName("Values", collectionType.GetTypeReference(dictionaryValueParam)), true, false);

            var dictionaryContainsKeyMethod = dictionaryType.Methods.AddNew(new TypedName("ContainsKey", typeof(bool)));
            var dictionaryContainsKeyParam = dictionaryContainsKeyMethod.Parameters.AddNew(new TypedName("key", dictionaryKeyParam));

            var dictionaryTryGetValueMethod = dictionaryType.Methods.AddNew(new TypedName("TryGetValue", typeof(bool)));
            dictionaryTryGetValueMethod.Parameters.AddNew(new TypedName("key", dictionaryKeyParam));
            var dictionaryTryGetValueValueParam = dictionaryTryGetValueMethod.Parameters.AddNew(new TypedName("value", dictionaryValueParam));
            dictionaryTryGetValueValueParam.Direction = FieldDirection.Out;
            #endregion
            #endregion

            #region Stream State Construction

            var streamState = assemblyChildSpace.Partials.AddNew().Classes.AddNew(string.Format("{0}StreamState", target.Options.RulePrefix));
            streamState.AccessLevel = DeclarationAccessLevel.Internal;
            var streamStatePath = streamState.Partials.AddNew().Classes.AddNew("StatePath");
            streamStatePath.AccessLevel = DeclarationAccessLevel.Internal;
            var streamRulePath = streamState.Partials.AddNew().Classes.AddNew("RulePath");
            streamRulePath.AccessLevel = DeclarationAccessLevel.Internal;
            streamRulePath.BaseType = streamStatePath.GetTypeReference();

            const string isRulePropertyName = "IsRule";
            const string rulePropertyName = "Rule";
            const string ruleFieldName = "rule";
            const string originalFieldName = "original";
            const string originalPropertyName = "Original";
            const string otherParam = "other";
            const string equalsMethodName = "Equals";
            const string parentFieldName = "parent";
            const string parentPropertyName = "Parent";
            //const string lookAheadName = "LookAhead";
            //const string sourceTypeFieldName = "sourcedFrom";
            //const string sourceTypePropertyName = "SourcedFrom";
            const string followFieldName = "follow";
            const string followPropertyName = "Follow";
            const string followSetFieldName = "followSet";
            const string followSetPropertyName = "FollowSet";
            const string followTypeName = "FollowInfo";
            const string otherParamName = "other";
            const string getHashCodeMethodName = "GetHashCode";

            //var streamPathSourceEnum = assemblyChildSpace.Partials.AddNew().Enumerators.AddNew(string.Format("{0}PathSource", target.Options.RulePrefix));

            const string sourcesBuilderName = "SourcesBuilder";
            const string initialSetFieldName = "initialSet";
            const string initialSetPropertyName = "InitialSet";
            const string firstSetFieldName = "firstSet";
            const string firstSetPropertyName = "FirstSet";
            const string fullSetFieldName = "fullSet";
            const string fullSetPropertyName = "FullSet";
            var streamStateSources = streamState.Partials.AddNew().Classes.AddNew("SourcesInfo");
            streamStateSources.AccessLevel = DeclarationAccessLevel.Public;
            var streamStatePathArray = streamStatePath.GetTypeReference().MakeArray(1);
            var streamStateSourcesField = streamState.Fields.AddNew(new TypedName("sources", streamStateSources));
            var streamStateSourcesProperty = streamState.Properties.AddNew(new TypedName("Sources", streamStateSources), true, false);
            streamStateSourcesProperty.GetPart.Return(streamStateSourcesField.GetReference());
            streamStateSourcesProperty.AccessLevel = DeclarationAccessLevel.Public;
            var streamStateCtor = streamState.Constructors.AddNew();
            streamStateCtor.AccessLevel = DeclarationAccessLevel.Private;
            var streamStateCtorSourcesParam = streamStateCtor.Parameters.AddNew(new TypedName("sources", streamStateSources));
            streamStateCtor.Statements.Assign(streamStateSourcesField.GetReference(), streamStateCtorSourcesParam.GetReference());

            var streamStateSourcesInitialSetField = streamStateSources.Fields.AddNew(new TypedName(initialSetFieldName, streamStatePathArray));
            var streamStateSourcesFirstSetField = streamStateSources.Fields.AddNew(new TypedName(firstSetFieldName, streamStatePathArray));
            var streamStateSourcesFollowSetField = streamStateSources.Fields.AddNew(new TypedName(followSetFieldName, streamStatePathArray));
            var streamStateSourcesInitialSetProperty = streamStateSources.Properties.AddNew(new TypedName(initialSetPropertyName, streamStatePathArray), true, false);
            streamStateSourcesInitialSetProperty.AccessLevel = DeclarationAccessLevel.Public;
            streamStateSourcesInitialSetProperty.GetPart
                .Return(streamStateSourcesInitialSetField.GetReference());
            var streamStateSourcesFirstSetProperty = streamStateSources.Properties.AddNew(new TypedName(firstSetPropertyName, streamStatePathArray), true, false);
            streamStateSourcesFirstSetProperty.AccessLevel = DeclarationAccessLevel.Public;
            streamStateSourcesFirstSetProperty.GetPart
                .Return(streamStateSourcesFirstSetField.GetReference());
            var streamStateSourcesFollowSetProperty = streamStateSources.Properties.AddNew(new TypedName(followSetPropertyName, streamStatePathArray), true, false);
            streamStateSourcesFollowSetProperty.AccessLevel = DeclarationAccessLevel.Public;
            streamStateSourcesFollowSetProperty.GetPart
                .Return(streamStateSourcesFollowSetField.GetReference());

            var streamStateSourcesCtor = streamStateSources.Constructors.AddNew();
            streamStateSourcesCtor.AccessLevel = DeclarationAccessLevel.Internal;
            var streamStateSourcesCtorInitialSetParam = streamStateSourcesCtor.Parameters.AddNew(new TypedName("initialSet", streamStatePathArray));
            var streamStateSourcesCtorFirstSetParam = streamStateSourcesCtor.Parameters.AddNew(new TypedName("firstSet", streamStatePathArray));
            var streamStateSourcesCtorFollowSetParam = streamStateSourcesCtor.Parameters.AddNew(new TypedName("followSet", streamStatePathArray));
            streamStateSourcesCtor.Statements
                .Assign(streamStateSourcesInitialSetField.GetReference(), streamStateSourcesCtorInitialSetParam.GetReference());
            streamStateSourcesCtor.Statements
                .Assign(streamStateSourcesFirstSetField.GetReference(), streamStateSourcesCtorFirstSetParam.GetReference());
            streamStateSourcesCtor.Statements
                .Assign(streamStateSourcesFollowSetField.GetReference(), streamStateSourcesCtorFollowSetParam.GetReference());


            //var pathSourceOriginal = streamPathSourceEnum.Fields.AddNew(originalPropertyName);
            //var pathSourceLookAhead = streamPathSourceEnum.Fields.AddNew(lookAheadName);
            var followInfoCore = streamRulePath.Partials.AddNew().Classes.AddNew(followTypeName);
            followInfoCore.AccessLevel = DeclarationAccessLevel.Internal;
            followInfoCore.ImplementsList.Add(typeof(IEquatable<>).GetTypeReference(new TypeReferenceCollection(followInfoCore)));
            var followInfoRuleField = followInfoCore.Fields.AddNew(new TypedName(ruleFieldName, streamRulePath));
            var followInfoRuleProperty = followInfoCore.Properties.AddNew(new TypedName(rulePropertyName, streamRulePath), true, false);
            followInfoRuleProperty.Summary = "Returns the path to the rule which originally spawned the rule owning the current <see cref=\"FollowInfo\"/>.";
            followInfoRuleProperty.GetPart.Return(followInfoRuleField.GetReference());
            followInfoRuleProperty.AccessLevel = DeclarationAccessLevel.Public;
            var followInfoFollowField = followInfoCore.Fields.AddNew(new TypedName(followFieldName, stateCore));
            var followInfoFollowProperty = followInfoCore.Properties.AddNew(new TypedName(followPropertyName, stateCore), true, false);
            followInfoFollowProperty.Summary = "Returns the state to merge into the source set when the rule hits a terminal edge.";
            followInfoFollowProperty.AccessLevel = DeclarationAccessLevel.Public;
            followInfoFollowProperty.GetPart.Return(followInfoFollowField.GetReference());

            var followInfoCtor = followInfoCore.Constructors.AddNew();
            var followInfoCtorRuleParam = followInfoCtor.Parameters.AddNew(new TypedName(ruleFieldName, streamRulePath));
            var followInfoCtorFollowParam = followInfoCtor.Parameters.AddNew(new TypedName(followFieldName, stateCore));
            followInfoCtor.Statements.Assign(followInfoRuleField.GetReference(), followInfoCtorRuleParam.GetReference());
            followInfoCtor.Statements.Assign(followInfoFollowField.GetReference(), followInfoCtorFollowParam.GetReference());
            followInfoCtor.AccessLevel = DeclarationAccessLevel.Public;

            var buildTransition = streamState.Partials.AddNew().Classes.AddNew("BuildTransition");
            buildTransition.AccessLevel = DeclarationAccessLevel.Private;
            var parentChildPair = buildTransition.Partials.AddNew().Classes.AddNew("ParentChildPairing");
            parentChildPair.AccessLevel = DeclarationAccessLevel.Internal;
            buildTransition.BaseType = typeof(List<>).GetTypeReference(new TypeReferenceCollection(parentChildPair.GetTypeReference()));
            var parentChildParentField = parentChildPair.Fields.AddNew(new TypedName(parentFieldName, stateCore));
            var parentChildParentProperty = parentChildPair.Properties.AddNew(new TypedName("Parent", stateCore), true, false);
            parentChildParentProperty.AccessLevel = DeclarationAccessLevel.Public;
            var parentChildChildField = parentChildPair.Fields.AddNew(new TypedName("child", stateCore));
            var parentChildChildProperty = parentChildPair.Properties.AddNew(new TypedName("Child", stateCore), true, false);
            parentChildChildProperty.AccessLevel = DeclarationAccessLevel.Public;
            parentChildParentProperty.GetPart.Return(parentChildParentField.GetReference());
            parentChildChildProperty.GetPart.Return(parentChildChildField.GetReference());

            var parentChildCtor = parentChildPair.Constructors.AddNew(new TypedName("parent", stateCore), new TypedName("child", stateCore));
            parentChildCtor.Statements.Assign(parentChildParentField.GetReference(), parentChildCtor.Parameters["parent"].GetReference());
            parentChildCtor.Statements.Assign(parentChildChildField.GetReference(), parentChildCtor.Parameters["child"].GetReference());
            parentChildCtor.AccessLevel = DeclarationAccessLevel.Internal;

            var buildTransitionCtor1 = buildTransition.Constructors.AddNew(new TypedName("parent", stateCore), new TypedName("child", stateCore));
            var buildTransitionCtor1Parent = buildTransitionCtor1.Parameters["parent"];
            var buildTransitionCtor1Child = buildTransitionCtor1.Parameters["child"];
            buildTransitionCtor1.AccessLevel = DeclarationAccessLevel.Internal;
            buildTransitionCtor1.Statements.CallMethod(new BaseReferenceExpression().GetMethod("Add").Invoke(new CreateNewObjectExpression(parentChildPair.GetTypeReference(), buildTransitionCtor1Parent.GetReference(), buildTransitionCtor1Child.GetReference())));

            var buildTransitionCtor2 = buildTransition.Constructors.AddNew(new TypedName(originalFieldName, buildTransition));
            buildTransitionCtor2.AccessLevel = DeclarationAccessLevel.Internal;
            buildTransitionCtor2.CascadeExpressionsTarget = ConstructorCascadeTarget.Base;
            buildTransitionCtor2.CascadeMembers.Add(buildTransitionCtor2.Parameters[originalFieldName].GetReference());

            var streamRulePathCtor = streamRulePath.Constructors.AddNew(new TypedName(originalFieldName, declarationRuleCore));
            var streamRulePathCtorOriginal = streamRulePathCtor.Parameters[originalFieldName];
            streamRulePathCtor.CascadeExpressionsTarget = ConstructorCascadeTarget.Base;
            streamRulePathCtor.CascadeMembers.Add(streamRulePathCtorOriginal.GetReference());


            var streamRulePathCtor2 = streamRulePath.Constructors.AddNew(
                new TypedName(originalFieldName, declarationRuleCore),
                new TypedName(parentFieldName, streamStatePath),
                new TypedName("followParent", streamRulePath),
                new TypedName(followFieldName, stateCore));



            var followInfoEquals = followInfoCore.Methods.AddNew(new TypedName(equalsMethodName, typeof(bool)));
            var followInfoEqualsOther = followInfoEquals.Parameters.AddNew(new TypedName(otherParamName, followInfoCore));
            followInfoEquals.AccessLevel = DeclarationAccessLevel.Public;

            // if (!(rule.Equals(other.rule)))
            var followInfoEqualsRuleCheck = followInfoEquals.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, followInfoRuleField.GetReference().GetMethod("Equals").Invoke(followInfoEqualsOther.GetReference().GetField(followInfoRuleField.Name))));
            //     return false;
            followInfoEqualsRuleCheck.Return(PrimitiveExpression.FalseValue);
            // if (follow  != other.follow)
            var followInfoEqualsFollowCheck = followInfoEquals.IfThen(new BinaryOperationExpression(followInfoFollowField.GetReference(), CodeBinaryOperatorType.IdentityInequality, followInfoEqualsOther.GetReference().GetField(followInfoFollowField.Name)));
            //     return false;
            followInfoEqualsFollowCheck.Return(PrimitiveExpression.FalseValue);

            // return true;
            followInfoEquals.Return(PrimitiveExpression.TrueValue);

            var followInfoEqualsOvl = followInfoCore.Methods.AddNew(new TypedName(equalsMethodName, typeof(bool)));
            var followInfoEqualsOvlOther = followInfoEqualsOvl.Parameters.AddNew(new TypedName(otherParamName, typeof(object)));
            var followInfoEqualsOvlNullCheck = followInfoEqualsOvl.IfThen(new BinaryOperationExpression(followInfoEqualsOvlOther.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            followInfoEqualsOvlNullCheck.Return(PrimitiveExpression.FalseValue);
            var followInfoEqualsOvlTypeCheck = followInfoEqualsOvl.IfThen(followInfoEqualsOvlOther.GetReference().GetMethod("GetType").Invoke().GetMethod("IsSubclassOf").Invoke(new TypeOfExpression(followInfoCore.GetTypeReference())));
            followInfoEqualsOvlTypeCheck.Return(followInfoEquals.GetReference().Invoke(new CastExpression(followInfoEqualsOvlOther.GetReference(), followInfoCore.GetTypeReference())));
            followInfoEqualsOvl.Return(PrimitiveExpression.FalseValue);

            followInfoEqualsOvl.AccessLevel = DeclarationAccessLevel.Public;
            followInfoEqualsOvl.Overrides = true;
            followInfoEqualsOvl.IsFinal = false;

            var followInfoGetHashCode = followInfoCore.Methods.AddNew(new TypedName(getHashCodeMethodName, typeof(int)));
            followInfoGetHashCode.Overrides = true;
            followInfoGetHashCode.AccessLevel = DeclarationAccessLevel.Public;
            followInfoGetHashCode.IsFinal = false;

            var followInfoGetHashCodeResult = followInfoGetHashCode.Locals.AddNew(new TypedName("result", typeof(int)), followInfoRuleField.GetReference().GetMethod("GetHashCode").Invoke());

            //var followInfoGetHashCodeFollowCheck = followInfoGetHashCode.IfThen(new BinaryOperationExpression(followInfoFollowField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            //followInfoGetHashCodeFollowCheck.Return(followInfoGetHashCodeResult.GetReference());

            var followInfoGetHashCodeFollowCode = followInfoGetHashCode.Locals.AddNew(new TypedName("followCode", typeof(int)), followInfoFollowField.GetReference().GetMethod("GetHashCode").Invoke());
            var followInfoGetHashCodeIntersection = followInfoGetHashCode.Locals.AddNew(new TypedName("intersection", typeof(int)), new UnaryOperationExpression(UnaryOperations.Compliment, new BinaryOperationExpression(followInfoGetHashCodeFollowCode.GetReference(), CodeBinaryOperatorType.BitwiseAnd, followInfoGetHashCodeResult.GetReference())));
            followInfoGetHashCodeFollowCode.AutoDeclare = false;
            followInfoGetHashCodeIntersection.AutoDeclare = false;
            followInfoGetHashCode.DefineLocal(followInfoGetHashCodeFollowCode);
            followInfoGetHashCode.DefineLocal(followInfoGetHashCodeIntersection);

            followInfoGetHashCode.Return(new BinaryOperationExpression(followInfoGetHashCodeIntersection.GetReference(), CodeBinaryOperatorType.BitwiseAnd, new BinaryOperationExpression(followInfoGetHashCodeResult.GetReference(), CodeBinaryOperatorType.BitwiseOr, followInfoGetHashCodeFollowCode.GetReference())));
            var listOfFollowInfoCore = typeof(List<>).GetTypeReference(new TypeReferenceCollection(followInfoCore.GetTypeReference()));
            var streamRuleFollowSetField = streamRulePath.Fields.AddNew(new TypedName(followSetFieldName, listOfFollowInfoCore));
            streamRuleFollowSetField.InitializationExpression = new CreateNewObjectExpression(streamRuleFollowSetField.FieldType);
            var streamRuleFollowSetProperty = streamRulePath.Properties.AddNew(new TypedName(followSetPropertyName, streamRuleFollowSetField.FieldType), true, false);
            streamRuleFollowSetProperty.GetPart.Return(streamRuleFollowSetField.GetReference());
            streamRuleFollowSetProperty.AccessLevel = DeclarationAccessLevel.Public;

            var streamStateIsRule = streamStatePath.Properties.AddNew(new TypedName(isRulePropertyName, typeof(bool)), true, false);
            var streamRuleIsRule = streamRulePath.Properties.AddNew(new TypedName(isRulePropertyName, typeof(bool)), true, false);
            streamStateIsRule.AccessLevel = DeclarationAccessLevel.Public;
            streamStateIsRule.IsVirtual = true;
            streamStateIsRule.GetPart.Return(PrimitiveExpression.FalseValue);
            streamRuleIsRule.AccessLevel = DeclarationAccessLevel.Public;
            streamRuleIsRule.Overrides = true;
            streamRuleIsRule.IsFinal = false;
            streamRuleIsRule.GetPart.Return(PrimitiveExpression.TrueValue);

            var streamStateRuleField = streamStatePath.Fields.AddNew(new TypedName(ruleFieldName, streamRulePath));
            var streamStateRuleProperty = streamStatePath.Properties.AddNew(new TypedName(rulePropertyName, streamRulePath), true, false);
            streamStateRuleProperty.AccessLevel = DeclarationAccessLevel.Public;
            streamStateRuleProperty.IsVirtual = true;

            var streamRuleRule = streamRulePath.Properties.AddNew(new TypedName(rulePropertyName, streamRulePath), true, false);
            streamRuleRule.GetPart.Return(streamRulePath.GetThisExpression());
            streamRuleRule.Overrides = true;
            streamRuleRule.IsFinal = false;
            streamRuleRule.AccessLevel = DeclarationAccessLevel.Public;
            streamStateRuleProperty.GetPart.Return(streamStateRuleField.GetReference());

            var streamOriginalField = streamStatePath.Fields.AddNew(new TypedName(originalFieldName, stateCore));
            var streamOriginalProperty = streamStatePath.Properties.AddNew(new TypedName(originalPropertyName, stateCore), true, false);
            streamOriginalProperty.AccessLevel = DeclarationAccessLevel.Public;

            streamOriginalProperty.GetPart.Return(streamOriginalField.GetReference());


            var streamStatePathCtor = streamStatePath.Constructors.AddNew(new TypedName(originalFieldName, stateCore));
            var streamStatePathOriginalParam = streamStatePathCtor.Parameters[originalFieldName];
            streamStatePathCtor.AccessLevel = DeclarationAccessLevel.Public;

            streamStatePathCtor.Statements.Assign(streamOriginalField.GetReference(), streamStatePathOriginalParam.GetReference());

            var streamStatePathCtor2 = streamStatePath.Constructors.AddNew(
                new TypedName(originalFieldName, stateCore),
                new TypedName(ruleFieldName, streamRulePath));
            var streamStatePathCtor2OriginalParam = streamStatePathCtor2.Parameters[originalFieldName];
            var streamStatePathCtor2RuleParam = streamStatePathCtor2.Parameters[ruleFieldName];
            streamStatePathCtor2.CascadeMembers.Add(streamStatePathCtor2OriginalParam.GetReference());
            streamStatePathCtor2.CascadeExpressionsTarget = ConstructorCascadeTarget.This;
            streamStatePathCtor2.Statements.Assign(streamStateRuleField.GetReference(), streamStatePathCtor2RuleParam.GetReference());
            streamStatePathCtor2.AccessLevel = DeclarationAccessLevel.Public;

            var streamRuleOriginalProperty = streamRulePath.Properties.AddNew(new TypedName(originalPropertyName, declarationRuleCore), true, false);
            streamRuleOriginalProperty.AccessLevel = DeclarationAccessLevel.Public;
            streamRuleOriginalProperty.HidesPrevious = true;
            streamRuleOriginalProperty.GetPart.Return(new CastExpression(new BaseReferenceExpression().GetProperty(originalPropertyName), declarationRuleCore.GetTypeReference()));

            var streamRuleParentField = streamRulePath.Fields.AddNew(new TypedName(parentFieldName, streamStatePath));

            var streamRuleParentProperty = streamRulePath.Properties.AddNew(new TypedName(parentPropertyName, streamStatePath), true, false);
            streamRuleParentProperty.AccessLevel = DeclarationAccessLevel.Public;
            streamRuleParentProperty.GetPart.Return(streamRuleParentField.GetReference());

            var streamRulePathCtor2Original = streamRulePathCtor2.Parameters[originalFieldName];
            var streamRulePathCtor2Parent = streamRulePathCtor2.Parameters[parentFieldName];
            var streamRulePathCtor2FollowParent = streamRulePathCtor2.Parameters["followParent"];
            var streamRulePathCtor2Follow = streamRulePathCtor2.Parameters[followFieldName];

            streamRulePathCtor2.CascadeExpressionsTarget = ConstructorCascadeTarget.This;
            streamRulePathCtor2.CascadeMembers.Add(streamRulePathCtor2Original.GetReference());
            streamRulePathCtor2.Statements.Assign(streamRuleParentField.GetReference(), streamRulePathCtor2Parent.GetReference());
            streamRulePathCtor.AccessLevel = DeclarationAccessLevel.Public;
            streamRulePathCtor2.AccessLevel = DeclarationAccessLevel.Public;

            streamRulePathCtor2.Statements.CallMethod(
                streamRuleFollowSetField.GetReference().
                    GetMethod("Add").Invoke(new CreateNewObjectExpression(followInfoCore.GetTypeReference(),
                        streamRulePathCtor2FollowParent.GetReference(),
                        streamRulePathCtor2Follow.GetReference())));

            streamStatePath.ImplementsList.Add(typeof(IEquatable<>).GetTypeReference(new TypeReferenceCollection(streamStatePath.GetTypeReference())));

            var streamStateEquals = streamStatePath.Methods.AddNew(new TypedName(equalsMethodName, typeof(bool)));
            streamStateEquals.AccessLevel = DeclarationAccessLevel.Public;
            streamStateEquals.IsVirtual = true;

            var streamStateOther = streamStateEquals.Parameters.AddNew(new TypedName(otherParam, streamStatePath.GetTypeReference()));
            var streamStateNullCheck = streamStateEquals.IfThen(new BinaryOperationExpression(streamStateOther.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            streamStateNullCheck.Return(PrimitiveExpression.FalseValue);

            var streamStateRefCheck = streamStateEquals.IfThen(typeof(object).GetTypeReferenceExpression().GetMethod("ReferenceEquals").Invoke(streamStateOther.GetReference(), new ThisReferenceExpression()));

            streamStateRefCheck.Return(PrimitiveExpression.TrueValue);
            var streamStateOriginalCheck = streamStateEquals.IfThen(new BinaryOperationExpression(streamOriginalField.GetReference(), CodeBinaryOperatorType.IdentityInequality, streamStateOther.GetReference().GetField(streamOriginalField.Name)));
            streamStateOriginalCheck.Return(PrimitiveExpression.FalseValue);
            var streamStateRuleCheck = streamStateEquals.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, new BinaryOperationExpression(typeof(object).GetTypeReferenceExpression().GetMethod("ReferenceEquals").Invoke(streamStateRuleField.GetReference(), PrimitiveExpression.NullValue), CodeBinaryOperatorType.BooleanOr, streamStateRuleProperty.GetReference().GetMethod(equalsMethodName).Invoke(streamStateOther.GetReference().GetProperty(streamStateRuleProperty.Name)))));
            streamStateRuleCheck.Return(PrimitiveExpression.FalseValue);
            streamStateEquals.Return(PrimitiveExpression.TrueValue);

            streamRulePath.ImplementsList.Add(typeof(IEquatable<>).GetTypeReference(new TypeReferenceCollection(streamRulePath.GetTypeReference())));
            var streamRuleEquals = streamRulePath.Methods.AddNew(new TypedName(equalsMethodName, typeof(bool)));
            streamRuleEquals.AccessLevel = DeclarationAccessLevel.Public;

            var streamRuleOther = streamRuleEquals.Parameters.AddNew(new TypedName(otherParam, streamRulePath));

            var streamRuleBaseCheck = streamRuleEquals.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, new BaseReferenceExpression().GetMethod(equalsMethodName).Invoke(streamRuleOther.GetReference())));
            streamRuleBaseCheck.Return(PrimitiveExpression.FalseValue);
            var streamRuleParentCheck = streamRuleEquals.IfThen(new BinaryOperationExpression(streamRuleParentField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            var streamRuleOtherParentCheck = streamRuleParentCheck.IfThen(new BinaryOperationExpression(streamRuleOther.GetReference().GetField(streamRuleParentField.Name), CodeBinaryOperatorType.IdentityInequality, PrimitiveExpression.NullValue));
            streamRuleOtherParentCheck.Return(PrimitiveExpression.FalseValue);
            streamRuleOtherParentCheck.FalseBlock.Return(PrimitiveExpression.TrueValue);
            streamRuleEquals.Return(streamRuleParentField.GetReference().GetMethod(equalsMethodName).Invoke(streamRuleOther.GetReference().GetField(streamRuleParentField.Name)));

            var streamRuleEqualsOv = streamRulePath.Methods.AddNew(new TypedName(equalsMethodName, typeof(bool)));
            var streamRuleOtherOv = streamRuleEqualsOv.Parameters.AddNew(new TypedName(otherParam, streamStatePath));
            streamRuleEqualsOv.AccessLevel = DeclarationAccessLevel.Public;
            streamRuleEqualsOv.Overrides = true;
            streamRuleEqualsOv.IsFinal = false;
            var streamRuleOvNullCheck = streamRuleEqualsOv.IfThen(new BinaryOperationExpression(streamRuleOtherOv.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            streamRuleOvNullCheck.Return(PrimitiveExpression.FalseValue);
            var streamRuleOvRuleCheck = streamRuleEqualsOv.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, streamRuleOtherOv.GetReference().GetProperty(streamRuleIsRule.Name)));

            streamRuleOvRuleCheck.Return(PrimitiveExpression.FalseValue);

            streamRuleEqualsOv.Return(streamRuleEquals.GetReference().Invoke(new CastExpression(streamRuleOtherOv.GetReference(), streamRulePath.GetTypeReference())));

            //var streamPathSourceField = streamStatePath.Fields.AddNew(new TypedName(sourceTypeFieldName, streamPathSourceEnum));
            //var streamPathSourceProperty = streamStatePath.Properties.AddNew(new TypedName(sourceTypePropertyName, streamPathSourceEnum), true, false);

            //streamPathSourceProperty.GetPart.Return(streamPathSourceField.GetReference());
            //streamPathSourceProperty.Summary = "Returns the point from which the path was sourced, either\r\nits original source, a source from obtaining the lookahead,\r\nor a source from the follow set.";


            const string lineContainsMethodName = "LineContains";
            BuildRulePathLineContainsMethod(ruleIdentifierEnum, stateCore, streamStatePath, streamRulePath, isRulePropertyName, originalPropertyName, parentPropertyName, followSetPropertyName, followInfoCore, lineContainsMethodName);

            const string addFirstMethodName = "AddFirst";
            const string addFollowMethodName = "AddFollow";
            const string firstParamName = "first";
            const string getSourcesMethodName = "GetSources";

            var sourcesBuilderCore = streamState.Partials.AddNew().Classes.AddNew(sourcesBuilderName);
            sourcesBuilderCore.AccessLevel = DeclarationAccessLevel.Private;
            var listOfStatePath = typeof(List<>).GetTypeReference(new TypeReferenceCollection(streamStatePath.GetTypeReference()));
            var builderFirstSetField = sourcesBuilderCore.Fields.AddNew(new TypedName(firstSetFieldName, listOfStatePath));
            var builderFollowSetField = sourcesBuilderCore.Fields.AddNew(new TypedName(followSetFieldName, listOfStatePath));
            var builderInitialSetField = sourcesBuilderCore.Fields.AddNew(new TypedName(initialSetFieldName, listOfStatePath));
            var builderFullSetField = sourcesBuilderCore.Fields.AddNew(new TypedName(fullSetFieldName, listOfStatePath));
            var builderInitialSetProperty = sourcesBuilderCore.Properties.AddNew(new TypedName(initialSetPropertyName, listOfStatePath), true, false);
            var builderFirstSetProperty = sourcesBuilderCore.Properties.AddNew(new TypedName(firstSetPropertyName, listOfStatePath), true, false);
            var builderFollowSetProperty = sourcesBuilderCore.Properties.AddNew(new TypedName(followSetPropertyName, listOfStatePath), true, false);
            var builderFullSetProperty = sourcesBuilderCore.Properties.AddNew(new TypedName(fullSetPropertyName, listOfStatePath), true, false);
            builderFirstSetField.InitializationExpression = new CreateNewObjectExpression(builderFirstSetField.FieldType);
            builderInitialSetField.InitializationExpression = builderFirstSetField.InitializationExpression;
            builderFollowSetField.InitializationExpression = builderFirstSetField.InitializationExpression;
            builderFullSetField.InitializationExpression = builderFirstSetField.InitializationExpression;
            builderInitialSetProperty.AccessLevel = DeclarationAccessLevel.Public;
            builderFirstSetProperty.AccessLevel = DeclarationAccessLevel.Public;
            builderFollowSetProperty.AccessLevel = DeclarationAccessLevel.Public;
            builderFullSetProperty.AccessLevel = DeclarationAccessLevel.Public;

            var listOfRulePath = typeof(List<>).GetTypeReference(new TypeReferenceCollection(streamRulePath.GetTypeReference()));
            var builderHideWatches = sourcesBuilderCore.Fields.AddNew(new TypedName("hideWatches", typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(ruleIdentifierEnum.GetTypeReference(), listOfRulePath))));
            builderHideWatches.InitializationExpression = new CreateNewObjectExpression(builderHideWatches.FieldType);

            var addHideWatchMethod = sourcesBuilderCore.Methods.AddNew(new TypedName("AddHideWatch", typeof(void)));
            var hideWatchPath = addHideWatchMethod.Parameters.AddNew(new TypedName("path", streamRulePath));
            var targetID = addHideWatchMethod.Locals.AddNew(new TypedName("id", ruleIdentifierEnum));
            addHideWatchMethod.Assign(targetID.GetReference(), hideWatchPath.GetReference().GetProperty(originalPropertyName).GetProperty("RuleId"));

            var hideWatchLevelOneCheck = addHideWatchMethod.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, builderHideWatches.GetReference().GetMethod("ContainsKey").Invoke(targetID.GetReference())));
            hideWatchLevelOneCheck.CallMethod(builderHideWatches.GetReference().GetMethod("Add").Invoke(targetID.GetReference(), new CreateNewObjectExpression(listOfRulePath)));
            var hideWatchLevelTwoCheck = addHideWatchMethod.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, builderHideWatches.GetReference().GetIndex(targetID.GetReference()).GetMethod("Contains").Invoke(hideWatchPath.GetReference())));
            hideWatchLevelTwoCheck.CallMethod(builderHideWatches.GetReference().GetIndex(targetID.GetReference()).GetMethod("Add").Invoke(hideWatchPath.GetReference()));


            var builderConstructor = sourcesBuilderCore.Constructors.AddNew(new TypedName(initialSetFieldName, typeof(IEnumerable<>).GetTypeReference(new TypeReferenceCollection(streamStatePath.GetTypeReference()))));
            var builderConstructorParam = builderConstructor.Parameters[initialSetFieldName];
            builderConstructor.Statements.CallMethod(builderInitialSetField.GetReference().GetMethod("AddRange").Invoke(builderConstructorParam.GetReference()));
            builderConstructor.Statements.CallMethod(builderFullSetField.GetReference().GetMethod("AddRange").Invoke(builderConstructorParam.GetReference()));
            builderConstructor.AccessLevel = DeclarationAccessLevel.Internal;

            var builderAddFirstMethod = sourcesBuilderCore.Methods.AddNew(new TypedName(addFirstMethodName, typeof(bool)));
            var builderAddFirstParam = builderAddFirstMethod.Parameters.AddNew(new TypedName(firstParamName, streamStatePath));
            builderAddFirstMethod.AccessLevel = DeclarationAccessLevel.Public;

            var builderAddFirstCheck = builderAddFirstMethod.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, builderFullSetField.GetReference().GetMethod("Contains").Invoke(builderAddFirstParam.GetReference())));
            var addFirstRuleCheck = builderAddFirstCheck.IfThen(builderAddFirstParam.GetReference().GetProperty("IsRule"));
            addFirstRuleCheck.CallMethod(addHideWatchMethod.GetReference().Invoke(new CastExpression(builderAddFirstParam.GetReference(), streamRulePath.GetTypeReference())));

            builderAddFirstCheck.CallMethod(builderFullSetField.GetReference().GetMethod("Add").Invoke(builderAddFirstParam.GetReference()));
            builderAddFirstCheck.CallMethod(builderFirstSetField.GetReference().GetMethod("Add").Invoke(builderAddFirstParam.GetReference()));
            builderAddFirstCheck.Return(PrimitiveExpression.TrueValue);
            builderAddFirstMethod.Return(PrimitiveExpression.FalseValue);

            var builderAddFollowMethod = sourcesBuilderCore.Methods.AddNew(new TypedName(addFollowMethodName, typeof(bool)));
            var builderAddFollowParam = builderAddFollowMethod.Parameters.AddNew(new TypedName(followFieldName, streamStatePath));
            builderAddFollowMethod.AccessLevel = DeclarationAccessLevel.Public;

            var builderAddFollowCheck = builderAddFollowMethod.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, builderFullSetField.GetReference().GetMethod("Contains").Invoke(builderAddFollowParam.GetReference())));
            var builderAddFollowCheckRuleLocal = builderAddFollowCheck.Locals.AddNew(new TypedName("rule", streamRulePath), builderAddFollowParam.GetReference().GetProperty("Rule"));
            var builderAddFollowCheckRuleId = builderAddFollowCheck.Locals.AddNew(new TypedName("id", ruleIdentifierEnum), builderAddFollowCheckRuleLocal.GetReference().GetProperty(originalPropertyName).GetProperty("RuleId"));
            var builderAddFollowHiddenRecursionCheck = builderAddFollowCheck.IfThen(new BinaryOperationExpression(builderHideWatches.GetReference().GetMethod("ContainsKey").Invoke(builderAddFollowCheckRuleId.GetReference()), CodeBinaryOperatorType.BooleanAnd, new BinaryOperationExpression(builderHideWatches.GetReference().GetIndex(builderAddFollowCheckRuleId.GetReference()).GetMethod("Contains").Invoke(builderAddFollowCheckRuleLocal.GetReference()), CodeBinaryOperatorType.BooleanAnd, builderAddFollowParam.GetReference().GetProperty(originalPropertyName).GetMethod("ContainsKey").Invoke(builderAddFollowCheckRuleId.GetReference()))));
            builderAddFollowHiddenRecursionCheck.CallMethod(builderAddFollowCheckRuleLocal.GetReference().GetMethod("LineContains").Invoke(builderAddFollowCheckRuleId.GetReference(), builderAddFollowCheckRuleLocal.GetReference(), builderAddFollowParam.GetReference().GetProperty(originalPropertyName).GetIndex(builderAddFollowCheckRuleId.GetReference()).GetProperty("FollowState"), builderAddFollowParam.GetReference()));

            builderAddFollowCheck.CallMethod(builderFullSetField.GetReference().GetMethod("Add").Invoke(builderAddFollowParam.GetReference()));
            builderAddFollowCheck.CallMethod(builderFollowSetField.GetReference().GetMethod("Add").Invoke(builderAddFollowParam.GetReference()));
            builderAddFollowCheck.Return(PrimitiveExpression.TrueValue);
            builderAddFollowMethod.Return(PrimitiveExpression.FalseValue);

            var builderGetSourcesMethod = sourcesBuilderCore.Methods.AddNew(new TypedName(getSourcesMethodName, streamStateSources));
            builderGetSourcesMethod.Return(new CreateNewObjectExpression(builderGetSourcesMethod.ReturnType, builderInitialSetField.GetReference().GetMethod("ToArray").Invoke(), builderFirstSetField.GetReference().GetMethod("ToArray").Invoke(), builderFollowSetField.GetReference().GetMethod("ToArray").Invoke()));
            builderGetSourcesMethod.AccessLevel = DeclarationAccessLevel.Internal;

            BuildSourcesBuilderDisposeMethod(sourcesBuilderCore, builderFirstSetField, builderFollowSetField, builderInitialSetField, builderFullSetField);

            BuildSourcesBuilderIndexer(streamStatePath, sourcesBuilderCore, builderFirstSetField, builderFollowSetField, builderInitialSetField);

            BuildSourcesBuilderCountProperty(sourcesBuilderCore, builderFirstSetField, builderFollowSetField, builderInitialSetField);

            BuildSourcesInfoIndexer(streamStatePath, streamStateSources, streamStateSourcesInitialSetField, streamStateSourcesFirstSetField, streamStateSourcesFollowSetField);
            #region SourcesBuilder Count
            var streamSourcesInfoCount = streamStateSources.Properties.AddNew(new TypedName("Count", typeof(int)), true, false);
            streamSourcesInfoCount.AccessLevel = DeclarationAccessLevel.Public;
            streamSourcesInfoCount.GetPart.Return(new BinaryOperationExpression(streamStateSourcesInitialSetField.GetReference().GetProperty("Length"), CodeBinaryOperatorType.Add, new BinaryOperationExpression(streamStateSourcesFirstSetField.GetReference().GetProperty("Length"), CodeBinaryOperatorType.Add, streamStateSourcesFollowSetField.GetReference().GetProperty("Length"))));
            #endregion

            builderInitialSetProperty.GetPart.Return(builderInitialSetField.GetReference());
            builderFirstSetProperty.GetPart.Return(builderFirstSetField.GetReference());
            builderFollowSetProperty.GetPart.Return(builderFollowSetField.GetReference());
            builderFullSetProperty.GetPart.Return(builderFullSetField.GetReference());

            const string obtainRuleMethodName = "ObtainRule";
            const string obtainStateMethodName = "ObtainState";
            const string sourceIdParamName = "sourceId";
            const string sourcesParamName = "sources";
            const string transitionParamName = "transition";
            const string ruleCacheFieldName = "ruleCache";
            const string stateCacheFieldName = "stateCache";
            const string sourceLocalName = "source";
            const string sourcesLocalName = "sources";
            const string nodeName = "node";
            const string getSourceSetMethodName = "GetSourceSet";
            const string pathSetParam = "pathSet";
            const string tryTargetLocalName = "tryTarget";
            const string calculateFollowSetMethodName = "CalculateFollowSet";
            const string sourceBuilderLocalName = "sourceBuilder";
            const string currentPathParamName = "currentPath";

            var calculateFollowSetMethod = BuildCalculateFollowSetMethod(isTerminalEdgeProperty, streamState, streamStatePath, rulePropertyName, originalPropertyName, followPropertyName, followSetPropertyName, followInfoCore, addFollowMethodName, sourcesBuilderCore, listOfStatePath, sourcesParamName, calculateFollowSetMethodName);

            IExternTypeReference listOfStreamState;
            IFieldMember stateCacheField;
            BuildStreamStateDataMembers(sai, ruleIdentifierEnum, streamState, ruleCacheFieldName, stateCacheFieldName, out listOfStreamState, out stateCacheField);

            IMethodMember getSourceSetMethod;
            IMethodParameterMember getSourceSetSourcesParam;
            IMethodParameterMember getSourceSetCurrentPath;
            IMethodMember getSourceSetSimple;
            BuildMiscAreas(streamState, streamStatePath, fullSetPropertyName, sourcesBuilderCore, sourcesParamName, getSourceSetMethodName, currentPathParamName, out getSourceSetMethod, out getSourceSetSourcesParam, out getSourceSetCurrentPath, out getSourceSetSimple);

            BuildGetSourceSetMethodBody(ruleIdentifierEnum, stateCore, stateRulePushCore, streamRulePath, isRulePropertyName, lineContainsMethodName, listOfStatePath, calculateFollowSetMethod, getSourceSetMethod, getSourceSetSourcesParam, getSourceSetCurrentPath);

            var obtainRuleMethod = BuildObtainRuleMethod(ruleData, ruleIdentifierEnum, ruleIdentifierFieldLookup, declarationRuleCore, ruleDeclarationMethodMapping, streamState, streamStatePath, streamRulePath, getSourcesMethodName, sourcesBuilderCore, obtainRuleMethodName, sourceIdParamName, ruleCacheFieldName, sourceLocalName, sourcesLocalName, nodeName, getSourceSetSimple);

            var obtainStateMethod = BuildObtainStateMethod(sai, streamState, streamStatePath, followSetPropertyName, initialSetPropertyName, firstSetPropertyName, fullSetPropertyName, getSourcesMethodName, sourcesBuilderCore, obtainStateMethodName, sourcesParamName, transitionParamName, sourcesLocalName, pathSetParam, tryTargetLocalName, sourceBuilderLocalName, calculateFollowSetMethod, listOfStreamState, stateCacheField, getSourceSetSimple);

            IEnumeratorType sourcedFromEnum;
            IFieldMember sourcedFromInitial;
            IFieldMember sourcedFromFirst;
            IFieldMember sourcedFromFollow;
            BuildSourcedFromEnum(streamState, followPropertyName, out sourcedFromEnum, out sourcedFromInitial, out sourcedFromFirst, out sourcedFromFollow);


            var kvpOfSourcedFromStatePath = typeof(KeyValuePair<,>).GetTypeReference(new TypeReferenceCollection(sourcedFromEnum.GetTypeReference(), streamStatePath));
            var sourcesInfoGetEnum = BuildSourcesInfoGetEnumeratorMethod(streamStatePath, streamStateSources, streamStateSourcesInitialSetField, streamStateSourcesFirstSetField, streamStateSourcesFollowSetField, sourcedFromInitial, sourcedFromFirst, sourcedFromFollow, kvpOfSourcedFromStatePath);


            streamStateSources.ImplementsList.Add(typeof(IEnumerable<>).GetTypeReference(new TypeReferenceCollection(kvpOfSourcedFromStatePath)));

            BuildSourcesInfoGetEnumeratorMethod(streamStateSources, sourcesInfoGetEnum);


            var transitionOrderer = sai.data.EnumResults.ResultantType.Methods.AddNew(new TypedName("OrderingSelector", typeof(string)));
            var transitionOrdererParam = transitionOrderer.Parameters.AddNew(new TypedName("target", sai.data.EnumResults.ResultantType));
            transitionOrderer.AccessLevel = DeclarationAccessLevel.Private;
            transitionOrderer.IsStatic = true;
            transitionOrderer.Return(transitionOrdererParam.GetReference().GetMethod("ToString").Invoke());

            const string transitionOrdererPointerFieldName = "OrderingSelectorPointer";

            var transitionOrdererPointer = sai.data.EnumResults.ResultantType.Fields.AddNew(new TypedName(transitionOrdererPointerFieldName, typeof(Func<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), typeof(string).GetTypeReference()))));
            transitionOrdererPointer.InitializationExpression = transitionOrderer.GetReference();
            transitionOrdererPointer.IsStatic = true;
            transitionOrdererPointer.AccessLevel = DeclarationAccessLevel.Internal;

            #region StreamState GetLookAhead method
            var getLookAheadMethod = BuildGetLookAheadMethod(sai, stateCore, stateCoreTransition, streamState, streamStatePath, originalPropertyName, parentPropertyName, streamStateSources, streamStatePathArray, buildTransition, parentChildPair, listOfStatePath, sourcesParamName, kvpOfSourcedFromStatePath, transitionOrdererPointerFieldName);
            #endregion

            var transitionTable = BuildTransitionsTable(sai, collectionType, dictionaryType, streamState, streamStatePathArray, sourcesLocalName, obtainStateMethod, getLookAheadMethod);

            var streamTransitionsField = streamState.Fields.AddNew(new TypedName("transitions", transitionTable));
            var transitions = streamState.Properties.AddNew(new TypedName("Transitions", transitionTable), true, false);
            transitions.AccessLevel = DeclarationAccessLevel.Public;
            var transitionsCheck = transitions.GetPart.IfThen(new BinaryOperationExpression(streamTransitionsField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            transitionsCheck.Assign(streamTransitionsField.GetReference(), new CreateNewObjectExpression(transitionTable.GetTypeReference(), new ThisReferenceExpression()));

            transitions.GetPart.Return(streamTransitionsField.GetReference());
            #endregion
        }

        private static IClassType BuildTransitionsTable(SyntaxActivationInfo sai, IInterfaceType collectionType, IInterfaceType dictionaryType, IClassType streamState, ITypeReference streamStatePathArray, string sourcesLocalName, IMethodMember obtainStateMethod, IMethodMember getLookAheadMethod)
        {
            #region Stream State TransitionTable Construction
            var transitionTable = streamState.Partials.AddNew().Classes.AddNew("TransitionTable");
            transitionTable.ImplementsList.Add(dictionaryType.GetTypeReference(sai.data.EnumResults.ResultantType, streamState));
            transitionTable.AccessLevel = DeclarationAccessLevel.Public;
            var fullSetObtained = transitionTable.Fields.AddNew(new TypedName("fullSetObtained", typeof(bool)));
            var fullSet = transitionTable.Fields.AddNew(new TypedName("fullSet", sai.data.EnumResults.ResultantType));
            var partialKeys = transitionTable.Fields.AddNew(new TypedName("partialKeys", typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), sai.data.EnumResults.ResultantType.GetTypeReference()))));
            partialKeys.InitializationExpression = new CreateNewObjectExpression(partialKeys.FieldType);
            var dataSource = transitionTable.Fields.AddNew(new TypedName("dataSource", typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), streamStatePathArray))));
            var dataCopy = transitionTable.Fields.AddNew(new TypedName("dataCopy", typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), streamState.GetTypeReference()))));
            dataCopy.InitializationExpression = new CreateNewObjectExpression(dataCopy.FieldType);
            var owner = transitionTable.Fields.AddNew(new TypedName("owner", streamState));
            var kvpOfTransitionToStreamState = typeof(KeyValuePair<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), streamState.GetTypeReference()));
            var indexCache = transitionTable.Fields.AddNew(new TypedName("indexCache", typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(typeof(int).GetTypeReference(), kvpOfTransitionToStreamState))));
            indexCache.InitializationExpression = new CreateNewObjectExpression(indexCache.FieldType);

            var transitionTableCtor = transitionTable.Constructors.AddNew();
            var transitionTableOwnerParam = transitionTableCtor.Parameters.AddNew(new TypedName("owner", streamState));
            transitionTableCtor.Statements.Assign(owner.GetReference(), transitionTableOwnerParam.GetReference());
            transitionTableCtor.Statements.Assign(dataSource.GetReference(), getLookAheadMethod.GetReference().Invoke(transitionTableOwnerParam.GetReference().GetField(sourcesLocalName)));
            transitionTableCtor.AccessLevel = DeclarationAccessLevel.Internal;

            var keysCollection = BuildTransitionTableKeysCollection(sai, collectionType, transitionTable, dataSource);

            #region Transition Table Keys Property
            var keysField = transitionTable.Fields.AddNew(new TypedName("keys", keysCollection));

            var keys = transitionTable.Properties.AddNew(new TypedName("Keys", collectionType.GetTypeReference(sai.data.EnumResults.ResultantType)), true, false);
            keys.AccessLevel = DeclarationAccessLevel.Public;

            var keysCheck = keys.GetPart.IfThen(new BinaryOperationExpression(keysField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            keysCheck.Assign(keysField.GetReference(), new CreateNewObjectExpression(keysCollection.GetTypeReference(), new ThisReferenceExpression()));
            keys.GetPart.Return(keysField.GetReference());
            #endregion

            var valuesCollection = BuildTransitionTableValuesCollection(sai, collectionType, streamState, transitionTable);

            #region Transition Table Values Property
            var valuesField = transitionTable.Fields.AddNew(new TypedName("values", valuesCollection));

            var values = transitionTable.Properties.AddNew(new TypedName("Values", collectionType.GetTypeReference(streamState)), true, false);
            values.AccessLevel = DeclarationAccessLevel.Public;

            var valuesCheck = values.GetPart.IfThen(new BinaryOperationExpression(valuesField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            valuesCheck.Assign(valuesField.GetReference(), new CreateNewObjectExpression(valuesCollection.GetTypeReference(), new ThisReferenceExpression()));
            values.GetPart.Return(valuesField.GetReference());
            #endregion

            #region TransitionTable ContainsKey Method
            var containsKey = BuildTransitionTableContainsKeyMethod(sai, transitionTable, partialKeys, dataSource);
            #endregion

            #region Transition Table CheckItemAt(TokenTransition) Method
            var checkItemAtKey = BuildTransitionTableCheckItemAtKey(sai, streamState, obtainStateMethod, transitionTable, partialKeys, dataSource, dataCopy, containsKey);
            #endregion

            #region TransitionTable CheckItemAt(int) Method
            var checkItemAtIndex = BuildTransitionTableCheckItemAtIndex(sai, transitionTable, dataSource, dataCopy, kvpOfTransitionToStreamState, indexCache, keys, checkItemAtKey);

            #endregion

            #region Transition Table TryGetValue Method
            var tryGetValueMethod = BuildTransitionTableTryGetValueMethod(sai, streamState, transitionTable, partialKeys, dataCopy, checkItemAtKey);
            #endregion

            var count = BuildTransitionTableCountProperty(transitionTable, dataSource);

            var keyedIndex = BuildTransitionTableKeyedIndex(sai, streamState, transitionTable, tryGetValueMethod);

            var indexedIndexer = BuildTransitionTableIndexedIndexer(transitionTable, kvpOfTransitionToStreamState, indexCache, checkItemAtIndex);

            var contains = BuildTransitionTableContainsMethod(transitionTable, kvpOfTransitionToStreamState, containsKey);

            var copyTo = BuildTransitionTableCopyToMethod(transitionTable, kvpOfTransitionToStreamState, count);

            var toArray = BuildTransitionTableToArrayMethod(transitionTable, kvpOfTransitionToStreamState, count, copyTo);

            var getEnumerator = BuildTransitionTableGetEnumerator(sai, transitionTable, dataSource, kvpOfTransitionToStreamState);

            var getEnumerator2 = transitionTable.Methods.AddNew(new TypedName("_GetEnumerator", typeof(IEnumerator)));
            getEnumerator2.PrivateImplementationTarget = typeof(IEnumerable).GetTypeReference();
            getEnumerator2.Return(getEnumerator.GetReference().Invoke());
            getEnumerator2.Name = getEnumerator.Name;


            #endregion
            return transitionTable;
        }

        private static IPropertyMember BuildTransitionTableCountProperty(IClassType transitionTable, IFieldMember dataSource)
        {
            var count = transitionTable.Properties.AddNew(new TypedName("Count", typeof(int)), true, false);
            count.AccessLevel = DeclarationAccessLevel.Public;
            count.GetPart.Return(dataSource.GetReference().GetProperty("Count"));
            return count;
        }

        private static IMethodMember BuildTransitionTableContainsKeyMethod(SyntaxActivationInfo sai, IClassType transitionTable, IFieldMember partialKeys, IFieldMember dataSource)
        {
            var containsKey = transitionTable.Methods.AddNew(new TypedName("ContainsKey", typeof(bool)));
            containsKey.AccessLevel = DeclarationAccessLevel.Public;
            var containsKeyParam = containsKey.Parameters.AddNew(new TypedName("key", sai.data.EnumResults.ResultantType));

            var containsKeyCheck = containsKey.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, dataSource.GetReference().GetMethod("ContainsKey").Invoke(containsKeyParam.GetReference())));
            var containsKeyPartialCheck = containsKeyCheck.IfThen(partialKeys.GetReference().GetMethod("ContainsKey").Invoke(containsKeyParam.GetReference()));
            containsKeyPartialCheck.Return(PrimitiveExpression.TrueValue);

            var containsKeyEnum = containsKeyCheck.Enumerate(dataSource.GetReference().GetProperty("Keys"), sai.data.EnumResults.ResultantType.GetTypeReference());
            containsKeyEnum.CurrentMember.Name = "fullTransition";
            var containsKeyIntersection = containsKeyEnum.Locals.AddNew(new TypedName("intersection", sai.data.EnumResults.ResultantType));
            containsKeyIntersection.InitializationExpression = new BinaryOperationExpression(containsKeyEnum.CurrentMember.GetReference(), CodeBinaryOperatorType.BitwiseAnd, containsKeyParam.GetReference());
            var containsKeyIntersectCheck = containsKeyEnum.IfThen(new BinaryOperationExpression(containsKeyIntersection.GetReference(), CodeBinaryOperatorType.IdentityEquality, containsKeyParam.GetReference()));
            containsKeyIntersectCheck.CallMethod(partialKeys.GetReference().GetMethod("Add").Invoke(containsKeyParam.GetReference(), containsKeyEnum.CurrentMember.GetReference()));
            containsKeyIntersectCheck.Return(PrimitiveExpression.TrueValue);
            containsKeyCheck.Return(PrimitiveExpression.FalseValue);
            containsKeyCheck.FalseBlock.Return(PrimitiveExpression.TrueValue);
            return containsKey;
        }

        private static IMethodMember BuildTransitionTableGetEnumerator(SyntaxActivationInfo sai, IClassType transitionTable, IFieldMember dataSource, IExternTypeReference kvpOfTransitionToStreamState)
        {
            var getEnumerator = transitionTable.Methods.AddNew(new TypedName("GetEnumerator", typeof(IEnumerator<>).GetTypeReference(new TypeReferenceCollection(kvpOfTransitionToStreamState))));
            getEnumerator.AccessLevel = DeclarationAccessLevel.Public;
            var enumeration = getEnumerator.Enumerate(dataSource.GetReference().GetProperty("Keys"), sai.data.EnumResults.ResultantType.GetTypeReference());
            enumeration.CurrentMember.Name = "key";
            enumeration.Yield(new CreateNewObjectExpression(kvpOfTransitionToStreamState, enumeration.CurrentMember.GetReference(), new ThisReferenceExpression().GetIndex(enumeration.CurrentMember.GetReference())));
            return getEnumerator;
        }

        private static IMethodMember BuildTransitionTableToArrayMethod(IClassType transitionTable, IExternTypeReference kvpOfTransitionToStreamState, IPropertyMember count, IMethodMember copyTo)
        {
            var toArray = transitionTable.Methods.AddNew(new TypedName("ToArray", kvpOfTransitionToStreamState.MakeArray(1)));
            toArray.AccessLevel = DeclarationAccessLevel.Public;
            var resultLocal = toArray.Locals.AddNew(new TypedName("result", kvpOfTransitionToStreamState.MakeArray(1)));
            resultLocal.InitializationExpression = new CreateArrayExpression(kvpOfTransitionToStreamState, count.GetReference());

            toArray.CallMethod(copyTo.GetReference().Invoke(resultLocal.GetReference(), PrimitiveExpression.NumberZero));
            toArray.Return(resultLocal.GetReference());

            return toArray;
        }

        private static IMethodMember BuildTransitionTableCopyToMethod(IClassType transitionTable, IExternTypeReference kvpOfTransitionToStreamState, IPropertyMember count)
        {
            var copyTo = transitionTable.Methods.AddNew(new TypedName("CopyTo", typeof(void)));
            copyTo.AccessLevel = DeclarationAccessLevel.Public;
            var array = copyTo.Parameters.AddNew(new TypedName("array", kvpOfTransitionToStreamState.MakeArray(1)));
            var arrayIndex = copyTo.Parameters.AddNew(new TypedName("arrayIndex", typeof(int)));
            var iLocal = copyTo.Locals.AddNew(new TypedName("i", typeof(int)), PrimitiveExpression.NumberZero);
            iLocal.AutoDeclare = false;
            var copyToIteration = copyTo.Iterate(iLocal.GetDeclarationStatement(), new CrementStatement(CrementType.Postfix, CrementOperation.Increment, iLocal.GetReference()), new BinaryOperationExpression(iLocal.GetReference(), CodeBinaryOperatorType.LessThan, count.GetReference()));
            copyToIteration.Assign(array.GetReference().GetIndex(new BinaryOperationExpression(arrayIndex.GetReference(), CodeBinaryOperatorType.Add, iLocal.GetReference())), new ThisReferenceExpression().GetIndex(iLocal.GetReference()));

            return copyTo;
        }

        private static IMethodMember BuildTransitionTableContainsMethod(IClassType transitionTable, IExternTypeReference kvpOfTransitionToStreamState, IMethodMember containsKey)
        {
            var contains = transitionTable.Methods.AddNew(new TypedName("Contains", typeof(bool)));
            contains.AccessLevel = DeclarationAccessLevel.Public;
            var containsParam = contains.Parameters.AddNew(new TypedName("item", kvpOfTransitionToStreamState));
            var containsNegativeAssertion = contains.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, containsKey.GetReference().Invoke(containsParam.GetReference().GetProperty("Key"))));
            containsNegativeAssertion.Return(PrimitiveExpression.FalseValue);

            contains.Return(new BinaryOperationExpression(new ThisReferenceExpression().GetIndex(containsParam.GetReference().GetProperty("Key")), CodeBinaryOperatorType.IdentityEquality, containsParam.GetReference().GetProperty("Value")));

            return contains;
        }

        private static IIndexerMember BuildTransitionTableIndexedIndexer(IClassType transitionTable, IExternTypeReference kvpOfTransitionToStreamState, IFieldMember indexCache, IMethodMember checkItemAtIndex)
        {

            var indexedIndexer = transitionTable.Properties.AddNew(kvpOfTransitionToStreamState, true, false, new TypedName("index", typeof(int)));
            indexedIndexer.AccessLevel = DeclarationAccessLevel.Public;
            var index = indexedIndexer.Parameters["index"];

            indexedIndexer.GetPart.CallMethod(checkItemAtIndex.GetReference().Invoke(index.GetReference()));
            var indexerCheck = indexedIndexer.GetPart.IfThen(indexCache.GetReference().GetMethod("ContainsKey").Invoke(index.GetReference()));
            indexerCheck.Return(indexCache.GetReference().GetIndex(index.GetReference()));
            indexedIndexer.GetPart.Return(new CreateNewObjectExpression(indexedIndexer.PropertyType));
            return indexedIndexer;
        }

        private static IMethodMember BuildTransitionTableCheckItemAtIndex(SyntaxActivationInfo sai, IClassType transitionTable, IFieldMember dataSource, IFieldMember dataCopy, IExternTypeReference kvpOfTransitionToStreamState, IFieldMember indexCache, IPropertyMember keys, IMethodMember checkItemAtKey)
        {
            var checkItemAtIndex = transitionTable.Methods.AddNew(new TypedName("CheckItemAt", typeof(void)));
            var index = checkItemAtIndex.Parameters.AddNew(new TypedName("index", typeof(int)));
            var rangeCheckExp1 = new BinaryOperationExpression(index.GetReference(), CodeBinaryOperatorType.GreaterThanOrEqual, dataSource.GetReference().GetProperty("Count"));
            var rangeCheckExp2 = new BinaryOperationExpression(index.GetReference(), CodeBinaryOperatorType.LessThan, PrimitiveExpression.NumberZero);
            var rangeCheckExp = new BinaryOperationExpression(rangeCheckExp1, CodeBinaryOperatorType.BooleanOr, rangeCheckExp2);

            var rangeCheck = checkItemAtIndex.IfThen(rangeCheckExp);
            rangeCheck.Return();

            var cacheCheck = checkItemAtIndex.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, indexCache.GetReference().GetMethod("ContainsKey").Invoke(index.GetReference())));
            var currentKey = cacheCheck.Locals.AddNew(new TypedName("indexedTransition", sai.data.EnumResults.ResultantType), keys.GetReference().GetIndex(index.GetReference()));
            cacheCheck.CallMethod(checkItemAtKey.GetReference().Invoke(currentKey.GetReference()));
            cacheCheck.CallMethod(indexCache.GetReference().GetMethod("Add").Invoke(index.GetReference(), new CreateNewObjectExpression(kvpOfTransitionToStreamState, currentKey.GetReference(), dataCopy.GetReference().GetIndex(currentKey.GetReference()))));
            return checkItemAtIndex;
        }

        private static IIndexerMember BuildTransitionTableKeyedIndex(SyntaxActivationInfo sai, IClassType streamState, IClassType transitionTable, IMethodMember tryGetValueMethod)
        {
            var keyedIndex = transitionTable.Properties.AddNew(streamState.GetTypeReference(), true, false, new TypedName("key", sai.data.EnumResults.ResultantType));
            keyedIndex.AccessLevel = DeclarationAccessLevel.Public;
            var keyedIndexParam = keyedIndex.Parameters["key"];
            var result = keyedIndex.GetPart.Locals.AddNew(new TypedName("result", streamState));
            keyedIndex.GetPart.CallMethod(tryGetValueMethod.GetReference().Invoke(keyedIndexParam.GetReference(), new DirectionExpression(FieldDirection.Out, result.GetReference())));
            keyedIndex.GetPart.Return(result.GetReference());
            return keyedIndex;
        }

        private static IMethodMember BuildTransitionTableTryGetValueMethod(SyntaxActivationInfo sai, IClassType streamState, IClassType transitionTable, IFieldMember partialKeys, IFieldMember dataCopy, IMethodMember checkItemAtKey)
        {
            var tryGetValueMethod = transitionTable.Methods.AddNew(new TypedName("TryGetValue", typeof(bool)));
            var key = tryGetValueMethod.Parameters.AddNew(new TypedName("key", sai.data.EnumResults.ResultantType));
            var value = tryGetValueMethod.Parameters.AddNew(new TypedName("value", streamState));
            value.Direction = FieldDirection.Out;
            tryGetValueMethod.AccessLevel = DeclarationAccessLevel.Public;
            var preCheck = tryGetValueMethod.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, new BinaryOperationExpression(partialKeys.GetReference().GetMethod("ContainsKey").Invoke(key.GetReference()), CodeBinaryOperatorType.BooleanOr, dataCopy.GetReference().GetMethod("ContainsKey").Invoke(key.GetReference()))));
            preCheck.CallMethod(checkItemAtKey.GetReference().Invoke(key.GetReference()));
            var fullCheck = tryGetValueMethod.IfThen(dataCopy.GetReference().GetMethod("ContainsKey").Invoke(key.GetReference()));
            fullCheck.Assign(value.GetReference(), dataCopy.GetReference().GetIndex(key.GetReference()));
            fullCheck.Return(PrimitiveExpression.TrueValue);
            var partialCheck = fullCheck.FalseBlock.IfThen(partialKeys.GetReference().GetMethod("ContainsKey").Invoke(key.GetReference()));
            partialCheck.Assign(value.GetReference(), dataCopy.GetReference().GetIndex(partialKeys.GetReference().GetIndex(key.GetReference())));
            partialCheck.Return(PrimitiveExpression.TrueValue);
            tryGetValueMethod.Assign(value.GetReference(), PrimitiveExpression.NullValue);
            tryGetValueMethod.Return(PrimitiveExpression.FalseValue);
            return tryGetValueMethod;
        }

        private static IMethodMember BuildTransitionTableCheckItemAtKey(SyntaxActivationInfo sai, IClassType streamState, IMethodMember obtainStateMethod, IClassType transitionTable, IFieldMember partialKeys, IFieldMember dataSource, IFieldMember dataCopy, IMethodMember containsKey)
        {
            var checkItemAtKey = transitionTable.Methods.AddNew(new TypedName("CheckItemAt", typeof(void)));
            var checkitemAtKeyParam = checkItemAtKey.Parameters.AddNew(new TypedName("key", sai.data.EnumResults.ResultantType));
            var checkItemAtKeyCondition1 = new UnaryOperationExpression(UnaryOperations.LogicalNot, containsKey.GetReference().Invoke(checkitemAtKeyParam.GetReference()));
            var checkItemAtKeyCondition2A = partialKeys.GetReference().GetMethod("ContainsKey").Invoke(checkitemAtKeyParam.GetReference());
            var checkItemAtKeyCondition2B = dataCopy.GetReference().GetMethod("ContainsKey").Invoke(partialKeys.GetReference().GetIndex(checkitemAtKeyParam.GetReference()));
            var checkItemAtKeyCondition2 = new BinaryOperationExpression(checkItemAtKeyCondition2A, CodeBinaryOperatorType.BooleanAnd, checkItemAtKeyCondition2B);
            var checkItemAtKeyCondition3 = dataCopy.GetReference().GetMethod("ContainsKey").Invoke(checkitemAtKeyParam.GetReference());
            var checkItemAtKeyConditionA = new BinaryOperationExpression(checkItemAtKeyCondition1, CodeBinaryOperatorType.BooleanOr, checkItemAtKeyCondition2);
            var checkItemAtKeyCondition = new BinaryOperationExpression(checkItemAtKeyConditionA, CodeBinaryOperatorType.BooleanOr, checkItemAtKeyCondition3);
            var checkItemAtKeyCheck = checkItemAtKey.IfThen(checkItemAtKeyCondition);
            checkItemAtKeyCheck.Return();

            var checkItemAtResult = checkItemAtKey.Locals.AddNew(new TypedName("result", streamState), PrimitiveExpression.NullValue);
            checkItemAtResult.AutoDeclare = false;
            checkItemAtKey.DefineLocal(checkItemAtResult);

            var checkItemAtFullCheck = checkItemAtKey.IfThen(dataSource.GetReference().GetMethod("ContainsKey").Invoke(checkitemAtKeyParam.GetReference()));
            checkItemAtFullCheck.Assign(checkItemAtResult.GetReference(), obtainStateMethod.GetReference().Invoke(dataSource.GetReference().GetIndex(checkitemAtKeyParam.GetReference()), checkitemAtKeyParam.GetReference()));
            checkItemAtFullCheck.CallMethod(dataCopy.GetReference().GetMethod("Add").Invoke(checkitemAtKeyParam.GetReference(), checkItemAtResult.GetReference()));
            var checkItemAtPartialCheck = checkItemAtFullCheck.FalseBlock;
            var fullKey = checkItemAtPartialCheck.Locals.AddNew(new TypedName("fullKey", sai.data.EnumResults.ResultantType), partialKeys.GetReference().GetIndex(checkitemAtKeyParam.GetReference()));
            checkItemAtPartialCheck.Assign(checkItemAtResult.GetReference(), obtainStateMethod.GetReference().Invoke(dataSource.GetReference().GetIndex(fullKey.GetReference()), fullKey.GetReference()));
            checkItemAtPartialCheck.CallMethod(dataCopy.GetReference().GetMethod("Add").Invoke(fullKey.GetReference(), checkItemAtResult.GetReference()));
            return checkItemAtKey;
        }

        private static IClassType BuildTransitionTableValuesCollection(SyntaxActivationInfo sai, IInterfaceType collectionType, IClassType streamState, IClassType transitionTable)
        {
            #region Values Collection
            var valuesCollection = transitionTable.Partials.AddNew().Classes.AddNew("ValuesCollection");
            var valuesCollectionOwner = valuesCollection.Fields.AddNew(new TypedName("owner", transitionTable));
            var valuesOwnerDataSource = valuesCollectionOwner.GetReference().GetField("dataSource");
            var valuesOwnerDataCopy = valuesCollectionOwner.GetReference().GetField("dataCopy");

            valuesCollection.ImplementsList.Add(collectionType.GetTypeReference(streamState));
            var valuesCollectionCtor = valuesCollection.Constructors.AddNew();
            valuesCollectionCtor.AccessLevel = DeclarationAccessLevel.Public;
            var valuesCollectionCtorParam = valuesCollectionCtor.Parameters.AddNew(new TypedName("owner", transitionTable));
            valuesCollectionCtor.Statements.Assign(valuesCollectionOwner.GetReference(), valuesCollectionCtorParam.GetReference());

            var valuesCollectionCount = valuesCollection.Properties.AddNew(new TypedName("Count", typeof(int)), true, false);
            valuesCollectionCount.AccessLevel = DeclarationAccessLevel.Public;
            valuesCollectionCount.GetPart.Return(valuesOwnerDataSource.GetProperty("Count"));

            var valuesIndexer = valuesCollection.Properties.AddNew(streamState.GetTypeReference(), true, false, new TypedName("index", typeof(int)));
            valuesIndexer.AccessLevel = DeclarationAccessLevel.Public;
            valuesIndexer.GetPart.Return(valuesCollectionOwner.GetReference().GetIndex(valuesIndexer.Parameters["index"].GetReference()).GetProperty("Value"));

            var valuesContains = valuesCollection.Methods.AddNew(new TypedName("Contains", typeof(bool)));
            valuesContains.AccessLevel = DeclarationAccessLevel.Public;
            var valuesContainsParam = valuesContains.Parameters.AddNew(new TypedName("value", streamState));
            var valuesContainsCheck = valuesContains.IfThen(typeof(Enumerable).GetTypeReferenceExpression().GetMethod("Contains", streamState.GetTypeReference()).Invoke(valuesOwnerDataCopy.GetProperty("Values"), valuesContainsParam.GetReference()));
            valuesContainsCheck.Statements.Add(new CommentStatement(
                "A quick check to the cache."));
            valuesContainsCheck.Return(PrimitiveExpression.TrueValue);

            var fullSizeCheck = valuesContainsCheck.FalseBlock.IfThen(new BinaryOperationExpression(valuesOwnerDataSource.GetProperty("Count"), CodeBinaryOperatorType.IdentityEquality, valuesOwnerDataCopy.GetProperty("Count")));
            fullSizeCheck.Statements.Add(new CommentStatement(
                "If the cache for the current table is to size,\r\n" +
                "then it doesn't exist in the current table."));
            fullSizeCheck.Return(PrimitiveExpression.FalseValue);

            var valueContainsElse = fullSizeCheck.FalseBlock;

            var valueContainsEnumeration = valueContainsElse.Enumerate(valuesCollectionOwner.GetReference(), typeof(KeyValuePair<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), streamState.GetTypeReference())));
            valueContainsEnumeration.CurrentMember.Name = "currentPair";
            var valueContainsEnumCheck = valueContainsEnumeration.IfThen(new BinaryOperationExpression(valuesContainsParam.GetReference(), CodeBinaryOperatorType.IdentityEquality, valueContainsEnumeration.CurrentMember.GetReference().GetProperty("Value")));
            valueContainsEnumCheck.Return(PrimitiveExpression.TrueValue);

            valueContainsElse.Return(PrimitiveExpression.FalseValue);

            var valueCopyTo = valuesCollection.Methods.AddNew(new TypedName("CopyTo", typeof(void)));
            valueCopyTo.AccessLevel = DeclarationAccessLevel.Public;
            var valueCopyToArrayParam = valueCopyTo.Parameters.AddNew(new TypedName("array", streamState.GetTypeReference().MakeArray(1)));
            var valueCopyToArrayIndexParam = valueCopyTo.Parameters.AddNew(new TypedName("arrayIndex", typeof(int)));
            var valueCopyToILocal = valueCopyTo.Locals.AddNew(new TypedName("i", typeof(int)), PrimitiveExpression.NumberZero);
            valueCopyToILocal.AutoDeclare = false;

            var copyToIteration = valueCopyTo.Iterate(valueCopyToILocal.GetDeclarationStatement(), new CrementStatement(CrementType.Postfix, CrementOperation.Increment, valueCopyToILocal.GetReference()), new BinaryOperationExpression(valueCopyToILocal.GetReference(), CodeBinaryOperatorType.LessThan, valuesOwnerDataSource.GetProperty("Count")));
            copyToIteration.Assign(valueCopyToArrayParam.GetReference().GetIndex(new BinaryOperationExpression(valueCopyToArrayIndexParam.GetReference(), CodeBinaryOperatorType.Add, valueCopyToILocal.GetReference())), new ThisReferenceExpression().GetIndex(valueCopyToILocal.GetReference()));

            var valueToArray = valuesCollection.Methods.AddNew(new TypedName("ToArray", streamState.GetTypeReference().MakeArray(1)));
            valueToArray.AccessLevel = DeclarationAccessLevel.Public;
            var valueToArrayResultLocal = valueToArray.Locals.AddNew(new TypedName("result", streamState.GetTypeReference().MakeArray(1)));
            valueToArrayResultLocal.InitializationExpression = new CreateArrayExpression(streamState.GetTypeReference(), valuesCollectionCount.GetReference());

            valueToArray.CallMethod(valueCopyTo.GetReference().Invoke(valueToArrayResultLocal.GetReference(), PrimitiveExpression.NumberZero));

            valueToArray.Return(valueToArrayResultLocal.GetReference());

            var valueGetEnumerator = valuesCollection.Methods.AddNew(new TypedName("GetEnumerator", typeof(IEnumerator<>).GetTypeReference(new TypeReferenceCollection(streamState.GetTypeReference()))));
            var valueGetEnum = valueGetEnumerator.Enumerate(valuesOwnerDataSource.GetProperty("Keys"), sai.data.EnumResults.ResultantType.GetTypeReference());
            valueGetEnum.CurrentMember.Name = "transition";
            valueGetEnum.Yield(valuesCollectionOwner.GetReference().GetIndex(valueGetEnum.CurrentMember.GetReference()));
            valueGetEnumerator.AccessLevel = DeclarationAccessLevel.Public;

            var valueGetEnumerator2 = valuesCollection.Methods.AddNew(new TypedName("_GetEnumerator", typeof(IEnumerator)));
            valueGetEnumerator2.PrivateImplementationTarget = typeof(IEnumerable).GetTypeReference();
            valueGetEnumerator2.Name = valueGetEnumerator.Name;
            valueGetEnumerator2.Return(valueGetEnumerator.GetReference().Invoke());
            #endregion
            return valuesCollection;
        }

        private static IClassType BuildTransitionTableKeysCollection(SyntaxActivationInfo sai, IInterfaceType collectionType, IClassType transitionTable, IFieldMember dataSource)
        {

            #region KeysCollection
            var keysCollection = transitionTable.Partials.AddNew().Classes.AddNew("KeysCollection");
            keysCollection.AccessLevel = DeclarationAccessLevel.Private;
            keysCollection.ImplementsList.Add(collectionType.GetTypeReference(sai.data.EnumResults.ResultantType));
            var keysOwner = keysCollection.Fields.AddNew(new TypedName("owner", transitionTable));
            var keysCollectionCtor = keysCollection.Constructors.AddNew();
            var keysCollectionOwnerParam = keysCollectionCtor.Parameters.AddNew(new TypedName("owner", transitionTable));
            keysCollectionCtor.Statements.Assign(keysOwner.GetReference(), keysCollectionOwnerParam.GetReference());
            keysCollectionCtor.AccessLevel = DeclarationAccessLevel.Public;

            var keysCount = keysCollection.Properties.AddNew(new TypedName("Count", typeof(int)), true, false);
            keysCount.AccessLevel = DeclarationAccessLevel.Public;
            var keysOwnerDataSource = keysOwner.GetReference().GetField(dataSource.Name);
            keysCount.GetPart.Return(keysOwnerDataSource.GetProperty("Count"));

            var keysIndexer = keysCollection.Properties.AddNew(sai.data.EnumResults.ResultantType.GetTypeReference(), true, false, new TypedName("index", typeof(int)));
            keysIndexer.AccessLevel = DeclarationAccessLevel.Public;
            var keysIndexerParam = keysIndexer.Parameters["index"];
            var keysIndexerILocal = keysIndexer.GetPart.Locals.AddNew(new TypedName("i", typeof(int)), PrimitiveExpression.NumberZero);
            var transitionEnum = keysIndexer.GetPart.Enumerate(keysOwnerDataSource.GetProperty("Keys"), sai.data.EnumResults.ResultantType.GetTypeReference());
            transitionEnum.CurrentMember.Name = "currentTransition";
            var transitionEnumCheck = transitionEnum.IfThen(new BinaryOperationExpression(keysIndexerILocal.GetReference(), CodeBinaryOperatorType.IdentityEquality, keysIndexerParam.GetReference()));
            transitionEnumCheck.Return(transitionEnum.CurrentMember.GetReference());
            transitionEnumCheck.FalseBlock.Crement(keysIndexerILocal.GetReference(), CrementType.Postfix, CrementOperation.Increment);
            keysIndexer.GetPart.Return(new CreateNewObjectExpression(sai.data.EnumResults.ResultantType.GetTypeReference()));

            var keysContains = keysCollection.Methods.AddNew(new TypedName("Contains", typeof(bool)));
            var keysContainsParam = keysContains.Parameters.AddNew(new TypedName("key", sai.data.EnumResults.ResultantType));
            keysContains.AccessLevel = DeclarationAccessLevel.Public;
            keysContains.Return(keysOwner.GetReference().GetMethod("ContainsKey").Invoke(keysContainsParam.GetReference()));


            var keysCopyToMethod = keysCollection.Methods.AddNew(new TypedName("CopyTo", typeof(void)));
            keysCopyToMethod.AccessLevel = DeclarationAccessLevel.Public;
            var keysCopyToTarget = keysCopyToMethod.Parameters.AddNew(new TypedName("array", sai.data.EnumResults.ResultantType.GetTypeReference().MakeArray(1)));
            var keysCopyToArrayIndex = keysCopyToMethod.Parameters.AddNew(new TypedName("arrayIndex", typeof(int)));
            var keysCopyToILocal = keysCopyToMethod.Locals.AddNew(new TypedName("i", typeof(int)));
            keysCopyToILocal.InitializationExpression = PrimitiveExpression.NumberZero;
            var keysCopyToEnum = keysCopyToMethod.Enumerate(keysOwnerDataSource.GetProperty("Keys"), sai.data.EnumResults.ResultantType.GetTypeReference());
            keysCopyToEnum.CurrentMember.Name = "currentTransition";
            keysCopyToEnum.Assign(keysCopyToTarget.GetReference().GetIndex(new BinaryOperationExpression(keysCopyToILocal.GetReference(), CodeBinaryOperatorType.Add, keysCopyToArrayIndex.GetReference())), keysCopyToEnum.CurrentMember.GetReference());
            keysCopyToEnum.Crement(keysCopyToILocal.GetReference(), CrementType.Postfix, CrementOperation.Increment);

            var keysToArrayMethod = keysCollection.Methods.AddNew(new TypedName("ToArray", sai.data.EnumResults.ResultantType.GetTypeReference().MakeArray(1)));
            var keysToArrayResult = keysToArrayMethod.Locals.AddNew(new TypedName("result", sai.data.EnumResults.ResultantType.GetTypeReference().MakeArray(1)), new CreateArrayExpression(sai.data.EnumResults.ResultantType.GetTypeReference(), keysOwnerDataSource.GetProperty("Count")));
            keysToArrayMethod.AccessLevel = DeclarationAccessLevel.Public;
            keysToArrayMethod.CallMethod(keysCopyToMethod.GetReference().Invoke(keysToArrayResult.GetReference(), PrimitiveExpression.NumberZero));
            keysToArrayMethod.Return(keysToArrayResult.GetReference());

            var keysGetEnumeratorA = keysCollection.Methods.AddNew(new TypedName("GetEnumerator", typeof(IEnumerator<>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference()))));
            keysGetEnumeratorA.Return(keysOwnerDataSource.GetProperty("Keys").GetMethod("GetEnumerator").Invoke());
            keysGetEnumeratorA.AccessLevel = DeclarationAccessLevel.Public;

            var keysGetEnumeratorB = keysCollection.Methods.AddNew(new TypedName("_GetEnumerator", typeof(IEnumerator)));
            keysGetEnumeratorB.PrivateImplementationTarget = typeof(IEnumerable).GetTypeReference();
            keysGetEnumeratorB.Name = keysGetEnumeratorA.Name;
            keysGetEnumeratorB.Return(keysGetEnumeratorA.GetReference().Invoke());

            #endregion
            return keysCollection;
        }

        private static IMethodMember BuildGetLookAheadMethod(SyntaxActivationInfo sai, IClassType stateCore, IClassType stateCoreTransition, IClassType streamState, IClassType streamStatePath, string originalPropertyName, string parentPropertyName, IClassType streamStateSources, ITypeReference streamStatePathArray, IClassType buildTransition, IClassType parentChildPair, IExternTypeReference listOfStatePath, string sourcesParamName, IExternTypeReference kvpOfSourcedFromStatePath, string transitionOrdererPointerFieldName)
        {
            const string getLookAheadMethodName = "GetLookAhead";
            var getLookAheadMethod = streamState.Methods.AddNew(new TypedName(getLookAheadMethodName, typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), streamStatePathArray))));
            getLookAheadMethod.IsStatic = true;
            const string groupsLocalName = "groups";
            var groups = getLookAheadMethod.Locals.AddNew(new TypedName(groupsLocalName, typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(stateCore.GetTypeReference(), listOfStatePath))));
            var getLookAheadSourcesParameter = getLookAheadMethod.Parameters.AddNew(new TypedName(sourcesParamName, streamStateSources));
            groups.InitializationExpression = new CreateNewObjectExpression(groups.LocalType);
            getLookAheadMethod.Statements.Add(new CommentStatement(
                "Create a reverse lookup on the original sources.\r\n" +
                "-\r\n" +
                "Sometimes there might be as many as four to ten paths to\r\n" +
                "a given state due to multiple rules targeting the same\r\n" +
                "sub-rule.\r\n" +
                "-\r\n" +
                "The actual look-ahead won't change, so there's no need to\r\n" +
                "calculate the ahead for that state more than once, follow\r\n" +
                "set discovery was completed before this stage."));
            var getLookAheadSourcesEnumeration = getLookAheadMethod.Enumerate(getLookAheadSourcesParameter.GetReference(), kvpOfSourcedFromStatePath);

            getLookAheadSourcesEnumeration.CurrentMember.Name = "fromAndSource";
            var sourcesOriginalSource = getLookAheadSourcesEnumeration.Locals.AddNew(new TypedName("originalSource", stateCore), getLookAheadSourcesEnumeration.CurrentMember.GetReference().GetProperty("Value").GetProperty(originalPropertyName));
            var groupsCheckPath = getLookAheadSourcesEnumeration.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, groups.GetReference().GetMethod("ContainsKey").Invoke(sourcesOriginalSource.GetReference())));
            groupsCheckPath.CallMethod(groups.GetReference().GetMethod("Add").Invoke(sourcesOriginalSource.GetReference(), new CreateNewObjectExpression(listOfStatePath)));
            getLookAheadSourcesEnumeration.CallMethod(groups.GetReference().GetIndex(sourcesOriginalSource.GetReference()).GetMethod("Add").Invoke(getLookAheadSourcesEnumeration.CurrentMember.GetReference().GetProperty("Value")));

            var keysChanged = getLookAheadMethod.Locals.AddNew(new TypedName("keysChanged", typeof(bool)), PrimitiveExpression.FalseValue);
            var transitionKeys = getLookAheadMethod.Locals.AddNew(new TypedName("transitionKeys", sai.data.EnumResults.ResultantType.GetTypeReference().MakeArray(1)));
            transitionKeys.InitializationExpression = new CreateArrayExpression(sai.data.EnumResults.ResultantType.GetTypeReference(), new PrimitiveExpression(0));
            var groupings = getLookAheadMethod.Locals.AddNew(new TypedName("groupings", typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), buildTransition.GetTypeReference()))));
            groupings.InitializationExpression = new CreateNewObjectExpression(groupings.LocalType);


            keysChanged.AutoDeclare = false;
            getLookAheadMethod.DefineLocal(keysChanged);
            transitionKeys.AutoDeclare = false;
            getLookAheadMethod.DefineLocal(transitionKeys);
            groupings.AutoDeclare = false;
            getLookAheadMethod.DefineLocal(groupings);

            var groupEnumeration = getLookAheadMethod.Enumerate(groups.GetReference().GetProperty("Keys"), stateCore.GetTypeReference());
            groupEnumeration.CurrentMember.Name = "currentSource";
            var groupTransitionEnumeration = groupEnumeration.Enumerate(groupEnumeration.CurrentMember.GetReference().GetProperty("Transitions"), stateCoreTransition.GetTypeReference());
            groupTransitionEnumeration.CurrentMember.Name = "currentTransitionInfo";
            var transitionTarget = groupTransitionEnumeration.Locals.AddNew(new TypedName("transitionTarget", stateCore), groupTransitionEnumeration.CurrentMember.GetReference().GetProperty("Target"));
            var sourceTransition = groupTransitionEnumeration.Locals.AddNew(new TypedName("sourceTransition", sai.data.EnumResults.ResultantType), groupTransitionEnumeration.CurrentMember.GetReference().GetProperty("Check"));

            var keysChangedCheck = groupTransitionEnumeration.IfThen(keysChanged.GetReference());
            keysChangedCheck.Statements.Add(
                new CommentStatement("This doesn't occur on cases where the \r\n" +
                                     "previous sourceTransition overlapped \r\n" +
                                     "perfectly on an existing entry."));

            keysChangedCheck.Assign(transitionKeys.GetReference(), typeof(Enumerable).GetTypeReferenceExpression().GetMethod("ToArray", sai.data.EnumResults.ResultantType.GetTypeReference()).Invoke(groupings.GetReference().GetProperty("Keys")));
            keysChangedCheck.Assign(keysChanged.GetReference(), PrimitiveExpression.FalseValue);
            groupTransitionEnumeration.Statements.Add(
                new CommentStatement("Breakdown the current transition against the\r\n" +
                                     "preexisting set."));
            var nextTransitionInfoLabel = new LabelStatement(groupTransitionEnumeration.Statements, "nextTransitionInfo");
            var transitionKeyEnumeration = groupTransitionEnumeration.Enumerate(transitionKeys.GetReference(), sai.data.EnumResults.ResultantType.GetTypeReference());
            transitionKeyEnumeration.CurrentMember.Name = "transitionKey";
            var intersection = transitionKeyEnumeration.Locals.AddNew(new TypedName("intersection", sai.data.EnumResults.ResultantType), new BinaryOperationExpression(transitionKeyEnumeration.CurrentMember.GetReference(), CodeBinaryOperatorType.BitwiseAnd, sourceTransition.GetReference()));
            var intersectionEmptyCheck = transitionKeyEnumeration.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, intersection.GetReference().GetProperty("Empty")));


            var complement = intersectionEmptyCheck.Locals.AddNew(new TypedName("complement", sai.data.EnumResults.ResultantType), sai.data.EnumResults.ResultantType.GetTypeReference().GetTypeExpression().GetMethod("ExclusiveOr").Invoke(transitionKeyEnumeration.CurrentMember.GetReference(), sourceTransition.GetReference()));
            intersectionEmptyCheck.Assign(sourceTransition.GetReference(), new BinaryOperationExpression(sourceTransition.GetReference(), CodeBinaryOperatorType.BitwiseAnd, complement.GetReference()));
            var sourceTransitionEmptyExp = sourceTransition.GetReference().GetProperty("Empty");
            var complementEqualsTransitionKey = new BinaryOperationExpression(transitionKeyEnumeration.CurrentMember.GetReference(), CodeBinaryOperatorType.IdentityEquality, intersection.GetReference());
            var skipChangeExp = new BinaryOperationExpression(sourceTransitionEmptyExp, CodeBinaryOperatorType.BooleanAnd, complementEqualsTransitionKey);
            var skipFullSetChangeCheck = intersectionEmptyCheck.IfThen(skipChangeExp);
            skipFullSetChangeCheck.CallMethod(groupings.GetReference().GetIndex(transitionKeyEnumeration.CurrentMember.GetReference()).GetMethod("Add").Invoke(new CreateNewObjectExpression(parentChildPair.GetTypeReference(), groupEnumeration.CurrentMember.GetReference(), transitionTarget.GetReference())));
            skipFullSetChangeCheck.Statements.Add(nextTransitionInfoLabel.GetGoTo(groupTransitionEnumeration.Statements));

            var backup = intersectionEmptyCheck.Locals.AddNew(new TypedName("backup", buildTransition));
            backup.AutoDeclare = false;
            intersectionEmptyCheck.DefineLocal(backup);
            backup.InitializationExpression = groupings.GetReference().GetIndex(transitionKeyEnumeration.CurrentMember.GetReference());

            intersectionEmptyCheck.CallMethod(groupings.GetReference().GetMethod("Remove").Invoke(transitionKeyEnumeration.CurrentMember.GetReference()));

            var newTransition = intersectionEmptyCheck.Locals.AddNew(new TypedName("newTransition", buildTransition));
            newTransition.InitializationExpression = new CreateNewObjectExpression(buildTransition.GetTypeReference(), backup.GetReference());
            newTransition.AutoDeclare = false;
            intersectionEmptyCheck.DefineLocal(newTransition);
            intersectionEmptyCheck.CallMethod(newTransition.GetReference().GetMethod("Add").Invoke(new CreateNewObjectExpression(parentChildPair.GetTypeReference(), groupEnumeration.CurrentMember.GetReference(), transitionTarget.GetReference())));
            intersectionEmptyCheck.CallMethod(groupings.GetReference().GetMethod("Add").Invoke(intersection.GetReference(), newTransition.GetReference()));
            var transitionRemainder = intersectionEmptyCheck.Locals.AddNew(new TypedName("transitionRemainder", sai.data.EnumResults.ResultantType));
            transitionRemainder.InitializationExpression = new BinaryOperationExpression(transitionKeyEnumeration.CurrentMember.GetReference(), CodeBinaryOperatorType.BitwiseAnd, complement.GetReference());
            transitionRemainder.AutoDeclare = false;
            intersectionEmptyCheck.DefineLocal(transitionRemainder);
            var transitionRemainderNotEmptyCheck = intersectionEmptyCheck.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, transitionRemainder.GetReference().GetProperty("Empty")));
            transitionRemainderNotEmptyCheck.CallMethod(groupings.GetReference().GetMethod("Add").Invoke(transitionRemainder.GetReference(), newTransition.InitializationExpression));

            intersectionEmptyCheck.Assign(keysChanged.GetReference(), PrimitiveExpression.TrueValue);
            var sourceTransitionEmptyCheck = intersectionEmptyCheck.IfThen(sourceTransitionEmptyExp);
            sourceTransitionEmptyCheck.Statements.Add(nextTransitionInfoLabel.GetGoTo(groupTransitionEnumeration.Statements));
            //transitionKeyEnumeration.BreakLocal.AutoDeclare = false;
            //transitionKeyEnumeration.ExitLabel.Skip = true;

            var sourceTransitionNotEmptyCheck = groupTransitionEnumeration.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, sourceTransitionEmptyExp));
            var keysChangedCheck2 = sourceTransitionNotEmptyCheck.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, keysChanged.GetReference()));
            keysChangedCheck2.Assign(keysChanged.GetReference(), PrimitiveExpression.TrueValue);
            var newTransition2 = sourceTransitionNotEmptyCheck.Locals.AddNew(new TypedName("newTransition", buildTransition));
            newTransition2.InitializationExpression = new CreateNewObjectExpression(buildTransition.GetTypeReference(), groupEnumeration.CurrentMember.GetReference(), transitionTarget.GetReference());
            newTransition2.AutoDeclare = false;
            sourceTransitionNotEmptyCheck.DefineLocal(newTransition2);
            sourceTransitionNotEmptyCheck.CallMethod(groupings.GetReference().GetMethod("Add").Invoke(sourceTransition.GetReference(), newTransition2.GetReference()));


            groupTransitionEnumeration.Statements.Add(nextTransitionInfoLabel);

            getLookAheadMethod.Statements.Add(
                new CommentStatement("After finding the look-ahead of the individual\r\n" +
                                     "states involved, use the reverse lookup to construct\r\n" +
                                     "the paths and build the final source look ahead table.\r\n" +
                                     "-\r\n" +
                                     "Once the source table is built, the table works lazily\r\n" +
                                     "and only constructs the individual union paths once a\r\n" +
                                     "transition is requested."));

            var resultDictionary = getLookAheadMethod.Locals.AddNew("result", typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), streamStatePathArray)));
            resultDictionary.AutoDeclare = false;
            getLookAheadMethod.DefineLocal(resultDictionary);
            resultDictionary.InitializationExpression = new CreateNewObjectExpression(resultDictionary.LocalType);

            var transitionOrdererPointerReference = sai.data.EnumResults.ResultantType.GetTypeReference().GetTypeExpression().GetField(transitionOrdererPointerFieldName);

            var orderedGroupTransitionEnumeration = getLookAheadMethod.Enumerate(typeof(Enumerable).GetTypeReferenceExpression().GetMethod("OrderBy", sai.data.EnumResults.ResultantType.GetTypeReference(), typeof(string).GetTypeReference()).Invoke(groupings.GetReference().GetProperty("Keys"), transitionOrdererPointerReference), sai.data.EnumResults.ResultantType.GetTypeReference());
            orderedGroupTransitionEnumeration.CurrentMember.Name = "groupTransition";
            var currentPaths = orderedGroupTransitionEnumeration.Locals.AddNew(new TypedName("currentPaths", listOfStatePath), new CreateNewObjectExpression(listOfStatePath));
            var currentTransition = orderedGroupTransitionEnumeration.Locals.AddNew(new TypedName("currentTransition", buildTransition), groupings.GetReference().GetIndex(orderedGroupTransitionEnumeration.CurrentMember.GetReference()));
            var parentChildEnum = orderedGroupTransitionEnumeration.Enumerate(currentTransition.GetReference(), parentChildPair.GetTypeReference());
            parentChildEnum.CurrentMember.Name = "currentParentChildPair";
            parentChildEnum.Statements.Add(new CommentStatement("Here's where the reverse lookup comes in handy."));
            var pathEnumeration = parentChildEnum.Enumerate(groups.GetReference().GetIndex(parentChildEnum.CurrentMember.GetReference().GetProperty(parentPropertyName)), streamStatePath.GetTypeReference());
            pathEnumeration.CurrentMember.Name = "currentPath";
            var newPathLocal = pathEnumeration.Locals.AddNew(new TypedName("newPath", streamStatePath), new CreateNewObjectExpression(streamStatePath.GetTypeReference(), parentChildEnum.CurrentMember.GetReference().GetProperty("Child"), pathEnumeration.CurrentMember.GetReference().GetProperty("Rule")));

            var currentPathsCheck = pathEnumeration.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, currentPaths.GetReference().GetMethod("Contains").Invoke(newPathLocal.GetReference())));
            currentPathsCheck.CallMethod(currentPaths.GetReference().GetMethod("Add").Invoke(newPathLocal.GetReference()));
            orderedGroupTransitionEnumeration.CallMethod(resultDictionary.GetReference().GetMethod("Add").Invoke(orderedGroupTransitionEnumeration.CurrentMember.GetReference(), currentPaths.GetReference().GetMethod("ToArray").Invoke()));
            getLookAheadMethod.Return(resultDictionary.GetReference());
            return getLookAheadMethod;
        }

        private static void BuildSourcesInfoGetEnumeratorMethod(IClassType streamStateSources, IMethodMember sourcesInfoGetEnum)
        {
            var sourcesInfoGetEnumPrivate = streamStateSources.Methods.AddNew(new TypedName("_GetEnumerator", typeof(IEnumerator).GetTypeReference()));
            sourcesInfoGetEnumPrivate.PrivateImplementationTarget = typeof(IEnumerable).GetTypeReference();
            sourcesInfoGetEnumPrivate.Name = "GetEnumerator";
            sourcesInfoGetEnumPrivate.Return(sourcesInfoGetEnum.GetReference().Invoke());
        }

        private static IMethodMember BuildSourcesInfoGetEnumeratorMethod(IClassType streamStatePath, IClassType streamStateSources, IFieldMember streamStateSourcesInitialSetField, IFieldMember streamStateSourcesFirstSetField, IFieldMember streamStateSourcesFollowSetField, IFieldMember sourcedFromInitial, IFieldMember sourcedFromFirst, IFieldMember sourcedFromFollow, IExternTypeReference kvpOfSourcedFromStatePath)
        {
            var sourcesInfoGetEnum = streamStateSources.Methods.AddNew(new TypedName("GetEnumerator", typeof(IEnumerator<>).GetTypeReference(new TypeReferenceCollection(kvpOfSourcedFromStatePath))));
            sourcesInfoGetEnum.AccessLevel = DeclarationAccessLevel.Public;

            var initialEnumerate = sourcesInfoGetEnum.Enumerate(streamStateSourcesInitialSetField.GetReference(), streamStatePath.GetTypeReference());
            initialEnumerate.CurrentMember.Name = "initial";
            initialEnumerate.Yield(new CreateNewObjectExpression(kvpOfSourcedFromStatePath, sourcedFromInitial.GetReference(), initialEnumerate.CurrentMember.GetReference()));

            var firstEnumerate = sourcesInfoGetEnum.Enumerate(streamStateSourcesFirstSetField.GetReference(), streamStatePath.GetTypeReference());
            firstEnumerate.CurrentMember.Name = "first";
            firstEnumerate.Yield(new CreateNewObjectExpression(kvpOfSourcedFromStatePath, sourcedFromFirst.GetReference(), firstEnumerate.CurrentMember.GetReference()));

            var followEnumerate = sourcesInfoGetEnum.Enumerate(streamStateSourcesFollowSetField.GetReference(), streamStatePath.GetTypeReference());
            followEnumerate.CurrentMember.Name = "follow";
            followEnumerate.Yield(new CreateNewObjectExpression(kvpOfSourcedFromStatePath, sourcedFromFollow.GetReference(), followEnumerate.CurrentMember.GetReference()));
            return sourcesInfoGetEnum;
        }

        private static void BuildSourcedFromEnum(IClassType streamState, string followPropertyName, out IEnumeratorType sourcedFromEnum, out IFieldMember sourcedFromInitial, out IFieldMember sourcedFromFirst, out IFieldMember sourcedFromFollow)
        {
            sourcedFromEnum = streamState.Partials.AddNew().Enumerators.AddNew("SourcedFrom");

            sourcedFromInitial = sourcedFromEnum.Fields.AddNew("Initial");
            sourcedFromInitial.Summary = "The path was sourced from the initial set for the state.";
            sourcedFromFirst = sourcedFromEnum.Fields.AddNew("First");
            sourcedFromFirst.Summary =
                "The path was sourced from the sub-rule states introduced into the stream.";
            sourcedFromFollow = sourcedFromEnum.Fields.AddNew(followPropertyName);
            sourcedFromFollow.Summary =
                "The path was sourced from the terminal rule edges of a given state\r\n" +
                "causing the state following the caller of the rule to be introduced\r\n" +
                "into the stream.";
            sourcedFromEnum.AccessLevel = DeclarationAccessLevel.Public;
        }

        private static void BuildRulePathLineContainsMethod(IEnumeratorType ruleIdentifierEnum, IClassType stateCore, IClassType streamStatePath, IClassType streamRulePath, string isRulePropertyName, string originalPropertyName, string parentPropertyName, string followSetPropertyName, IClassType followInfoCore, string lineContainsMethodName)
        {
            var streamRuleLineContains = streamRulePath.Methods.AddNew(new TypedName(lineContainsMethodName, typeof(bool)));
            streamRuleLineContains.AccessLevel = DeclarationAccessLevel.Public;

            var streamRuleLineContainsTargetParam = streamRuleLineContains.Parameters.AddNew(new TypedName("target", ruleIdentifierEnum));
            var streamRuleLineContiansFollowOwner = streamRuleLineContains.Parameters.AddNew(new TypedName("followOwner", streamRulePath));
            var streamRuleLineContainsFollower = streamRuleLineContains.Parameters.AddNew(new TypedName("follower", stateCore));
            var streamRuleLineContainsParent = streamRuleLineContains.Parameters.AddNew(new TypedName("parent", streamStatePath));
            streamRuleLineContains.Statements.Add(new CommentStatement(
                "Line contains is a specialized check to determine if\r\n" +
                "left recursion exists on the current rule.\r\n" +
                "-\r\n" +
                "With exception to initial states being a terminal\r\n" +
                "edge; thus, resulting in possible hidden left\r\n" +
                "recursion, this should always be called on rules\r\n" +
                "only."));
            var streamRuleLineContainsCurrent = streamRuleLineContains.Locals.AddNew(new TypedName("currentState", streamStatePath));
            var streamRuleLineContainsCurrentRule = streamRuleLineContains.Locals.AddNew(new TypedName("currentRule", streamRulePath));
            var streamRuleLineContainsFollowInfo = streamRuleLineContains.Locals.AddNew(new TypedName("followData", followInfoCore));
            streamRuleLineContainsCurrent.AutoDeclare = false;
            streamRuleLineContainsCurrentRule.AutoDeclare = false;
            streamRuleLineContainsFollowInfo.AutoDeclare = false;
            streamRuleLineContainsFollowInfo.InitializationExpression = PrimitiveExpression.NullValue;
            streamRuleLineContainsCurrent.InitializationExpression = new ThisReferenceExpression();
            streamRuleLineContainsCurrentRule.InitializationExpression = streamRuleLineContainsCurrent.InitializationExpression;
            streamRuleLineContains.DefineLocal(streamRuleLineContainsCurrent);
            streamRuleLineContains.DefineLocal(streamRuleLineContainsFollowInfo);
            var lineDo = streamRuleLineContains.Iterate(streamRuleLineContainsCurrentRule.GetDeclarationStatement(), null, new BinaryOperationExpression(streamRuleLineContainsCurrentRule.GetReference(), CodeBinaryOperatorType.IdentityInequality, PrimitiveExpression.NullValue));
            var lineDoIdCheck = lineDo.IfThen(new BinaryOperationExpression(streamRuleLineContainsCurrentRule.GetReference().GetProperty(originalPropertyName).GetProperty("RuleId"), CodeBinaryOperatorType.IdentityEquality, streamRuleLineContainsTargetParam.GetReference()));
            var lineDoIdCheckFollowCheck = lineDoIdCheck.IfThen(new BinaryOperationExpression(streamRuleLineContainsFollowInfo.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            lineDoIdCheckFollowCheck.Assign(streamRuleLineContainsFollowInfo.GetReference(), new CreateNewObjectExpression(followInfoCore.GetTypeReference(), streamRuleLineContiansFollowOwner.GetReference(), streamRuleLineContainsFollower.GetReference()));
            var lineDoIdCheckFollowSetCheck = lineDoIdCheck.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, streamRuleLineContainsCurrentRule.GetReference().GetProperty(followSetPropertyName).GetMethod("Contains").Invoke(streamRuleLineContainsFollowInfo.GetReference())));
            lineDoIdCheckFollowSetCheck.CallMethod(streamRuleLineContainsCurrentRule.GetReference().GetProperty(followSetPropertyName).GetMethod("Add").Invoke(streamRuleLineContainsFollowInfo.GetReference()));
            lineDoIdCheck.Return(PrimitiveExpression.TrueValue);
            lineDo.Assign(streamRuleLineContainsCurrent.GetReference(), streamRuleLineContainsCurrentRule.GetReference().GetProperty(parentPropertyName));
            var lineDoRuleCheck = lineDo.IfThen(new BinaryOperationExpression(new BinaryOperationExpression(streamRuleLineContainsCurrent.GetReference(), CodeBinaryOperatorType.IdentityInequality, PrimitiveExpression.NullValue), CodeBinaryOperatorType.BooleanAnd, streamRuleLineContainsCurrent.GetReference().GetProperty(isRulePropertyName)));
            lineDoRuleCheck.Statements.Add(new CommentStatement(isRulePropertyName + " property due to lack of 'is' statement\r\nin generator."));
            lineDoRuleCheck.Assign(streamRuleLineContainsCurrentRule.GetReference(), new CastExpression(streamRuleLineContainsCurrent.GetReference(), streamRulePath.GetTypeReference()));

            lineDoRuleCheck.FalseBlock.Return(PrimitiveExpression.FalseValue);
            streamRuleLineContains.Return(PrimitiveExpression.FalseValue);
        }

        private static void BuildSourcesInfoIndexer(IClassType streamStatePath, IClassType streamStateSources, IFieldMember streamStateSourcesInitialSetField, IFieldMember streamStateSourcesFirstSetField, IFieldMember streamStateSourcesFollowSetField)
        {
            #region SourcesInfo Indexer

            //public *StatePath this[int index]
            //{
            var streamStateSourcesIndexer = streamStateSources.Properties.AddNew(streamStatePath.GetTypeReference(), true, false, new TypedName("index", typeof(int)));
            var streamStateSourcesIndexerParam = streamStateSourcesIndexer.Parameters["index"];
            streamStateSourcesIndexer.AccessLevel = DeclarationAccessLevel.Public;

            //    get
            //    {

            //        int r1 = 0;
            var streamStateSourcesIndexer1Local = streamStateSourcesIndexer.GetPart.Locals.AddNew(new TypedName("r1", typeof(int)), PrimitiveExpression.NumberZero);

            //        int r2 = this.initialSet.Length;
            var streamStateSourcesIndexer2Local = streamStateSourcesIndexer.GetPart.Locals.AddNew(new TypedName("r2", typeof(int)), streamStateSourcesInitialSetField.GetReference().GetProperty("Length"));

            //        int r3 = r2 + this.firstSet.Length;
            var streamStateSourcesIndexer3Local = streamStateSourcesIndexer.GetPart.Locals.AddNew(new TypedName("r3", typeof(int)), new BinaryOperationExpression(streamStateSourcesIndexer2Local.GetReference(), CodeBinaryOperatorType.Add, streamStateSourcesFirstSetField.GetReference().GetProperty("Length")));

            //        int r4 = r3 + this.followSet.Length;
            var streamStateSourcesIndexer4Local = streamStateSourcesIndexer.GetPart.Locals.AddNew(new TypedName("r4", typeof(int)), new BinaryOperationExpression(streamStateSourcesIndexer3Local.GetReference(), CodeBinaryOperatorType.Add, streamStateSourcesFollowSetField.GetReference().GetProperty("Length")));

            //        if (index >= r1 && index < r2)
            var streamStateSourcesInitialRangeCheck = streamStateSourcesIndexer.GetPart.IfThen(
                new BinaryOperationExpression(
                    new BinaryOperationExpression(
                        streamStateSourcesIndexerParam.GetReference(),
                        CodeBinaryOperatorType.GreaterThanOrEqual,
                        streamStateSourcesIndexer1Local.GetReference()),
                    CodeBinaryOperatorType.BooleanAnd,
                    new BinaryOperationExpression(
                        streamStateSourcesIndexerParam.GetReference(),
                        CodeBinaryOperatorType.LessThan,
                        streamStateSourcesIndexer2Local.GetReference())));

            //            return this.initialSet[index];
            streamStateSourcesInitialRangeCheck.Return(streamStateSourcesInitialSetField.GetReference().GetIndex(streamStateSourcesIndexerParam.GetReference()));

            //        else if (index >= r2 && index < r3)
            var streamStateSourcesFirstRangeCheck = streamStateSourcesInitialRangeCheck.FalseBlock.IfThen(
                new BinaryOperationExpression(
                    new BinaryOperationExpression(
                        streamStateSourcesIndexerParam.GetReference(),
                        CodeBinaryOperatorType.GreaterThanOrEqual,
                        streamStateSourcesIndexer2Local.GetReference()),
                    CodeBinaryOperatorType.BooleanAnd,
                    new BinaryOperationExpression(
                        streamStateSourcesIndexerParam.GetReference(),
                        CodeBinaryOperatorType.LessThan,
                        streamStateSourcesIndexer3Local.GetReference())));

            //            return this.firstSet[index - r2];
            streamStateSourcesFirstRangeCheck.Return(streamStateSourcesFirstSetField.GetReference().GetIndex(new BinaryOperationExpression(streamStateSourcesIndexerParam.GetReference(), CodeBinaryOperatorType.Subtract, streamStateSourcesIndexer2Local.GetReference())));

            //        else if (index >= r3 && index < r4)
            var streamStateSourcesFollowRangeCheck = streamStateSourcesFirstRangeCheck.FalseBlock.IfThen(
                new BinaryOperationExpression(
                    new BinaryOperationExpression(
                        streamStateSourcesIndexerParam.GetReference(),
                        CodeBinaryOperatorType.GreaterThanOrEqual,
                        streamStateSourcesIndexer3Local.GetReference()),
                    CodeBinaryOperatorType.BooleanAnd,
                    new BinaryOperationExpression(
                        streamStateSourcesIndexerParam.GetReference(),
                        CodeBinaryOperatorType.LessThan,
                        streamStateSourcesIndexer4Local.GetReference())));

            //            return this.followSet[index - r3];
            streamStateSourcesFollowRangeCheck.Return(streamStateSourcesFollowSetField.GetReference().GetIndex(new BinaryOperationExpression(streamStateSourcesIndexerParam.GetReference(), CodeBinaryOperatorType.Subtract, streamStateSourcesIndexer3Local.GetReference())));

            //        return null;
            streamStateSourcesIndexer.GetPart.Return(PrimitiveExpression.NullValue);
            #endregion
        }

        private static void BuildSourcesBuilderDisposeMethod(IClassType sourcesBuilderCore, IFieldMember builderFirstSetField, IFieldMember builderFollowSetField, IFieldMember builderInitialSetField, IFieldMember builderFullSetField)
        {
            #region SourcesBuilder Dispose Method
            sourcesBuilderCore.ImplementsList.Add(typeof(IDisposable));
            var builderDisposeMethod = sourcesBuilderCore.Methods.AddNew(new TypedName("Dispose", typeof(void)));
            builderDisposeMethod.AccessLevel = DeclarationAccessLevel.Public;
            builderDisposeMethod.CallMethod(builderInitialSetField.GetReference().GetMethod("Clear").Invoke());
            builderDisposeMethod.Assign(builderInitialSetField.GetReference(), PrimitiveExpression.NullValue);
            builderDisposeMethod.CallMethod(builderFirstSetField.GetReference().GetMethod("Clear").Invoke());
            builderDisposeMethod.Assign(builderFirstSetField.GetReference(), PrimitiveExpression.NullValue);
            builderDisposeMethod.CallMethod(builderFollowSetField.GetReference().GetMethod("Clear").Invoke());
            builderDisposeMethod.Assign(builderFollowSetField.GetReference(), PrimitiveExpression.NullValue);
            builderDisposeMethod.CallMethod(builderFullSetField.GetReference().GetMethod("Clear").Invoke());
            builderDisposeMethod.Assign(builderFullSetField.GetReference(), PrimitiveExpression.NullValue);
            #endregion
        }

        private static void BuildSourcesBuilderIndexer(IClassType streamStatePath, IClassType sourcesBuilderCore, IFieldMember builderFirstSetField, IFieldMember builderFollowSetField, IFieldMember builderInitialSetField)
        {
            #region SourcesBuilder Indexer
            //public *StatePath this[int index]
            //{
            var builderIndexer = sourcesBuilderCore.Properties.AddNew(streamStatePath.GetTypeReference(), true, false, new TypedName("index", typeof(int)));
            var builderIndexerParam = builderIndexer.Parameters["index"];
            builderIndexer.AccessLevel = DeclarationAccessLevel.Public;

            //    get
            //    {

            //        int r1 = 0;
            var builderIndexer1Local = builderIndexer.GetPart.Locals.AddNew(new TypedName("r1", typeof(int)), PrimitiveExpression.NumberZero);

            //        int r2 = this.initialSet.Count;
            var builderIndexer2Local = builderIndexer.GetPart.Locals.AddNew(new TypedName("r2", typeof(int)), builderInitialSetField.GetReference().GetProperty("Count"));

            //        int r3 = r2 + this.firstSet.Count;
            var builderIndexer3Local = builderIndexer.GetPart.Locals.AddNew(new TypedName("r3", typeof(int)), new BinaryOperationExpression(builderIndexer2Local.GetReference(), CodeBinaryOperatorType.Add, builderFirstSetField.GetReference().GetProperty("Count")));

            //        int r4 = r3 + this.followSet.Count;
            var builderIndexer4Local = builderIndexer.GetPart.Locals.AddNew(new TypedName("r4", typeof(int)), new BinaryOperationExpression(builderIndexer3Local.GetReference(), CodeBinaryOperatorType.Add, builderFollowSetField.GetReference().GetProperty("Count")));

            //        if (index >= r1 && index < r2)
            var builderInitialRangeCheck = builderIndexer.GetPart.IfThen(
                new BinaryOperationExpression(
                    new BinaryOperationExpression(
                        builderIndexerParam.GetReference(),
                        CodeBinaryOperatorType.GreaterThanOrEqual,
                        builderIndexer1Local.GetReference()),
                    CodeBinaryOperatorType.BooleanAnd,
                    new BinaryOperationExpression(
                        builderIndexerParam.GetReference(),
                        CodeBinaryOperatorType.LessThan,
                        builderIndexer2Local.GetReference())));

            //            return this.initialSet[index];
            builderInitialRangeCheck.Return(builderInitialSetField.GetReference().GetIndex(builderIndexerParam.GetReference()));

            //        else if (index >= r2 && index < r3)
            var builderFirstRangeCheck = builderInitialRangeCheck.FalseBlock.IfThen(
                new BinaryOperationExpression(
                    new BinaryOperationExpression(
                        builderIndexerParam.GetReference(),
                        CodeBinaryOperatorType.GreaterThanOrEqual,
                        builderIndexer2Local.GetReference()),
                    CodeBinaryOperatorType.BooleanAnd,
                    new BinaryOperationExpression(
                        builderIndexerParam.GetReference(),
                        CodeBinaryOperatorType.LessThan,
                        builderIndexer3Local.GetReference())));

            //            return this.firstSet[index - r2];
            builderFirstRangeCheck.Return(builderFirstSetField.GetReference().GetIndex(new BinaryOperationExpression(builderIndexerParam.GetReference(), CodeBinaryOperatorType.Subtract, builderIndexer2Local.GetReference())));

            //        else if (index >= r3 && index < r4)
            var builderFollowRangeCheck = builderFirstRangeCheck.FalseBlock.IfThen(
                new BinaryOperationExpression(
                    new BinaryOperationExpression(
                        builderIndexerParam.GetReference(),
                        CodeBinaryOperatorType.GreaterThanOrEqual,
                        builderIndexer3Local.GetReference()),
                    CodeBinaryOperatorType.BooleanAnd,
                    new BinaryOperationExpression(
                        builderIndexerParam.GetReference(),
                        CodeBinaryOperatorType.LessThan,
                        builderIndexer4Local.GetReference())));

            //            return this.followSet[index - r3];
            builderFollowRangeCheck.Return(builderFollowSetField.GetReference().GetIndex(new BinaryOperationExpression(builderIndexerParam.GetReference(), CodeBinaryOperatorType.Subtract, builderIndexer3Local.GetReference())));

            //        return null;
            builderIndexer.GetPart.Return(PrimitiveExpression.NullValue);

            #endregion
        }

        private static void BuildSourcesBuilderCountProperty(IClassType sourcesBuilderCore, IFieldMember builderFirstSetField, IFieldMember builderFollowSetField, IFieldMember builderInitialSetField)
        {
            #region SourcesBuilder Count
            var sourcesBuilderCount = sourcesBuilderCore.Properties.AddNew(new TypedName("Count", typeof(int)), true, false);
            sourcesBuilderCount.AccessLevel = DeclarationAccessLevel.Public;
            sourcesBuilderCount.GetPart.Return(new BinaryOperationExpression(builderInitialSetField.GetReference().GetProperty("Count"), CodeBinaryOperatorType.Add, new BinaryOperationExpression(builderFirstSetField.GetReference().GetProperty("Count"), CodeBinaryOperatorType.Add, builderFollowSetField.GetReference().GetProperty("Count"))));
            #endregion
        }

        private static void BuildStreamStateDataMembers(SyntaxActivationInfo sai, IEnumeratorType ruleIdentifierEnum, IClassType streamState, string ruleCacheFieldName, string stateCacheFieldName, out IExternTypeReference listOfStreamState, out IFieldMember stateCacheField)
        {
            #region Stream State Data members
            var ruleCacheField = streamState.Fields.AddNew(new TypedName(ruleCacheFieldName, typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(ruleIdentifierEnum.GetTypeReference(), streamState.GetTypeReference()))));
            ruleCacheField.InitializationExpression = new CreateNewObjectExpression(ruleCacheField.FieldType);
            ruleCacheField.IsStatic = true;

            listOfStreamState = typeof(List<>).GetTypeReference(new TypeReferenceCollection(streamState.GetTypeReference()));
            stateCacheField = streamState.Fields.AddNew(new TypedName(stateCacheFieldName, typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(sai.data.EnumResults.ResultantType.GetTypeReference(), listOfStreamState))));
            stateCacheField.InitializationExpression = new CreateNewObjectExpression(stateCacheField.FieldType);
            stateCacheField.IsStatic = true;

            #endregion
        }

        private static IMethodMember BuildObtainStateMethod(SyntaxActivationInfo sai, IClassType streamState, IClassType streamStatePath, string followSetPropertyName, string initialSetPropertyName, string firstSetPropertyName, string fullSetPropertyName, string getSourcesMethodName, IClassType sourcesBuilderCore, string obtainStateMethodName, string sourcesParamName, string transitionParamName, string sourcesLocalName, string pathSetParam, string tryTargetLocalName, string sourceBuilderLocalName, IMethodMember calculateFollowSetMethod, IExternTypeReference listOfStreamState, IFieldMember stateCacheField, IMethodMember getSourceSetSimple)
        {
            #region StreamState ObtainState Method

            var obtainStateMethod = streamState.Methods.AddNew(new TypedName(obtainStateMethodName, streamState));
            var obtainStatePathSetParam = obtainStateMethod.Parameters.AddNew(new TypedName(pathSetParam, streamStatePath.GetTypeReference().MakeArray(1)));
            var obtainStateTransitionParam = obtainStateMethod.Parameters.AddNew(new TypedName(transitionParamName, sai.data.EnumResults.ResultantType));
            obtainStateMethod.IsStatic = true;

            var obtainStateTryTarget = obtainStateMethod.Locals.AddNew(new TypedName(tryTargetLocalName, listOfStreamState));
            obtainStateTryTarget.AutoDeclare = false;
            obtainStateMethod.Statements.Add(new CommentStatement("The target series of stream states to check against."));
            obtainStateMethod.DefineLocal(obtainStateTryTarget);
            var negativeTryTargetCheck = obtainStateMethod.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, streamState.GetTypeReference().GetTypeExpression().GetField(stateCacheField.Name).GetMethod("TryGetValue").Invoke(obtainStateTransitionParam.GetReference(), new DirectionExpression(FieldDirection.Out, obtainStateTryTarget.GetReference()))));

            negativeTryTargetCheck.Assign(obtainStateTryTarget.GetReference(), new CreateNewObjectExpression(listOfStreamState));
            negativeTryTargetCheck.CallMethod(streamState.GetTypeReference().GetTypeExpression().GetField(stateCacheField.Name).GetMethod("Add").Invoke(obtainStateTransitionParam.GetReference(), obtainStateTryTarget.GetReference()));

            var obtainStateSourceBuilder = obtainStateMethod.Locals.AddNew(new TypedName(sourceBuilderLocalName, sourcesBuilderCore));
            obtainStateSourceBuilder.AutoDeclare = false;
            obtainStateSourceBuilder.InitializationExpression = new CreateNewObjectExpression(sourcesBuilderCore.GetTypeReference(), obtainStatePathSetParam.GetReference());
            obtainStateMethod.DefineLocal(obtainStateSourceBuilder);

            obtainStateMethod.CallMethod(calculateFollowSetMethod.GetReference().Invoke(obtainStateSourceBuilder.GetReference(), new CreateNewObjectExpression(typeof(List<>).GetTypeReference(new TypeReferenceCollection(streamStatePath.GetTypeReference())),obtainStateSourceBuilder.GetReference().GetProperty(fullSetPropertyName))));
            obtainStateMethod.CallMethod(getSourceSetSimple.GetReference().Invoke(obtainStateSourceBuilder.GetReference()));

            var tryTargetEnumeration = obtainStateMethod.Enumerate(obtainStateTryTarget.GetReference(), streamState.GetTypeReference());
            tryTargetEnumeration.CurrentMember.Name = "tryState";

            var tryTargetCountCheckExpressionA = new BinaryOperationExpression(tryTargetEnumeration.CurrentMember.GetReference().GetField(sourcesParamName).GetProperty(initialSetPropertyName).GetProperty("Length"), CodeBinaryOperatorType.IdentityEquality, obtainStateSourceBuilder.GetReference().GetProperty(initialSetPropertyName).GetProperty("Count"));
            var tryTargetCountCheckExpressionB = new BinaryOperationExpression(tryTargetEnumeration.CurrentMember.GetReference().GetField(sourcesParamName).GetProperty(firstSetPropertyName).GetProperty("Length"), CodeBinaryOperatorType.IdentityEquality, obtainStateSourceBuilder.GetReference().GetProperty(firstSetPropertyName).GetProperty("Count"));
            var tryTargetCountCheckExpressionC = new BinaryOperationExpression(tryTargetEnumeration.CurrentMember.GetReference().GetField(sourcesParamName).GetProperty(followSetPropertyName).GetProperty("Length"), CodeBinaryOperatorType.IdentityEquality, obtainStateSourceBuilder.GetReference().GetProperty(followSetPropertyName).GetProperty("Count"));
            var tryTargetCountCheckExpression = new BinaryOperationExpression(tryTargetCountCheckExpressionA, CodeBinaryOperatorType.BooleanAnd, new BinaryOperationExpression(tryTargetCountCheckExpressionB, CodeBinaryOperatorType.BooleanAnd, tryTargetCountCheckExpressionC));

            //    //The length->count comparison should expedite the overall check
            //    //versus the full source count: this should be fractionally faster.
            tryTargetEnumeration.Statements.Add(new CommentStatement("The length->count comparison should expedite the overall check\r\nversus the full source count: this should be fractionally faster."));
            //    if ((tryState.sources.InitialSet.Length == sourceBuilder.InitialSet.Count) && ((tryState.sources.FirstSet.Length == sourceBuilder.FirstSet.Count) && (tryState.sources.FollowSet.Length == sourceBuilder.FollowSet.Count)))
            var tryTargetCountCheck = tryTargetEnumeration.IfThen(tryTargetCountCheckExpression);
            //    {
            //        bool fullMatchFound = true;
            var fullMatchFound = tryTargetCountCheck.Locals.AddNew(new TypedName("fullMatchFound", typeof(bool)), PrimitiveExpression.TrueValue);
            var trySourceIndex = tryTargetCountCheck.Locals.AddNew(new TypedName("trySourceIndex", typeof(int)), PrimitiveExpression.NumberZero);
            trySourceIndex.AutoDeclare = false;
            //        for (int trySourceIndex = 0; trySourceIndex < tryState.sources.Count; trySourceIndex++)
            var tryStateSourceIteration = tryTargetCountCheck.Iterate(trySourceIndex.GetDeclarationStatement(), new CrementStatement(CrementType.Postfix, CrementOperation.Increment, trySourceIndex.GetReference()), new BinaryOperationExpression(trySourceIndex.GetReference(), CodeBinaryOperatorType.LessThan, tryTargetEnumeration.CurrentMember.GetReference().GetField(sourcesLocalName).GetProperty("Count")));
            //        {
            //            StatePath tryPath = tryState.sources[trySourceIndex];
            var tryPath = tryStateSourceIteration.Locals.AddNew(new TypedName("tryPath", streamStatePath), tryTargetEnumeration.CurrentMember.GetReference().GetField(sourcesLocalName).GetIndex(trySourceIndex.GetReference()));
            //            bool matchFound = false;
            var tryMatchFound = tryStateSourceIteration.Locals.AddNew(new TypedName("matchFound", typeof(bool)), PrimitiveExpression.FalseValue);

            var buildSourceIndex = tryStateSourceIteration.Locals.AddNew(new TypedName("buildSourceIndex", typeof(int)), PrimitiveExpression.NumberZero);
            buildSourceIndex.AutoDeclare = false;
            //            for (int buildSourceIndex = 0; buildSourceIndex < sourceBuilder.Count; buildSourceIndex++)
            var buildSourceIteration = tryStateSourceIteration.Iterate(buildSourceIndex.GetDeclarationStatement(), new CrementStatement(CrementType.Postfix, CrementOperation.Increment, buildSourceIndex.GetReference()), new BinaryOperationExpression(buildSourceIndex.GetReference(), CodeBinaryOperatorType.LessThan, obtainStateSourceBuilder.GetReference().GetProperty("Count")));
            //            {
            //                if (tryPath.Equals(sourceBuilder[buildSourceIndex])
            var buildSourceCheck = buildSourceIteration.IfThen(tryPath.GetReference().GetMethod("Equals").Invoke(obtainStateSourceBuilder.GetReference().GetIndex(buildSourceIndex.GetReference())));
            //                {
            buildSourceCheck.Assign(tryMatchFound.GetReference(), PrimitiveExpression.TrueValue);
            //                    matchFound = true;
            buildSourceCheck.Break();
            //                    break;
            //                }
            //            }
            //            if (!matchFound)
            var noMatchFoundCheck = tryStateSourceIteration.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, tryMatchFound.GetReference()));
            //            {
            //                fullMatchFound = false;
            noMatchFoundCheck.Assign(fullMatchFound.GetReference(), PrimitiveExpression.FalseValue);
            //                break;
            noMatchFoundCheck.Break();
            //            }
            //        }
            //    }
            tryStateSourceIteration.BreakLocal.AutoDeclare = false;
            tryStateSourceIteration.ExitLabel.Skip = true;
            buildSourceIteration.BreakLocal.AutoDeclare = false;
            buildSourceIteration.ExitLabel.Skip = true;


            var tryFullFound = tryTargetCountCheck.IfThen(fullMatchFound.GetReference());
            tryFullFound.Return(tryTargetEnumeration.CurrentMember.GetReference());
            var obtainStateResultLocal = obtainStateMethod.Locals.AddNew(new TypedName("result", streamState));
            obtainStateResultLocal.InitializationExpression = new CreateNewObjectExpression(streamState.GetTypeReference(), obtainStateSourceBuilder.GetReference().GetMethod(getSourcesMethodName).Invoke());
            obtainStateResultLocal.AutoDeclare = false;
            obtainStateMethod.DefineLocal(obtainStateResultLocal);
            obtainStateMethod.CallMethod(obtainStateTryTarget.GetReference().GetMethod("Add").Invoke(obtainStateResultLocal.GetReference()));
            obtainStateMethod.Return(obtainStateResultLocal.GetReference());
            #endregion
            return obtainStateMethod;
        }

        private static IMethodMember BuildObtainRuleMethod(Dictionary<IProductionRuleEntry, FlattenedRuleEntry> ruleData, IEnumeratorType ruleIdentifierEnum, Dictionary<IProductionRuleEntry, IFieldMember> ruleIdentifierFieldLookup, IClassType declarationRuleCore, Dictionary<FlattenedRuleEntry, IMethodMember> ruleDeclarationMethodMapping, IClassType streamState, IClassType streamStatePath, IClassType streamRulePath, string getSourcesMethodName, IClassType sourcesBuilderCore, string obtainRuleMethodName, string sourceIdParamName, string ruleCacheFieldName, string sourceLocalName, string sourcesLocalName, string nodeName, IMethodMember getSourceSetSimple)
        {
            #region StreamState ObtainRule Method
            var obtainRuleMethod = streamState.Methods.AddNew(new TypedName(obtainRuleMethodName, streamState));
            obtainRuleMethod.AccessLevel = DeclarationAccessLevel.Public;
            var sourceId = obtainRuleMethod.Parameters.AddNew(new TypedName(sourceIdParamName, ruleIdentifierEnum));
            obtainRuleMethod.IsStatic = true;
            var obtainRuleCacheCheck = obtainRuleMethod.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, streamState.GetTypeReference().GetTypeExpression().GetField(ruleCacheFieldName).GetMethod("ContainsKey").Invoke(sourceId.GetReference())));

            var obtainRuleSource = obtainRuleCacheCheck.Locals.AddNew(new TypedName(sourceLocalName, declarationRuleCore.GetTypeReference()), PrimitiveExpression.NullValue);
            var obtainRuleSwitch = obtainRuleCacheCheck.SelectCase(sourceId.GetReference());
            foreach (IProductionRuleEntry entry in ruleData.Keys)
            {
                var currentCase = obtainRuleSwitch.Cases.AddNew(ruleIdentifierFieldLookup[entry].GetReference());
                currentCase.Assign(obtainRuleSource.GetReference(), ruleDeclarationMethodMapping[ruleData[entry]].GetReference().Invoke());
            }
            var obtainRuleNullCheck = obtainRuleCacheCheck.IfThen(new BinaryOperationExpression(obtainRuleSource.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
            obtainRuleNullCheck.Return(PrimitiveExpression.NullValue);
            var obtainRuleNodeLocal = obtainRuleCacheCheck.Locals.AddNew(new TypedName(nodeName, streamRulePath));
            obtainRuleNodeLocal.InitializationExpression = new CreateNewObjectExpression(streamRulePath.GetTypeReference(), obtainRuleSource.GetReference());
            obtainRuleNodeLocal.AutoDeclare = false;
            obtainRuleCacheCheck.DefineLocal(obtainRuleNodeLocal);
            var obtainRuleSourcesLocal = obtainRuleCacheCheck.Locals.AddNew(new TypedName(sourcesLocalName, sourcesBuilderCore));
            obtainRuleSourcesLocal.InitializationExpression = new CreateNewObjectExpression(sourcesBuilderCore.GetTypeReference(), new CreateArrayExpression(streamStatePath, new IExpression[] { obtainRuleNodeLocal.GetReference() }));

            obtainRuleSourcesLocal.AutoDeclare = false;
            obtainRuleCacheCheck.DefineLocal(obtainRuleSourcesLocal);

            obtainRuleCacheCheck.CallMethod(getSourceSetSimple.GetReference().Invoke(obtainRuleSourcesLocal.GetReference()));
            var obtainRuleResult = obtainRuleCacheCheck.Locals.AddNew(new TypedName("result", streamState));
            obtainRuleResult.InitializationExpression = new CreateNewObjectExpression(streamState.GetTypeReference(), obtainRuleSourcesLocal.GetReference().GetMethod(getSourcesMethodName).Invoke());
            obtainRuleResult.AutoDeclare = false;
            obtainRuleCacheCheck.DefineLocal(obtainRuleResult);

            obtainRuleCacheCheck.CallMethod(streamState.GetTypeReference().GetTypeExpression().GetField(ruleCacheFieldName).GetMethod("Add").Invoke(sourceId.GetReference(), obtainRuleResult.GetReference()));

            obtainRuleMethod.Return(streamState.GetTypeReference().GetTypeExpression().GetField(ruleCacheFieldName).GetIndex(sourceId.GetReference()));
            #endregion
            return obtainRuleMethod;
        }

        private static void BuildGetSourceSetMethodBody(IEnumeratorType ruleIdentifierEnum, IClassType stateCore, IClassType stateRulePushCore, IClassType streamRulePath, string isRulePropertyName, string lineContainsMethodName, IExternTypeReference listOfStatePath, IMethodMember calculateFollowSetMethod, IMethodMember getSourceSetMethod, IMethodParameterMember getSourceSetSourcesParam, IMethodParameterMember getSourceSetCurrentPath)
        {

            #region StreamState GetSourcesSet Body
            const string originalStateLocalName = "originalState";
            const string currentRulePathLocalName = "currentRulePath";
            const string newlyAddedLocalName = "newlyAdded";
            const string forwardNodeLocalName = "firstNode";
            var originalState = getSourceSetMethod.Locals.AddNew(new TypedName(originalStateLocalName, stateCore));
            originalState.InitializationExpression = getSourceSetCurrentPath.GetReference().GetProperty("Original");
            var currentRulePath = getSourceSetMethod.Locals.AddNew(new TypedName(currentRulePathLocalName, streamRulePath), PrimitiveExpression.NullValue);
            var newlyAdded = getSourceSetMethod.Locals.AddNew(new TypedName(newlyAddedLocalName, listOfStatePath), new CreateNewObjectExpression(listOfStatePath));

            var currentRulePathCheck = getSourceSetMethod.IfThen(getSourceSetCurrentPath.GetReference().GetProperty(isRulePropertyName));
            currentRulePathCheck.Assign(currentRulePath.GetReference(), new CastExpression(getSourceSetCurrentPath.GetReference(), streamRulePath.GetTypeReference()));

            getSourceSetMethod.Statements.Add(new CommentStatement(
                "Gather the rule references at the current level and construct\r\n" +
                "nodes relative to each item, delving further when necessary."));

            var firstEnumeration = getSourceSetMethod.Enumerate(originalState.GetReference().GetProperty("Keys"), ruleIdentifierEnum.GetTypeReference());
            firstEnumeration.CurrentMember.Name = "id";
            var firstNode = firstEnumeration.Locals.AddNew(new TypedName(forwardNodeLocalName, streamRulePath));
            var ruleTransition = firstEnumeration.Locals.AddNew(new TypedName("ruleTransition", stateRulePushCore.GetTypeReference()), originalState.GetReference().GetIndex(firstEnumeration.CurrentMember.GetReference()));
            firstEnumeration.Statements.Add(new CommentStatement(
                "Left recursive rules are only evaluated this far, to ensure the\r\n" +
                "entire thing doesn't recurse infinitely.\r\n" +
                "-\r\n" +
                "The trick here is to check whether the current path contains the\r\n" +
                "rule already (from the last non-rule state only), if it does it\r\n" +
                "stores the source of origin and the follow state.\r\n" +
                "-\r\n" +
                "It doesn't do any good to include the left recursive path because\r\n" +
                "its children aren't evaluated and it won't persist through the rest\r\n" +
                "of the states.\r\n" +
                "-\r\n" +
                "Also, they wouldn't add anything to the first set, or the follow\r\n" +
                "set; the follow portion is taken care of by LineContains, which\r\n" +
                "adds the path information to its own follow information, effectively\r\n" +
                "as long as the follow states at the terminal points of that rule\r\n" +
                "persist, it's still recursing on the left side."));
            var ruleNodeNullCheckExp = new BinaryOperationExpression(currentRulePath.GetReference(), CodeBinaryOperatorType.IdentityInequality, PrimitiveExpression.NullValue);
            var ruleNodeLineContainsExp = currentRulePath.GetReference().GetMethod(lineContainsMethodName).Invoke(
                firstEnumeration.CurrentMember.GetReference(),
                currentRulePath.GetReference(),
                ruleTransition.GetReference().GetProperty("FollowState"),
                getSourceSetCurrentPath.GetReference());
            var continuePoint = new LabelStatement(firstEnumeration.Statements, "continuePoint");
            var leftRecursionCheck = firstEnumeration.IfThen(new BinaryOperationExpression(ruleNodeNullCheckExp, CodeBinaryOperatorType.BooleanAnd, ruleNodeLineContainsExp));
            leftRecursionCheck.Statements.Add(continuePoint.GetGoTo(firstEnumeration.Statements));

            firstEnumeration.Statements.Add(new CommentStatement(
                "Everything else is pretty simple.\r\n" +
                "Create a node, mark the origin point, and the follow point.\r\n" +
                "Add if it doesn't exist."));

            firstEnumeration.Assign(firstNode.GetReference(),
                new CreateNewObjectExpression(
                    streamRulePath.GetTypeReference(),
                    ruleTransition.GetReference().GetProperty("RuleState"),
                    getSourceSetCurrentPath.GetReference(),
                    getSourceSetCurrentPath.GetReference().GetProperty("Rule"),
                    ruleTransition.GetReference().GetProperty("FollowState")));

            var sourcesCheck = firstEnumeration.IfThen(getSourceSetSourcesParam.GetReference().GetMethod("AddFirst").Invoke(firstNode.GetReference()));
            sourcesCheck.CallMethod(newlyAdded.GetReference().GetMethod("Add").Invoke(firstNode.GetReference()));
            sourcesCheck.CallMethod(getSourceSetMethod.GetReference().Invoke(getSourceSetSourcesParam.GetReference(), firstNode.GetReference()));

            firstEnumeration.Statements.Add(continuePoint);

            getSourceSetMethod.Statements.Add(new CommentStatement(
                "Include what comes after the current set of states\r\n" +
                "when there's a terminal edge."));
            getSourceSetMethod.CallMethod(calculateFollowSetMethod.GetReference().Invoke(getSourceSetSourcesParam.GetReference(), newlyAdded.GetReference()));
            #endregion
        }

        private static void BuildMiscAreas(IClassType streamState, IClassType streamStatePath, string fullSetPropertyName, IClassType sourcesBuilderCore, string sourcesParamName, string getSourceSetMethodName, string currentPathParamName, out IMethodMember getSourceSetMethod, out IMethodParameterMember getSourceSetSourcesParam, out IMethodParameterMember getSourceSetCurrentPath, out IMethodMember getSourceSetSimple)
        {

            #region Stream State Method Headers/Misc.
            getSourceSetMethod = streamState.Methods.AddNew(new TypedName(getSourceSetMethodName, typeof(void)));
            getSourceSetSourcesParam = getSourceSetMethod.Parameters.AddNew(new TypedName(sourcesParamName, sourcesBuilderCore));
            getSourceSetCurrentPath = getSourceSetMethod.Parameters.AddNew(new TypedName(currentPathParamName, streamStatePath));
            getSourceSetMethod.AccessLevel = DeclarationAccessLevel.Private;
            getSourceSetMethod.IsStatic = true;

            getSourceSetSimple = streamState.Methods.AddNew(new TypedName(getSourceSetMethodName, typeof(void)));
            var getSourceSetSimpleParam = getSourceSetSimple.Parameters.AddNew(new TypedName(sourcesParamName, sourcesBuilderCore));

            var getSourceSetSimpleEnum = getSourceSetSimple.Enumerate(getSourceSetSimpleParam.GetReference().GetProperty(fullSetPropertyName).GetMethod("ToArray").Invoke(), streamStatePath.GetTypeReference());
            getSourceSetSimpleEnum.CurrentMember.Name = "currentPath";
            getSourceSetSimpleEnum.CallMethod(getSourceSetMethod.GetReference().Invoke(getSourceSetSimpleParam.GetReference(), getSourceSetSimpleEnum.CurrentMember.GetReference()));

            getSourceSetSimple.AccessLevel = DeclarationAccessLevel.Private;
            getSourceSetSimple.IsStatic = true;
            #endregion
        }

        private static IMethodMember BuildCalculateFollowSetMethod(IPropertyMember isTerminalEdgeProperty, IClassType streamState, IClassType streamStatePath, string rulePropertyName, string originalPropertyName, string followPropertyName, string followSetPropertyName, IClassType followInfoCore, string addFollowMethodName, IClassType sourcesBuilderCore, IExternTypeReference listOfStatePath, string sourcesParamName, string calculateFollowSetMethodName)
        {

            #region StreamState CalculateFollowSet Method

            const string newlyAddedParamName = "newlyAdded";
            var calculateFollowSetMethod = streamState.Methods.AddNew(new TypedName(calculateFollowSetMethodName, typeof(void)));
            var calculateFollowSetMethodSources = calculateFollowSetMethod.Parameters.AddNew(new TypedName(sourcesParamName, sourcesBuilderCore));
            var calculateFollowSetNewlyAdded = calculateFollowSetMethod.Parameters.AddNew(new TypedName(newlyAddedParamName, listOfStatePath));
            calculateFollowSetMethod.AccessLevel = DeclarationAccessLevel.Private;
            calculateFollowSetMethod.IsStatic = true;
            var followUpSet = calculateFollowSetMethod.Locals.AddNew(new TypedName("followUpSet", listOfStatePath));
            followUpSet.InitializationExpression = new CreateNewObjectExpression(followUpSet.LocalType);

            followUpSet.AutoDeclare = false;

            var newlyAddedCountCheck = calculateFollowSetMethod.IfThen(new BinaryOperationExpression(calculateFollowSetNewlyAdded.GetReference().GetProperty("Count"), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NumberZero));
            newlyAddedCountCheck.Return();

            calculateFollowSetMethod.DefineLocal(followUpSet);
            calculateFollowSetMethod.Statements.Add(new CommentStatement(
                "This works by checking the currently active path nodes, when one\r\n" +
                "is encountered as a terminal edge, peek above it and use its follow \r\n" +
                "set.\r\n" +
                "-\r\n" +
                "There can be more than one follow for a given path, because \r\n" +
                "left-recursion introduces multiplicity on a rule."));
            var newlyAddedScan = calculateFollowSetMethod.Enumerate(calculateFollowSetNewlyAdded.GetReference(), streamStatePath.GetTypeReference());
            newlyAddedScan.CurrentMember.Name = "possibleTerminalPath";
            var possibleTerminalPath = newlyAddedScan.CurrentMember.GetReference();
            var possibleTerminalCheckExp = possibleTerminalPath.GetProperty(originalPropertyName).GetProperty(isTerminalEdgeProperty.Name);
            var followSetCheckExp = new BinaryOperationExpression(possibleTerminalPath.GetProperty("Rule").GetProperty(followSetPropertyName).GetProperty("Count"), CodeBinaryOperatorType.GreaterThan, PrimitiveExpression.NumberZero);
            var terminalEdgeCheck = newlyAddedScan.IfThen(new BinaryOperationExpression(possibleTerminalCheckExp, CodeBinaryOperatorType.BooleanAnd, followSetCheckExp));

            var followEnumerator = terminalEdgeCheck.Enumerate(possibleTerminalPath.GetProperty("Rule").GetProperty(followSetPropertyName), followInfoCore.GetTypeReference());
            var newNode = followEnumerator.Locals.AddNew(new TypedName("newNode", streamStatePath), new CreateNewObjectExpression(streamStatePath.GetTypeReference(), followEnumerator.CurrentMember.GetReference().GetProperty(followPropertyName), followEnumerator.CurrentMember.GetReference().GetProperty(rulePropertyName)));
            var followEnumCheck = followEnumerator.IfThen(calculateFollowSetMethodSources.GetReference().GetMethod(addFollowMethodName).Invoke(newNode.GetReference()));
            followEnumCheck.CallMethod(followUpSet.GetReference().GetMethod("Add").Invoke(newNode.GetReference()));
            followEnumerator.CurrentMember.Name = "currentFollowData";
            calculateFollowSetMethod.CallMethod(calculateFollowSetMethod.GetReference().Invoke(calculateFollowSetMethodSources.GetReference(), followUpSet.GetReference()));
            #endregion
            return calculateFollowSetMethod;
        }

        private static void BuildLogicConstruct(IProductionRuleEntry startRule, IIntermediateProject result, IGDFile targetFile, INameSpaceDeclaration assemblyChildSpace, IDictionary<IProductionRuleEntry, FlattenedRuleEntry> ruleData, IClassType parserClass, IClassType scannerTokenSetData, TokenFinalDataSet endStateSet, Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> precedenceAssociation, SyntaxActivationInfo sai, ITokenEntry[][] tokenEntries)
        {
            Dictionary<SimpleLanguageBitArray, List<SimpleLanguageState>> stateReductionChart = new Dictionary<SimpleLanguageBitArray, List<SimpleLanguageState>>();
            Dictionary<SimpleLanguageBitArray, int> stateIndexChart = new Dictionary<SimpleLanguageBitArray, int>();
            Dictionary<IProductionRuleEntry, Dictionary<SimpleLanguageState, int>> stateChartIndex = new Dictionary<IProductionRuleEntry, Dictionary<SimpleLanguageState, int>>();
            //var tree = LookAheadCommon.ObtainSyntaxTree(startRule, ruleData);

            #region Basic Concept
            /* -------------------------------------------------\
            |  Build a syntactical representation in code based |
            |  upon the known transitions as they are in the    |
            |  language in the here and now.                    |
            \------------------------------------------------- */
            #endregion
            Dictionary<SimpleLanguageState, IClassType> StateParsers = new Dictionary<SimpleLanguageState, IClassType>();
            Dictionary<IProductionRuleEntry, IClassType> ruleParsers = new Dictionary<IProductionRuleEntry, IClassType>();
            Dictionary<IProductionRuleEntry, Dictionary<IProductionRuleEntry, IConstructorMember>> ruleStateConstructors = new Dictionary<IProductionRuleEntry, Dictionary<IProductionRuleEntry, IConstructorMember>>();
            Dictionary<IProductionRuleEntry, Dictionary<ITokenEntry, IConstructorMember>> tokenStateConstructors = new Dictionary<IProductionRuleEntry, Dictionary<ITokenEntry, IConstructorMember>>();
            //IClassType ruleStructureBase = null;

            var defaultNamespace = result.DefaultNameSpace.Partials.AddNew().ChildSpaces.AddNew("Rules");
            //IFieldMember lookaheadReference = BuildLookAheadTable(startRule, endStateSet, ruleData, parserClass, stateReductionChart, stateIndexChart, stateChartIndex);
            IFieldMember lookAheadStack = parserClass.Fields.AddNew(new TypedName("lookAheadStack", typeof(Stack<int[]>).GetTypeReference()));
            //Console.WriteLine("Look-ahead table size: {0} elements", stateReductionChart.Count);
            var scanner = BuildScanner(targetFile, parserClass, endStateSet, scannerTokenSetData, precedenceAssociation, sai, tokenEntries);
            IFieldMember scannerField = parserClass.Fields.AddNew(new TypedName("currentScanner", scanner));
            #region Rule Base Construction
            //Initialize base interfaces and so on.
            foreach (var rule in ruleData.Keys)
                BuildRuleConstructs(rule, ruleData, targetFile, defaultNamespace, parserClass, scanner, scannerField, lookAheadStack);
            /* *
             * Utilize 'ElementsAreChildren' to associate children
             * to the parent rule, which will (after look-ahead determination), parse the
             * appropriate rule.
             * */
            foreach (var rule in ruleData.Keys)
            {
                var currentRuleData = ruleData[rule];
                if (rule.ElementsAreChildren)
                {
                    IInterfaceType iRule = currentRuleData.RuleInterface;
                    foreach (var expression in rule)
                    {
                        foreach (var ruleElement in expression)
                            if (ruleElement is IRuleReferenceProductionRuleItem)
                            {
                                var rpE = ((IRuleReferenceProductionRuleItem)(ruleElement));
                                ruleData[rpE.Reference].RuleInterface.ImplementsList.Add(iRule);
                            }
                            else if (ruleElement is ITokenReferenceProductionRuleItem)
                            {
                                var trpE = ((ITokenReferenceProductionRuleItem)(ruleElement));
                                var currentSet = endStateSet[trpE.Reference];
                                if (!currentSet.TokenInterface.ImplementsList.Contains(iRule.GetTypeReference()))
                                    currentSet.TokenInterface.ImplementsList.Add(iRule);
                            }
                            else if (ruleElement is ILiteralReferenceProductionRuleItem)
                            {
                            }
                    }
                }
                else
                {
                    /*//
                    Console.WriteLine(rule.Name);
                    foreach (var namedSet in currentRuleData.DataSet.NamedSets)
                    {
                        Console.WriteLine("\t{0}", namedSet.Name);
                        foreach (var item in namedSet)
                        {
                            Console.WriteLine("\t\t{0}", item);
                        }
                    }
                    //*/
                }
            }
            #endregion
            //foreach (var rule in ruleData.Keys)
            //    BuildRuleParser(ruleData[rule], ruleData, targetFile, defaultNamespace, parserClass, scanner, scannerField, lookAheadStack, stateReductionChart, stateChartIndex);
            scannerField.Summary = "The scanner used by the parser which also cross-links with the " + lookAheadStack.GetReference().GetReferenceParticle().BuildCommentBody(ctoCommon) + " to determine the look-ahead.";

            #region Parser Class Construction
            /*//
            foreach (var rule in ruleData.Keys)
            {
                var currentRuleData = ruleData[rule];
                IClassType currentRuleClass = null;
                if (rule == startRule)
                {
                    //So the startRule's parser is joined with the root parser file.
                    ruleStructureBase = assemblyChildSpace.Partials.AddNew().Classes.AddNew(string.Format("{0}SyntaxRootRule", result.Name));
                    currentRuleClass = ruleStructureBase.ParentTarget.Classes.AddNew(string.Format("{0}{1}{2}Parser", targetFile.Options.RulePrefix, rule.Name, targetFile.Options.RuleSuffix));
                }
                else
                    currentRuleClass = assemblyChildSpace.Partials.AddNew().Classes.AddNew(string.Format("{0}{1}{2}Parser", targetFile.Options.RulePrefix, rule.Name, targetFile.Options.RuleSuffix));
                ruleParsers.Add(rule, currentRuleClass);
                //currentRuleClass.BaseType = ruleStructureBase.GetTypeReference();
            }
            //*/
            #endregion
            //ruleStructureBase.IsAbstract = true;
            #region Parser State Construction
            //foreach (var rule in ruleData.Keys)
            //    ruleParsers[rule].BaseType = ruleStructureBase.GetTypeReference();

            //Setup Deterministic parser build here.
            #endregion
        }

        private static T[] GetArrayForm<T>(T item)
        {
            return new T[] { item };
        }

        private static List<T> GetListForm<T>(T[] elements)
        {
            return new List<T>(elements);
        }

        /*//
        private static void CrossRelate(SimpleLanguageRuleState state, List<SimpleLanguageState> stateFlatform, FlattenedRuleEntry data, IDictionary<IProductionRuleEntry, FlattenedRuleEntry> ruleData, IGDFile target, IMethodMember parseMethod, IClassType parserClass, IFieldMember lookAheadStack, IFieldMember scannerField, IClassType scanner, Dictionary<SimpleLanguageBitArray, List<SimpleLanguageState>> stateReductionChart, Dictionary<IProductionRuleEntry, Dictionary<SimpleLanguageState, int>> stateChartIndex, SimpleLanguageState transitionGraph, Dictionary<SimpleLanguageState, Dictionary<SimpleLanguageState.Graph, List<SimpleLanguageState>>> crossReferencePoint)
        {
            var currentRange = state.Transitions.GetCheckRange();
            var relativeGraphs = crossReferencePoint[state];
            
        }
        //*/

        private static void BuildRuleConstructs(IProductionRuleEntry rule, IDictionary<IProductionRuleEntry, FlattenedRuleEntry> ruleData, IGDFile target, INameSpaceDeclaration defaultNamespace, IClassType parserClass, IClassType scanner, IFieldMember scannerField, IFieldMember lookAheadStack)
        {
            var ruleInterface = defaultNamespace.Partials.AddNew().Interfaces.AddNew(string.Format("I{0}{1}{2}", target.Options.RulePrefix, rule.Name, target.Options.RuleSuffix));
            ruleInterface.AccessLevel = DeclarationAccessLevel.Public;
            ruleData[rule].RuleInterface = ruleInterface;
            //var parseMethod = parserClass.Methods.AddNew(new TypedName(string.Format("Parse{0}", rule.Name), ruleInterface));
            //ruleData[rule].ParseMethod = parseMethod;
        }


        private static IClassType BuildScanner(IGDFile target, IClassType parserClass, TokenFinalDataSet endStateSet, IClassType scannerTokenSetData, Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> precedenceAssociation, SyntaxActivationInfo sai, ITokenEntry[][] tokenSets)
        {
            IClassType scanner = parserClass.Partials.AddNew().Classes.AddNew("Scanner");
            IMethodMember nextTokenMethod = scanner.Methods.AddNew(new TypedName("NextToken", typeof(void)));
            IMethodParameterMember nextTokenMethodValid = nextTokenMethod.Parameters.AddNew(new TypedName("valid", sai.data.EnumResults.ResultantType));
            IFieldMember parserReference = scanner.Fields.AddNew(new TypedName("parser", parserClass));
            IConstructorMember constructor = scanner.Constructors.AddNew();
            IConstructorParameterMember ctorParserParam = constructor.Parameters.AddNew(new TypedName("parser", parserClass));
            constructor.Statements.Assign(parserReference.GetReference(), ctorParserParam.GetReference());
            Dictionary<ITokenEntry, IClassType> tokenStateMachines = new Dictionary<ITokenEntry, IClassType>();
            Dictionary<ITokenEntry, IFieldMember> tokenStateMachineFields = new Dictionary<ITokenEntry, IFieldMember>();
            //IMethodMember lookAheadMethod = scanner.Methods.AddNew(new TypedName("LookAhead", typeof(char[])));
            //var lookAheadDistanceParam = lookAheadMethod.Parameters.AddNew(new TypedName("howFar", typeof(long)));
            IClassType scannerDataEntryBase = scannerTokenSetData.Classes["Entry"];
            scannerDataEntryBase.IsAbstract = true;
            IEnumeratorType subsetField = scannerTokenSetData.Enumerators["SubsetField"];
            IPropertyMember subsetIndex = scannerDataEntryBase.Properties["SubsetIndex"];
            IFieldMember subsetIndexField = scannerDataEntryBase.Fields["subsetIndex"];
            MethodMember scannerDataGetTransitionType = (MethodMember)scannerDataEntryBase.Methods.AddNew(new TypedName("GetTransition", sai.data.EnumResults.ResultantType));
            scannerDataGetTransitionType.AccessLevel = DeclarationAccessLevel.Internal;
            scannerDataGetTransitionType.IsFinal = false;
            scannerDataGetTransitionType.IsAbstract = true;
            IClassType scannerEnumEntryBase = scannerTokenSetData.Partials.AddNew().Classes.AddNew("SubsetEntry");
            scannerEnumEntryBase.AccessLevel = DeclarationAccessLevel.Public;
            scannerEnumEntryBase.BaseType = scannerDataEntryBase.GetTypeReference();
            IMethodMember scannerEnumGetTransitionType = scannerEnumEntryBase.Methods.AddNew(new TypedName(scannerDataGetTransitionType.Name, scannerDataGetTransitionType.ReturnType));
            scannerEnumGetTransitionType.AccessLevel = DeclarationAccessLevel.Internal;
            IFieldMember scannerEnumSubsetValueField = scannerEnumEntryBase.Fields.AddNew(new TypedName("subsetValue", typeof(int)));
            IPropertyMember scannerEnumSubsetValueProperty = scannerEnumEntryBase.Properties.AddNew(new TypedName("SubsetValue", typeof(int)), true, false);
            scannerEnumSubsetValueProperty.GetPart.Return(scannerEnumSubsetValueField.GetReference());
            var nullableTransition = sai.data.EnumResults.ResultantType.GetTypeReference();
            nullableTransition.Nullable = true;
            IFieldMember subsetTransitionValue = scannerEnumEntryBase.Fields.AddNew(new TypedName("transition", nullableTransition));
            scannerEnumGetTransitionType.Overrides = true;
            scannerEnumGetTransitionType.IsFinal = false;
            scannerEnumSubsetValueProperty.AccessLevel = DeclarationAccessLevel.Public;
            var subsetTransitionCheck = scannerEnumGetTransitionType.IfThen(BOp((IExpression)subsetTransitionValue.GetReference(), CodeBinaryOperatorType.IdentityEquality, (IExpression)PrimitiveExpression.NullValue));
            var scannerEnumSwitch = subsetTransitionCheck.SelectCase(new CastExpression(new BaseReferenceExpression().GetProperty(subsetIndex.Name), subsetField.GetTypeReference()));
            scannerEnumGetTransitionType.Return(subsetTransitionValue.GetReference().GetProperty("Value"));

            var scannerEnumCtor = scannerEnumEntryBase.Constructors.AddNew();
            var scannerEnumSubsetIndexParam = scannerEnumCtor.Parameters.AddNew(new TypedName("subsetIndex", subsetField.GetTypeReference()));
            var scannerEnumSubsetValueParam = scannerEnumCtor.Parameters.AddNew(new TypedName("subsetValue", typeof(int)));
            scannerEnumCtor.CascadeExpressionsTarget = ConstructorCascadeTarget.Base;
            scannerEnumCtor.CascadeMembers.Add(new CastExpression(scannerEnumSubsetIndexParam.GetReference(), typeof(int).GetTypeReference()));
            scannerEnumCtor.Statements.Assign(scannerEnumSubsetValueField.GetReference(), scannerEnumSubsetValueParam.GetReference());
            scannerEnumCtor.AccessLevel = DeclarationAccessLevel.Public;

            var scannerDataEntryCtor = scannerDataEntryBase.Constructors.AddNew();
            scannerDataEntryCtor.AccessLevel = DeclarationAccessLevel.Public;
            var scannerDataSubsetIndexParam = scannerDataEntryCtor.Parameters.AddNew(new TypedName("subsetIndex", typeof(int)));
            scannerDataEntryCtor.Statements.Assign(subsetIndexField.GetReference(), scannerDataSubsetIndexParam.GetReference());
            List<IClassType> captureEntries = new List<IClassType>();
            foreach (var token in endStateSet.Values)
            {
                if (token.Entry is ITokenEofEntry)
                    continue;
                IFieldMember currentField = scanner.Fields.AddNew(new TypedName(string.Format("stateMachineFor{0}", token.Entry.Name), token.StateMachine));
                tokenStateMachines.Add(token.Entry, token.StateMachine);
                tokenStateMachineFields.Add(token.Entry, currentField);
            }

            Dictionary<ITokenEntry, int> subsetIndices = new Dictionary<ITokenEntry, int>();
            int currentSet = 0;
            foreach (var tokenSet in tokenSets)
            {
                foreach (var token in tokenSet)
                    subsetIndices.Add(token, currentSet);
                currentSet++;
            }

            foreach (var token in endStateSet.Values)
            {
                if (token is TokenCaptureFinalData)
                {
                    var cToken = token as TokenCaptureFinalData;
                    IClassType entryClass = cToken.EntryClass;
                    BuildCaptureEntry(sai, scannerDataEntryBase, subsetIndex, nullableTransition, captureEntries, token, entryClass);
                    var injectMethod = cToken.StateMachine.Methods.AddNew(new TypedName("Inject", typeof(void)));
                    var injectReference = injectMethod.Parameters.AddNew(new TypedName("data", scannerTokenSetData));
                    var isValidEndState = cToken.StateMachine.Properties["IsValidEndState"];
                    var injectCheck = injectMethod.IfThen(isValidEndState.GetReference());
                    injectCheck.CallMethod(injectReference.GetReference().GetMethod(cToken.DataSetAddMethod.Name).Invoke(new ThisReferenceExpression().GetMethod("GetCapture").Invoke(), token.ValidCaseField.GetReference()));
                    injectMethod.AccessLevel = DeclarationAccessLevel.Public;
                }
                else if (token is TokenEnumFinalData)
                {
                    var eToken = token as TokenEnumFinalData;
                    eToken.EntryClass = scannerEnumEntryBase;
                    foreach (var item in eToken.DataSetAddMethods)
                    {
                        var currentEnum = item.Key;
                        var currentMethod = item.Value;
                        var currentSubsetField = eToken.ScannerSetEnumFields[item.Key];
                        currentMethod.CallMethod(new ThisReferenceExpression().GetMethod("Add").Invoke(new CreateNewObjectExpression(scannerEnumEntryBase.GetTypeReference(), currentSubsetField.GetReference(), new CastExpression(currentMethod.Parameters[eToken.Entry.Name].GetReference(), typeof(int).GetTypeReference()))));
                    }
                    foreach (var kvEF in eToken.ScannerSetEnumFields)
                    {
                        var currentEnum = kvEF.Key;
                        var currentField = kvEF.Value;
                        var currentSubsetCase = scannerEnumSwitch.Cases.AddNew(currentField.GetReference());
                        currentSubsetCase.Assign(subsetTransitionValue.GetReference(), new CreateNewObjectExpression(sai.data.EnumResults.ResultantType.GetTypeReference(), new CastExpression(scannerEnumSubsetValueProperty.GetReference(), currentEnum.GetTypeReference())));
                    }
                    //var currentSubsetCase = scannerEnumSwitch.Cases.AddNew(eToken.ScannerSetEnumFields); ;
                    var injectMethod = eToken.StateMachine.Methods.AddNew(new TypedName("Inject", typeof(void)));
                    injectMethod.AccessLevel = DeclarationAccessLevel.Public;
                    var injectReference = injectMethod.Parameters.AddNew(new TypedName("data", scannerTokenSetData));
                    var isValidEndState = eToken.StateMachine.Properties["IsValidEndState"];
                    var injectCheck = injectMethod.IfThen(isValidEndState.GetReference());
                    var injectSwitch = injectCheck.SelectCase(eToken.ExitStateField.GetReference());
                    foreach (var item in from i in eToken.TokenRelationshipInfo
                                         orderby i.Key.Name
                                         select i)
                    {
                        ISwitchStatementCase currentCase = injectSwitch.Cases.AddNew(item.Value.PrimaryExitState.GetReference());
                        currentCase.Statements.Add(new CommentStatement(string.Format("Exit point for {0}.", item.Key.Name)));
                        var ambiguousElement = item.Value as SelfAmbiguousTokenItemData;
                        /* *
                         * On self-ambiguous enumeration series, if the element is
                         * ambiguous between itself and others, insert injection code
                         * for each case, checking to see whether it's in scope.
                         * */
                        if (ambiguousElement != null)
                        {
                            bool first = true;
                            foreach (var ambiguous in from a in ambiguousElement
                                                      orderby (a.SubsetField == ((ITokenItemData)(ambiguousElement)).SubsetField) ? "\0" : a.SubsetField.Name
                                                      select a)
                            {
                                if (first)
                                    first = false;
                                else
                                    currentCase.Statements.Add(new CommentStatement(string.Format("Self ambiguity rule, {0} eclipses {1}.", ((ITokenItemData)(ambiguousElement)).SubsetField.Name, ambiguous.SubsetField.Name)));
                                var currentCaseCondition = currentCase.IfThen(BOp((IExpression)BOp((IExpression)ambiguous.AllowedField.GetReference(), CodeBinaryOperatorType.BitwiseAnd, (IExpression)ambiguous.SubsetField.GetReference()), CodeBinaryOperatorType.IdentityInequality, (IExpression)ambiguous.NoneReference));
                                currentCaseCondition.CallMethod(injectReference.GetReference().GetMethod(string.Format("Add{0}", token.Entry.Name)).Invoke(ambiguous.SubsetField.GetReference()));
                            }
                        }
                        else
                        {
                            var currentCaseCondition = currentCase.IfThen(BOp((IExpression)BOp((IExpression)item.Value.AllowedField.GetReference(), CodeBinaryOperatorType.BitwiseAnd, (IExpression)item.Value.SubsetField.GetReference()), CodeBinaryOperatorType.IdentityInequality, (IExpression)item.Value.NoneReference));
                            currentCaseCondition.CallMethod(injectReference.GetReference().GetMethod(string.Format("Add{0}", token.Entry.Name)).Invoke(item.Value.SubsetField.GetReference()));
                        }
                    }
                }
                else //EOF
                {
                    IClassType entryClass = token.EntryClass;
                    BuildCaptureEntry(sai, scannerDataEntryBase, subsetIndex, nullableTransition, captureEntries, token, entryClass);
                }
            }


            IFieldMember bufferSizeField = scanner.Fields.AddNew(new TypedName("bufferSize", typeof(long)));
            IFieldMember bufferPositionField = scanner.Fields.AddNew(new TypedName("bufferPosition", typeof(long)));
            IFieldMember bufferField = scanner.Fields.AddNew(new TypedName("buffer", typeof(char[])));

            IFieldMember source = scanner.Fields.AddNew(new TypedName("source",typeof(Stream)));
            IFieldMember sourceReader = scanner.Fields.AddNew(new TypedName("sourceReader", typeof(TextReader)));

            IMethodMember doubleBuffer = scanner.Methods.AddNew(new TypedName("DoubleBuffer", typeof(void)));
            IMethodMember lookAhead = scanner.Methods.AddNew(new TypedName("LookAhead", typeof(bool)));
            IMethodParameterMember lookAheadDistance = lookAhead.Parameters.AddNew(new TypedName("distance", typeof(int)));
            IMethodParameterMember lookAheadResult = lookAhead.Parameters.AddNew(new TypedName("result", typeof(char)));
            lookAheadResult.Direction = FieldDirection.Out;

            var doubleBufferCheck = doubleBuffer.IfThen(new BinaryOperationExpression(bufferField.GetReference(),  CodeBinaryOperatorType.IdentityInequality, PrimitiveExpression.NullValue));
            var bufferCopy = doubleBufferCheck.Locals.AddNew(new TypedName("buffer", typeof(char[])));
            bufferCopy.InitializationExpression = new CreateArrayExpression(typeof(char).GetTypeReference(), new BinaryOperationExpression(bufferField.GetReference().GetProperty("LongLength"), CodeBinaryOperatorType.Multiply, new PrimitiveExpression(2)));
            bufferCopy.AutoDeclare = false;
            doubleBufferCheck.DefineLocal(bufferCopy);
            doubleBufferCheck.CallMethod(bufferField.GetReference().GetMethod("CopyTo").Invoke(bufferCopy.GetReference(), PrimitiveExpression.NumberZero));
            doubleBufferCheck.Assign(bufferField.GetReference(), bufferCopy.GetReference());
            doubleBufferCheck.FalseBlock.Assign(bufferField.GetReference(), new CreateArrayExpression(typeof(char).GetTypeReference(), new PrimitiveExpression(1024)));
            doubleBufferCheck.FalseBlock.Assign(bufferSizeField.GetReference(), PrimitiveExpression.NumberZero);

            var lookPointLocal = lookAhead.Locals.AddNew(new TypedName("lookPoint", typeof(long)));
            lookPointLocal.InitializationExpression = new BinaryOperationExpression(bufferPositionField.GetReference(), CodeBinaryOperatorType.Add, new BinaryOperationExpression(lookAheadDistance.GetReference(), CodeBinaryOperatorType.Add, new PrimitiveExpression(1)));
            var lookPointLabel = new LabelStatement("bufferCheck");
            lookAhead.Statements.Add(lookPointLabel);
            var lookPointCheck = lookAhead.IfThen(new BinaryOperationExpression(new BinaryOperationExpression(bufferField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue), CodeBinaryOperatorType.BooleanOr, new BinaryOperationExpression(new BinaryOperationExpression(bufferSizeField.GetReference(), CodeBinaryOperatorType.IdentityEquality, bufferField.GetReference().GetProperty("LongLength")), CodeBinaryOperatorType.BooleanOr, new BinaryOperationExpression(new BinaryOperationExpression(lookPointLocal.GetReference(), CodeBinaryOperatorType.Add, new PrimitiveExpression(1024)), CodeBinaryOperatorType.GreaterThan, bufferField.GetReference().GetProperty("LongLength")))));
            lookPointCheck.CallMethod(doubleBuffer.GetReference().Invoke());
            lookPointCheck.Statements.Add(lookPointLabel.GetGoTo(lookPointCheck.Statements));

            lookAhead.Statements.Add(new CommentStatement("Check to see if the ahead needs read in."));
            var boundaryCheck = lookAhead.IfThen(new BinaryOperationExpression(bufferSizeField.GetReference(), CodeBinaryOperatorType.LessThan, lookPointLocal.GetReference()));
            var boundaryReadLocal = boundaryCheck.Locals.AddNew(new TypedName("bytesToRead", typeof(int)));
            boundaryReadLocal.InitializationExpression = new CastExpression(new BinaryOperationExpression(lookPointLocal.GetReference(),  CodeBinaryOperatorType.Subtract, bufferSizeField.GetReference()), typeof(int).GetTypeReference());
            var boundaryLocLocal = boundaryCheck.Locals.AddNew(new TypedName("readLoc", typeof(int)), new CastExpression(new BinaryOperationExpression(lookPointLocal.GetReference(), CodeBinaryOperatorType.Subtract, boundaryReadLocal.GetReference()), typeof(int).GetTypeReference()));
            var boundaryMinCheck = boundaryCheck.IfThen(new BinaryOperationExpression(boundaryReadLocal.GetReference(), CodeBinaryOperatorType.LessThan, new PrimitiveExpression(1024)));
            boundaryMinCheck.Assign(boundaryReadLocal.GetReference(), new PrimitiveExpression(1024));
            
            var boundaryActualLocal = boundaryCheck.Locals.AddNew(new TypedName("actualBytes", typeof(int)), sourceReader.GetReference().GetMethod("Read").Invoke(bufferField.GetReference(), boundaryLocLocal.GetReference(), boundaryReadLocal.GetReference()));
            boundaryActualLocal.AutoDeclare = false;
            boundaryCheck.DefineLocal(boundaryActualLocal);
            boundaryCheck.Assign(bufferSizeField.GetReference(), new BinaryOperationExpression(bufferSizeField.GetReference(), CodeBinaryOperatorType.Add, boundaryActualLocal.GetReference()));

            var lookAheadEOFCheck = lookAhead.IfThen(new BinaryOperationExpression(lookPointLocal.GetReference(), CodeBinaryOperatorType.GreaterThan, bufferSizeField.GetReference()));
            lookAheadEOFCheck.Assign(lookAheadResult.GetReference(), typeof(char).GetTypeReferenceExpression().GetField("MinValue"));
            lookAheadEOFCheck.Return(PrimitiveExpression.TrueValue);
            lookAheadEOFCheck.FalseBlock.Assign(lookAheadResult.GetReference(), bufferField.GetReference().GetIndex(new BinaryOperationExpression(lookPointLocal.GetReference(), CodeBinaryOperatorType.Subtract, new PrimitiveExpression(1))));
            lookAhead.Return(PrimitiveExpression.FalseValue);

            IConditionStatement currentConditionStatement = null;
            IStatementBlockLocalMember currentCharLocal = nextTokenMethod.Locals.AddNew(new TypedName("char", typeof(char)));
            IStatementBlockLocalMember aheadLength = nextTokenMethod.Locals.AddNew(new TypedName("aheadDistance", typeof(int)), PrimitiveExpression.NumberZero);
            IStatementBlockLocalMember eofLocal = nextTokenMethod.Locals.AddNew(new TypedName("eof", typeof(bool)));
            eofLocal.InitializationExpression = lookAhead.GetReference().Invoke(aheadLength.GetReference(), new DirectionExpression(FieldDirection.Out, currentCharLocal.GetReference()));
            var eofCheck = nextTokenMethod.IfThen(eofLocal.GetReference());
            nextTokenMethod.ReturnType = scannerTokenSetData.GetTypeReference();
            nextTokenMethod.AccessLevel = DeclarationAccessLevel.Public;

            var nextTokenResult = nextTokenMethod.Locals.AddNew(new TypedName("result", scannerTokenSetData), new CreateNewObjectExpression(scannerTokenSetData.GetTypeReference()));
            var eofToken = endStateSet.First(p => p.Key is ITokenEofEntry);
            var eofMethod = ((TokenEofFinalData)(eofToken.Value)).DataSetAddMethod;

            eofCheck.Statements.Add(new CommentStatement("End of file token."));
            eofCheck.CallMethod(nextTokenResult.GetReference().GetMethod(eofMethod.Name).Invoke(PrimitiveExpression.NullValue, eofToken.Value.ValidCaseField.GetReference()));
            eofCheck.Return(nextTokenResult.GetReference());
            Dictionary<IEnumeratorType, IStatementBlockLocalMember> fullActiveLocals = new Dictionary<IEnumeratorType, IStatementBlockLocalMember>();
            Dictionary<IEnumeratorType, IStatementBlockLocalMember> fullTailActiveLocals = new Dictionary<IEnumeratorType, IStatementBlockLocalMember>();
            Dictionary<string, IStatementBlockLocalMember> longestMatchers = new Dictionary<string, IStatementBlockLocalMember>();
            foreach (var kvp in endStateSet.Relationships)
            {
                //nextTokenMethod.Statements.Add(new CommentStatement(kvp.Key.ToString()));
                RegularLanguageBitArray.RangeData currentRangeData = kvp.Key.GetRange();
                IExpression currentCondition = null;
                /* *
                 * Build an in-order version of the condition requirement for the current 
                 * character range overlap.
                 * */
                foreach (var whicher in currentRangeData)
                {
                    IExpression currentSubCondition = null;
                    switch (whicher.Which)
                    {
                        case RegularLanguageBitArray.AOrBSet<char, RegularLanguageBitArray.RangeSet>.ABSelect.A:
                            currentSubCondition = new BinaryOperationExpression(currentCharLocal.GetReference(), CodeBinaryOperatorType.IdentityEquality, new PrimitiveExpression(whicher.A.Value));
                            break;
                        case RegularLanguageBitArray.AOrBSet<char, RegularLanguageBitArray.RangeSet>.ABSelect.B:
                            currentSubCondition = new BinaryOperationExpression(
                                new BinaryOperationExpression(new PrimitiveExpression(whicher.B.Value.Start), CodeBinaryOperatorType.LessThanOrEqual, currentCharLocal.GetReference()),
                                CodeBinaryOperatorType.BooleanAnd,
                                new BinaryOperationExpression(currentCharLocal.GetReference(), CodeBinaryOperatorType.LessThanOrEqual, new PrimitiveExpression(whicher.B.Value.End)));
                            break;
                    }
                    if (currentCondition == null)
                        currentCondition = currentSubCondition;
                    else
                        currentCondition = new BinaryOperationExpression(currentCondition, CodeBinaryOperatorType.BooleanOr, currentSubCondition);
                }
                /* *
                 * String together the conditions.
                 * */
                if (currentConditionStatement == null)
                    currentConditionStatement = nextTokenMethod.IfThen(currentCondition);
                else
                    currentConditionStatement = currentConditionStatement.FalseBlock.IfThen(currentCondition);
                var precedences = GetTokenPrecedences(kvp.Value, precedenceAssociation);
                Dictionary<IEnumeratorType, List<TokenFinalData>> currentActiveData = new Dictionary<IEnumeratorType, List<TokenFinalData>>();
                Dictionary<IEnumeratorType, IPropertyMember> captureSectors = new Dictionary<IEnumeratorType, IPropertyMember>();
                /* *
                 * Breakdown the different active sectors, if there's a capture in the
                 * current set, find its associated token transition field.
                 * *
                 * This will be used to create the bitset for which state machines are active,
                 * the listing will dwindle as it goes on.
                 * *
                 * Data sets which don't have a capture associated to them are initialized to 
                 * a default value: None.
                 * */
                Dictionary<IEnumeratorType, IExpression> captureLimits = new Dictionary<IEnumeratorType, IExpression>();
                foreach (var token in kvp.Value)
                {
                    var tokenData = endStateSet[token];
                    var currentEnum = ((IEnumeratorType)(tokenData.ValidCaseField.FieldType.TypeInstance));
                    List<TokenFinalData> currentTokenSet;
                    if (!currentActiveData.TryGetValue(currentEnum, out currentTokenSet))
                        currentActiveData.Add(currentEnum, (currentTokenSet = new List<TokenFinalData>()));
                    currentTokenSet.Add(tokenData);
                    if (tokenData is TokenCaptureFinalData)
                    {
                        if (!captureSectors.ContainsKey(currentEnum))
                            captureSectors.Add(currentEnum, sai.data.EnumResults.ResultantType.Properties.Values.First(p => p.PropertyType.TypeInstance == currentEnum));
                        if (!captureLimits.ContainsKey(currentEnum))
                            captureLimits.Add(currentEnum, tokenData.ValidCaseField.GetReference());
                        else
                            captureLimits[currentEnum] = new BinaryOperationExpression(captureLimits[currentEnum], CodeBinaryOperatorType.BitwiseOr, tokenData.ValidCaseField.GetReference());
                    }
                }
                /* *
                 * Active locals refers to the active state machines.
                 * *
                 * Active tail locals refers to the state machines which were 
                 * active to begin with, since machines are only reset
                 * when necessary.
                 * */
                Dictionary<IEnumeratorType, IStatementBlockLocalMember> activeLocals = new Dictionary<IEnumeratorType, IStatementBlockLocalMember>();
                Dictionary<IEnumeratorType, IStatementBlockLocalMember> tailActiveLocals = new Dictionary<IEnumeratorType, IStatementBlockLocalMember>();

                foreach (var enumerator in currentActiveData.Keys)
                {
                    if (!fullActiveLocals.ContainsKey(enumerator))
                        fullActiveLocals.Add(enumerator, nextTokenMethod.Locals.AddNew(new TypedName(string.Format("activeMachinesIn{0}", enumerator.Name), enumerator)));
                    var currentLocal = fullActiveLocals[enumerator];//currentConditionStatement.Locals.AddNew(new TypedName(string.Format("activeMachinesIn{0}", enumerator.Name), enumerator));
                    //currentLocal.AutoDeclare = false;
                    //currentConditionStatement.DefineLocal(currentLocal);
                    activeLocals.Add(enumerator, currentLocal);
                    if (!captureSectors.ContainsKey(enumerator))
                        /* *
                         * Due to a bug in OIL on fields not updating
                         * the key when the name changes.
                         * */
                        currentConditionStatement.Assign(currentLocal.GetReference(), enumerator.Fields.Values.First(GetNoneField).GetReference());
                        //currentLocal.InitializationExpression = enumerator.Fields.Values.First(GetNoneField).GetReference();
                    else
                        /* *
                         * Initialize the current state machine group,
                         * limit it to what's valid in the current overlap,
                         * so it breaks out of the loop properly.
                         * */
                        currentConditionStatement.Assign(currentLocal.GetReference(), new BinaryOperationExpression(nextTokenMethodValid.GetReference().GetProperty(captureSectors[enumerator].Name), CodeBinaryOperatorType.BitwiseAnd, captureLimits[enumerator]));
                    //currentLocal.InitializationExpression = new BinaryOperationExpression(nextTokenMethodValid.GetReference().GetProperty(captureSectors[enumerator].Name), CodeBinaryOperatorType.BitwiseAnd, captureLimits[enumerator]);

                    foreach (var tfd in currentActiveData[enumerator])
                    {
                        IExpression activationCheck = null;
                        IExpressionCollection resetSet = new ExpressionCollection();
                        if (tfd is TokenEnumSetFinalData)
                        {
                            var etfd = tfd as TokenEnumSetFinalData;
                            foreach (var en in etfd.ScannerSetEnumFields.Keys)
                            {
                                IExpression activationSubCheck = null;
                                var currentProperty = sai.data.EnumResults.ResultantType.Properties.Values.First(p => p.PropertyType.TypeInstance == en);
                                var currentPropertyExpression = nextTokenMethodValid.GetReference().GetProperty(currentProperty.Name);
                                resetSet.Add(currentPropertyExpression);
                                activationSubCheck = new BinaryOperationExpression(currentPropertyExpression, CodeBinaryOperatorType.IdentityInequality, en.Fields.Values.First(GetNoneField).GetReference());
                                if (activationCheck == null)
                                    activationCheck = activationSubCheck;
                                else
                                    activationCheck = new BinaryOperationExpression(activationCheck, CodeBinaryOperatorType.BooleanOr, activationSubCheck);
                            }
                            var currentActivationCheck = currentConditionStatement.IfThen(activationCheck);
                            currentActivationCheck.CallMethod(tokenStateMachineFields[tfd.Entry].GetReference().GetMethod("Reset").Invoke(resetSet.ToArray()));
                            currentActivationCheck.Assign(currentLocal.GetReference(), new BinaryOperationExpression(currentLocal.GetReference(), CodeBinaryOperatorType.BitwiseOr, etfd.ValidCaseField.GetReference()));
                        }
                        else if (tfd is TokenEnumFinalData)
                        {
                            var etfd = tfd as TokenEnumFinalData;
                            var currentProperty = sai.data.EnumResults.ResultantType.Properties.Values.First(p => p.PropertyType.TypeInstance == etfd.CaseEnumeration);
                            var currentPropertyExpression = nextTokenMethodValid.GetReference().GetProperty(currentProperty.Name);
                            activationCheck = new BinaryOperationExpression(currentPropertyExpression, CodeBinaryOperatorType.IdentityInequality, etfd.CaseEnumeration.Fields.Values.First(GetNoneField).GetReference());
                            resetSet.Add(currentPropertyExpression);
                            var currentActivationCheck = currentConditionStatement.IfThen(activationCheck);
                            currentActivationCheck.CallMethod(tokenStateMachineFields[tfd.Entry].GetReference().GetMethod("Reset").Invoke(resetSet.ToArray()));
                            currentActivationCheck.Assign(currentLocal.GetReference(), new BinaryOperationExpression(currentLocal.GetReference(), CodeBinaryOperatorType.BitwiseOr, etfd.ValidCaseField.GetReference()));
                        }
                        else
                        {
                            var currentActivationCheck = currentConditionStatement.IfThen(new BinaryOperationExpression(new BinaryOperationExpression(nextTokenMethodValid.GetReference().GetProperty(captureSectors[(IEnumeratorType)tfd.ValidCaseField.FieldType.TypeInstance].Name), CodeBinaryOperatorType.BitwiseAnd, tfd.ValidCaseField.GetReference()),CodeBinaryOperatorType.IdentityEquality, tfd.ValidCaseField.GetReference()));
                            currentActivationCheck.CallMethod(tokenStateMachineFields[tfd.Entry].GetReference().GetMethod("Reset").Invoke());
                        }
                    }
                    if (!fullTailActiveLocals.ContainsKey(enumerator))
                        fullTailActiveLocals.Add(enumerator, nextTokenMethod.Locals.AddNew(new TypedName(string.Format("valid{0}", enumerator.Name), enumerator)));
                    var activeLocal = fullTailActiveLocals[enumerator];//currentConditionStatement.Locals.AddNew(new TypedName(string.Format("valid{0}", enumerator.Name), enumerator));
                    currentConditionStatement.Assign(activeLocal.GetReference(), currentLocal.GetReference());
                    //activeLocal.InitializationExpression = currentLocal.GetReference();
                    //activeLocal.AutoDeclare = false;
                    //currentConditionStatement.DefineLocal(activeLocal);
                    tailActiveLocals.Add(enumerator, activeLocal);
                }

                var repeatLabel = new LabelStatement("nextCharacter");
                currentConditionStatement.Statements.Add(repeatLabel);

                int relI = 0;
                Dictionary<ITokenEntry, int> precedenceIndex = new Dictionary<ITokenEntry, int>();
                Dictionary<IStatementBlockLocalMember, IStatementBlockLocalMember> lengthGroupngs = new Dictionary<IStatementBlockLocalMember, IStatementBlockLocalMember>();
                Dictionary<ITokenEntry, IStatementBlockLocalMember> tokenLengthLocals = new Dictionary<ITokenEntry, IStatementBlockLocalMember>();
                foreach (var rel in precedences)
                {
                    //if (precedences.Count > 1)
                    //    currentConditionStatement.Statements.Add(new CommentStatement(string.Format("Token precedence {0} of {1}", ++relI, precedences.Count)));
                    foreach (var token in rel)
                    {
                        var tokenData = endStateSet[token];
                        IEnumeratorType currentEnum = (IEnumeratorType)tokenData.ValidCaseField.FieldType.TypeInstance;
                        IExpression activationCheck = new BinaryOperationExpression(new BinaryOperationExpression(activeLocals[currentEnum].GetReference(), CodeBinaryOperatorType.BitwiseAnd, tokenData.ValidCaseField.GetReference()), CodeBinaryOperatorType.IdentityEquality, tokenData.ValidCaseField.GetReference());
                        IExpression deactivationCheck = new UnaryOperationExpression(UnaryOperations.LogicalNot, tokenStateMachineFields[token].GetReference().GetMethod("Next").Invoke(currentCharLocal.GetReference()));
                        var deactivateCheck = currentConditionStatement.IfThen(new BinaryOperationExpression(activationCheck, CodeBinaryOperatorType.BooleanAnd, deactivationCheck));
                        deactivateCheck.Assign(activeLocals[currentEnum].GetReference(), new BinaryOperationExpression(activeLocals[currentEnum].GetReference(), CodeBinaryOperatorType.BitwiseAnd, new UnaryOperationExpression(UnaryOperations.Compliment, tokenData.ValidCaseField.GetReference())));
                        precedenceIndex.Add(token, ++relI);
                    }
                }
                IExpression deactivationExpression = null;
                foreach (var enumerator in currentActiveData.Keys)
                {
                    var noneField = enumerator.Fields.Values.First(GetNoneField);
                    IExpression currentDeactivationExpression = new BinaryOperationExpression(activeLocals[enumerator].GetReference(), CodeBinaryOperatorType.IdentityInequality, noneField.GetReference());
                    if (deactivationExpression == null)
                        deactivationExpression = currentDeactivationExpression;
                    else
                        deactivationExpression = new BinaryOperationExpression(deactivationExpression, CodeBinaryOperatorType.BitwiseOr, currentDeactivationExpression);
                }
                var fullDeactivationCheck = currentConditionStatement.IfThen(deactivationExpression);
                fullDeactivationCheck.Increment(aheadLength.GetReference(), CrementType.Postfix);
                var subDeactivationCheck = fullDeactivationCheck.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, lookAhead.GetReference().Invoke(aheadLength.GetReference(), new DirectionExpression(FieldDirection.Out, currentCharLocal.GetReference()))));
                subDeactivationCheck.Statements.Add(repeatLabel.GetGoTo(currentConditionStatement.Statements));
                relI = 0;
                foreach (var rel in precedences)
                {
                    string currentLengthName = null;
                    if (precedences.Count == 1)
                        currentLengthName = "longest";
                    else
                        currentLengthName = string.Format("group{0}Longest", ++relI);
                    if (!longestMatchers.ContainsKey(currentLengthName))
                        longestMatchers.Add(currentLengthName, nextTokenMethod.Locals.AddNew(new TypedName(currentLengthName, typeof(int))));
                    IStatementBlockLocalMember currentLengthLocal = longestMatchers[currentLengthName];
                    //currentLengthLocal.AutoDeclare = false;
                    //currentConditionStatement.DefineLocal(currentLengthLocal);
                    currentConditionStatement.Assign(currentLengthLocal.GetReference(), PrimitiveExpression.NumberZero);

                    IStatementBlockLocalMember last = lengthGroupngs.Count == 0 ? null : lengthGroupngs.Keys.Last();
                    lengthGroupngs.Add(currentLengthLocal, last);
                    foreach (var token in rel)
                    {
                        var tokenData = endStateSet[token];
                        tokenLengthLocals.Add(token, currentLengthLocal);
                        var enumerator = (IEnumeratorType)tokenData.ValidCaseField.FieldType.TypeInstance;
                        var noneField = enumerator.Fields.Values.First(GetNoneField);
                        IExpression currentActiveCheck = new BinaryOperationExpression(new BinaryOperationExpression(tailActiveLocals[enumerator].GetReference(), CodeBinaryOperatorType.BitwiseAnd, tokenData.ValidCaseField.GetReference()), CodeBinaryOperatorType.IdentityInequality, noneField.GetReference());
                        IExpression currentValidCheck = tokenStateMachineFields[token].GetReference().GetProperty("IsValidEndState");

                        IExpression currentLengthCheckExpression = new BinaryOperationExpression(currentLengthLocal.GetReference(), CodeBinaryOperatorType.LessThan, tokenStateMachineFields[token].GetReference().GetProperty("BytesConsumed"));
                        var currentLengthCheck = currentConditionStatement.IfThen(new BinaryOperationExpression(new BinaryOperationExpression(currentActiveCheck, CodeBinaryOperatorType.BooleanAnd, currentValidCheck), CodeBinaryOperatorType.BooleanAnd, currentLengthCheckExpression));
                        currentLengthCheck.Assign(currentLengthLocal.GetReference(), tokenStateMachineFields[token].GetReference().GetProperty("BytesConsumed"));
                    }
                }
                IStatementBlock currentPrecedenceCondition = currentConditionStatement.Statements;
                Dictionary<int, ILabelStatement> precedenceLabels = new Dictionary<int, ILabelStatement>();
                foreach (var rel in precedences.Reverse<List<ITokenEntry>>())
                {
                    string currentLengthName = null;
                    //precedenceLabels.Add(relI, new LabelStatement(string.Format("Precedence{0}", relI)));
                    if (precedences.Count == 1)
                        currentLengthName = "longest";
                    else
                        currentLengthName = string.Format("group{0}Longest", relI--);
                    IStatementBlockLocalMember currentLengthLocal = longestMatchers[currentLengthName];
                    IStatementBlockLocalMember last = lengthGroupngs[currentLengthLocal];
                    if (precedences.Count > 1)
                    {
                        IExpression currentLengthCheck = null;
                        while (last != null)
                        {
                            IExpression currentLengthSubCheck = new BinaryOperationExpression(currentLengthLocal.GetReference(), CodeBinaryOperatorType.GreaterThan, last.GetReference());
                            if (currentLengthCheck == null)
                                currentLengthCheck = currentLengthSubCheck;
                            else
                                currentLengthCheck = new BinaryOperationExpression(currentLengthCheck,  CodeBinaryOperatorType.BooleanAnd, currentLengthSubCheck);
                            last = lengthGroupngs[last];
                        }
                        if (currentLengthCheck != null)
                            currentPrecedenceCondition = currentPrecedenceCondition.IfThen(currentLengthCheck).Statements;
                    }
                    if (precedences.Count > 1 && relI > 0)
                            currentPrecedenceCondition.Add(new CommentStatement("Current precedence must be higher than all others."));
                    foreach (var token in rel)
                    {
                        var tokenData = endStateSet[token];
                        var enumerator = (IEnumeratorType)tokenData.ValidCaseField.FieldType.TypeInstance;
                        var noneField = enumerator.Fields.Values.First(GetNoneField);
                        IExpression currentActiveCheck = new BinaryOperationExpression(new BinaryOperationExpression(tailActiveLocals[enumerator].GetReference(), CodeBinaryOperatorType.BitwiseAnd, tokenData.ValidCaseField.GetReference()), CodeBinaryOperatorType.IdentityInequality, noneField.GetReference());
                        IExpression currentValidCheck = tokenStateMachineFields[token].GetReference().GetProperty("IsValidEndState");
                        IExpression currentLengthCheckExpression = new BinaryOperationExpression(currentLengthLocal.GetReference(), CodeBinaryOperatorType.IdentityEquality, tokenStateMachineFields[token].GetReference().GetProperty("BytesConsumed"));
                        var currentLengthCheck = currentPrecedenceCondition.IfThen(new BinaryOperationExpression(new BinaryOperationExpression(currentActiveCheck, CodeBinaryOperatorType.BooleanAnd, currentValidCheck), CodeBinaryOperatorType.BooleanAnd, currentLengthCheckExpression));
                        currentLengthCheck.CallMethod(tokenStateMachineFields[token].GetReference().GetMethod("Inject").Invoke(nextTokenResult.GetReference()));
                    }
                    if (precedences.Count > 1)
                        currentPrecedenceCondition = ((IConditionStatement)currentPrecedenceCondition.Parent).FalseBlock;
                }
                //var bSt = new BlockStatement(nextTokenMethod.Statements);
                //int relI = 0;
                //foreach (var rel in precedences)
                //{
                //    if (precedences.Count > 1)
                //        bSt.Statements.Add(new CommentStatement(string.Format("Token precedence {0} of {1}", ++relI, precedences.Count)));
                //    foreach (var token in rel)
                //        bSt.Statements.Add(new CommentStatement(token.Name));
                //}
                //nextTokenMethod.Statements.Add(bSt);
            }
            nextTokenMethod.Return(nextTokenResult.GetReference());
            return scanner;
        }


        private static bool GetNoneField(IFieldMember t)
        {
            return t.Name == "None";
        }

        private static void BuildCaptureEntry(SyntaxActivationInfo sai, IClassType scannerDataEntryBase, IPropertyMember subsetIndex, IDeclaredTypeReference<CodeTypeDeclaration> nullableTransition, List<IClassType> captureEntries, TokenFinalData token, IClassType entryClass)
        {
            if (!captureEntries.Contains(entryClass))
            {
                var currentTokenEntry = entryClass;
                var captureEntryCtor = currentTokenEntry.Constructors.AddNew();
                var captureEntrySubsetID = captureEntryCtor.Parameters.AddNew(new TypedName("tokenId", token.ValidCaseField.FieldType));
                var captureEntryParam = captureEntryCtor.Parameters.AddNew(new TypedName("capture", typeof(string)));
                captureEntryCtor.CascadeMembers.Add(new CastExpression(captureEntrySubsetID.GetReference(), typeof(int).GetTypeReference()));
                if (currentTokenEntry.BaseType.TypeInstance != scannerDataEntryBase)
                    captureEntryCtor.CascadeMembers.Add(captureEntryParam.GetReference());
                else
                {
                    var captureField = currentTokenEntry.Fields["capture"];
                    captureEntryCtor.Statements.Assign(captureField.GetReference(), captureEntryParam.GetReference());
                }
                captureEntryCtor.AccessLevel = DeclarationAccessLevel.Public;
                captureEntryCtor.CascadeExpressionsTarget = ConstructorCascadeTarget.Base;
                captureEntries.Add(entryClass);
                var transition = currentTokenEntry.Fields.AddNew(new TypedName("transition", nullableTransition));
                var getTransitionType = currentTokenEntry.Methods.AddNew(new TypedName("GetTransition", sai.data.EnumResults.ResultantType));
                getTransitionType.Overrides = true;
                getTransitionType.IsFinal = false;
                getTransitionType.AccessLevel = DeclarationAccessLevel.Internal;
                var getTTCheck = getTransitionType.IfThen(new BinaryOperationExpression(transition.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
                getTTCheck.Assign(transition.GetReference(), new CreateNewObjectExpression(sai.data.EnumResults.ResultantType.GetTypeReference(), new CastExpression(new BaseReferenceExpression().GetProperty(subsetIndex.Name), token.ValidCaseField.FieldType)));
                getTransitionType.Return(transition.GetReference().GetProperty("Value"));
            }
        }

        private static void BuildChartFor(FlattenedRuleEntry contextEntry, IProductionRuleEntry context, bool getFullRange, Dictionary<SimpleLanguageBitArray, List<SimpleLanguageState>> stateReductionChart, Dictionary<SimpleLanguageBitArray, int> stateIndexChart, Dictionary<IProductionRuleEntry, Dictionary<SimpleLanguageState, int>> stateChartIndex, SimpleLanguageState transitionGraph)
        {
            var flatFormTransitionGraph = transitionGraph.GetFlatform();
            flatFormTransitionGraph.Add(transitionGraph);
            foreach (var state in flatFormTransitionGraph.Distinct())
            {
                if (state.OutTransitions.Count == 0)
                    continue;
                SimpleLanguageBitArray stateTransitionRange = null;
                if (getFullRange)
                {
                    var tt = state.ObtainMergedTransitions(contextEntry);
                    stateTransitionRange = tt.GetCheckRange();
                }
                else
                    stateTransitionRange = new SimpleLanguageBitArray(state.OutTransitions.GetCheckRange());
                stateTransitionRange.DisposeRuleData();
                if (!stateReductionChart.ContainsKey(stateTransitionRange))
                {
                    stateIndexChart.Add(stateTransitionRange, stateReductionChart.Count);
                    stateReductionChart.Add(stateTransitionRange, new List<SimpleLanguageState>());
                }
                if (!stateChartIndex.ContainsKey(context))
                     stateChartIndex.Add(context, new Dictionary<SimpleLanguageState, int>());
                stateChartIndex[context].Add(state, stateIndexChart[stateTransitionRange]);
                stateReductionChart[stateTransitionRange].Add(state);
            }
        }

        /*//
        private static void DivulgeDepths(RuleDataGroupNode dataNode)
        {
            DivulgeDepths(dataNode, 1);
        }

        private static void DivulgeDepths(RuleDataGroupNode dataNode, int depth)
        {
            int lN = 0;

            foreach (var item in dataNode)
                lN = Math.Max(item.Key.Length, lN);

            foreach (var item in dataNode)
            {
                int cN = lN - item.Key.Length,
                    cT = (int)Math.Floor(((double)cN / 8));
                if (item.Value.Count == 1)
                {
                    for (int i = 0; i < depth; i++)
                        Console.Write("\t");
                    Console.Write("\t{0}", item.Key);
                    for (int j = 0; j < cT; j++)
                        Console.Write("\t");
                    Console.Write("\t{0}\t", item.Value[0].MoreThanOne, item.Value[0].Type);
                    Console.Write(item.Value[0].Type);
                    if (item.Value[0].Type == RuleDataNodeType.GroupNode)
                    {
                        Console.WriteLine();
                        DivulgeDepths((RuleDataGroupNode)item.Value[0], depth + 1);
                    }
                    else if (item.Value[0].Type == RuleDataNodeType.RuleDataNode)
                        Console.Write(" ({0})", ((RuleDataRuleNode)(item.Value[0])).Source.Reference.Name);
                    else if (item.Value[0].Type == RuleDataNodeType.EnumDataNode)
                        Console.Write(" ({0}: {1})", ((RuleDataEnumTokenNode)(item.Value[0])).finalData.Entry.Name, ((RuleDataEnumTokenNode)(item.Value[0])).CoveredEntities.ToString());
                    Console.WriteLine();
                }
                else
                {
                    for (int i = 0; i < depth; i++)
                        Console.Write("\t");
                    Console.WriteLine("\t{0}", item.Key);
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        for (int j = 0; j < depth; j++)
                            Console.Write("\t");
                        Console.Write("\t\t");
                        for (int j = 0; j < cT; j++)
                            Console.Write("\t");
                        Console.Write(item.Value[i].MoreThanOne);
                        Console.Write("\t");
                        Console.Write(item.Value[i].Type);
                        if (item.Value[i].Type == RuleDataNodeType.GroupNode)
                        {
                            Console.WriteLine();
                            DivulgeDepths((RuleDataGroupNode)item.Value[i], depth + 1);
                        }
                        else if (item.Value[i].Type == RuleDataNodeType.RuleDataNode)
                            Console.Write(" ({0})",((RuleDataRuleNode)(item.Value[i])).Source.Reference.Name);
                        else if (item.Value[i].Type == RuleDataNodeType.EnumDataNode)
                            Console.Write(" ({0}: {1})", ((RuleDataEnumTokenNode)(item.Value[i])).finalData.Entry.Name,((RuleDataEnumTokenNode)(item.Value[i])).CoveredEntities.ToString());
                        Console.WriteLine();
                    }
                }
            }
        }
        //*/

        private static                          TokenFinalDataSet BuildTokens
                                                       (IGDFile       target,
                                          INameSpaceDeclaration       assemblyChildSpace,
                                                         string       stateMachineFormat,
                   IDictionary<ITokenEntry, FlattenedTokenEntry>      tokenData,
                                           IIntermediateProject       result,
                                                     IClassType       parser,
                                                 ref IClassType       scannerTokenSetData,
                                                CharStreamClass       captureType,
Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>>     precedences,
                                                out ITokenEntry  [][] outTokenEntries)
        {
            bool first = true;
            var defaultNamepace = result.DefaultNameSpace.Partials.AddNew().ChildSpaces.AddNew("Tokens");
            RLBitArrayToTokenEntrySetLookup relationships = new RLBitArrayToTokenEntrySetLookup();
            scannerTokenSetData = defaultNamepace.Partials.AddNew().Classes.AddNew(string.Format("{0}ScanData", target.Options.AssemblyName));
            //IStructType validDataType = defaultNamepace.Partials.AddNew().Structures.AddNew(string.Format("Valid{0}Data", target.Options.AssemblyName));
            TokenEntryToFieldLookup EntryToCaseFieldList = new TokenEntryToFieldLookup();
            //TokenEntryToFieldLookup EntryToValidFieldList = new TokenEntryToFieldLookup();
            //TokenEntryToConstructorLookup tokenConstructors = new TokenEntryToConstructorLookup();
            TokenEntryToEnumLookup EntryToCaseTypeList = new TokenEntryToEnumLookup();
            TokenEntryToStateTypeLookup tokenTypes = new TokenEntryToStateTypeLookup();
            Dictionary<ITokenEntry, IPropertySignatureMember> EntryToPropertyCaseLookup = new Dictionary<ITokenEntry, IPropertySignatureMember>();
            EnumToConstructorLookup caseConstructors = new EnumToConstructorLookup();
            TokenFinalDataSet dataSet = new TokenFinalDataSet();//validDataType);
            IInterfaceType tokenBaseInterface = assemblyChildSpace.Partials.AddNew().Interfaces.AddNew(string.Format("I{0}Token", target.Options.AssemblyName));
            IClassType tokenBaseClass = assemblyChildSpace.Partials.AddNew().Classes.AddNew(string.Format("{0}TokenBase", target.Options.AssemblyName));
            Dictionary<ITokenEntry, IClassType> tokenBaseTypeLookup = new Dictionary<ITokenEntry, IClassType>();
            dataSet.BaseInterface = tokenBaseInterface;
            tokenBaseClass.IsAbstract = true;
            tokenBaseClass.AccessLevel = DeclarationAccessLevel.Internal;
            tokenBaseInterface.AccessLevel = DeclarationAccessLevel.Public;
            tokenBaseClass.ImplementsList.Add(tokenBaseInterface);
            //IIntermediateModule tokenModule = result.Modules.AddNew("TokenModule");
            int numSets = (int)Math.Ceiling(((double)(tokenData.Count)) / SetCommon.MinimalSetData.setSize);
            IEnumeratorType scannerTokenSetDataSetIndices = scannerTokenSetData.Partials.AddNew().Enumerators.AddNew("SubsetField");
            scannerTokenSetDataSetIndices.AccessLevel = DeclarationAccessLevel.Public;

                               ITokenEntry []   tokenEntries          = tokenData.Keys.ToArray();
                               ITokenEntry [][] tokenEntrySets        = new ITokenEntry[numSets][];   
                           IEnumeratorType []   caseSelector          = new IEnumeratorType[numSets];
                                IClassType []   captureTokenBases     = new IClassType[numSets];
                              IFieldMember []   noneFields            = new IFieldMember[numSets];

                             IMethodMember []   captureAdders         = new IMethodMember[numSets];
 Dictionary<IEnumeratorType, IMethodMember>[]   setAdders             = new Dictionary<IEnumeratorType, IMethodMember>[numSets];
               Dictionary<ITokenEntry, int>     setIndices            = new Dictionary<ITokenEntry,int>();
                                IClassType []   scannerCaptureEntries = new IClassType[numSets];
                                IClassType      scannerEntry          = scannerTokenSetData.Partials.AddNew().Classes.AddNew("Entry");
                                IClassType      scannerCaptureBase    = scannerTokenSetData.Partials.AddNew().Classes.AddNew("CaptureBase");

            scannerEntry.AccessLevel = DeclarationAccessLevel.Public;
            scannerCaptureBase.BaseType = scannerEntry.GetTypeReference();
            var captureField = scannerCaptureBase.Fields.AddNew(new TypedName("capture", typeof(string)));
            var capturePropety = scannerCaptureBase.Properties.AddNew(new TypedName("Capture", typeof(string)), true, false);
            capturePropety.GetPart.Return(captureField.GetReference());
            scannerCaptureBase.AccessLevel = DeclarationAccessLevel.Public;
            var subsetIndexField = scannerEntry.Fields.AddNew(new TypedName("subsetIndex", typeof(int)));
            var subsetIndexProperty = scannerEntry.Properties.AddNew(new TypedName("SubsetIndex", typeof(int)), true, false);
            subsetIndexProperty.AccessLevel = DeclarationAccessLevel.Public;
            subsetIndexProperty.GetPart.Return(subsetIndexField.GetReference());
            scannerTokenSetData.BaseType = typeof(List<>).GetTypeReference(new TypeReferenceCollection(scannerEntry.GetTypeReference()));
            scannerTokenSetData.AccessLevel = DeclarationAccessLevel.Public;

            if (numSets > 1)
            {
                var scannerCaptureBaseCtor = scannerCaptureBase.Constructors.AddNew();
                scannerCaptureBaseCtor.AccessLevel = DeclarationAccessLevel.Protected;
                scannerCaptureBase.IsAbstract = true;
                var scannerCaptureBaseTokenIdParam = scannerCaptureBaseCtor.Parameters.AddNew(new TypedName("tokenId", typeof(int)));
                var scannerCaptureBaseParam = scannerCaptureBaseCtor.Parameters.AddNew(new TypedName("capture", typeof(string)));
                scannerCaptureBaseCtor.Statements.Assign(captureField.GetReference(), scannerCaptureBaseParam.GetReference());
                scannerCaptureBaseCtor.CascadeExpressionsTarget = ConstructorCascadeTarget.Base;
                scannerCaptureBaseCtor.CascadeMembers.Add(scannerCaptureBaseTokenIdParam.GetReference());
            }

            #region Standard subset constuction.
            /* *
             * Standard subset constuction.
             * */

            for (int i = 0; i < numSets; i++)
            {

                setAdders[i] = new Dictionary<IEnumeratorType, IMethodMember>();
                int setLength = 0;
                if (tokenData.Count - (i * SetCommon.MinimalSetData.setSize) > SetCommon.MinimalSetData.setSize)
                    setLength = (int)SetCommon.MinimalSetData.setSize;
                else
                    setLength = (int)(tokenData.Count - (i * SetCommon.MinimalSetData.setSize));
                IEnumeratorType currentCaseSelector = null;
                if (numSets > 1)
                    currentCaseSelector = defaultNamepace.Partials.AddNew().Enumerators.AddNew(string.Format("{0}TokensInSet{1}", target.Options.AssemblyName, i + 1));
                else
                    currentCaseSelector = defaultNamepace.Partials.AddNew().Enumerators.AddNew(string.Format("{0}Tokens", target.Options.AssemblyName));
                var noneField = currentCaseSelector.Fields.AddNew("None", 0);
                noneFields[i] = noneField;
                caseSelector[i] = currentCaseSelector;
                //IFieldMember caseField = null;
                IPropertyMember bcCaseField = null;
                IPropertyMember currentBaseProperty = null;
                IClassType currentCaptureTokenBase = null;
                /* *
                 * Build the core output token kinds based upon the number of sets.
                 * */
                IPropertySignatureMember currentBaseInterfaceSetProperty = null;
                if (numSets > 1)
                {
                    var scannerCaptureEntry = scannerCaptureEntries[i] = scannerTokenSetData.Partials.AddNew().Classes.AddNew(string.Format("CaptureEntry{0}", (i + 1)));
                    scannerCaptureEntry.BaseType = scannerCaptureBase.GetTypeReference();

                    //caseField = validDataType.Fields.AddNew(new TypedName(string.Format("ValidTokensInSet{0}", i + 1), currentCaseSelector));
                    currentBaseInterfaceSetProperty = tokenBaseInterface.Properties.AddNew(new TypedName(string.Format("Set{0}TokenID", i + 1), currentCaseSelector), true, false);
                    bcCaseField = tokenBaseClass.Properties.AddNew(new TypedName(string.Format("Set{0}TokenID", i + 1), currentCaseSelector), true, false);
                    captureTokenBases[i] = assemblyChildSpace.Partials.AddNew().Classes.AddNew(string.Format("{0}CapturingTokenForSet{1}", target.Options.AssemblyName, i + 1));
                    currentCaptureTokenBase = captureTokenBases[i];
                    var scannerTokenDataAddCaptureMethod = captureAdders[i] = scannerTokenSetData.Methods.AddNew(new TypedName("AddCapture", typeof(void)));
                    scannerTokenDataAddCaptureMethod.AccessLevel = DeclarationAccessLevel.Public;
                    var addCaptureStringParam = scannerTokenDataAddCaptureMethod.Parameters.AddNew(new TypedName("capture", typeof(string)));
                    var addCaptureSetParam = scannerTokenDataAddCaptureMethod.Parameters.AddNew(new TypedName("captureSet", currentCaseSelector));
                    //var currentCaptureCacheField = captureCacheFields[i] = scannerTokenSetData.Fields.AddNew(new TypedName(string.Format("CapturesFromSet{0}", i + 1), typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(currentCaseSelector.GetTypeReference(), typeof(string).GetTypeReference()))));
                    //var addCaptureCheck = scannerTokenDataAddCaptureMethod.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, currentCaptureCacheField.GetReference().GetMethod("ContainsKey").Invoke(addCaptureSetParam.GetReference())));
                    //addCaptureCheck.CallMethod(currentCaptureCacheField.GetReference().GetMethod("Add").Invoke(addCaptureSetParam.GetReference(), addCaptureStringParam.GetReference()));
                    scannerTokenDataAddCaptureMethod.CallMethod(new ThisReferenceExpression().GetMethod("Add").Invoke(new CreateNewObjectExpression(scannerCaptureEntry.GetTypeReference(), addCaptureSetParam.GetReference(), addCaptureStringParam.GetReference())));
                    for (int j = 0; j < i; j++)
                    {
                        IPropertyMember currentSetProperty = null;
                        /* *
                         * Later, if there are no captures in the current set, the 
                         * capture for that set is removed and nulled.
                         * */
                        if (captureTokenBases[j] != null)
                        {
                            currentSetProperty = captureTokenBases[j].Properties.AddNew(new TypedName(string.Format("Set{0}TokenID", i + 1), currentCaseSelector), true, false);
                            currentSetProperty.AccessLevel = DeclarationAccessLevel.Public;
                            currentSetProperty.Overrides = true;
                            currentSetProperty.GetPart.Return(noneField.GetReference());
                            currentSetProperty.Summary = string.Format("Returns which token the current capture is in on set {0}.", i + 1);
                            currentSetProperty.Remarks = string.Format("Returns {0}.", noneField.GetReference().GetReferenceParticle().BuildCommentBody(ctoCommon));
                        }
                        currentSetProperty = currentCaptureTokenBase.Properties.AddNew(new TypedName(string.Format("Set{0}TokenID", j + 1), caseSelector[j]), true, false);
                        currentSetProperty.AccessLevel = DeclarationAccessLevel.Public;
                        currentSetProperty.Overrides = true;
                        currentSetProperty.GetPart.Return(noneFields[j].GetReference());
                        currentSetProperty.Summary = string.Format("Returns which token the current capture is in on set {0}.", j + 1);
                        if (captureTokenBases[j] != null)
                            currentSetProperty.Remarks = string.Format("Returns {0}.", noneFields[j].GetReference().GetReferenceParticle().BuildCommentBody(ctoCommon));
                        else
                            currentSetProperty.Remarks = string.Format("<para>Returns {0}.</para><para>No tokens in set {1} are captures.</para>", noneFields[j].GetReference().GetReferenceParticle().BuildCommentBody(ctoCommon), j + 1);
                    }
                    currentBaseProperty = currentCaptureTokenBase.Properties.AddNew(new TypedName(string.Format("Set{0}TokenID", i + 1), currentCaseSelector), true, false);
                    currentBaseProperty.Summary = "Returns which token the current capture is in the active set.";
                }
                else
                {
                    scannerCaptureBase.Name = "CaptureEntry";
                    scannerCaptureEntries[i] = scannerCaptureBase;
                    //caseField = validDataType.Fields.AddNew(new TypedName("ValidTokens", currentCaseSelector));
                    currentBaseInterfaceSetProperty = tokenBaseInterface.Properties.AddNew(new TypedName("TokenID", currentCaseSelector), true, false);
                    bcCaseField = tokenBaseClass.Properties.AddNew(new TypedName("TokenID", currentCaseSelector), true, false);
                    currentCaptureTokenBase = assemblyChildSpace.Partials.AddNew().Classes.AddNew(string.Format("{0}CapturingToken", target.Options.AssemblyName));
                    captureTokenBases[i] = currentCaptureTokenBase;
                    currentBaseProperty = currentCaptureTokenBase.Properties.AddNew(new TypedName("TokenID", currentCaseSelector), true, false);
                    currentBaseProperty.Summary = "Returns which token the current capture is.";
                    var scannerTokenDataAddCaptureMethod = captureAdders[i] = scannerTokenSetData.Methods.AddNew(new TypedName("AddCapture", typeof(void)));
                    var addCaptureStringParam = scannerTokenDataAddCaptureMethod.Parameters.AddNew(new TypedName("capture", typeof(string)));
                    var addCaptureSetParam = scannerTokenDataAddCaptureMethod.Parameters.AddNew(new TypedName("captureSet", currentCaseSelector));
                    scannerTokenDataAddCaptureMethod.CallMethod(new ThisReferenceExpression().GetMethod("Add").Invoke(new CreateNewObjectExpression(scannerCaptureBase.GetTypeReference(), addCaptureSetParam.GetReference(), addCaptureStringParam.GetReference())));
                    scannerTokenDataAddCaptureMethod.AccessLevel = DeclarationAccessLevel.Public;
                    //var currentCaptureCacheField = captureCacheFields[i] = scannerTokenSetData.Fields.AddNew(new TypedName("Captures", typeof(Dictionary<,>).GetTypeReference(new TypeReferenceCollection(currentCaseSelector.GetTypeReference(), typeof(string).GetTypeReference()))));
                    //var addCaptureCheck = scannerTokenDataAddCaptureMethod.IfThen(new UnaryOperationExpression(UnaryOperations.LogicalNot, currentCaptureCacheField.GetReference().GetMethod("ContainsKey").Invoke(addCaptureSetParam.GetReference())));
                    //addCaptureCheck.CallMethod(currentCaptureCacheField.GetReference().GetMethod("Add").Invoke(addCaptureSetParam.GetReference(), addCaptureStringParam.GetReference()));
                }
                var captureToStringMethod = currentCaptureTokenBase.Methods.AddNew(new TypedName("ToString", typeof(string)));
                
                currentBaseProperty.AccessLevel = DeclarationAccessLevel.Public;
                currentBaseProperty.Overrides = true;
                currentCaptureTokenBase.AccessLevel = DeclarationAccessLevel.Internal;
                currentCaptureTokenBase.IsSealed = true;
                currentCaptureTokenBase.BaseType = tokenBaseClass.GetTypeReference();
                var captureTokenCaptureProperty = currentCaptureTokenBase.Properties.AddNew(new TypedName("Capture", typeof(string)), true, false);
                bcCaseField.AccessLevel = DeclarationAccessLevel.Public;
                var captureTokenSelector = currentCaptureTokenBase.Fields.AddNew(new TypedName("selector", currentCaseSelector));
                var captureTokenCaptureField = currentCaptureTokenBase.Fields.AddNew(new TypedName("capture", typeof(string)));
                var captureTokenCtor = currentCaptureTokenBase.Constructors.AddNew();
                captureToStringMethod.Return(typeof(string).GetTypeReferenceExpression().GetMethod("Format").Invoke(new PrimitiveExpression("{0} ({1})"), currentBaseProperty.GetReference().GetMethod("ToString").Invoke(), captureTokenCaptureProperty.GetReference()));
                captureToStringMethod.AccessLevel = DeclarationAccessLevel.Public;
                captureToStringMethod.Overrides = true;
                captureToStringMethod.IsFinal = false;
                currentBaseProperty.GetPart.Return(captureTokenSelector.GetReference());
                captureTokenCaptureProperty.AccessLevel = DeclarationAccessLevel.Public;
                captureTokenCaptureProperty.GetPart.Return(captureTokenCaptureField.GetReference());
                captureTokenCtor.AccessLevel = DeclarationAccessLevel.Public;
                var captureTokenCtorSelectorParam = captureTokenCtor.Parameters.AddNew(new TypedName("tokenType", currentCaseSelector));
                var captureTokenCtorCaptureParam = captureTokenCtor.Parameters.AddNew(new TypedName("capture", typeof(string)));
                captureTokenCtor.Statements.Assign(captureTokenSelector.GetReference(), captureTokenCtorSelectorParam.GetReference());
                captureTokenCtor.Statements.Assign(captureTokenCaptureField.GetReference(), captureTokenCtorCaptureParam.GetReference());
                bcCaseField.IsAbstract = true;
                //IConstructorParameterMember currentFullCtorParam = null;
                //if (numSets > 1)
                //    currentFullCtorParam = fullConstructor.Parameters.AddNew(new TypedName(string.Format("validTokensFor{0}In{1}", target.Options.AssemblyName, i + 1), currentCaseSelector));
                //else
                //    currentFullCtorParam = fullConstructor.Parameters.AddNew(new TypedName(string.Format("validTokens", target.Options.AssemblyName), currentCaseSelector));
                currentCaseSelector.AccessLevel = DeclarationAccessLevel.Public;
                int index = 0;
                if (setLength == SetCommon.MinimalSetData.setSize)
                    currentCaseSelector.BaseType = EnumeratorBaseType.UInt;
                bool containsCapture = false;
                tokenEntrySets[i] = new ITokenEntry[setLength];
                for (int j = 0; j < setLength; j++)
                {

                    /* *
                     * Create a sub-set of the token kinds, store the emit type of the token
                     * so the individual token can emit its respective sub-set information
                     * if it exists as a set of literal values in a series.
                     * */
                    var token = tokenEntries[i * SetCommon.MinimalSetData.setSize + j];
                    tokenEntrySets[i][j] = token;
                    setIndices.Add(token, i);
                    EntryToPropertyCaseLookup.Add(token, currentBaseInterfaceSetProperty);
                    tokenTypes.Add(token, DiscernType(token));
                    if (tokenTypes[token] == StateMachineType.Capture)
                    {
                        tokenBaseTypeLookup.Add(token, currentCaptureTokenBase);
                        containsCapture = true;
                    }
                    //Create and cross relate the field member for the current type of token.
                    IFieldMember currentField = currentCaseSelector.Fields.AddNew(token.Name);
                    index++;
                    //Set its value to a power of two.
                    if (index == SetCommon.MinimalSetData.setSize)
                        currentField.InitializationExpression = new PrimitiveExpression(uint.MaxValue);
                    else
                        currentField.InitializationExpression = new PrimitiveExpression((int)(Math.Pow(2, index - 1)));
                    currentField.Summary = "Valid token case selector for scanner context awareness.";

                    /* *
                     * This enables the scanner, based upon range overlap, to properly extract
                     * data on which tokens are valid in the current scope.
                     * */
                    //EntryToValidFieldList.Add(token, caseField);
                    /* *
                     * So token instances can refer to their proper case type
                     * and field within that case.
                     * */
                    EntryToCaseFieldList.Add(token, currentField);
                    EntryToCaseTypeList.Add(token, currentCaseSelector);
                }
                //caseField.AccessLevel = DeclarationAccessLevel.Public;
                //fullConstructor.Statements.Assign(caseField.GetReference(), currentFullCtorParam.GetReference());
                if (!containsCapture)
                {
                    captureAdders[i].ParentTarget.Methods.Remove(captureAdders[i]);
                    captureAdders[i].Dispose();
                    captureAdders[i] = null;
                    scannerCaptureEntries[i].ParentTarget.Classes.Remove(scannerCaptureEntries[i]);
                    scannerCaptureEntries[i].Dispose();
                    scannerCaptureEntries[i] = null;
                    //captureCacheFields[i].ParentTarget.Fields.Remove(captureCacheFields[i]);
                    //captureCacheFields[i].Dispose();
                    //captureCacheFields[i] = null;
                    captureTokenBases[i].ParentTarget.Classes.Remove(captureTokenBases[i]);
                    captureTokenBases[i].Dispose();
                    captureTokenBases[i] = null;
                    continue;
                }
                //IConstructorMember caseConstructor = validDataType.Constructors.AddNew();
                //var caseParameter = caseConstructor.Parameters.AddNew(new TypedName(numSets > 1 ? string.Format("validTokensInSet{0}", i + 1) : "validTokens", currentCaseSelector));
                //caseConstructor.CascadeExpressionsTarget = ConstructorCascadeTarget.This;
                //caseConstructor.Statements.Assign(caseField.GetReference(), caseParameter.GetReference());
                //caseConstructors.Add(currentCaseSelector, caseConstructor);
                //caseConstructor.AccessLevel = DeclarationAccessLevel.Internal;
            }
            #endregion
            
            //dataSet.FullCaseConstructor = fullConstructor;
            //fullConstructor.AccessLevel = DeclarationAccessLevel.Internal;
            foreach (var item in tokenData)
            {
                var state = item.Value.GetState();
                #region State-machine initialization
                string tokenStateMachineName = string.Format(stateMachineFormat, target.Options.TokenPrefix, item.Value.Source.Name, target.Options.TokenSuffix);

                if (first)
                    first = false;
                else
                    defaultNamepace = defaultNamepace.Partials.AddNew();

                var tokenStateMachine = defaultNamepace.Classes.AddNew(tokenStateMachineName);
                tokenStateMachine.AccessLevel = DeclarationAccessLevel.Internal;
                #endregion
                //tokenStateMachine.Module = tokenModule;
                /* *
                 * Based upon the type, emit a proper verifier.
                 * * 
                 * Note: Capture-state machines are capturing recognizers only;
                 * hopefully, in version 2.0 this can change into a properly
                 * implemented regular language capture.
                 * */
                
                switch (tokenTypes[item.Key])
                {
                    case StateMachineType.Capture:
                        
                        tokenStateMachine.BaseType = captureType.BitStream.GetTypeReference();
                        if (item.Key is ITokenEofEntry)
                            dataSet.Add(item.Key, new TokenEofFinalData((ITokenEofEntry)item.Key, scannerCaptureEntries[setIndices[item.Key]]));
                        else
                            dataSet.Add(item.Key, CreateCaptureStateMachine(item.Key, state, tokenStateMachine, captureType, scannerTokenSetData, scannerCaptureEntries[setIndices[item.Key]]));
                        dataSet[item.Key].TokenInterface = tokenBaseInterface;
                        //Associate the base type with the token.
                        dataSet[item.Key].TokenBaseType = tokenBaseTypeLookup[item.Key];
                        if (!(item.Key is ITokenEofEntry))
                            ((TokenCaptureFinalData)(dataSet[item.Key])).DataSetAddMethod = captureAdders[setIndices[item.Key]];
                        else
                        {
                            ((TokenEofFinalData)(dataSet[item.Key])).DataSetAddMethod = captureAdders[setIndices[item.Key]];
                            tokenStateMachine.ParentTarget.Classes.Remove(tokenStateMachine);
                            tokenStateMachine.Dispose();
                            tokenStateMachine = null;
                        }
                        break;
                    case StateMachineType.Enumeration:
                        var data = CreateEnumerationStateMachine(assemblyChildSpace, state, item.Key, tokenStateMachine, target, EntryToCaseFieldList[item.Key], noneFields.Distinct().Where(p => p.ParentTarget != EntryToCaseFieldList[item.Key].ParentTarget).ToArray(), EntryToPropertyCaseLookup[item.Key], EntryToPropertyCaseLookup.Values.Distinct().Where(p => p != EntryToPropertyCaseLookup[item.Key]).ToArray(), tokenBaseInterface, scannerTokenSetData, setAdders[setIndices[item.Key]], scannerTokenSetDataSetIndices, scannerEntry);
                        dataSet.Add(item.Key, data);
                        var enumData = (TokenEnumFinalData)(data);
                        var enumSetData = enumData as TokenEnumSetFinalData;
                        enumData.DataSetAddMethods = new Dictionary<IEnumeratorType, IMethodMember>();
                        if (enumSetData != null)
                        {
                            foreach (var caseEnum in enumSetData.SubsetEnumerations.Values)
                            {
                                var m = setAdders[setIndices[item.Key]].First(p => p.Key == caseEnum).Value;
                                m.AccessLevel = DeclarationAccessLevel.Public;
                                enumSetData.DataSetAddMethods.Add(caseEnum, m);
                            }
                        }
                        else
                        {
                            var m = setAdders[setIndices[item.Key]].First(p => p.Key == enumData.CaseEnumeration).Value;
                            m.AccessLevel = DeclarationAccessLevel.Public;
                            enumData.DataSetAddMethods.Add(enumData.CaseEnumeration, m);
                        }
                        break;
                }
                //Console.WriteLine("{1} Reduced by {0}% ({2}->{3})", 100 - (int)((double)dataSet[item.Key].FinalState.Count() * 100d / (double)state.Count()), item.Key.Name, state.Count(), dataSet[item.Key].FinalState.Count());
                //Console.WriteLine("{0}", dataSet[item.Key].FinalState);
                //if (tokenTypes[item.Key] == StateMachineType.Capture)
                //    dataSet[item.Key].ValidDataConstructor = caseConstructors[EntryToCaseTypeList[item.Key]];
                //dataSet[item.Key].ValidDataField = EntryToValidFieldList[item.Key];
                dataSet[item.Key].ValidCaseField = EntryToCaseFieldList[item.Key];

                #region Simple map between range overlap across all token entries

                /* *
                 * The following code serves as a measure to segment the tokens
                 * by the character ranges shared between them.
                 * *
                 * Such data is used to properly build the next token algorithm
                 * in the scanner for the parser.
                 * */
                Dictionary<RegularLanguageBitArray, RegularLanguageBitArray> intersections = new Dictionary<RegularLanguageBitArray, RegularLanguageBitArray>();
                Dictionary<RegularLanguageBitArray, RegularLanguageBitArray> complements = new Dictionary<RegularLanguageBitArray, RegularLanguageBitArray>();
                RegularLanguageBitArray fullRange = null;
                if (dataSet[item.Key].FinalState != null)
                    fullRange = dataSet[item.Key].FinalState.OutTransitions.GetCheckRange();
                if (fullRange != null)
                {
                    foreach (var rlba in relationships)
                    {
                        var intersection = rlba.Key & fullRange;
                        if (intersection.AllFalse)
                            continue;
                        var complement = rlba.Key ^ intersection;
                        intersections.Add(rlba.Key, intersection);
                        complements.Add(rlba.Key, complement);
                        fullRange ^= intersection;
                    }
                    Dictionary<RegularLanguageBitArray, List<ITokenEntry>> backup = new Dictionary<RegularLanguageBitArray, List<ITokenEntry>>();
                    foreach (var original in intersections.Keys)
                        backup.Add(original, new List<ITokenEntry>(relationships[original]));
                    foreach (var intersectionKVP in intersections)
                    {
                        var intersection = intersections[intersectionKVP.Key];
                        var complement = complements[intersectionKVP.Key];
                        relationships.Remove(intersectionKVP.Key);
                        relationships.Add(intersection, new List<ITokenEntry>(backup[intersectionKVP.Key]));
                        relationships[intersection].Add(item.Key);
                        if (!complement.AllFalse)
                            relationships.Add(complement, new List<ITokenEntry>(backup[intersectionKVP.Key]));
                    }
                    if (!fullRange.AllFalse)
                        relationships.Add(fullRange, new List<ITokenEntry>(new ITokenEntry[] { item.Key }));
                }
                #endregion
            }
            //foreach (var item in tokenData)
            //    if (tokenTypes[item.Key] == StateMachineType.Enumeration)
            //        dataSet[item.Key].ValidDataConstructor = tokenConstructors[item.Key];
            //fullConstructor.Parameters.Remove(dummyParam);
            //if (!enumSet)
            //    fullConstructor.ParentTarget.Constructors.Remove(fullConstructor);
            dataSet.TokenSelectorSets = EntryToCaseTypeList.Values.Distinct().ToArray();
            dataSet.Relationships = relationships;
            outTokenEntries = tokenEntrySets;
            return dataSet;
        }

        private static Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> BuildPrecedenceTable(IEnumerable<ITokenEntry> tokenData)
        {
            Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>> result = new Dictionary<ITokenEntry, Dictionary<ITokenEntry, TokenPrecedence>>();
            foreach (var token in tokenData)
            {
                var currentSet = new Dictionary<ITokenEntry, TokenPrecedence>();
                var currentLowerSet = GetLowerTokens(token);
                result.Add(token, currentSet);
                foreach (var lower in currentLowerSet)
                    currentSet.Add(lower, TokenPrecedence.Higher);
            }
            foreach (var a in tokenData)
                foreach (var b in tokenData)
                {
                    if (!result[a].ContainsKey(b))
                    {
                        if (result[b].ContainsKey(a))
                            if (result[b][a] == TokenPrecedence.Higher)
                                result[a].Add(b, TokenPrecedence.Lower);
                            else
                                result[a].Add(b, TokenPrecedence.Equal);
                        else
                            result[a].Add(b, TokenPrecedence.Equal);
                    }
                    if (!result[b].ContainsKey(a))
                        if (result[a].ContainsKey(b))
                            if (result[a][b] == TokenPrecedence.Higher)
                                result[b].Add(a, TokenPrecedence.Lower);
                            else
                                result[b].Add(a, TokenPrecedence.Equal);
                        else
                            result[b].Add(a, TokenPrecedence.Equal);

                }
            foreach (var token in tokenData)
                foreach (var equal in result[token].Keys)
                {
                    if (token == equal)
                        continue;
                    if (result[token][equal] == TokenPrecedence.Equal)
                    {
                        foreach (var lower in result[token].Keys)
                            if (result[token][lower] == TokenPrecedence.Higher)
                            {
                                result[equal][lower] = TokenPrecedence.Higher;
                                result[lower][equal] = TokenPrecedence.Lower;
                            }
                    }
                }
            return result;
        }

        private static List<ITokenEntry> GetLowerTokens(ITokenEntry start)
        {
            List<ITokenEntry> result = new List<ITokenEntry>();
            GetLowerTokens(start, result);
            return result;
        }

        private static void GetLowerTokens(ITokenEntry start, List<ITokenEntry> currentList)
        {
            if (start.LowerPrecedenceTokens == null)
                return;
            foreach (var lower in start.LowerPrecedenceTokens)
                if (!currentList.Contains(lower))
                {
                    currentList.Add(lower);
                    GetLowerTokens(lower, currentList);
                }
        }

        /* *
         * This differs from the capture method, for the simple fact that the capture
         * aspect needs to track the current length and the capture itself.
         * *
         * The length of every string is known, so length tracking is un-necessary.
         * */
        private static TokenEnumFinalData    CreateEnumerationStateMachine(
                    INameSpaceDeclaration    assemblyChildSpace,
                     RegularLanguageState    state, 
                              ITokenEntry    target, 
                               IClassType    tokenStateMachine, 
                                  IGDFile    source,
                             IFieldMember    fullTokensCase,
                             IFieldMember [] fullTokensCases,
                 IPropertySignatureMember    fullTokensProperty,
                 IPropertySignatureMember [] fullTokensProperties,
                           IInterfaceType    tokenBaseInterface,
                               IClassType    scannerTokenSetData,
Dictionary<IEnumeratorType, IMethodMember>   setAdder,
                          IEnumeratorType    scannerTokenSetDataSetIndices,
                               IClassType    scannerEntry)
        {
            state = RegularLanguageState.Merger.ToDFA(state, false);
            state.Enumerate();
            //Console.WriteLine(state);
            #region Locals

            StatePrimitiveLookup statePrimitives = ObtainFlatStates(state).Sort();
            PrimitiveStateLookup primitiveStates = statePrimitives.ObtainReverseLookup();
            var exitStatesEnum = tokenStateMachine.Enumerators.AddNew("ExitStates");
            /* *
             * Gather the elements contained within the enumerator.
             * */
            List<ITokenItem> items = GatherEnumItems(target);
            /* *
             * Obtain the edges of the initial-state.
             * */
            IEnumerable<RegularLanguageState> edges = state.ObtainEdges();
            /* *
             * The transition graph basically allows easier navigation of the 
             * terminal->source structure in a reversed manner.  Instead of
             * state->source it's source->state.  In both the relationship
             * can be many:many, as sources can exist on multiple states
             * and multiple sources can exist on a state.
             * */
            SourceTransitionGraph graph = SourceTransitionGraph.GetForkTransitionGraph(state, items);
            #region SimpleState-Machine Data-members
            //All IFieldMember
            var stateField = tokenStateMachine.Fields.AddNew(new TypedName("state", typeof(int)));
            var exitStateField = tokenStateMachine.Fields.AddNew(new TypedName("exitState", exitStatesEnum));
            var exitStateFieldRef = exitStateField.GetReference();
            var stateFieldRef = stateField.GetReference();
            #endregion

            var bytesConsumed = tokenStateMachine.Properties.AddNew(new TypedName("BytesConsumed", typeof(int)), true, false);
            bytesConsumed.AccessLevel = DeclarationAccessLevel.Public;

            var nextMethod = tokenStateMachine.Methods.AddNew(new TypedName("Next", typeof(bool)));
            nextMethod.AccessLevel = DeclarationAccessLevel.Public;
            var charParameter = nextMethod.Parameters.AddNew(new TypedName("char", typeof(char)));
            var charParameterRef = charParameter.GetReference();

            var nextStateSwitch = nextMethod.SelectCase(stateFieldRef);

            //Default response if no path is followed, or a terminal is encountered.
            nextMethod.Return(PrimitiveExpression.FalseValue);
            #endregion

            EnumStateMachineDataSet dataSet = new EnumStateMachineDataSet();

            int setCount = 0;
            EnumFieldLookup enumToCaseLookup;
            EnumFieldRefLookup enumAllFieldLookup;
            EnumFieldRefLookup enumNoneFieldLookup;
            EnumFieldRefLookup enumLocalRefLookup = new EnumFieldRefLookup();
            EnumFieldLookup enumLocalLookup = new EnumFieldLookup();
            Dictionary<ITokenItem, IEnumeratorType> itemToEnumLookup = new Dictionary<ITokenItem, IEnumeratorType>();
            IClassType[] subsetTokenClasses;
            IInterfaceType baseTokenInterface;
            IFieldMember[] scannerSetFields;
            IDictionary<IEnumeratorType, IFieldMember> relationalScannerSetFields = new Dictionary<IEnumeratorType, IFieldMember>();
            IEnumeratorType[] enums = BuildDataset(
                assemblyChildSpace, 
                (INameSpaceDeclaration)(tokenStateMachine.ParentTarget), graph, dataSet,
                target, edges, items, exitStatesEnum, source, tokenBaseInterface, 
                out baseTokenInterface, fullTokensCase, fullTokensCases, fullTokensProperty,
                fullTokensProperties, out setCount, out enumToCaseLookup, out enumAllFieldLookup, 
                out enumNoneFieldLookup, out subsetTokenClasses, setAdder, scannerTokenSetData,
                scannerTokenSetDataSetIndices, out scannerSetFields);
            for (int i = 0; i < scannerSetFields.Length; i++)
            {
                relationalScannerSetFields.Add(enums[i], scannerSetFields[i]);
            }
            foreach (var item in dataSet.Keys)
                itemToEnumLookup.Add(item, (IEnumeratorType)dataSet[item].ResultEnumerationMember.ParentTarget);
            stateField.Summary = "Data member which tracks the current execution path of the state machine.";
            exitStateField.Summary = "Data member which denotes the final execution point of the state machine.";
            exitStateField.Remarks = "Only set when an edge in the state machine is hit.";
            var isValidState = tokenStateMachine.Properties.AddNew(new TypedName("IsValidEndState", typeof(bool)), true, false);
            /* *
             * I realize this can take a lot of processing power as the list rises
             * in quantity; however, I don't know another method.
             * */
            Dictionary<RegularLanguageBitArraySeries, RegularLanguageStateSeries> completedTerminalChecks = new Dictionary<List<RegularLanguageBitArray>, List<RegularLanguageState>>();

            /* *
             * The series of transitions that has been represented by the current
             * state machine.
             * *
             * Used to construct a listing of the self ambiguous end states with the 
             * sub-states that also end in the middle of longer terms.
             * As well as reduce the state logic needed when certain subsets share
             * left-hand states.
             * */
            TransitionGraph transitionGraph = GetTransitionGraph(state);
            IDictionary<ITokenItem, ITokenItemData> tokenRelationshipInfo = new Dictionary<ITokenItem, ITokenItemData>();
            var resetMethodO1 = tokenStateMachine.Methods.AddNew(new TypedName("Reset", typeof(void)));
            var voidResetForwardCallParams = new ExpressionCollection();
            resetMethodO1.Summary = "Resets the state machine to its default state.";
            if (enums.Length > 1)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    var ce = enums[i];
                    string vName = null;
                    string pName = null;
                    if (i == enums.Length - 1)
                        vName = "ActiveSets";
                    else
                    {
                        pName = string.Format("allowedInSet{0}", i + 1);
                        vName = string.Format("AllowedInSet{0}", i + 1);
                    }
                    var currentField = tokenStateMachine.Fields.AddNew(new TypedName(vName, ce), enumNoneFieldLookup[ce]);
                    if (i == enums.Length - 1)
                        currentField.Summary = "Defines the allowable range for the overall series";
                    else
                        currentField.Summary = string.Format("Defines the allowable range for set {0}", i + 1);
                    var currentFieldRef = currentField.GetReference();
                    enumLocalLookup.Add(ce, currentField);
                    enumLocalRefLookup.Add(ce, currentFieldRef);
                    if (i < enums.Length - 1)
                    {
                        var currentParameter = resetMethodO1.Parameters.AddNew(new TypedName(pName, ce));
                        currentParameter.DocumentationComment = string.Format("Sets the state machine up to limit the elements in set {0}.", i + 1);
                        var currentParameterRef = currentParameter.GetReference();
                        voidResetForwardCallParams.Add(enumAllFieldLookup[ce]);
                        resetMethodO1.Assign(currentFieldRef, currentParameterRef);
                    }
                    else if (i == enums.Length - 1)
                    {
                        resetMethodO1.Assign(currentFieldRef, enumNoneFieldLookup[ce]);
                        for (int j = 0; j < i; j++)
                        {
                            var currentLastFieldRef = enumLocalRefLookup[enums[j]];
                            var currentCondition = resetMethodO1.IfThen(new BinaryOperationExpression(currentLastFieldRef, Operators.IdentityInequality, enumNoneFieldLookup[enums[j]]));
                            currentCondition.Assign(currentFieldRef, new BinaryOperationExpression(currentFieldRef, Operators.BitwiseOr, enumToCaseLookup[enums[j]].GetReference()));
                        }
                    }
                }
            }
            else
            {
                var currentParameter = resetMethodO1.Parameters.AddNew(new TypedName(string.Format("allowed{0}", target.Name), enums[0]));
                currentParameter.DocumentationComment = "Sets the allowable range for the overall series.";
                voidResetForwardCallParams.Add(enumAllFieldLookup[enums[0]]);

                var currentField = tokenStateMachine.Fields.AddNew(new TypedName(string.Format("Allowed{0}", target.Name), enums[0]));
                enumLocalLookup.Add(enums[0], currentField);
                currentField.Summary = "Defines the allowable range for the overall series";
                enumLocalRefLookup.Add(enums[0], currentField.GetReference());
                resetMethodO1.Assign(currentField.GetReference(), currentParameter.GetReference());
            }

            resetMethodO1.Assign(exitStateFieldRef, exitStatesEnum.Fields.Values.First().GetReference());
            resetMethodO1.Assign(stateFieldRef, PrimitiveExpression.NumberZero);
            resetMethodO1.AccessLevel = DeclarationAccessLevel.Public;

            var resetMethod = tokenStateMachine.Methods.AddNew(new TypedName("Reset", typeof(void)));
            resetMethod.CallMethod(resetMethodO1.GetReference(), voidResetForwardCallParams.ToArray());
            resetMethod.AccessLevel = DeclarationAccessLevel.Public;

            /* *
             * Order the elements by their value's length.
             * *
             * This ensures BytesConsumed returns properly.
             * */
            var lengthOrdering = from g in graph.GetEnumerator()
                                 let y = ((ILiteralTokenItem)(g.Target)).Value.ToString().Length
                                 orderby y
                                 group g by y;
            var bCSwitch = bytesConsumed.GetPart.SelectCase(exitStateFieldRef);
            bytesConsumed.GetPart.Return(PrimitiveExpression.NumberZero);
            var valueIndex = 0;
            List<IFieldMember> insertedElements = new List<IFieldMember>();

            #region Exit SimpleState Indexing
            /* *
             * Reindex the exit state enumeration based upon the length
             * of the elements.*/
            foreach (var lengthSet in lengthOrdering)
            {
                ISwitchStatementCase currentCase = null;
                foreach (var branch in lengthSet)
                {
                    //Initial value used for a placeholder, change it to optimize the switch.
                    foreach (var stateToFieldKVP in dataSet[branch.Target].ExitStateConstants)
                    {
                        var field = stateToFieldKVP.Value;
                        if (insertedElements.Contains(field))
                            continue;
                        var st = stateToFieldKVP.Key;
                        /* *
                         * Only index the value if it's the transitionFirst element,
                         * if it's not, the element's already been reindexed and
                         * inserted into the switch.
                         * *///*
                        if (st.Sources.First() == branch.Target)
                        {
                            field.InitializationExpression = new PrimitiveExpression(++valueIndex);
                            if (currentCase == null)
                                currentCase = bCSwitch.Cases.AddNew(field.GetReference());
                            else
                                currentCase.Cases.Add(field.GetReference());
                        }
                        insertedElements.Add(field);
                    }
                }
                if (currentCase == null)
                    continue;
                currentCase.Return(new PrimitiveExpression(lengthSet.Key));
            }
            // * */
            #endregion

            isValidState.AccessLevel = DeclarationAccessLevel.Public;
            var exitStateTemp = isValidState.GetPart.Locals.AddNew(new TypedName("_temp_exitState", typeof(int)), new CastExpression(exitStateFieldRef, typeof(int).GetTypeReference()));
            var exitStateTempRef = exitStateTemp.GetReference();
            var lowCheck = new BinaryOperationExpression(exitStateTempRef, Operators.GreaterThan, PrimitiveExpression.NumberZero);
            var highCheck = new BinaryOperationExpression(exitStateTempRef, Operators.LessThanOrEqual, new PrimitiveExpression(valueIndex));
            
            var isValidCondition = isValidState.GetPart.IfThen(new BinaryOperationExpression(lowCheck, Operators.BooleanAnd, highCheck));
            isValidCondition.Return(PrimitiveExpression.TrueValue);
            isValidState.GetPart.Return(PrimitiveExpression.FalseValue);
            //isValidState.GetPart.Return(new BinaryOperationExpression(exitStateFieldRef, Operators.IdentityInequality, PrimitiveExpression.NumberZero));

            /* *
             * Create the labels to do the state->state transitions.
             * */
            var gEnum = graph.GetEnumerator().ToArray();

            var graphTransitionEdges = graph.GetEnumerator().ToArray();
            var finalState = new PrimitiveExpression(-1);
            foreach (RegularLanguageState currentState in statePrimitives.Keys)
            {
                /* *
                 * Ignore terminal edges.
                 * *
                 * No logic exists for transitioning away from them, since they're
                 * never reached.  All terminal edges states are '-1'.
                 * *
                 * See finalState variable above.
                 * */
                if (currentState.OutTransitions.Count == 0)
                    goto SelfAmbiguityCheck;
                var currentCase = nextStateSwitch.Cases.AddNew(statePrimitives[currentState]).NewBlock();
                var currentSwitch = currentCase.SelectCase(charParameterRef);
                foreach (var transition in currentState.OutTransitions)
                {
                    var transitionState = transition.Targets.First();
                    var currentRange = transition.Check;
                    ISwitchStatementCase rangeCase = null;
                    var currentTerminalEdgeSet = transitionState.ObtainEdges().ToArray();
                    bool terminalLogicApplied = false;
                    RegularLanguageBitArraySeries currentGraphTransition = null;
                    TransitionGraphElement currentGraph = transitionGraph[transitionState];
                    foreach (TransitionGraphElementEntry ctg in currentGraph)
                        if (ctg.ElementAt(ctg.Count - 2).Key == currentState)
                            currentGraphTransition = new RegularLanguageBitArraySeries(ctg.Values.ToArray());
                    foreach (var transitionCheckKV in completedTerminalChecks)
                    {
                        var previousTransitionSet = transitionCheckKV.Value;
                        bool found = true;
                        foreach (var subTState in previousTransitionSet)
                        {
                            foreach (var terminalState in currentTerminalEdgeSet)
                                if (!currentTerminalEdgeSet.Contains(subTState))
                                {
                                    foreach (var terminalSource in subTState.Sources)
                                        if (!terminalState.Sources.Contains(terminalSource))
                                        {
                                            found = false;
                                            break;
                                        }
                                    if (!found)
                                        break;
                                }
                            if (!found)
                                break;
                        }
                        if (found)
                        {
                            //Elements of an enumeration are fixed-length for a given set of transitions.
                            int setMax = Math.Min(transitionCheckKV.Key.Count, currentGraphTransition.Count);
                            for (int i = 0; i < setMax; i++)
                                if (currentGraphTransition[i] != transitionCheckKV.Key[i])
                                {
                                    found = false;
                                    break;
                                }
                            if (found)
                                terminalLogicApplied = true;
                        }
                    }
                    /* *
                     * Iterate through the current transition range, and insert the characters
                     * as needed.
                     * *
                     * This might not always yield a single character due to cases
                     * where the language uses case-insensitive keywords.
                     * */
                    for (uint i = currentRange.Offset; i < currentRange.Length; i++)
                    {
                        if (currentRange[i])
                        {
                            if (rangeCase == null)
                                rangeCase = currentSwitch.Cases.AddNew(new PrimitiveExpression((char)(i)));
                            else
                                rangeCase.Cases.Add(new PrimitiveExpression((char)(i)));
                        }
                    }
                    #region Build Current Set String
                    StringBuilder currentSet = new StringBuilder();
                    foreach (var charElement in currentGraphTransition)
                    {
                        if (charElement == null)
                            continue;
                        charElement.Reduce();
                        if (charElement.CountTrue() == 1)
                            currentSet.Append((char)charElement.Offset);
                        else
                        {
                            RegularLanguageBitArray.RangeData data = charElement.GetRange();
                            currentSet.Append("[");
                            for (int i = 0; i < data.Singletons.Count; i++)
                                currentSet.Append(data.Singletons[i]);
                            for (int i = 0; i < data.Sets.Count; i++)
                            {
                                currentSet.Append(data.Sets[i].Start);
                                currentSet.Append("-");
                                currentSet.Append(data.Sets[i].End);
                            }
                            currentSet.Append("]");
                        }
                    }
                    #endregion

                    rangeCase.Statements.Add(new CommentStatement(currentSet.ToString()));
                    IStatementBlockInsertBase targetCodeBase = rangeCase;
                    var currentFork = (from i in gEnum
                                       where i.TargetStates.Contains(transitionState)
                                       select i).FirstOrDefault();
                    IFieldMember exitField = null;
                    IEnumeratorType exitFieldEnum = null;
                    IConditionStatement exitArea = null;
                    IFieldMember currentExitStateField = null;
                    ITokenItem[] overlap = new ITokenItem[0];
                    if (currentFork != null)
                    {
                        exitField = dataSet[currentFork.Target].ResultEnumerationMember;
                        currentExitStateField = dataSet[currentFork.Target].ExitStateConstants[transitionState];
                        overlap = (from v in gEnum
                                   where v.Target != currentFork.Target
                                   where dataSet[v.Target].ExitStateConstants.ContainsValue(currentExitStateField)
                                   select v.Target).ToArray();
                    }
                    if (!terminalLogicApplied)
                    {
                        Dictionary<IEnumeratorType, List<IFieldMember>> terminalCases = new Dictionary<IEnumeratorType, List<IFieldMember>>();
                        foreach (var termState in currentTerminalEdgeSet)
                        {
                            foreach (var terminalSource in termState.Sources)
                            {
                                if (terminalSource == null)
                                    continue;
                                var enumMember = dataSet[terminalSource].ResultEnumerationMember;
                                /* *
                                 * Ensures that the exit field member isn't included in the over-all
                                 * series.
                                 * */
                                var currentEnum = itemToEnumLookup[terminalSource];
                                if (enumMember == exitField)
                                {
                                    exitFieldEnum = currentEnum;
                                    continue;
                                }
                                else if (overlap.Length > 0)
                                {
                                    bool overlapFound = false;
                                    for (int o = 0; o < overlap.Length; o++)
                                    {
                                        var currentOverlap = overlap[o];
                                        if (dataSet[currentOverlap].ResultEnumerationMember == enumMember)
                                            overlapFound = true;
                                    }
                                    if (overlapFound)
                                        continue;
                                }
                                if (!terminalCases.ContainsKey(currentEnum))
                                     terminalCases.Add(currentEnum, new List<IFieldMember>());
                                if (!terminalCases[currentEnum].Contains(dataSet[terminalSource].ResultEnumerationMember))
                                     terminalCases[currentEnum].Add(enumMember);
                            }
                        }
                        completedTerminalChecks.Add(new RegularLanguageBitArraySeries(currentGraphTransition), new RegularLanguageStateSeries(currentTerminalEdgeSet));
                        if (terminalCases.Count == 0 && exitField == null)
                            goto IgnoreTerminalCheck;

                        bool insertAsFalsePart = false;
                        //Special case.
                        if (exitField != null)
                        {
                            var currentFieldRef = enumLocalRefLookup[exitFieldEnum];
                            insertAsFalsePart = true;
                            var exitFieldExpression = (IExpression)exitField.GetReference();
                            if (overlap.Length > 0)
                            {
                                for (int o = 0; o < overlap.Length; o++)
                                {
                                    var currentOverlap = overlap[0];
                                    /* *
                                     * ToDo: In the off chance that two members
                                     * exist as the same value in the same set, but with
                                     * different names.
                                     * *
                                     * _Add code here to enable the ability to discern
                                     * the different subsets.
                                     * */
                                    exitFieldExpression = BOp(exitFieldExpression, CodeBinaryOperatorType.BitwiseOr, (IExpression)dataSet[currentOverlap].ResultEnumerationMember.GetReference());
                                }
                            }
                            IExpression currentElementExpression = new BinaryOperationExpression(currentFieldRef, Operators.BitwiseAnd, exitFieldExpression);
                            targetCodeBase = exitArea = targetCodeBase.IfThen(new BinaryOperationExpression(currentElementExpression, Operators.IdentityInequality, enumNoneFieldLookup[exitFieldEnum]));
                        }

                        if (terminalCases.Count == 0)
                            goto IgnoreTerminalCheck;
                        else if (exitField != null && terminalCases.Count == 1 && 
                                 terminalCases.ElementAt(0).Value.Count == 1)
                        {
                            /* *
                             * The terminal state will emit the proper
                             * context awareness check.
                             * */
                            targetCodeBase = exitArea.FalseBlock;
                            goto IgnoreTerminalCheck;
                        }
                        /* *
                         * If there is more than one set, include the logic on the set
                         * selector as well.
                         * */
                        if (enums.Length > 1)
                        {
                            IExpression fullSetExpression = null;
                            IExpression setSelectorExpression = null;
                            bool buildSetSelectorExpression = terminalCases.Count > 1;
                            foreach (var ce in terminalCases.Keys)
                            {
                                if (buildSetSelectorExpression)
                                {
                                    if (setSelectorExpression == null)
                                        setSelectorExpression = enumToCaseLookup[ce].GetReference();
                                    else
                                        setSelectorExpression = new BinaryOperationExpression(setSelectorExpression, Operators.BitwiseOr, enumToCaseLookup[ce].GetReference());
                                }
                                IExpression currentEnumExpression = null;
                                foreach (var item in terminalCases[ce])
                                {
                                    if (currentEnumExpression == null)
                                        currentEnumExpression = item.GetReference();
                                    else
                                        currentEnumExpression = new BinaryOperationExpression(currentEnumExpression, Operators.BitwiseOr, item.GetReference());
                                }
                                currentEnumExpression = new BinaryOperationExpression(new BinaryOperationExpression(enumLocalRefLookup[ce], Operators.BitwiseAnd, currentEnumExpression), Operators.IdentityInequality, enumNoneFieldLookup[ce]);
                                if (fullSetExpression == null)
                                    fullSetExpression = currentEnumExpression;
                                else
                                    fullSetExpression = new BinaryOperationExpression(fullSetExpression, Operators.BooleanOr, currentEnumExpression);
                            }
                            int lastEnumIndex = enums.Length - 1;
                            //(ActiveSets & setSelectorExpression) != [TokenName]Cases.None;
                            if (buildSetSelectorExpression)
                            {
                                setSelectorExpression = new BinaryOperationExpression(new BinaryOperationExpression(enumLocalRefLookup[enums[lastEnumIndex]], Operators.BitwiseAnd, setSelectorExpression), Operators.IdentityInequality, enumNoneFieldLookup[enums[lastEnumIndex]]);
                                if (insertAsFalsePart)
                                    targetCodeBase = ((IConditionStatement)(targetCodeBase)).FalseBlock.IfThen(new BinaryOperationExpression(setSelectorExpression, Operators.BooleanAnd, fullSetExpression));
                                else
                                    targetCodeBase = targetCodeBase.IfThen(new BinaryOperationExpression(setSelectorExpression, Operators.BooleanAnd, fullSetExpression));
                            }
                            else if (insertAsFalsePart)
                                    targetCodeBase = ((IConditionStatement)(targetCodeBase)).FalseBlock.IfThen(fullSetExpression);
                                else
                                    targetCodeBase = targetCodeBase.IfThen(fullSetExpression);
                        }
                        else
                        {
                            /* *
                             * Special case since it doesn't contain the set selection code.
                             * */
                            var firstEnum = enums[0];
                            var currentFieldRef = enumLocalRefLookup[enums[0]];
                            IExpression checkExpression = null;
                            foreach (var item in terminalCases[firstEnum])
                            {
                                if (checkExpression == null)
                                    checkExpression = item.GetReference();
                                else
                                    //checkedExpression | itemReference
                                    checkExpression = new BinaryOperationExpression(checkExpression, Operators.BitwiseOr, item.GetReference());
                            }
                            //(Allowed[TokenName] & checkedExpression) != [TokenName]Cases.None
                            checkExpression = new BinaryOperationExpression(new BinaryOperationExpression(enumLocalRefLookup[firstEnum], Operators.BitwiseAnd, checkExpression), Operators.IdentityInequality, enumNoneFieldLookup[enums[0]]);

                            if (insertAsFalsePart)
                                targetCodeBase = ((IConditionStatement)(targetCodeBase)).FalseBlock.IfThen(checkExpression);
                            else
                                targetCodeBase = targetCodeBase.IfThen(checkExpression);
                        }
                    }
                IgnoreTerminalCheck:
                    if (currentFork != null)
                    {
                        exitField = dataSet[currentFork.Target].ExitStateConstants[transitionState];
                        if (exitArea == null)
                            targetCodeBase.Assign(exitStateFieldRef, exitField.GetReference());
                        else
                            exitArea.Assign(exitStateFieldRef, exitField.GetReference());
                    }
                    if (transitionState.OutTransitions.Count() > 0)
                    {
                        if (exitArea != null)
                        {
                            exitArea.Assign(stateFieldRef, statePrimitives[transitionState]);
                            exitArea.Return(PrimitiveExpression.TrueValue);
                        }
                        targetCodeBase.Assign(stateFieldRef, statePrimitives[transitionState]);
                        targetCodeBase.Return(PrimitiveExpression.TrueValue);
                    }
                    else if (exitArea != null)
                        exitArea.Assign(stateFieldRef, finalState);
                    else
                        targetCodeBase.Assign(stateFieldRef, finalState);

                }
            SelfAmbiguityCheck:
                if (currentState.IsEdge())
                {
                    var currentTrail = transitionGraph[currentState];
                    foreach (var subTrail in currentTrail)
                    {
                        var lastKVP = subTrail.Last();
                        var currentFork = (from i in gEnum
                                           where i.TargetStates.Contains(lastKVP.Key)
                                           select i).FirstOrDefault();
                        if (currentFork == null)
                            continue;
                        var currentDataSetEntry = dataSet[currentFork.Target];
                        var currentSet = new SelfAmbiguousTokenItemData(currentDataSetEntry.ExitStateConstants[lastKVP.Key]);
                        currentSet.Add(new SelfAmbiguousTokenItemData.AllowedInfo(
                            enumLocalLookup[(IEnumeratorType)currentDataSetEntry.ResultEnumerationMember.FieldType.TypeInstance], 
                            currentDataSetEntry.ResultEnumerationMember,
                            enumNoneFieldLookup[(IEnumeratorType)currentDataSetEntry.ResultEnumerationMember.FieldType.TypeInstance]));
                        tokenRelationshipInfo.Add(currentFork.Target, currentSet);
                        var subSet = subTrail.Skip(1).Take(subTrail.Count - 2);
                        
                        foreach (var subElement in subSet)
                        {
                            var subFork = (from i in gEnum
                                           where i.TargetStates.Contains(subElement.Key)
                                           select i).FirstOrDefault();
                            if (subFork == null)
                                continue;
                            var dataSetEntry = dataSet[subFork.Target];
                            currentSet.Add(new SelfAmbiguousTokenItemData.AllowedInfo(enumLocalLookup[(IEnumeratorType)dataSetEntry.ResultEnumerationMember.FieldType.TypeInstance], dataSetEntry.ResultEnumerationMember, enumNoneFieldLookup[(IEnumeratorType)dataSetEntry.ResultEnumerationMember.FieldType.TypeInstance]));
                        }
                        if (!target.SelfAmbiguous || currentSet.Count == 1)
                        {
                            SelfAmbiguousTokenItemData.AllowedInfo zeroInfo = currentSet[0];
                            TokenItemData newData = new TokenItemData(currentSet.PrimaryExitState, zeroInfo.AllowedField, zeroInfo.SubsetField, zeroInfo.NoneReference);
                            tokenRelationshipInfo.Remove(currentFork.Target);
                            tokenRelationshipInfo.Add(currentFork.Target, newData);
                        }
                    }
                }
            }
            TokenEnumFinalData result = null;
            if (setCount > 1)
            {
                Dictionary<IFieldMember, IEnumeratorType> resultEnumItemLookup = new Dictionary<IFieldMember, IEnumeratorType>();
                foreach (var item in enumToCaseLookup)
                    resultEnumItemLookup.Add(item.Value, item.Key);
                TokenEnumSetFinalData r = new TokenEnumSetFinalData(target, dataSet.Keys.ToArray(), enums[enums.Length - 1], tokenStateMachine, state, dataSet, resultEnumItemLookup, relationalScannerSetFields, scannerEntry, tokenRelationshipInfo, exitStatesEnum, exitStateField);
                result = r;//new TokenEnumSetFinalData(target, dataSet.Keys.ToArray(), enums[enums.Length - 1], tokenStateMachine, state, dataSet, resultEnumItemLookup, scannerSetFields, scannerEntry);
                r.TokenBaseTypes = subsetTokenClasses;
            }
            else
                result = new TokenEnumFinalData(target, dataSet.Keys.ToArray(), enums[0], tokenStateMachine, state, dataSet, relationalScannerSetFields, scannerEntry, tokenRelationshipInfo, exitStatesEnum, exitStateField);
            result.TokenInterface = baseTokenInterface;
            result.TokenBaseType = subsetTokenClasses[0];

            //EnumerationTokenData result = new EnumerationTokenData(setCount, enums, items, state);
            return result;
        }

        private static TransitionGraph GetTransitionGraph(RegularLanguageState state)
        {
            var result = new TransitionGraph();
            GetTransitionGraph(new TransitionGraphElementEntry() { { state, null } }, state, result);
            return result;
        }

        private static void GetTransitionGraph(TransitionGraphElementEntry currentSet, RegularLanguageState currentState, TransitionGraph result)
        {
            TransitionGraphElementEntry currentSetCopy = new TransitionGraphElementEntry(currentSet);
            if (currentSet.Count > 0)
                if (!result.ContainsKey(currentState))
                    result.Add(currentState, new TransitionGraphElement(new TransitionGraphElementEntry[] { new TransitionGraphElementEntry(currentSetCopy) }));
                else
                    result[currentState].Add(new TransitionGraphElementEntry(currentSetCopy));
            foreach (var transition in currentState.OutTransitions)
            {
#if OLD_REGULAR_LANGUAGE_STATE
                var first = transition.Value.First();
                currentSetCopy.Add(first, transition.Key);
#else
                var first = transition.Targets.First();
                currentSetCopy.Add(first, transition.Check);
#endif
                GetTransitionGraph(currentSetCopy, first, result);
                currentSetCopy.Remove(first);
            }
        }

        private static     IEnumeratorType [] BuildDataset 
                    (INameSpaceDeclaration    assemblyChildSpace,
                     INameSpaceDeclaration    targetNamespace,
                     SourceTransitionGraph    graph,
                   EnumStateMachineDataSet    dataSet,
                               ITokenEntry    target,
          IEnumerable<RegularLanguageState>   edges,
                           List<ITokenItem>   items,
                           IEnumeratorType    stateMachineCases,
                                   IGDFile    source,
                            IInterfaceType    tokenInterface,
                        out IInterfaceType    tokenBaseInterface,
                              IFieldMember    fullTokensCase,
                              IFieldMember [] fullTokensCases,
                  IPropertySignatureMember    fullTokensProperty,
                  IPropertySignatureMember [] fullTokensProperties,
                                   out int    setCount,
                       out EnumFieldLookup    enumToCaseLookup,
                    out EnumFieldRefLookup    enumAllFieldLookup,
                    out EnumFieldRefLookup    enumNoneFieldLookup,
                            out IClassType [] subsetTokenClasses,
 Dictionary<IEnumeratorType, IMethodMember>   setAdder,
                                IClassType    scannerTokenSetData,
                           IEnumeratorType    scannerTokenSetDataSetIndices,
                          out IFieldMember [] scannerSetFields)
        {
            IEnumeratorType[] result = null;
            enumAllFieldLookup = new EnumFieldRefLookup();
            enumNoneFieldLookup = new EnumFieldRefLookup();
            var enumNoneFieldLookup2 = new EnumFieldLookup();
            var enumAllFieldLookup2 = new EnumFieldLookup();
            int itemCount = items.Count;
            const int INT32SET = 0;
            const int UINT32SET = 1;
            ITokenItem[][] tokenSets = null;
            const int overflowPoint = 32;
            const int highestSetSize = 32;
            if (itemCount > overflowPoint)
            {
                setCount = (int)Math.Ceiling((double)items.Count / (double)highestSetSize);
                tokenSets = new ITokenItem[setCount][];
                /* *
                 * Neatly divide the sets into overflowPoint-element lists.
                 * *
                 * ToDo: On different architectures, such as 64-bit,
                 * target 64-bit via altering set constant.
                 * */
                for (int i = 0; i < setCount; i++)
                {
                    int setOffset = i * highestSetSize;
                    int currentSetSize = 0;
                    if (itemCount - setOffset < highestSetSize)
                        currentSetSize = itemCount - setOffset;
                    else
                        currentSetSize = highestSetSize;
                    tokenSets[i] = new ITokenItem[currentSetSize];
                    for (int j = 0; j < currentSetSize; j++)
                        tokenSets[i][j] = items[setOffset + j];
                }
            }
            else
            {
                tokenSets = new ITokenItem[1][] { new ITokenItem[itemCount] };
                for (int i = 0; i < items.Count; i++)
                    tokenSets[0][i] = items[i];
                setCount = 1;
            }
            IClassType[] tokenBaseTypes = new IClassType[setCount];
            result = new IEnumeratorType[setCount > 1 ? setCount + 1 : 1];
            scannerSetFields = new IFieldMember[setCount];
            if (setCount > 1)
                enumToCaseLookup = new EnumFieldLookup();
            else
                enumToCaseLookup = null;
            /* *
             * Now that the set grouping is done, focus on the more useful logic.
             * *
             * Individually creating the elements for the exit-states for the 
             * enumeration element and the field members associated to its 
             * enumerator.
             * */
            string enumNamePattern = "{0}Case";
            string enumNamePattern2 = "{1}{0}{2}";
            List<IFieldMember> caseFieldLookup = new List<IFieldMember>();
            if (setCount > 1)
            {
                string currentEnumerationName = null;
                if (target.Name.EndsWith("s"))
                    currentEnumerationName = string.Format(enumNamePattern + "s", target.Name.Substring(0, target.Name.Length - 1));
                else
                    currentEnumerationName = string.Format(enumNamePattern + "s", target.Name);
                var enumCases = result[setCount] = targetNamespace.Partials.AddNew().Enumerators.AddNew(currentEnumerationName);
                
                enumCases.Attributes.AddNew(typeof(FlagsAttribute).GetTypeReference());
                var noneField = enumCases.Fields.AddNew("UNNAMED\u0F0F", 0);
                noneField.Summary = "Represents a case where no sets are selected.";
                enumNamePattern += "{1}";
                enumNamePattern2 = "{2}{0}Set{1}{3}";
                enumCases.AccessLevel = DeclarationAccessLevel.Public;
                for (int i = 0; i < setCount; i++)
                {
                    caseFieldLookup.Add(enumCases.Fields.AddNew(string.Format("Case{0}", i + 1), (int)Math.Pow(2, i)));
                    caseFieldLookup[caseFieldLookup.Count - 1].Summary = string.Format("Set {0} in the {1} series.", i + 1, target.Name);
                }
                var allField = enumCases.Fields.AddNew("UNNAMED\u0F0E", (int)(Math.Pow(2, setCount) - 1));
                allField.Summary = string.Format("Represents all sets associated to the {0} token.", target.Name);
                enumAllFieldLookup.Add(result[setCount], allField.GetReference());
                enumAllFieldLookup2.Add(result[setCount], allField);
                enumNoneFieldLookup2.Add(result[setCount], noneField);
                enumNoneFieldLookup.Add(result[setCount], noneField.GetReference());
            }
            else
                enumNamePattern += "s";
            string p = target.Name;
            if (p.EndsWith("s"))
                p = p.Substring(0, p.Length - 1).ToLower();
            bool startsWithVowel = false;
            //Picky author.
            string[] vowels = new string[] { "a", "e", "i", "observedIndex", "u" };
            foreach (var vowel in vowels)
                if (p.StartsWith(vowel))
                    startsWithVowel = true;
            int baseValue = 0;
            /* *
             * None exit state given a temp name to avoid an element named
             * 'None' conflicting.
             * */
            var noneExitState = stateMachineCases.Fields.AddNew("UNNAMED\u0F0F");
            /* *
             * Iterate through the sets, create an enumeration for each.
             * *
             * For every element in the set, create a field for the member.
             * Associate this back to the information given by the path-graph 
             * created earlier.
             * */
            IClassType currentBaseToken = targetNamespace.Partials.AddNew().Classes.AddNew(string.Format("{0}{1}{2}{3}", source.Options.TokenPrefix, target.Name, source.Options.TokenSuffix, setCount > 1 ? "Base" : String.Empty));
            var baseToString = currentBaseToken.Methods.AddNew(new TypedName("ToString", typeof(string)));
            currentBaseToken.ImplementsList.Add(tokenInterface);
            IInterfaceType currentBaseTokenInterface = targetNamespace.Partials.AddNew().Interfaces.AddNew(string.Format("I{0}{1}{2}", source.Options.TokenPrefix, target.Name, source.Options.TokenSuffix));
            tokenBaseInterface = currentBaseTokenInterface;
            currentBaseTokenInterface.ImplementsList.Add(tokenInterface);
            currentBaseTokenInterface.AccessLevel = DeclarationAccessLevel.Public;
            currentBaseToken.ImplementsList.Add(currentBaseTokenInterface);
            currentBaseToken.AccessLevel = DeclarationAccessLevel.Internal;
            var tokenSelectorProperty = currentBaseToken.Properties.AddNew(new TypedName(fullTokensProperty.Name, fullTokensProperty.PropertyType), true, false);
            tokenSelectorProperty.AccessLevel = DeclarationAccessLevel.Public;
            tokenSelectorProperty.GetPart.Return(fullTokensCase.GetReference());
            tokenSelectorProperty.Summary = "Returns the token identifier with respect to the overall series.";
            tokenSelectorProperty.Remarks = string.Format("Returns {0}", fullTokensCase.GetReference().GetReferenceParticle().BuildCommentBody(ctoCommon));
            foreach (var otherSubsetProperty in fullTokensProperties)
            {
                tokenSelectorProperty = currentBaseToken.Properties.AddNew(new TypedName(otherSubsetProperty.Name, otherSubsetProperty.PropertyType), true, false);
                var targetRef=(from o in fullTokensCases
                               where o.ParentTarget == otherSubsetProperty.PropertyType.TypeInstance
                               select o).FirstOrDefault().GetReference();
                tokenSelectorProperty.GetPart.Return((from o in fullTokensCases
                                                      where o.ParentTarget == otherSubsetProperty.PropertyType.TypeInstance
                                                      select o).FirstOrDefault().GetReference());
                tokenSelectorProperty.AccessLevel = DeclarationAccessLevel.Public;
                tokenSelectorProperty.Summary = "Returns the token identifier with respect to the overall subset series.";
                tokenSelectorProperty.Remarks = string.Format("Returns {0}.", targetRef.GetReferenceParticle().BuildCommentBody(ctoCommon));
            }
            IPropertyMember subSetCase = null;
            ISwitchStatement toStringSwitch = null;
            if (setCount > 1)
            {
                subSetCase = currentBaseToken.Properties.AddNew(new TypedName(string.Format("{0}Case", target.Name), result[setCount]), true, false);
                subSetCase.IsAbstract = true;
                subSetCase.AccessLevel = DeclarationAccessLevel.Public;
                subSetCase.Summary = string.Format("Returns the subset for the {0} token.", target.Name);
                toStringSwitch = baseToString.SelectCase(subSetCase.GetReference());
                baseToString.Return(new PrimitiveExpression(string.Format("{0} (None)", target.Name)));
            }
            baseToString.AccessLevel = DeclarationAccessLevel.Public;
            baseToString.Overrides = true;
            for (int i = 0; i < setCount; i++)
            {
                IClassType currentSubSetToken = null;
                if (setCount > 1)
                {
                    if (setCount > 1 ^ target.Name.EndsWith("s"))
                        tokenBaseTypes[i] = targetNamespace.Partials.AddNew().Classes.AddNew(string.Format(enumNamePattern2, target.Name.Substring(0, target.Name.Length - 1), i + 1, source.Options.TokenPrefix, source.Options.TokenSuffix));
                    else
                        tokenBaseTypes[i] = targetNamespace.Partials.AddNew().Classes.AddNew(string.Format(enumNamePattern2, target.Name, i + 1, source.Options.TokenPrefix, source.Options.TokenSuffix));
                    currentSubSetToken = tokenBaseTypes[i];
                    currentSubSetToken.AccessLevel = DeclarationAccessLevel.Internal;
                    currentBaseToken.IsAbstract = true;
                    currentSubSetToken.BaseType = currentBaseToken.GetTypeReference(); ;
                }
                else
                    currentSubSetToken = currentBaseToken;
                IPropertyMember currentTokenSubSetValue = null;
                var currentSet = tokenSets[i];
                int currentSetType = INT32SET;
                if (currentSet.Length > overflowPoint)
                    currentSetType = UINT32SET;
                string currentEnumerationName = null;
                if (setCount > 1 ^ target.Name.EndsWith("s"))
                    currentEnumerationName = string.Format(enumNamePattern, target.Name.Substring(0, target.Name.Length - 1), i + 1);
                else
                    currentEnumerationName = string.Format(enumNamePattern, target.Name, i + 1);
                var currentDataSetEnumField = scannerTokenSetDataSetIndices.Fields.AddNew(currentEnumerationName);
                var currentEnumeration = targetNamespace.Partials.AddNew().Enumerators.AddNew(currentEnumerationName);
                var currentAdder = scannerTokenSetData.Methods.AddNew(new TypedName(string.Format("Add{0}", target.Name), typeof(void)));
                var currentAdderParam = currentAdder.Parameters.AddNew(new TypedName(target.Name, currentEnumeration));
                //var currentDataSetDataField = scannerTokenSetData.Fields.AddNew(new TypedName(currentEnumerationName, typeof(List<>).GetTypeReference(new TypeReferenceCollection(currentEnumeration.GetTypeReference()))));
                scannerSetFields[i] = currentDataSetEnumField;

                setAdder.Add(currentEnumeration, currentAdder);                
                currentEnumeration.AccessLevel = DeclarationAccessLevel.Public;
                result[i] = currentEnumeration;
#if WIN64
                if (currentSetType == UINT32SET)
                    currentEnumeration.BaseType = EnumeratorBaseType.ULong;
                else 
#endif
                if (currentSet.Length == overflowPoint)
                    currentEnumeration.BaseType = EnumeratorBaseType.UInt;
                else
                    currentEnumeration.BaseType = EnumeratorBaseType.SInt;
                currentEnumeration.Attributes.AddNew(typeof(FlagsAttribute).GetTypeReference());
                var noneField = currentEnumeration.Fields.AddNew("UNNAMED\u0F0F", 0);
                noneField.Summary = "Represents a case where no elements in the current set are selected.";
                enumNoneFieldLookup2.Add(currentEnumeration, noneField);
                enumNoneFieldLookup.Add(currentEnumeration, noneField.GetReference());
                IPropertySignatureMember currentBaseSubSetValueSig = null;
                if (setCount > 1)
                {
                    var currentBaseSubSetValue = currentBaseToken.Properties.AddNew(new TypedName(string.Format("{0}Set{1}Case", target.Name, i + 1), currentEnumeration), true, false);
                    currentBaseSubSetValueSig = currentBaseTokenInterface.Properties.AddNew(new TypedName(string.Format("{0}Set{1}Case", target.Name, i + 1), currentEnumeration), true, false);

                    currentBaseSubSetValue.IsVirtual = true;
                    currentBaseSubSetValue.GetPart.Return(enumNoneFieldLookup[currentEnumeration]);
                    currentBaseSubSetValue.AccessLevel = DeclarationAccessLevel.Public;
                    currentTokenSubSetValue = currentSubSetToken.Properties.AddNew(new TypedName(string.Format("{0}Set{1}Case", target.Name, i + 1), currentEnumeration), true, false);
                    currentTokenSubSetValue.Overrides = true;
                    enumToCaseLookup.Add(currentEnumeration, caseFieldLookup[i]);
                    var currentSubSetCase = currentSubSetToken.Properties.AddNew(new TypedName(subSetCase.Name, subSetCase.PropertyType), true, false);
                    currentSubSetCase.AccessLevel = DeclarationAccessLevel.Public;
                    currentSubSetCase.GetPart.Return(caseFieldLookup[i].GetReference());
                    currentSubSetCase.Overrides = true;
                    currentSubSetCase.Summary = string.Format("Returns the subset case in the {0} token.", target.Name, i + 1);
                    currentSubSetCase.Remarks = string.Format("Returns {0}.", caseFieldLookup[i].GetReference().GetReferenceParticle().BuildCommentBody(ctoCommon));
                    var currentCase = toStringSwitch.Cases.AddNew(caseFieldLookup[i].GetReference());
                    currentCase.Return(typeof(string).GetTypeReferenceExpression().GetMethod("Format").Invoke(new PrimitiveExpression(string.Format("{0} ({{0}})", target.Name)), currentBaseSubSetValue.GetReference()));
                }
                else
                {
                    currentBaseSubSetValueSig = currentBaseTokenInterface.Properties.AddNew(new TypedName(string.Format("{0}Case", target.Name, i + 1), currentEnumeration), true, false);
                    currentTokenSubSetValue = currentSubSetToken.Properties.AddNew(new TypedName(string.Format("{0}Case", target.Name), currentEnumeration), true, false);
                    baseToString.Return(typeof(string).GetTypeReferenceExpression().GetMethod("Format").Invoke(new PrimitiveExpression(string.Format("{0} ({{0}})", target.Name)), currentTokenSubSetValue.GetReference()));
                }
                currentBaseSubSetValueSig.Summary = string.Format("Returns the {2} {0}identifier for the current token{1}.", setCount > 1 ? "sub-set " : string.Empty, setCount > 1 ? string.Format(" in set {0}", i + 1) : string.Empty, target.Name);
                currentTokenSubSetValue.Summary = string.Format("Returns the {2} {0}identifier for the current token{1}.", setCount > 1 ? "sub-set " : string.Empty, setCount > 1 ? string.Format(" in set {0}", i + 1) : string.Empty, target.Name);
                currentTokenSubSetValue.AccessLevel = DeclarationAccessLevel.Public;
                IFieldMember setSelector = currentSubSetToken.Fields.AddNew(new TypedName("value", currentEnumeration));
                currentTokenSubSetValue.GetPart.Return(setSelector.GetReference());
                IConstructorMember subSetCtor = currentSubSetToken.Constructors.AddNew();
                IConstructorParameterMember subSetCtorValueParameter = subSetCtor.Parameters.AddNew(new TypedName("value", currentEnumeration));
                subSetCtor.Statements.Assign(setSelector.GetReference(), subSetCtorValueParameter.GetReference());

                for (int j = 0; j < currentSet.Length; j++)
                {
                    var currentItem = currentSet[j];
                    ulong value = (ulong)Math.Pow(2, j);

                    var enumFieldMember = currentEnumeration.Fields.AddNew(currentItem.Name);
                    /* *
                     * OIL is type-sensitive, so if you give it a long, it writes the 'L' at the end
                     * for enforcing strict typing on the values you give it.
                     * *
                     * But it's not smart enough to know when you're giving it a value
                     * that's a ulong, but the containing enum isn't as large as a ulong.
                     * */
#if WIN64
                    if (currentSetType == UINT32SET)
                        enumFieldMember.InitializationExpression = new PrimitiveExpression(value);
                    else 
#endif
                    if (j == overflowPoint - 1 && currentSet.Length == overflowPoint)
                        enumFieldMember.InitializationExpression = new PrimitiveExpression((uint)value);
                    else
                        enumFieldMember.InitializationExpression = new PrimitiveExpression((int)value);
                    ILiteralTokenItem q = (ILiteralTokenItem)currentItem;
                    enumFieldMember.Summary = string.Format(
                        "<para>Defines a{4} {0} in the {1} lexer that is '{2}' " +
                        "characters long.</para>\r\n<para>Actual value: {3}</para>",
                        p, source.Options.LexerName, q.Value.ToString().Length,
                        q.Value, startsWithVowel ? "n" : string.Empty);
                    enumFieldMember.Remarks = string.Format(
                        "<para>Original definition: {0}</para>\r\n<para>Defined in " +
                        "\"{1}\" on line {2}.</para>",
                        q.ToString(), Path.GetFileName(target.FileName), q.Line);

                    EnumStateMachineData currentElement = null;
                    var currentTransition = graph.GetTransitionFor(currentItem);
                    bool mode1 = true;
                    foreach (var item in currentTransition.TargetStates)
                        if (item.Sources.First() != currentItem)
                        {
                            mode1 = false;
                            break;
                        }
                    if (mode1)
                        currentElement = new EnumStateMachineData(currentItem, currentTransition, stateMachineCases, ref baseValue, enumFieldMember);
                    else
                    {
                        Dictionary<RegularLanguageState, IFieldMember> baseStates = new Dictionary<RegularLanguageState, IFieldMember>();
                        /* *
                         * Iterate through the transition graph's target states for the 
                         * current element, if it's the transitionFirst element in the source 
                         * listing for the target then it'll create the enumeration member 
                         * itself; otherwise, look up the field member from the previously 
                         * created elements.
                         * *
                         * Further, this *should* always work, elements are created by the order
                         * they're defined in the file, state reduction on overlapping elements
                         * yields the members created transitionFirst being the state that retains its
                         * ordinal source index of '0'.  Thus, they're always transitionFirst, and are 
                         * therefore always going to create the enumeration entities prior to this
                         * logic branch being executed.
                         * */
                        foreach (var sourceState in currentTransition.TargetStates)
                        {
                            var first = sourceState.Sources.First();
                            if (first == currentItem)
                                continue;
                            else
                                baseStates.Add(sourceState, dataSet[first].ExitStateConstants[sourceState]);
                        }
                        currentElement = new EnumStateMachineData(currentItem, currentTransition, stateMachineCases, ref baseValue, enumFieldMember, baseStates);
                    }
                    dataSet.Add(currentItem, currentElement);
                }
                IFieldMember currentAllMember = null;
                if (currentSetType == UINT32SET)
                    if (currentSet.Length == highestSetSize)
                        currentAllMember = currentEnumeration.Fields.AddNew(currentEnumeration.Fields.GetUniqueName("UNNAMED\u0F0E"), ulong.MaxValue);
                    else
                        currentAllMember = currentEnumeration.Fields.AddNew(currentEnumeration.Fields.GetUniqueName("UNNAMED\u0F0E"), (ulong)((Math.Pow(2, currentSet.Length)) - 1));
                else if (currentSet.Length == overflowPoint)
                    currentAllMember = currentEnumeration.Fields.AddNew(currentEnumeration.Fields.GetUniqueName("UNNAMED\u0F0E"), (uint)((Math.Pow(2, currentSet.Length)) - 1));
                else
                    currentAllMember = currentEnumeration.Fields.AddNew(currentEnumeration.Fields.GetUniqueName("UNNAMED\u0F0E"), (int)((Math.Pow(2, currentSet.Length)) - 1));
                enumAllFieldLookup2.Add(currentEnumeration, currentAllMember);
                currentAllMember.Summary = string.Format("Represents all the members of the current set of values for {0}.", target.Name);
                enumAllFieldLookup.Add(currentEnumeration, currentAllMember.GetReference());
            }
            
            foreach (var key in enumNoneFieldLookup2.Keys)
            {
                enumNoneFieldLookup2[key].Name = key.Fields.GetUniqueName("None");
                enumAllFieldLookup2[key].Name = key.Fields.GetUniqueName("All");
            }
            noneExitState.Name = stateMachineCases.Fields.GetUniqueName("None");
            subsetTokenClasses = tokenBaseTypes;
            return result;
        }

        #region Gathering Aspect

        public static List<ITokenItem> GatherEnumItems(ITokenEntry target)
        {
            List<ITokenItem> result = new List<ITokenItem>();
            GatherEnumItems(target.Branches, result);
            return result;
        }

        public static void GatherEnumItems(ITokenExpressionSeries target, List<ITokenItem> result)
        {
            foreach (var item in target)
                GatherEnumItems(item, result);
        }

        public static void GatherEnumItems(ITokenExpression target, List<ITokenItem> result)
        {
            foreach (var item in target)
                GatherEnumItems(item, result);
        }

        public static void GatherEnumItems(ITokenItem target, List<ITokenItem> result)
        {
            if (target is ILiteralTokenItem)
                result.Add(target);
            else if (target is ILiteralReferenceTokenItem)
                result.Add(((ILiteralReferenceTokenItem)target).Literal);
            else if (target is ITokenReferenceTokenItem)
                GatherEnumItems(((ITokenReferenceTokenItem)target).Reference.Branches, result);
            else if (target is ITokenGroupItem)
                GatherEnumItems((ITokenExpressionSeries)(target), result);
        }

        #endregion

        #region Discernment Aspect
        public static StateMachineType DiscernType(ITokenEntry target)
        {
            if (target is ITokenEofEntry)
                return StateMachineType.Capture;
            //return StateMachineType.Capture;
            return DiscernType(target.Branches);
        }

        public static StateMachineType DiscernType(ITokenExpressionSeries target)
        {
            if (target.Count == 0)
                return StateMachineType.Enumeration;
            foreach (var item in target)
                if (DiscernType(item) == StateMachineType.Capture)
                    return StateMachineType.Capture;
            return StateMachineType.Enumeration;
        }

        public static StateMachineType DiscernType(ITokenExpression target)
        {
            if (target.Count == 0)
                return StateMachineType.Enumeration;
            foreach (var item in target)
                if (DiscernType(item) == StateMachineType.Capture)
                    return StateMachineType.Capture;
            return StateMachineType.Enumeration;
        }

        public static StateMachineType DiscernType(ITokenItem target)
        {
            if (target is ILiteralTokenItem ||
                target is ILiteralReferenceTokenItem)
                return target.RepeatOptions == ScannableEntryItemRepeatOptions.None ? StateMachineType.Enumeration :
                       StateMachineType.Capture;
            else if (target is ITokenReferenceTokenItem)
                return target.RepeatOptions == ScannableEntryItemRepeatOptions.None ? DiscernType(((ITokenReferenceTokenItem)(target)).Reference) :
                       StateMachineType.Capture;
            else if (target is ITokenGroupItem)
                return target.RepeatOptions == ScannableEntryItemRepeatOptions.None ? DiscernType(((ITokenExpressionSeries)(target))) : 
                       StateMachineType.Capture;
            return StateMachineType.Capture;
        }

        #endregion

        /* *
         * Creates the state-machine necessary for a capture-type state-machine.
         * *
         * Typically important for non-fixed data-sets.  Such as identifiers
         * strings, numbers, comments, and so on.
         * */
        private static TokenFinalData CreateCaptureStateMachine(ITokenEntry entry, RegularLanguageState state, IClassType tokenStateMachine, CharStreamClass captureType, IClassType scannerTokenSetData, IClassType scannerEntry)
        {
            state = RegularLanguageState.Merger.ToDFA(state, true);
            state.Enumerate();
            var states = ObtainFlatStates(state);
            var nextMethod = tokenStateMachine.Methods.AddNew(new TypedName("Next", typeof(bool)));
            nextMethod.AccessLevel = DeclarationAccessLevel.Public;
            var charParameter = nextMethod.Parameters.AddNew(new TypedName("char", typeof(char)));
            var stateField = tokenStateMachine.Fields.AddNew(new TypedName("state", typeof(int)));
            //var lengthField = tokenStateMachine.Fields.AddNew(new TypedName("length", typeof(int)));
            var charParameterRef = charParameter.GetReference();
            var stateFieldRef = stateField.GetReference();
            //var lengthFieldRef = lengthField.GetReference();
            var exitStateField = tokenStateMachine.Fields.AddNew(new TypedName("exitState", typeof(bool)));
            var exitLengthField = tokenStateMachine.Fields.AddNew(new TypedName("exitlength", typeof(int)));
            var exitStateFieldRef = exitStateField.GetReference();
            var exitLengthFieldRef = exitLengthField.GetReference();
            var getCaptureMethod = tokenStateMachine.Methods.AddNew(new TypedName("GetCapture", typeof(string)));
            if (state.IsEdge())
                tokenStateMachine.Fields.Remove(exitStateField);

            #region GetCaptureMethod
            getCaptureMethod.AccessLevel = DeclarationAccessLevel.Public;
            //char[] result = new char[this.actualSize];
            var resultChars = getCaptureMethod.Locals.AddNew(new TypedName("result", typeof(char[])), new CreateArrayExpression(typeof(char).GetTypeReference(), exitLengthFieldRef));
            var iLocal = getCaptureMethod.Locals.AddNew(new TypedName("i", typeof(int)));
            //int i = 0;
            iLocal.InitializationExpression = PrimitiveExpression.NumberZero;
            //So it isn't declared in the main body.
            iLocal.AutoDeclare = false;
            //i++
            ICrementStatement icrement = new CrementStatement(CrementType.Postfix, CrementOperation.Increment, iLocal.GetReference());
            //for (int i = 0; i < this.actualSize; i++)
            var loop = getCaptureMethod.Iterate(iLocal.GetDeclarationStatement(), icrement, new BinaryOperationExpression(iLocal.GetReference(), CodeBinaryOperatorType.LessThan, exitLengthFieldRef));
            //    result[i] = this.buffer[i];
            loop.Assign(resultChars.GetReference().GetIndex(iLocal.GetReference()), captureType.Buffer.GetReference().GetIndex(iLocal.GetReference()));
            //return new string(result);
            getCaptureMethod.Return(new CreateNewObjectExpression(typeof(string).GetTypeReference(), resultChars.GetReference()));
            #endregion

            stateField.Summary = "Data member which tracks the current execution path of the state machine.";
            exitStateField.Summary = "Data member which denotes the final execution point of the state machine.";
            exitStateField.Remarks = "Only set when an edge in the state machine is hit.";
            var isValidState = tokenStateMachine.Properties.AddNew(new TypedName("IsValidEndState", typeof(bool)), true, false);
            isValidState.AccessLevel = DeclarationAccessLevel.Public;
            if (!state.IsEdge())
                isValidState.GetPart.Return(exitStateFieldRef);
            else
                isValidState.GetPart.Return(PrimitiveExpression.TrueValue);
            var bytesConsumedProperty = tokenStateMachine.Properties.AddNew(new TypedName("BytesConsumed", typeof(int)), true, false);
            var logicPoints = new Dictionary<RegularLanguageState, LabelStatement>();
            var resetMethod = tokenStateMachine.Methods.AddNew(new TypedName("Reset", typeof(void)));
            resetMethod.Assign(exitLengthFieldRef, PrimitiveExpression.NumberZero);
            resetMethod.CallMethod(captureType.PurgeMethod.GetReference().Invoke());
            //resetMethod.Assign(lengthFieldRef, PrimitiveExpression.NumberZero);
            if (!state.IsEdge())
                resetMethod.Assign(exitStateFieldRef, PrimitiveExpression.FalseValue);
            //else
            //    resetMethod.Assign(exitStateFieldRef, PrimitiveExpression.TrueValue);
            resetMethod.Assign(stateFieldRef, PrimitiveExpression.NumberZero);
            resetMethod.AccessLevel = DeclarationAccessLevel.Public;
            bytesConsumedProperty.GetPart.Return(exitLengthFieldRef);
            bytesConsumedProperty.AccessLevel = DeclarationAccessLevel.Public;
            PrimitiveStateLookup inverseStates = states.ObtainReverseLookup();
            var nextStateSwitch = nextMethod.SelectCase(stateFieldRef);
            var logicInserted = new RegularLanguageStateSeries();
            //Return before the labels are added.
            nextMethod.Return(PrimitiveExpression.FalseValue);
            LabelStatement termExit = null;
            LabelStatement normExit = null;
            LabelStatement standardMove = null;

            bool addedExitCommon = false;
            bool addedMoveCommon = false;
            bool addedTermExitCommon = false;
            var stateQuery = from p in inverseStates
                             orderby p.Key.Value
                             select p;
            foreach (var sPrim in stateQuery)
            {
                //First state almost never has logic.
                if (sPrim.Value.StateValue == 0 && sPrim.Value.InTransitions.Count() == 0)
                    continue;
                var currentLabel = new LabelStatement(string.Format("MOVETOSTATE{0}", sPrim.Value.StateValue));
                logicPoints.Add(sPrim.Value, currentLabel);
                nextMethod.Statements.Add(currentLabel);
            }
            foreach (var sPrim in stateQuery)
            /*from p in inverseStates
                              orderby p.Key.Value
                              select p*/
            {
                if (sPrim.Value.OutTransitions.Count == 0)
                    continue;
                var currentStateCase = nextStateSwitch.Cases.AddNew(sPrim.Key);
                RegularLanguageBitArray currentRange = null;
                //Explore the range of the current element.
                foreach (var transition in sPrim.Value.OutTransitions)
                    if (currentRange == null)
                        currentRange = transition.Check;
                    else
                        currentRange |= transition.Check;
                var trueCount = currentRange.CountTrue();
                /* *
                 * Determine the best method for export.
                 * */
                foreach (var transition in sPrim.Value.OutTransitions)
                {
                    var curKey = transition.Check;
                    RegularLanguageBitArray.RangeData rangeInfo = curKey.GetRange();
                    IExpression fullCase = null;
                    foreach (RegularLanguageBitArray.RangeSet set in rangeInfo.Sets)
                    {
                        var lower = BOp(set.Start, Operators.LessThanOrEqual, (IExpression)charParameterRef);
                        var upper = BOp((IExpression)charParameterRef, Operators.LessThanOrEqual, set.End);
                        var currentCase = BOp((IExpression)lower, Operators.BooleanAnd, (IExpression)upper);
                        if (fullCase == null)
                            fullCase = currentCase;
                        else
                            fullCase = BOp(fullCase, Operators.BooleanOr, (IExpression)currentCase);
                    }
                    foreach (char c in rangeInfo.Singletons)
                    {
                        var currentCase = BOp(c, Operators.IdentityEquality, (IExpression)charParameterRef);
                        if (fullCase == null)
                            fullCase = currentCase;
                        else
                            fullCase = BOp(fullCase, Operators.BooleanOr, (IExpression)currentCase);
                    }
                    var currentValue = transition.Targets[0];
                    var currentLabel = logicPoints[currentValue];
                    CheckLogicStatus(state, states, nextMethod, stateFieldRef, exitStateFieldRef, exitLengthFieldRef, charParameterRef, logicInserted, sPrim, currentValue, currentLabel, ref addedExitCommon, ref addedTermExitCommon, ref termExit, ref normExit, ref addedMoveCommon, ref standardMove, captureType);
                    var currentSelector = currentStateCase.IfThen(fullCase);
                    if (currentValue != sPrim.Value)
                        currentSelector.Statements.Add(currentLabel.GetGoTo(currentSelector.Statements));
                    else if (currentValue.IsEdge())
                        currentSelector.Statements.Add(normExit.GetGoTo(currentSelector.Statements));
                    else
                        currentSelector.Statements.Add(standardMove.GetGoTo(currentSelector.Statements));
                }
            }
            return new TokenCaptureFinalData(entry, tokenStateMachine, state, scannerEntry);
        }

        public static BinaryOperationExpression BOp(IExpression left, Operators op, IExpression right)
        {
            return new BinaryOperationExpression(left, op, right);
        }


        public static BinaryOperationExpression BOp<TLeft>(TLeft left, Operators op, IExpression right)
            where TLeft :
                struct
        {
            return new BinaryOperationExpression(new PrimitiveExpression(left), op, right);
        }

        public static BinaryOperationExpression BOp<TRight>(IExpression left, Operators op, TRight right)
            where TRight :
                struct
        {
            return new BinaryOperationExpression(left, op, new PrimitiveExpression(right));
        }

        private static void CheckLogicStatus(
                       RegularLanguageState rootState,
                       StatePrimitiveLookup states, 
                              IMethodMember nextMethod, 
                  IFieldReferenceExpression stateFieldRef, 
                  IFieldReferenceExpression exitStateFieldRef, 
                  IFieldReferenceExpression exitLengthFieldRef, 
               IVariableReferenceExpression currentChar,
                 RegularLanguageStateSeries logicInserted,
                         PrimitiveStatePair sPrim, 
                       RegularLanguageState currentValue, 
                             LabelStatement currentLabel, 
                                   ref bool insertedExitLogic, 
                                   ref bool insertedTermExitLogic, 
                         ref LabelStatement termExit, 
                         ref LabelStatement exitNorm, 
                                   ref bool addedCommonMove, 
                         ref LabelStatement commonMove,
                            CharStreamClass bitStream)
        {
            if (!logicInserted.Contains(currentValue))
            {
                int index = nextMethod.Statements.IndexOf(currentLabel);
                if (currentValue != sPrim.Value)
                    nextMethod.Statements.Insert(++index, new AssignStatement(stateFieldRef, states[currentValue]));
                if (currentValue.IsEdge())
                {
                    if (currentValue != sPrim.Value && !rootState.IsEdge())
                        nextMethod.Statements.Insert(++index, new AssignStatement(exitStateFieldRef, PrimitiveExpression.TrueValue));
                    if (currentValue.OutTransitions.Count() == 0)
                    {
                        if (!insertedTermExitLogic)
                        {
                            termExit = new LabelStatement("TERMINAL_EXIT");
                            nextMethod.Statements.Add(termExit);
                            int loc = nextMethod.Statements.IndexOf(termExit);
                            nextMethod.Statements.Insert(++loc, new SimpleStatement(bitStream.PushCharMethod.GetReference().Invoke(currentChar)));
                            nextMethod.Statements.Insert(++loc, new AssignStatement(exitLengthFieldRef, bitStream.ActualSize.GetReference()));
                            nextMethod.Statements.Insert(++loc, new ReturnStatement(PrimitiveExpression.FalseValue));
                            insertedTermExitLogic = true;
                        }
                        nextMethod.Statements.Insert(++index, termExit.GetGoTo(nextMethod.Statements));
                    }
                    else
                    {
                        if (!insertedExitLogic)
                        {
                            exitNorm = new LabelStatement("NOMINAL_EXIT");
                            nextMethod.Statements.Add(exitNorm);
                            int loc = nextMethod.Statements.IndexOf(exitNorm);
                            //nextMethod.Statements.Insert(++loc, new AssignStatement(exitLengthFieldRef, new UnaryOperationExpression(UnaryOperations.PrefixIncrement, lengthFieldRef)));
                            nextMethod.Statements.Insert(++loc, new SimpleStatement(bitStream.PushCharMethod.GetReference().Invoke(currentChar)));
                            nextMethod.Statements.Insert(++loc, new AssignStatement(exitLengthFieldRef, bitStream.ActualSize.GetReference()));
                            nextMethod.Statements.Insert(++loc, new ReturnStatement(PrimitiveExpression.TrueValue));
                            insertedExitLogic = true;
                        }
                        nextMethod.Statements.Insert(++index, exitNorm.GetGoTo(nextMethod.Statements));
                    }
                }
                else 
                {
                    if (!addedCommonMove)
                    {
                        commonMove = new LabelStatement("COMMON_STATEMOVE");
                        nextMethod.Statements.Add(commonMove);
                        int loc = nextMethod.Statements.IndexOf(commonMove);
                        //nextMethod.Statements.Insert(++loc, new CrementStatement(CrementType.Prefix, CrementOperation.Increment, lengthFieldRef));
                        nextMethod.Statements.Insert(++loc, new SimpleStatement(bitStream.PushCharMethod.GetReference().Invoke(currentChar)));
                        nextMethod.Statements.Insert(++loc, new ReturnStatement(PrimitiveExpression.TrueValue));
                        addedCommonMove = true;
                    }
                    nextMethod.Statements.Insert(++index, commonMove.GetGoTo(nextMethod.Statements));

                }
                logicInserted.Add(currentValue);
            }
        }
        private static List<RegularLanguageState> flattenStatesCycle = new List<RegularLanguageState>();

        internal static StatePrimitiveLookup ObtainFlatStates(RegularLanguageState initialState)
        {
            StatePrimitiveLookup r = new StatePrimitiveLookup();
            FlattenStates(r, initialState);
            return r;
        }

        private static void FlattenStates(StatePrimitiveLookup target, RegularLanguageState state)
        {
            if (flattenStatesCycle.Contains(state))
                return;
            else
                flattenStatesCycle.Add(state);
            try
            {
                if (target.ContainsKey(state))
                    return;
                target.Add(state, new PrimitiveExpression((int)state.StateValue));
                foreach (var item in state.OutTransitions)
#if OLD_REGULAR_LANGUAGE_STATE
                    foreach (var subItem in item.Value)
#else
                    foreach (var subItem in item.Targets)
#endif
                        FlattenStates(target, subItem);
            }
            finally
            {
                if (flattenStatesCycle[0] == state)
                    flattenStatesCycle.Clear();
            }
        }

        public static string BitArrayToString(this BitArray chars, bool inverted)
        {
            bool inRange = false;
            BitArray used = new BitArray(chars);

            //if (inverted)
            //    used = used.Not();
            int rangeStart = -1;
            StringBuilder range = new StringBuilder();
            bool hitAny = false;
            for (int i = 0; i < chars.Length; i++)
            {
                if (!inRange)
                {
                    if (used[i])
                    {
                        if (!hitAny)
                        {
                            if (inverted)
                                range.Append("^");
                            hitAny = true;
                        }
                        if (used.Length <= i + 1)
                            inRange = false;
                        else
                            inRange = used[i + 1];
                        if (!inRange)
                            range.Append(GetCharacterString((char)i));
                        else
                            rangeStart = i;
                    }
                }
                else
                    if (!used[i] || i == chars.Length - 1)
                    {
                        inRange = false;
                        range.Append(GetCharacterString((char)rangeStart));
                        range.Append('-');
                        range.Append(GetCharacterString((char)(i - (i == chars.Length - 1 ? 0 : 1))));
                    }
            }
            if (!hitAny)
                if (inverted)
                    range.Append("ALL");
                else
                    range.Append("None");
            return string.Format("{0}", range.ToString());
        }

        public static int CountTrue(this BitArray target)
        {
            int c = 0;
            for (int i = 0; i < target.Length; i++)
                if (target[i])
                    c++;
            return c;
        }

        internal static string GetCharacterString(char c)
        {
            //Special escapes.
            if (c == (char)0x5E)
                return "\\x5E";
            else if (c == char.MinValue)
                return "\\0";
            else if (c == '\\')
                return @"\\";
            else if (c == '\'')
                return "\\'";
            else if (c == '\r')
                return "\\r";
            else if (c == '\t')
                return "\\t";
            else if (c == '\v')
                return "\\v";
            else if (c == '\f')
                return "\\f";
            else if (c == '\n')
                return "\\n";
            else if (c == (char)32)
                return "\\x20";
            else if (c == char.MinValue)
                return "\\0";
            else if (c > (char)0xCC && c < 256)
            {
                string s = string.Format("{0:X}", (int)c);
                while (s.Length < 2)
                    s = "0" + s;
                return string.Format("\\x{0}", s);
            }
            else if (c >= 256)
            {
                string s = string.Format("{0:X}", (int)c);
                while (s.Length < 4)
                    s = "0" + s;
                return string.Format("\\u{0}", s);
            }
            return c.ToString();
        }

        private static IEnumerable<T> ConcatinateSets<T>(this IEnumerable<IEnumerable<T>> target)
        {
            foreach (var set in target)
                foreach (var item in set)
                    yield return item;
            yield break;
        }

    }
}
