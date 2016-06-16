using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Languages.CSharp;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Languages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using System.Globalization;
using AllenCopeland.Abstraction.Globalization;
using System.Threading;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Translation;
using AllenCopeland.Abstraction.Slf.Translation;
using System.IO;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class MultikeyedDictionaryBuilder
    {
        /* ACC - Update June 11, 2015: Ported from the old Code Generator.  Messy has hell, but it works. */
        public static string GetEnglishNumberIdentifier(uint number)
        {
            var text = ci.TextInfo.ToTitleCase(GetEnglishNumber(number, false));
            return text.Replace(" ", "");
        }
        private static CultureInfo ci = new CultureInfo((int)CultureIdentifiers.NumericIdentifiers.English_UnitedStates);
        public static string GetEnglishNumber(uint number, bool suffix = true)
        {
            if (number == 0)
                return "zero";
            double logValue = Math.Log10(number);
            int[] places = new int[(uint)Math.Ceiling(logValue) + (((double)((uint)logValue) == logValue) ? 1 : 0)];
            for (int i = 0; i < places.Length; i++)
            {
                number -= (uint)(places[i] = (int)(number % 10));
                number /= 10;
            }
            string[] terms = new string[(int)Math.Ceiling((double)places.Length / 3D)];
            for (int i = 0, g = 0, c = places.Length; i < c; i += 3, g++)
            {
                terms[g] = string.Empty;
                bool last = i == c - 1;
                bool nextIsLast = i == c - 2;
                bool allZero = true;
                if (!last && nextIsLast &&
                    places[i + 1] != 0)
                    allZero = false;
                if (!(last || nextIsLast))
                {
                    int place = places[i + 2];
                    if (place != 0)
                    {
                        allZero = false;
                        terms[g] += AppendSmallValue(place, terms[g], suffix) + " hundred";
                    }
                }
                if (!last)
                    switch (places[i + 1])
                    {
                        case 0:
                            if (places[i] != 0)
                                allZero = false;
                            if ((!last && places[i + 1] == 0) && !suffix && places[i] != 0)
                                terms[g] += " and";
                            terms[g] = AppendSmallValue(places[i], terms[g], suffix, g == 0);
                            break;
                        case 1:
                            allZero = false;
                            if (g == 0 && !suffix)
                                terms[g] += "and ";
                            if (!string.IsNullOrEmpty(terms[g]))
                                terms[g] += " ";
                            if (places[i + 1] != 0 && g == 0 && !suffix && (!(nextIsLast || last) && places[i + 2] != 0))
                                terms[g] += "and ";
                            switch (places[i])
                            {
                                case 0:
                                    terms[g] += "ten" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                                case 1:
                                    terms[g] += "eleven" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                                case 2:
                                    terms[g] += "twelve" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                                case 3:
                                    terms[g] += "thirteen" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                                case 4:
                                    terms[g] += "fourteen" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                                case 5:
                                    terms[g] += "fifteen" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                                case 6:
                                    terms[g] += "sixteen" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                                case 7:
                                    terms[g] += "seventeen" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                                case 8:
                                    terms[g] += "eighteen" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                                case 9:
                                    terms[g] += "ninteen" + (suffix && g == 0 ? "th" : string.Empty);
                                    break;
                            }
                            break;
                        case 2:
                            if (!string.IsNullOrEmpty(terms[g]))
                                terms[g] += " ";
                            if (places[i + 1] != 0 && !suffix && (!(nextIsLast || last) && places[i + 2] != 0))
                                terms[g] += "and ";
                            if ((suffix && g == 0 && places[i] == 0))
                            {
                                terms[g] = "twentieth";
                                break;
                            }
                            terms[g] += "twenty";
                            goto case 0;
                        case 3:
                            if (!string.IsNullOrEmpty(terms[g]))
                                terms[g] += " ";
                            if (places[i + 1] != 0 && !suffix && (!(nextIsLast || last) && places[i + 2] != 0))
                                terms[g] += "and ";
                            if ((suffix && g == 0 && places[i] == 0))
                            {
                                terms[g] = "thirtieth";
                                break;
                            }
                            terms[g] += "thirty";
                            goto case 0;
                        case 4:
                            if (!string.IsNullOrEmpty(terms[g]))
                                terms[g] += " ";
                            if (places[i + 1] != 0 && !suffix && (!(nextIsLast || last) && places[i + 2] != 0))
                                terms[g] += "and ";
                            if ((suffix && g == 0 && places[i] == 0))
                            {
                                terms[g] = "fourtieth";
                                break;
                            }
                            terms[g] += "fourty";
                            goto case 0;
                        case 5:
                            if (!string.IsNullOrEmpty(terms[g]))
                                terms[g] += " ";
                            if (places[i + 1] != 0 && !suffix && (!(nextIsLast || last) && places[i + 2] != 0))
                                terms[g] += "and ";
                            if ((suffix && g == 0 && places[i] == 0))
                            {
                                terms[g] = "fiftieth";
                                break;
                            }
                            terms[g] += "fifty";
                            goto case 0;
                        case 6:
                            if (!string.IsNullOrEmpty(terms[g]))
                                terms[g] += " ";
                            if (places[i + 1] != 0 && !suffix && (!(nextIsLast || last) && places[i + 2] != 0))
                                terms[g] += "and ";
                            if ((suffix && g == 0 && places[i] == 0))
                            {
                                terms[g] = "sixtieth";
                                break;
                            }
                            terms[g] += "sixty";
                            goto case 0;
                        case 7:
                            if (!string.IsNullOrEmpty(terms[g]))
                                terms[g] += " ";
                            if (places[i + 1] != 0 && !suffix && (!(nextIsLast || last) && places[i + 2] != 0))
                                terms[g] += "and ";
                            if ((suffix && g == 0 && places[i] == 0))
                            {
                                terms[g] = "seventieth";
                                break;
                            }
                            terms[g] += "seventy";
                            goto case 0;
                        case 8:
                            if (!string.IsNullOrEmpty(terms[g]))
                                terms[g] += " ";
                            if (places[i + 1] != 0 && !suffix && (!(nextIsLast || last) && places[i + 2] != 0))
                                terms[g] += "and ";
                            if ((suffix && g == 0 && places[i] == 0))
                            {
                                terms[g] = "eightieth";
                                break;
                            }
                            terms[g] += "eighty";
                            goto case 0;
                        case 9:
                            if (!string.IsNullOrEmpty(terms[g]))
                                terms[g] += " ";
                            if (places[i + 1] != 0 && !suffix && (!(nextIsLast || last) && places[i + 2] != 0))
                                terms[g] += "and ";
                            if ((suffix && g == 0 && places[i] == 0))
                            {
                                terms[g] = "ninetieth";
                                break;
                            }
                            terms[g] += "ninety";
                            goto case 0;
                    }
                else
                {
                    if (places[i] != 0)
                        allZero = false;
                    terms[g] = AppendSmallValue(places[i], terms[g], suffix, g == 0);
                }

                if (!allZero)
                    switch (g)
                    {
                        case 1:
                            terms[g] += " thousand";
                            break;
                        case 2:
                            terms[g] += " million";
                            break;
                        case 3:
                            terms[g] += " billion";
                            break;
                        case 4:
                            terms[g] += " trillion";
                            break;
                    }
            }
            if (terms.Length == 0)
                return string.Empty;
            return string.Join(" ", from q in terms.Reverse()
                                    where q != string.Empty
                                    select q);
        }
        private static string AppendSmallValue(int place, string term, bool suffix, bool last = false)
        {
            if (place == 0)
                return term;
            else if (!string.IsNullOrEmpty(term))
                term += " ";
            switch (place)
            {
                case 1:
                    if (!suffix || !last)
                        term += "one";
                    else
                        term += "first";
                    break;
                case 2:
                    if (!suffix || !last)
                        term += "two";
                    else
                        term += "second";
                    break;
                case 3:
                    if (!suffix || !last)
                        term += "three";
                    else
                        term += "third";
                    break;
                case 4:
                    if (!suffix || !last)
                        term += "four";
                    else
                        term += "fourth";
                    break;
                case 5:
                    if (!suffix || !last)
                        term += "five";
                    else
                        term += "fifth";
                    break;
                case 6:
                    if (!suffix || !last)
                        term += "six";
                    else
                        term += "sixth";
                    break;
                case 7:
                    if (!suffix || !last)
                        term += "seven";
                    else
                        term += "seventh";
                    break;
                case 8:
                    if (!suffix || !last)
                        term += "eight";
                    else
                        term += "eighth";
                    break;
                case 9:
                    if (!suffix || !last)
                        term += "nine";
                    else
                        term += "ninth";
                    break;
            }
            return term;
        }
        public static Tuple<IInterfaceType[], IClassType[], IStructType[], IStructType, IIntermediateAssembly> CreateNormalVariation(IIntermediateAssembly assembly, IIntermediateCliManager identityManager, ParserCompiler compiler, int multipleKeyCount = 2)
        {
            var defNamespace = assembly.DefaultNamespace;
            GenericParameterData[] typeParameterData = new GenericParameterData[multipleKeyCount];
            for (int i = 0; i < typeParameterData.Length; i++)
                typeParameterData[i] = new GenericParameterData(string.Format("TKey{0}", i + 1));
            var keysValuePair = defNamespace.Parts.Add().Structs.Add("{0}KeysValuePair", assembly.Name);
            var keysValueTKeys = keysValuePair.TypeParameters.Add("TKeys");
            var keysValueTValue = keysValuePair.TypeParameters.Add("TValue");
            var keysValueKeys = keysValuePair.Properties.Add(new TypedName("Keys", keysValueTKeys), true, false);
            keysValueKeys.SummaryText = "Returns the @T:TKeys; which relate to the keys associated to the pair.";
            var keysValueValue = keysValuePair.Properties.Add(new TypedName("Value", keysValueTValue), true, false);
            keysValueValue.SummaryText = "Returns the @T:TValue; which relates to the value associated to the pair.";
            var keysValue_keys = keysValuePair.Fields.Add(new TypedName("_keys", keysValueTKeys));
            var keysValue_value = keysValuePair.Fields.Add(new TypedName("_value", keysValueTValue));
            var keysValueCtor = keysValuePair.Constructors.Add(new TypedName("keys", keysValueTKeys), new TypedName("value", keysValueTValue));
            var keysValueCtorKeys = keysValueCtor.Parameters.Values[0];
            var keysValueCtorValue = keysValueCtor.Parameters.Values[1];
            keysValueCtor.Assign(keysValue_keys.GetReference(), keysValueCtorKeys.GetReference());
            keysValueCtor.Assign(keysValue_value.GetReference(), keysValueCtorValue.GetReference());
            keysValueCtor.SummaryText = string.Format("Creates a new @S:KeysValueCtor{{{0}, {1}}}; with the @P:{2}; and @P:{3}; provided.", keysValueTKeys.Name, keysValueTValue.Name, keysValueCtorKeys.Name, keysValueCtorValue.Name);
            keysValueCtorKeys.SummaryText = string.Format("The @T:{0}; value which denotes the keys of the pair.", keysValueTKeys.Name);
            keysValueCtorValue.SummaryText = string.Format("The @T:{0}; value which denotes the value of the pair.", keysValueTValue.Name);
            keysValueCtor.AccessLevel = AccessLevelModifiers.Public;
            keysValue_keys.SummaryText = string.Format("Data member for @S:{0};.", keysValueKeys.Name);
            keysValue_value.SummaryText = string.Format("Data member for @S:{0};.", keysValueValue.Name);
            keysValueKeys.GetMethod.Return(keysValue_keys.GetReference());
            keysValueValue.GetMethod.Return(keysValue_value.GetReference());
            keysValueKeys.AccessLevel = AccessLevelModifiers.Public;
            keysValueValue.AccessLevel = AccessLevelModifiers.Public;
            keysValuePair.AccessLevel = AccessLevelModifiers.Public;
            keysValuePair.SummaryText = "Provides a pair for the keys and values associated to a multi-keyed dictionary.";
            keysValueTKeys.SummaryText = "The type of the keys tuple struct used in the pair.";
            keysValueTValue.SummaryText = "The type of the value used in the pair.";
            IIntermediateClassType[] resultClasses = new IIntermediateClassType[multipleKeyCount - 1];
            IIntermediateInterfaceType[] resultInterfaces = new IIntermediateInterfaceType[multipleKeyCount - 1];
            IIntermediateStructType[] resultedStructs = new IIntermediateStructType[multipleKeyCount - 1];

            IIntermediateClassType subkeyNotFound = defNamespace.Parts.Add().Classes.Add("SubKeyNotFoundException");
            subkeyNotFound.SummaryText = "The exception that is thrown when a sub-key of a multiple-keyed dictionary is requested but not found.";
            subkeyNotFound.BaseType = (IClassType)identityManager.ObtainTypeReference(typeof(ArgumentException));
            var subKeyNotFoundCtor = subkeyNotFound.Constructors.Add(new TypedName("subKeyParameter", identityManager.ObtainTypeReference(RuntimeCoreType.String)));
            var subKeyParam = subKeyNotFoundCtor.Parameters.Values[0];
            var subkeyMessage = identityManager.ObtainTypeReference(typeof(string)).GetTypeExpression().GetMethod("Format").Invoke("The subkey '{0}' was not found in the multiple-keyed dictionary.".ToPrimitive(), subKeyParam.GetReference());
            subKeyNotFoundCtor.AccessLevel = AccessLevelModifiers.Public;
            subKeyNotFoundCtor.CascadeTarget = ConstructorCascadeTarget.Base;
            subKeyNotFoundCtor.CascadeMembers.Add(subkeyMessage);
            subKeyNotFoundCtor.CascadeMembers.Add(subKeyParam.GetReference());

            for (int i = 2; i <= multipleKeyCount; i++)
            {
                GenericParameterData[] typeParameterCopyData = new GenericParameterData[i + 1];
                Array.ConstrainedCopy(typeParameterData, 0, typeParameterCopyData, 0, typeParameterCopyData.Length - 1);
                typeParameterCopyData[typeParameterCopyData.Length - 1] = new GenericParameterData("TValue");
                var currentClass = defNamespace.Parts.Add().Classes.Add(string.Format("{0}MultikeyedDictionary", assembly.Name), typeParameterCopyData);
                currentClass.Assembly.ScopeCoercions.Add(typeof(Monitor).Namespace);
                var currentInterface = defNamespace.Parts.Add().Interfaces.Add(string.Format("I{0}MultikeyedDictionary", assembly.Name), typeParameterCopyData);
                IType[] interfaceKeysRefs = new IType[i];
                for (int j = 0; j < i; j++)
                    interfaceKeysRefs[j] = currentInterface.TypeParameters.Values[j];
                TypedName[] keys = new TypedName[i];
                for (int j = 0; j < i; j++)
                    keys[j] = new TypedName(string.Format("key{0}", j + 1), currentInterface.TypeParameters.Values[j]);
                TypedName[] keysValue = new TypedName[i + 1];
                keys.CopyTo(keysValue, 0);
                keysValue[i] = new TypedName("value", currentInterface.TypeParameters.Values[i]);
                var joinedTParams = string.Join(",", from tp in currentClass.TypeParameters.Values
                                                     select tp.Name);
                var levels = new IIntermediateClassType[i - 1];
                var keyDictionaries = new IIntermediateClassType[i - 2];

                for (int j = 1; j < i - 1; j++)
                {
                    IType currentInterfaceTarget;
                    if (j == i - 2)
                        currentInterfaceTarget = ((IInterfaceType)identityManager.ObtainTypeReference(typeof(IDictionary<,>))).MakeGenericClosure(new TypeCollection(interfaceKeysRefs[j + 1], currentInterface.TypeParameters.Values[i]));
                    else
                    {
                        List<IType> currentSet = new List<IType>(interfaceKeysRefs.Skip(1).Take(resultInterfaces[j].TypeParameters.Count - 1));
                        currentSet.Add(currentInterface.TypeParameters.Values[i]);
                        currentInterfaceTarget = resultInterfaces[j].MakeGenericClosure(new TypeCollection(currentSet.ToArray()));
                    }
                }
                for (int j = 0; j < i - 1; j++)
                    levels[j] = currentClass.Parts.Add().Classes.Add(string.Format("Level{0}", GetEnglishNumberIdentifier((uint)j + 1)));
                for (int j = 0; j < i - 1; j++)
                {
                    bool last = j == i - 2;
                    bool beforeLast = j == i - 3;
                    var currentLevel = levels[j];

                    if (!last)
                    {
                        currentLevel.BaseType = ((IClassType)identityManager.ObtainTypeReference(typeof(Dictionary<,>))).MakeGenericClosure(new TypeCollection(interfaceKeysRefs[j + 1], levels[j + 1]));
                        currentLevel.SummaryText = string.Format("The {0} level of the @S:{1}{{{2}}};. Represents a pairing between @T:{3}; and @S:{4};.", GetEnglishNumber((uint)(j + 1)), currentClass.Name, joinedTParams, currentInterface.TypeParameters.Values[j + 1].Name, levels[j + 1].Name);
                        var currentCount = currentLevel.Properties.Add(new TypedName("Count", identityManager.ObtainTypeReference(typeof(int))), true, false);
                        var currentCountAggregator = currentCount.GetMethod.Locals.Add(new TypedName("count", identityManager.ObtainTypeReference(typeof(int))), IntermediateGateway.NumberZero);
                        var countLock = new LockStatement(currentCount.GetMethod, new SpecialReferenceExpression(SpecialReferenceKind.This));
                        currentCount.GetMethod.Add(countLock);
                        var enumeration = countLock.Enumerate(string.Format("level{0}", GetEnglishNumberIdentifier((uint)j + 2)), new SpecialReferenceExpression(SpecialReferenceKind.This).GetProperty("Values"));
                        if (beforeLast)
                        {
                            var nextLock = new LockStatement(enumeration, enumeration.Local.GetReference());
                            enumeration.Add(nextLock);

                            nextLock.Assign(currentCountAggregator, currentCountAggregator.Add(enumeration.Local.GetReference().GetProperty("Count")));
                        }
                        else
                            enumeration.Assign(currentCountAggregator.GetReference(), currentCountAggregator.Add(enumeration.Local.GetReference().GetProperty("Count")));
                        //enumeration.Local.Name = string.Format("level{0}", GetEnglishNumberIdentifier((uint)j + 2));
                        currentCount.AccessLevel = AccessLevelModifiers.Public;
                        currentCount.GetMethod.Return(currentCountAggregator.GetReference());
                    }
                    else
                    {
                        currentLevel.BaseType = ((IClassType)identityManager.ObtainTypeReference(typeof(Dictionary<,>))).MakeGenericClosure(new TypeCollection(interfaceKeysRefs[j + 1], currentInterface.TypeParameters.Values[i]));
                        currentLevel.SummaryText = string.Format("The {0} level of the @S:{1}{{{2}}};. Represents a pairing between @T:{3}; and @T:{4};.", GetEnglishNumber((uint)(j + 1)), currentClass.Name, joinedTParams, currentInterface.TypeParameters.Values[j + 1].Name, currentInterface.TypeParameters.Values[i].Name);
                    }
                }
                var keysType = defNamespace.Parts.Add().Structs.Add(string.Format("Keys{0}", i));
                resultedStructs[i - 2] = keysType;
                keysType.AccessLevel = AccessLevelModifiers.Public;
                var keysCtor = keysType.Constructors.Add();
                keysType.SummaryText = "Provides a tuple for the keys associated to a multi-keyed dictionary.";

                var keysToString = keysType.Methods.Add(new TypedName("ToString", identityManager.ObtainTypeReference(typeof(string))));
                keysToString.AccessLevel = AccessLevelModifiers.Public;
                ((IIntermediateStructMethodMember)keysToString).IsOverride = true;
                IMalleableExpressionCollection formatCollection = new MalleableExpressionCollection();
                var formatHeader = identityManager.ObtainTypeReference(typeof(string)).GetTypeExpression().GetMethod("Format");
                var formatPrimitive = "".ToPrimitive();
                formatCollection.Add(formatPrimitive);
                StringBuilder formatBuilder = new StringBuilder();
                formatBuilder.Append("{{");
                for (int j = 0; j < typeParameterCopyData.Length - 1; j++)
                {
                    var englishTerm = GetEnglishNumber((uint)(j + 1));
                    var currentTParam = keysType.TypeParameters.Add(typeParameterCopyData[j]);
                    var currentProperty = keysType.Properties.Add(new TypedName(string.Format("Key{0}", j + 1), currentTParam), true, false);
                    var currentField = keysType.Fields.Add(new TypedName(string.Format("_key{0}", j + 1), currentTParam));
                    var currentParameter = keysCtor.Parameters.Add(new TypedName(string.Format("key{0}", j + 1), currentTParam));
                    currentParameter.SummaryText = string.Format("The @T:TKey{0};; the {1} key of the set.", j + 1, englishTerm);
                    currentTParam.SummaryText = string.Format("The type of the {0} key.", englishTerm);
                    currentField.SummaryText = string.Format("Data member for @S:Key{0};.", j + 1);
                    currentProperty.SummaryText = string.Format("Returns the @T:TKey{0};; the {1} key of the set.", j + 1, englishTerm);
                    currentProperty.AccessLevel = AccessLevelModifiers.Public;
                    currentProperty.GetMethod.Return(currentField.GetReference());
                    keysCtor.Assign(currentField.GetReference(), currentParameter.GetReference());
                    currentClass.TypeParameters.Values[j].SummaryText = string.Format("The type of the {0} key of the multiple key dictionary;.", englishTerm);
                    currentInterface.TypeParameters.Values[j].SummaryText = string.Format("The type of the {0} key of the multiple key dictionary.", englishTerm);
                    if (j > 0)
                        formatBuilder.Append(", ");
                    formatBuilder.AppendFormat("{{{0}}}", j);
                    formatCollection.Add(currentProperty.GetReference());
                }
                formatBuilder.Append("}}");
                formatPrimitive.Value = formatBuilder.ToString();
                keysToString.Return(formatHeader.Invoke(formatCollection.ToArray()));
                currentInterface.TypeParameters.Values[i].SummaryText = "The type of the value in the multiple key dictionary.";
                currentClass.TypeParameters.Values[i].SummaryText = "The type of the value in the multiple key dictionary.";
                var interfaceCountProperty = currentInterface.Properties.Add(new TypedName("Count", identityManager.ObtainTypeReference(typeof(int))), true, false);
                interfaceCountProperty.SummaryText = string.Format("Returns the number of elements within the @S:{0}{{{1}}};.", currentInterface.Name, joinedTParams);
                var groupEnglishTerm = GetEnglishNumber((uint)i, false);
                currentClass.SummaryText = string.Format("Provides a base implementation of a {0} key dictionary.", groupEnglishTerm);
                currentInterface.SummaryText = string.Format("Defines properties and methods for working with a multiple keyed dictionary with {0} keys.", groupEnglishTerm);
                keysCtor.AccessLevel = AccessLevelModifiers.Public;
                keysType.Name = string.Format("{0}MultikeyedDictionaryKeys", assembly.Name);
                keysToString.SummaryText = string.Format("Returns the string representation of the current @S:{0}{{{1}}};", keysType.Name, joinedTParams);
                var concatenatedKeys = string.Join(",", from k in keys
                                                        select string.Format("@P:{0};", k.Name));
                keysCtor.SummaryText = string.Format("Creates a new @S:{0}{{{1}}}; with the keys {2} provided.",
                                                 keysType.Name, joinedTParams,
                                                 string.Join(",", (from param in keysCtor.Parameters.Values
                                                                   select string.Format("@P:{0};", param.Name)).ToArray()));

                var classDataCopy = currentClass.Fields.Add(new TypedName("dataCopy", ((IClassType)identityManager.ObtainTypeReference(typeof(Dictionary<,>))).MakeGenericClosure(new TypeCollection(interfaceKeysRefs[0], levels[0]))));
                classDataCopy.SummaryText = string.Format("Represents the root logical pairing between @T:{0}; and @S:{1};.", currentClass.TypeParameters.Values[0].Name, levels[0].Name);
                classDataCopy.InitializationExpression = classDataCopy.FieldType.GetNewExpression();

                var classCountProperty = CreateClassCountProperty(currentClass, currentInterface, levels, joinedTParams, classDataCopy, identityManager);
                var interfaceAdd = currentInterface.Methods.Add(new TypedName("Add", identityManager.ObtainTypeReference(typeof(void))), keysValue);
                var interfaceRemove = currentInterface.Methods.Add(new TypedName("Remove", identityManager.ObtainTypeReference(typeof(bool))), keys);
                var interfaceTryAdd = currentInterface.Methods.Add(new TypedName("TryAdd", identityManager.ObtainTypeReference(typeof(bool))), keysValue);
                interfaceRemove.SummaryText = string.Format("Removes the value with the keys {0}.", concatenatedKeys);
                interfaceTryAdd.SummaryText = string.Format("Attempts to add the @P:{0}; with the keys {1}.", keysValue[i].Name, concatenatedKeys);
                interfaceAdd.SummaryText = string.Format("Adds the @P:{0}; with the keys {1}.", keysValue[i].Name, concatenatedKeys);

                var interfaceKeyAccessor = currentInterface.Indexers.Add(currentInterface.TypeParameters.Values[i], new TypedNameSeries(keys), true, true);
                var interfaceIndexAccessor = currentInterface.Indexers.Add(keysValuePair.MakeGenericClosure(new TypeCollection(keysType.MakeGenericClosure(keysType.TypeParameters.Values.ToCollection()), currentInterface.TypeParameters.Values[i])), new TypedNameSeries(new TypedName("index", identityManager.ObtainTypeReference(typeof(int)))), true, false);
                interfaceIndexAccessor.SummaryText = string.Format("Obtains the @S:{0}{{{1}, {2}}}; at the @P:index; provided.", keysValuePair.Name, keysValueTKeys.Name, keysValueTValue.Name);
                interfaceIndexAccessor.Parameters.Values[0].SummaryText = string.Format("The @S:Int32; value which denotes the zero-based index of the @S:{0}{{{1}, {2}}}; to retrieve.", keysValuePair.Name, keysValueTKeys.Name, keysValueTValue.Name);
                var interfaceTryGetValue = currentInterface.Methods.Add(new TypedName("TryGetValue", identityManager.ObtainTypeReference(typeof(bool))), keysValue);
                var interfaceTryGetValueValueParam = interfaceTryGetValue.Parameters.Values[i];
                var interfaceTryAddValueParam = interfaceTryAdd.Parameters.Values[i];
                var interfaceAddValueParam = interfaceAdd.Parameters.Values[i];
                interfaceAddValueParam.SummaryText = string.Format("The @T:{0}; to add to the @S:{1}{{{2}}};.", keysValue[i].Name, currentInterface.Name, joinedTParams);
                interfaceTryAddValueParam.SummaryText = string.Format("The @T:{0}; to attempt to add to the @S:{1}{{{2}}};.", keysValue[i].Name, currentInterface.Name, joinedTParams);
                interfaceTryGetValueValueParam.SummaryText = string.Format("The @T:{0}; to attempt to retrieve from the @S:{1}{{{2}}};.", keysValue[i].Name, currentInterface.Name, joinedTParams);
                for (int j = 0; j < i; j++)
                {
                    string englishName = GetEnglishNumber((uint)j + 1);
                    var currentKeyAccessorParam = interfaceKeyAccessor.Parameters.Values[j];
                    var currentTryGetValueParam = interfaceTryGetValue.Parameters.Values[j];
                    var currentAddParam = interfaceAdd.Parameters.Values[j];
                    var currentTryAddParam = interfaceTryAdd.Parameters.Values[j];
                    var currentRemoveParam = interfaceRemove.Parameters.Values[j];
                    currentRemoveParam.SummaryText = string.Format("The @T:{0}; which is the {1} key of the element to remove.", typeParameterCopyData[j].Name, GetEnglishNumber((uint)j + 1));
                    currentTryGetValueParam.SummaryText = string.Format("The @T:{0}; which is the {1} key of the @P:{2}; to attempt to retrieve.", typeParameterCopyData[j].Name, englishName, interfaceTryGetValueValueParam.Name);
                    currentKeyAccessorParam.SummaryText = string.Format("The @T:{0}; which is the {1} key of the element to set/retrieve.", typeParameterCopyData[j].Name, englishName);
                    currentAddParam.SummaryText = string.Format("The @T:{0}; which is the {1} key of the @P:{2}; to add.", typeParameterCopyData[j].Name, englishName, interfaceAddValueParam.Name);
                    currentTryAddParam.SummaryText = string.Format("The @T:{0}; which is the {1} key of the @P:{2}; to attempt to add.", typeParameterCopyData[j].Name, englishName, interfaceTryAddValueParam.Name);
                }
                interfaceTryGetValue.Parameters.Values[i].Direction = ParameterCoercionDirection.Out;
                var classTryGetValue = CreateClassTryGetValue(i, currentClass, currentInterface, keysValue, levels, classDataCopy, identityManager);
                classTryGetValue.AccessLevel = AccessLevelModifiers.Public;

                var classKeyAccessor = CreateClassKeyAccessor(i, currentClass, keys, levels, classDataCopy, subkeyNotFound, identityManager);
                CreateClassIndexedAccessor(keysValuePair, i, currentClass, currentInterface, interfaceKeysRefs, levels, keysType, classDataCopy, identityManager);
                CreateClassEnumerator(keysValuePair, i, currentClass, currentInterface, interfaceKeysRefs, levels, keysType, classDataCopy, identityManager);
                interfaceTryGetValue.SummaryText = string.Format("Attempts to retrieve the @P:{1}; with the keys {0}.", concatenatedKeys, keysValue[i].Name);
                interfaceKeyAccessor.SummaryText = string.Format("Returns/sets the value with the keys {0}.", concatenatedKeys);

                var classAddMethod = CreateClassAdd(i, currentClass, keysValue, levels, classDataCopy, currentInterface, joinedTParams, identityManager);
                classAddMethod.SummaryText = string.Format("Adds the @P:{0}; with the keys {1}.", keysValue[i].Name, concatenatedKeys);
                var classTryAddMethod = CreateClassTryAdd(i, currentClass, keysValue, levels, classDataCopy, currentInterface, joinedTParams, identityManager);
                classTryAddMethod.SummaryText = string.Format("Attempts to add the @P:{0}; with the keys {1}.", keysValue[i].Name, concatenatedKeys);
                var classRemoveMethod = CreateClassRemove(i, currentClass, currentInterface, keys, levels, classDataCopy, identityManager);
                var classGetEnumerator2 = currentClass.Methods.Add(new TypedName("GetEnumerator2", identityManager.ObtainTypeReference(typeof(IEnumerator))));
                classGetEnumerator2.Implementations.Add(identityManager.ObtainTypeReference(typeof(IEnumerable)));
                /* Currently due to the way C# works, this 'language specific qualifier' is necessary since the same private implementation isn't handled the same way in VB.NET. */
                classGetEnumerator2.UserSpecificQualifier = "IEnumerable.";
                classGetEnumerator2.Name = "GetEnumerator";
                classGetEnumerator2.Return(new SpecialReferenceExpression(SpecialReferenceKind.This).GetMethod("GetEnumerator").Invoke());
                classKeyAccessor.AccessLevel = AccessLevelModifiers.Public;
                classAddMethod.AccessLevel = AccessLevelModifiers.Public;
                classRemoveMethod.AccessLevel = AccessLevelModifiers.Public;
                classTryAddMethod.AccessLevel = AccessLevelModifiers.Public;
                currentInterface.AccessLevel = AccessLevelModifiers.Public;
                currentInterface.ImplementedInterfaces.Add(((IInterfaceType)identityManager.ObtainTypeReference(typeof(IEnumerable<>))).MakeGenericClosure(new TypeCollection(keysValuePair.MakeGenericClosure(new TypeCollection(keysType.MakeGenericClosure(keysType.TypeParameters.Values.ToCollection()), currentInterface.TypeParameters.Values[i])))));
                currentClass.ImplementedInterfaces.ImplementInterfaceQuick(currentInterface.MakeGenericClosure((from tp in currentClass.TypeParameters.Values
                                                                                                                select (IType)tp).ToArray()));
                resultClasses[i - 2] = currentClass;
                resultInterfaces[i - 2] = currentInterface;

            }
            return new Tuple<IInterfaceType[], IClassType[], IStructType[], IStructType, IIntermediateAssembly>(resultInterfaces, resultClasses, resultedStructs, keysValuePair, assembly);
        }
        private static IPropertyMember CreateClassCountProperty(IIntermediateClassType currentClass, IIntermediateInterfaceType currentInterface, IIntermediateClassType[] levels, string joinedTParams, IIntermediateFieldMember classDataCopy, IIntermediateCliManager identityManager)
        {
            var classCountProperty = currentClass.Properties.Add(new TypedName("Count", identityManager.ObtainTypeReference(typeof(int))), true, false);
            //classCountProperty.ImplementationTypes.Add(currentInterface);
            var countAggregator = classCountProperty.GetMethod.Locals.Add(new TypedName("count", identityManager.ObtainTypeReference(typeof(int))), IntermediateGateway.NumberZero);
            var dataCopyLock = new LockStatement(classCountProperty.GetMethod, classDataCopy.GetReference());
            classCountProperty.GetMethod.Add(dataCopyLock);
            var countEnumerator = dataCopyLock.Enumerate("level1", classDataCopy.GetReference().GetProperty("Values")); //, levels[0].GetTypeReference());
            //countEnumerator.Local.Name = "level1";
            if (levels.Length == 1)
            {
                var levelLock = new LockStatement(countEnumerator, countEnumerator.Local.GetReference());
                countEnumerator.Add(levelLock);
                levelLock.Assign(countAggregator.GetReference(), countAggregator.Add(countEnumerator.Local.GetReference().GetProperty("Count")));
            }
            else
                countEnumerator.Assign(countAggregator.GetReference(), countAggregator.Add(countEnumerator.Local.GetReference().GetProperty("Count")));
            classCountProperty.GetMethod.Return(countAggregator.GetReference());
            classCountProperty.AccessLevel = AccessLevelModifiers.Public;
            classCountProperty.SummaryText = string.Format("Returns the number of elements within the @S:{0}{{{1}}};.", currentClass.Name, joinedTParams);
            return classCountProperty;
        }

        private static void CreateClassEnumerator(IIntermediateStructType keysValuePair, int i, IIntermediateClassType currentClass, IIntermediateInterfaceType currentInterface, IType[] interfaceKeysRefs, IClassType[] levels, IStructType keysType, IIntermediateFieldMember classDataCopy, IIntermediateCliManager identityManager)
        {
            var classEnumerator = currentClass.Methods.Add(new TypedName("GetEnumerator", ((IInterfaceType)identityManager.ObtainTypeReference(typeof(IEnumerator<>))).MakeGenericClosure(new TypeCollection(keysValuePair.MakeGenericClosure(new TypeCollection(keysType.MakeGenericClosure(keysType.TypeParameters.Values.ToCollection()), currentInterface.TypeParameters.Values[i]))))));
            IMemberParentReferenceExpression currentParent = classDataCopy.GetReference();
            IBlockStatementParent currentInsertTarget = classEnumerator;
            IEnumerateSetBreakableBlockStatement currentEnumeration = null;
            IMemberParentReferenceExpression currentLocal = null;
            ILocalReferenceExpression[] enumLocals = new ILocalReferenceExpression[i];
            for (int j = 0; j < i; j++)
            {
                bool last = j == i - 1;
                currentEnumeration = currentInsertTarget.Enumerate(string.Format("item{0}", j + 1), currentParent);
                currentLocal = currentEnumeration.Local.GetReference();
                //if (!last)
                //{
                //    //currentEnumeration = currentInsertTarget.Enumerate(string.Format("item{0}", j + 1), currentParent);
                //    //currentLocal = currentEnumeration.Local.GetReference();
                //}
                //else
                //{
                //    //currentEnumeration = currentInsertTarget.Enumerate(currentParent, ((IStructType)identityManager.ObtainTypeReference(((IStructType)(identityManager.ObtainTypeReference(typeof(KeyValuePair<,>)))))).GetTypeReference(new TypeCollection(currentClass.TypeParameters.Values[j], currentClass.TypeParameters.Values[i])));
                //    //currentEnumeration = currentInsertTarget.Enumerate(string.Format("item{0}", j + 1), currentParent);
                //    //currentLocal = currentEnumeration.Local.GetReference();
                //}
                if (!last)
                    currentEnumeration.Local.Name = string.Format("keyLevelPair{0}", j + 1);
                else
                    currentEnumeration.Local.Name = "keyValuePair";
                enumLocals[j] = (ILocalReferenceExpression)currentLocal;
                currentInsertTarget = currentEnumeration;
                currentParent = currentLocal.GetProperty("Value");
            }
            var newKeysType = keysType.MakeGenericClosure(new TypeCollection(interfaceKeysRefs)).GetNewExpression((from local in enumLocals
                                                                                                                   select local.GetProperty("Key")).ToArray());
            var newKeysValueType = keysValuePair.MakeGenericClosure(new TypeCollection(keysType.MakeGenericClosure(new TypeCollection(interfaceKeysRefs)), currentClass.TypeParameters.Values[i])).GetNewExpression(newKeysType, currentParent);
            currentEnumeration.YieldReturn(newKeysValueType);
            classEnumerator.AccessLevel = AccessLevelModifiers.Public;
        }

        private static void CreateClassIndexedAccessor(IIntermediateStructType keysValuePair, int i, IIntermediateClassType currentClass, IIntermediateInterfaceType currentInterface, IType[] interfaceKeysRefs, IIntermediateClassType[] levels, IIntermediateStructType keysType, IIntermediateFieldMember classDataCopy, IIntermediateCliManager identityManager)
        {
            var classIndexAccessor = currentClass.Indexers.Add(keysValuePair.MakeGenericClosure(new TypeCollection(keysType.MakeGenericClosure(keysType.TypeParameters.Values.ToCollection()), currentInterface.TypeParameters.Values[i])), new TypedNameSeries(new TypedName("index", identityManager.ObtainTypeReference(typeof(int)))), true, false);
            var classIndexAccessorIndex = classIndexAccessor.Parameters.Values[0];
            var classIndexAccessorIndexRef = classIndexAccessorIndex.GetReference();
            var classIndexAccessorGet = classIndexAccessor.GetMethod;
            var currentLocation = classIndexAccessorGet.Locals.Add(new TypedName("location", identityManager.ObtainTypeReference(typeof(int))), IntermediateGateway.NumberZero);
            var currentLocationRef = currentLocation.GetReference();
            IMemberParentReferenceExpression currentParent = classDataCopy.GetReference();
            IBlockStatementParent currentInsertTarget = classIndexAccessorGet;
            IEnumerateSetBreakableBlockStatement currentEnumeration = null;
            IMemberParentReferenceExpression currentLocal = null;
            ILocalReferenceExpression[] enumLocals = new ILocalReferenceExpression[i];
            for (int j = 0; j < i; j++)
            {
                bool last = j == i - 1;
                if (!last)
                {
                    //((IStructType)(identityManager.ObtainTypeReference(typeof(KeyValuePair<,>)))).MakeGenericClosure(new TypeCollection(currentClass.TypeParameters.Values[j].GetTypeReference(), levels[j].GetTypeReference()))
                    currentEnumeration = currentInsertTarget.Enumerate("dummyname", currentParent);
                    currentLocal = currentEnumeration.Local.GetReference();
                }
                else
                {
                    //, ((IStructType)(identityManager.ObtainTypeReference(typeof(KeyValuePair<,>)))).MakeGenericClosure(new TypeCollection(currentClass.TypeParameters.Values[j].GetTypeReference(), currentClass.TypeParameters.Values[i].GetTypeReference()))
                    currentEnumeration = currentInsertTarget.Enumerate("dummyname", currentParent);
                    currentLocal = currentEnumeration.Local.GetReference();
                }
                if (!last)
                    currentEnumeration.Local.Name = string.Format("keyLevelPair{0}", j + 1);
                else
                    currentEnumeration.Local.Name = "keyValuePair";
                enumLocals[j] = (ILocalReferenceExpression)currentLocal;
                currentInsertTarget = currentEnumeration;
                currentParent = currentLocal.GetProperty("Value");
            }

            var finalCheckPoint = currentEnumeration.If(classIndexAccessorIndexRef.EqualTo(currentLocationRef));
            var newKeysType = keysType.MakeGenericClosure(new TypeCollection(interfaceKeysRefs)).GetNewExpression((from local in enumLocals
                                                                                                                   select local.GetProperty("Key")).ToArray());
            var newKeysValueType = keysValuePair.MakeGenericClosure(new TypeCollection(keysType.MakeGenericClosure(new TypeCollection(interfaceKeysRefs)), currentClass.TypeParameters.Values[i])).GetNewExpression(newKeysType, currentParent);
            finalCheckPoint.Return(newKeysValueType);
            finalCheckPoint.Next.Increment(currentLocationRef);//, CrementType.Postfix);
            classIndexAccessor.AccessLevel = AccessLevelModifiers.Public;
            classIndexAccessorGet.Throw(identityManager.ObtainTypeReference(typeof(IndexOutOfRangeException)).GetNewExpression("index".ToPrimitive()));
        }


        private static IIntermediateClassMethodMember CreateClassTryAdd(int i, IIntermediateClassType currentClass, TypedName[] keysValue, IClassType[] levels, IIntermediateFieldMember classDataCopy, IInterfaceType currentInterface, string joinedTParams, IIntermediateCliManager identityManager)
        {
            var classAdd = currentClass.Methods.Add(new TypedName("TryAdd", identityManager.ObtainTypeReference(typeof(bool))), new TypedNameSeries(keysValue));
            //classAdd.Implementations.Add(currentInterface);
            //classAdd.SetMethod.Assign(thisAccessor, classAdd.SetMethod.ValueLocal);
            var valueRef = classAdd.Parameters.Values[i].GetReference();
            IMemberParentReferenceExpression currentTargetExpression = classDataCopy.GetReference();
            ILocalReferenceExpression[] classTryAddLevelRefs = new ILocalReferenceExpression[i - 1];
            IBlockStatementParent currentTargetBlock = classAdd;
            IConditionBlockStatement[] levelConditions = new IConditionBlockStatement[i];
            IMethodInvokeExpression[] failExpressions = new IMethodInvokeExpression[i];
            ILocalMember[] lockChecks = new ILocalMember[i];
            classAdd.Comment("Declare the locals relative to the different levels.");
            for (int j = 0; j < i - 1; j++)
            {
                var currentLocal = classAdd.Locals.Add(new TypedName(string.Format("level{0}", GetEnglishNumberIdentifier((uint)j + 1)), levels[j]));
                classTryAddLevelRefs[j] = currentLocal.GetReference();
                currentLocal.AutoDeclare = false;
                classAdd.Add(currentLocal.GetDeclarationStatement());
            }
            classAdd.Comment(("Check each level individually, if the key set is already present, indicate that nothing was done, and yield; otherwise note that it was inserted."));
            var classTryAddValueParam = classAdd.Parameters.Values[i];
            classTryAddValueParam.SummaryText = string.Format("The @T:{0}; to add to the @S:{1}{{{2}}};", currentClass.TypeParameters.Values[i].Name, currentClass.Name, joinedTParams);
            for (int j = 0; j < i; j++)
            {
                IExpression currentCondition;
                IMemberParentReferenceExpression currentLocal;
                if (j > 0)
                    currentLocal = classTryAddLevelRefs[j - 1];
                else
                    currentLocal = classDataCopy.GetReference();
                IMemberParentReferenceExpression nextLocal;
                classAdd.Parameters.Values[j].SummaryText = string.Format("The @T:{0}; which is the {1} key of the @P:{2}; to add to the @S:{3}{{{4}}};.", currentClass.TypeParameters.Values[j].Name, GetEnglishNumber((uint)j + 1), classTryAddValueParam.Name, currentClass.Name, joinedTParams);
                if (j != i - 1)
                {
                    currentCondition = currentTargetExpression.GetMethod("TryGetValue").Invoke(classAdd.Parameters.Values[j].GetReference(), (nextLocal = classTryAddLevelRefs[j]).Direct(ParameterCoercionDirection.Out));

                    failExpressions[j] = currentLocal.GetMethod("Add").Invoke(classAdd.Parameters.Values[j].GetReference(), nextLocal.Assign(levels[j].GetNewExpression()));
                }
                else
                {
                    currentCondition = currentTargetExpression.GetMethod("ContainsKey").Invoke(classAdd.Parameters.Values[j].GetReference());
                    failExpressions[j] = currentLocal.GetMethod("Add").Invoke(classAdd.Parameters.Values[j].GetReference(), valueRef);
                    nextLocal = null;
                }

                var currentNullCheck = currentTargetBlock.If(classAdd.Parameters.Values[j].GetReference().Cast(identityManager.ObtainTypeReference(typeof(object))).EqualTo(IntermediateGateway.NullValue));
                currentNullCheck.Throw(identityManager.ObtainTypeReference(typeof(ArgumentNullException)).GetNewExpression(classAdd.Parameters.Values[j].Name.ToPrimitive()));

                var currentLockCheckLocal = lockChecks[j] = currentTargetBlock.Locals.Add(new TypedName(string.Format("{0}Lock", j == 0 ? "topLevel" : string.Format("level{0}", GetEnglishNumberIdentifier((uint)j))), identityManager.ObtainTypeReference(typeof(bool))), IntermediateGateway.FalseValue);
                currentLockCheckLocal.AutoDeclare = false;
                currentTargetBlock.DefineLocal(currentLockCheckLocal);
                currentTargetBlock.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Enter").Invoke(currentTargetExpression, currentLockCheckLocal.GetReference().Direct(ParameterCoercionDirection.Reference)));
                var currentIfCondition = currentTargetBlock.If(currentCondition);
                var currentLockCheck = currentIfCondition.If(currentLockCheckLocal.GetReference());
                currentLockCheck.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentTargetExpression));
                if (j == i - 1)
                    currentIfCondition.Return(IntermediateGateway.FalseValue);
                currentTargetExpression = nextLocal;
                currentTargetBlock = levelConditions[j] = currentIfCondition;
            }
            for (int j = i - 1; j >= 0; j--)
            {
                var currentBlock = levelConditions[j];
                for (int k = j; k < i; k++)
                    currentBlock.Next.Call(failExpressions[k]);
                IMemberParentReferenceExpression currentLocal;
                var currentLockCheckLocal = lockChecks[j];
                if (j > 0)
                    currentLocal = classTryAddLevelRefs[j - 1];
                else
                    currentLocal = classDataCopy.GetReference();
                currentBlock.Next.If(currentLockCheckLocal.GetReference()).Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentLocal));
            }
            classAdd.Return(IntermediateGateway.TrueValue);
            return classAdd;
        }

        private static IIntermediateClassMethodMember CreateClassAdd(int i, IIntermediateClassType currentClass, TypedName[] keysValue, IClassType[] levels, IIntermediateFieldMember classDataCopy, IInterfaceType currentInterface, string joinedTParams, IIntermediateCliManager identityManager)
        {
            var classAdd = currentClass.Methods.Add(new TypedName("Add", identityManager.ObtainTypeReference(typeof(void))), keysValue);
            //classAdd.ImplementationTypes.Add(currentInterface);
            //classAdd.SetMethod.Assign(thisAccessor, classAdd.SetMethod.ValueLocal);
            var valueRef = classAdd.Parameters.Values[i].GetReference();
            IMemberParentReferenceExpression currentTargetExpression = classDataCopy.GetReference();
            ILocalReferenceExpression[] classAddLevelRefs = new ILocalReferenceExpression[i - 1];
            IBlockStatementParent currentTargetBlock = classAdd;
            IConditionBlockStatement[] levelConditions = new IConditionBlockStatement[i];
            IMethodInvokeExpression[] failExpressions = new IMethodInvokeExpression[i];
            ILocalMember[] lockChecks = new ILocalMember[i];
            classAdd.Comment(("Declare the locals relative to the different levels."));
            for (int j = 0; j < i - 1; j++)
            {
                var currentLocal = classAdd.Locals.Add(new TypedName(string.Format("level{0}", GetEnglishNumberIdentifier((uint)j + 1)), levels[j]));
                currentLocal.AutoDeclare = false;
                classAddLevelRefs[j] = currentLocal.GetReference();
                classAdd.Add(currentLocal.GetDeclarationStatement());
            }
            classAdd.Comment(("Check each level individually, if the key set is already present, throw an the appropriate exception."));
            var classAddValueParam = classAdd.Parameters.Values[i];
            classAddValueParam.SummaryText = string.Format("The @T:{0}; to add to the @S:{1}{{{2}}};.", currentClass.TypeParameters.Values[i].Name, currentClass.Name, joinedTParams);
            for (int j = 0; j < i; j++)
            {
                IExpression currentCondition;
                IMemberParentReferenceExpression currentLocal;
                classAdd.Parameters.Values[j].SummaryText = string.Format("The @T:{0}; which is the {1} key of the @P:{2}; to add to the @S:{3}{{{4}}};.", currentClass.TypeParameters.Values[j].Name, GetEnglishNumber((uint)j + 1), classAddValueParam.Name, currentClass.Name, joinedTParams);
                if (j > 0)
                    currentLocal = classAddLevelRefs[j - 1];
                else
                    currentLocal = classDataCopy.GetReference();
                IMemberParentReferenceExpression nextLocal;
                if (j != i - 1)
                {
                    currentCondition = currentTargetExpression.GetMethod("TryGetValue").Invoke(classAdd.Parameters.Values[j].GetReference(), (nextLocal = classAddLevelRefs[j]).Direct(ParameterCoercionDirection.Out));

                    failExpressions[j] = currentLocal.GetMethod("Add").Invoke(classAdd.Parameters.Values[j].GetReference(), nextLocal.Assign(levels[j].GetNewExpression()));
                }
                else
                {
                    currentCondition = currentTargetExpression.GetMethod("ContainsKey").Invoke(classAdd.Parameters.Values[j].GetReference());
                    failExpressions[j] = currentLocal.GetMethod("Add").Invoke(classAdd.Parameters.Values[j].GetReference(), valueRef);
                    nextLocal = null;
                }
                var currentNullCheck = currentTargetBlock.If(classAdd.Parameters.Values[j].GetReference().Cast(identityManager.ObtainTypeReference(typeof(object))).EqualTo(IntermediateGateway.NullValue));
                currentNullCheck.Throw(identityManager.ObtainTypeReference(typeof(ArgumentNullException)).GetNewExpression(classAdd.Parameters.Values[j].Name.ToPrimitive()));

                var currentLockCheckLocal = lockChecks[j] = currentTargetBlock.Locals.Add(new TypedName(string.Format("{0}Lock", j == 0 ? "topLevel" : string.Format("level{0}", GetEnglishNumberIdentifier((uint)j))), identityManager.ObtainTypeReference(typeof(bool))), IntermediateGateway.FalseValue);
                currentLockCheckLocal.AutoDeclare = false;
                currentTargetBlock.DefineLocal(currentLockCheckLocal);

                currentTargetBlock.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Enter").Invoke(currentTargetExpression, currentLockCheckLocal.GetReference().Direct(ParameterCoercionDirection.Reference)));
                var currentIfCondition = currentTargetBlock.If(currentCondition);
                var currentLockCheck = currentIfCondition.If(currentLockCheckLocal.GetReference());
                currentLockCheck.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentTargetExpression));
                if (j == i - 1)
                    currentIfCondition.Throw(identityManager.ObtainTypeReference(typeof(InvalidOperationException)).GetNewExpression());
                if (nextLocal != null)
                    currentTargetExpression = nextLocal;
                currentTargetBlock = levelConditions[j] = currentIfCondition;
            }
            for (int j = i - 1; j >= 0; j--)
            {
                var currentBlock = levelConditions[j];
                for (int k = j; k < i; k++)
                    currentBlock.Next.Call(failExpressions[k]);
                IMemberParentReferenceExpression currentLocal;
                var currentLockCheckLocal = lockChecks[j];
                if (j > 0)
                    currentLocal = classAddLevelRefs[j - 1];
                else
                    currentLocal = classDataCopy.GetReference();
                currentBlock.Next.If(currentLockCheckLocal.GetReference()).Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentLocal));
            }

            return classAdd;
        }

        private static IIntermediateClassMethodMember CreateClassTryGetValue(int i, IIntermediateClassType currentClass, IIntermediateInterfaceType currentInterface, TypedName[] keysValue, IIntermediateClassType[] levels, IIntermediateFieldMember classDataCopy, IIntermediateCliManager identityManager)
        {
            var classTryGetValue = currentClass.Methods.Add(new TypedName("TryGetValue", identityManager.ObtainTypeReference(typeof(bool))), new TypedNameSeries(keysValue));
            //classTryGetValue.ImplementationTypes.Add(currentInterface);
            classTryGetValue.SummaryText = string.Format("Attempts to retrieve the @P:{1}; with the keys {0}.", string.Join(",", from k in keysValue.Take(i)
                                                                                                                                 select string.Format("@P:{0};", k.Name)), keysValue[i].Name);
            classTryGetValue.Parameters.Values[i].Direction = ParameterCoercionDirection.Out;// FieldDirection.Out;
            IMemberParentReferenceExpression currentTargetExpression = classDataCopy.GetReference();
            ILocalReferenceExpression[] tryGetValueLevelRefs = new ILocalReferenceExpression[i - 1];
            for (int j = 0; j < i - 1; j++)
                tryGetValueLevelRefs[j] = classTryGetValue.Locals.Add(new TypedName(string.Format("level{0}", GetEnglishNumberIdentifier((uint)j + 1)), levels[j])).GetReference();
            IBlockStatementParent currentTargetBlock = classTryGetValue;
            //IExpression fullCondition = null;
            for (int j = 0; j < i; j++)
            {
                IMemberParentReferenceExpression nextLocal = null;
                IExpression currentCondition;
                classTryGetValue.Parameters.Values[j].SummaryText = string.Format("The @T:{0}; which is the {1} key of the @P:{2}; to attempt to retrieve.", currentClass.TypeParameters.Values[j].Name, GetEnglishNumber((uint)j + 1), keysValue[i].Name);
                currentCondition = currentTargetExpression.GetMethod("TryGetValue").Invoke(classTryGetValue.Parameters.Values[j].GetReference(), (j != i - 1 ? (nextLocal = tryGetValueLevelRefs[j]) : classTryGetValue.Parameters.Values[i].GetReference()).Direct(ParameterCoercionDirection.Out));
                var currentLockCheckLocal = currentTargetBlock.Locals.Add(new TypedName(string.Format("{0}Lock", j == 0 ? "topLevel" : string.Format("level{0}", GetEnglishNumberIdentifier((uint)j))), identityManager.ObtainTypeReference(typeof(bool))), IntermediateGateway.FalseValue);


                var currentNullCheck = currentTargetBlock.If(classTryGetValue.Parameters.Values[j].GetReference().Cast(identityManager.ObtainTypeReference(typeof(object))).EqualTo(IntermediateGateway.NullValue));
                currentNullCheck.Throw(identityManager.ObtainTypeReference(typeof(ArgumentNullException)).GetNewExpression(classTryGetValue.Parameters.Values[j].Name.ToPrimitive()));
                currentLockCheckLocal.AutoDeclare = false;
                currentTargetBlock.DefineLocal(currentLockCheckLocal);
                currentTargetBlock.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Enter").Invoke(currentTargetExpression, currentLockCheckLocal.GetReference().Direct(ParameterCoercionDirection.Reference)));

                var currentConditionStatement = currentTargetBlock.If(currentCondition);
                var currentLockCheck = currentConditionStatement.If(currentLockCheckLocal.GetReference());
                currentLockCheck.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentTargetExpression));
                currentConditionStatement.Next.If(currentLockCheckLocal.GetReference()).Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentTargetExpression));
                currentTargetBlock = currentConditionStatement;
                //if (fullCondition == null)
                //    fullCondition = currentCondition;
                //else
                //    fullCondition = new BinaryOperationExpression(fullCondition, CodeBinaryOperatorType.BooleanAnd, currentCondition);

                currentTargetExpression = nextLocal;
            }

            classTryGetValue.Parameters.Values[i].SummaryText = string.Format("The @T:{0}; which is the reference to the value to attempt to retrieve.", currentClass.TypeParameters.Values[i].Name);
            currentTargetBlock.Return(IntermediateGateway.TrueValue);
            classTryGetValue.Assign(classTryGetValue.Parameters.Values[i].GetReference(), new DefaultValueExpression(classTryGetValue.Parameters.Values[i].ParameterType));
            classTryGetValue.Return(IntermediateGateway.FalseValue);
            return classTryGetValue;
        }
        private static IIntermediateClassMethodMember CreateClassRemove(int i, IIntermediateClassType currentClass, IIntermediateInterfaceType currentInterface, TypedName[] keys, IIntermediateClassType[] levels, IIntermediateFieldMember classDataCopy, IIntermediateCliManager identityManager)
        {
            var classRemove = currentClass.Methods.Add(new TypedName("Remove", identityManager.ObtainTypeReference(typeof(bool))), keys);
            //classRemove.ImplementationTypes.Add(currentInterface);
            IMemberParentReferenceExpression currentTargetExpression = classDataCopy.GetReference();
            ILocalReferenceExpression[] removeLevelRefs = new ILocalReferenceExpression[i - 1];
            IBlockStatementParent currentTargetBlock = classRemove;
            ILocalMember[] lockChecks = new ILocalMember[i];
            IConditionBlockStatement[] lockConditionStatements = new IConditionBlockStatement[i];
            classRemove.SummaryText = string.Format("Removes the value with the keys {0}.", string.Join(",", from k in keys
                                                                                                             select string.Format("@P:{0};", k.Name)));
            for (int j = 0; j < i - 1; j++)
                removeLevelRefs[j] = classRemove.Locals.Add(new TypedName(string.Format("level{0}", GetEnglishNumberIdentifier((uint)j + 1)), levels[j])).GetReference();
            for (int j = 0; j < i; j++)
            {
                IExpression currentCondition;
                if (j != i - 1)
                    currentCondition = currentTargetExpression.GetMethod("TryGetValue").Invoke(classRemove.Parameters.Values[j].GetReference(), removeLevelRefs[j].Direct(ParameterCoercionDirection.Out));
                else
                    currentCondition = currentTargetExpression.GetMethod("ContainsKey").Invoke(classRemove.Parameters.Values[j].GetReference());


                var currentNullCheck = currentTargetBlock.If(classRemove.Parameters.Values[j].GetReference().Cast(identityManager.ObtainTypeReference(typeof(object))).EqualTo(IntermediateGateway.NullValue));
                currentNullCheck.Throw(identityManager.ObtainTypeReference(typeof(ArgumentNullException)).GetNewExpression(classRemove.Parameters.Values[j].Name.ToPrimitive()));

                var currentLockCheckLocal = lockChecks[j] = currentTargetBlock.Locals.Add(new TypedName(string.Format("{0}Lock", j == 0 ? "topLevel" : string.Format("level{0}", GetEnglishNumberIdentifier((uint)j))), identityManager.ObtainTypeReference(typeof(bool))), IntermediateGateway.FalseValue);
                currentLockCheckLocal.AutoDeclare = false;
                currentTargetBlock.DefineLocal(currentLockCheckLocal);
                currentTargetBlock.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Enter").Invoke(currentTargetExpression, currentLockCheckLocal.GetReference().Direct(ParameterCoercionDirection.Reference)));
                var currentIfCondition = currentTargetBlock.If(currentCondition);
                IConditionBlockStatement currentLockCheck = null;
                if (j != i - 1)
                    lockConditionStatements[j] = currentLockCheck = currentIfCondition.If(currentLockCheckLocal.GetReference());
                else
                    lockConditionStatements[j] = currentLockCheck = new ConditionBlockStatement(currentIfCondition) { Condition = currentLockCheckLocal.GetReference() };

                currentLockCheck.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentTargetExpression));
                currentIfCondition.Next.If(currentLockCheckLocal.GetReference()).Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentTargetExpression));
                if (j != i - 1)
                    currentTargetExpression = removeLevelRefs[j];
                currentTargetBlock = currentIfCondition;
            }
            IBlockStatementParent returnTarget = currentTargetBlock;
            //currentTargetBlock.Add(lastLockCheck);
            //currentTargetBlock.Return(IntermediateGateway.TrueValue);
            for (int j = i - 1; j >= 0; j--)
            {
                IMemberParentReferenceExpression nextLevel;
                if (j == 0)
                    currentTargetExpression = classDataCopy.GetReference();
                else
                    currentTargetExpression = removeLevelRefs[j - 1];
                if (j != i - 1)
                    nextLevel = removeLevelRefs[j];
                else
                    nextLevel = null;
                IBlockStatementParent removeTargetBlock;
                if (nextLevel != null)
                {
                    currentTargetBlock.Comment((string.Format("When the items on level {0} reach zero, it's time to release it.", GetEnglishNumber((uint)j + 1, false))));

                    IConditionBlockStatement currentCondition = currentTargetBlock.If(nextLevel.Cast(levels[j].BaseType).GetProperty("Count").EqualTo(IntermediateGateway.NumberZero));
                    var currentLockExit = lockConditionStatements[j + 1];
                    currentCondition.Add(currentLockExit);
                    currentCondition.Assign(lockChecks[j].GetReference(), IntermediateGateway.FalseValue);
                    currentCondition.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Enter").Invoke(currentTargetExpression, lockChecks[j].GetReference().Direct(ParameterCoercionDirection.Reference)));
                    currentCondition.Next.Add(currentLockExit);
                    currentCondition.Comment((string.Format("To ensure that the state is still valid, re-check the key at {0} since a sequential remove statement might have already removed it since the lock above was released.", j == 0 ? "the top level" : string.Format("level {0}", GetEnglishNumber((uint)j, false)))));
                    currentCondition = currentCondition.If(currentTargetExpression.GetMethod("ContainsKey").Invoke(classRemove.Parameters.Values[j].GetReference()));
                    removeTargetBlock = currentCondition;

                }
                else
                    removeTargetBlock = currentTargetBlock;
                removeTargetBlock.Call(currentTargetExpression.GetMethod("Remove").Invoke(classRemove.Parameters.Values[j].GetReference()));
                currentTargetBlock = removeTargetBlock;
                if (j == 0)
                    removeTargetBlock.Add(lockConditionStatements[0]);
            }
            returnTarget.Return(IntermediateGateway.TrueValue);
            classRemove.Return(IntermediateGateway.FalseValue);
            return classRemove;
        }

        private static IIntermediateClassIndexerMember CreateClassKeyAccessor(int i, IIntermediateClassType currentClass, TypedName[] keys, IIntermediateClassType[] levels, IIntermediateFieldMember classDataCopy, IIntermediateClassType subkeyNotFound, IIntermediateCliManager identityManager)
        {
            var classAccessor = currentClass.Indexers.Add(currentClass.TypeParameters.Values[i], new TypedNameSeries(keys), true, true);
            classAccessor.SummaryText = string.Format("Returns/sets the value with the {0} provided.", string.Join(",", from k in keys
                                                                                                                        select string.Format("@P:{0};", k.Name)));
            CreateClassKeyGetAccessor(classAccessor, i, currentClass, keys, levels, classDataCopy, subkeyNotFound, identityManager);

            var accessorSetPart = classAccessor.SetMethod;
            CreateClassKeySetAccessor(i, currentClass, levels, classDataCopy, classAccessor, accessorSetPart, identityManager);

            return classAccessor;
        }

        private static void CreateClassKeySetAccessor(int i, IIntermediateClassType currentClass, IIntermediateClassType[] levels, IIntermediateFieldMember classDataCopy, IIntermediateClassIndexerMember classAccessor, IIntermediatePropertySignatureSetMethodMember accessorSetPart, IIntermediateCliManager identityManager)
        {
            IMemberParentReferenceExpression currentTargetExpression = classDataCopy.GetReference();
            ILocalReferenceExpression[] accessorLevelRefs = new ILocalReferenceExpression[i - 1];
            IBlockStatementParent currentTargetBlock = (IBlockStatementParent)accessorSetPart;
            IConditionBlockStatement[] levelConditions = new IConditionBlockStatement[i];
            IMethodInvokeExpression[] failExpressions = new IMethodInvokeExpression[i];
            ILocalMember[] lockChecks = new ILocalMember[i];
            for (int j = 0; j < i - 1; j++)
                accessorLevelRefs[j] = ((IIntermediateMethodMember)accessorSetPart).Locals.Add(new TypedName(string.Format("level{0}", GetEnglishNumberIdentifier((uint)j + 1)), levels[j])).GetReference();
            for (int j = 0; j < i; j++)
            {
                IExpression currentCondition;
                IMemberParentReferenceExpression currentLocal;
                if (j > 0)
                    currentLocal = accessorLevelRefs[j - 1];
                else
                    currentLocal = classDataCopy.GetReference();
                IMemberParentReferenceExpression nextLocal;
                classAccessor.Parameters.Values[j].SummaryText = string.Format("The @T:{0}; which is the {1} key of the element to set/retrieve.", currentClass.TypeParameters.Values[j].Name, GetEnglishNumber((uint)j + 1));
                if (j != i - 1)
                {
                    currentCondition = currentTargetExpression.GetMethod("TryGetValue").Invoke(classAccessor.Parameters.Values[j].GetReference(), (nextLocal = accessorLevelRefs[j]).Direct(ParameterCoercionDirection.Out));
                    failExpressions[j] = currentLocal.GetMethod("Add").Invoke(classAccessor.Parameters.Values[j].GetReference(), nextLocal.Assign(levels[j].GetNewExpression()));
                }
                else
                {
                    currentCondition = currentTargetExpression.GetMethod("ContainsKey").Invoke(classAccessor.Parameters.Values[j].GetReference());
                    failExpressions[j] = currentLocal.GetMethod("Add").Invoke(classAccessor.Parameters.Values[j].GetReference(), accessorSetPart.ValueParameter.GetReference());
                    nextLocal = null;
                }

                var currentNullCheck = currentTargetBlock.If(classAccessor.Parameters.Values[j].GetReference().Cast(identityManager.ObtainTypeReference(typeof(object))).EqualTo(IntermediateGateway.NullValue));
                currentNullCheck.Throw(identityManager.ObtainTypeReference(typeof(ArgumentNullException)).GetNewExpression(classAccessor.Parameters.Values[j].Name.ToPrimitive()));

                var currentLockCheckLocal = lockChecks[j] = currentTargetBlock.Locals.Add(new TypedName(string.Format("{0}Lock", j == 0 ? "topLevel" : string.Format("level{0}", GetEnglishNumberIdentifier((uint)j))), identityManager.ObtainTypeReference(typeof(bool))), IntermediateGateway.FalseValue);
                currentLockCheckLocal.AutoDeclare = false;
                currentTargetBlock.DefineLocal(currentLockCheckLocal);
                currentTargetBlock.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Enter").Invoke(currentTargetExpression, currentLockCheckLocal.GetReference().Direct(ParameterCoercionDirection.Reference)));

                var currentIfCondition = currentTargetBlock.If(currentCondition);

                var currentLockCheck = currentIfCondition.If(currentLockCheckLocal.GetReference());
                currentLockCheck.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentTargetExpression));
                if (j == i - 1)
                    currentIfCondition.Assign(currentLocal.GetIndexer(classAccessor.Parameters.Values[j].GetReference()), accessorSetPart.ValueParameter.GetReference());
                currentTargetExpression = nextLocal;
                currentTargetBlock = levelConditions[j] = currentIfCondition;
            }
            for (int j = i - 1; j >= 0; j--)
            {
                var currentBlock = levelConditions[j];
                for (int k = j; k < i; k++)
                    currentBlock.Next.Call(failExpressions[k]);
                IMemberParentReferenceExpression currentLocal;
                var currentLockCheckLocal = lockChecks[j];
                if (j > 0)
                    currentLocal = accessorLevelRefs[j - 1];
                else
                    currentLocal = classDataCopy.GetReference();
                currentBlock.Next.If(currentLockCheckLocal.GetReference()).Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentLocal));
            }
        }

        private static void CreateClassKeyGetAccessor(IIntermediateClassIndexerMember property, int i, IIntermediateClassType currentClass, TypedName[] keys, IIntermediateClassType[] levels, IIntermediateFieldMember classDataCopy, IClassType subkeyNotFound, IIntermediateCliManager identityManager)
        {
            var getAccessor = property.GetMethod;
            IMemberParentReferenceExpression currentTargetExpression = classDataCopy.GetReference();
            ILocalReferenceExpression[] tryGetValueLevelRefs = new ILocalReferenceExpression[i - 1];
            for (int j = 0; j < i - 1; j++)
                tryGetValueLevelRefs[j] = getAccessor.Locals.Add(new TypedName(string.Format("level{0}", GetEnglishNumberIdentifier((uint)j + 1)), levels[j])).GetReference();
            IBlockStatementParent currentTargetBlock = getAccessor;
            //IExpression fullCondition = null;
            ILocalMember valueLocal = null;
            for (int j = 0; j < i; j++)
            {
                IMemberParentReferenceExpression nextLocal = null;
                IExpression currentCondition;
                if (j == i - 1)
                {
                    valueLocal = currentTargetBlock.Locals.Add(new TypedName("value", property.PropertyType));
                    currentCondition = currentTargetExpression.GetMethod("TryGetValue").Invoke(property.Parameters.Values[j].GetReference(), valueLocal.GetReference().Direct(ParameterCoercionDirection.Out));
                }
                else
                    currentCondition = currentTargetExpression.GetMethod("TryGetValue").Invoke(property.Parameters.Values[j].GetReference(), (nextLocal = tryGetValueLevelRefs[j]).Direct(ParameterCoercionDirection.Out));
                var currentLockCheckLocal = currentTargetBlock.Locals.Add(new TypedName(string.Format("{0}Lock", j == 0 ? "topLevel" : string.Format("level{0}", GetEnglishNumberIdentifier((uint)j))), identityManager.ObtainTypeReference(typeof(bool))), IntermediateGateway.FalseValue);
                currentLockCheckLocal.AutoDeclare = false;

                var currentNullCheck = currentTargetBlock.If(property.Parameters.Values[j].GetReference().Cast(identityManager.ObtainTypeReference(typeof(object))).EqualTo(IntermediateGateway.NullValue));
                currentNullCheck.Throw(identityManager.ObtainTypeReference(typeof(ArgumentNullException)).GetNewExpression(property.Parameters.Values[j].Name.ToPrimitive()));

                currentTargetBlock.DefineLocal(currentLockCheckLocal);
                currentTargetBlock.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Enter").Invoke(currentTargetExpression, currentLockCheckLocal.GetReference().Direct(ParameterCoercionDirection.Reference)));

                var currentConditionStatement = currentTargetBlock.If(currentCondition);
                var currentLockCheck = currentConditionStatement.If(currentLockCheckLocal.GetReference());
                currentLockCheck.Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentTargetExpression));

                var failCondition = currentConditionStatement.Next;
                failCondition.If(currentLockCheckLocal.GetReference()).Call(identityManager.ObtainTypeReference(typeof(Monitor)).GetTypeExpression().GetMethod("Exit").Invoke(currentTargetExpression));
                failCondition.Throw(subkeyNotFound.GetNewExpression(property.Parameters.Values[j].Name.ToPrimitive()));
                currentTargetBlock = currentConditionStatement;

                currentTargetExpression = nextLocal;
            }
            currentTargetBlock.Return(valueLocal.GetReference());
        }

        private static IIndexerReferenceExpression thisAccessorResult(IFieldReferenceExpression rootDictionary, IParameterReferenceExpression[] parameters)
        {
            IMemberParentReferenceExpression currentExpression = rootDictionary;
            for (int i = 0; i < parameters.Length; i++)
                currentExpression = currentExpression.GetIndexer(parameters[i]);
            return (IIndexerReferenceExpression)currentExpression;
        }
    }
}
