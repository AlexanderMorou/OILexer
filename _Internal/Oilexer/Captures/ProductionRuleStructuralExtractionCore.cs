using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Tuples;
using AllenCopeland.Abstraction.Utilities.Arrays;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if x64
using SlotType = System.UInt64;
#elif x86
#if HalfWord
using SlotType = System.UInt16;
#else
using SlotType = System.UInt32;
#endif
#endif

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal static class ProductionRuleStructuralExtractionCore
    {
        internal static IProductionRuleCaptureStructure BuildStructureFor(OilexerGrammarProductionRuleEntry entry, IOilexerGrammarFile source)
        {
            return BuildStructureFor(entry, entry, source);
        }

        private static IProductionRuleCaptureStructure BuildStructureFor(OilexerGrammarProductionRuleEntry entry, IProductionRuleSeries expressionSeries, IOilexerGrammarFile source)
        {
            IProductionRuleCaptureStructure result = null;
            HashList<HashList<string>> currentResultVariants = new HashList<HashList<string>>();
            foreach (var expression in expressionSeries)
            {
                var current = BuildStructureFor(entry, expressionSeries, expression, source);
                if (result == null)
                    result = current;
                else
                    result = result.Union(current);
                var dataSet = new HashList<string>(current.Keys);
                if (!currentResultVariants.Any(k => k.SequenceEqual(dataSet)))
                    currentResultVariants.Add(dataSet);
            }
            foreach (var variant in currentResultVariants)
                result.Structures.Add(variant);
            if (expressionSeries == entry)
                ((ControlledCollection<IProductionRuleSource>)result.Sources).baseList.Add(entry);
            result.ResultedTypeName = string.Format("{0}{1}{2}", source.Options.RulePrefix, entry.Name, source.Options.RuleSuffix);
            return result;
        }

        private static IProductionRuleCaptureStructure BuildStructureFor(OilexerGrammarProductionRuleEntry entry, IProductionRuleSeries expressionSeries, IProductionRule expression, IOilexerGrammarFile source)
        {
            IProductionRuleCaptureStructure result = new ProductionRuleCaptureStructure(entry);
            foreach (var item in expression)
            {
                var current = BuildStructureFor(entry, expressionSeries, expression, item, source);
                result = result.Concat(current);
            }
            return result;
        }

        private static IProductionRuleCaptureStructuralItem BuildStructureFor(OilexerGrammarProductionRuleEntry entry, IProductionRuleSeries expressionSeries, IProductionRule expression, IProductionRuleItem item, IOilexerGrammarFile source)
        {
            IProductionRuleCaptureStructuralItem result = null;
            if (item is IProductionRuleGroupItem)
            {
                var ruleGroup = ((IProductionRuleGroupItem)(item));
                if (!ruleGroup.Name.IsEmptyOrNull() && AllAreUnnamedEquivalents(ruleGroup))
                    SetAllNames(ruleGroup, ruleGroup.Name);
                result = BuildStructureFor(entry, ruleGroup, source);
                if (result.ResultType == ResultedDataType.None && !item.Name.IsEmptyOrNull())
                {
                    if (ruleGroup.Count == 1 && ruleGroup[0].Count == 1)
                    {
                        var singleItem = ruleGroup[0][0];
                        if (singleItem is ILiteralCharReferenceProductionRuleItem)
                        {
                            if (singleItem.RepeatOptions == ScannableEntryItemRepeatInfo.None && ruleGroup.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                                result.ResultType = ResultedDataType.Character;
                            else
                                result.ResultType = ResultedDataType.String;
                        }
                        else
                            result.ResultType = ResultedDataType.String;
                    }
                    else
                        result.ResultType = ResultedDataType.String;
                }
                else if (item.Name.IsEmptyOrNull())
                {
                    result.ResultType = ResultedDataType.PassThrough;
                }
                ((ControlledCollection<IProductionRuleSource>)(result.Sources)).baseList.Add(item);
            }
            else if (item is ILiteralReferenceProductionRuleItem)
            {
                var literalRefItem = (ILiteralReferenceProductionRuleItem)item;
                var inlinedRef = ((InlinedTokenEntry)(literalRefItem.Source));
                result = new ProductionRuleLiteralTokenItemReferenceStructuralItem(literalRefItem.Source, literalRefItem, entry);
                if (inlinedRef.CaptureKind == RegularCaptureType.Transducer)
                    result.ResultType = ResultedDataType.EnumerationItem;
                else if (inlinedRef.CaptureKind == RegularCaptureType.ContextfulTransducer)
                    result.ResultType = ResultedDataType.FlagEnumerationItem;
            }
            else if (item is ITokenReferenceProductionRuleItem)
            {
                var tokenRefItem = ((ITokenReferenceProductionRuleItem)(item));
                var inlinedRef = ((InlinedTokenEntry)(tokenRefItem.Reference));
                result = new ProductionRuleTokenReferenceStructuralItem((ITokenReferenceProductionRuleItem)item, entry);
                if (inlinedRef.CaptureKind == RegularCaptureType.Transducer)
                    result.ResultType = ResultedDataType.Enumeration;
                else
                    result.ResultType = ResultedDataType.ImportType;
            }
            else if (item is IRuleReferenceProductionRuleItem)
            {
                var ruleItem = (IRuleReferenceProductionRuleItem)item;
                result = new ProductionRuleCaptureReferenceStructuralItem(ruleItem, entry);
            }
            Deform(item, result, expression);

            return result;
        }

        private static bool AllAreUnnamedEquivalents(IProductionRuleGroupItem ruleGroup)
        {
            IProductionRuleItem rootItem = null;
            return AllAreUnnamedEquivalents((IProductionRuleSeries)ruleGroup, ref rootItem);
        }

        private static bool AllAreUnnamedEquivalents(IProductionRuleSeries ruleSeries, ref IProductionRuleItem rootItem)
        {
            foreach (var exp in ruleSeries)
                if (!AllAreUnnamedEquivalents(exp, ref rootItem))
                    return false;
            return true;
        }
        private static bool AllAreUnnamedEquivalents(IProductionRule rule, ref IProductionRuleItem rootItem)
        {
            foreach (var item in rule)
                if (!AllAreUnnamedEquivalents(item, ref rootItem))
                    return false;
            return true;
        }
        private static bool AllAreUnnamedEquivalents(IProductionRuleItem item, ref IProductionRuleItem rootItem)
        {
            if (item is IProductionRuleGroupItem)
                return AllAreUnnamedEquivalents((IProductionRuleSeries)item, ref rootItem);
            else
            {
                if (rootItem == null)
                {
                    rootItem = item;
                    return rootItem.Name.IsEmptyOrNull();
                }
                else if (item is ILiteralCharReferenceProductionRuleItem ||
                         item is ILiteralStringReferenceProductionRuleItem)
                    return item.Name.IsEmptyOrNull() && 
                           rootItem is ILiteralCharReferenceProductionRuleItem ||
                           rootItem is ILiteralStringReferenceProductionRuleItem;
                return item.GetType() == rootItem.GetType() && item.Name.IsEmptyOrNull();
            }
        }
        private static void SetAllNames(IProductionRuleGroupItem ruleGroup, string p)
        {
            Stack<IProductionRuleSeries> seriesToSet = new Stack<IProductionRuleSeries>();
            seriesToSet.Push(ruleGroup);
            while (seriesToSet.Count > 0)
            {
                var curSeries = seriesToSet.Pop();
                foreach (var rule in curSeries)
                {
                    foreach (var item in rule)
                    {
                        if (item is IProductionRuleGroupItem)
                            seriesToSet.Push((IProductionRuleGroupItem)item);
                        else
                            item.Name = p;
                    }
                }
            }
        }

        private static void Deform(IProductionRuleItem item, IProductionRuleCaptureStructuralItem structuralItem, IProductionRule owner)
        {
            /* *
             * Items reduced to their max are for recognizer sections..
             * */
            if (!(item is IProductionRuleGroupItem))
            {
                if ((item.RepeatOptions.Options & ScannableEntryItemRepeatOptions.MaxReduce) == ScannableEntryItemRepeatOptions.MaxReduce && string.IsNullOrEmpty(item.Name))
                {
                    return;
                }

                switch (item.RepeatOptions.Options & (ScannableEntryItemRepeatOptions.OneOrMore | ScannableEntryItemRepeatOptions.ZeroOrMore | ScannableEntryItemRepeatOptions.ZeroOrOne))
                {
                    case ScannableEntryItemRepeatOptions.ZeroOrOne:
                        structuralItem.Optional = true;
                        break;
                    case ScannableEntryItemRepeatOptions.ZeroOrMore:
                    case ScannableEntryItemRepeatOptions.OneOrMore:
                        switch (structuralItem.ResultType)
                        {
                            case ResultedDataType.ImportType:
                                structuralItem.ResultType = ResultedDataType.ImportTypeList;
                                break;
                        }
                        structuralItem.Optional = true;
                        break;
                }
            }
            else
            {
                IProductionRuleCaptureStructure structure = (IProductionRuleCaptureStructure)structuralItem;
                var groupItem = (IProductionRuleGroupItem)item;
                switch (item.RepeatOptions.Options & (ScannableEntryItemRepeatOptions.OneOrMore | ScannableEntryItemRepeatOptions.ZeroOrMore | ScannableEntryItemRepeatOptions.ZeroOrOne))
                {
                    case ScannableEntryItemRepeatOptions.ZeroOrOne:
                        structuralItem.Optional = true;
                        break;
                    case ScannableEntryItemRepeatOptions.ZeroOrMore:
                    case ScannableEntryItemRepeatOptions.OneOrMore:
                        foreach (var subItem in structure.Values)
                        {
                            switch (subItem.ResultType)
                            {
                                case ResultedDataType.ImportType:
                                    subItem.ResultType = ResultedDataType.ImportTypeList;
                                    break;
                            }
                        }
                        structuralItem.Optional = true;
                        break;
                }
            }

        }

        internal static Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> ObtainReplacements(IProductionRuleCaptureStructure structure)
        {
            Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> result = new Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem>();
            ObtainReplacements(structure, result);
            return result;
        }

        internal static void ObtainReplacements(IProductionRuleCaptureStructure structure, Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> result)
        {
            foreach (var element in structure.Keys)
                ObtainReplacements(structure[element], result);
        }

        internal static void ObtainReplacements(IProductionRuleCaptureStructuralItem target, Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem> result)
        {
            foreach (var source in target.Sources)
                if (!result.ContainsKey(source))
                    result.Add(source, target);
            if (target is IProductionRuleCaptureStructure)
                ObtainReplacements((IProductionRuleCaptureStructure)target, result);
        }

        internal static SelectiveTuple<Tuple<IIntermediateClassType, IIntermediateInterfaceType>, IIntermediateEnumType, Tuple<IIntermediateEnumType, IIntermediateEnumType[]>> CreateProgramStructure(IProductionRuleCaptureStructure targetStructure, IIntermediateNamespaceDeclaration owner, OilexerGrammarProductionRuleEntry structureRoot)
        {
            switch (targetStructure.ResultType)
            {
                case ResultedDataType.Enumeration:
                    bool isFlagSet = targetStructure.Values.All(k => k.ResultType == ResultedDataType.FlagEnumerationItem);
                    bool useBucketName = targetStructure != structureRoot.CaptureStructure;
                    if (targetStructure.Count > sizeof(SlotType) * 8 && isFlagSet)
                    {
                        var chunkedSets = targetStructure.Values.ToArray().Chunk(sizeof(SlotType) * 8);
                        IIntermediateEnumType[] resultSet = new IIntermediateEnumType[chunkedSets.Length];

                        IIntermediateEnumType aggregateSet = null;
                        aggregateSet = owner.Parts.Add().Enums.Add("{0}Cases", targetStructure.ResultedTypeName);
                        for (int chunkIndex = 0; chunkIndex < chunkedSets.Length; chunkIndex++)
                        {
                            var chunkSet = chunkedSets[chunkIndex];
                            int fieldIndex = 0;
                            var resultEnum = resultSet[chunkIndex] = owner.Parts.Add().Enums.Add("Valid{0}Set{1}", useBucketName ? targetStructure.BucketName : structureRoot.Name, (chunkIndex + 1));
                            resultEnum.AccessLevel = AccessLevelModifiers.Public;
                            resultEnum.Fields.Add("None");
#if x86
                            resultEnum.ValueType = EnumerationBaseType.UInt32;
#elif x64
                            resultEnum.ValueType = EnumerationBaseType.UInt64;
#endif
                            foreach (var element in chunkSet)
                            {
                                switch (element.ResultType)
                                {
                                    case ResultedDataType.EnumerationItem:
                                        resultEnum.Fields.Add(element.BucketName);
                                        break;
                                    case ResultedDataType.FlagEnumerationItem:
                                        var resultField = resultEnum.Fields.Add(element.BucketName, (SlotType)Math.Pow(2, fieldIndex++));
                                        break;
                                    default:
                                        throw new InvalidOperationException("Enums are supposed to yield an " +
                                            "enumeration, result element is of an invalid type.");
                                }
                                aggregateSet.Fields.Add(element.BucketName);
                            }
                        }
                        targetStructure.ResultEnumSet = resultSet;
                        targetStructure.AggregateSetEnum = aggregateSet;
                        return new SelectiveTuple<Tuple<IIntermediateClassType, IIntermediateInterfaceType>, IIntermediateEnumType, Tuple<IIntermediateEnumType, IIntermediateEnumType[]>>(Tuple.Create(aggregateSet, resultSet));
                    }
                    else
                    {
                        IIntermediateEnumType resultEnum = null;
                        IIntermediateEnumType aggregateSet = null;
                        if (isFlagSet)
                        {
                            aggregateSet = owner.Enums.Add("{0}Cases", targetStructure.ResultedTypeName);
                            resultEnum = owner.Enums.Add("Valid{0}", useBucketName ? targetStructure.BucketName : structureRoot.Name);
                        }
                        else
                            resultEnum = owner.Enums.Add(targetStructure.ResultedTypeName);
                        resultEnum.AccessLevel = AccessLevelModifiers.Public;
                        int numNonFlags = (from f in targetStructure.Values
                                           where f.ResultType == ResultedDataType.EnumerationItem
                                           select f).Count();
                        int flagFieldIndex = isFlagSet ? 0 : (int)(Math.Ceiling(Math.Log10(numNonFlags) / Math.Log10(2)));
                        int regularFieldIndex = 1;
#if x86
                        resultEnum.ValueType = EnumerationBaseType.UInt32;
#elif x64
                        resultEnum.ValueType = EnumerationBaseType.UInt64;
#endif
                        resultEnum.Fields.Add("None", (SlotType)0);
                        foreach (var element in targetStructure)
                        {
                            switch (element.Value.ResultType)
                            {
                                case ResultedDataType.EnumerationItem:
                                    resultEnum.Fields.Add(element.Value.BucketName, (SlotType)regularFieldIndex++);
                                    break;
                                case ResultedDataType.FlagEnumerationItem:
                                    var resultField = resultEnum.Fields.Add(element.Value.BucketName, (SlotType)Math.Pow(2, flagFieldIndex++));
                                    break;
                                default:
                                    throw new InvalidOperationException("Enums are supposed to yield an " +
                                        "enumeration, result element is an of invalid type.");
                            }
                            if (isFlagSet)
                                aggregateSet.Fields.Add(element.Value.BucketName);
                        }
                        targetStructure.ResultEnumSet = new IIntermediateEnumType[1] { resultEnum };
                        targetStructure.AggregateSetEnum = aggregateSet;
                        return new SelectiveTuple<Tuple<IIntermediateClassType, IIntermediateInterfaceType>, IIntermediateEnumType, Tuple<IIntermediateEnumType, IIntermediateEnumType[]>>(resultEnum);
                    }
                case ResultedDataType.ComplexType:
                    var resultClass = owner.Classes.Add(targetStructure.ResultedTypeName);
                    var resultInterface = owner.Interfaces.Add("I{0}", targetStructure.ResultedTypeName);
                    resultInterface.AccessLevel = AccessLevelModifiers.Public;
                    resultClass.AccessLevel = AccessLevelModifiers.Internal;
                    resultClass.ImplementedInterfaces.ImplementInterfaceQuick(resultInterface);
                    targetStructure.ResultClass = resultClass;
                    targetStructure.ResultInterface = resultInterface;
                    foreach (var element in targetStructure.Values)
                    {
                        IType simpleType;
                        switch (element.ResultType)
                        {
                            case ResultedDataType.Enumeration:
                            case ResultedDataType.ComplexType:
                                IProductionRuleCaptureStructure cts = element as IProductionRuleCaptureStructure;
                                if (cts == null)
                                    throw new InvalidOperationException("Complex types are supposed to be IProductionRuleCaptureStructure instances.");
                                cts.ResultedTypeName = string.Format("{0}{1}", targetStructure.ResultedTypeName, cts.Name);
                                var currentResult = CreateProgramStructure(cts, owner, structureRoot);
                                currentResult.Visit(
                                    dualClassInterface =>
                                    {
                                        var interfaceProp = resultInterface.Properties.Add(new TypedName(cts.BucketName, dualClassInterface.Item2), true, false);
                                        var classProp = resultClass.Properties.Add(new TypedName(cts.BucketName, dualClassInterface.Item2), true, false);
                                        classProp.AccessLevel = AccessLevelModifiers.Public;
                                        var classField = resultClass.Fields.Add(new TypedName("_{0}", dualClassInterface.Item2, LowerFirstCharacter(cts.BucketName)));
                                        classField.AccessLevel = AccessLevelModifiers.Private;
                                        classProp.GetMethod.Return(classField.GetReference());
                                        cts.AssociatedField = classField;
                                    },
                                    enumeration =>
                                    {
                                        var interfaceProp = resultInterface.Properties.Add(new TypedName(cts.BucketName, enumeration), true, false);
                                        var classProp = resultClass.Properties.Add(new TypedName(cts.BucketName, enumeration), true, false);
                                        classProp.AccessLevel = AccessLevelModifiers.Public;
                                        var classField = resultClass.Fields.Add(new TypedName("_{0}", enumeration, LowerFirstCharacter(cts.BucketName)));
                                        classField.AccessLevel = AccessLevelModifiers.Private;
                                        classProp.GetMethod.Return(classField.GetReference());
                                        cts.AssociatedField = classField;
                                    },
                                    aggregateWithValidSet =>
                                    {
                                        var enumeration = aggregateWithValidSet.Item1;
                                        var interfaceProp = resultInterface.Properties.Add(new TypedName(cts.BucketName, enumeration), true, false);
                                        var classProp = resultClass.Properties.Add(new TypedName(cts.BucketName, enumeration), true, false);
                                        classProp.AccessLevel = AccessLevelModifiers.Public;
                                        var classField = resultClass.Fields.Add(new TypedName("_{0}", enumeration, LowerFirstCharacter(cts.BucketName)));
                                        classField.AccessLevel = AccessLevelModifiers.Private;
                                        classProp.GetMethod.Return(classField.GetReference());
                                        cts.AssociatedField = classField;
                                    },
                                () => { throw new InvalidOperationException("Complex types are supposed to yield a complex type, result type unknown"); });
                                break;
                            case ResultedDataType.EnumerationItem:
                            case ResultedDataType.Flag:
                                simpleType = owner.IdentityManager.ObtainTypeReference(RuntimeCoreType.Boolean);
                                goto simpleType;
                            case ResultedDataType.Counter:
                                simpleType = owner.IdentityManager.ObtainTypeReference(RuntimeCoreType.Int32);
                                goto simpleType;
                            case ResultedDataType.Character:
                                simpleType = owner.IdentityManager.ObtainTypeReference(RuntimeCoreType.Char);
                                goto simpleType;
                            case ResultedDataType.String:
                                simpleType = owner.IdentityManager.ObtainTypeReference(RuntimeCoreType.String);
                                goto simpleType;
                            default:
                                break;
                        }
                        continue;
                    simpleType:
                        {
                            var interfaceProp = resultInterface.Properties.Add(new TypedName(element.BucketName, simpleType), true, false);
                            var classProp = resultClass.Properties.Add(new TypedName(element.BucketName, simpleType), true, false);
                            classProp.AccessLevel = AccessLevelModifiers.Public;
                            var classField = resultClass.Fields.Add(new TypedName("_{0}", simpleType, LowerFirstCharacter(element.BucketName)));
                            classField.AccessLevel = AccessLevelModifiers.Private;
                            classProp.GetMethod.Return(classField.GetReference());
                            element.AssociatedField = classField;
                        }
                    }
                    BuildStructureConstructors(targetStructure, structureRoot, owner.IdentityManager);
                    return new SelectiveTuple<Tuple<IIntermediateClassType, IIntermediateInterfaceType>, IIntermediateEnumType, Tuple<IIntermediateEnumType, IIntermediateEnumType[]>>(Tuple.Create(resultClass, resultInterface));
                case ResultedDataType.PassThrough:
                    return null;
                default:
                    throw new InvalidOperationException();
            }

        }

        internal static string LowerFirstCharacter(this string value)
        {
            if (Char.IsUpper(value[0]))
                return string.Format("{0}{1}", Char.ToLower(value[0]), value.Substring(1));
            return value;
        }

        private static Dictionary<IProductionRuleGroupItem, List<IProductionRuleCaptureStructuralItem>> PickOptionalGroups(IProductionRuleSeries series, IProductionRuleCaptureStructure structure)
        {
            Dictionary<IProductionRuleGroupItem, List<IProductionRuleCaptureStructuralItem>> result = new Dictionary<IProductionRuleGroupItem, List<IProductionRuleCaptureStructuralItem>>();
            foreach (var expression in series)
            {
                foreach (var item in expression)
                {
                    if (item is IProductionRuleGroupItem)
                    {
                        var groupItem = (IProductionRuleGroupItem)item;
                        if ((item.RepeatOptions.Options & ScannableEntryItemRepeatOptions.ZeroOrMore) == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                            (item.RepeatOptions.Options & ScannableEntryItemRepeatOptions.ZeroOrOne) == ScannableEntryItemRepeatOptions.ZeroOrOne ||
                            (item.RepeatOptions.Options & ScannableEntryItemRepeatOptions.Specific) == ScannableEntryItemRepeatOptions.Specific &&
                            (!item.RepeatOptions.Min.HasValue || item.RepeatOptions.Min.Value == 0))
                        {
                            /* *
                             * Found an optional token group, now to search
                             * within to see what we have inside.
                             * */
                            var firstOrDef = structure.Values.FirstOrDefault(k => k.Sources.Contains(item));
                            if (firstOrDef != null)
                                result.Add(groupItem, new List<IProductionRuleCaptureStructuralItem>() { firstOrDef });
                            else
                            {
                                var currentSet = PollGroup(groupItem, structure);
                                if (currentSet.Count > 0)
                                    result.Add(groupItem, currentSet);
                            }
                        }
                        else
                        {
                            var subResult = PickOptionalGroups(groupItem, structure);
                            foreach (var element in subResult.Keys)
                                if (result.ContainsKey(element))
                                    result[element].AddRange(subResult[element]);
                        }
                    }


                }
            }
            return result;
        }

        private static List<IProductionRuleCaptureStructuralItem> PollGroup(IProductionRuleSeries series, IProductionRuleCaptureStructure structure)
        {
            List<IProductionRuleCaptureStructuralItem> result = new List<IProductionRuleCaptureStructuralItem>();
            foreach (var expression in series)
            {
                foreach (var item in expression)
                {
                    var firstOrDef = structure.Values.FirstOrDefault(k => k.Sources.Contains(item));
                    if (firstOrDef != null)
                        result.Add(firstOrDef);
                    else if (item is IProductionRuleGroupItem)
                        result.AddRange(PollGroup((IProductionRuleGroupItem)item, structure));
                }
            }
            return result;
        }

        private static void BuildStructureConstructors(IProductionRuleCaptureStructure targetStructure, OilexerGrammarProductionRuleEntry structureRoot, ITypeIdentityManager identityManager)
        {
            int stateIndex = 0;
            foreach (var element in targetStructure.Values)
                element.StateIndex = stateIndex++;
            IEnumerable<IEnumerable<Tuple<IProductionRuleCaptureStructuralItem, bool>>> validVariations = new IEnumerable<Tuple<IProductionRuleCaptureStructuralItem, bool>>[0];
            var firstOrDefSeries = (IProductionRuleSeries)targetStructure.Sources.FirstOrDefault(k => k is IProductionRuleSeries);
            var firstOrDefProductionRule = (IOilexerGrammarProductionRuleEntry)targetStructure.Sources.FirstOrDefault(k => k is IOilexerGrammarProductionRuleEntry);
            var optg = PickOptionalGroups(firstOrDefProductionRule == null ? firstOrDefSeries : firstOrDefProductionRule, targetStructure);
            foreach (var parameterSet in targetStructure.Structures)
            {
                var limitedSet = (from param in parameterSet
                                  join item in targetStructure.Keys on param equals item
                                  select new { Name = item, Item = targetStructure[item] }).ToArray();
                var optionalSet = (from l in limitedSet
                                   where l.Item.Optional
                                   select l).ToArray();
                if (limitedSet.Length > 0)
                {
                    var parameterPermutations = VariateSeries(new LockedLinkedList<IProductionRuleCaptureStructuralItem>(from l in limitedSet
                                                                                                                         select l.Item).First, optg).Distinct();
                    validVariations = validVariations.Concat(parameterPermutations);
                    foreach (var variation in parameterPermutations)
                    {
                        bool[] parameterStateFlags = (from k in variation.ToArray()
                                                      select k.Item2).ToArray();
                        List<string> parameterResult = new List<string>();
                        for (int parameterIndex = 0; parameterIndex < parameterStateFlags.Length; parameterIndex++)
                        {
                            if (parameterStateFlags[parameterIndex])
                            {
                                var limitedEntry = limitedSet[parameterIndex];
                                parameterResult.Add(limitedEntry.Name);
                            }
                        }
                        //if (!resultantParameterSets.Any(k => k.SequenceEqual(parameterResult)))
                        //    resultantParameterSets.Add(parameterResult);
                    }
                }
            }
            List<IEnumerable<Tuple<IProductionRuleCaptureStructuralItem, bool>>> toRemove = new List<IEnumerable<Tuple<IProductionRuleCaptureStructuralItem, bool>>>();
            var exclusiveDistinctions = (from set in validVariations.Distinct().ToArray()
                                         let parameters =
                                            from parameter in set
                                            where parameter.Item2
                                            select parameter.Item1
                                         orderby string.Join(string.Empty, from k in parameters
                                                                           select k.BucketName)
                                         select new HashList<IProductionRuleCaptureStructuralItem>(parameters)).Distinct();
            var stateField = targetStructure.ResultClass.Fields.Add(new TypedName("state", identityManager.ObtainTypeReference(RuntimeCoreType.Int32)));
            var toStringMethod = targetStructure.ResultClass.Methods.Add(new TypedName("ToString", identityManager.ObtainTypeReference(RuntimeCoreType.String)));
            toStringMethod.AccessLevel = AccessLevelModifiers.Public;
            toStringMethod.IsOverride = true;
            var toStringStateSwitch = toStringMethod.Switch(stateField.GetReference());
            toStringMethod.Return(identityManager.ObtainTypeReference(RuntimeCoreType.String).GetTypeExpression().GetField("Empty"));
            var formatMethod = identityManager.ObtainTypeReference(RuntimeCoreType.String).GetTypeExpression().GetMethod("Format");
            foreach (var parameterSetVariation in exclusiveDistinctions)
            {
                TypedNameSeries currentCtorTNS = new TypedNameSeries();
                int currentStateValue = 0;
                foreach (var parameterEntry in parameterSetVariation)
                {
                    var parameterName = parameterEntry.BucketName;
                    var currentElement = targetStructure[parameterName];
                    IType parameterType = null;
                    switch (currentElement.ResultType)
                    {
                        case ResultedDataType.EnumerationItem:
                        case ResultedDataType.Flag:
                            parameterType = identityManager.ObtainTypeReference(RuntimeCoreType.Boolean);
                            break;
                        case ResultedDataType.Enumeration:
                            IProductionRuleCaptureStructure enumStructure = (IProductionRuleCaptureStructure)currentElement;
                            parameterType = enumStructure.AggregateSetEnum ?? enumStructure.ResultEnumSet[0];
                            break;
                        case ResultedDataType.ComplexType:
                            IProductionRuleCaptureStructure dualStructure = (IProductionRuleCaptureStructure)currentElement;
                            parameterType = dualStructure.ResultInterface;
                            break;
                        case ResultedDataType.Counter:
                            parameterType = identityManager.ObtainTypeReference(RuntimeCoreType.Int32);
                            break;
                        case ResultedDataType.Character:
                            parameterType = identityManager.ObtainTypeReference(RuntimeCoreType.Char);
                            break;
                        case ResultedDataType.String:
                            parameterType = identityManager.ObtainTypeReference(RuntimeCoreType.String);
                            break;
                        default:
                            throw new InvalidOperationException("Unknown parameter type.");
                    }
                    currentStateValue |= (int)Math.Pow(2, parameterEntry.StateIndex);
                    currentCtorTNS.Add(LowerFirstCharacter(parameterName), parameterType);
                }
                var currentCtor = targetStructure.ResultClass.Constructors.Add(currentCtorTNS);
                currentCtor.AccessLevel = AccessLevelModifiers.Public;
                currentCtor.Assign(stateField.GetReference(), currentStateValue.ToPrimitive());
                var currentStateCase = toStringStateSwitch.Case(currentStateValue.ToPrimitive());
                var currentInvocation = formatMethod.Invoke();
                currentStateCase.Return(currentInvocation);
                var currentFormat = string.Empty.ToPrimitive();
                currentInvocation.Arguments.Add(currentFormat);
                StringBuilder formatBuilder = new StringBuilder();
                bool first = true;
                int currentParamIndex = 0;
                foreach (var parameterEntry in parameterSetVariation)
                {
                    if (first)
                        first = false;
                    else
                        formatBuilder.Append(", ");
                    formatBuilder.AppendFormat("{{{0}}}", currentParamIndex++);
                    var parameterName = parameterEntry.BucketName;
                    var currentElement = targetStructure[parameterName];
                    IIntermediateFieldMember currentField = currentElement.AssociatedField;
                    currentInvocation.Arguments.Add(currentField.GetReference());
                    currentCtor.Assign(currentField.GetReference(), currentCtor.Parameters[LowerFirstCharacter(parameterName)].GetReference());
                }
                currentFormat.Value = formatBuilder.ToString();
            }

        }

        private static IEnumerable<IEnumerable<Tuple<IProductionRuleCaptureStructuralItem, bool>>> VariateSeries(ILockedLinkedListNode<IProductionRuleCaptureStructuralItem> startingPoint, Dictionary<IProductionRuleGroupItem, List<IProductionRuleCaptureStructuralItem>> optionalTogglers, int depth = 0)
        {
            var next = startingPoint.Next;
            if (next == null)
            {
                if (startingPoint.Element.Optional)
                    yield return YieldValue(startingPoint.Element, false);
                yield return YieldValue(startingPoint.Element, true);
            }
            else
                foreach (var set in VariateSeries(next, optionalTogglers, depth + 1))
                {
                    if (startingPoint.Element.Optional)
                    {
                        yield return YieldValue(startingPoint.Element, false).Concat(set);
                        if (depth == 0)
                            foreach (var toggler in optionalTogglers.Keys)
                            {
                                var alternateSet = Replace(set, structuralItem => optionalTogglers[toggler].Contains(structuralItem.Item1), structuralItem =>
                                                                    Tuple.Create(structuralItem.Item1, false));
                                if (!alternateSet.SequenceEqual(set))
                                    yield return YieldValue(startingPoint.Element, false).Concat(alternateSet);
                            }
                    }
                    if (depth == 0)
                        foreach (var toggler in optionalTogglers.Keys)
                        {
                            var alternateSet = Replace(set, structuralItem => optionalTogglers[toggler].Contains(structuralItem.Item1), structuralItem =>
                                                                Tuple.Create(structuralItem.Item1, false));
                            if (!alternateSet.SequenceEqual(set))
                                yield return YieldValue(startingPoint.Element, true).Concat(alternateSet);
                        }
                    yield return YieldValue(startingPoint.Element, true).Concat(set);
                }
        }

        private static IEnumerable<T> Replace<T>(IEnumerable<T> series, Predicate<T> predicate, Func<T, T> replacer)
        {
            foreach (var element in series)
                if (predicate(element))
                    yield return replacer(element);
                else
                    yield return element;
        }

        private static IEnumerable<Tuple<IProductionRuleCaptureStructuralItem, bool>> YieldValue(IProductionRuleCaptureStructuralItem item, bool value)
        {
            yield return Tuple.Create(item, value);
        }
    }
}