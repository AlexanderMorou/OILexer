using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using Resources = AllenCopeland.Abstraction.Utilities.Properties.Resources;
using System.IO.Compression;
using AllenCopeland.Abstraction.Slf.Cli;
using System.IO;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using System.Diagnostics;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class ExtensionsBuilder
    {
        private IIntermediateClassMethodMember _getValidSyntaxMethodInternalImpl;
        private IIntermediateInterfaceMethodMember _getValidSyntaxMethod;
        private IIntermediateClassMethodMember _getValidSyntaxMethodImpl;
        private IIntermediateClassMethodMember _cullAmbiguities;
        private IIntermediateClassPropertyMember _AllAmbiguitiesField;

        public void Build(ParserCompiler compiler, ParserBuilder parserBuilder, IIntermediateCliManager identityManager)
        {
            /* *
             * After much of the static analysis has been completed on the grammar, it's now time to generate the parser's full state transition detail.
             * */
            BuildGetValidSyntaxCore(compiler, parserBuilder, identityManager);
        }

        public void Build2(ParserCompiler compiler, ParserBuilder parserBuilder, IIntermediateCliManager identityManager)
        {
            BuildCullAmbiguitiesCore(compiler, parserBuilder, identityManager);
            BuildGetValidSyntax(compiler, parserBuilder, identityManager);
            BuildDebuggerDisplayForSymbols(compiler, parserBuilder, identityManager);
            BuildVisitorModel(compiler, identityManager);
            BuildCullAmbiguities(compiler, parserBuilder, identityManager);
        }

        private void BuildCullAmbiguities(ParserCompiler compiler, ParserBuilder parserBuilder, IIntermediateCliManager identityManager)
        {
            bool lexicallyAmbiguousModel = compiler._GrammarSymbols.AmbiguousSymbols.Count() > 0;
            if (!lexicallyAmbiguousModel)
                return;
            var runningAmbiguities = this._cullAmbiguities.Locals.Add(
                new TypedName("runningAmbiguities", compiler.LexicalSymbolModel.ValidSymbols), new DefaultValueExpression(compiler.LexicalSymbolModel.ValidSymbols));
            var resultAmbiguities = this._cullAmbiguities.Locals.Add(
                new TypedName("resultAmbiguities", compiler.LexicalSymbolModel.ValidSymbols), new DefaultValueExpression(compiler.LexicalSymbolModel.ValidSymbols));
            var unambiguousSource = this._cullAmbiguities.Parameters["unambiguousSource"];
            this._cullAmbiguities.Comment("Remove the ambiguities that are currently present, a larger multiple-symbol ambiguity may trump what was there due to the follow-union.");
            this._cullAmbiguities.Assign((IMemberReferenceExpression)unambiguousSource.GetReference(), AssignmentOperation.BitwiseExclusiveOrAssign, unambiguousSource.GetReference().BitwiseAnd(_AllAmbiguitiesField.GetReference()));
            bool first = true;
            foreach (var ambiguity in compiler._GrammarSymbols.AmbiguousSymbols)
            {
                var ambiguityDetail = compiler.AmbiguityDetail[ambiguity];
                IBlockStatementParent target = this._cullAmbiguities;
                if (first)
                    first = false;
                else
                    target = target.If(runningAmbiguities.GetReference().BitwiseAnd(ambiguityDetail.AmbiguityKeyReference.GetReference()).InequalTo(ambiguityDetail.AmbiguityKeyReference.GetReference()));
                var assignmentCheck = target.If(unambiguousSource.BitwiseAnd(ambiguityDetail.AmbiguityKeyReference).EqualTo(ambiguityDetail.AmbiguityKeyReference));
                assignmentCheck.Assign(runningAmbiguities.GetReference(), AssignmentOperation.BitwiseOrAssign, ambiguityDetail.AmbiguityKeyReference.GetReference());
                assignmentCheck.Assign(resultAmbiguities.GetReference(), AssignmentOperation.BitwiseOrAssign, ambiguityDetail.AmbiguityReference.GetReference());
            }
            _cullAmbiguities.Return(resultAmbiguities.GetReference());
        }

        private void BuildCullAmbiguitiesCore(ParserCompiler compiler, ParserBuilder parserBuilder, IIntermediateCliManager identityManager)
        {
            bool lexicallyAmbiguousModel = compiler._GrammarSymbols.AmbiguousSymbols.Count() > 0;
            if (!lexicallyAmbiguousModel)
                return;
            this._cullAmbiguities = this._getValidSyntaxMethodInternalImpl
                .Parent
                .Methods
                .Add(
                    new TypedName("CullAmbiguitiesFrom", compiler.LexicalSymbolModel.ValidSymbols),
                    new TypedNameSeries(
                        new TypedName("unambiguousSource", compiler.LexicalSymbolModel.ValidSymbols)));
            var allAmbiguities = compiler.LexicalSymbolModel.GenerateSymbolstoreVariation(new GrammarVocabulary(compiler.GrammarSymbols, compiler._GrammarSymbols.AmbiguousSymbols.ToArray()), "AllAmbiguities", string.Format("Returns a @s:{0}; value that represents all ambiguous identities.", compiler.LexicalSymbolModel.ValidSymbols.Name));
            allAmbiguities.Name = "AllAmbiguities";
            this._AllAmbiguitiesField = allAmbiguities;
            _cullAmbiguities.AccessLevel = AccessLevelModifiers.Public;
        }

        private void BuildVisitorModel(ParserCompiler compiler, IIntermediateCliManager identityManager)
        {
            var returnType = identityManager.ObtainTypeReference(RuntimeCoreType.VoidType);
            var visitorInterface = compiler.RuleSymbolBuilder.ILanguageRuleSymbol.Assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}Visitor", compiler.Source.Options.AssemblyName);
            var visitorResultInterface = compiler.RuleSymbolBuilder.ILanguageRuleSymbol.Assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}ReturnVisitor", compiler.Source.Options.AssemblyName);
            var tParamResult = visitorResultInterface.TypeParameters.Add("TResult");
            BuildVisitor(compiler, returnType, visitorInterface, identityManager);
            BuildVisitor(compiler, tParamResult, visitorResultInterface, identityManager);
        }

        private static void BuildVisitor(ParserCompiler compiler, IType returnType, IIntermediateInterfaceType visitorInterface, IIntermediateCliManager identityManager)
        {
            var voidType = identityManager.ObtainTypeReference(RuntimeCoreType.VoidType);
            string rtType = returnType.IsGenericTypeParameter ? "@t:" : "@s:";
            var methods = new Dictionary<IOilexerGrammarProductionRuleEntry,IIntermediateInterfaceMethodMember>();
            foreach (var rule in compiler.Source.GetRules()/*.Where(r => !r.IsRuleCollapsePoint)*/)
            {
                string ruleNameParameter = rule.Name.LowerFirstCharacter();
                var targetInterface = compiler.RuleDetail[rule].RelativeInterface;
                object target = rule;
                if (rule.IsRuleCollapsePoint)
                    continue;
                AddVisitMethodToVisitor(returnType, visitorInterface, voidType, rtType, methods, ruleNameParameter, targetInterface, target);
            }

            var rootVisit = AddVisitMethodToClass(returnType, voidType, visitorInterface, compiler.RootRuleBuilder.LanguageRuleRoot, true, AddVisitMethodToInterface(returnType, visitorInterface, compiler.RootRuleBuilder.ILanguageRule));

            AddVisitMethodToInterface(returnType, visitorInterface, compiler.TokenSymbolBuilder.ILanguageToken);
            IIntermediateInterfaceMethodMember tokenSymbolVisitMethod;
            AddVisitMethodToClass(returnType, voidType, visitorInterface, compiler.VariableTokenBaseBuilder.LanguageVariableToken, false, AddVisitMethodToVisitor(returnType, visitorInterface, voidType, rtType, methods, "token", compiler.VariableTokenBaseBuilder.ILanguageVariableToken, null));
            AddVisitMethodToClass(returnType, voidType, visitorInterface, compiler.FixedTokenBaseBuilder.LanguageFixedToken, false, tokenSymbolVisitMethod = AddVisitMethodToVisitor(returnType, visitorInterface, voidType, rtType, methods, "token", compiler.TokenSymbolBuilder.ILanguageToken, null));

            foreach (var rule in compiler.Source.GetRules().Where(r => !r.IsRuleCollapsePoint))
                BuildMethodOn(returnType, visitorInterface, voidType, methods[rule], compiler.RuleDetail[rule].Class);

            visitorInterface.AccessLevel = AccessLevelModifiers.Public;
            var tokenDerived = compiler.TokenCastAsRule;
            if (tokenDerived != null)
            {
                var orm = compiler.RelationalModelMapping.ImplementationDetails[tokenDerived];
                var tokenDerivedVisit = BuildMethodOn(returnType, visitorInterface, voidType, null, orm.Value.Class, true);
                var call = rootVisit.GetReference(compiler.RootRuleBuilder.TokenDerived_Token.GetReference()).Invoke(tokenDerivedVisit.Parameters["visitor"].GetReference());
                if (voidType != returnType)
                    tokenDerivedVisit.Return(call);
                else
                    tokenDerivedVisit.Call(call);

            }
            //BuildMethodOn(returnType, visitorInterface, voidType, method, compiler.RelationalModelMapping[compiler.TokenCastAsRule].ImplementationDetails.Value.Class);
        }

        private static IIntermediateClassMethodMember AddVisitMethodToClass(IType returnType, IType voidType, IIntermediateInterfaceType visitorInterface, IIntermediateClassType visitedClass, bool isAbstract, IIntermediateInterfaceMethodMember memberToVisit)
        {
            var rootVisit = visitedClass.Methods.Add(
                new TypedName("Accept", returnType),
                    new TypedNameSeries(new TypedName("visitor", visitorInterface)));
            if (returnType.IsGenericTypeParameter)
                rootVisit.TypeParameters.Add(returnType.Name);
            rootVisit.SummaryText = "Invokes the appropriate overload for the current @p:visitor;.";
            rootVisit.AccessLevel = AccessLevelModifiers.Public;
            var invocation = isAbstract ? null : memberToVisit.GetReference(rootVisit.Parameters["visitor"].GetReference()).Invoke(new SpecialReferenceExpression(SpecialReferenceKind.This));
            if (isAbstract)
                rootVisit.IsAbstract = true;
            else if (returnType != voidType)
                rootVisit.Return(invocation);
            else
                rootVisit.Call(invocation);
            return rootVisit;
        }

        private static IIntermediateInterfaceMethodMember AddVisitMethodToInterface(IType returnType, IIntermediateInterfaceType visitorInterface, IIntermediateInterfaceType interfaceTarget)
        {

            var resultVisitMethod = interfaceTarget.Methods.Add(
                new TypedName("Accept", returnType),
                    new TypedNameSeries(new TypedName("visitor", visitorInterface)));
            if (returnType.IsGenericTypeParameter)
                resultVisitMethod.TypeParameters.Add(returnType.Name);
            return resultVisitMethod;
        }

        private static IIntermediateInterfaceMethodMember AddVisitMethodToVisitor(IType returnType, IIntermediateInterfaceType visitorInterface, IType voidType, string rtType, Dictionary<IOilexerGrammarProductionRuleEntry, IIntermediateInterfaceMethodMember> methods, string ruleNameParameter, IIntermediateInterfaceType targetInterface, object target)
        {
            var visitMethod = visitorInterface.Methods.Add(
                new TypedName("Visit", returnType),
                new TypedNameSeries(new TypedName(ruleNameParameter, targetInterface)));
            visitMethod.SummaryText = string.Format("Visits the @p:{0}; provided.", ruleNameParameter);
            if (target is IOilexerGrammarProductionRuleEntry)
                methods.Add(((IOilexerGrammarProductionRuleEntry)target), visitMethod);
            var ruleParam = visitMethod.Parameters[ruleNameParameter];
            ruleParam.SummaryText = string.Format("The @s:{0}; to visit.", ruleParam.ParameterType);
            if (returnType != voidType)
                visitMethod.ReturnsText = string.Format("A {0}{1}; which represents the result of the visit.", rtType, returnType.Name);
            return visitMethod;
        }

        private static IIntermediateClassMethodMember BuildMethodOn(IType returnType, IIntermediateInterfaceType visitorInterface, IType voidType, IIntermediateInterfaceMethodMember method, IIntermediateClassType ruleClass, bool skipBody = false)
        {
            var ruleVisit = ruleClass.Methods.Add(
                new TypedName("Accept", returnType),
                new TypedNameSeries(new TypedName("visitor", visitorInterface)));
            if (returnType.IsGenericTypeParameter)
                ruleVisit.TypeParameters.Add(returnType.Name);
            ruleVisit.AccessLevel = AccessLevelModifiers.Public;
            ruleVisit.IsOverride = true;
            if (!skipBody)
                if (returnType != voidType)
                    ruleVisit.Return(method.GetReference(ruleVisit.Parameters["visitor"].GetReference()).Invoke(new SpecialReferenceExpression(SpecialReferenceKind.This)));
                else
                    ruleVisit.Call(method.GetReference(ruleVisit.Parameters["visitor"].GetReference()).Invoke(new SpecialReferenceExpression(SpecialReferenceKind.This)));
            return ruleVisit;
        }

        private void BuildDebuggerDisplayForSymbols(ParserCompiler compiler, ParserBuilder parserBuilder, IIntermediateCliManager identityManager)
        {
            var extensionsClass = compiler.RuleSymbolBuilder.ILanguageRuleSymbol.Assembly.DefaultNamespace.Parts.Add().Classes.Add("{0}Extensions", compiler.Source.Options.AssemblyName);
            extensionsClass.AccessLevel = AccessLevelModifiers.Internal;
            extensionsClass.SpecialModifier = SpecialClassModifier.Static;
            this.ExtensionsClass = extensionsClass;
            var symbolDebuggerDisplay = extensionsClass.Methods.Add(
                new TypedName("SymbolDebuggerDisplay", RuntimeCoreType.String, identityManager),
                new TypedNameSeries(
                    new TypedName("symbol", compiler.SymbolStoreBuilder.Identities)));
            var symbol = symbolDebuggerDisplay.Parameters["symbol"];

            var constantEntries = (from s in compiler._GrammarSymbols.SymbolsOfType<IGrammarConstantEntrySymbol>()
                                   where !(s.Source is IOilexerGrammarTokenEofEntry)
                                   let literalItem = s.Source.Branches[0][0] as ILiteralTokenItem
                                   where literalItem != null
                                   orderby literalItem.Line, literalItem.Column, literalItem.Value.ToString()
                                   select new { LiteralItem = literalItem, Symbol = s }).ToArray();
            var constantItems =
                (from s in compiler._GrammarSymbols.SymbolsOfType<IGrammarConstantItemSymbol>()
                 let literalItem = s.SourceItem
                 orderby literalItem.Line, literalItem.Column, literalItem.Value.ToString()
                 select new { LiteralItem = literalItem, Symbol = s }).ToArray();

            var ambiguousItems =
                (from s in compiler._GrammarSymbols.SymbolsOfType<IGrammarAmbiguousSymbol>()
                 let literalConstantEntry = (IGrammarConstantEntrySymbol)s.FirstOrDefault(aSym => aSym is IGrammarConstantEntrySymbol && constantEntries.Any(ce => ce.Symbol == aSym))
                 let literalConstantItem = (IGrammarConstantItemSymbol)s.FirstOrDefault(aSym => aSym is IGrammarConstantItemSymbol && constantItems.Any(ci => ci.Symbol == aSym))
                 where literalConstantEntry != null || literalConstantItem != null
                 let literalItem = literalConstantEntry != null ? literalConstantEntry.Source.Branches[0][0] as ILiteralTokenItem : literalConstantItem.SourceItem
                 orderby literalItem.Line, literalItem.Column, literalItem.Value.ToString()
                 select new { LiteralItem = literalItem, Symbol = s }).ToArray();

            var mainSwitch = symbolDebuggerDisplay.Switch(symbol.GetReference());

            //compiler.TokenSymbolDetail
            foreach (var constantEntry in constantEntries)
            {
                var symbolDet = compiler.TokenSymbolDetail[constantEntry.Symbol];
                var currentCase = mainSwitch.Case(symbolDet.Identity.GetReference());
                var fN=symbolDet.Symbol.Source.FileName;
                if (fN.ToLower().StartsWith(compiler.Source.RelativeRoot))
                    currentCase.Comment(string.Format("In .{0} on line {1}, column {2}", fN.Substring(compiler.Source.RelativeRoot.Length), constantEntry.LiteralItem.Line, constantEntry.LiteralItem.Column));
                else
                    currentCase.Comment(string.Format("In {0} on line {1}, column {2}", fN, constantEntry.LiteralItem.Line, constantEntry.LiteralItem.Column));
                currentCase.Return(constantEntry.LiteralItem.Value.ToString().ToPrimitive());
            }
            foreach (var constantItem in constantItems)
            {
                var symbolDet = compiler.TokenSymbolDetail[constantItem.Symbol];
                var currentCase = mainSwitch.Case(symbolDet.Identity.GetReference());
                var fN = symbolDet.Symbol.Source.FileName;
                if (fN.ToLower().StartsWith(compiler.Source.RelativeRoot))
                    currentCase.Comment(string.Format("In .{0} on line {1}, column {2}", fN.Substring(compiler.Source.RelativeRoot.Length), constantItem.LiteralItem.Line, constantItem.LiteralItem.Column));
                else
                    currentCase.Comment(string.Format("In {0} on line {1}, column {2}", fN, constantItem.LiteralItem.Line, constantItem.LiteralItem.Column));
                currentCase.Return(constantItem.LiteralItem.Value.ToString().ToPrimitive());
            }

            foreach (var ambiguousItem in ambiguousItems)
            {
                var identityField = compiler.LexicalSymbolModel.GetIdentitySymbolField(ambiguousItem.Symbol);
                var currentCase = mainSwitch.Case(identityField.GetReference());
                StringBuilder ambiguityBuilder = new StringBuilder();
                ambiguityBuilder.Append("Ambiguity {");
                bool first=true;
                foreach (var ambigSymbol in ambiguousItem.Symbol)
                {
                    if (first)
                        first=false;
                    else
                        ambiguityBuilder.Append(", ");
                    ambiguityBuilder.Append(ambigSymbol.ElementName);
                }
                ambiguityBuilder.Append("} :");
                ambiguityBuilder.Append(ambiguousItem.LiteralItem.Value.ToString().ToPrimitive());
                currentCase.Return(ambiguityBuilder.ToString().ToPrimitive());
            }

            mainSwitch.Case(true).Return(symbol.GetReference().GetMethod("ToString").Invoke());

            this.SymbolDebuggerDisplay = symbolDebuggerDisplay;
            symbolDebuggerDisplay.AccessLevel = AccessLevelModifiers.Public;
            compiler.SymbolStoreBuilder.Identities.Metadata.Add(new MetadatumDefinitionParameterValueCollection(identityManager.ObtainTypeReference(typeof(DebuggerDisplayAttribute))) { string.Format("{{{0}.{1}.{2}(this),nq}}", extensionsClass.NamespaceName, extensionsClass.Name, symbolDebuggerDisplay.Name) });
            compiler.FixedTokenBaseBuilder.LanguageFixedToken.Metadata.Add(new MetadatumDefinitionParameterValueCollection(identityManager.ObtainTypeReference(typeof(DebuggerDisplayAttribute))) { string.Format("{{{0}.{1}.{2}(Identity),nq}}", extensionsClass.NamespaceName, extensionsClass.Name, symbolDebuggerDisplay.Name) });
            compiler.VariableTokenBaseBuilder.LanguageVariableToken.Metadata.Add(new MetadatumDefinitionParameterValueCollection(identityManager.ObtainTypeReference(typeof(DebuggerDisplayAttribute))) { "{Value,nq}" });
            compiler.RootRuleBuilder.LanguageRuleRoot.Metadata.Add(new MetadatumDefinitionParameterValueCollection(identityManager.ObtainTypeReference(typeof(DebuggerDisplayAttribute))) { "{Context.Identity,nq}: {Context}" });
        }
        private void BuildGetValidSyntaxCore(ParserCompiler compiler, ParserBuilder parserBuilder, IIntermediateCliManager identityManager)
        {
            var targetClassPartial = parserBuilder.ParserClass.Parts.Add();
            this._getValidSyntaxMethodImpl = targetClassPartial.Methods.Add(new TypedName("GetValidSyntax", compiler.LexicalSymbolModel.ValidSymbols));
            this._getValidSyntaxMethod = parserBuilder.ParserInterface.Methods.Add(new TypedName("GetValidSyntax", compiler.LexicalSymbolModel.ValidSymbols));
            this._getValidSyntaxMethodInternalImpl =
                targetClassPartial.Methods.Add(
                new TypedName("GetValidSyntaxInternal", compiler.LexicalSymbolModel.ValidSymbols),
                new TypedNameSeries(
                    new TypedName("state", RuntimeCoreType.Int32, identityManager),
                    new TypedName("ruleContext", compiler.RuleSymbolBuilder.ILanguageRuleSymbol), 
                    new TypedName("initialPass", RuntimeCoreType.Boolean, identityManager)));
        }
        private void BuildGetValidSyntax(ParserCompiler compiler, ParserBuilder parserBuilder, IIntermediateCliManager identityManager)
        {
            bool lexicallyAmbiguousModel = compiler._GrammarSymbols.AmbiguousSymbols.Count() > 0;
            var advanceAdapters =
                compiler
                    .AdvanceMachines.Values
                    .Select(k => k.All)
                    .DefaultIfEmpty().Aggregate(
                        (a, b) =>
                            a.Concat(b));
            if (advanceAdapters == null)
                advanceAdapters = new PredictionTreeDFAdapter[0];
            var adapters = compiler.FollowAdapters.Select(k => k.Value.AssociatedState).Concat(compiler.AllRuleAdapters.Select(k => k.Value.AssociatedState)).Concat(advanceAdapters.DefaultIfEmpty().Where(k => k != null).Select(k => k.AssociatedState)).Distinct().OrderBy(k => k.StateValue).ToArray();
            var distinctStateValues = adapters.Select(k => k.StateValue).Distinct().ToArray();
            var stateSymbolStoreEntry =
                (from state in adapters
                 /* Since the other adapters are state-machines derived from the projection of a given state of a rule, normal adapters (derived from non-expanded lookahead) must be aggregated */
                 join normalAdapter in compiler.AllRuleAdapters on state equals normalAdapter.Value.AssociatedState into normalVariantSet
                 from normalAdapter in normalVariantSet.DefaultIfEmpty()
                 let fc = (normalAdapter.Value == null || normalAdapter.Value.OutgoingTransitions.Count == 0 || normalAdapter.Value.AssociatedContext.Leaf.LookAhead.Count == 0) ? state.OutTransitions.FullCheck : normalAdapter.Value.AssociatedContext.Leaf.LookAhead.Keys.Aggregate(GrammarVocabulary.UnionAggregateDelegate)
                 group new { State = state, Grammar = fc, GrammarStore = compiler.LexicalSymbolModel.GenerateSymbolstoreVariation(fc) } by new { Grammar = fc, IsEdge = state.IsEdge }).ToDictionary(k => k.Key, v => v.ToArray());
            var stateParam = _getValidSyntaxMethodInternalImpl.Parameters["state"];
            var ruleContext = _getValidSyntaxMethodInternalImpl.Parameters["ruleContext"];
            var initialPass = _getValidSyntaxMethodInternalImpl.Parameters["initialPass"];
            if (!lexicallyAmbiguousModel)
            {
                _getValidSyntaxMethodInternalImpl.Parameters.Remove(initialPass);
                initialPass = null;
            }

            _getValidSyntaxMethodImpl.AccessLevel = AccessLevelModifiers.Public;

            var getSyntaxMethodImplInvocation = _getValidSyntaxMethodInternalImpl.GetReference().Invoke(parserBuilder._StateImpl.GetReference(), parserBuilder._CurrentContextImpl.GetReference());
            _getValidSyntaxMethodImpl.Return(getSyntaxMethodImplInvocation);
            if (lexicallyAmbiguousModel)
                getSyntaxMethodImplInvocation.Arguments.Add(IntermediateGateway.TrueValue);
            ITypedLocalMember pushAmbiguityContext = null;
            if (lexicallyAmbiguousModel)
                pushAmbiguityContext = _getValidSyntaxMethodInternalImpl.Locals.Add(
                    new TypedName("pushAmbiguityContext", RuntimeCoreType.Boolean, identityManager),
                    IntermediateGateway.FalseValue);

            ITypedLocalMember validResult = null;
            if (lexicallyAmbiguousModel)
                validResult = _getValidSyntaxMethodInternalImpl.Locals.Add(
                    new TypedName("result", compiler.LexicalSymbolModel.ValidSymbols),
                    this._getValidSyntaxMethodInternalImpl.ReturnType.GetNewExpression());

            var switchStatement = this._getValidSyntaxMethodInternalImpl.Switch(stateParam.GetReference());
            foreach (var uniqueGrammarSet in stateSymbolStoreEntry.Keys)
            {
                var currentSet = stateSymbolStoreEntry[uniqueGrammarSet];
                IExpression[] stateIndices = new IExpression[currentSet.Length];
                currentSet = currentSet.OrderBy(k => k.State.StateValue).ToArray();
                for (int stateIndex = 0; stateIndex < currentSet.Length; stateIndex++)
                    stateIndices[stateIndex] = currentSet[stateIndex].State.StateValue.ToPrimitive();

                var currentCase = switchStatement.Case(stateIndices);
                currentCase.Comment(currentSet[0].Grammar.ToString());
                if (uniqueGrammarSet.IsEdge)
                {
                    //Inject logic to look up in the stack.
                    if (lexicallyAmbiguousModel)
                    {
                        var nullCheck = currentCase.If(ruleContext.InequalTo(IntermediateGateway.NullValue));
                        nullCheck.Assign(validResult.GetReference(), currentSet[0].GrammarStore.GetReference().BitwiseOr(_getValidSyntaxMethodInternalImpl.GetReference().Invoke(compiler.RuleSymbolBuilder.FollowState.GetReference(ruleContext.GetReference()), compiler.RuleSymbolBuilder.Parent.GetReference(ruleContext.GetReference()), IntermediateGateway.FalseValue)));
                        nullCheck.If(initialPass.GetReference())
                            .Assign(pushAmbiguityContext.GetReference(), IntermediateGateway.TrueValue);
                        nullCheck.CreateNext();
                        nullCheck.Next.Assign(validResult.GetReference(), currentSet[0].GrammarStore.GetReference());
                    }
                    else
                    {
                        var nullCheck = currentCase.If(ruleContext.InequalTo(IntermediateGateway.NullValue));
                        nullCheck.Return(currentSet[0].GrammarStore.GetReference().BitwiseOr(_getValidSyntaxMethodInternalImpl.GetReference().Invoke(compiler.RuleSymbolBuilder.FollowState.GetReference(ruleContext.GetReference()), compiler.RuleSymbolBuilder.Parent.GetReference(ruleContext.GetReference()))));
                        nullCheck.CreateNext();
                        nullCheck.Next.Return(currentSet[0].GrammarStore.GetReference());
                    }
                }
                else if (lexicallyAmbiguousModel)
                    currentCase.Assign(validResult.GetReference(), currentSet[0].GrammarStore.GetReference());
                else
                    currentCase.Return(currentSet[0].GrammarStore.GetReference());
            }
            if (lexicallyAmbiguousModel)
                this._getValidSyntaxMethodInternalImpl.If(pushAmbiguityContext.GetReference())
                    .Assign(validResult.GetReference(), AssignmentOperation.BitwiseOrAssign,
                        _cullAmbiguities.GetReference().Invoke(validResult.GetReference()));
            if (lexicallyAmbiguousModel)
                this._getValidSyntaxMethodInternalImpl.Return(validResult.GetReference());
            else
                this._getValidSyntaxMethodInternalImpl.Return(this._getValidSyntaxMethodInternalImpl.ReturnType.GetNewExpression());
            this._getValidSyntaxMethodInternalImpl.AccessLevel = AccessLevelModifiers.Private;
        }

        public IIntermediateInterfaceMethodMember GetValidSyntaxMethod { get { return this._getValidSyntaxMethod; } }
        public IIntermediateClassMethodMember GetValidSyntaxMethodInternalImpl { get { return this._getValidSyntaxMethodInternalImpl; } }
        public IIntermediateClassMethodMember GetValidSyntaxMethodImpl { get { return this._getValidSyntaxMethodImpl; } }



        public IIntermediateClassType ExtensionsClass { get; set; }

        public IIntermediateClassMethodMember SymbolDebuggerDisplay { get; set; }
    }
}
