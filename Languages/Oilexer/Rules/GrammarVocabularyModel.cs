
#if x64
#if HalfWord
using SlotType = System.UInt32;
#else
using SlotType = System.UInt64;
#endif
#elif x86
#if HalfWord
using SlotType = System.UInt16;
#else
using SlotType = System.UInt32;
#endif
#endif
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
using AllenCopeland.Abstraction.Utilities.Arrays;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using System.Diagnostics;
using AllenCopeland.Abstraction.Slf.Cli;
using System.ComponentModel;
using System.IO;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Ast.Cli;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Responsible for creating the AST which represents a given grammar's <see cref="IGrammarSymbolSet"/>, as well as
    /// embedding concrete instances of <see cref="GrammarVocabulary"/> into the AST.
    /// </summary>
    public class GrammarVocabularyModelBuilder
    {
        private ControlledDictionary<IGrammarRuleSymbol, IOilexerGrammarProductionRuleEntry> ruleSymbolToRuleLookup;
        private ControlledDictionary<IOilexerGrammarProductionRuleEntry, IGrammarRuleSymbol> symbolToRuleLookup;
        private MultikeyedDictionary<string, GrammarVocabulary, IIntermediateClassPropertyMember> symbolStoreCache = new MultikeyedDictionary<string, GrammarVocabulary, IIntermediateClassPropertyMember>();
        private MultikeyedDictionary<string, GrammarVocabulary, IIntermediateClassFieldMember> symbolStoreCacheFields = new MultikeyedDictionary<string, GrammarVocabulary, IIntermediateClassFieldMember>();
        private MultikeyedDictionary<string, GrammarVocabulary, IIntermediateEnumFieldMember> symbolStoreCacheVariations = new MultikeyedDictionary<string, GrammarVocabulary, IIntermediateEnumFieldMember>();
        private Dictionary<string, int> SymbolStoreCount = new Dictionary<string, int>();
        private IIntermediateStructType validSymbols;
        private IIntermediateEnumType[] validSymbolsEnums;
        private IIntermediateEnumFieldMember[] validSymbolsEnumNones;
        private ControlledDictionary<IOilexerGrammarTokenEntry, IIntermediateStructPropertyMember> hasAnyLookup;
        private ControlledDictionary<IGrammarSymbol, IIntermediateEnumFieldMember> validSymbolLookup;
        private ControlledDictionary<IGrammarSymbol, IIntermediateEnumFieldMember> identitySymbolLookup;
        private ControlledDictionary<IGrammarSymbol, IIntermediateClassFieldMember> singletonLookup;
        private OilexerGrammarFile sourceOrigin;
        private ParserCompiler _compiler;
        private IGrammarSymbol[][] symbolChunks;

        private IIntermediateStructPropertyMember[] validSymbolProperties;
        //private IIntermediateEnumType symbolStoreVariations;
        private string _contextualName;
        private Predicate<IGrammarSymbol> _filter;
        private bool addNoneAndMultipleIdentities;

        public GrammarVocabularyModelBuilder(IGrammarSymbolSet symbols, IIntermediateAssembly project, OilexerGrammarFile sourceOrigin, ParserCompiler compiler, string contextualName, Predicate<IGrammarSymbol> filter, bool addNoneAndMultipleIdentities)
        {
            this._contextualName = contextualName;
            this._filter = filter;
            this._compiler = compiler;
            this.Project = project;
            this.Symbols = symbols;
            this.sourceOrigin = sourceOrigin;
            this.addNoneAndMultipleIdentities = addNoneAndMultipleIdentities;
        }

        public IControlledDictionary<IGrammarRuleSymbol, IOilexerGrammarProductionRuleEntry> RuleSymbolToRuleLookup
        {
            get
            {
                this.CheckValidSymbols();
                if (this.ruleSymbolToRuleLookup == null)
                {
                    this.ruleSymbolToRuleLookup = new ControlledDictionary<IGrammarRuleSymbol, IOilexerGrammarProductionRuleEntry>();
                }
                return this.ruleSymbolToRuleLookup;
            }
        }

        public IControlledDictionary<IOilexerGrammarProductionRuleEntry, IGrammarRuleSymbol> SymbolToRuleLookup
        {
            get
            {
                this.CheckValidSymbols();
                if (this.symbolToRuleLookup == null)
                {
                    this.symbolToRuleLookup = new ControlledDictionary<IOilexerGrammarProductionRuleEntry, IGrammarRuleSymbol>();
                }
                return this.symbolToRuleLookup;
            }
        }

        public IIntermediateStructType ValidSymbols
        {
            get
            {
                CheckValidSymbols();
                return this.validSymbols;
            }
        }

        public IIntermediateEnumFieldMember GetValidSymbolField(IGrammarSymbol symbol)
        {
            this.CheckValidSymbols();
            return this.validSymbolLookup[symbol];
        }

        public IIntermediateEnumFieldMember GetIdentitySymbolField(IGrammarSymbol symbol)
        {
            this.CheckValidSymbols();

            return this.identitySymbolLookup[symbol];
        }

        public IEnumerable<IIntermediateEnumFieldMember> GetIdentitySymbolsFields(IEnumerable<IGrammarSymbol> symbols)
        {
            this.CheckValidSymbols();
            foreach (var symbol in symbols)
                yield return this.identitySymbolLookup[symbol];
        }

        public IEnumerable<IExpression> GetIdentitySymbolsFieldReferences(IEnumerable<IGrammarSymbol> symbols)
        {
            this.CheckValidSymbols();
            foreach (var symbol in symbols)
                yield return (IExpression)this.identitySymbolLookup[symbol].GetReference();
        }

        public IIntermediateEnumFieldMember GetEofIdentityField()
        {
            this.CheckValidSymbols();
            var current = from s in this.Symbols.Where(k => this._filter(k))
                          where s is IGrammarTokenSymbol
                          let gts = (IGrammarTokenSymbol)s
                          where gts.Source is IOilexerGrammarTokenEofEntry
                          select gts;
            var firstOrDefEof = current.FirstOrDefault();
            if (firstOrDefEof == null)
                return null;
            return this.GetIdentitySymbolField(firstOrDefEof);
        }

        internal void CheckValidSymbols()
        {
            if (this.validSymbols == null)
                this.validSymbols = CreateValidSymbolsStruct();
        }

        public IEnumerable<IIntermediateEnumType> ValidSymbolsEnums
        {
            get
            {
                this.CheckValidSymbols();
                foreach (var validEnum in this.validSymbolsEnums)
                    yield return validEnum;
            }
        }

        private IIntermediateStructType CreateValidSymbolsStruct()
        {
            /* *
             * A little setup, breakdown the symbols into a series of
             * processor-dependent word size sets.  Each symbol will
             * take one bit per set up to n per set, where n is the
             * number of bytes of the architecture word size, multiplied
             * by the number of bits per byte.
             * */
            var symbolChunks = this.Symbols.Where(k => k != null && this._filter(k)).ToArray().Chunk(sizeof(SlotType) * 8);
            IIntermediateEnumType[] validEnums = new IIntermediateEnumType[symbolChunks.Length];
            IIntermediateEnumFieldMember[] validNones = new IIntermediateEnumFieldMember[symbolChunks.Length];
            //this.symbolStoreVariations = Project.DefaultNamespace.Parts.Add().Enums.Add("SymbolStoreVariations");

            this.symbolChunks = symbolChunks;
            if (this.addNoneAndMultipleIdentities)
            {
                this.NoIdentityField = this._compiler.SymbolStoreBuilder.Identities.Fields.Add(GetUniqueEnumFieldName("None", this._compiler.SymbolStoreBuilder.Identities));
                this.NoIdentityField.SummaryText = "Represents no identity within the language, used for a default state to compare against for identity determination.";
            }

            this._compiler.SymbolStoreBuilder.Identities.AccessLevel = AccessLevelModifiers.Public;
            /* *
             * Basic patterning, makes it consistent.
             * */
            var numZeroes = symbolChunks.Length.ToString().Length;
            var zeroFormat = string.Format("{{0:{0}}}", new string('0', numZeroes));
            this.validSymbolLookup = new ControlledDictionary<IGrammarSymbol, IIntermediateEnumFieldMember>();
            this.identitySymbolLookup = new ControlledDictionary<IGrammarSymbol, IIntermediateEnumFieldMember>();
            this.singletonLookup = new ControlledDictionary<IGrammarSymbol, IIntermediateClassFieldMember>();
            IIntermediateStructType validSymbols = Project.DefaultNamespace.Parts.Add().Structs.Add("{0}Valid{1}", SourceOrigin.Options.AssemblyName, this._contextualName);
            validSymbols.AccessLevel = AccessLevelModifiers.Public;
            validSymbolProperties = new IIntermediateStructPropertyMember[symbolChunks.Length];
            /* *
             * Requires more than one pass.
             * Cannot directly refer to components of a model 
             * that are not yet created.
             * */
            for (int symbolChunkIndex = 0, startIndex = 1; symbolChunkIndex < symbolChunks.Length; startIndex += symbolChunks[symbolChunkIndex++].Length)
            {
                var currentChunk = symbolChunks[symbolChunkIndex];
                var currentEnum = validEnums[symbolChunkIndex] = this.Project.DefaultNamespace.Parts.Add().Enums.Add(string.Format("{{1}}Valid{{2}}{0}", zeroFormat), symbolChunkIndex + 1, SourceOrigin.Options.AssemblyName, this._contextualName);
                currentEnum.AccessLevel = AccessLevelModifiers.Public;
                currentEnum.Metadata.Add(new MetadatumDefinitionParameterValueCollection(currentEnum.IdentityManager.ObtainTypeReference(currentEnum.IdentityManager.ObtainTypeReference(RuntimeCoreType.RootType).Assembly.UniqueIdentifier.GetTypeIdentifier(typeof(FlagsAttribute).Namespace, typeof(FlagsAttribute).Name))));
                IIntermediateEnumFieldMember currentNone = validNones[symbolChunkIndex] = currentEnum.Fields.Add(GetUniqueEnumFieldName("None", currentEnum), (SlotType)0);

                GenerateChunkSymbols(currentChunk, currentEnum);

                IIntermediateStructPropertyMember currentValidSetProperty = GenerateSymbolSetChunkProperty(zeroFormat, validSymbols, symbolChunkIndex, currentNone);
                currentValidSetProperty.SummaryText = string.Format("Returns the @s:{3}; value which is the {0} set of elements (symbols {1} through {2})", SetIndexToSetName(symbolChunkIndex), startIndex, startIndex + currentChunk.Length - 1, currentValidSetProperty.PropertyType.Name);
                currentValidSetProperty.RemarksText = string.Format("This may return @s:{0}.{1}; if no symbols within this subset are present.", currentNone.Parent.Name, currentNone.Name);
                validSymbolProperties[symbolChunkIndex] = currentValidSetProperty;
            }
            this.validSymbolsEnumNones = validNones;
            this.validSymbolsEnums = validEnums;
            /* *
             * Symbol store singleton creation.
             * */
            ConstructSymbolStoreSingletons(symbolChunks, validEnums, validNones, this._compiler.SymbolStoreBuilder.Singletons, validSymbols);

            this.hasAnyLookup = new ControlledDictionary<IOilexerGrammarTokenEntry, IIntermediateStructPropertyMember>(ObtainHasAnySet(validEnums, validNones, validSymbols));

            CreateIsEmptyProperty(symbolChunks, validNones, validSymbols);

            CreateValidSetBinaryOperators(symbolChunks, validSymbols);

            if (this.addNoneAndMultipleIdentities)
            {
                this.MultipleIdentitiesIdentityField = this._compiler.SymbolStoreBuilder.Identities.Fields.Add(GetUniqueEnumFieldName("MultipleIdentities", this._compiler.SymbolStoreBuilder.Identities));
                this.MultipleIdentitiesIdentityField.SummaryText = "Represents the identity when performing identity determination that multiple identities are possible at the current juncture, and an assertion should fail.";
                CreateValidToIdentityOperator(symbolChunks, validSymbols);
                this.StreamAccessOutOfSequence = this._compiler.SymbolStoreBuilder.Identities.Fields.Add(GrammarVocabularyModelBuilder.GetUniqueEnumFieldName("StreamAccessOutOfSequence", this._compiler.SymbolStoreBuilder.Identities));
                StreamAccessOutOfSequence.SummaryText = "This is an identity that is yielded from the SymbolStream when the stream is accessed beyond one past the expected value.";
                StreamAccessOutOfSequence.RemarksText = "It's important to request a single symbol at a time, in order, as the parser progresses because ambiguities in the grammar are handled deterministically by the parsers state.  Going out of sequence could yield an incorrect parse.";
            }

            return validSymbols;
        }

        private void CreateValidToIdentityOperator(IGrammarSymbol[][] symbolChunks, IIntermediateStructType validSymbols)
        {
            var conversionMethod = validSymbols.TypeCoercions.Add(TypeConversionRequirement.Explicit, TypeConversionDirection.FromContainingType, this.IdentityEnum);
            conversionMethod.Incoming.Name = string.Format("valid{0}", this._contextualName);
            var resultLocal = conversionMethod.Locals.Add(new TypedName("result", IdentityEnum), this.NoIdentityField.GetReference());
            var slotTypeReference = ((ICliManager)this.Project.IdentityManager).ObtainTypeReference(typeof(SlotType));
            for (int symbolChunkIndex = 0; symbolChunkIndex < symbolChunks.Length; symbolChunkIndex++)
            {
                var symbolChunk = symbolChunks[symbolChunkIndex];
                var currentSlot = this.validSymbolProperties[symbolChunkIndex];
                /* if (result == {GrammarName}Symbols.None) { //P1
                 *     if (this.ValidSymbols{symbolChunkIndex} != {GrammarName}ValidSymbolSet{symbolChunkIndex}.None{UniqueNoneIndex?}) { //P2
                 *     ...
                 *     }
                 * }
                 * else if (result != {GrammarName}Symbols.Multiple{UniqueMultipleIndex?} && this.ValidSymbols{symbolChunkIndex} != {GrammarName}ValidSymbolSet{symbolChunkIndex}.None{UniqueNoneIndex?}) //P3.LogicalAnd(P2)
                 * {
                 *     result = {GrammarName}Symbols.Multiple{UniqueMultipleIndex?};
                 * }*/

                var P1 = resultLocal.GetReference().EqualTo(NoIdentityField);
                var P2 = currentSlot.GetReference(conversionMethod.Incoming.GetReference()).InequalTo(this.validSymbolsEnumNones[symbolChunkIndex]);
                var P3 = resultLocal.GetReference().InequalTo(MultipleIdentitiesIdentityField);
                var slotNecessaryCheckLevel1 = conversionMethod.If(P1);
                slotNecessaryCheckLevel1.CreateNext(P3.LogicalAnd(P2));//P3
                var slotNecessaryNextCheck = slotNecessaryCheckLevel1.Next;
                var slotNecessaryCheck = slotNecessaryCheckLevel1.If(P2);
                var slotSwitch = slotNecessaryCheck.Switch(currentSlot.GetReference(conversionMethod.Incoming.GetReference()));
                slotNecessaryNextCheck.Assign(resultLocal, MultipleIdentitiesIdentityField);
                for (int symbolIndex = 0; symbolIndex < symbolChunk.Length; symbolIndex++)
                {
                    var currentSymbol = symbolChunk[symbolIndex];
                    var currentIdentityCase = slotSwitch.Case(this.validSymbolLookup[currentSymbol].GetReference());
                    currentIdentityCase.Assign(resultLocal, this.identitySymbolLookup[currentSymbol]);
                }
                var defaultBlock = slotSwitch.Case(true);
                if (symbolChunkIndex == 0)
                    defaultBlock.Comment("It isn't one of the defined identities, so it must be multiple identities.");
                defaultBlock.Assign(resultLocal, MultipleIdentitiesIdentityField);
            }
            conversionMethod.Return(resultLocal.GetReference());
        }

        public static string GetUniqueEnumFieldName(string baseName, IIntermediateEnumType targetEnum)
        {
            string multipleIdentityName = baseName;
            int multipleIdentityOffset = 0;
            while (targetEnum.Fields.Values.Any(k => k.Name == multipleIdentityName))
                multipleIdentityName = string.Format("{0}{1}", baseName, ++multipleIdentityOffset);
            return multipleIdentityName;
        }

        private void ConstructSymbolStoreSingletons(IGrammarSymbol[][] symbolChunks, IIntermediateEnumType[] validEnums, IIntermediateEnumFieldMember[] validNones, IIntermediateClassType singletons, IIntermediateStructType validSymbols)
        {
            for (int symbolChunkIndex = 0; symbolChunkIndex < symbolChunks.Length; symbolChunkIndex++)
            {
                var currentChunk = symbolChunks[symbolChunkIndex];

                for (int symbolIndex = 0; symbolIndex < currentChunk.Length; symbolIndex++)
                {
                    var grammarSymbol = currentChunk[symbolIndex];
                    var currentEnum = validEnums[symbolChunkIndex];
                    var identityField = identitySymbolLookup[grammarSymbol];
                    var validField = validSymbolLookup[grammarSymbol];
                    var creationExpression = validSymbols.GetNewExpression();
                    for (int secondChunkIndex = 0; secondChunkIndex < symbolChunks.Length; secondChunkIndex++)
                    {
                        var currentNone = validNones[secondChunkIndex];
                        if (secondChunkIndex == symbolChunkIndex)
                            creationExpression.Parameters.Add(validField.GetReference());
                        else
                            creationExpression.Parameters.Add(validNones[secondChunkIndex].GetReference());
                    }
                    var currentSingleton = singletons.Fields.Add(new TypedName(identityField.Name, validSymbols), creationExpression);
                    currentSingleton.AccessLevel = AccessLevelModifiers.Public;
                    currentSingleton.IsStatic = true;
                    currentSingleton.ReadOnly = true;
                    singletonLookup._Add(grammarSymbol, currentSingleton);
                }
            }
        }

        public IIntermediateClassFieldMember GetSingletonReference(IGrammarSymbol symbol)
        {
            this.CheckValidSymbols();
            if (this.singletonLookup.ContainsKey(symbol))
                return this.singletonLookup[symbol];
            return null;
        }

        private void CreateValidSetBinaryOperators(IGrammarSymbol[][] symbolChunks, IIntermediateStructType validSymbols)
        {
            bool tailLanguage = this.SourceOrigin.Options.GrammarName.ToLower().EndsWith("language");
            var orBinaryOp = validSymbols.BinaryOperatorCoercions.Add(CoercibleBinaryOperators.BitwiseOr, validSymbols);
            var orCreation = validSymbols.GetNewExpression();
            orBinaryOp.SummaryText = string.Format("Performs a bitwise or against the valid symbols of the {0}{1}.", this.SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
            for (int symbolChunkIndex = 0; symbolChunkIndex < symbolChunks.Length; symbolChunkIndex++)
            {
                if (symbolChunkIndex % 2 == 0)
                    orCreation.Parameters.Add(validSymbolProperties[symbolChunkIndex].GetReference(orBinaryOp.LeftSide.GetReference().LeftNewLine()).BitwiseOr(validSymbolProperties[symbolChunkIndex].GetReference(orBinaryOp.RightSide.GetReference())));
                else
                    orCreation.Parameters.Add(validSymbolProperties[symbolChunkIndex].GetReference(orBinaryOp.LeftSide.GetReference()).BitwiseOr(validSymbolProperties[symbolChunkIndex].GetReference(orBinaryOp.RightSide.GetReference())));
            }
            orBinaryOp.Return(orCreation);
            var andBinaryOp = validSymbols.BinaryOperatorCoercions.Add(CoercibleBinaryOperators.BitwiseAnd, validSymbols);
            var andCreation = validSymbols.GetNewExpression();
            andBinaryOp.SummaryText = string.Format("Performs a bitwise and against the valid symbols of the {0}{1}.", this.SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
            for (int symbolChunkIndex = 0; symbolChunkIndex < symbolChunks.Length; symbolChunkIndex++)
            {
                if (symbolChunkIndex % 2 == 0)
                    andCreation.Parameters.Add(validSymbolProperties[symbolChunkIndex].GetReference(andBinaryOp.LeftSide.GetReference().LeftNewLine()).BitwiseAnd(validSymbolProperties[symbolChunkIndex].GetReference(andBinaryOp.RightSide.GetReference())));
                else
                    andCreation.Parameters.Add(validSymbolProperties[symbolChunkIndex].GetReference(andBinaryOp.LeftSide.GetReference()).BitwiseAnd(validSymbolProperties[symbolChunkIndex].GetReference(andBinaryOp.RightSide.GetReference())));
            }
            andBinaryOp.Return(andCreation);
            var exclusiveOrBinaryOp = validSymbols.BinaryOperatorCoercions.Add(CoercibleBinaryOperators.ExclusiveOr, validSymbols);
            exclusiveOrBinaryOp.SummaryText = string.Format("Performs an exclusive or against the valid symbols of the {0}{1}.", this.SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");

            var exclusiveOrCreation = validSymbols.GetNewExpression();
            for (int symbolChunkIndex = 0; symbolChunkIndex < symbolChunks.Length; symbolChunkIndex++)
            {
                if (symbolChunkIndex % 2 == 0)
                    exclusiveOrCreation.Parameters.Add(validSymbolProperties[symbolChunkIndex].GetReference(exclusiveOrBinaryOp.LeftSide.GetReference().LeftNewLine()).BitwiseXOr(validSymbolProperties[symbolChunkIndex].GetReference(exclusiveOrBinaryOp.RightSide.GetReference())));
                else
                    exclusiveOrCreation.Parameters.Add(validSymbolProperties[symbolChunkIndex].GetReference(exclusiveOrBinaryOp.LeftSide.GetReference()).BitwiseXOr(validSymbolProperties[symbolChunkIndex].GetReference(exclusiveOrBinaryOp.RightSide.GetReference())));
            }

            exclusiveOrBinaryOp.Return(exclusiveOrCreation);

            var equalityBinaryOp = validSymbols.BinaryOperatorCoercions.Add(CoercibleBinaryOperators.IsEqualTo, ((ICliManager)validSymbols.IdentityManager).ObtainTypeReference(RuntimeCoreType.Boolean));
            equalityBinaryOp.SummaryText = string.Format("Performs a check of equality against the valid symbols of the {0}{1}.", this.SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");

            IExpression equalityResult = null;
            for (int symbolChunkIndex = 0; symbolChunkIndex < symbolChunks.Length; symbolChunkIndex++)
            {
                if (equalityResult != null)
                    if (symbolChunkIndex % 2 == 0)
                        equalityResult = equalityResult.LogicalAnd(validSymbolProperties[symbolChunkIndex].GetReference(equalityBinaryOp.LeftSide.GetReference().LeftNewLine()).EqualTo(validSymbolProperties[symbolChunkIndex].GetReference(equalityBinaryOp.RightSide.GetReference())));
                    else
                        equalityResult = equalityResult.LogicalAnd(validSymbolProperties[symbolChunkIndex].GetReference(equalityBinaryOp.LeftSide.GetReference()).EqualTo(validSymbolProperties[symbolChunkIndex].GetReference(equalityBinaryOp.RightSide.GetReference())));
                else
                    equalityResult = validSymbolProperties[symbolChunkIndex].GetReference(equalityBinaryOp.LeftSide.GetReference()).EqualTo(validSymbolProperties[symbolChunkIndex].GetReference(equalityBinaryOp.RightSide.GetReference()));
            }

            equalityBinaryOp.Return(equalityResult);

            var inequalityBinaryOp = validSymbols.BinaryOperatorCoercions.Add(CoercibleBinaryOperators.IsNotEqualTo, ((ICliManager)validSymbols.IdentityManager).ObtainTypeReference(RuntimeCoreType.Boolean));
            IExpression inequalityResult = null;
            for (int symbolChunkIndex = 0; symbolChunkIndex < symbolChunks.Length; symbolChunkIndex++)
            {
                if (inequalityResult != null)
                    if (symbolChunkIndex % 2 == 0)
                        inequalityResult = inequalityResult.LogicalOr(validSymbolProperties[symbolChunkIndex].GetReference(inequalityBinaryOp.LeftSide.GetReference().LeftNewLine()).InequalTo(validSymbolProperties[symbolChunkIndex].GetReference(inequalityBinaryOp.RightSide.GetReference())));
                    else
                        inequalityResult = inequalityResult.LogicalOr(validSymbolProperties[symbolChunkIndex].GetReference(inequalityBinaryOp.LeftSide.GetReference()).InequalTo(validSymbolProperties[symbolChunkIndex].GetReference(inequalityBinaryOp.RightSide.GetReference())));
                else
                    inequalityResult = validSymbolProperties[symbolChunkIndex].GetReference(inequalityBinaryOp.LeftSide.GetReference()).InequalTo(validSymbolProperties[symbolChunkIndex].GetReference(inequalityBinaryOp.RightSide.GetReference()));
            }

            inequalityBinaryOp.Return(inequalityResult);


            RenameLeftRight(orBinaryOp);
            RenameLeftRight(andBinaryOp);
            RenameLeftRight(exclusiveOrBinaryOp);
            RenameLeftRight(equalityBinaryOp);
            RenameLeftRight(inequalityBinaryOp);
        }

        private void CreateIsEmptyProperty(IGrammarSymbol[][] symbolChunks, IIntermediateEnumFieldMember[] validNones, IIntermediateStructType validSymbols)
        {
            var isEmptyProp = validSymbols.Properties.Add(new TypedName("IsEmpty", RuntimeCoreType.Boolean, validSymbols.IdentityManager), true, false);
            IExpression isEmptyBooleanCheck = null;
            for (int symbolChunkIndex = 0; symbolChunkIndex < symbolChunks.Length; symbolChunkIndex++)
            {
                var currentEmptyCheck = validSymbolProperties[symbolChunkIndex].GetReference().EqualTo(validNones[symbolChunkIndex]);
                if (isEmptyBooleanCheck == null)
                    isEmptyBooleanCheck = currentEmptyCheck;
                else
                    isEmptyBooleanCheck = isEmptyBooleanCheck.LogicalAnd(currentEmptyCheck);
            }
            isEmptyProp.AccessLevel = AccessLevelModifiers.Public;

            isEmptyProp.GetMethod.Return(isEmptyBooleanCheck);
        }
        private class ReferenceGroupField<TSymbol>
        {
            public IIntermediateEnumFieldMember Field { get; set; }
            public TSymbol Symbol { get; set; }
            public override int GetHashCode() { return this.Field.GetHashCode() ^ this.Symbol.GetHashCode(); }
            public override bool Equals(object other)
            {
                if (!(other is ReferenceGroupField<TSymbol>))
                    return false;
                var fieldSymbol = (ReferenceGroupField<TSymbol>)other;
                return fieldSymbol.Field == this.Field &&
                       object.ReferenceEquals(fieldSymbol.Symbol, this.Symbol);
            }
        }
        private class ReferenceGroupKey
        {
            public IIntermediateEnumType SymbolGroup { get; set; }
            public IIntermediateStructPropertyMember Property { get; set; }
            public IIntermediateEnumFieldMember None { get; set; }
            public override int GetHashCode() { return this.SymbolGroup.GetHashCode() ^ this.Property.GetHashCode() ^ this.None.GetHashCode(); }
            public override bool Equals(object obj)
            {
                if (!(obj is ReferenceGroupKey))
                    return false;
                var other = (ReferenceGroupKey)obj;
                return other.None == this.None &&
                    other.Property == this.Property &&
                    other.SymbolGroup == this.SymbolGroup;
            }
        }
        private Dictionary<IOilexerGrammarTokenEntry, IIntermediateStructPropertyMember> ObtainHasAnySet(IIntermediateEnumType[] validEnums, IIntermediateEnumFieldMember[] validNones, IIntermediateStructType validSymbols)
        {
            var allLiteralSetTokens = (from s in this.Symbols.Where(k => this._filter(k))
                                       where s is IGrammarConstantItemSymbol
                                       let cis = (IGrammarConstantItemSymbol)s
                                       group cis by cis.Source).ToDictionary(k => k.Key, v => v.ToArray());
            Dictionary<IOilexerGrammarTokenEntry, IIntermediateStructPropertyMember> hasLiteralItemLookup = new Dictionary<IOilexerGrammarTokenEntry, IIntermediateStructPropertyMember>();
            var constSymbols = (from s in this.Symbols.Where(k => this._filter(k))
                                where s is IGrammarConstantEntrySymbol
                                select ((IGrammarConstantEntrySymbol)s)).ToArray();
            IIntermediateStructPropertyMember hasConstTokens = null;
            if (constSymbols.Length > 0)
            {
                string hasAnyConstantTokensName = "HasConstantTokens";
                int hactnIndex = 0;
                while (validSymbols.Properties.ContainsKey(TypeSystemIdentifiers.GetMemberIdentifier(hasAnyConstantTokensName)))
                    hasAnyConstantTokensName = string.Format("HasConstantTokens{0}", ++hactnIndex);
                hasConstTokens = BuildHasAny(validEnums, validNones, validSymbols, constSymbols, hasAnyConstantTokensName, "Constant Tokens");//validSymbols.Properties.Add(new TypedName(hasAnyConstantTokensName, RuntimeCoreType.Boolean, validSymbols.IdentityManager), true, false);
            }
            foreach (var tokenSet in allLiteralSetTokens.Keys)
            {
                string hasAnyName = string.Format("Has{0}", tokenSet.Name);
                var anyCurrent = BuildHasAny<IGrammarConstantItemSymbol>(validEnums, validNones, validSymbols, allLiteralSetTokens[tokenSet], hasAnyName, tokenSet.Name);
                hasLiteralItemLookup.Add(tokenSet, anyCurrent);
            }

            var variableTokenSymbols = (from s in this.Symbols.Where(k => this._filter(k))
                                        where s is IGrammarVariableSymbol
                                        select (IGrammarVariableSymbol)s).ToArray();
            IIntermediateStructPropertyMember hasVariableTokens = null;
            if (variableTokenSymbols.Length > 0)
            {
                string hasVariableTokensName = "HasVariableToken";
                int hvtnIndex = 0;
                while (validSymbols.Properties.ContainsKey(TypeSystemIdentifiers.GetMemberIdentifier(hasVariableTokensName)))
                    hasVariableTokensName = string.Format("HasVariableToken{0}", ++hvtnIndex);
                hasVariableTokens = BuildHasAny(validEnums, validNones, validSymbols, variableTokenSymbols, hasVariableTokensName, "Variable Tokens");//validSymbols.Properties.Add(new TypedName(hasAnyConstantTokensName, RuntimeCoreType.Boolean, validSymbols.IdentityManager), true, false);
            }

            var ruleSymbols = (from s in this.Symbols.Where(k => this._filter(k))
                               where s is IGrammarRuleSymbol
                               select (IGrammarRuleSymbol)s).ToArray();
            var ambiguousSymbols =
                (from s in this.Symbols.Where(k => this._filter(k))
                 where s is IGrammarAmbiguousSymbol
                 select (IGrammarAmbiguousSymbol)s).ToArray();
            IIntermediateStructPropertyMember hasRules = null;
            if (ruleSymbols.Length > 0)
            {
                string hasRuleName = "HasRule";
                int hrnIndex = 0;
                while (validSymbols.Properties.ContainsKey(TypeSystemIdentifiers.GetMemberIdentifier(hasRuleName)))
                    hasRuleName = string.Format("HasRule{0}", ++hrnIndex);
                hasRules = BuildHasAny(validEnums, validNones, validSymbols, ruleSymbols, hasRuleName, "Rules");//validSymbols.Properties.Add(new TypedName(hasAnyConstantTokensName, RuntimeCoreType.Boolean, validSymbols.IdentityManager), true, false);
            }
            IIntermediateStructPropertyMember hasAmbiguities = null;
            if (ambiguousSymbols.Length > 0)
            {
                string hasAmbiguityName = "HasAmbiguity";
                int hrnIndex = 0;
                while (validSymbols.Properties.ContainsKey(TypeSystemIdentifiers.GetMemberIdentifier(hasAmbiguityName)))
                    hasAmbiguityName = string.Format("HasAmbiguity{0}", ++hrnIndex);
                hasAmbiguities = BuildHasAny(validEnums, validNones, validSymbols, ambiguousSymbols, hasAmbiguityName, "Ambiguities");
            }
            var toStringMethod = validSymbols.Parts.Add().Methods.Add(new TypedName("ToString", RuntimeCoreType.String, validSymbols.IdentityManager));
            toStringMethod.AccessLevel = AccessLevelModifiers.Public;
            toStringMethod.IsOverride = true;
            var toStringBuilder = toStringMethod.Locals.Add(new TypedName("resultBuilder", validSymbols.IdentityManager.ObtainTypeReference(validSymbols.IdentityManager.ObtainTypeReference(RuntimeCoreType.RootType).Assembly.UniqueIdentifier.GetTypeIdentifier(typeof(StringBuilder).Namespace, typeof(StringBuilder).Name))));

            toStringBuilder.InitializationExpression = toStringBuilder.LocalType.GetNewExpression();
            var firstLocal = toStringMethod.Locals.Add(new TypedName("first", RuntimeCoreType.Boolean, validSymbols.IdentityManager), IntermediateGateway.TrueValue);
            var firstMajorLocal = toStringMethod.Locals.Add(new TypedName("firstMajor", RuntimeCoreType.Boolean, validSymbols.IdentityManager), IntermediateGateway.TrueValue);
            bool first = true;
            if (constSymbols.Length > 0)
                first = BuildSectionToString(validEnums, validNones, toStringMethod, toStringBuilder, firstLocal, firstMajorLocal, first, hasConstTokens, "Constant Tokens", symbol => symbol.Source.Name, constSymbols);
            foreach (var tokenEntry in allLiteralSetTokens.Keys)
                first = BuildSectionToString(validEnums, validNones, toStringMethod, toStringBuilder, firstLocal, firstMajorLocal, first, hasLiteralItemLookup[tokenEntry], tokenEntry.Name, symbol => string.Format("'{0}'", symbol.SourceItem.Value), allLiteralSetTokens[tokenEntry]);
            if (variableTokenSymbols.Length > 0)
                first = BuildSectionToString(validEnums, validNones, toStringMethod, toStringBuilder, firstLocal, firstMajorLocal, first, hasVariableTokens, "Variable Tokens", symbol => symbol.Source.Name, variableTokenSymbols);
            if (ruleSymbols.Length > 0)
                first = BuildSectionToString(validEnums, validNones, toStringMethod, toStringBuilder, firstLocal, firstMajorLocal, first, hasRules, "Rules", symbol => symbol.Source.Name, ruleSymbols);
            if (ambiguousSymbols.Length > 0)
                first = BuildSectionToString(validEnums, validNones, toStringMethod, toStringBuilder, firstLocal, firstMajorLocal, first, hasAmbiguities, "Ambiguities", symbol => symbol.ToString(), ambiguousSymbols);
            toStringMethod.Return(toStringBuilder.GetReference().GetMethod("ToString").Invoke());
            return hasLiteralItemLookup;
        }

        private bool BuildSectionToString<TSymbol>(IIntermediateEnumType[] validEnums, IIntermediateEnumFieldMember[] validNones, IIntermediateStructMethodMember toStringMethod, ITypedLocalMember toStringBuilder, ITypedLocalMember firstLocal, ITypedLocalMember firstMajorLocal, bool first, IIntermediateStructPropertyMember currentProp, string currentSetName, Func<TSymbol, string> elementStringer, TSymbol[] symbols)
            where TSymbol :
                IGrammarSymbol
        {
            var currentIfCondition = toStringMethod.If(currentProp.GetReference());
            var crossSection = CreateCrossSection(validEnums, validNones, symbols);
            if (first)
            {
                first = false;
                currentIfCondition.Assign(firstMajorLocal, IntermediateGateway.FalseValue);
            }
            else
            {
                var firstStatement = currentIfCondition.If(firstMajorLocal.GetReference());
                firstStatement.Assign(firstMajorLocal, IntermediateGateway.FalseValue);
                firstStatement.CreateNext();
                firstStatement.Next.Assign(firstLocal, IntermediateGateway.TrueValue);
                firstStatement.Next.Call(toStringBuilder.GetReference().GetMethod("Append").Invoke(", ".ToPrimitive()));
            }
            if (symbols.Length > 1)
            {
                currentIfCondition.Call(toStringBuilder.GetReference().GetMethod("Append").Invoke(string.Format("{0} [", currentSetName).ToPrimitive()));
                foreach (var sectionKey in crossSection.Keys)
                {
                    var sectionElements = crossSection[sectionKey];
                    if (crossSection.Count == 1)
                        GetToStringSingleSectionBody<TSymbol>(toStringBuilder, firstLocal, elementStringer, currentIfCondition, sectionKey, sectionElements);
                    else
                        GetToStringSingleSectionBody<TSymbol>(toStringBuilder, firstLocal, elementStringer,
                            currentIfCondition.If(GetSubGroupComparisonExpression(sectionKey, sectionElements, currentSetName)), sectionKey, sectionElements);
                }
                currentIfCondition.Call(toStringBuilder.GetReference().GetMethod("Append").Invoke("]".ToPrimitive()));
            }
            else
            {
                var firstItem = crossSection.Values.First().First();
                currentIfCondition.Call(toStringBuilder.GetReference().GetMethod("Append").Invoke(string.Format("{0} [{1}]", currentSetName, elementStringer(firstItem.Symbol)).ToPrimitive()));
            }
            return first;
        }

        private static void GetToStringSingleSectionBody<TSymbol>(ITypedLocalMember toStringBuilder, ITypedLocalMember firstLocal, Func<TSymbol, string> elementStringer, Ast.Statements.IConditionBlockStatement currentIfCondition, ReferenceGroupKey sectionKey, ReferenceGroupField<TSymbol>[] sectionElements) where TSymbol : IGrammarSymbol
        {
            foreach (var element in sectionElements)
            {
                var nestedIfStatement = currentIfCondition.If(sectionKey.Property.BitwiseAnd(element.Field).EqualTo(element.Field));
                if (sectionElements.Length > 1)
                {
                    var nestedFirstStatement = nestedIfStatement.If(firstLocal.GetReference());
                    nestedFirstStatement.Assign(firstLocal, IntermediateGateway.FalseValue);
                    nestedFirstStatement.CreateNext();
                    nestedFirstStatement.Next.Call(toStringBuilder.GetReference().GetMethod("Append").Invoke(", ".ToPrimitive()));
                }
                nestedIfStatement.Call(toStringBuilder.GetReference().GetMethod("Append").Invoke(elementStringer(element.Symbol).ToPrimitive()));
            }
        }

        private IIntermediateStructPropertyMember BuildHasAny<TSymbol>(IIntermediateEnumType[] validEnums, IIntermediateEnumFieldMember[] validNones, IIntermediateStructType validSymbols, TSymbol[] tokenSeries, string hasAnyName, string setName)
            where TSymbol :
                IGrammarSymbol
        {
            var anyCurrent = validSymbols.Properties.Add(new TypedName(hasAnyName, RuntimeCoreType.Boolean, validSymbols.IdentityManager), true, false);
            var crossSection = CreateCrossSection<TSymbol>(validEnums, validNones, tokenSeries);
            IExpression comparisonExpression = null;
            foreach (var sgpn in crossSection.Keys)
            {
                var fieldSymbols = crossSection[sgpn];
                IExpression subComparison = GetSubGroupComparisonExpression<TSymbol>(sgpn, fieldSymbols, setName);
                if (comparisonExpression == null)
                    comparisonExpression = subComparison;
                else
                    comparisonExpression = comparisonExpression.LogicalOr(subComparison.LeftNewLine());
            }
            anyCurrent.GetMethod.Return(comparisonExpression);
            anyCurrent.AccessLevel = AccessLevelModifiers.Public;
            anyCurrent.SummaryText = string.Format("Returns whether any elements of the {0} set are present within the @s:{1};.", setName, anyCurrent.Parent.Name);
            return anyCurrent;
        }

        private IExpression GetSubGroupComparisonExpression<TSymbol>(ReferenceGroupKey sgpn, ReferenceGroupField<TSymbol>[] fieldSymbols, string setName) 
            where TSymbol : 
                IGrammarSymbol
        {
            IExpression subComparison = null;
            /* *
             * Shortcut, if the sub-group represents the 
             * whole subset, just compare it to none.
             * *
             * Saves a step.
             * */
            if (fieldSymbols.Length == sgpn.SymbolGroup.Fields.Count - 1)//sizeof(SlotType) * 8)
                subComparison = sgpn.Property.GetReference().InequalTo(sgpn.None.GetReference().RightComment("{1} contains all of the {0} set.", SetIndexToSetName(validSymbolsEnums.GetIndexOf(sgpn.SymbolGroup)), setName));
            else
            {
                foreach (var fieldSymbol in fieldSymbols)
                {
                    var currentReference = fieldSymbol.Field.GetReference();
                    if (subComparison == null)
                        subComparison = currentReference;
                    else
                        subComparison = subComparison.BitwiseOr(currentReference.LeftNewLine());
                }
                if (fieldSymbols.Length == 1)
                    subComparison = sgpn.Property.GetReference().BitwiseAnd(subComparison).InequalTo(sgpn.None);
                else
                    subComparison = sgpn.Property.GetReference().BitwiseAnd(subComparison.LeftComment("{1} elements within the {0} set.", SetIndexToSetName(validSymbolsEnums.GetIndexOf(sgpn.SymbolGroup)), setName).LeftNewLine()).InequalTo(sgpn.None);
            }
            
            return subComparison;
        }

        private Dictionary<ReferenceGroupKey, ReferenceGroupField<TSymbol>[]> CreateCrossSection<TSymbol>(IIntermediateEnumType[] validEnums, IIntermediateEnumFieldMember[] validNones, TSymbol[] tokenSeries) where TSymbol : IGrammarSymbol
        {
            var crossSection = (from symbol in tokenSeries
                                let enumField = validSymbolLookup[symbol]
                                let index = validEnums.GetIndexOf(enumField.Parent)
                                let property = validSymbolProperties[index]
                                let currentNone = validNones[index]
                                group new ReferenceGroupField<TSymbol>() { Field = enumField, Symbol = symbol } by new ReferenceGroupKey() { SymbolGroup = enumField.Parent, Property = property, None = currentNone }).ToDictionary(k => k.Key, v => v.ToArray());
            return crossSection;
        }

        private static void RenameLeftRight(IIntermediateBinaryOperatorCoercionMember<IStructType, IIntermediateStructType> op)
        {
            op.Locals[TypeSystemIdentifiers.GetMemberIdentifier("__leftSide")].Name = "left";
            op.Locals[TypeSystemIdentifiers.GetMemberIdentifier("__rightSide")].Name = "right";

        }

        private static string SetIndexToSetName(int index)
        {
            switch (index + 1)
            {
                case 1:
                    return "first";
                case 2:
                    return "second";
                case 3:
                    return "third";
                case 4:
                    return "fourth";
                case 5:
                    return "fifth";
                case 6:
                    return "sixth";
                case 7:
                    return "seventh";
                case 8:
                    return "eighth";
                case 9:
                    return "ninth";
                case 10:
                    return "tenth";
                default:
                    var name = (index + 1).ToString();
                    var last = name[name.Length - 1];
                    if (name[name.Length - 2] == '1')
                        return name + "th";
                    switch (last)
                    {
                        case '1':
                            return name + "st";
                        case '2':
                            return name + "nd";
                        case '3':
                            return name + "rd";
                        default:
                            return name + "th";
                    }
            }
        }

        private void GenerateChunkSymbols(IGrammarSymbol[] currentChunk, IIntermediateEnumType currentEnum)
        {
            for (int symbolIndex = 0; symbolIndex < currentChunk.Length; symbolIndex++)
            {
                var currentSymbol = currentChunk[symbolIndex];
                IIntermediateEnumFieldMember validSymbolEnumMember;
                IIntermediateEnumFieldMember identitySymbolEnumMember;
                GenerateSymbolFields(currentEnum, symbolIndex, currentSymbol, out validSymbolEnumMember, out identitySymbolEnumMember);
                validSymbolLookup._Add(currentSymbol, validSymbolEnumMember);
                identitySymbolLookup._Add(currentSymbol, identitySymbolEnumMember);
            }
        }

        private static IIntermediateStructPropertyMember GenerateSymbolSetChunkProperty(string zeroFormat, IIntermediateStructType validSymbols, int symbolChunkIndex, IIntermediateEnumFieldMember currentNone)
        {
#if x64
            currentNone.Parent.ValueType= EnumerationBaseType.UInt64;
#elif x86
            currentNone.Parent.ValueType = EnumerationBaseType.UInt32;
#endif
            var ctor = validSymbols.Constructors.Values.FirstOrDefault();
            IIntermediateConstructorParameterMember iicpm;
            if (ctor == null)
            {
                ctor = validSymbols.Constructors.Add(new TypedName(string.Format("validSymbols" + zeroFormat, symbolChunkIndex + 1), currentNone.Parent, validSymbols.IdentityManager));
                ctor.AccessLevel = AccessLevelModifiers.Internal;
                iicpm = ctor.Parameters.Values.First();
            }
            else
            {
                var fParam = ctor.Parameters.Add(new TypedName(string.Format("validSymbols" + zeroFormat, symbolChunkIndex + 1), currentNone.Parent, validSymbols.IdentityManager));
                iicpm = fParam;
            }

            IIntermediateStructPropertyMember currentValidSetProperty = validSymbols.Properties.Add(new TypedName(string.Format("ValidSymbols" + zeroFormat, symbolChunkIndex + 1), currentNone.Parent, validSymbols.IdentityManager), true, false);
            currentValidSetProperty.AccessLevel = AccessLevelModifiers.Public;
            IIntermediateStructFieldMember currentValidSetField = validSymbols.Fields.Add(new TypedName(string.Format("validSymbols" + zeroFormat, symbolChunkIndex + 1), currentNone.Parent, validSymbols.IdentityManager));
            currentValidSetProperty.GetMethod.Return(currentValidSetField.GetReference());
            currentValidSetField.AccessLevel = AccessLevelModifiers.Private;
            ctor.Assign(currentValidSetField, iicpm.GetReference());
            return currentValidSetProperty;
        }

        private void GenerateSymbolFields(IIntermediateEnumType currentEnum, int symbolIndex, IGrammarSymbol currentSymbol, out IIntermediateEnumFieldMember validSymbolEnumMember, out IIntermediateEnumFieldMember identitySymbolEnumMember)
        {
            var ruleSymbol = currentSymbol as IGrammarRuleSymbol;
            var tokenSymbol = currentSymbol as IGrammarVariableSymbol;
            var constSymbol = currentSymbol as IGrammarConstantEntrySymbol;
            var literalSymbol = currentSymbol as IGrammarConstantItemSymbol;
            //var lexicalAmbiguitySymbol = currentSymbol as IGrammarLexicalAmbiguitySymbol;
            var ambiguousSymbol = currentSymbol as IGrammarAmbiguousSymbol;
            var namePattern = string.Empty;
            validSymbolEnumMember = null;
            identitySymbolEnumMember = null;
            bool tailLanguage = this.SourceOrigin.Options.GrammarName.ToLower().EndsWith("language");
            /* *
             * There are two enum values generated for each symbol:
             * one which represents the symbol in a 'valid' context,
             * which might be a member of multiple sets.
             * *
             * The other represents the identity of that symbol,
             * for use within a symbol matcher.
             * */
            if (ruleSymbol != null)
            {
                validSymbolEnumMember = currentEnum.Fields.Add(string.Format("{0}{1}{2}", SourceOrigin.Options.RulePrefix, ruleSymbol.ElementName, SourceOrigin.Options.RuleSuffix), (SlotType)Math.Pow(2, symbolIndex));
                identitySymbolEnumMember = this._compiler.SymbolStoreBuilder.Identities.Fields.Add(string.Format("{0}{1}{2}", SourceOrigin.Options.RulePrefix, ruleSymbol.ElementName, SourceOrigin.Options.RuleSuffix));
                identitySymbolEnumMember.SummaryText = string.Format("The symbol represents the @s:I{0}; rule from the {1}{2}.", identitySymbolEnumMember.Name, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
                identitySymbolEnumMember.RemarksText = string.Format("'.{0}' at: line {1}, column {2}", ruleSymbol.Source.FileName.Substring(SourceOrigin.RelativeRoot.Length), ruleSymbol.Source.Line, ruleSymbol.Source.Column);
                validSymbolEnumMember.SummaryText = string.Format("The set of valid symbols includes the @s:I{0}; rule from the {1}{2}.", identitySymbolEnumMember.Name, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
                validSymbolEnumMember.RemarksText = string.Format("@para;'.{0}' at: line {1}, column {2}@/para;", ruleSymbol.Source.FileName.Substring(SourceOrigin.RelativeRoot.Length), ruleSymbol.Source.Line, ruleSymbol.Source.Column);
            }
            else if (tokenSymbol != null)
            {
                validSymbolEnumMember = currentEnum.Fields.Add(string.Format("{0}{1}{2}", SourceOrigin.Options.TokenPrefix, tokenSymbol.ElementName, SourceOrigin.Options.TokenSuffix), (SlotType)Math.Pow(2, symbolIndex));
                identitySymbolEnumMember = this._compiler.SymbolStoreBuilder.Identities.Fields.Add(string.Format("{0}{1}{2}", SourceOrigin.Options.TokenPrefix, tokenSymbol.ElementName, SourceOrigin.Options.TokenSuffix));
                identitySymbolEnumMember.SummaryText = string.Format("The symbol represents the {0} token from the {1}{2}.", identitySymbolEnumMember.Name, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
                identitySymbolEnumMember.RemarksText = string.Format("'.{0}' at: line {1}, column {2}", tokenSymbol.Source.FileName.Substring(SourceOrigin.RelativeRoot.Length), tokenSymbol.Source.Line, tokenSymbol.Source.Column);
                validSymbolEnumMember.SummaryText = string.Format("The set of valid symbols includes the {0} token from the {1}{2}.", identitySymbolEnumMember.Name, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
                validSymbolEnumMember.RemarksText = string.Format("'.{0}' at: line {1}, column {2}", tokenSymbol.Source.FileName.Substring(SourceOrigin.RelativeRoot.Length), tokenSymbol.Source.Line, tokenSymbol.Source.Column);
            }
            else if (constSymbol != null)
            {
                validSymbolEnumMember = currentEnum.Fields.Add(string.Format("{0}{1}{2}", SourceOrigin.Options.TokenPrefix, constSymbol.ElementName, SourceOrigin.Options.TokenSuffix), (SlotType)Math.Pow(2, symbolIndex));
                identitySymbolEnumMember = this._compiler.SymbolStoreBuilder.Identities.Fields.Add(string.Format("{0}{1}{2}", SourceOrigin.Options.TokenPrefix, constSymbol.ElementName, SourceOrigin.Options.TokenSuffix));
                identitySymbolEnumMember.SummaryText = string.Format("The symbol represents the {0} token from the {1}{2}.", identitySymbolEnumMember.Name, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
                validSymbolEnumMember.SummaryText = string.Format("The set of valid symbols includes the {0} token from the {1}{2}.", identitySymbolEnumMember.Name, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
                if (!(constSymbol.Source is IOilexerGrammarTokenEofEntry))
                {
                    validSymbolEnumMember.RemarksText = string.Format("'.{0}' at: line {1}, column {2}.", constSymbol.Source.FileName.Substring(SourceOrigin.RelativeRoot.Length), constSymbol.Source.Line, constSymbol.Source.Column);
                    identitySymbolEnumMember.RemarksText = string.Format("'.{0}' at: line {1}, column {2}.", constSymbol.Source.FileName.Substring(SourceOrigin.RelativeRoot.Length), constSymbol.Source.Line, constSymbol.Source.Column);
                }
                else
                    identitySymbolEnumMember.RemarksText = string.Format("Automatically generated End Of File token.");
            }
            else if (literalSymbol != null)
            {
                validSymbolEnumMember = currentEnum.Fields.Add(string.Format("{0}{1}{2}_{3}", SourceOrigin.Options.TokenPrefix, literalSymbol.Source.Name, SourceOrigin.Options.TokenSuffix, literalSymbol.SourceItem.Name), (SlotType)Math.Pow(2, symbolIndex));
                identitySymbolEnumMember = this._compiler.SymbolStoreBuilder.Identities.Fields.Add(string.Format("{0}{1}{2}_{3}", SourceOrigin.Options.TokenPrefix, literalSymbol.Source.Name, SourceOrigin.Options.TokenSuffix, literalSymbol.SourceItem.Name));
                identitySymbolEnumMember.SummaryText = string.Format("The symbol represents the '{0}' {1} token from the {2}{3}.", literalSymbol.SourceItem.Name, literalSymbol.Source.Name, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
                identitySymbolEnumMember.RemarksText = string.Format("@para;'.{0}' at: line {1}, column {2}.@/para;@para;Original Definition:\r\n\t{3}@/para;", literalSymbol.Source.FileName.Substring(SourceOrigin.RelativeRoot.Length), literalSymbol.SourceItem.Line, literalSymbol.SourceItem.Column, literalSymbol.SourceItem);
                validSymbolEnumMember.SummaryText = string.Format("The set of valid symbols includes the '{0}' {1} token from the {2}{3}.", literalSymbol.SourceItem.Name, literalSymbol.Source.Name, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
                validSymbolEnumMember.RemarksText = string.Format("@para;'.{0}' at: line {1}, column {2}.@/para;@para;Original Definition:\r\n\t{3}@/para;", literalSymbol.Source.FileName.Substring(SourceOrigin.RelativeRoot.Length), literalSymbol.SourceItem.Line, literalSymbol.SourceItem.Column, literalSymbol.SourceItem);
            }
            else if (ambiguousSymbol != null)
            {
                validSymbolEnumMember = currentEnum.Fields.Add(ambiguousSymbol.ElementName, (SlotType)Math.Pow(2, symbolIndex));
                identitySymbolEnumMember = this._compiler.SymbolStoreBuilder.Identities.Fields.Add(ambiguousSymbol.ElementName);
                identitySymbolEnumMember.SummaryText = string.Format("The symbol represents the an ambiguity between the following tokens: '{0}' from the {1}{2}.", ambiguousSymbol.AmbiguityKey, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
                validSymbolEnumMember.SummaryText = string.Format("The set of valid symbols includes an ambiguity of the following tokens: {0} from the {1}{2}.", ambiguousSymbol.AmbiguityKey, SourceOrigin.Options.GrammarName, tailLanguage ? string.Empty : " language");
            }
        }

        /// <summary>
        /// Returns the <see cref="IGrammarSymbolSet"/> 
        /// used by the <see cref="GrammarVocabularyModelBuilder"/>.
        /// </summary>
        public IGrammarSymbolSet Symbols { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateAssembly"/> which
        /// is targeted during generation.
        /// </summary>
        public IIntermediateAssembly Project { get; private set; }

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarFIle"/> which represents the 
        /// results of the lexical analysis, parse, linking, and 
        /// reduction of the source grammar files.
        /// </summary>
        public IOilexerGrammarFile SourceOrigin { get { return this.sourceOrigin; } }

        public IIntermediateEnumType IdentityEnum { get { return this._compiler.SymbolStoreBuilder.Identities; } }
        public bool Created { get { return this.validSymbols != null; } }

        public IIntermediateClassPropertyMember GenerateSymbolstoreVariation(GrammarVocabulary vocabulary, string setName = "StateSyntaxCheck", string summary = "", string remarks = "")
        {
            this.CheckValidSymbols();
            IIntermediateClassPropertyMember result;
            if (!this.symbolStoreCache.TryGetValue(setName, vocabulary, out result))
            {
                var vocabularySymbols = vocabulary.GetSymbols().Where(k => k != null);
                var parameterFieldValues =
                    (from chunkIndex in 0.RangeTo(this.symbolChunks.Length)
                     from definedSymbol in this.symbolChunks[chunkIndex]
                     join grammarSymbol in vocabularySymbols on definedSymbol equals grammarSymbol into chunkRelevantItems
                     from chunkRelevantItem in chunkRelevantItems.DefaultIfEmpty()
                     group chunkRelevantItems.ToArray() by chunkIndex into chunkSection1
                     from chunkRelevantItem in chunkSection1.ToArray().ConcatinateSeries().DefaultIfEmpty()
                     let chunkIndex = chunkSection1.Key
                     let chunkReference = chunkRelevantItem == null ? validSymbolsEnumNones[chunkIndex] : validSymbolLookup[chunkRelevantItem]
                     group chunkReference by chunkIndex).Select(chunkSet => chunkSet.Distinct().Aggregate((IExpression)null, (runningResult, currentItem) =>
                         {
                             var currentReference = currentItem.GetReference();
                             if (runningResult == null)
                                 runningResult = currentReference;
                             else
                                 runningResult = runningResult.BitwiseOr(currentReference);
                             return runningResult;
                         })).ToArray();
                int setCount;
                if (!this.SymbolStoreCount.TryGetValue(setName, out setCount))
                    this.SymbolStoreCount.Add(setName, setCount = 0);

                this.symbolStoreCache.Add(setName, vocabulary, result = this.SymbolStore.Properties.Add(new TypedName(string.Format("{0}{1}", setName, SymbolStoreCount[setName] = ++setCount), this.ValidSymbols), true, false));
                IIntermediateClassFieldMember resultField;
                this.symbolStoreCacheFields.Add(setName, vocabulary, resultField = this.SymbolStore.Fields.Add(new TypedName("_{0}", ((IStructType)typeof(Nullable<>).GetTypeReference((IIntermediateCliManager)this.Project.IdentityManager)).MakeGenericClosure(result.PropertyType), result.Name.LowerFirstCharacter())));
                var ifCheck = result.GetMethod.If(resultField.GetReference().EqualTo(IntermediateGateway.NullValue));
                ifCheck.Assign(resultField, result.PropertyType.GetNewExpression(parameterFieldValues));
                result.GetMethod.Return(resultField.GetReference().GetProperty("Value"));
                //symbolStoreCacheVariations.Add(setName, vocabulary, this.symbolStoreVariations.Fields.Add(result.Name));
                if (!string.IsNullOrEmpty(remarks))
                    result.RemarksText = remarks;
                if (!string.IsNullOrEmpty(summary))
                    result.SummaryText = summary;

                resultField.SummaryText = string.Format("Data member for @s:{0};", result.Name);
                result.AccessLevel = AccessLevelModifiers.Public;
                resultField.AccessLevel = AccessLevelModifiers.Private;
            }
            return result;
        }
        public IIntermediateClassType SymbolStore { get { return this._compiler.SymbolStoreBuilder.SymbolStore; } }

        public IIntermediateEnumFieldMember MultipleIdentitiesIdentityField { get; set; }

        public IIntermediateEnumFieldMember NoIdentityField { get; set; }


        public IIntermediateEnumFieldMember StreamAccessOutOfSequence { get; set; }
    }
}
