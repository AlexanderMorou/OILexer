using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Tuples;
using AllenCopeland.Abstraction.Utilities.Arrays;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
#if x64
using SlotType = System.UInt64;
#elif x86
#if HalfWord
using SlotType = System.UInt16;
#else
using SlotType = System.UInt32;
#endif
#endif
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal static class TokenStructuralExtractionCore
    {
        internal static ICaptureTokenStructure BuildStructureFor(InlinedTokenEntry entry, IOilexerGrammarFile source)
        {
            return BuildStructureFor(entry, entry.Branches, source);
        }

        private static ICaptureTokenStructure BuildStructureFor(InlinedTokenEntry entry, ITokenExpressionSeries expressionSeries, IOilexerGrammarFile source)
        {
            ICaptureTokenStructure result = null;
            HashList<HashList<string>> currentResultVariants = new HashList<HashList<string>>();
            foreach (var expression in expressionSeries)
            {

                var current = BuildStructureFor(entry, expressionSeries, expression, source);
                if (result == null)
                    result = current;
                else
                    result = result.Union(current);
                if (entry.CaptureKind == RegularCaptureType.Capturer)
                {
                    var dataSet = new HashList<string>(current.Keys);
                    if (!currentResultVariants.Any(k => k.SequenceEqual(dataSet)))
                        currentResultVariants.Add(dataSet);
                }
            }
            foreach (var variant in currentResultVariants)
                result.Structures.Add(variant);
            if (expressionSeries == entry.Branches)
                ((ControlledCollection<ITokenSource>)result.Sources).baseList.Add(entry);
            result.ResultedTypeName = string.Format("{0}{1}{2}", source.Options.TokenPrefix, entry.Name, source.Options.TokenSuffix);
            return result;
        }

        private static ICaptureTokenStructure BuildStructureFor(InlinedTokenEntry entry, ITokenExpressionSeries expressionSeries, ITokenExpression expression, IOilexerGrammarFile source)
        {
            ICaptureTokenStructure result = new CaptureTokenStructure();
            foreach (var item in expression)
            {
                var current = BuildStructureFor(entry, expressionSeries, expression, item, source);
                result = result.Concat(current);
            }
            return result;
        }

        private static ICaptureTokenStructuralItem BuildStructureFor(InlinedTokenEntry entry, ITokenExpressionSeries expressionSeries, ITokenExpression expression, ITokenItem item, IOilexerGrammarFile source)
        {
            ICaptureTokenStructuralItem result = null;
            if (item is ITokenGroupItem)
            {
                var tokenGroup = ((ITokenGroupItem)(item));
                result = BuildStructureFor(entry, tokenGroup, source);
                if (result.ResultType == ResultedDataType.None && !string.IsNullOrEmpty(item.Name))
                {
                    if (tokenGroup.Count == 1 && tokenGroup[0].Count == 1)
                    {
                        var singleItem = tokenGroup[0][0];
                        if (singleItem is ILiteralCharTokenItem || singleItem is ILiteralCharReferenceTokenItem)
                        {
                            if (singleItem.RepeatOptions == ScannableEntryItemRepeatInfo.None && tokenGroup.RepeatOptions == ScannableEntryItemRepeatInfo.None)
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
                bool groupOptional = tokenGroup != null &&
                                   ((tokenGroup.RepeatOptions.Options & ScannableEntryItemRepeatOptions.ZeroOrMore) == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                                    (tokenGroup.RepeatOptions.Options & ScannableEntryItemRepeatOptions.ZeroOrOne) == ScannableEntryItemRepeatOptions.ZeroOrOne ||
                                  (((tokenGroup.RepeatOptions.Options & ScannableEntryItemRepeatOptions.Specific) == ScannableEntryItemRepeatOptions.Specific) &&
                                   (!tokenGroup.RepeatOptions.Min.HasValue || tokenGroup.RepeatOptions.Min.Value == 0)));
                if (groupOptional)
                    result.GroupOptional = groupOptional;
                if (item.Name.IsEmptyOrNull())
                {
                    result.ResultType = ResultedDataType.PassThrough;
                    /*
                    if ((item.RepeatOptions.Options & ScannableEntryItemRepeatOptions.ZeroOrMore) == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                        (item.RepeatOptions.Options & ScannableEntryItemRepeatOptions.ZeroOrOne) == ScannableEntryItemRepeatOptions.ZeroOrOne ||
                        ((item.RepeatOptions.Options & ScannableEntryItemRepeatOptions.Specific) == ScannableEntryItemRepeatOptions.Specific &&
                        (!item.RepeatOptions.Min.HasValue || item.RepeatOptions.Min.Value == 0)))
                        foreach (var element in ((ICaptureTokenStructure)result).Values)
                            element.Optional = true;*/
                }

                ((ControlledCollection<ITokenSource>)(result.Sources)).baseList.Add(item);
            }
            else if (item is ILiteralTokenItem)
            {
                result = new CaptureTokenLiteralStructuralItem((ILiteralTokenItem)item);
                if (entry.CaptureKind == RegularCaptureType.Transducer)
                    result.ResultType = ResultedDataType.EnumerationItem;
                else if (entry.CaptureKind == RegularCaptureType.ContextfulTransducer)
                    result.ResultType = ResultedDataType.FlagEnumerationItem;
            }
            else if (item is ICharRangeTokenItem)
            {
                result = new CaptureTokenCharRangeStructuralItem((ICharRangeTokenItem)item);
                if (entry.CaptureKind == RegularCaptureType.Transducer)
                    result.ResultType = ResultedDataType.EnumerationItem;
                else if (entry.CaptureKind == RegularCaptureType.ContextfulTransducer)
                    result.ResultType = ResultedDataType.FlagEnumerationItem;
            }

            Deform(item, result, expression);

            return result;
        }

        private static void Deform(ITokenItem item, ICaptureTokenStructuralItem structuralItem, ITokenExpression owner)
        {
            /* *
             * Items reduced to their max are for recognizer sections..
             * */
            if ((item.RepeatOptions.Options & ScannableEntryItemRepeatOptions.MaxReduce) == ScannableEntryItemRepeatOptions.MaxReduce && string.IsNullOrEmpty(item.Name))
                return;
            switch (item.RepeatOptions.Options & (ScannableEntryItemRepeatOptions.OneOrMore | ScannableEntryItemRepeatOptions.ZeroOrMore | ScannableEntryItemRepeatOptions.ZeroOrOne))
            {
                case ScannableEntryItemRepeatOptions.ZeroOrOne:
                    structuralItem.Optional = true;
                    break;
                case ScannableEntryItemRepeatOptions.ZeroOrMore:
                case ScannableEntryItemRepeatOptions.OneOrMore:
                    if (structuralItem.ResultType == ResultedDataType.Character)
                    {
                        if (structuralItem is ICaptureTokenCharRangeStructuralItem)
                        {
                            var ctcrsi = (ICaptureTokenCharRangeStructuralItem)structuralItem;
                            if (!ctcrsi.HasSiblings)
                            {
                                ctcrsi.ResultType = ResultedDataType.String;
                                return;
                            }
                        }
                    }
                    structuralItem.Optional = true;
                    //structuralItem.Rank++;
                    break;
            }
        }

        internal static Dictionary<ITokenSource, ICaptureTokenStructuralItem> ObtainReplacements(ICaptureTokenStructure structure)
        {
            Dictionary<ITokenSource, ICaptureTokenStructuralItem> result = new Dictionary<ITokenSource, ICaptureTokenStructuralItem>();
            ObtainReplacements(structure, result);
            return result;
        }

        internal static void ObtainReplacements(ICaptureTokenStructure structure, Dictionary<ITokenSource, ICaptureTokenStructuralItem> result)
        {
            foreach (var element in structure.Keys)
                ObtainReplacements(structure[element], result);
        }

        internal static void ObtainReplacements(ICaptureTokenStructuralItem target, Dictionary<ITokenSource, ICaptureTokenStructuralItem> result)
        {
            foreach (var source in target.Sources)
                if (!result.ContainsKey(source))
                    result.Add(source, target);
            if (target is ICaptureTokenStructure)
                ObtainReplacements((ICaptureTokenStructure)target, result);
        }

        internal static SelectiveTuple<Tuple<IIntermediateClassType, IIntermediateInterfaceType>, IIntermediateEnumType, Tuple<IIntermediateEnumType, IIntermediateEnumType[]>> CreateProgramStructure(ICaptureTokenStructure targetStructure, IIntermediateNamespaceDeclaration owner, InlinedTokenEntry structureRoot, ParserCompiler compiler)
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
                            aggregateSet = owner.Parts.Add().Enums.Add("{0}Cases", targetStructure.ResultedTypeName);
                            resultEnum = owner.Parts.Add().Enums.Add("Valid{0}", useBucketName ? targetStructure.BucketName : structureRoot.Name);
                        }
                        else
                            resultEnum = owner.Parts.Add().Enums.Add(targetStructure.ResultedTypeName);
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
                    var resultClass = owner.Parts.Add().Classes.Add(targetStructure.ResultedTypeName);
                    var resultInterface = owner.Parts.Add().Interfaces.Add("I{0}", targetStructure.ResultedTypeName);
                    bool tailLanguage = compiler.Source.Options.GrammarName.ToLower().EndsWith("language");
                    resultInterface.SummaryText = string.Format("Represents the {0} structure from the {1}{2}.", targetStructure.ResultedTypeName, compiler.Source.Options.GrammarName, tailLanguage ? string.Empty : " language");
                    resultClass.SummaryText = string.Format("Provides a base implementation of a @s:{2}; from the {1}{3}.", targetStructure.Name, compiler.Source.Options.GrammarName, resultInterface.Name, tailLanguage ? string.Empty : " language");
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
                                ICaptureTokenStructure cts = element as ICaptureTokenStructure;
                                if (cts == null)
                                    throw new InvalidOperationException("Complex types are supposed to be ICaptureTokenStructure instances.");
                                cts.ResultedTypeName = string.Format("{0}{1}", targetStructure.ResultedTypeName, cts.Name);
                                var currentResult = CreateProgramStructure(cts, owner, structureRoot, compiler);
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

        private static string LowerFirstCharacter(string value)
        {
            if (Char.IsUpper(value[0]))
                return string.Format("{0}{1}", Char.ToLower(value[0]), value.Substring(1));
            return value;
        }

        private static Dictionary<ITokenGroupItem, List<ICaptureTokenStructuralItem>> PickOptionalGroups(ITokenExpressionSeries series, ICaptureTokenStructure structure)
        {
            Dictionary<ITokenGroupItem, List<ICaptureTokenStructuralItem>> result = new Dictionary<ITokenGroupItem, List<ICaptureTokenStructuralItem>>();
            foreach (var expression in series)
            {
                foreach (var item in expression)
                {
                    if (item is ITokenGroupItem)
                    {
                        var groupItem = (ITokenGroupItem)item;
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
                                result.Add(groupItem, new List<ICaptureTokenStructuralItem>() { firstOrDef });
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

        private static List<ICaptureTokenStructuralItem> PollGroup(ITokenExpressionSeries series, ICaptureTokenStructure structure)
        {
            List<ICaptureTokenStructuralItem> result = new List<ICaptureTokenStructuralItem>();
            foreach (var expression in series)
            {
                foreach (var item in expression)
                {
                    var firstOrDef = structure.Values.FirstOrDefault(k => k.Sources.Contains(item));
                    if (firstOrDef != null)
                        result.Add(firstOrDef);
                    else if (item is ITokenGroupItem)
                        result.AddRange(PollGroup((ITokenGroupItem)item, structure));
                }
            }
            return result;
        }

        private static void BuildStructureConstructors(ICaptureTokenStructure targetStructure, InlinedTokenEntry structureRoot, ITypeIdentityManager identityManager)
        {
            int stateIndex = 0;
            foreach (var element in targetStructure.Values)
                element.StateIndex = stateIndex++;
            IEnumerable<IEnumerable<Tuple<ICaptureTokenStructuralItem, bool>>> validVariations = new IEnumerable<Tuple<ICaptureTokenStructuralItem, bool>>[0];
            //List<List<string>> resultantParameterSets = new List<List<string>>();
            var firstOrDefSeries = (ITokenExpressionSeries)targetStructure.Sources.FirstOrDefault(k => k is ITokenExpressionSeries);
            var firstOrDefToken = (IOilexerGrammarTokenEntry)targetStructure.Sources.FirstOrDefault(k => k is IOilexerGrammarTokenEntry);
            var optg = PickOptionalGroups(firstOrDefToken == null ? firstOrDefSeries : firstOrDefToken.Branches, targetStructure);
            foreach (var parameterSet in targetStructure.Structures)
            {
                var limitedSet = (from param in parameterSet
                                  join item in targetStructure.Keys on param equals item
                                  select new { Name = item, Item = targetStructure[item] }).ToArray();
                var optionalSet = (from l in limitedSet
                                   where l.Item.Optional || l.Item.GroupOptional
                                   select l).ToArray();
                if (limitedSet.Length > 0)
                {
                    var parameterPermutations = VariateSeries(new LockedLinkedList<ICaptureTokenStructuralItem>(from l in limitedSet
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
            List<IEnumerable<Tuple<ICaptureTokenStructuralItem, bool>>> toRemove = new List<IEnumerable<Tuple<ICaptureTokenStructuralItem, bool>>>();
            var exclusiveDistinctions = (from set in validVariations.Distinct().ToArray()
                                         let parameters =
                                            from parameter in set
                                            where parameter.Item2
                                            select parameter.Item1
                                         orderby string.Join(string.Empty, from k in parameters
                                                                           select k.BucketName)
                                         select new HashList<ICaptureTokenStructuralItem>(parameters)).Distinct();
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
                            ICaptureTokenStructure enumStructure = (ICaptureTokenStructure)currentElement;
                            parameterType = enumStructure.AggregateSetEnum ?? enumStructure.ResultEnumSet[0];
                            break;
                        case ResultedDataType.ComplexType:
                            ICaptureTokenStructure dualStructure = (ICaptureTokenStructure)currentElement;
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

        private static IEnumerable<IEnumerable<Tuple<ICaptureTokenStructuralItem, bool>>> VariateSeries(ILockedLinkedListNode<ICaptureTokenStructuralItem> startingPoint, Dictionary<ITokenGroupItem, List<ICaptureTokenStructuralItem>> optionalTogglers, int depth = 0)
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
                    if (startingPoint.Element.Optional || startingPoint.Element.GroupOptional)
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

        private static IEnumerable<Tuple<ICaptureTokenStructuralItem, bool>> YieldValue(ICaptureTokenStructuralItem item, bool value)
        {
            yield return Tuple.Create(item, value);
        }
    }
}
