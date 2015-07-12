using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Cli;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Utilities;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Utilities.Collections;
using System.Diagnostics;
using System.Globalization;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Ast.Cli;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class VariableTokenBaseBuilder
    {
        private ParserCompiler _compiler;
        private IIntermediateAssembly _assembly;
        private IIntermediateCliManager _identityManager;
        public void Build(ParserCompiler compiler, IIntermediateAssembly assembly)
        {
            this._compiler = compiler;
            this._assembly = assembly;
            this._identityManager = ((IIntermediateCliManager)(this._assembly.IdentityManager));
            this.BuildVariableTokenInterface();
            this.BuildVariableTokenClass();
        }

        private void BuildVariableTokenInterface()
        {
            this.ILanguageVariableToken = this._assembly.DefaultNamespace.Namespaces[string.Format("{0}.Cst", this._assembly.DefaultNamespace.FullName)].Parts.Add().Interfaces.Add("I{0}VariableToken", this._compiler.Source.Options.AssemblyName);
            this.ILanguageVariableToken.ImplementedInterfaces.Add(this._compiler.TokenSymbolBuilder.ILanguageToken);
            this.ILanguageVariableToken.ImplementedInterfaces.Add(this._compiler.CharStreamSegmentBuilder.ICharStreamSegment);
            this.ILanguageVariableToken.AccessLevel = AccessLevelModifiers.Public;
            this.BuildLookAhead();
        }

        private void BuildVariableTokenClass()
        {
            this.LanguageVariableToken = this._assembly.DefaultNamespace.Namespaces[string.Format("{0}.Cst", this._assembly.DefaultNamespace.FullName)].Parts.Add().Classes.Add("{0}VariableToken", this._compiler.Source.Options.AssemblyName);
            this.LanguageVariableToken.ImplementedInterfaces.ImplementInterfaceQuick(ILanguageVariableToken);
            this.LanguageVariableToken.BaseType = this._compiler.CharStreamSegmentBuilder.CharStreamSegment;
            this.LanguageVariableToken.AccessLevel = AccessLevelModifiers.Public;
            this.BuildLookAheadImpl();
            this.BuildTokenCountImpl();
            this.BuildIdentityImpl();
            this.Build_StartTokenIndexImpl();
            this.BuildStartTokenIndexImpl();
            this.BuildClassCtor();
        }

        private void BuildClassCtor()
        {
            //IJsonReaderCharStream owner, int start, int end, JsonReaderSymbols identity
            var ctor = this.LanguageVariableToken.Constructors.Add(
                new TypedName("owner", this._compiler.CharStreamBuilder.ICharStream),
                new TypedName("start", RuntimeCoreType.Int32, this._identityManager),
                new TypedName("end", RuntimeCoreType.Int32, this._identityManager),
                new TypedName("identity", this._compiler.SymbolStoreBuilder.Identities));
            ctor.CascadeTarget = ConstructorCascadeTarget.Base;
            //: base(owner, start, end)
            ctor.CascadeMembers.Add(ctor.Parameters["owner"].GetReference());
            ctor.CascadeMembers.Add(ctor.Parameters["start"].GetReference());
            ctor.CascadeMembers.Add(ctor.Parameters["end"].GetReference());
            ctor.Assign(this._IdentityImpl.GetReference(), ctor.Parameters["identity"].GetReference());
            ctor.AccessLevel = AccessLevelModifiers.Public;
        }


        private void Build_StartTokenIndexImpl()
        {
            var startTokenIndexImpl = this.LanguageVariableToken.Fields.Add(new TypedName("_startTokenIndex", RuntimeCoreType.Int32, this._identityManager));
            startTokenIndexImpl.AccessLevel = AccessLevelModifiers.Private;
            this._StartTokenImpl = startTokenIndexImpl;
        }

        private void BuildStartTokenIndexImpl()
        {
            var startTokenImpl = this.LanguageVariableToken.Properties.Add(new TypedName("StartTokenIndex", RuntimeCoreType.Int32, this._identityManager), true, true);
            startTokenImpl.AccessLevel = AccessLevelModifiers.Public;
            startTokenImpl.GetMethod.Return(this._StartTokenImpl.GetReference());
            startTokenImpl.SetMethod.Assign(this._StartTokenImpl.GetReference(), startTokenImpl.SetMethod.ValueParameter.GetReference());
            this.StartTokenImpl = startTokenImpl;
        }
        private void BuildLookAheadImpl()
        {
            this.LookAheadImpl = this.LanguageVariableToken.Methods.Add(new TypedName("LookAhead", RuntimeCoreType.Int32, this._identityManager), new TypedNameSeries(new TypedName("distance", RuntimeCoreType.Int32, this._identityManager)));
            var distanceParam = this.LookAheadImpl.Parameters["distance"];
            var rangeCheck = this.LookAheadImpl.If(distanceParam.LessThan(0).LogicalOr(distanceParam.GreaterThanOrEqualTo(this._compiler.CharStreamSegmentBuilder.LengthImpl.GetReference())));
            rangeCheck.Return((-1).ToPrimitive());
            this.LookAheadImpl.Return(this._compiler.CharStreamBuilder.CharIndexerImpl.GetReference(this._compiler.CharStreamSegmentBuilder.OwnerImpl.GetReference(), this._compiler.CharStreamSegmentBuilder.StartPosition.Add(distanceParam)));
            this.LookAheadImpl.AccessLevel = AccessLevelModifiers.Public;

        }

        private void BuildLookAhead()
        {
            this.LookAhead = this.ILanguageVariableToken.Methods.Add(new TypedName("LookAhead", RuntimeCoreType.Int32, this._identityManager), new TypedNameSeries(new TypedName("distance", RuntimeCoreType.Int32, this._identityManager)));
        }

        private void BuildTokenCountImpl()
        {
            var tokenCountImpl = this.LanguageVariableToken.Properties.Add(new TypedName("TokenCount", RuntimeCoreType.Int32, this._identityManager), true, false);
            tokenCountImpl.AccessLevel = AccessLevelModifiers.Public;
            tokenCountImpl.GetMethod.Return(1.ToPrimitive());
            this.TokenCountImpl = tokenCountImpl;
        }
        public IIntermediateInterfaceType ILanguageVariableToken { get; set; }

        public IIntermediateClassType LanguageVariableToken { get; set; }

        public IIntermediateClassMethodMember LookAheadImpl { get; set; }

        public IIntermediateInterfaceMethodMember LookAhead { get; set; }

        private void BuildIdentityImpl()
        {
            var _identityImpl = this.LanguageVariableToken.Fields.Add(new TypedName("_identity", this._compiler.SymbolStoreBuilder.Identities));
            var identityImpl = this.LanguageVariableToken.Properties.Add(new TypedName("Identity", this._compiler.SymbolStoreBuilder.Identities), true, false);
            identityImpl.GetMethod.Return(_identityImpl.GetReference());
            this.IdentityImpl = identityImpl;
            this._IdentityImpl = _identityImpl;
            this._IdentityImpl.AccessLevel = AccessLevelModifiers.Private;
            this.IdentityImpl.AccessLevel = AccessLevelModifiers.Public;
        }

        public IIntermediateClassPropertyMember TokenCountImpl { get; set; }

        public IIntermediateClassPropertyMember IdentityImpl { get; set; }

        public IIntermediateClassFieldMember _IdentityImpl { get; set; }

        public IIntermediateClassPropertyMember StartTokenImpl { get; set; }

        public IIntermediateClassFieldMember _StartTokenImpl { get; set; }
    }
    public class FixedTokenBaseBuilder
    {
        private ParserCompiler _compiler;
        private IIntermediateAssembly _assembly;
        private IIntermediateCliManager _identityManager;

        public void Build(ParserCompiler compiler, IIntermediateAssembly assembly)
        {
            this._compiler = compiler;
            this._assembly = assembly;
            this._identityManager = ((IIntermediateCliManager)(this._assembly.IdentityManager));
            this.BuildFixedTokenClass();
        }
        private void BuildFixedTokenClass()
        {
            /* *
             * To pay homage to the hand-written version I used to boot-strap this phase, I've decided to leave
             * the Fixed and Variable tokens in the same file.
             * */
            this.LanguageFixedToken = this._assembly.DefaultNamespace.Namespaces[string.Format("{0}.Cst", this._assembly.DefaultNamespace.FullName)].Parts.Add().Classes.Add("{0}FixedToken", this._compiler.Source.Options.AssemblyName);
            this.LanguageFixedToken.ImplementedInterfaces.ImplementInterfaceQuick(this._compiler.TokenSymbolBuilder.ILanguageToken);

            LanguageFixedToken.AccessLevel = AccessLevelModifiers.Public;
            this.BuildStartPositionImpl();
            this.BuildIdentityImpl();
            this.BuildClassCtor();
            this.BuildLengthImpl();
            this.BuildEndPositionImpl();
            this.BuildTokenCountImpl();
            this.Build_StartTokenIndexImpl();
            this.BuildStartTokenIndexImpl();
        }

        private void Build_StartTokenIndexImpl()
        {
            var startTokenIndexImpl = this.LanguageFixedToken.Fields.Add(new TypedName("_startTokenIndex", RuntimeCoreType.Int32, this._identityManager));
            startTokenIndexImpl.AccessLevel = AccessLevelModifiers.Private;
            this._StartTokenImpl = startTokenIndexImpl;
        }

        private void BuildStartTokenIndexImpl()
        {
            var startTokenImpl = this.LanguageFixedToken.Properties.Add(new TypedName("StartTokenIndex", RuntimeCoreType.Int32, this._identityManager), true, true);
            startTokenImpl.AccessLevel = AccessLevelModifiers.Public;
            startTokenImpl.GetMethod.Return(this._StartTokenImpl.GetReference());
            startTokenImpl.SetMethod.Assign(this._StartTokenImpl.GetReference(), startTokenImpl.SetMethod.ValueParameter.GetReference());
            this.StartTokenImpl = startTokenImpl;
        }

        private void BuildTokenCountImpl()
        {
            var tokenCountImpl = this.LanguageFixedToken.Properties.Add(new TypedName("TokenCount", RuntimeCoreType.Int32, this._identityManager), true, false);
            tokenCountImpl.AccessLevel = AccessLevelModifiers.Public;
            tokenCountImpl.GetMethod.Return(1.ToPrimitive());
            this.TokenCountImpl = tokenCountImpl;
        }

        private void BuildEndPositionImpl()
        {
            var endPositionImpl = this.LanguageFixedToken.Properties.Add(new TypedName("EndPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
            endPositionImpl.GetMethod.Return(this.StartPositionImpl.Add(this.LengthImpl));
            this.EndPositionImpl = endPositionImpl;
            this.EndPositionImpl.AccessLevel = AccessLevelModifiers.Public;
        }

        private void BuildLengthImpl()
        {
            var lengthImpl = this.LanguageFixedToken.Properties.Add(new TypedName("Length", RuntimeCoreType.Int32, this._identityManager), true, false);
            var identitySwitch = lengthImpl.GetMethod.Switch(this.IdentityImpl.GetReference());
            var constantEntries = (from s in this._compiler._GrammarSymbols.SymbolsOfType<IGrammarConstantEntrySymbol>()
                                   where !(s.Source is IOilexerGrammarTokenEofEntry)
                                   let sItem = s.Source.Branches[0][0] as ILiteralTokenItem
                                   where sItem != null
                                   select new { LiteralItem = sItem, Symbol = (IGrammarSymbol)s }).ToArray();

            var constantItems =
                (from s in this._compiler._GrammarSymbols.SymbolsOfType<IGrammarConstantItemSymbol>()
                 let item = s.SourceItem
                 select new { LiteralItem = item, Symbol = (IGrammarSymbol)s }).ToArray();


            /* Ambiguities that are ambiguous with a variable token are still of known length due to matching the known-length element. */
            var ambiguousItems =
                (from s in this._compiler._GrammarSymbols.SymbolsOfType<IGrammarAmbiguousSymbol>()
                 let literalConstantEntry = (IGrammarConstantEntrySymbol)s.FirstOrDefault(aSym => aSym is IGrammarConstantEntrySymbol && constantEntries.Any(ce => ce.Symbol == aSym))
                 let literalConstantItem = (IGrammarConstantItemSymbol)s.FirstOrDefault(aSym => aSym is IGrammarConstantItemSymbol && constantItems.Any(ci => ci.Symbol == aSym))
                 where literalConstantEntry != null || literalConstantItem != null
                 let literalItem = literalConstantEntry != null ? literalConstantEntry.Source.Branches[0][0] as ILiteralTokenItem : literalConstantItem.SourceItem
                 select new { LiteralItem = literalItem, Symbol = (IGrammarSymbol)s }).ToArray();

            var aggregatedItems = constantEntries.Concat(constantItems).Concat(ambiguousItems);

            var itemsByLength = (from literalAndSymbol in aggregatedItems
                                 let value = literalAndSymbol.LiteralItem.Value.ToString()
                                 let identityField = this._compiler.LexicalSymbolModel.GetIdentitySymbolField(literalAndSymbol.Symbol)
                                 group new { literalAndSymbol.LiteralItem, literalAndSymbol.Symbol, Identity = identityField } by value.Length).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToArray());

            foreach (var length in itemsByLength.Keys)
            {
                List<IExpression> caseElements = new List<IExpression>();
                foreach (var identity in itemsByLength[length].OrderBy(k => k.Identity.Name))
                    caseElements.Add(identity.Identity.GetReference());
                var currentCaseSet = identitySwitch.Case(caseElements.ToArray());
                currentCaseSet.Return(length.ToPrimitive());
            }
            identitySwitch.Case(true).Return(IntermediateGateway.NumberZero);
            this.LengthImpl = lengthImpl;
            this.LengthImpl.AccessLevel = AccessLevelModifiers.Public;
        }

        private void BuildIdentityImpl()
        {
            var _identityImpl = this.LanguageFixedToken.Fields.Add(new TypedName("_identity", this._compiler.SymbolStoreBuilder.Identities));
            var identityImpl = this.LanguageFixedToken.Properties.Add(new TypedName("Identity", this._compiler.SymbolStoreBuilder.Identities), true, false);
            identityImpl.GetMethod.Return(_identityImpl.GetReference());
            this.IdentityImpl = identityImpl;
            this._IdentityImpl = _identityImpl;
            this._IdentityImpl.AccessLevel = AccessLevelModifiers.Private;
            this.IdentityImpl.AccessLevel = AccessLevelModifiers.Public;
        }

        private void BuildStartPositionImpl()
        {
            var _startPositionImpl = this.LanguageFixedToken.Fields.Add(new TypedName("_startPosition", RuntimeCoreType.Int32, this._identityManager));
            var startPositionImpl = this.LanguageFixedToken.Properties.Add(new TypedName("StartPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
            startPositionImpl.GetMethod.Return(_startPositionImpl.GetReference());
            this.StartPositionImpl = startPositionImpl;
            this._StartPositionImpl = _startPositionImpl;
            this._StartPositionImpl.AccessLevel = AccessLevelModifiers.Private;
            this.StartPositionImpl.AccessLevel = AccessLevelModifiers.Public;


        }

        private void BuildClassCtor()
        {
            var rootCtor = this.LanguageFixedToken.Constructors.Add(
                new TypedName("identity", this._compiler.SymbolStoreBuilder.Identities),
                new TypedName("startPosition", RuntimeCoreType.Int32, this._identityManager));
            rootCtor.AccessLevel = AccessLevelModifiers.Public;
            rootCtor
                .Assign(this._IdentityImpl.GetReference(), rootCtor.Parameters["identity"].GetReference()).FollowedBy()
                .Assign(this._StartPositionImpl.GetReference(), rootCtor.Parameters["startPosition"].GetReference());

        }

        public IIntermediateClassType LanguageFixedToken { get; set; }

        public IIntermediateClassPropertyMember IdentityImpl { get; set; }

        public IIntermediateClassFieldMember _IdentityImpl { get; set; }

        public IIntermediateClassPropertyMember StartPositionImpl { get; set; }

        public IIntermediateClassFieldMember _StartPositionImpl { get; set; }

        public IIntermediateClassPropertyMember LengthImpl { get; set; }

        public IIntermediateClassPropertyMember EndPositionImpl { get; set; }

        public IIntermediateClassPropertyMember TokenCountImpl { get; set; }

        public IIntermediateClassFieldMember _StartTokenImpl { get; set; }

        public IIntermediateClassPropertyMember StartTokenImpl { get; set; }
    }
}
